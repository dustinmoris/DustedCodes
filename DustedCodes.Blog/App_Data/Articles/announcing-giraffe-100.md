<!--
    Published: 2018-02-09 02:34
    Author: Dustin Moris Gorski
    Title: Announcing Giraffe 1.0.0
	Tags: giraffe aspnet-core fsharp dotnet-core web
-->
I am pleased to announce the release of [Giraffe 1.0.0](https://github.com/giraffe-fsharp/Giraffe/releases/tag/v1.0.0), a functional ASP.NET Core web framework for F# developers. After more than a year of building, improving and testing the foundations of Giraffe it makes me extremely happy to hit this important milestone today. With the help of [32 independent contributors](https://github.com/giraffe-fsharp/Giraffe/graphs/contributors), more than a hundred [closed GitHub issues](https://github.com/giraffe-fsharp/Giraffe/issues?q=is%3Aissue+is%3Aclosed) and an astonishing [79 merged pull requests](https://github.com/giraffe-fsharp/Giraffe/pulls?utf8=%E2%9C%93&q=is%3Apr+is%3Aclosed+is%3Amerged) (and counting) it is fair to say that Giraffe has gone through many big and small changes which made it to what I believe one of the best functional web frameworks available today.

The release of Giraffe 1.0.0 continues with this trend and also brings some new features and improvements along its way:

## Streaming support

Giraffe 1.0.0 offers a new [streaming API](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#streaming) which can be used to stream (large) files and other content directly to a client.

A lot of work has been put into making this feature properly work like supporting conditional HTTP headers or range processing functionality.  As a result of working on Giraffe's streaming functionality I was even able to highlight a [few bugs in ASP.NET Core MVC](https://github.com/aspnet/Mvc/issues/7208)'s implementation as well.

## Conditional HTTP Headers

In addition to the new streaming API the [validation of conditional HTTP headers](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#conditional-requests) has been exposed as a separate feature as well. The `ValidatePreconditions` function is available as a `HttpContext` extension method which can be used to validate `If-{...}` HTTP headers from within any http handler in Giraffe. The function will self determine the context in which it is called (e.g. `GET` `POST`, `PUT`, etc.) and return a correct result denoting whether a request should be further processed or not.

## Configuration of serializers

A much desired and important improvement was the ability to change the default implementation of [data serializers](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#serialization) and [content negotiation](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#content-negotiation). Giraffe 1.0.0 allows an application to configure the default JSON or XML serializer via ASP.NET Core's services container now.

## Detailed XML documentation

For the first time Giraffe has detailed XML documentation for all public facing functions available:

<a href="https://www.flickr.com/photos/130657798@N05/40125759872/in/dateposted/" title="giraffe-xml-docs"><img src="https://farm5.staticflickr.com/4611/40125759872_92f6239fdf_z.jpg" alt="giraffe-xml-docs"></a>

Even though this is not a feature itself, it aims at improving the general development experience by providing better IntelliSense and more detailed information at a developer's hand when building a Giraffe web application.

## Giraffe.Tasks deprecated

When Giraffe introduced the `task {}` CE for the first time it was a copy of the single file project [TaskBuilder.fs](https://github.com/rspeele/TaskBuilder.fs) written by [Robert Peele](https://github.com/rspeele). However, maintaining our own copy of the task CE is resource expensive and not exactly my personal field of expertise. Besides, since the initial release Robert has made great improvements to TaskBuilder.fs whereas Giraffe's version has been lacking behind. When TaksBuilder.fs has been published to NuGet it felt like a good idea to deprecate `Giraffe.Tasks` and resort back to the original.

This allows me and other Giraffe contributors to focus more on the web part of Giraffe again and let Robert do his excellent work on the async/task side of things. Nothing has changed otherwise and Giraffe will continue to build on top of `Task` and `Task<'T>`. If you used `Giraffe.Tasks` outside of Giraffe you can continue doing so by referencing `TaskBuilder.fs`. If you like the `task {}` computation expression then please go to the [official GitHub repository](https://github.com/rspeele/TaskBuilder.fs) and hit the star button to show this project some love!

## TokenRouter as NuGet package

[TokenRouter](https://github.com/giraffe-fsharp/Giraffe.TokenRouter) is a popular alternative to Giraffe's default routing API aimed at providing maximum performance. Given the complexity of TokenRouter and the fact that the core Giraffe library already ships a default version of the routing API it made only sense to free TokenRouter from it.

This change allows TokenRouter to become more independent and evolve at its own pace. It can also benefit from having its own release cycle and be much bolder in introducing new features and breaking changing without breaking the core Giraffe library.

If your project is using the TokenRouter API then you will need to add a new dependency to the `Giraffe.TokenRouter` NuGet package going forward from now.

## Improved documentation

At last I have worked on improving the official [Giraffe documentation](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md) by completely restructuring the document, providing a wealth of new information and focusing on popular topics by demand.

The documentation has also been broken out of the README, but remains as a Markdown file in the git repository for reasons which I hope to blog in a separate blog post soon.

The complete list of changes and new features can be found in the [official release notes](https://github.com/giraffe-fsharp/Giraffe/releases/tag/v1.0.0).

Thank you for reading this blog and using Giraffe (and if you don't, then give it a try ;))!