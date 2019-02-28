<!--
	Tags: aspnet-core mvc error-pages error-logging
-->

# Error Handling in ASP.NET Core

Almost two years ago I wrote a blog post on [demystifying ASP.NET MVC 5 error pages and error logging](https://dusted.codes/demystifying-aspnet-mvc-5-error-pages-and-error-logging), which became one of my most popular posts on this blog. At that time of writing the issue was that there were an awful lot of choices on how to deal with unhandled exceptions in ASP.NET MVC 5 and no clear guidance or recommendation on how to do it the right way.

Fortunately with ASP.NET Core the choices have been drastically reduced and there is also a much better [documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling) on the topic itself. However, there's still a few interesting things to consider which I wanted to point out in this follow up blog post and clear up any remaining questions on ASP.NET Core error handling.

## ASP.NET Core <> MVC

First I thought it's worth mentioning that the relationship between [ASP.NET Core](https://www.asp.net/core) and [ASP.NET Core MVC](https://github.com/aspnet/Mvc) hasn't changed much since what it used to be in Classic ASP.NET.

ASP.NET Core remains the main underlying platform for building web applications in .NET Core while MVC is still an optional web framework which can be plugged into the ASP.NET Core pipeline. It's basically a NuGet library which sits on top of ASP.NET Core and offers a few additional features for the [Model-View-Controller design pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller).

What that means in terms of error handling is that any exception handling capability offered by MVC will be limited to MVC. This will become much more apparent when we look at the ASP.NET Core architecture.

## ASP.NET Core middleware

ASP.NET Core is completely modular and the request pipeline is mainly defined by the installed middleware in an application.

For better demonstration let's create a new boilerplate MVC application and check out the `void Configure(..)` method inside the `Startup.cs` class file:

<pre><code>public void Configure(
    IApplicationBuilder app,
    IHostingEnvironment env,
    ILoggerFactory loggerFactory)
{
    loggerFactory.AddConsole(Configuration.GetSection(&quot;Logging&quot;));
    loggerFactory.AddDebug();

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
        app.UseBrowserLink();
    }
    else
    {
        app.UseExceptionHandler(&quot;/Home/Error&quot;);
    }

    app.UseStaticFiles();

    app.UseIdentity();

    app.UseMvc(routes =&gt;
    {
        routes.MapRoute(
            name: &quot;default&quot;,
            template: &quot;{controller=Home}/{action=Index}/{id?}&quot;);
    });
}</code></pre>

Because that is a lot of boilerplate code for a simple web application I'll trim it down to the main points of interest:

<pre><code>app.UseExceptionHandler(&quot;/Home/Error&quot;);
app.UseStaticFiles();
app.UseIdentity();
app.UseMvc(routes =&gt; ...);</code></pre>

What you can see here is fairly self explanatory, but there's a few key things to understand from this code. The `app.Use...()` (extension-) method calls are enabling several middleware by registering them with the `IApplicationBuilder` object. Each middleware will be made responsible for invoking the next middleware in the request pipeline, which is why the order of the `app.Use...()` method calls matter.

For example this is a rough skeleton of the [StaticFileMiddleware](https://github.com/aspnet/StaticFiles/blob/master/src/Microsoft.AspNetCore.StaticFiles/StaticFileMiddleware.cs):

<pre><code>public StaticFileMiddleware(RequestDelegate next, ...)
{
    // Some stuff before

    _next = next;

    // Some stuff after
}

public Task Invoke(HttpContext context)
{
    // A bunch of code to see if this middleware can
    // serve a static file that matches the HTTP request...

    // If not the code will eventually reach this line:
    return _next(context);
}</code></pre>

I cut out some noise to highlight the usage of the `RequestDelegate` variable.

As you can see each middleware must accept a `RequestDelegate` object in the constructor and each middleware must implement a method of type `Task Invoke(HttpContext context)`.

The `RequestDelegate` is, as its name suggest, a delegate which represents the next middleware in the lifecycle. ASP.NET Core defers the responsibility of invoking it to the current middleware itself. For example if the `StaticFileMiddleware` is not able find a static file which matches the incoming HTTP request then it will invoke the next middleware by calling `return _next(context);` at the end. On the other hand if it was able to find the requested static file then it will return it to the client and never invoke the next or any subsequent middleware anymore.

This is why the order of the `app.Use...()` method calls matter. When you think about it the underlying pattern can be seen a little bit like an onion:

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-01-19/31564566283_dca040a066_o.png" alt="aspnet-core-middleware-onion-architecture, Image by Dustin Moris Gorski">

A HTTP request will travel from the top level middleware down to the last middleware, unless a middleware in between can satisfy the request and return a HTTP response earlier to the client. In contrast an unhandled exception would travel from the bottom up. Beginning at the middleware where it got thrown it would bubble up all the way to the top most middleware waiting for something to catch it.

In theory a middleware could also attempt to make changes to the response *after* it has invoked the next middleware, but this is normally not the case and I would advise against it, because it could result in an exception if the other middleware already wrote to the response.

### Error handling should be the first middleware

With that in mind it is clear that in order to catch any unhandled exception an error handling middleware should be the first in the pipeline. Only then it can guarantee a final catch if nothing else caught the exception before.

Because MVC is typically registered towards the end of the middleware pipeline it is also clear that exception handling features (like the infamous [ExceptionFilters](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#exception-filters)) within MVC will not be able to catch every exception.

For more information on middleware please check out the [official documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware).

## Custom Exception Handlers

Now that middleware and exception handling hopefully makes sense I also wanted to quickly show how to create your own global exception handler in ASP.NET Core.

Even though there is already quite a few useful [exception handlers](http://www.talkingdotnet.com/aspnet-core-diagnostics-middleware-error-handling/) in the [Microsoft.AspNetCore.Diagnostics](https://www.nuget.org/packages/Microsoft.AspNetCore.Diagnostics/) NuGet package available, it still might make sense to create your own one as well. For example one might want to have an exception handler which logs critical exceptions to [Sentry](https://sentry.io/welcome/) by using Sentry's [Raven Client for .NET](https://github.com/getsentry/raven-csharp) or one might want to implement an integration with a bug tracking tool and log a new ticket for every `NullReferenceException` that gets thrown. Another option would be an integration with [elmah.io](https://elmah.io/).

There is many good reasons why someone might want to create additional exception handlers and it might even be useful to have multiple exception handlers registered at once. For example the first exception handler logs a ticket in a bug tracking system and re-throws the original exception. Then the next exception handler could log the error in ELMAH and re-trigger the original exception again. The final exception handler might catch the exception and return a friendly error page to the client. By having each exception handler focusing on a single responsibility they automatically become more re-usable across multiple projects and it would also enable to use different combinations on different environments (think dev/staging/production).

A good example of writing your own exception handling middleware is the default [ExceptionHandlerMiddleware](https://github.com/aspnet/Diagnostics/blob/master/src/Microsoft.AspNetCore.Diagnostics/ExceptionHandler/ExceptionHandlerMiddleware.cs) in ASP.NET Core.

A default exception handler boilerplate would look like this:

<pre><code>using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SomeApp
{
    public sealed class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.
                    CreateLogger&lt;CustomExceptionHandlerMiddleware&gt;();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    // Do custom stuff
                    // Could be just as simple as calling _logger.LogError

                    // if you don't want to rethrow the original exception
                    // then call return:
                    // return;
                }
                catch (Exception ex2)
                {
                    _logger.LogError(
                        0, ex2,
                        &quot;An exception was thrown attempting &quot; +
                        &quot;to execute the error handler.&quot;);
                }

                // Otherwise this handler will
                // re -throw the original exception
                throw;
            }
        }
    }
}</code></pre>

In addition to the `RequestDelegate` the constructor also accepts an `ILoggerFactory` which can be used to instantiate a new `ILogger` object.

In the `Task Invoke(HttpContext context)` method the error handler basically does nothing other than immediately calling the next middleware. Only if an exception is thrown it will come into action by capturing it in the `catch` block. What you put into the `catch` block is up to you, but it would be good practice to wrap any non trivial code in a second try-catch block and default back to basic logging if everything else is falling apart.

I hope all of this made sense and that this blog post was useful again. Personally I find it extremely nice to see how well ASP.NET Core has evolved from its predecessor. If you have any more questions just drop me a comment below.