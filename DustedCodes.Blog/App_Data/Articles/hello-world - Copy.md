<!--
    Published: 2017-04-16 22:44
    Author: Dustin Moris Gorski
    Title: A brief overview of Giraffe
	Tags: giraffe aspnet-core fsharp
-->
# Functional ASP.NET Core 2 - The evolution of Giraffe

This is a follow up blog post on the [functional ASP.NET Core]() article from about two months ago. First of all I'd like to say that this has been the longest period I haven't published anything new to my blog since I started blogging in early 2015. The reason is because I have been pretty busy with a private project which I hope to write more about in the future, but more importantly I have been extremely busy organising my own wedding which took place at the end of last month :). Yes!, I've been extremely lucky to have found the love of my life and best friend and after being engaged for almost a year and a half we've finally tied the knot last week. Normally I don't blog about my private life here, but since this has been such a significant moment in my life I thought I should mention a few words here as well and let everyone know that the quiet time has been for a good reason and will not last for much longer now.

While this has primarily occupied the majority of my time I was also quite happy to see my [functional ASP.NET Core project]() receiving some recognition from the community and some really great support from other developers who've been helping me in adding lots of new functionality since then. In this first blog post after my small break I thought I'd take the opportunity and showcase some of the work we've done since the initial release and explain some of the design decisions behind some features.

But first I shall say what most of my readers probably already noticed anyway - the framework has been renamed to Giraffe.

## ASP.NET Core Lambda is now Giraffe

ASP.NET Core Lambda was a good name in terms of being very descriptive of what it stood for, but at the same time there were plenty of other issues which led me and other people believe that a different name would be a better fit.

Initially I named the project ASP.NET Core Lambda, because, at its core it was a functional framework built on top of (and tightly integrated with) ASP.NET Core, so I put one and one together and went with that name.

However, it quickly became apparent that &quot;ASP.NET Core Lambda&quot; wasn't a great name for the following reasons:

- ASP.NET Core Lambda is a bit of a tongue twister.
- &quot;ASP&quot;, &quot;.NET&quot;, &quot;Core&quot; and &quot;Lambda&quot; are extremely overloaded words with more than one meaning. If the project turns out to be successful then any type of search or information lookup (e.g StackOverflow) would be an absolute nightmare with this name.
- Specifically Lambda is associated with Amazon's serverless cloud offering which would add even more to the confusion.
- Finally the name is not very tasteful. Let's be honest, the mix of capitalized and pascal cased words, the additional whitespace and the dot in the wording makes the name look very busy and simply doesn't resemble an elegant or tasteful product.

As a result I decided to rename the project to something different and [put the name up for a vote](), which ultimately led to Giraffe. Looking back I think it was a great choice and I would like to thank everyone who helped me in picking the new name, as well as coming up with other great name suggestions which made the decision not easy at all.

I think Giraffe is a great name now because it is short, it is very clear and distinctive and there is no ambiguity around the spelling or pronunciation. There is also no other product called Giraffe in the .NET space and not really anything else which it could be confused with. The name Giraffe also hasn't been taken as a NuGet package yet which made things really easy. On top of that Giraffe gave loads of room for creating a beautiful logo for which I used [99designs.co.uk](). I created a design challenge there and the winner impressed with this clever and creative design:

![Giraffe Logo](https://raw.githubusercontent.com/dustinmoris/Giraffe/develop/giraffe.png)

Now I can only hope that the product will live up to this beautiful new logo and name, which brings me to the actual topic of this blog post.

## Overview of new features

There has been quite a few changes and new features since my last blog post and a few which I am very excited about are:

- Dotnet new template
- Nested routing
- Razor pages
- Functional view engine
- Content negotiation
- Model binding

### Dotnet new template

One really cool thing you can do with the new .NET tooling is to create project templates which can be easily installed via NuGet packages.

Thanks to [David Sinclair](https://github.com/dsincl12) you can install a Giraffe template by running the following command now:

<pre><code>dotnet new -i giraffe-template::*</code></pre>

This will install the [giraffe-template](https://www.nuget.org/packages/giraffe-template) NuGet package to your locally available dotnet templates.

Then you can start using `Giraffe` as a new project type when running the `dotnet new` command:

<pre><code>dotnet new giraffe</code></pre>

It is as simple as that to get started with Giraffe now. After that a quick `dotnet restore` and `dotnet run` gets you a small hello world app running with Giraffe.

### Nested routing

Another cool feature which [Stuart Lang](slang25) helped to develop is nested routing.

The new `subRoute` handler allows users to create more complex nested routes which can be very useful when grouping certain routes.

A good example would be when an API changes it's authentication method and you'd want to group routes together, so that the authentication method needs to be specified only once for a group of routes:

<pre><code>let app = 
    subRoute "/api"
        (choose [
            subRoute "/v1"
                (oldAuthentication >=> choose [
                    route "/foo" >=> text "Foo 1"
                    route "/bar" >=> text "Bar 1" ])
            subRoute "/v2"
                (newAuthentication >=> choose [
                    route "/foo" >=> text "Foo 2"
                    route "/bar" >=> text "Bar 2" ]) ])</code></pre>

In this example a request to `http://localhost:5000/api/v1/foo` would use `oldAuthentication` and a request to `http://localhost:5000/api/v2/foo` would end up using `newAuthentication`.

There is also a [`subRouteCi`](https://github.com/dustinmoris/Giraffe#subrouteci) handler which is the case insensitive equivalent of `subRoute`.

### Razor pages

Next is the support of Razor pages in Giraffe. [Nicolás Herrera](https://github.com/nicolocodev) developed the first version of Razor pages by utilising the [RazorLight](https://github.com/toddams/RazorLight) engine.

At a later stage I discovered that by referencing the `Microsoft.AspNetCore.Mvc` NuGet package I can easily re-use the original Razor engine and therefore offer a more complete and original Razor experience in Giraffe as well. While under the hood the engine changed from [RazorLight]() to [ASP.NET Core MVC Razor engine]() the functionality remained more or less the same as implemented by Nicolás in the first place.

In order to enable Razor pages in Giraffe one has to register the Razor engine first:

<pre><code>type Startup() =
    member __.ConfigureServices (svc : IServiceCollection,
                                 env : IHostingEnvironment) =    
        Path.Combine(env.ContentRootPath, "views")
        |> svc.AddRazorEngine
        |> ignore</code></pre>

After that one can use the `razorView` handler to return a Razor page from Giraffe:

<pre><code>let model = { WelcomeText = "Hello World" }

let app = 
    choose [
        route "/" >=> razorView "text/html" "Index" model
    ]</code></pre>

The above example assumes that there is a `/views` folder in the project with an `Index.cshtml` file inside.

One of the arguments passed into the `razorView` handler is the mime type which should be returned by the Razor handler. In this example it is set to `text/html`, but if the Razor page represents something different (like an SVG image template for example) then with the `razorView` handler a different `Content-Type` can be set as well.

In most cases `text/html` is probably the desired mime type and therefore there is a second handler called `razorHtmlView` which does exactly that:

<pre><code>let model = { WelcomeText = "Hello World" }

let app = 
    choose [
        route  "/" >=> razorHtmlView "Index" model
    ]</code></pre>

A more involved example with a layout page and a partial view can be seen in the [SampleApp](https://github.com/dustinmoris/Giraffe/tree/develop/samples/SampleApp/SampleApp/views) project in Giraffe.

#### Using DotNet.Watcher to reload the project on Razor page changes

If you come from ASP.NET Core MVC then you might be used to having Razor pages automatically re-compile every time a page changes during development without having to manually restart your web application. In Giraffe you can get the same experience by adding the [DotNet Watcher Tools]() in your `.fsproj` file and put a watch on all `.cshtml` files:

<pre><code>&lt;ItemGroup&gt;
    &lt;DotNetCliToolReference Include=&quot;Microsoft.DotNet.Watcher.Tools&quot; Version=&quot;1.0.0&quot; /&gt;
&lt;/ItemGroup&gt;
  
&lt;ItemGroup&gt;
    &lt;Watch Include=&quot;**\*.cshtml&quot; Exclude=&quot;bin\**\*&quot; /&gt;
&lt;/ItemGroup&gt;</code></pre>

By adding these sections into the project file you can make changes to a `.cshtml` file and immediately see the changes take effect in a running Giraffe web application without having to manually restart.

#### Dependency on AspNetCore.Mvc

One other thing which might sound strange is the dependency on the `Microsoft.AspNetCore.Mvc` NuGet package. Isn't that the full MVC library being referenced by Giraffe now? Yes it is, but does it really matter?

In order to get Razor pages working in Giraffe there were three options available:

- Implement Giraffe's own Razor engine
- Use someone else's custom implemented Razor engine
- Use the original Razor engine

I certainly didn't have an appetite for the first option, which is hopefully understandable, and therefore was left with the choice between a custom Razor engine vs. the original Razor engine.

At the time of writing there was only one .NET Core compatible custom Razor engine available, which is [RazorLight](). From what I know RazorLight is a very nice library and definitely highly recommended, but not necessarily the right choice for Giraffe.

When you ignore the name of the NuGet package for a second then there is really not much difference between referencing [RazorLight]() or [Microsoft.AspNetCore.Mvc]() in Giraffe. Both require a new NuGet dependency and both are a library which exposes some functionality to render Razor views. The ASP.NET Core MVC package might be slightly bigger and offer more functionality than what Giraffe actually needs, but that doesn't really matter, because Giraffe ignores the rest and only uses what it needs for the Razor support. I think it is pretty normal that any given library often offers more functionality than what a single project actually makes use of.

In the case of Giraffe I was faced with a trade-off between a dependency which uses slightly more KBs disk space in the NuGet packages folder with complete and original Razor support vs. a slightly smaller library which offers a custom implementation of Razor pages.

Maybe I am wrong, but as far as I know there is absolutely no disadvantage in Giraffe using the MVC NuGet package in order to gain the original Razor experience in return. I also believe that this option is more aligned with Giraffe's goal to be tightly integrated with the original ASP.NET Core experience and to make use of existing, well supported APIs for easier portability.

I truly believe that the strength of Giraffe comes in its reliance on other strongly supported APIs and by **standing on the shoulders of giants**.

### Functional view engine