<!--
    Tags: aspnet-core firewall cloudflare security
-->

# ASP.NET Core Firewall

About a month ago I experienced an issue with one of my online services which is running in the Google Cloud and also protected by [Cloudflare](https://www.cloudflare.com/). I had noticed a spike in traffic which only showed up in my Google Cloud dashboard but not in Cloudflare. It was odd because all requests should normally route through Cloudflare's proxy servers but it seemed like someone was circumventing the DNS resolution and hitting my service directly via its exposed IP address. It was a significant issue, because the endpoint which was being hit was quite expensive and I had specifically configured Cloudflare to rate limit a caller to a maximum of 100 requests per second. Unfortunately someone must have spoofed my service's IP address and managed to bypass Cloudflare and the configured rate limit and was able to issue thousands of requests per second which put a huge strain on my rather cheap infrastructure.

After a quick Google search I discovered that [bypassing Cloudflare](https://blog.christophetd.fr/bypassing-cloudflare-using-internet-wide-scan-data/) is not that difficult and actually quite [well documented on the internet](https://support.cloudflare.com/hc/en-us/articles/115003687931-Warning-about-exposing-your-origin-IP-address-via-DNS-records). To my rescue I also discovered that [Cloudflare publishes a list of all their IPv4 and IPv6 addresses](https://www.cloudflare.com/ips/) which web administrators (is that even still a thing?) can use to set up IP address filtering on their web services to specifically prevent scenarios like this. I needed a quick solution and therefore went on another internet search for an ASP.NET Core middleware which would block all incoming requests which did not originate from a known Cloudflare address. The closest I could find was an article on a [Client IP safelist](https://docs.microsoft.com/en-us/aspnet/core/security/ip-safelist?view=aspnetcore-2.1), but it didn't allow me to "safelist" an entire IP address range like the ones which Cloudflare has made public (e.g. `103.21.244.0/22`).

Knowing that I couldn't afford to run with this issue for much longer I decided to quickly hack my own IP address filtering middleware together. After a couple of hours of mad programming and copy pasting from Stackoverflow  I had a quick and dirty solution deployed to production. It wasn't perfect, but it worked. My initial hack was able to validate an incoming IP address against all of Cloudflare's published CIDR notations and either grant or deny access to the requested resource. I was really happy how well it worked and after my pressing issue had been solved I wanted to deploy the same solution to all of my other ASP.NET Core services too.

A week later I published a slightly more polished version of the middleware as a NuGet package called [Firewall](https://www.nuget.org/packages/Firewall/). Today I deployed another version with major architectural improvements which made [Firewall](https://github.com/dustinmoris/Firewall) a much more flexible and useful library to a wider range of applications. In the rest of this blog post I would like to demonstrate some of the features which Firewall can do for an ASP.NET Core application.

## How Firewall works

Firewall is an ASP.NET Core access control middleware. It primarily lets an application filter incoming requests based on their IP address and either grant or deny access. IP address filtering can be configured through a list of specific IP addresses and/or a list of CIDR notations:

```
using Firewall;

namespace BasicApp
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var allowedIPs =
                new List<IPAddress>
                    {
                        IPAddress.Parse("10.20.30.40"),
                        IPAddress.Parse("1.2.3.4"),
                        IPAddress.Parse("5.6.7.8")
                    };

            var allowedCIDRs =
                new List<CIDRNotation>
                    {
                        CIDRNotation.Parse("110.40.88.12/28"),
                        CIDRNotation.Parse("88.77.99.11/8")
                    };

            app.UseFirewall(
                FirewallRulesEngine
                    .DenyAllAccess()
                    .ExceptFromIPAddressRanges(allowedCIDRs)
                    .ExceptFromIPAddresses(allowedIPs));

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
```

The main feature can be enabled through the `UseFirewall()` extension method, which registers the `FirewallMiddleware` in the ASP.NET Core pipeline.

Rules for the Firewall are configured through the so called `FirewallRulesEngine`. The [Firewall NuGet package](https://www.nuget.org/packages/Firewall/) comes with a set of default rules which are ready to use. For example the `ExceptFromCloudflare()` extension method will automatically configure the Firewall to retrieve the latest version of all of Cloudflare's IPv4 and IPv6 address ranges and subsequently validate incoming requests against them:

```
app.UseFirewall(
    FirewallRulesEngine
        .DenyAllAccess()
        .ExceptFromCloudflare());
```

A list of all currently available rules can be found on the [project's documentation page](https://github.com/dustinmoris/Firewall/blob/master/README.md).

Rules can be chained in the reverse order in which they will get evaluated against an incoming HTTP request:

```
var adminIPAddresses = new [] { IPAddress.Parse("1.2.3.4) };

app.UseFirewall(
    FirewallRulesEngine
        .DenyAllAccess()
        .ExceptFromCloudflare()
        .ExceptFromIPAddresses(adminIPAddresses)
        .ExceptFromLocalhost());
```

In the example above an incoming request will be first checked if it came from the same host, then if it came from the web administrator's home address and afterwards if it came from one of Cloudflare's IP addresses before the request will get denied. The request needs to satisfy only one of the rules in order to pass validation.

The reverse order of validation might seem a little bit weird at first, but it is simply explained by exposing the underlying architecture which is nothing more than a standard decorator composition pattern:

```
// Pseudo code:

var rules =
    new LocalhostRule(
        new IPAddressRule(
            new CloudflareRule(
                new DenyAllAccessRule())));
```

The `FirewallRulesEngine` is only syntactic sugar on top of the decorator pattern which allows a user to compose a set of rules without having to new up a bunch of classes and dependencies.

## Custom Rules

Custom rules can either be configured via the `ExceptWhen` extension method or by creating a new class which implements the `IFirewallRule` interface:

```
var adminIPAddresses = IPAddress.Parse("1.2.3.4);

app.UseFirewall(
    FirewallRulesEngine
        .DenyAllAccess()
        .ExceptFromCloudflare()
        .ExceptWhen(ctx => ctx.Connection.RemoteIpAddress == adminIPAddress));
```

More complex rules can be created by implementing `IFirewallRule`:

```
public class IPCountryRule : IFirewallRule
{
    private readonly IFirewallRule _nextRule;
    private readonly IList<string> _allowedCountryCodes;

    public IPCountryRule(
        IFirewallRule nextRule,
        IList<string> allowedCountryCodes)
    {
        _nextRule = nextRule;
        _allowedCountryCodes = allowedCountryCodes;
    }

    public bool IsAllowed(HttpContext context)
    {
        const string headerKey = "CF-IPCountry";

        if (!context.Request.Headers.ContainsKey(headerKey))
            return _nextRule.IsAllowed(context);

        var countryCode = context.Request.Headers[headerKey].ToString();
        var isAllowed = _allowedCountryCodes.Contains(countryCode);

        return isAllowed || _nextRule.IsAllowed(context);
    }
}
```

There's a [complete example of creating a custom rule](https://github.com/dustinmoris/Firewall/blob/master/README.md#custom-rules) available in the latest documentation.

## X-Forwarded-For HTTP Header

Firewall has more features like a [GeoIP2](https://dev.maxmind.com/geoip/geoip2/geolite2/) powered `CountryRule`, detailed diagnostics for debugging and examples of how to load rule settings from external configuration providers, but one more ASP.NET Core feature which I wanted to specifically highlight here is the `UseForwardedHeaders` middleware.

If an application sits behind more than one proxy server (e.g. Cloudflare + a custom load balancer) then you'll need to enable the `ForwardedHeader` middleware in order to retrieve the correct client IP address in the `HttpContext.Connection.RemoteIpAddress` property:

```
public void Configure(IApplicationBuilder app)
{
    app.UseForwardedHeaders(
        new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor,
            ForwardLimit = 1
        }
    );

    app.UseCloudflareFirewall();

    app.Run(async (context) =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
}
```

It is important to understand that this HTTP header is not guaranteed to be safe (as anything else which is client generated) and therefore it is not recommended to set the `ForwardedLimit` to a value greater than 1 unless the application is also set up with a list of trusted proxies (`KnownProxies` or `KnownNetworks`). If this is not done correctly then a malicious user could pretend to be a trusted source by setting the `X-Forwarded-For` header to a known (trusted) IP address.

If you think this short article was useful or if you've got your own ASP.NET Core website running behind Cloudflare then please go and check out the [Firewall](https://github.com/dustinmoris/Firewall) project and secure your application against unwanted traffic too.