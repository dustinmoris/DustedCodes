<!--
    Tags: fsharp suave error-handling logging
-->

# Custom error handling and logging in Suave

Some years ago when you wanted to develop a .NET web application it was almost given that it will run on IIS, but today we have a sheer amount of different web server technologies to our availability. If you are an F# developer or in the process of learning F# (like me) then you will most likely come across the [Suave](https://suave.io/) web framework.

Suave is a lightweight, non-blocking web server written in F#. It is fairly new to the .NET space, but works wonderfully well and is very idiomatic to functional paradigms. Even though it is still early days it has probably the coolest name and logo amongst its competitors already:

<a href="https://www.flickr.com/photos/130657798@N05/26774440854/in/dateposted-public/" title="suave"><img src="https://c7.staticflickr.com/8/7674/26774440854_6a9c9217c1_z.jpg" alt="suave" class="two-third-width"></a>

After working with Suave for more than a month now I can say that I find it very nice and intuitive. It is a well designed web framework which is easy to work with and allows rapid development if you know how it works. However, if you don't know how it works then you might struggle to get anything done at all. This is exactly what happened to me when I started adopting the framework in the beginning. The documentation is almost non existent and if you look for any advice on how to implement certain things then you are better off by browsing the [GitHub repository](https://github.com/SuaveIO/suave) directly. The lack of documentation is not a mistake, but rather a concious design decision which the Suave team explains as following:

> In suave, we have opted to write a lot of documentation inside the code; so just hover the function in your IDE or use an assembly browser to bring out the XML docs.

Even though I found myself around I wish there would have been a little bit more documentation or more code examples available which would have put me on the right track straight away. With that in mind I will jump straight into some code examples myself and try to explain proper error handling and logging in Suave.

## Hello World in Suave

Before we get started we need a simple web application to begin with:

<pre><code>open Suave
open Suave.Operators
open Suave.Successful
open Suave.Filters

let app =
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World&quot;
    ]

[&lt;EntryPoint&gt;]
let main argv =
    startWebServer defaultConfig app
    0</code></pre>

When I navigate to [http://localhost:8083](http://localhost:8083) I can see the &quot;Hello World&quot; message.

When I try to browse a path which doesn't exist then Suave will not serve the request by default. For example [http://localhost:8083/foo](http://localhost:8083/foo) will not return anything. If you want Suave to return a 404 (or anything else) for non existing paths then you can append a generic fallback case to the end of the webpart options:

<pre><code>let app =
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World&quot;
        NOT_FOUND "Resource not found."
    ]</code></pre>

The `NOT_FOUND` webpart and other pre-defined client error responses can be found in the `Suave.RequestErrors` namespace.

Now that I covered that, let's look at some proper error handling next.

## Error Handling in Suave

In order to test error handling I need a route which throws an exception:

<pre><code>let errorAction =
    fun _ -&gt; async { return failwith &quot;Uncaught exception!!&quot; }

let app =
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World&quot;
        GET &gt;=&gt; path &quot;/error&quot; &gt;=&gt; errorAction
        NOT_FOUND &quot;Resource not found.&quot;
    ]</code></pre>

The [default Suave error handler](https://github.com/SuaveIO/suave/blob/releases/v1.x/src/Suave/Web.fs#L14) will return a 500 Internal Server error and a static message in plain text, unless you run your application locally in which case it will return the exception message in HTML.

If you want to change the default behaviour and provide a different implementation, then you can do this by setting a custom function of type `ErrorHandler` as part of the web server configuration.

The `ErrorHandler` type is defined as following:

<pre><code>type ErrorHandler = Exception -&gt; String -&gt; HttpContext -&gt; WebPart</code></pre>

For example you can declare a new error handler like this:

<pre><code>let customErrorHandler ex msg ctx =
    // Change implementation as you wish
    INTERNAL_ERROR (&quot;Custom error handler: &quot; + msg) ctx

let customConfig =
    {
        defaultConfig with
            errorHandler = customErrorHandler
    }

[&lt;EntryPoint&gt;]
let main argv =
    startWebServer customConfig app
    0</code></pre>

Let's say you have a RESTful service and you want to return an error in Json instead of plain text. You could do this by amending the code as following:

<pre><code>let JSON_ERROR obj =
    JsonConvert.SerializeObject obj
    |&gt; INTERNAL_ERROR
    &gt;=&gt; setMimeType &quot;application/json; charset=utf-8&quot;

let customErrorHandler ex msg ctx =
    JSON_ERROR ex ctx</code></pre>

The `JSON_ERROR` function is a custom helper function, which uses [Newtonsoft.Json](https://www.nuget.org/packages/newtonsoft.json/) to serialize an object and pipe it through to the `INTERNAL_ERROR` function in combination with a mime type of &quot;application/json; charset=utf-8&quot;.

You could go one step further and examine the Accept header of the incoming HTTP request and return the error response in a mime type which is supported by the client. The `ctx` parameter which is of type `Suave.Http.HttpContext` has all the relevant information to make that distinction:

<pre><code>type AcceptType =
    | Json
    | Xml
    | Other

let getAcceptTypeFromRequest ctx =
    match ctx.request.header &quot;Accept&quot; with
    | Choice1Of2 accept -&gt;
        match accept with
        | &quot;application/json&quot; -&gt; Json
        | &quot;application/xml&quot;  -&gt; Xml
        | _                  -&gt; Other
    | _                      -&gt; Other

let customErrorHandler ex msg ctx =
    match getAcceptTypeFromRequest ctx with
    | Json  -&gt; JSON_ERROR ex ctx
    | Xml   -&gt; XML_ERROR ex ctx
    | Other -&gt; INTERNAL_ERROR msg ctx</code></pre>

*The `XML_ERROR` function doesn't exist by default and would need to be implemented similarly to the `JSON_ERROR` function from the previous example.*


This code is not full proof, but you can get the idea of it. There is nothing you can't do with the error handler and you can tailor a custom implementation entirely to your own application requirements.

Let's move on to logging now.

## Custom logging in Suave

Just like the error handler is set in the web server configuration the default logger can be overridden there as well, but with the small difference that the logger needs to be of type `Suave.Logging.Logger`.

The `Logger` interface has only one method for logging events:

<pre><code>abstract member Log : LogLevel -&gt; (unit -&gt; LogLine) -&gt; unit</code></pre>

It's simple but enough to build custom loggers for most use cases:

<pre><code>let pressTheRedButton line =
    line |&gt; ignore // Implement here

let logAsNormal level line =
    line |&gt; ignore // Implement here

type CustomLogger() =
    interface Logger with
        member __.Log level line =
            match level with
            | LogLevel.Fatal -&gt; pressTheRedButton line
            | _ -&gt; logAsNormal level line

let customConfig =
    { defaultConfig with
        errorHandler = customErrorHandler
        logger = new CustomErrorLogger() }</code></pre>

The log level argument is an enum of type `Suave.Logging.LogLevel` and has the following options available:

- Verbose
- Debug
- Info
- Warn
- Error
- Fatal

It also provides a few helper methods to convert between its string and integer representation as well as a few overloads for comparison and an implementation of the `IComparable` and `IEquatable` interface. Check the full [implementation](https://github.com/SuaveIO/suave/blob/releases/v1.x/src/Suave/Logging/LogLevel.fs) for a complete overview.

Suave also provides a few [default loggers](https://github.com/SuaveIO/suave/blob/releases/v1.x/src/Suave/Logging/Logger.fs) which can be used out of the box. There is a `ConsoleWindowLogger` and an `OutputWindowLogger` which do exactly what they say.

Additionally there is another useful logger called `CombiningLogger` which lets you combine multiple loggers at once. This allows you to build smaller and more specialised loggers instead of one big bloated implementation:

<pre><code>let sendMessageToSlack line =
    line |&gt; ignore // Implement here

let sendEmailToTeam line =
    line |&gt; ignore // Implement here

type SlackLogger() =
    interface Logger with
        member __.Log level line =
            if level = LogLevel.Fatal then sendMessageToSlack line

type EmailNotifier() =
    interface Logger with
        member __.Log level line =
            if level &gt;= LogLevel.Error then sendEmailToTeam line

let customConfig =
    { defaultConfig with
        errorHandler = customErrorHandler
        logger = CombiningLogger(
            [SlackLogger()
             EmailNotifier()
             OutputWindowLogger(LogLevel.Info)]) }</code></pre>

Again, there is no limit to what you can do with the logger implementation.

A popular logging framework in F# is [Logary](https://github.com/logary/logary) and there is even an [adapter for Suave](https://www.nuget.org/packages/Logary.Adapters.Suave/) available. Funnily the official documentation for the Suave adapter is a [bunch of Lorem Ipsum](https://logary.github.io/adapters/suave/). Maybe it's the whole F# community which doesn't like documentation that much :) ? While this page is clearly still in progress you can find a [simple example](https://suave.io/logs.html) on the official Suave website in the meantime.

As you can see the Suave web framework can be very simple and powerful when you know how it works. I have to admit that the source code is well written, concise and often self explaining. Once you are familiar with the framework you will quickly find what you need, but until then you might be missing some general pointers to main topics which are common with every modern web application.