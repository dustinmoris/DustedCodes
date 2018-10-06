<!--
    Tags: nancyfx architecture
-->

# How to make an IStatusCodeHandler portable in NancyFx

I am currently working on several micro services using the [NancyFx](http://nancyfx.org/) framework with many projects sharing the same underlying architecture.

The [IStatusCodeHandler](https://github.com/NancyFx/Nancy/blob/master/src/Nancy/IStatusCodeHandler.cs) interface is one of the core infrastructure components which is used by many Nancy projects to intercept and process a Nancy context before the final response gets returned to the client.

Having a few identical implemenations of IStatusCodeHandler I wanted to extract them into a re-usable NuGet package.

The problem is that Nancy automagically detects all implementations of IStatusCodeHandler and wires them up in the Nancy pipeline. In other words, if a library exposes an IStatusCodeHandler implementation then it gets automatically hooked into your Nancy application just by adding a reference to the assembly.

This is a nice feature, but makes it more difficult to include an IStatusCodeHandler in a shared library. I certainly don't want to modify an application's behaviour by simply adding a reference to the project.

Fortunately with some additional wiring you can easily expose an IStatusCodeHandler in a disabled state and allow your application to enable it only when required.

Here is an example of how to make an IStatusCodeHandler portable:

<pre><code>public class NotFoundStatusCodeHandler : IStatusCodeHandler
{
    private static bool _isEnabled = false;

    public static void Enable()
    {
        _isEnabled = true;
    }

    public bool HandlesStatusCode(
        HttpStatusCode statusCode,
        NancyContext context)
    {
        return _isEnabled && statusCode == HttpStatusCode.NotFound;
    }

    public void Handle(HttpStatusCode statusCode, NancyContext context)
    {
        // Do work
    }
}</code></pre>

I added a static Boolean field `_isEnabled` to the class definition. The field is initialised with `false` and only the `Enable()` method changes the value to be `true`.

Inside the `IStatusCodeHandler.HandlesStatusCode` method we check for the `_isEnabled` flag before evaluating the rest of the condition.

This will not stop Nancy from detecting and adding the IStatusCodeHandler into the pipeline, but will essentially suppress its behaviour entirely.

Enabling the handler is as easy as adding one line of code to your application startup event:

<pre><code>public class Bootstrapper : DefaultNancyBootstrapper
{
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
        base.ApplicationStartup(container, pipelines);

        NotFoundStatusCodeHandler.Enable();
    }
}</code></pre>

The fact that the `_isEnabled` flag is static is not a problem, because it would either never change or only once during the application startup and remain that way for the rest of the application's lifetime.

It is a very simple but effective trick to make IStatusCodeHandler classes re-usable across multiple projects.
