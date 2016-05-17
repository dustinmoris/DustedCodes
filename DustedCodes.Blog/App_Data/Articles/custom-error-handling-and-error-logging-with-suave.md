<!--
    Published: 2016-05-16 19:36
    Author: Dustin Moris Gorski
    Title: Custom error handling and error logging with Suave
    Tags: fsharp suave error-handling error-logging
-->
Some years ago when you wanted to develop a .NET web application it was almost given that it will run in IIS, but today we have a sheer amount of different web server technologies to our availability. If you are an F# developer or in the process of learning F# (like me) then you will most likely come across the [Suave](https://suave.io/) web framework.

Suave is a lightweight, non-blocking web server written in F#. It is fairly new to the .NET space, but works wonderfully well and is very idiomatic to functional paradigms. Even though it is still early days it has probably the coolest name and logo amongst its competitors already:

<img src="https://raw.githubusercontent.com/SuaveIO/resources/master/images/suave1.png" alt="Suave" class="two-third-width" />

After working with Suave for more than a month now I can say that I find it very nice and intuitive. It is a well designed web framework which is easy to work with and allows rapid development if you know how it works. However, if you don't know how it works then you might struggle to get anything done at all. This is exactly what happened to me when I started adopting the framework in the beginning. The documentation is almost non existent and if you look for any advice on how to implement certain things then you are probably better off by browsing the [GitHub repository](https://github.com/SuaveIO/suave) directly. The lack of documentation is not a mistake, but rather a concious design decision which the Suave teams explains as following:

> In suave, we have opted to write a lot of documentation inside the code; so just hover the function in your IDE or use an assembly browser to bring out the XML docs.

Even though I found myself around I wish there would have been a little bit more documentation and more code examples which would have put me on the right track straight away. With this in mind I will try to cover proper error handling and error logging with Suave in this blog post.

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

This doesn't count as error handling yet, but I thought it would be good to mention here as well. If you want Suave to return a 404 (or anything else) for not found resources, then you have to add a fallback case to the end of the webpart options:

<pre><code>let app = 
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World!&quot;
        NOT_FOUND "Resource not found."
    ]</code></pre>

The `NOT_FOUND` webpart and other pre-defined client error responses can be found in the `Suave.RequestErrors` namespace.

Now that I covered that, let's look at error handling next.

## Error Handling in Suave

In order to test error handling I need a route which throws an exception:

<pre><code>let app = 
    choose [
        GET &gt;=&gt; path &quot;/&quot; &gt;=&gt; OK &quot;Hello World!&quot;
        GET &gt;=&gt; path &quot;/error&quot; &gt;=&gt; failwith &quot;Uncaught exception!!&quot;
        NOT_FOUND &quot;Resource not found.&quot;
    ]</code></pre>

The [default Suave error handler](https://github.com/SuaveIO/suave/blob/master/src/Suave/Web.fs#L14) will return a 500 Internal Server error and output the exception message in plain text.

If you wanted to change the default behaviour and provide a different implementation, then you can do this by providing your own error handler when configuring the web server. As per [API documentation](https://suave.io/api.html#server-configuration) you can set a function of type `ErrorHandler` in the `SuaveConfig`.

The `ErrorHandler` type is defined as the following:

<pre><code>type ErrorHandler = Exception -&gt; String -&gt; HttpContext -&gt; WebPart</code></pre>

In other words we can declare a new error handler and configure it like this:

<pre><code>let customErrorHandler ex msg ctx =
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

