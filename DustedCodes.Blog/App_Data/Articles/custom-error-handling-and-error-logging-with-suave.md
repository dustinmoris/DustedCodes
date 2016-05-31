<!--
    Published: 2016-05-16 19:36
    Author: Dustin Moris Gorski
    Title: Custom error handling and error logging with Suave
    Tags: fsharp suave error-handling error-logging
-->
Some years ago when you wanted to develop a .NET web application it was almost given that it will run in IIS, but today we have a sheer amount of different web server technologies to our availability. If you are an F# developer or in the process of learning F# (like me) then you will most likely come across the [Suave](https://suave.io/) web framework.

Suave is a lightweight, non-blocking web server written in F#. It is fairly new to the .NET space, but works wonderfully well and is very idiomatic to functional paradigms. Even though it is still early days it has probably the coolest name and logo amongst its competitors already:

<img src="https://raw.githubusercontent.com/SuaveIO/resources/master/images/suave1.png" alt="Suave" class="two-third-width" />

After working with Suave for more than a month now I can say that I find it very nice and intuitive. It is a well designed web framework which is easy to work with and allows rapid development if you know how it works. However, if you don't know how it works then you might struggle to get anything done at all. This is exactly what happened to me when I started adopting the framework in the beginning. The documentation is almost non existent and if you look for any advice on how to implement certain things then you are probably better off by browsing the [GitHub repository](https://github.com/SuaveIO/suave) directly. The lack of documentation is not a mistake, but rather a concious design decision which the Suave team explains as following:

> In suave, we have opted to write a lot of documentation inside the code; so just hover the function in your IDE or use an assembly browser to bring out the XML docs.

Even though I found myself around I wish there would have been a little bit more documentation and more code examples which would have put me on the right track straight away. A common topic that everyone will have to think about at some point is proper error handling and error logging.

## Hello World in Suave

Before we get started we need a simple web application to begin with:

<pre><code>open Suave
open Suave.Operators
open Suave.Successful
open Suave.Filters

let app = 
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World!&quot;
    ]

[&lt;EntryPoint&gt;]
let main argv = 
    startWebServer defaultConfig app
    0</code></pre>

When I navigate to [http://localhost:8083](http://localhost:8083) I can see the &quot;Hello World!&quot; message.

When I try to browse a path which doesn't exist then Suave will not serve the request by default. For example [http://localhost:8083/foo](http://localhost:8083/foo) will not return anything.

It is not really part of error handling, but I thought it would be good to mention here as well. If you want Suave to return a 404 (or anything else) for non existing paths then you can append a generic fallback case to the end of the webpart options:

<pre><code>let app = 
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World!&quot;
        NOT_FOUND "Resource not found."
    ]</code></pre>

The `NOT_FOUND` webpart and other pre-defined client error responses can be found in the `Suave.RequestErrors` namespace.

Now that I covered that, let's look at error handling next.

## Error Handling in Suave

In order to test error handling I need a route which throws an exception:

<pre><code>let errorAction = 
    fun _ -&gt; async { return failwith &quot;Uncaught exception!!&quot; }

let app = 
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World!&quot;
        GET &gt;=&gt; path &quot;/error&quot; &gt;=&gt; errorAction
        NOT_FOUND &quot;Resource not found.&quot;
    ]</code></pre>

The [default Suave error handler](https://github.com/SuaveIO/suave/blob/releases/v1.x/src/Suave/Web.fs#L14) will return a 500 Internal Server error and a static message in plain text, unless you run your application locally in which case it will return the exception message in HTML.

If you wanted to change the default behaviour and provide a different implementation, then you can do this by setting a custom function of type `ErrorHandler` as part of the web server configuration.

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

The `JSON_ERROR` function is a custom helper function, which uses [Newtonsoft.Json](https://www.nuget.org/packages/newtonsoft.json/) to serialize an object and pipe it to the `INTERNAL_ERROR` function in combination with a mime type of &quot;application/json; charset=utf-8&quot;.

You could go one step further and examine the Accept header of the incoming HTTP request and return the error response in a mime type which is supported by the client. The `ctx` parameter which is of type `Suave.Http.HttpContent` has all the relevant information to make that distinction:

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

*The `XML_ERROR` function doesn't exist by default and needs to be defined similarly as the `JSON_ERROR` function from the previous example.*


This code is not full proof, but you get the idea of it. There is nothing you can't do with the error handler and you can provide any implementation which is 100% tailored to your application requirements. Let's move on to error logging.

## Custom logging in Suave

Just like the error handler is set in the web server configuration the logger is set as well, but with the small difference that the logger needs to be an object of type `Suave.Logging.Logger`.

The `Logger` interface has only one member for logging application events:

<pre><code>abstract member Log : LogLevel -&gt; (unit -&gt; LogLine) -&gt; unit</code></pre>

It's simple but enough to build powerful loggers:

<pre><code>type CustomLogger() =
    interface Logger with
        member __.Log level line =
            match level with
            | LogLevel.Fatal -&gt; pressTheRedButton
            | _ -&gt; logAsNormal level line

let customConfig =
    { defaultConfig with
        errorHandler = customErrorHandler
        logger = new CustomErrorLogger() }</code></pre>

*The two functions `pressTheRedButton` and `logAsNormal` do not exist and are used for illustrating a custom error logger.*

Suave also provides a few [default loggers](https://github.com/SuaveIO/suave/blob/releases/v1.x/src/Suave/Logging/Logger.fs) which can be used out of the box. There is a `ConsoleWindowLogger` and an `OutputWindowLogger` which do exactly what they say.

Additionally there is another useful logger called `CombiningLogger`.