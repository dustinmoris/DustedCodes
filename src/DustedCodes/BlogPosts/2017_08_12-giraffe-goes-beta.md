<!--
	Tags: giraffe aspnet-core fsharp
-->

# Giraffe goes Beta

![Giraffe Logo](https://raw.githubusercontent.com/dustinmoris/Giraffe/develop/giraffe.png)

After 6 months of working on [Giraffe](https://github.com/dustinmoris/Giraffe), lots of improvements, [great community contributions](https://github.com/dustinmoris/Giraffe/pulls?q=is%3Apr+is%3Aclosed) and running several [private](https://buildstats.info/) as well as commercial web applications in production I am really happy to announce that [Giraffe is finally going Beta](https://github.com/dustinmoris/Giraffe/releases/tag/v0.1.0-beta-001). This might not sound like a big milestone to you, but given that I was very hesitant on pre-maturely labelling the project anything beyond Alpha and the many breaking changes which we had in the past it certainly feels like a big achievement to me! After plenty of testing, tweaking, re-architecting and some real world exposure I think we finally reached a point where I can confidently say that the external facing API will remain fairly stable as of now - and seems to work really well too ;).

## What has changed

### XmlViewEngine

Since the last blog post I have made several improvements to the previously known [functional HTML engine](https://dusted.codes/functional-aspnet-core-part-2-hello-world-from-giraffe#functional-html-view-engine), which has been renamed to a more generic [XmlViewEngine](https://github.com/dustinmoris/Giraffe#renderhtml) now. The new changes allow the `XmlViewEngine` to be used beyond simple HTML views and can be used for things like generating dynamic XML such as [SVG images](https://github.com/dustinmoris/CI-BuildStats/blob/master/src/BuildStats/Views.fs) and more. Personally I think the [XmlViewEngine](https://github.com/dustinmoris/Giraffe/blob/v0.1.0-beta-001/src/Giraffe/XmlViewEngine.fs) is the most feature rich and powerful view engine you can find in any .NET framework today and I will certainly dedicate a whole separate blog post on that topic alone in the near future soon.

### Continuations instead of bindings

One particular big change which we recently had was the move from [binding HttpHandler functions](https://medium.com/@gerardtoconnor/carry-on-continuation-over-binding-pipelines-for-functional-web-58bd7e6ea009) to [chaining HttpFunc continuations](https://github.com/dustinmoris/Giraffe/issues/69) instead. The `HttpHandler` function has changed its signature from a `HttpContext -> Async<HttpContext option>` to a `HttpFunc -> HttpFunc`, whereas a `HttpFunc` is defined as `HttpContext -> Task<HttpContext option>`.

The main difference is that a `HttpHandler` function doesn't return `Some HttpContext` any longer (unless you want to immediately short circuit the chain) and is responsible for invoking the `next` continuation function from within itself. If you think this sounds very similar to ASP.NET Core's middleware then you are not mistaken. It is the same concept which brings several benefits such as better control flow and improved performance.

Even though this posed a fundamental change in architecture, we didn't have to compromise on how easy it is to compose a larger web application in Giraffe:

<pre><code>let webApp =
    choose [
        GET &gt;=&gt;
            choose [
                route &quot;/&quot;       &gt;=&gt; renderHtml indexPage
                route &quot;/signup&quot; &gt;=&gt; renderHtml signUpPage
                route &quot;/login&quot;  &gt;=&gt; renderHtml loginPage
            ]
        POST &gt;=&gt;
            choose [
                route &quot;/signup&quot; &gt;=&gt; signUpHandler
                route &quot;/login&quot;  &gt;=&gt; loginHandler
            ]
        setStatusCode 404 &gt;=&gt; text &quot;Not Found&quot; ]</code></pre>

All credit goes to [Gerard](https://twitter.com/gerardtoconnor) who worked on this big change entirely from concept to implementation on his own.

### Tasks

Another architectural change was that Giraffe works natively with `Task` and `Task<'T>` objects now. Previously you would have had to convert from a C# `Task<'T>` to an F# `Async<'T>` workflow and then back again to a `Task<'T>` before returning the flow to ASP.NET Core, but not any longer.

If you paid close attention in the previous example then you might have noticed that a `HttpFunc` is defined as `HttpContext -> Task<HttpContext option>`. Apart from the additional convenience of not having to convert between tasks and asyncs any more, this change netted us a two figure % perf improvement overall.

From now on you can reference the `Giraffe.Tasks` module and make use of the new `task {}` workflow which will allow you to write asynchronous code just as easily as it was with F# `async {}` before:

<pre><code>open Giraffe.Tasks
open Giraffe.HttpHandlers

let personHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -&gt;
        task {
            let! person = ctx.BindModel&lt;Person&gt;()
            return! json person next ctx
        }</code></pre>

The original code for Giraffe's task implementation has been taken from [Robert Peele's TaskBuilder.fs](https://github.com/rspeele/TaskBuilder.fs) and minimally modified to better fit Giraffe's use case for a highly scalable ASP.NET Core web application.

Again, all credit goes to [Gerard](https://github.com/gerardtoconnor) and his endless efforts in improving Giraffe's overall architecture and performance.

## What to expect next?

### Stability

First of all, as Giraffe has officially entered the Beta phase you can expect a much more stable API with no to minimal breaking changes going forward.

### More performance

Next there are still many ways of improving the internals of `Giraffe.Tasks` which we think will yield even further perf improvements. Additionally Gerard plans to implement an alternative trie routing API which should promise another perf gain on web applications with large routing layers as well.

### More sample applications and templates

Another area where I would like to focus on more in the future is in providing more [sample applications](https://github.com/dustinmoris/Giraffe#demo-apps) and templates which will help people to get up and running with Giraffe in as little time as possible.

Also I would like to blog more about my own usage of Giraffe and show case a few production applications with a closer look at some stats, deployments and general tips &amp; tricks.

I hope you like the latest changes and are still as excited about Giraffe as I am or even considering to build your next F# web application with the fastest functional .NET web framework you'll find anywhere today ;).

If you already use Giraffe for a commercial or hobby project please let me know in the comments below and I can feature you [in the official GitHub repository](https://github.com/dustinmoris/Giraffe#live-apps) if you like.

Thanks for reading and stay tuned until next time!