<!--
    Published: 2016-04-12 21:01
    Author: Dustin Moris Gorski
    Title: Asynchronous F# workflows in NancyFx
    Tags: nancyfx fsharp async
-->
I have been playing with [F#](http://fsharp.org/) quite a lot recently and to be honest I started to really like it. Besides the fact that F# is a very [powerful language](https://fsharpforfunandprofit.com/posts/why-use-fsharp-intro/) it is a lot of fun too! As a result I began to migrate one of my smaller web services from C# and ASP.NET MVC 5 to F# and [NancyFx](http://nancyfx.org/). Nancy as a .NET web framework works perfectly fine with any .NET language, but has certainly not been written with F# much in mind. While it works really well in C#, it can feel quite clunky with F# at some times. There are [other web frameworks](https://suave.io/) which are more idiomatic, but come with other tradeoffs instead.

However, when I started to migrate from C# to NancyFx and F# I had quite some difficulties to implement even the simplest things in the beginning. [Asynchronous routes](https://github.com/NancyFx/Nancy/wiki/Async) were one of those things. The fact that I am an F# beginner didn't help either. Unfortunately there is not a lot of documentation on NancyFx with F# available and therefore I thought I'd share some of my own examples in this blog post.

As I said before, some of the stuff is **really simple** and probably super easy for an experienced F# developer, but for me as a beginner it was not that obvious in the beginning.

A great start on NancyFx and F# is [Michal Franc's blog post](http://www.mfranc.com/blog/f-and-nancy-beyond-hello-world/), where he shows how to register a normal route in NancyFx with F#:

<pre><code>type MyModule() as this =
    inherit NancyModule()
    do this.Get.["/"] &lt;- fun ctx -&gt; 
        "Hello World" :&gt; obj</code></pre>

This code is not much different from the equivalent C# implementation. `MyModule` inherits from the base `NancyModule` class and assigns a function to the root path of the HTTP GET verb. The only difference between C# and F# is that I had to explicitly upcast the `string` value to an `object` to match the method's expected signature.

Registering an [asynchronous route](https://github.com/NancyFx/Nancy/wiki/Defining-routes) is slightly different. The function expects an additional input parameter for the cancellation token and returns a `Task` or `Task<T>` object instead:

<pre><code>do this.Get.["/foo", true] &lt;- fun ctx ct -&gt;
    Task.FromResult("bar" :&gt; obj)</code></pre>

This simple example works, but is not asynchronous, because `Task.FromResult` blocks on the current thread. What I really want to do is to call an [asynchronous workflow](https://msdn.microsoft.com/en-us/library/dd233250.aspx) from F# and execute it from an asynchronous route in NancyFx.

[Asynchronous workflows in F# are different from Tasks in C#](http://stackoverflow.com/questions/12708504/is-asynchronous-in-c-sharp-the-same-implementation-as-in-f#answer-12708955) and therefore need to be converted from an <code>Async&lt;'T&gt;</code> object into a <code>Task&lt;T&gt;</code> in NancyFx.

Luckily the .NET [Control.Async class](https://msdn.microsoft.com/en-us/library/ee370232.aspx) offers plenty of predefined methods to make the translation very easy.

With `Async.RunSynchronously` one can convert an async workflow into a C# Task and execute it on the current thread synchronously:

<pre><code>do this.Get.["/foo"] &lt;- fun ctx ct -&gt;
  async {
      return "bar" :&gt; obj
  }
  |&gt; Async.RunSynchronously</code></pre>

This works, but it is still not asynchronous. If you want to run the async workflow in a non-blocking fashion in Nancy then you can pipe it to `Async.StartAsTask` which runs it asynchronously and returns a completed task:

<pre><code>do this.Get.["/foo", true] &lt;- fun ctx ct -&gt;
  async {
      return "bar" :&gt; obj
  }
  |&gt; Async.StartAsTask</code></pre>

In every case I had to upcast the string value to an object to match the expected return type. The [Async class](https://msdn.microsoft.com/en-us/library/ee370232.aspx) is full of useful functions which can run and convert asynchronous workflows from F# to C# and vice-versa.

If you are building a Nancy application in F# then you might also be interested in [Fancy](https://github.com/simonhdickson/Fancy) or Tiny Blue Robots' [blog post](http://tinybluerobots.github.io/fsharp/2015/03/17/nancy-fsharp.html), where both show some neat tricks on how to make Nancy feel a bit more functional.

With this I am going back to my F# work and wish everyone a [happy F# day](https://fsharpforfunandprofit.com/posts/happy-fsharp-day/)!