<!--
    Tags: aspnet-core logging architecture mvc
-->

# Tips and tricks for ASP.NET Core applications

This is a small collection of some tips and tricks which I keep repeating myself in every ASP.NET Core application. There's nothing ground breaking in this list, but some general advice and minor tricks which I have picked up over the course of several real world applications.

## Logging

Let's begin with logging. There are many logging frameworks available for .NET Core, but my absolute favourite is [Serilog](https://serilog.net/) which offers a very nice structured logging interface for a [vast number of available storage providers](https://github.com/serilog/serilog/wiki/Provided-Sinks) (sinks).

### Tip 1: Configure logging before anything else

The logger should be the very first thing configured in an ASP.NET Core application. Everything else should be wrapped in a try-catch block:

```
public class Program
{
    public static int Main(string[] args) => StartWebServer(args);

    public static int StartWebServer(string[] args)
    {
        Log.Logger =
            new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.WithProperty("Application", "MyApplicationName")
                .WriteTo.Console()
                .CreateLogger();

        try
        {
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseKestrel(k => k.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
            return -1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
```

### Tip 2: Flush the logger before the application terminates

Make sure to put `Log.CloseAndFlush();` into the `finally` block of your try-catch block so that no log data is getting lost when the application terminates before all logs have been written to the log stream.

### Tip 3: Enrich your log entries

Configure your logger to automatically decorate every log entry with an `Application` property which contains a unique identifier for your application (typically a human readable name which identifies your app):

```
.Enrich.WithProperty("Application", "MyApplicationName")
```

This is extremely useful if you write logs from more than one application into a single log stream (e.g. a single Elasticsearch database). Personally I prefer to write logs from multiple (smaller) services of a coherent system into a single logging database and filter logs by properties.

Appending an additional `Application` property to all your application logs has the advantage that one can easily filter and view the overall health of a single application as well as getting a holistic view of the entire system.

Other really useful information which could be appended to your log entries is the application version and the environment name:

```
Log.Logger =
    new LoggerConfiguration()
        .MinimumLevel.Warning()
        .Enrich.WithProperty("Application", "MyApplicationName")
        .Enrich.WithProperty("ApplicationVersion", "<version number>")
        .Enrich.WithProperty("EnvironmentName", "Staging")
        .WriteTo.Console()
        .CreateLogger();
```

This will allow one to better visualise if issues had been resolved (or appeared) after a certain version has been deployed and it will also make it very easy to filter out any logs which might have accidentally been written from a different environment (e.g. a developer was debugging locally with the production connection string in their settings).

## Startup Configuration

In ASP.NET Core there are two main places where features and functionality get configured. First there is the `Configure` method which can be used to plug middleware into the ASP.NET Core pipeline and secondly there is the `ConfigureServices` method to register dependencies.

For example adding [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to ASP.NET Core would look a bit like this:

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    services.AddSwaggerGen(
        c =>
        {
            var name = "<my app name>"
            var version = "v1";

            c.SwaggerDoc(
                version,
                new Info { Version = version, Title = name });

            c.DescribeAllEnumsAsStrings();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app .UseSwagger()
        .UseSwaggerUI(
            c =>
            {
                var name = "<my app name>"
                var version = "v1";
                c.RoutePrefix = "";
                c.SwaggerEndpoint(
                    "/swagger/{version}/swagger.json", name);
            }
        )
        .UseMvc();
}
```

Middleware and dependencies are obviously two different things and therefore their configuration is split into two different methods, but from a developer's point of view it is very annoying that most features are configured across more than just one place.

### Tip 4: Create 'Config' classes

One nice way to combat this is by creating a `Config` folder in the root of your ASP.NET Core application and create `<FeatureName>Config` classes for each feature/functionality which needs to be registered in `Startup`:

```
public static class SwaggerConfig
{
    private static string Name => "My Cool API";
    private static string Version => "v1";
    private static string Endpoint => $"/swagger/{Version}/swagger.json";
    private static string UIEndpoint => "";

    public static void SwaggerUIConfig(SwaggerUIOptions config)
    {
        config.RoutePrefix = UIEndpoint;
        config.SwaggerEndpoint(Endpoint, Name);
    }

    public static void SwaggerGenConfig(SwaggerGenOptions config)
    {
        config.SwaggerDoc(
            Version,
            new Info { Version = Version, Title = Name });

        config.DescribeAllEnumsAsStrings();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        config.IncludeXmlComments(xmlPath);
    }
}
```

By doing this one can move all related configuration of a feature into a single place and also nicely distinguish between the individual configuration steps (e.g. `SwaggerUIConfig` vs `SwaggerGenConfig`).

Afterwards one can tidy up the `Startup` class by invoking the respective class methods:

```
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddMvc(MvcConfig.AddFilters)
        .AddJsonOptions(MvcConfig.JsonOptions);

    services.AddSwaggerGen(SwaggerConfig.SwaggerGenConfig);
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app .UseSwagger()
        .UseSwaggerUI(SwaggerConfig.SwaggerUIConfig)
        .UseMvc();
}
```

### Tip 5: Extension methods for conditional configurations

Another common use case is to configure different features based on the current environment or other conditional cases:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
}
```

A neat trick which I like to apply here is to implement an extension method for conditional configurations:

```
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder When(
        this IApplicationBuilder builder,
        bool predicate,
        Func<IApplicationBuilder> compose) => predicate ? compose() : builder;
}
```

The `When` extension method will invoke a `compose` function only if a given `predicate` is true.

Now with the `When` method someone can set up conditional middleware in a much nicer and fluent way:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app .When(env.IsDevelopment(), app.UseDeveloperExceptionPage)
        .When(!env.IsDevelopment(), app.UseHsts)
        .UseSwagger()
        .UseSwaggerUI(SwaggerConfig.SwaggerUIConfig)
        .UseMvc();
}
```

## Exit scenarios

### Tip 6: Don't forget to return a default 404 response

Don't forget to register a middleware which will return a `404 Not Found` HTTP response if no other middleware was able to deal with an incoming request:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app .When(env.IsDevelopment(), app.UseDeveloperExceptionPage)
        .When(!env.IsDevelopment(), app.UseHsts)
        .UseSwagger()
        .UseSwaggerUI(SwaggerConfig.SwaggerUIConfig)
        .UseMvc()
        .Run(NotFoundHandler);
}

private readonly RequestDelegate NotFoundHandler =
    async ctx =>
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.WriteAsync("Page not found.");
    };
```

If you don't do this then a request which couldn't be matched by any middleware will be left unhandled (unless you have another web server sitting behind Kestrel).

### Tip 7: Return non zero exit code on failure

Return a non zero exit code when the application terminates with an error. This will allow parent processes to pick up the fact that the application terminated unexpectedly and give them a chance to handle such a situation more gracefully (e.g. when your ASP.NET Core application is run from a Kubernetes cluster):

```
try
{
    // Start WebHost

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
    return -1;
}
finally
{
    Log.CloseAndFlush();
}
```

## Error Handling

Every ASP.NET Core application is likely going to have to deal with at least three types of errors:

- Server errors
- Client errors
- Business logic errors

Server errors are unexpected exceptions which get thrown by an application. [Normally these exceptions bubble up to a global error handler](https://dusted.codes/error-handling-in-aspnet-core) which will log the exception and return a `500 Internal Server Error` response to the client.

Client errors are mistakes which a client can make when sending a request to the server. These normally include things like missing or wrong authentication data, badly formatted request bodies, calling endpoints which do not exist or perhaps sending data in an unsupported format. Most of these errors will get picked up by a built-in ASP.NET Core feature which will return a corresponding `4xx` HTTP error back to the client.

Business logic errors are application specific errors which are not handled by ASP.NET Core by default because they are very unique to each individual application. For example an invoicing application might want to throw an exception when a customer tries to raise an invoice with an unsupported currency whereas an online gaming application might want to throw an error when a user ran out of credits.

These errors are often raised from lower level domain code and might want to return a specific `4xx` or `5xx` HTTP response back to the client.

### Tip 8: Create a base exception type for domain errors

Create a base exception class for business or domain errors and additional exception classes which derive from the base class for all possible error cases:

```
public enum DomainErrorCode
{
    InsufficientCredits = 1000
}

public class DomainException : Exception
{
    public readonly DomainErrorCode ErrorCode;

    public DomainException(DomainErrorCode code, string message) : base(message)
    {
        ErrorCode = code;
    }
}

public class InsufficientCreditsException : DomainException
{
    public InsufficientCreditsException()
        : base(DomainErrorCode.InsufficientCredits,
                "User ran out of free credit. Please upgrade your plan to continue using our service.")
    { }
}
```

Include a unique `DomainErrorCode` for each custom exception type which later can be used to identify the specific error case from higher level code.

Afterwards one can use the newly created exception classes to throw more meaningful errors from inside the domain layer:

```
throw new InsufficientCreditsException();
```

This has now the benefit that the ASP.NET Core application can look for domain exceptions from a central point (e.g. custom error middleware) and handle them accordingly:

```
public class DomainErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public DomainErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch(DomainException ex)
        {
            ctx.Response.StatusCode = 422;
            await ctx.Response.WriteAsync($"{ex.ErrorCode}: {ex.Message}");
        }
    }
}
```

Because every domain exception includes a unique `DomainErrorCode` the generic error handler can even implement a slightly different response based on the given domain error.

This architecture has a few benefits:

- The domain layer can throw meaningful exceptions
- The domain layer works nicely with the higher level web layer without tight coupling
- Domain exceptions are clearly distinguishable from other errors
- Domain exceptions are self documenting
- The web layer can handle all domain errors in a unified way without having to replicate the same try-catch block across multiple controllers
- The additional error code in the response can be parsed and understood by third party clients
- The custom exception types can be easily documented through Swagger

### Tip 9: Expose an endpoint which returns all error codes

When you followed tip 8 and implemented a custom exception type with a unique error code for each error case then it can be extremely handy to expose all possible error codes through a single API endpoint. This will allow third party clients to quickly retrieve a list of the latest possible error codes and their meaning:

```
[HttpGet("/error-codes")]
public ActionResult<IDictionary<int, string>> ErrorCodes()
{
    var values = Enum
        .GetValues(typeof(DomainErrorCode))
        .Cast<DomainErrorCode>();

    var result = new Dictionary<int, string>();

    foreach(var v in values)
        result.Add((int)v, v.ToString());

    return result;
}
```

## Other Tips &amp; Tricks

### Tip 10: Expose a version endpoint

Another really useful thing to have in an API (or website) is a version endpoint. Often it can be extremely helpful to customer support staff, QA or other members of a team to quickly be able to establish what version of an application is being deployed to an environment.

This version is different than the customer facing API version which often only includes the major version number (e.g. https://my-api.com/v3/some/resource).

Exposing an endpoint which displays the current application version and the build date and time is a nice way of quickly making this information accessible to relevant people:

```
[HttpGet("/info")]
public ActionResult<string> Info()
{
    var assembly = typeof(Startup).Assembly;

    var creationDate = File.GetCreationTime(assembly.Location);
    var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

    return Ok($"Version: {version}, Last Updated: {creationDate}");
}
```

### Tip 11: Remove the 'Server' HTTP header

Whilst one is at configuring their ASP.NET Core application they might as well remove the `Server` HTTP header from every HTTP response by deactivating that setting in Kestrel:

```
.UseKestrel(k => k.AddServerHeader = false)
```

### Tip 12: Working with Null Collections

My last tip on this list is not specific to ASP.NET Core but all of .NET Core development where a collection or `IEnumerable` type is being used.

How often do .NET developers write something like this:

```
var someCollection = GetSomeCollectionFromSomewhere();

if (someCollection != null && someCollection.Count > 0)
{
    foreach(var item in someCollection)
    {
        // Do stuff
    }
}
```

Adding a one line extension method can massively simplify the above code across an entire applicatoin:

```
public static class EnumerableExtensions
{
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source) =>
        source ?? Enumerable.Empty<T>();
}
```

Now the above `if` statement can be reduced to a single loop like this:

```
var someCollection = GetSomeCollectionFromSomewhere();

foreach(var item in someCollection.OrEmptyIfNull())
{
    // Do stuff
}
```

Or converting the `IEnumerbale` to an `IList` and use the `ForEach` LINQ extension method to turn this into a one liner:

```
someCollection.OrEmptyIfNull().ToList().ForEach(i => i.DoSomething());
```

## What tips and tricks do you have?

So this is it, this was my brief post on some tips and tricks which I like to apply in my personal ASP.NET Core development. I hope this was at least somewhat useful to someone?! Let me know what you think and please feel free to share your own tips and tricks which make your ASP.NET Core development life easier in the comments below!