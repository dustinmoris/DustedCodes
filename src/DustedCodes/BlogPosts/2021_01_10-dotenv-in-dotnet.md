<!--
    Tags: dotnet csharp fsharp
-->

# Using .env in .NET

.NET (Core) comes with a lot of bells and whistles. One of them is the sheer amount of managing application secrets and settings. Developers have a variety of options from which they can load application settings using the `ConfigurationBuilder` class. The [official ASP.NET Core documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) lists all options as following:

- Settings files, such as appsettings.json
- Environment variables
- Azure Key Vault
- Azure App Configuration
- Command-line arguments
- Custom providers, installed or created
- Directory files
- In-memory .NET object

Additionally .NET developers can choose between strongly typed configuration classes, `IOptions<T>` wrappers, `IOptionsSnapshot<T>` wrappers or the `IOptionsMonitor<T>` interface to access their settings. Theoretically there are also the [`IOptionsChangeTokenSource<T>`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionschangetokensource-1?view=dotnet-plat-ext-5.0), [`IOptionsFactory<T>`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionsfactory-1?view=dotnet-plat-ext-5.0), [`IOptionsMonitorCache<T>`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.ioptionsmonitorcache-1?view=dotnet-plat-ext-5.0) interfaces and the [`OptionsManager<T>`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.options.optionsmanager-1?view=dotnet-plat-ext-5.0) class, but most users will never need to use them.

Most modern cloud based applications don't even require half of those features. If anything the [ASP.NET Core options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0) can feel a little bit bloated which can overcomplicate an application and make it more difficult to understand for people outside one's team. The much simpler and often entirely sufficient alternative are environment variables!

<div class="tip"><p><strong>Tip:</strong> If you would like to learn more about the different configuration implementations in ASP.NET Core then check out <a href="https://andrewlock.net/tag/configuration/" target="_blank">Andrew Lock's blog</a> where he wrote about <a href="https://andrewlock.net/using-multiple-instances-of-strongly-typed-settings-with-named-options-in-net-core-2-x/" target="_blank">several of the mentioned interfaces</a> and <a href="https://andrewlock.net/creating-singleton-named-options-with-ioptionsmonitor/" target="_blank">explained how and when to use them</a>.</p></div>

## Environment variables

In the cloud most settings are configured via environment variables. The ease of configuration, their wide spread support and the simplicity of environment variables makes them a very compelling option. Setting environment variables during development is a little bit more tricky though. It's not any harder than in the cloud, but it's significantly more inconvenient when someone wants to quickly add, remove or edit a variable. Additionally there is a risk of collision when working on multiple applications at the same time. Environment variables like `LOG_LEVEL`, `SECRET_KEY` or `WEB_PORT` are common enough to appear in more than one project. Having to constantly change those values when switching context can become tiresome. Luckily environment variables can be configured at different levels. They can be set on a machine level, user level or for a single process. The latter is the preferred solution during development. Dotenv (.env) files are a great way of making that easy!

## The .NET way

Before explaining &quot;Dotenv&quot; files let's take a quick look at how configuration is typically done in .NET 5 (Core).

The framework strongly prescribes developers to create an `appsettings.json` file in the root of their project and configure their application settings in JSON:

```
{
    "Logging": {
        "Level": "Debug"
    },
    "Foo": "foo",
    "Bar": "bar",
    "Server": {
        "Port": 8080,
        "ForceHttps": true
    }
}
```

However not every cloud environment makes editing JSON files in a deployed application's directory easy and therefore most .NET developers still end up using environment variables in production. The `ConfigurationBuilder` makes it possible to specify more than one source and load configuration settings from various places:

```
var config =
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true)
        .AddEnvironmentVariables()
        .Build();
```

The `AddEnvironmentVariables` instruction comes after `AddJsonFile`, which means that any environment variables which have been set would override a previously configured setting in `appsettings.json`. This is a common pattern and standard code seen in almost every .NET application.

Although one thing which is not obvious from the example above is the unidiomatic way of declaring environment variables in order to make this happen. The .NET configuration architecture has been primarily designed with JSON files in mind, which means that .NET developers have to configure nested settings with a double underscore (`__`) in environment variables:

```
LOGGING__LEVEL=Debug
FOO=foo
BAR=bar
SERVER__PORT=8080
SERVER__FORCEHTTPS=true
```

This is such an odd way of configuring environment variables that seeing names such as `SERVER__FORCEHTTPS` are almost a certain giveaway that the underlying architecture is in .NET.

## The ALT.NET way (using .env)

I'd much rather keep my development environment as close to production as possible. Therefore I'd much rather use environment variables as the main configuration mechanism during development too.

What if instead of using a nested JSON document I could configure my application just like in production:

```
LOG_LEVEL=Debug
FOO=foo
BAR=bar
SERVER_PORT=8080
SERVER_FORCE_HTTPS=true
```

Well that's how developers would do it in many other programming languages where the use of `.env` files is more prevalent. A `.env` file is essentially just a flat file specifying environment variables like the ones above. When an engineer launches their application during development then the `.env` file gets parsed and all variables within it will get set on a process level before anything else tries to read them. As the values are set on a process level they will only persist during the currently executing process and vanish on shutdown.

This has several benefits over the `appsettings.json` file approach. First is the incredible simplicity. Secondly is predictability. There is no need to configure multiple configuration providers. An application only retrieves its settings from one source and nowhere else. There is also no complexity around what happens when certain settings are stored in one location (e.g. `appsettings.json`) and other settings in another (e.g. environment variables). Will they merge or replace each other? If an application relies on only environment variables then this is not something to worry about.

Another benefit is how engineers think of configuration. The `appsettings.json` approach invites developers to create overly complex configuration hierarchies. They are easy to read and change during development, but more cumbersome to manage in production.

For example take this snippet as an illustration:

```
{
    "Databases": {
        "SqlServer": {
            "ConnectionString": "foo-bar"
        },
        "Redis": {
            "Endpoint": "localhost:6379"
        }
    }
}
```

In JSON format this looks totally fine, but in reality it probably isn't. Apart from being a data persistence technology, Redis and SQL Server have very little in common. In fact they are probably used for complete different application functionalities. Thus it makes very little sense to group them under one universal `Databases` configuration node together.

Remember in production these will need to get configured as following:

```
DATABASES__SQLSERVER__CONNECTIONSTRING=foo-bar
DATABASES__REDIS__ENDPOINT=localhost:6379
```

This notion makes it much more obvious that the original configuration structure is unfit for everyday use in production.

If environment variables were the primary configuration strategy during development too, then developers would presumably name them more sensibly:

```
SQL_SERVER_CS=foo-bar
REDIS_ENDPOINT=localhost:6379
```

Fortunately using `.env` in .NET is a straightforward alternative to `appsettings.json`.

### Loading .env files in C#

The code for loading and parsing a `.env` file is so simple that it hardly warrants the use of an external dependency via NuGet.

Personally I like to create a `DotEnv.cs` file in my C# project and copy the following code into it:

```
namespace YourApplication
{
    using System;
    using System.IO;

    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
```

Then I add `DotEnv.Load("..")` at the beginning of the `Main` function inside my `Program.cs` file:

```
public static class Program
{
    public static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, ".env");
        DotEnv.Load(dotenv);

        // Other code
    }
}
```

This makes sure that all environment variables get set before any class or function tries to access them.

Finally I specify environment variables as the only required configuration provider:

```
var config =
    new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();
```

Now I can add a `.env` file into the root of my application and configure environment variables like in production.

Of course the file doesn't have to be named `.env` and one can rename it to whichever name suits them best. Regardless which name one settles on, don't forget to add it to one's `.gitignore` file. Especially in open source projects you wouldn't want to commit development secrets into the public domain.

### Loading .env files in F#

In F# the implementation is very similar to C#:

```
namespace YourApplication

module DotEnv =
    open System
    open System.IO

    let private parseLine(line : string) =
        Console.WriteLine (sprintf "Parsing: %s" line)
        match line.Split('=', StringSplitOptions.RemoveEmptyEntries) with
        | args when args.Length = 2 ->
            Environment.SetEnvironmentVariable(
                args.[0],
                args.[1])
        | _ -> ()

    let private load() =
        lazy (
            Console.WriteLine "Trying to load .env file..."
            let dir = Directory.GetCurrentDirectory()
            let filePath = Path.Combine(dir, ".env")
            filePath
            |> File.Exists
            |> function
                | false -> Console.WriteLine "No .env file found."
                | true  ->
                    filePath
                    |> File.ReadAllLines
                    |> Seq.iter parseLine
        )

    let init = load().Value
```

The only main difference is that the `load()` function has been made private and lazy loaded via the `init` variable, meaning that the code inside `load` will only get executed once, regardless of how often `DotEnv.init` gets called. This is to allow the loading of environment variables before `Program.fs` gets invoked.

In functional programming it is very common to make use of static variables and functions. For example I often load my application settings using a static module like this:

```
module Config =
    open System

    let private get key =
        DotEnv.init
        Environment.GetEnvironmentVariable key

    let secretKey = get "SECRET_KEY"
    let redisEndpoint = get "REDIS_ENDPOINT"

    // etc.
```

Those static values will get initialised as soon as the assembly loads into the domain, which is well in advance of any code being called in `Program.fs`. Therefore I have to place the `DotEnv.init` command inside the `get` helper function, making sure that settings from the `.env` file get initialised before the first `Environment.GetEnvironmentVariable` invocation. Given that `DotNet.load()` is `lazy` it will only execute once and not reload the `.env` file on subsequent calls.

Additionally I must also put the `DotEnv.fs` file as the first compilation item in the `.fsproj` file:

```
<ItemGroup>
    <Compile Include="DotEnv.fs" />
    <Compile Include="Other.fs" />
    <Compile Include="Stuff.fs" />
    <Compile Include="Program.fs" />
</ItemGroup>
```

All in all this completely replaces .NET's huge configuration pattern with an extremely simple solution. It's &quot;cloud native&quot; as Microsoft likes to call it and extremely easy to understand.

Just like in C# don't forget to add the `.env` file to your `.gitignore` rules!

## Side notes

### What if an environment variable changes?

A lot of complexity in .NET's configuration classes come from the &quot;need&quot; to react to changes. A web server is a long running process and if someone wants to change a value in `appsettings.json` then any functionality which relies on that setting also has to learn about the update. However most current application hosting solutions such as serverless functions or Kubernetes clusters automatically reload an application on configuration changes, so while it might be an interesting problem to think about, it's certainly more of a theoretical than practical issue. The simple `.env` solution works just fine.

### Why not load environment variables via X?

Could I not just load environment variables via:

- bash/PowerShell?
- launchSettings.json?
- this tool?
- that tool?
- etc.?

Yes, there are many ways to load environment variables into the process before launching an application. However, in this blog post I wanted to show a way which satisfies two requirements (which most of these tools don't):

1. It works for everyone, regardless of OS or IDE
2. It works during <kbd>F5</kbd> debugging from an IDE as well

Ultimately it doesn't matter how you load environment variables, but if it can be done from within .NET so that it just works for everyone on every platform, and also works during debugging without any extra hacks then why not go with such a solution?

### Existing OSS projects for .NET?

If you wondered if there are any existing .NET OSS projects to support `.env` files then you will be pleased to hear that there are some such as [dotenv.net](https://github.com/bolorundurowb/dotenv.net), [dotnet-env](https://github.com/tonerdo/dotnet-env) and [net-dotenv](https://github.com/codeyu/net-dotenv). I have not used any of them but they all seem to be actively maintained.