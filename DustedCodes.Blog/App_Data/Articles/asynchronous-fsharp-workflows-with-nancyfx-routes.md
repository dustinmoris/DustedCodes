<!--
    Published: 2016-03-30 21:01
    Author: Dustin Moris Gorski
    Title: Asynchronous F# workflows with NancyFx
    Tags: nancyfx fsharp async
-->
I have been playing with [F#](http://fsharp.org/) quite a lot recently and started to really like it. Besides the fact that F# is a very [powerful language](https://fsharpforfunandprofit.com/posts/why-use-fsharp-intro/) it is a lot of fun too! As a result I began to migrate one of my smaller web services from C# and ASP.NET MVC 5 to F# and [NancyFx](http://nancyfx.org/). Nancy as a .NET web framework works perfectly fine with any .NET language, but has certainly not been written with F# much in mind. While it works really well in C#, it can feel quite clunky in F# at some times. There are [other web frameworks](https://suave.io/) which are more idiomatic, but come with other tradeoffs instead.

However, when I started to migrate from ASP.NET in C# to NancyFx in F# I had quite some difficulties to implement even the simplest things in the beginning. [Asynchronous routes]() were one of those. The fact that I am an F# beginner didn't help either. Unfortunately there is not a lot of documentation or examples on NancyFx with F# available and therefore I wanted to share some of my own examples in this blog post.

As I said before, some of the stuff is really simple and probably super easy for an experienced F# developer, but for me as a beginner it took me some time to figure it out and I am sure that someone else will run into the same questions one day as well. I hope this blog post can be of help then.

A great start on NancyFx and F# was [Michal Franc's blog post](http://www.mfranc.com/blog/f-and-nancy-beyond-hello-world/). Registering a normal route in F# is very much like in C#:

<pre><code>type MyModule() as this =
    inherit NancyModule()
    do this.Get.["/"] &lt;- fun ctx -&gt; 
        "Hello World" :&gt; obj</code></pre>

`MyModule` inherits from the base `NancyModule` and assigns a function delegate to the root path of the HTTP GET verb. The only difference between C# and F# is that I had to explicitly upcast the `string` value to an `object`, because the method signature expects an `object` as return type.

Registering an [asynchronous route]() is slightly different. The function takes in an additional parameter for the cancellation token and returns a `Task` or `Task<T>`.

In simple terms I could write the following endpoint, except that `Task.FromResult` is not asynchronous and will block on the current thread:

<pre><code>do this.Get.["/foo", true] &lt;- fun ctx ct -&gt;
    Task.FromResult("bar" :&gt; obj)</code></pre>

What I really want to do is to call an asynchronous workflow instead. If you are an F# beginner like me you might want to know that F# async workflows are not the same as C# tasks. Async workflows are ...


<pre><code>do this.Get.["/foo"] &lt;- fun ctx ct -&gt;
  async {
      return ""
  }
  |> Async.RunSynchronously
  :> obj</-></code></pre>

<pre><code>do this.Get.["/test", true] <- fun=fun ctx=ctx ct=ct -=->
  async {
      return "" :> obj
  }
  |> Async.StartAsTask</-></code></pre>
