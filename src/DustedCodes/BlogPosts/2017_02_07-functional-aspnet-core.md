<!--
	Tags: fsharp aspnet-core kestrel
-->

# Functional ASP.NET Core

In December 2016 I participated in the [F# Advent Calendar](https://sergeytihon.wordpress.com/2016/10/23/f-advent-calendar-in-english-2016/) where I wrote a blog post on [running Suave in ASP.NET Core](https://dusted.codes/running-suave-in-aspnet-core-and-on-top-of-kestrel). As part of that blog post I introduced the [Suave.AspNetCore](https://www.nuget.org/packages/Suave.AspNetCore/) NuGet package which makes it possible to run a [Suave web application](https://suave.io/) inside ASP.NET Core via a new middleware.

So far this has been pretty good and as of last week the [GitHub repository](https://github.com/SuaveIO/Suave.AspNetCore) has been moved to the official [SuaveIO GitHub organisation](https://github.com/SuaveIO) as well. A while ago someone even tweeted me that the performance has been pretty good too:

<a href="https://twitter.com/jamesjrg/status/809923555894902784" title="suave-aspnet-core-perf-tweet"><img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-02-07/32713608366_88c2eca85d_o.png" alt="suave-aspnet-core-perf-tweet"></a>

Even though this made me very happy there was still one thing that bugged me until today.

## Why I created Suave.AspNetCore

My main motivation for running Suave inside ASP.NET Core was to benefit from the speed and power of [Kestrel](https://github.com/aspnet/KestrelHttpServer) while still being able to build a web application in a functional approach. [Suave.AspNetCore](https://www.nuget.org/packages/Suave.AspNetCore/) made this possible, but afterwards I realised that this was not my final goal yet.

Ultimately I would like to build a web application in ASP.NET Core with a functional framework which does not only benefit from Kestrel but also from the entire ASP.NET Core eco system, including other middleware such as [static files](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files), [authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity), [authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction), [security](https://docs.microsoft.com/en-us/aspnet/core/security/), the flexibility of the [config system](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration), [logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging) or simply being able to retrieve information from the current [hosting environment](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments) and more.

There is a lot of great features in ASP.NET Core which have been carefully crafted by experts (e.g. security) and I wouldn't want to miss out on those based on the framework of my choice. Unfortunately with Suave and Suave.AspNetCore I am limited in what other middleware I can use in combination with a Suave ASP.NET Core web application.

## Suave and ASP.NET Core - Buddies but not family

Suave doesn't naturally fit into ASP.NET Core the way MVC does.

The reason is because Suave is not only a web framework but more of a web platform similar to what ASP.NET Core is itself. It's probably never been intended to be integrated with ASP.NET Core in the first place.

Think of it like this, ASP.NET Core is a web platform which sets the ground work for building any web application and MVC is a framework on top of the platform, which enables building web applications with an object oriented [Model-View-Controller design pattern](https://msdn.microsoft.com/en-us/library/ff649643.aspx). Ideally as an F# developer I would like to replace the object oriented MVC framework with a functional equivalent, but keep the rest of ASP.NET Core's offering at the same time. This is very difficult with Suave at the moment.

Suave has its own HTTP server, its own Socket server and its own HTTP abstractions. As a result Suave's [Socket](https://github.com/SuaveIO/suave/blob/master/src/Suave/WebSocket.fs) implementation is not compatible with [ASP.NET Core's web socket server](https://github.com/aspnet/WebSockets) and Suave's [HttpContext](https://github.com/SuaveIO/suave/blob/master/src/Suave/Http.fs#L526) is vastly different from ASP.NET Core's [HttpContext](https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.Http.Abstractions/HttpContext.cs).

This is why the [Suave.AspNetCore middleware](https://github.com/SuaveIO/Suave.AspNetCore/blob/master/src/Suave.AspNetCore/SuaveMiddleware.cs#L33) has to translate one `HttpContext` into another and then back again. It works, but it is not ideal, because there's a lot of information that gets lost along the way (e.g. the `User` object in ASP.NET Core is of type `IPrincipal` and in Suave it is a `Map<string, obj>`). This is a limiting factor. For example there's no way to access the `Authentication` property, the `Session` object or the `Features` collection of the original ASP.NET Core `HttpContext` from inside a Suave application.

This means that even though I can run a Suave web application in ASP.NET Core, I still have to re-build a lot of the ground work that has been laid out for me by other ASP.NET Core middleware. In some cases this might be a minor problem, but in others I consider it a big issue. Especially when it comes to critical components such as security I would much rather want to rely on the implementation provided by Microsoft and other industry experts than (re-)inventing it myself.

None of this is Suave's fault though, because it has not been designed with ASP.NET Core in mind, which is more than fair, but for people like me who want to benefit from both platforms this is still an important issue to consider.

## Why ASP.NET Core?

If Suave is already a well working standalone product for building web applications in a functional way, why would I even bother with ASP.NET Core? Well this is a good question and I can only answer it for myself.

For me the main reasons for using ASP.NET Core are the following:

- [Performance](#performance)
- [Security](#security)
- [Laziness](#laziness)
- [Community](#community)
- [Impatience](#impatience)
- [Fear of missing out](#fear-of-missing-out)
- [Support](#support)

<h3 id="performance">Performance</h3>

[Kestrel is extremely fast](https://www.ageofascent.com/2016/02/18/asp-net-core-exeeds-1-15-million-requests-12-6-gbps/) and the Microsoft team is working hard on making it even faster. Depending on what type of application you are trying to build this can be a big or a small selling point.

Personally I am working on a project which anticipates a significant load at most times and therefore performance is key. I also have a few smaller side projects which run in Docker containers in the cloud and the quicker these applications can handle web requests, the less I have to pay for additional computation power.

<h3 id="security">Security</h3>

As mentioned before I am very reluctant to using 3rd party security code which hasn't been vetted, audited and tested to the same depth as the one provided by Microsoft and the ASP.NET Core team. They have years of invaluable experience and I trust them with security more than other individuals who I barely know or even myself. Call me pessimistic, but security is such a complex topic that I don't even think that a single person should do this on their own. It's one of those things where I believe you have to have a big team of experts and resources behind you to stand a chance against the various vulnerabilities of today.

<h3 id="laziness">Laziness</h3>

I am a lazy developer. I don't enjoy building the 100th logging framework or coming up with yet another configuration API. I want to build new original ideas and not waste my time on stuff which has been done by thousands of other developers before me. ASP.NET Core is a platform which offers many things out of the box and essentially saves me a lot of valuable time.

Additionally it offers easy integration points for other 3rd party code and has a huge developer base behind it which gives me access to even more useful tools and libraries which can benefit my projects.

<h3 id="community">Community</h3>

The community around ASP.NET (Core) is probably the biggest of all .NET web frameworks. There is significantly more people reporting issues, working on bug fixes and building new features into the product than anyone else. The value of such a vibrant community shall not be underestimated. There's nothing better than having bugs fixed by other people before I even encounter them myself.

Another great side effect is that a lot of my questions might have already been answered on StackOverflow and that there is a ton of other useful blog posts explaining stuff that I don't have to work out myself. As an employer it might be also beneficial to pick a framework which has a bigger talent pool than others.

<h3 id="impatience">Impatience</h3>

I am very impatient and for me it is very important that certain issues get addressed fairly quickly. For instance in recent years many web servers were upgrading to support HTTP/2 which is an important improvement over HTTP/1.1. My chances of getting such critical updates are probably higher with a product which is used by thousands of developers than something else which is maybe a little bit more niche. This is not always true, but from my experience this is generally the case.

<h3 id="fear-of-missing-out">Fear of missing out</h3>

We don't know what the future holds for us. Tomorrow someone might release something incredibly awesome which might not be available on smaller platforms in its initial phase. I love working with bleeding edge technology and as such I do have a certain degree of FOMO when it comes to software innovations.

<h3 id="support">Support</h3>

Lastly ASP.NET Core, even though open source, remains an enterprise supported product and that has a lot of value as well. Over the years I had to replace many successful open source packages because the original maintainers got tired of supporting it and no one else stepped in to replace them, which meant they became stale and essentially completely out of date. There is no guarantee that Microsoft will support the ASP.NET (Core) platform for ever, but from the looks of it I don't expect it to go away any time soon either. In fact the current signs seem to suggest the exact opposite considering that ASP.NET itself has already more than a decade on its shoulders and Microsoft recently decided to invest in a complete new re-design to continue its success.

Not everyone might agree with my reasoning, but for me these are very compelling points to stick with ASP.NET Core as my preferred .NET web platform and try to use as much of its ready made features as possible.

## Building a functional framework for ASP.NET Core

Now that I have explained why I would like to build a web application with ASP.NET Core I have the only problem that as an F# developer there's no ideal framework available yet.

This got me thinking what if I could build my own little micro framework in F# that borrows the functional design pattern of Suave, but embraces the power of ASP.NET Core?

### Defining a functional HttpHandler

A functional ASP.NET Core web application could look as simple as a function which takes in a `HttpContext` and returns a `HttpContext`. In functional programming everything is a function after all. Inside that function it would have full access to the `Request` and `Response` object as well as all the other objects to successfully process an incoming web request and return a response.

I could call such a web function a `HttpHandler`:

<pre><code>type HttpHandler = HttpContext -&gt; HttpContext</code></pre>

Not every incoming web request can or should be handled by a `HttpHandler`. For example if the request was made to a route which wasn't anticipated, like a static file which should be picked up by the static file middleware, then a `HttpHandler` should be able to skip this particular request.

In this case there should be an option to return nothing so that another `HttpHandler` can try to satisfy the incoming request or the calling middleware can defer the `HttpContext` to the next middleware:

<pre><code>type HttpHandler = HttpContext -&gt; HttpContext option</code></pre>

By making the `HttpContext` optional the function can now either return `Some HttpContext` or `None`.

Additionally the `HttpHandler` shouldn't block on IO operations or other long running tasks and therefore return the `HttpContext option` wrapped in an asynchronous workflow:

<pre><code>type HttpHandler = HttpContext -&gt; Async&lt;HttpContext option&gt;</code></pre>

This is slowly taking shape, but there's still something missing.

In ASP.NET Core MVC controller dependencies are automatically resolved during instantiation. This is very useful, because in ASP.NET Core dependencies are registered in a central place inside the `ConfigureServices` method of the `Startup.cs` class file.

Automatic dependency resolution is not really a thing in functional programming, because dependencies are normally functions and not objects. Functions can be passed around or partially applied which usually makes object oriented dependency management obsolete.

However, because most ASP.NET Core dependencies are registered as objects inside an `IServiceCollection` container, a `HttpHandler` can resolve these dependencies through an `IServiceProvider` object.

This is done by wrapping the original `HttpContext` and an `IServiceProvider` object inside a new type called `HttpHandlerContext`:

<pre><code>type HttpHandlerContext =
    {
        HttpContext : HttpContext
        Services    : IServiceProvider
    }</code></pre>

By changing the `HttpHandler` function definition we can take advantage of this new type:

<pre><code>type HttpHandler = HttpHandlerContext -&gt; Async&lt;HttpHandlerContext option&gt;</code></pre>

With that one should be able to build pretty much any web application of desire. If you have worked with Suave in the past then this should look extremely familiar as well.

### Combining smaller HttpHandlers to bigger applications

In principal there's nothing you cannot do with an `HttpHandler`, but it wouldn't be very practical to build a whole web application in one function. The beauty of functional programming is the composition of many smaller functions to one bigger application.

The simplest combinator would be a bind function which takes two `HttpHandler` functions and combines them to one:

<pre><code>let bind (handler : HttpHandler) (handler2 : HttpHandler) =
    fun (ctx : HttpHandlerContext) -&gt;
        async {
            let! result = handler ctx
            match result with
            | None      -&gt; return None
            | Some ctx2 -&gt; return Some ctx2
        }</code></pre>

As you can see the `bind` function takes two different `HttpHandler` functions and a `HttpHandlerContext`. First it evaluates the first handler and checks its result. If the result was `None` then it will stop at this point and return `None` as the final result. If the result was `Some HttpHandlerContext` then it will take the resulting context and use it to evaluate the second `HttpHandler`. Whatever the second `HttpHandler` returns will be the final result in this case.

This pattern is often referred to as [railway oriented programming](http://fsharpforfunandprofit.com/rop/). If you are interested to learn more about it then please check out [Scott Wlaschin's slides and video](http://fsharpforfunandprofit.com/rop/) on his website or this [lengthy blog post](http://fsharpforfunandprofit.com/posts/recipe-part2/) on that topic.

One more thing that should be considered in the `bind` function is to check if a `HttpResponse` has been already written by the first `HttpHandler` before invoking the second `HttpHandler`. This is required to prevent a potential exception when a `HttpHandler` tries to make changes to the `HttpResponse` after another `HttpHandler` has already written to the response. In this case the `bind` function should not invoke the second `HttpHandler`:

<pre><code>let bind (handler : HttpHandler) (handler2 : HttpHandler) =
    fun (ctx : HttpHandlerContext) -&gt;
        async {
            let! result = handler ctx
            match result with
            | None      -&gt; return None
            | Some ctx2 -&gt;
                match ctx2.HttpContext.Response.HasStarted with
                | true  -&gt; return  Some ctx2
                | false -&gt; return! handler2 ctx2
        }</code></pre>

To round this up we can bind the `bind` function to the `>>=` operator:

<pre><code>let (>>=) = bind</code></pre>

With the `bind` function we can combine unlimited `HttpHandler` functions to one.

The flow would look something like this:

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-02-07/32713574026_7ef98d6280_o.png" alt="aspnet-core-lambda-http-handler-flow-cropped">

Another very useful combinator which can be borrowed from Suave is the `choose` function. The `choose` function let's you define a list of multiple `HttpHandler` functions which will be iterated one by one until the first `HttpHandler` returns `Some HttpHandlerContext`:

<pre><code>let rec choose (handlers : HttpHandler list) =
    fun (ctx : HttpHandlerContext) -&gt;
        async {
            match handlers with
            | []                -&gt; return None
            | handler :: tail   -&gt;
                let! result = handler ctx
                match result with
                | Some c    -&gt; return Some c
                | None      -&gt; return! choose tail ctx
        }</code></pre>

In order to better see the usefulness of this combinator it's best to look at an actual example.

Let's define a few simple `HttpHandler` functions first:

<pre><code>let httpVerb (verb : string) =
    fun (ctx : HttpHandlerContext) -&gt;
        if ctx.HttpContext.Request.Method.Equals verb
        then Some ctx
        else None
        |&gt; async.Return

let GET     = httpVerb "GET"    : HttpHandler
let POST    = httpVerb "POST"   : HttpHandler

let route (path : string) =
    fun (ctx : HttpHandlerContext) -&gt;
        if ctx.HttpContext.Request.Path.ToString().Equals path
        then Some ctx
        else None
        |&gt; async.Return

let setBody (bytes : byte array) =
    fun (ctx : HttpHandlerContext) -&gt;
        async {
            ctx.HttpContext.Response.Headers.["Content-Length"] &lt;- new StringValues(bytes.Length.ToString())
            ctx.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length)
            |&gt; Async.AwaitTask
            |&gt; ignore
            return Some ctx
        }

let setBodyAsString (str : string) =
    Encoding.UTF8.GetBytes str
    |&gt; setBody</code></pre>

This already gives a good illustration of how easily one can create different `HttpHandler` functions to do various things. For instance the `httpVerb` handler checks if the incoming request matches a given HTTP verb. If yes it will proceed with the next `HttpHandler`, otherwise it will return `None`. The two functions `GET` and `POST` re-purpose `httpVerb` to specifically check for a GET or POST request.

The `route` function compares the request path with a given string and either proceeds or returns `None` again. Both, `setBody` and `setBodyAsString` write a given payload to the response of the `HttpContext`. This will trigger a response being made back to the client.

Each `HttpHandler` is kept very short and has a single responsibility. Through the `bind` and `choose` combinators we can combine many `HttpHandler` functions into one larger web application:

<pre><code>let webApp =
    choose [
        GET &gt;&gt;=
            choose [
                route "/"     &gt;&gt;= setBodyAsString "Index"
                route "/ping" &gt;&gt;= setBodyAsString "pong"
            ]
        POST &gt;&gt;=
            choose [
                route "/submit" &gt;&gt;= setBodyAsString "Submitted!"
                route "/upload" &gt;&gt;= setBodyAsString "Uploaded!"
            ]
    ]</code></pre>

Even though I've barely written any code this functional framework already proves to be quite powerful.

At last I need to create a new middleware which can run this functional web application:

<pre><code>type HttpHandlerMiddleware (next     : RequestDelegate,
                            handler  : HttpHandler,
                            services : IServiceProvider) =

    do if isNull next then raise (ArgumentNullException("next"))

    member __.Invoke (ctx : HttpContext) =
        async {
            let httpHandlerContext =
                {
                    HttpContext = ctx
                    Services    = services
                }
            let! result = handler httpHandlerContext
            if (result.IsNone) then
                return!
                    next.Invoke ctx
                    |&gt; Async.AwaitTask
        } |&gt; Async.StartAsTask</code></pre>

And finally hook it up in `Startup.cs`:

<pre><code>type Startup() =
    member __.Configure (app : IApplicationBuilder) =
        app.UseMiddleware&lt;LambdaMiddleware&gt;(webApp) |&gt; ignore</code></pre>

This web framework shows what I love about F# so much. With very little code I was able quickly write a basic web application from the ground up. It looks very much like an ASP.NET Core clone of Suave, but with the difference that it fully embraces the ASP.NET Core architecture and its `HttpContext`.

## Functional ASP.NET Core framework

By extending the above example with a few more useful `HttpHandler` functions to return JSON, XML, HTML or even templated (DotLiquid) views someone could create a very powerful ASP.NET Core functional web framework, which could be easily seen as an MVC replacement for F# developers.

This is exactly what I did and I named it [ASP.NET Core Lambda](https://github.com/dustinmoris/AspNetCore.Lambda), because I simply couldn't think of a more descriptive or &quot;cooler&quot; name. It is a functional ASP.NET Core micro framework and I've built it primarily for my own use. It is still very early days and in alpha testing, but I already use it for two of my private projects and it works like a charm.

### How does it compare to other .NET web frameworks

Now you might ask yourself how does this compare to other .NET web frameworks and particularly to Suave (since I've borrowed a lot of ideas from Suave and from Scott Wlaschin's blog)?

I think this table explains it very well:

<table>
    <tr>
        <th></th>
        <th>Paradigm</th>
        <th>Language</th>
        <th>Hosting</th>
        <th>Frameworks</th>
    </tr>
    <tr>
        <th>MVC</th>
        <td>Object oriented</td>
        <td>C#</td>
        <td>ASP.NET (Core) only</td>
        <td>Full .NET, .NET Core</td>
    </tr>
    <tr>
        <th>NancyFx</th>
        <td>Object oriented</td>
        <td>C#</td>
        <td>Self-hosted or ASP.NET (Core)</td>
        <td>Full .NET, .NET Core, Mono</td>
    </tr>
    <tr>
        <th>Suave</th>
        <td>Functional</td>
        <td>F#</td>
        <td>Primarily self-hosted</td>
        <td>Full .NET, .NET Core, Mono</td>
    </tr>
    <tr>
        <th>Lambda</th>
        <td>Functional</td>
        <td>F#</td>
        <td>ASP.NET Core only</td>
        <td>.NET Core</td>
    </tr>
</table>

MVC and NancyFx are both heavily object-oriented frameworks mainly targeting a C# audience. MVC probably the most feature rich framework and NancyFx with its [super-duper-happy-path](https://github.com/NancyFx/Nancy/wiki/Introduction) are probably the most wide spread .NET web frameworks. NancyFx is also very popular with .NET developers who want to run a .NET web application self-hosted on Linux (this was a big selling point pre .NET Core times).

Suave was the first functional web framework built for F# developers. It is a completely independent standalone product which was designed to be cross platform compatible (via Mono) and self-hosted. A large part of the Suave library is compounded of its own HTTP server and Socket server implementation. Unlike NancyFx it is primarily meant to be self-hosted, which is why the separation between web server and web framework is not as clean cut as in NancyFx (e.g. the Suave NuGet library contains everything in one while NancyFx has separate packages for different hosting options).

[ASP.NET Core Lambda](https://github.com/dustinmoris/AspNetCore.Lambda) is the smallest of all frameworks (and is meant to stay this way). It is also a functional web framework built for F# developers, but cannot exist outside of the ASP.NET Core platform. It has been tightly built around ASP.NET Core to leverage its features as much as possible. As a result it is currently the only (native) functional web framework which is a first class citizen in ASP.NET Core.

I think it has its own little niche market where it doesn't really compete with any of the other web frameworks. It is basically aimed at F# developers who want to use a ASP.NET Core in a functional way.

While all these web frameworks share some similarities they still have their own appliances and target a different set of developers.

Watch this space for more updates on [ASP.NET Core Lambda](https://github.com/dustinmoris/AspNetCore.Lambda) in the future and feel free to try it out and let me know how it goes! So far I've been very happy with the results and it has become my goto framework for new web development projects.