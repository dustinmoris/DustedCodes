<!--
    Published: 2018-02-09 02:34
    Author: Dustin Moris Gorski
    Title: Announcing Giraffe 1.0.0
	Tags: giraffe aspnet-core fsharp dotnet-core web
-->
I am pleased to announce the release of [Giraffe 1.0.0](https://github.com/giraffe-fsharp/Giraffe/releases/tag/v1.0.0), a functional ASP.NET Core web framework for F# developers. After more than a year of building, improving and testing the foundations of Giraffe it makes me extremely happy to hit this important milestone today. With the help of [32 independent contributors](https://github.com/giraffe-fsharp/Giraffe/graphs/contributors), more than a hundred [closed GitHub issues](https://github.com/giraffe-fsharp/Giraffe/issues?q=is%3Aissue+is%3Aclosed) and an astonishing [79 merged pull requests](https://github.com/giraffe-fsharp/Giraffe/pulls?utf8=%E2%9C%93&q=is%3Apr+is%3Aclosed+is%3Amerged) (and counting) it is fair to say that Giraffe has gone through many small and big changes which made it what I believe one of the best functional web frameworks available today.

The release of Giraffe 1.0.0 continues with this trend and also brings some new features and improvements along the way:

## Streaming support

Giraffe 1.0.0 offers a new [streaming API](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#streaming) which can be used to stream (large) files and other content directly to a client.

A lot of work has been put into making this feature properly work like supporting conditional HTTP headers and range processing capabilities. On top of that I was even able to help iron out a [few bugs in ASP.NET Core MVC](https://github.com/aspnet/Mvc/issues/7208)'s implementation as well (loving the fact that ASP.NET Core is all open source).

## Conditional HTTP Headers

In addition to the new streaming API the [validation of conditional HTTP headers](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#conditional-requests) has been exposed as a separate feature too. The `ValidatePreconditions` function is available as a `HttpContext` extension method which can be used to validate `If-{...}` HTTP headers from within any http handler in Giraffe. The function will self determine the context in which it is called (e.g. `GET` `POST`, `PUT`, etc.) and return a correct result denoting whether a request should be further processed or not.

## Configuration of serializers

A much desired and important improvement was the ability to change the default implementation of [data serializers](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#serialization) and [content negotiation](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#content-negotiation). Giraffe 1.0.0 allows an application to configure the default JSON or XML serializer via ASP.NET Core's services container.

## Detailed XML documentation

For the first time Giraffe has detailed XML documentation for all public facing functions available:

<a href="https://www.flickr.com/photos/130657798@N05/40125759872/in/dateposted/" title="giraffe-xml-docs"><img src="https://farm5.staticflickr.com/4611/40125759872_92f6239fdf_z.jpg" alt="giraffe-xml-docs"></a>

Even though this is not a feature itself, it aims at improving the general development experience by providing better IntelliSense and more detailed information when working with Giraffe.

## Giraffe.Tasks deprecated

When Giraffe introduced the `task {}` CE for the first time it was a copy of the single file project [TaskBuilder.fs](https://github.com/rspeele/TaskBuilder.fs) written by [Robert Peele](https://github.com/rspeele). However, maintaining our own copy of the task CE is resource expensive and not exactly my personal field of expertise. Besides that, since the initial release Robert has made great improvements to TaskBuilder.fs whereas Giraffe's version has been lacking behind. When TaksBuilder.fs has been published to NuGet it felt like a good idea to deprecate `Giraffe.Tasks` and resort back to the original.

This allows me and other Giraffe contributors to focus more on the web part of Giraffe and let Robert do his excellent work on the async/task side of things. Otherwise nothing has changed and Giraffe will continue to build on top of `Task` and `Task<'T>`. If you use `Giraffe.Tasks` outside of a Giraffe web application then you can continue doing so by referencing `TaskBuilder.fs` instead.

Giraffe also continues to use exclusively the context insensitive version of the task CE (meaning all task objects are awaited with `ConfigureAwait(false)`). If you encouter type inference issues after the upgrade to Giraffe 1.0.0 then you might have to add an extra open statement to your F# file:

<pre><code>open FSharp.Control.Tasks.ContextInsensitive</code></pre>

This is normally not required though unless you have <code>do!</code> bindings in your code.

If you like the `task {}` computation expression then please go to the [official GitHub repository](https://github.com/rspeele/TaskBuilder.fs) and hit the star button to show some support!

## TokenRouter as NuGet package

[TokenRouter](https://github.com/giraffe-fsharp/Giraffe.TokenRouter) is a popular alternative to Giraffe's default routing API aimed at providing maximum performance. Given the complexity of TokenRouter and the fact that Giraffe already ships a default version of the routing API it made only sense to decouple the TokenRouter into its own repository.

This change will allow TokenRouter to become more independent and evolve at its own pace. TokenRouter can also benefit from having its own release cycle and be much bolder in introducing new features and breaking changes without affecting Giraffe.

If your project is using the TokenRouter API then you will need to add a new dependency to the `Giraffe.TokenRouter` NuGet package now. The rest remains unchanged.

## Improved documentation

At last I have worked on improving the official [Giraffe documentation](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md) by completely restructuring the document, providing a wealth of new information and focusing on popular topics by demand.

The documentation has also been broken out of the README, but remains as a Markdown file in the git repository for reasons which I hope to blog about in a separate blog post soon.

The complete list of changes and new features can be found in the [official release notes](https://github.com/giraffe-fsharp/Giraffe/releases/tag/v1.0.0).

Thank you for reading this blog and using Giraffe (and if you don't, then give it a try ;))!
