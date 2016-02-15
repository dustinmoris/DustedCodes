<!--
    Published: 2016-01-12 08:15
    Author: Dustin Moris Gorski
    Title: ASP.NET 5 like configuration in regular .NET applications
    Tags: aspnet dotnet
-->
[ASP.NET 5](https://get.asp.net/) is Microsoft's latest web framework and the new big thing on the .NET landscape. It comes with a whole lot of [new features](https://github.com/aspnet/home/releases/v1.0.0-rc1-final) and other changes which makes it very distinctive from previous ASP.NET versions. It is basically a complete re-write of the framework, optimized for the cloud and cross platform compatible. If you haven't checked it out yet then you are definitely missing out!

Some of the newly introduced features are the new [configuration options](https://docs.asp.net/en/latest/fundamentals/configuration.html), which come as part of the [Microsoft.Extensions.Configuration](https://github.com/aspnet/Configuration) NuGet packages. They allow you a more flexible way of loading configuration values into an application and make it significantly easier to move an application across different environments without having to change configuration files or run through nasty configuration transformations as part of the process. [Scott Hanselman](http://www.hanselman.com/) has nicely summarized this topic in one of [his recent blog posts](http://www.hanselman.com/blog/BestPracticesForPrivateConfigDataAndConnectionStringsInConfigurationInASPNETAndAzure.aspx).

It is needless to say that a lot of people are hugely excited about the new framework and many projects are being written in ASP.NET 5 right now, but there is also a significant amount of people who cannot easily migrate their existing projects to the new framework yet. This might be for various reasons but essentially means they are stuck on an older version of ASP.NET at least for a while.

However, it doesn't mean that those people cannot already benefit from some of the new ideas which have been publicly introduced in ASP.NET 5 such as the new configuration options.

## Implementing ASP.NET 5 like configuration options

The idea is to provide multiple sources from where an app can load configuration values. Additionally we want to put those sources into an order of precedence, where one source overrules another.

While ASP.NET 5 implementes a [Builder](https://en.wikipedia.org/wiki/Builder_pattern) pattern, you can easily achieve the same thing with a good old [Decorator](https://en.wikipedia.org/wiki/Decorator_pattern).

First we need an interface which provides a method to retrieve a config value:

<pre><code>public interface IConfiguration
{
    string Get(string key);
}</code></pre>

Next I implement a simple class which loads a value from an app- or web.config file:

<pre><code>using System.Configuration;

public class AppConfigConfiguration : IConfiguration
{
    public string Get(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}
</code></pre>

This is trivial and probably the same as what you have in your current application, just that it has been wrapped in a class which is behind an interface.

Finally we can provide an additional implementation which loads a value from the environment variables:

<pre><code>using System;
    
public class EnvironmentVariablesConfiguration : IConfiguration
{
    private readonly IConfiguration _backupConfiguration;

    public EnvironmentVariablesConfiguration(
        IConfiguration backupConfiguration)
    {
        _backupConfiguration = backupConfiguration;
    }

    public string Get(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return value ?? _backupConfiguration.Get(key);
    }
}
</code></pre>

This particular implementation wraps another `IConfiguration` implementation. If the setting does not exist in the environment variables then it defers to the next implementation. This could be anything and in this particular example will be the `AppConfigConfiguration`:

<pre><code>container.Register&lt;IConfiguration&gt;(
    new EnvironmentVariablesConfiguration(
        new AppConfigConfiguration()));
</code></pre>

It is really as simple as that. The order of precedence is determined by the order of the classes being put together.

This pattern can be extended as far as you like. For example I could add two more classes and compose something like this:

<pre><code>container.Register&lt;IConfiguration&gt;(
    new EnvironmentVariablesConfiguration(
        new JsonFileConfiguration(
            new AzureTableStorageConfiguration(
                new AppConfigConfiguration()))));</code></pre>

Now I can access configuration values from other classes through an `IConfiguration` dependency:

<pre><code>var value = _configuration.Get("SomeKey");</code></pre>

With this trick you can easily implement a &quot;cloud optimized&quot; configuration in any version of ASP.NET and follow good practice patterns no matter where you code!