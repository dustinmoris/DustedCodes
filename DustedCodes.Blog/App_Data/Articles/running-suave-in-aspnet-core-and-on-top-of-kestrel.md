<!--
    Published: 2016-12-15 00:50
    Author: Dustin Moris Gorski
    Title: Running Suave in ASP.NET Core (and on top of Kestrel)
	Tags: fsharp suave aspnet-core kestrel dotnet-core
-->
Ho ho ho, happy F# Advent my friends! This is my blog post for the [F# Advent Calendar in English 2016](https://sergeytihon.wordpress.com/2016/10/23/f-advent-calendar-in-english-2016/). First a quick thanks to [Yan Cui](https://twitter.com/theburningmonk) who has pointed out this calendar to me last year and a big thanks to [Sergey Tihon](https://twitter.com/sergey_tihon) who is organising this blogging event and was kind enough to reserve me a spot this year.

<a href="https://www.flickr.com/photos/130657798@N05/31650186205/in/dateposted-public/" title="santa-suave"><img src="https://c6.staticflickr.com/6/5575/31650186205_48362a86e0_z.jpg" alt="santa-suave"></a>

In this blog post I wanted to write about two technologies which I am particularly excited about: [Suave](https://suave.io/) and [ASP.NET Core](https://www.asp.net/core). Both are frameworks for building web applications, both are written in .NET and both are open source and yet they are very different. [Suave is a lightweight web server](https://github.com/SuaveIO/suave) written entirely in F# and belongs to the family of micro frameworks similar to [NancyFx](http://nancyfx.org/). [ASP.NET Core](https://github.com/aspnet/Home) is Microsoft's new cloud optimised web framework which has been built from the ground up on top of [.NET Core](https://www.microsoft.com/net/core) and all of its goodness. Both are fairly new cutting edge technologies and both are extremely fun to work with.

What I like the most about Suave is that it's written in F# for F#. It is really well designed and embraces functional concepts like [railway oriented programming](http://fsharpforfunandprofit.com/rop/) in its core architecture. Lately I've been a big fan of functional programming and being able to build web applications in a functional way is not only very productive but also a heap of fun. ASP.NET Core is object oriented and closer related to C#, but nonetheless an extraodinary new web framework. After more than 14 years of developing (the old) ASP.NET stack Microsoft has completely revamped the platform and built something new which is [extremely fast](https://www.ageofascent.com/2016/02/18/asp-net-core-exeeds-1-15-million-requests-12-6-gbps/) and flexible. I love [Kestrel](https://github.com/aspnet/KestrelHttpServer), I love how ASP.NET Core is completely modular and extendable (via [middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware)) and I love how it is cross platform compatible and supported by Microsoft (Mono you have served us well but I am glad to move on now). There's more than one good reason to go with either framework and that's why I really wanted to combine them together.

Ideally I would like to continue building web applications with Suave in F# and then plug them into the ASP.NET Core pipeline to run them on top of Kestrel and benefit from both worlds.

## Suave inside ASP.NET Core in theory

In order to better understand Suave let's have a quick look at a simple web application:

<pre><code>open System
open Suave
open Suave.Successful
open Suave.Operators
open Suave.RequestErrors
open Suave.Filters

let simpleApp =
    choose [
        GET &gt;=&gt; choose [
            path &quot;/&quot; &gt;=&gt; OK &quot;Hello world from Suave.&quot;
            path &quot;/ping&quot; &gt;=&gt; OK &quot;pong&quot;
        ]
        NOT_FOUND &quot;404 - Not found.&quot;
    ]

[&lt;EntryPoint&gt;]
let main argv = 
    startWebServer defaultConfig simpleApp
    0</code></pre>

Even in this simple example you can clearly see the core concept behind Suave. An application is always an assemble of one or many web parts. A `WebPart` is a function which takes a `HttpContext` and returns an `option` of `HttpContext` wrapped in an async workflow. Through combinators such as `choose` or `>=>` (and many others) one can compose a complex web application with routing, model binding, view engines and anything else that someone might want to do. At the end there is one top level function of type `WebPart` which takes in a `HttpContext` and returns a `HttpContext`. In this example this function is called `simpleApp`.

In theory the one thing required to plug a Suave web app into ASP.NET Core would be to take an incoming HTTP request from ASP.NET Core and convert it into an `HttpContext` in Suave, execute the top level web part, and then translate the resulting `HttpContext` back into an ASP.NET Core response:

<a href="https://www.flickr.com/photos/130657798@N05/31534152341/in/dateposted-public/" title="suave-in-aspnetcore-concept"><img src="https://c6.staticflickr.com/1/531/31534152341_707b355e78_z.jpg" alt="suave-in-aspnetcore-concept"></a>

The other thing which you get with Suave is a self hosted web server which is built into the framework and the traditional way of starting a Suave web application. The `startWebServer` function takes a `SuaveConfig` object and the top level `WebPart` as input parameters. The config object allows web server specific configuration such as HTTP bindings, request quotas, timeout limits and many more things to be set.

When putting a Suave app into ASP.NET Core then it would be running on a different web server under the hood (Kestrel by default) and it wouldn't necessarily make sense to use an existing `SuaveConfig` in this scenario. Considering that ASP.NET Core offers other natural ways of configuring server settings, I think it is fair to skip the `SuaveConfig` when merging Suave into ASP.NET Core and mainly focusing on a smooth `WebPart` integration.

## Suave inside ASP.NET Core in practice

Taking theory into practise I thought I can make it happen, and when a programmer says that then it usually means to google for an existing solution first. I was lucky to discover [Suave.Kestrel](https://github.com/Krzysztof-Cieslak/Suave.Kestrel) which is a *super early alpha version* of the above concept written by [Krzysztof Cieślak](https://github.com/Krzysztof-Cieslak). Krzysztof is the developer behind the [Ionide](https://github.com/ionide) project which makes F# development in Visual Studio Code even possible, and therefore a massive thanks to him and his great contributions as well!

Even though this project was a good start to begin with there was still loads of work left to do. First I started off by using the existing code and trying to extend it, but then I quickly realised that I was fighting more with the tools than writing any code, which lead me to the decision of creating an entirely new project written in C#. Why C#? Because the Visual Studio tooling for F# projects in .NET Core is non existent (at least at the moment). As much as I love F# if I cannot properly debug or reason with my code then I rather switch to C# and get the job done.

However as a seasoned C# developer that was not a big problem and in the end it wouldn't even matter if the library was written in C#, F# or VB.NET as long as it would allow an easy integration from an F# point of view. Moments like this make me really appreciate the flexibility of the .NET framework.

## Introducing Suave.AspNetCore

After my initial start on the project it took me another 3 months (mainly because I had absolutely no time) to finally release the first version of [Suave.AspNetCore](https://github.com/dustinmoris/Suave.AspNetCore) and make it available. Since yesterday anyone can install [Suave.AspNetCore](https://www.nuget.org/packages/Suave.AspNetCore/) as a NuGet package for a .NET Core application.

I decided to name the package `Suave.AspNetCore` because I thought it is a more representative name of what the NuGet package has to offer. While this library makes it perfectly possible to run Suave on top of Kestrel it is certainly not limited to it. `Suave.AspNetCore` gives a way of plugging a Suave `WebPart` into the ASP.NET Core pipeline and run it on any environment of someone's desire. In theory Suave can be run alongside NancyFx and ASP.NET Core MVC in the same ASP.NET Core application and let the middleware decide which framework is suited best to satisfy an incoming request.

### Current release information

The current version should be able to deal with any incoming web request which can be handled by a Suave `WebPart`. One thing that is missing (but already in the works) is the support for Suave's web socket implementation.

I shall also note that Suave and F# itself don't have an official stable release for .NET Core yet and therefore the project as a whole should be taken with some caution.

## Suave.AspNetCore in action

Ok enough of the talk and let's look at a demo.

First I'll start by using one of my existing [F# ASP.NET Core project templates](https://github.com/dustinmoris/AspNetCoreFSharp) and [upgrade it to .NET Core 1.1](https://blogs.msdn.microsoft.com/dotnet/2016/11/16/announcing-net-core-1-1/#user-content-upgrading-existing-net-core-10-projects).

Then I add `Suave`, `Suave.AspNetCore` and `Newtonsoft.Json` to the dependencies:

<pre><code>&quot;dependencies&quot;: {
    &quot;Microsoft.FSharp.Core.netcore&quot;: &quot;1.0.0-alpha-*&quot;,
    &quot;Microsoft.AspNetCore.Diagnostics&quot;: &quot;1.1.0&quot;,
    &quot;Microsoft.AspNetCore.Server.Kestrel&quot;: &quot;1.1.0&quot;,
    &quot;Microsoft.Extensions.Logging.Console&quot;: &quot;1.1.0&quot;,

    &quot;Suave&quot;: &quot;2.0.0-*&quot;,
    &quot;Suave.AspNetCore&quot;: &quot;0.1.0&quot;,
    &quot;Newtonsoft.Json&quot;: &quot;9.0.1&quot;
  }</code></pre>

Next I move on to the `Startup.fs` file and create a Suave web application:

<pre><code>module App =
    let catchAll =
        fun (ctx : HttpContext) -&gt;
            let json = 
                JsonConvert.SerializeObject(
                    ctx.request,
                    Formatting.Indented)
            OK json 
            &gt;=&gt; Writers.setMimeType &quot;application/json&quot;
            &lt;| ctx</code></pre>

This app is very basic. You can see that it is a single web part which uses Json.NET to serialize the entire `HttpContext` and then later returns a successful response of the Json text with a mime type of `application/json`.

It is not hugely interesting but it is a nice function which will handle every incoming web request and output the resulting `HttpContext` in a more or less readable way. It's at least a good way of quickly verifying if the incoming web request has been correctly converted into a Suave `HttpContext` object.

Finally I go to the `Startup` class and hook up the Suave `catchAll` web app into the ASP.NET Core pipeline via a middleware:

<pre><code>type Startup() =
    member __.Configure (app : IApplicationBuilder)
                        (env : IHostingEnvironment)
                        (loggerFactory : ILoggerFactory) =
        app.UseSuave(App.catchAll) |&gt; ignore</code></pre>

Save all, `dotnet restore`, `dotnet build` and `dotnet run`:

<a href="https://www.flickr.com/photos/130657798@N05/31648972855/in/dateposted-public/" title="running-a-suave-aspnetcore-app"><img src="https://c8.staticflickr.com/6/5616/31648972855_6809777227_z.jpg" alt="running-a-suave-aspnetcore-app"></a>

If everything is correct then going to `http://localhost:5000/` should return a successful response like this:

<a href="https://www.flickr.com/photos/130657798@N05/31649057355/in/dateposted-public/" title="suave-in-aspnetcore-simple-get-request-result"><img src="https://c4.staticflickr.com/1/711/31649057355_07682b177b_z.jpg" alt="suave-in-aspnetcore-simple-get-request-result"></a>

You can [check out the sample app in GitHub](https://github.com/dustinmoris/Suave.AspNetCore/tree/master/test/Suave.AspNetCore.App) and try it yourself!

### Differences between vanilla Suave and Suave.AspNetCore

After running a few tests of my own I noticed a few minor differences.

First I noticed that the original Suave web server converts all HTTP headers into lower case values. For example `Content-Type: text/html` would be stored as `content-type: text/html` in Suave's HTTP header collection. In contrast ASP.NET Core preserves the original casing. When using the `Suave.AspNetCore` middleware then it will match the original Suave behaviour, but can be easily overriden by setting the `preserveHttpHeaderCasing` parameter to `true` in the `UseSuave` method:

<pre><code>app.UseSuave(App.catchAll, true) |&gt; ignore</code></pre>

Another difference which I found was that Suave always sets the `host` variable to `127.0.0.1` for local requests, even when I explicitely call the API with `http://localhost:5000/`. I wasn't able to find out why or where this is happening and if there is a good reason for it. In this case I didn't align with the original Suave behaviour and kept the values provided by ASP.NET Core.

Other than this I haven't found any big differences and hope that it is (mostly) bug free. The project is open source and I am open for ideas, help or suggestions of any kind.

Merry Christmas everyone!