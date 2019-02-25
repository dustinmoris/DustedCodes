<!--
	Tags: giraffe aspnet-core fsharp
-->

# Functional ASP.NET Core part 2 - Hello world from Giraffe

This is a follow up blog post on the [functional ASP.NET Core](https://dusted.codes/functional-aspnet-core) article from about two months ago. First of all I'd like to say that this has been the longest period I haven't published anything new to my blog since I started blogging in early 2015. The reason is because I have been pretty busy with a private project which I hope to write more about in the future, but more importantly I have been extremely busy organising my own wedding which took place at the end of last month :). Yes!, I've been extremely lucky to have found the love of my life and best friend and after being engaged for almost a year and a half we've finally tied the knot two weeks ago. Normally I don't blog about my private life here, but since this has been such a significant moment in my life I thought I should mention a few words here as well and let everyone know that the quiet time has been for a good reason and will not last for much longer now.

While this has primarily occupied the majority of my time I was also quite happy to see my [functional ASP.NET Core project](https://github.com/dustinmoris/Giraffe) receive recognition from the community and some really great support from other developers who've been helping me in adding lots of new functionality since then. In this first blog post after my small break I thought I'd take the opportunity and showcase some of the work we've done since the initial release and explain some of the design decisions behind some features.

But first I shall say that the framework has been renamed to **Giraffe**.

## ASP.NET Core Lambda is now Giraffe

ASP.NET Core Lambda was a good name in terms of being very descriptive of what it stood for, but at the same time there were plenty of other issues which led me and other people believe that a different name would be a better fit.

Initially I named the project ASP.NET Core Lambda, because, at its core it was a functional framework built on top of (and tightly integrated with) ASP.NET Core, so I put one and one together and went with that name.

However, it quickly became apparent that &quot;ASP.NET Core Lambda&quot; wasn't a great name for the following reasons:

- ASP.NET Core Lambda is a bit of a tongue twister.
- &quot;ASP&quot;, &quot;.NET&quot;, &quot;Core&quot; and &quot;Lambda&quot; are extremely overloaded words with more than one meaning. If the project turns out to be successful then any type of search or information lookup (e.g StackOverflow) would be an absolute nightmare with this name.
- Specifically Lambda is associated with [Amazon's serverless cloud offering](https://aws.amazon.com/lambda/) which would add even more to the confusion.
- Finally the name is not very tasteful. Let's be honest, the mix of capitalized and pascal cased words, the additional whitespace and the dot in the word makes the name look very busy and simply doesn't resemble an elegant or tasteful product.

As a result I decided to rename the project to something different and [put the name up for a vote](https://github.com/dustinmoris/Giraffe/issues/15), which ultimately led to **Giraffe**. Looking back I think it was a great choice and I would like to thank everyone who helped me in picking the new name, as well as suggesting other great names which made the decision not easy at all.

I think Giraffe is a much better name now, because it is short, it is very clear and distinctive and there is no ambiguity around the spelling or pronunciation. There is also no other product called Giraffe in the .NET space and not really anything else which it could be mistaken with. The name Giraffe also hasn't been taken as a NuGet package which made things really easy. On top of that Giraffe gave lots of creative room for creating a beautiful logo for which I used [99designs.co.uk](https://99designs.co.uk/). I set up a design challenge there and the winner impressed with this clever design:

![Giraffe Logo](https://raw.githubusercontent.com/dustinmoris/Giraffe/develop/giraffe.png)

Now I can only hope that the product will live up to this beautiful logo and the new name, which brings me to the actual topic of this blog post.

## Overview of new features

There has been quite a few changes and new features since my last blog post and there's a few of which I am very excited about:

- [Dotnet new template](#dotnet-new-template)
- [Nested routing](#nested-routing)
- [Razor views](#razor-views)
- [Functional HTML view engine](#functional-html-view-engine)
- [Content negotiation](#content-negotiation)
- [Model binding](#model-binding)

<h2 id="dotnet-new-template">Dotnet new template</h2>

One really cool thing you can do with the new .NET tooling is to create [project templates](https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates) which can be installed via NuGet packages.

Thanks to [David Sinclair](https://github.com/dsincl12) you can install a Giraffe template by running the following command:

<pre><code>dotnet new -i giraffe-template::*</code></pre>

This will install the [giraffe-template](https://www.nuget.org/packages/giraffe-template) NuGet package to your local templates folder.

Afterwards you can start using `Giraffe` as a new project type when running the `dotnet new` command:

<pre><code>dotnet new giraffe</code></pre>

This feature makes it significantly easier to get started with Giraffe now. The quickest way to get a working Giraffe application up and running is by executing these three commands:

1. `dotnet new giraffe`
2. `dotnet restore`
3. `dotnet run`

Everything should compile successfully and you should see a Hello-World Giraffe app running behind <a href="http://localhost:5000">http://localhost:5000</a>.

<h2 id="nested-routing">Nested routing</h2>

Another cool feature which has been added by [Stuart Lang](slang25) is nested routing.

The new `subRoute` handler allows users to create nested routes which can be very useful when logically grouping certain paths.

An example would be when an API changes it's authentication scheme and you'd want to group routes together which implement the same type of authentication. With the help of nested routing you can enable certain features like a new authentication scheme by only declaring it once per group:

<pre><code>let app =
    subRoute "/api"
        (choose [
            subRoute "/v1"
                (oldAuthentication >=> choose [
                    route "/foo" >=> text "Foo 1"
                    route "/bar" >=> text "Bar 1" ])
            subRoute "/v2"
                (newAuthentication >=> choose [
                    route "/foo" >=> text "Foo 2"
                    route "/bar" >=> text "Bar 2" ]) ])</code></pre>

In this example a request to `http://localhost:5000/api/v1/foo` will use `oldAuthentication` and a request to `http://localhost:5000/api/v2/foo` will end up using `newAuthentication`.

There is also a [`subRouteCi`](https://github.com/dustinmoris/Giraffe#subrouteci) handler which is the case insensitive equivalent of `subRoute`.

<h2 id="razor-views">Razor views</h2>

Next is the support of Razor views in Giraffe. [Nicol&aacute;s Herrera](https://github.com/nicolocodev) developed the first version of Razor views by utilising the [RazorLight](https://github.com/toddams/RazorLight) engine. Shortly after that I realised that by referencing the `Microsoft.AspNetCore.Mvc` NuGet package I can easily re-use the original Razor engine in order to offer a more complete and original Razor experience in Giraffe as well. While under the hood the engine changed from [RazorLight](https://www.nuget.org/packages/RazorLight/) to [ASP.NET Core MVC](https://github.com/aspnet/Mvc/tree/dev/src/Microsoft.AspNetCore.Mvc.Razor) the functionality remained more or less the same as implemented by Nicol&aacute;s in the first place.

In order to enable Razor views in Giraffe you have to register it's dependencies first:

<pre><code>type Startup() =
    member __.ConfigureServices (svc : IServiceCollection,
                                 env : IHostingEnvironment) =
        Path.Combine(env.ContentRootPath, "views")
        |> svc.AddRazorEngine
        |> ignore</code></pre>

After that you can use the `razorView` handler to return a Razor page from Giraffe:

<pre><code>let model = { WelcomeText = "Hello World" }

let app =
    choose [
        route "/" >=> razorView "text/html" "Index" model
    ]</code></pre>

The above example assumes that there is a `/views` folder in the project which contains an `Index.cshtml` file.

One of the parameters passed into the `razorView` handler is the mime type which should be returned by the handler. In this example it is set to `text/html`, but if the Razor page would represent something different (like an SVG image template for example) then with the `razorView` handler you can also set a different `Content-Type` as well.

In most cases `text/html` is probably the desired `Content-Type` of your response and therefore there is a second handler called `razorHtmlView` which does exactly that:

<pre><code>let model = { WelcomeText = "Hello World" }

let app =
    choose [
        route  "/" >=> razorHtmlView "Index" model
    ]</code></pre>

A more involved example with a layout page and a partial view can be found in the [SampleApp](https://github.com/dustinmoris/Giraffe/tree/develop/samples/SampleApp/SampleApp/views) project in the [Giraffe repository](https://github.com/dustinmoris/Giraffe).

### Using DotNet Watcher to reload the project on Razor page changes

If you come from an ASP.NET Core MVC background then you might be used to having Razor pages automatically re-compile on every page change during development, without having to manually restart an application. In Giraffe you can achieve the same experience by adding the [DotNet.Watcher.Tools](https://www.nuget.org/packages/Microsoft.DotNet.Watcher.Tools) to your `.fsproj` and put a watch on all `.cshtml` files:

<pre><code>&lt;ItemGroup&gt;
    &lt;DotNetCliToolReference Include=&quot;Microsoft.DotNet.Watcher.Tools&quot; Version=&quot;1.0.0&quot; /&gt;
&lt;/ItemGroup&gt;

&lt;ItemGroup&gt;
    &lt;Watch Include=&quot;**\*.cshtml&quot; Exclude=&quot;bin\**\*&quot; /&gt;
&lt;/ItemGroup&gt;</code></pre>

By adding the watcher to your project file you can start making changes to any `.cshtml` file in your project and immediately see the changes take effect during a running Giraffe web application (without having to manually restart the app).

### Dependency on Microsoft.AspNetCore.Mvc

One other thing which might sound a little bit strange is the dependency on the `Microsoft.AspNetCore.Mvc` NuGet package. It is essentially the full MVC library being referenced by Giraffe now and it has sparked a bit of confusion or disappointment amongst some users. Personally I think it really doesn't matter and I wanted to explain my thinking behind this design decision.

In order to get Razor views working in Giraffe there were three options available:

- Implement Giraffe's own Razor engine
- Use someone else's custom Razor engine
- Use the original Razor engine

I certainly did not have an appetite for the first option, which is hopefully understandable, and therefore was left with the choice between the latter two.

At the time of writing there was only one .NET Core compatible custom Razor engine available, which is [RazorLight](https://github.com/toddams/RazorLight). From what I know RazorLight is a very nice library and definitely highly recommended, but not necessarily the right choice for Giraffe.

When you ignore the name of the NuGet package for a second then there is really not much difference between referencing [RazorLight](https://www.nuget.org/packages/RazorLight/) or [Microsoft.AspNetCore.Mvc](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc) in Giraffe. Both require a new NuGet dependency in the project and both are a library which exposes some functionality to render Razor views. The ASP.NET Core MVC package might be slightly bigger and offer more functionality than what Giraffe actually needs, but that doesn't really matter, because Giraffe ignores the rest and only uses what is needed for the Razor support. I think it is pretty normal that any given library often implements far more functionality than what a single project actually makes use of.

In the case of Giraffe I was faced with a trade-off between a dependency which uses slightly more KBs disk space, but in return offers a complete and original Razor experience vs. a slightly smaller library which offers a custom implementation of Razor pages.

As far as I see this issue there is absolutely no disadvantage in Giraffe using the MVC NuGet package in order to get the original Razor experience in comparison to using any other Razor library. I also believe that this option is more in line with Giraffe's goal to be tightly integrated with the original ASP.NET Core experience. Users benefit by getting the original, well documented and understood Razor features which makes portability of existing Razor views also significantly easier.

For me it's really about making smart choices and I truly believe that the strength of Giraffe is by **standing on the shoulders of giants**, which is ASP.NET Core MVC in this case.

<h2 id="functional-html-view-engine">Functional HTML view engine</h2>

Speaking of the Razor view engine, another really cool feature which has been added to Giraffe is a new programmatic way of creating views. [Florian Verdonck](https://github.com/nojaf) helped me a lot with Giraffe over the last few weeks and one of his contributions was to port [Suave's experimental Html engine](https://github.com/SuaveIO/suave/blob/master/src/Experimental/Html.fs) to Giraffe.

I think the best way to describe the new `Giraffe.HtmlEngine` is by showing some code:

<pre><code>open Giraffe.HtmlEngine

let model = { Name = &quot;John Doe&quot; }

let layout (content: HtmlNode list) =
    html [] [
        head [] [
            title [] (encodedText &quot;Giraffe&quot;)
        ]
        body [] content
    ]

let partial () =
    p [] (encodedText &quot;Some partial text.&quot;)

let personView model =
    [
        div [] [
                h3 [] (sprintf &quot;Hello, %s&quot; model.Name |&gt; encodedText)
            ]
        div [] [partial()]
    ] |&gt; layout

let app =
    choose [
        route &quot;/&quot; &gt;=&gt; (personView model |&gt; renderHtml)
    ]</code></pre>

This examples demonstrates well how easy it is to create complex views with features like layout pages, partial views and model binding. There's really nothing that can't be done with this programmatic way of defining view-model pages and if you think there's something missing then it is really easy to extend as well.

Kudos to the Suave guys for coming up with this brilliant view engine and thanks to Florian for suggesting this feature as well as liaising with the Suave guys and porting the code to Giraffe!

<h2 id="content-negotiation">Content negotiation</h2>

The next feature which I'd like to show off is something which I did myself for a change. When exposing a web service endpoint you often want to respect the client's requested response type which is typically communicated via the HTTP Accept header.

For example a client might send the following information with a HTTP Accept header:

<pre><code>Accept: application/xml,application/json,text/html;q=0.8,text/plain;q=0.9,*/*;q=0.5</code></pre>

In this example the client says the following:

- Please give me either an XML or JSON response, both is my most preferred choice
- If you don't speak XML or JSON, then I'd like plain text as the next best option
- If you don't speak plain text either, then please just send the response in HTML
- If that doesn't suit you either, then just give me whatever you have

In Giraffe you can use the `negotiate` and `negotiateWith` handlers to return the client the best matching response based on the information passed through the request's `Accept` header.

The `negotiate` handler is very simple and speaks JSON, XML and plain text at the moment:

<pre><code>[&lt;CLIMutable&gt;]
type Person =
    {
        FirstName : string
        LastName  : string
    }
    override this.ToString() =
        sprintf &quot;%s %s&quot; this.FirstName this.LastNam

let app =
    choose [
        route  &quot;/foo&quot; &gt;=&gt; negotiate { FirstName = &quot;Foo&quot;; LastName = &quot;Bar&quot; }
    ]</code></pre>

By default the `negotiate` handler will check the request's `Accept` header and automatically serialize the model in either JSON, XML or plain text. If the client asks for `plain/text` then the `negotiate` handler will use the model's `ToString()` method otherwise it will use a JSON or XML serializer. Other mime types like `text/html` are not supported out of the box, because there is no default way to serialize an object into HTML.

However, if you want to support a wider range of accepted mime types then you can use the `negotiateWith` handler to set custom negotiation rules.

Let's assume you want to support two additional mime types, `application/x-protobuf` for [Google's Protocol Buffers](https://github.com/google/protobuf) serialization and `application/octet-stream` for generic binary serialization.

First you would want to implement two new `HttpHandler` functions which can return a response of those exact types:

<pre><code>let serializeProtobuf x =
    // Implement protobuf serialization

let serializeBinary x =
    // Implement binary serialization

let protobuf (dataObj : obj) =
    setHttpHeader &quot;Content-Type&quot; &quot;application/x-protobuf&quot;
    &gt;=&gt; setBodyAsString (serializeProtobuf dataObj)

let binary (dataObj : obj) =
    setHttpHeader &quot;Content-Type&quot; &quot;application/octet-stream&quot;
    &gt;=&gt; setBodyAsString (serializeBinary dataObj)</code></pre>

Then you can use the two new `HttpHandler` functions to set up custom negotiation rules and use them with the `negotiateWith` handler:

<pre><code>let rules =
    dict [
        &quot;*/*&quot;                     , json
        &quot;application/json&quot;        , json
        &quot;application/xml&quot;         , xml
        &quot;application/x-protobuf&quot;  , protobuf
        &quot;application/octet-stream&quot;, binary
    ]

let model = { FirstName = &quot;Foo&quot;; LastName = &quot;Bar&quot; }

let app =
    choose [
        route  &quot;/foo&quot; &gt;=&gt; negotiateWith rules model
    ]</code></pre>

You might find it more convenient to create a new negotiate handler altogether, which will make it much less verbose to use the custom rules in subsequent routes:

<pre><code>let negotiate2 = negotiateWith rules

let app =
    choose [
        route  &quot;/foo&quot; &gt;=&gt; negotiate2 { FirstName = &quot;Foo&quot;; LastName = &quot;Bar&quot; }
    ]</code></pre>

Even though there's still loads of room for improvement, I think this might be just enough for a large quantity of web applications already.

<h2 id="model-binding">Model Binding</h2>

While the `Accept` HTTP header denotes what mime types a client understands (typically more than just one), the `Content-Type` HTTP header specifies which mime type a client/server has chosen to send a message with. This is very useful information when it comes to model binding.

Giraffe exposes five different model binding functions which can deserialize the content of a HTTP request into a strongly typed object. Four of them can bind a specific request type into a typed model and the fifth method picks the most appropriate model binding function based on the request's `Content-Type` header.

It's the easiest to demonstrate this with a quick example again. Let's assume we have the following record type in a web application:

<pre><code>[&lt;CLIMutable&gt;]
type Car =
    {
        Name   : string
        Make   : string
        Wheels : int
        Built  : DateTime
    }</code></pre>

Now I'd like to expose different endpoints which can be used to HTTP POST a car object to the web service:

<pre><code>open Giraffe.HttpHandlers
open Giraffe.HttpContextExtensions

let submitAsJson =
    fun (ctx : HttpContext) -&gt;
        async {
            let! car = ctx.BindJson&lt;Car&gt;()
            // Do stuff
        }

let submitAsXml =
    fun (ctx : HttpContext) -&gt;
        async {
            let! car = ctx.BindXml&lt;Car&gt;()
            // Do stuff
        }

let submitAsForm =
    fun (ctx : HttpContext) -&gt;
        async {
            let! car = ctx.BindForm&lt;Car&gt;()
            // Do stuff
        }

let submitAsQueryString =
    fun (ctx : HttpContext) -&gt;
        async {
            let! car = ctx.BindQueryString&lt;Car&gt;()
            // Do stuff
        }

let submitHowYouLike =
    fun (ctx : HttpContext) -&gt;
        async {
            let! car = ctx.BindModel&lt;Car&gt;()
            // Do stuff
        }

let webApp =
    POST &gt;=&gt;
        choose [
            route &quot;/json&quot;  &gt;=&gt; submitAsJson
            route &quot;/xml&quot;   &gt;=&gt; submitAsXml
            route &quot;/form&quot;  &gt;=&gt; submitAsForm
            route &quot;/query&quot; &gt;=&gt; submitAsQueryString
            route &quot;/any&quot;   &gt;=&gt; submitHowYouLike ]</code></pre>

As you can see from the example, the model binding functions are extension methods on the `HttpContext` object and require to open the `Giraffe.HttpContextExtensions` module.

The `ctx.BindJson<'T>()` function will always try to retrieve an object by deserializing data from JSON. The `ctx.BindXml<'T>()` function behaves the same way but will try to deserialize from XML. The `ctx.BindForm<'T>()` function will bind a model from a request which has a `Content-Type` of `application/x-www-form-urlencoded` (typically a POST request from a HTML form element).

Sometimes you might want to bind a model from a query string, which could not only come from a HTTP POST but also from any other HTTP verb request. In this instance the `ctx.BindQueryString<'T>()` function can be used to bind the values from a query string to a strongly typed model.

At last you might want to allow a client to submit an object via any of the above mentioned options on the same endpoint. In this case your endpoint has to pick the correct model binding based on the `Content-Type` HTTP header and this can be achieved with the `ctx.BindModel<'T>()` function.

Since all model binding functions are extension methods of the `HttpContext` type they can be used from anywhere in a web application where you have access to the `HttpContext` object, which in Giraffe's case is every single `HttpHandler` function.

## What's next?

There were quite a few breaking changes since the first release, but APIs are slowly maturing as I get more feedback and exposure of the framework. So far the library has been in an alpha stage and will probably remain for another few weeks before I get around to finish some more examples and test projects which will eventually lead to the beta phase.

Once the project is in beta I will try to focus my effort more on collecting a lot of additional feedback before I feel confident enough to declare the first RC and subsequently the official version 1.0.0.

Even though breaking changes are not always the end of the world I would like to avoid drastic fundamental changes (as seen recently) once the project has entered the first stable release. Therefore I have been fairly reluctant to prematurely label Giraffe beyond an alpha and will probably want to enjoy the freedom of breaking stuff for a tiny bit longer. At the end of the day it's about setting the right expectations and I don't help anyone by labeling v1.0.0 too early when I know there's still a fair bit of danger to potentially move stuff around.

However, having said that I do want to stress that the underlying system (ASP.NET Core and Kestrel) have been very stable for a while now and as long as you don't mind that a namespace or method might still change in the near future then Giraffe is absolutely fit for production. So please go ahead and give it a try if you like what you've seen in this blog post so far :).

This basically brings me to the end of this follow up article and I thought what better way to finish it off than by sharing some of our memories from our wonderful wedding (in case you ever wondered what a British-Indian/Austrian-Polish wedding looks like ;)).

It was a very long day, which started off with a civil ceremony in the morning...

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34654715395_505f30f403_o.png" alt="Civil ceremony in the morning of the day">

Followed by a traditional Hindu ceremony shortly after lunch...

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34654707095_29a78034d4_o.jpg" alt="Drums during groom entrance at Hindu ceremony">

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34524044991_34308216d4_o.jpg" alt="Groom entrance at Hindu ceremony">

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34492683892_f8caa7cafe_o.jpg" alt="Prayers at Hindu ceremony">

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34524041711_e19221ff3c_o.jpg" alt="Listening to Hindu priest cracking jokes">

I had no idea how much fun Hindu ceremonies can be! There's lots of really fun and merry traditions which take place as part of us getting married. Then there's also a bit of banter between the two families. One of those little traditions is that the bride's family has to steal the groom's shoes before the ceremony ends so that the groom can't leave the house and take his newly wedded wife away from her family - at least not without having to pay for getting his shoes back. Normally this results in a bit of shoe pulling between the bride's side and the groomsmen, but I think in our case it is fair to say that there was a bit of a cultural clash when someone from my family rugby tackled a guy who tried to sneak away with my shoes, lol...

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34271006590_4217b6d130_o.jpg" alt="Shoe fight">

Luckily nothing serious happened and after everyone had a great laugh we continued with the ceremony...

Until finally we were able to celebrate at the reception party in the evening...

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/33844938503_c21b395eed_o.png" alt="Entering at the reception party">

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34654706015_1a1bd052ab_o.jpg" alt="Cake cutting">

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34524043471_52399b9e9f_o.jpg" alt="Our first toast as a married couple">

Throughout the day our guests wrote us lovely (I think) messages on little papers and my family decided to throw all these messages into a wooden box with a nice bottle of Red, which we had to seal ourselves with nails and hammer. We are not allowed to open this box until in seven years time and then we can enjoy a nicely matured bottle of vino while reading all those wonderful memories from our big day. What a brilliant idea!

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34525341611_cce8067f4c_o.jpg" alt="Sealing box of memories">

We had a fantastic day and before everyone stormed to the dance floor there was even a pretty impressive surprise firework display...

<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2017-05-14/34492692812_5e79eb9549_o.jpg" alt="Surprise firework display">

Getting married was a lot of fun and it all worked out so much better than we could have hoped for :)

Now we are ready for new adventures...