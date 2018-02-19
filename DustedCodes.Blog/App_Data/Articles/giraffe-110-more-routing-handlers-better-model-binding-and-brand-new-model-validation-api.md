<!--
    Published: 2018-02-16 13:25
    Author: Dustin Moris Gorski
    Title: Giraffe 1.1.0 - More routing handlers, better model binding and brand new model validation API
	Tags: giraffe aspnet-core fsharp dotnet-core web
-->
Last week I announced the release of [Giraffe 1.0.0](https://github.com/giraffe-fsharp/Giraffe/releases/tag/v1.1.0), which (apart from some initial confusion around the transition to [TaskBuilder.fs](https://github.com/rspeele/TaskBuilder.fs)) went mostly smoothly. However, if you have thought that I would be chilling out much since then, then you'll probably be disappointed to hear that today I've released another version of Giraffe with more exciting features and minor bug fixes.

The release of [Giraffe 1.1.0](https://github.com/giraffe-fsharp/Giraffe/releases/tag/v1.1.0) is mainly focused around improving Giraffe's [routing API](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#routing), making [model binding](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#model-binding) more functional and adding a new [model validation API](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#model-validation).

Some of these features address some long requested functionality, so let's not waste any more time and get straight down to it.

<h2 id="routes-with-trailing-slashes">Routes with trailing slashes</h2>

Often I've been asked how to make Giraffe treat a route with a trailing slash equal to the same route without a trailing slash:

<pre><code>https://example.org/foo/bar
https://example.org/foo/bar/</code></pre>

According to the technical specification a [route with a trailing slash is not the same as a route without it](https://webmasters.googleblog.com/2010/04/to-slash-or-not-to-slash.html). A web server might want to serve a different response for each route and therefore Giraffe (rightfully) treats them differently.

However, it is not uncommon that a web application chooses to not distinguish between two routes with and without a trailing slash and as such it wasn't a surprise when I received multiple bug reports for Giraffe not doing this by default.

Before version 1.1.0 one would have had to specify two individual routes in order to make it work:

<pre><code>let webApp =
    choose [
        route &quot;/foo&quot;  &gt;=&gt; text &quot;Foo&quot;
        route &quot;/foo/&quot; &gt;=&gt; text &quot;Foo&quot;
    ]</code></pre>

Giraffe version 1.1.0 offers a new routing handler called `routex` which is similar to `route` except that it allows a user to specify `Regex` in the route declaration.

This makes it possible to define routes with more complex rules such as allowing an optional trailing slash:

<pre><code>let webApp =
    choose [
        routex &quot;/foo(/?)&quot; &gt;=&gt; text &quot;Foo&quot;
    ]</code></pre>

The `(/?)` regex pattern denotes that there can be exactly zero or one slash after `/foo`.

With the help of `routex` and `routeCix` (the case insensitive version of `routex`) one can explicitly allow trailing slashes (or other non-standard behaviour) in a single route declaration.

<h2 id="parameterised-sub-routes">Parameterised sub routes</h2>

Another request which I have seen on several occasions was a parameterised version of the `subRoute` http handler.

Up until Giraffe 1.0.0 there was only a `routef` and a `subRoute` http handler, but not a combination of both.

Imagine you have a localised application which requires a language parameter at the beginning of each route:

<pre><code>https://example.org/en-gb/foo
https://example.org/de-at/bar
etc.</code></pre>

In previous versions of Giraffe one could have used `routef` to parse the parameter and pass it into another `HttpHandler` function:

<pre><code>let fooHandler (lang : string) =
    sprintf &quot;You have chosen the language %s.&quot; lang
    |&gt; text

let webApp =
    choose [
        routef &quot;/%s/foo&quot; fooHandler
    ]</code></pre>

This was all good up until someone needed to make use of something like `routeStartsWith` or `subRoute` to introduce additional validation/authentication before invoking the localised routes:

<pre><code>let webApp =
    choose [
        // Doesn't require authentication
        routef &quot;/%s/foo&quot; fooHandler
        routef &quot;/%s/bar&quot; barHandler

        // Requires authentication
        requiresAuth &gt;=&gt; choose [
            routef &quot;/%s/user/%s/foo&quot; userFooHandler
            routef &quot;/%s/user/%s/bar&quot; userBarHandler
        ]
    ]</code></pre>

The problem with above code is that the routing pipeline will always check if a user is authenticated (and potentially return an error response) before even knowing if all subsequent routes require it.

The workaround was to move the authentication check into each of the individual handlers, namely the `userFooHandler` and the `userBarHandler` in this instance.

A more elegant way would have been to specify the authentication handler only one time before declaring all protected routes in a single group. Normally the `subRoute` http handler would make this possible, but not if routes have parameterised arguments at the beginning of their paths.

The new `subRoutef` http handler solves this issue now:

<pre><code>
let webApp =
    choose [
        // Doesn't require authentication
        routef &quot;/%s/foo&quot; fooHandler
        routef &quot;/%s/bar&quot; barHandler

        // Requires authentication
        subRoutef "%s-%s/user" (
            fun (lang, dialect) -&gt;
                // At this point it is already
                // established that the path
                // is a protected user route:
                requiresAuth
                &gt;=&gt; choose [
                    routef &quot;/%s/foo&quot; (userFooHandler lang dialect)
                    routef &quot;/%s/bar&quot; (userBarHandler lang dialect)
                ]
        )
    ]</code></pre>

The `subRoutef` http handler can pre-parse parts of a route and group a collection of cohesive routes in one go.

<h2 id="improved-model-binding-and-model-validation">Improved model binding and model validation</h2>

The other big improvements in Giraffe 1.1.0 were all around model binding and model validation.

The best way to explain the new model binding and validation API is by looking at how Giraffe has done model binding in previous versions:

<pre><code>[&lt;CLIMutable&gt;]
type Adult =
    {
        FirstName  : string
        MiddleName : string option
        LastName   : string
        Age        : int
    }
    override this.ToString() =
        sprintf &quot;%s %s&quot;
            this.FirstName
            this.LastName

    member this.HasErrors() =
        if this.Age &lt; 18 then Some &quot;Person must be an adult (age &gt;= 18).&quot;
        else if this.Age &gt; 150 then Some &quot;Person must be a human being.&quot;
        else None

module WebApp =
    let personHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) -&gt;
            let adult = ctx.BindQueryString&lt;Adult&gt;()
            match adult.HasErrors() with
            | Some msg -&gt; RequestErrors.BAD_REQUEST msg
            | None     -&gt; text (adult.ToString())

    let webApp _ =
        choose [
            route &quot;/person&quot; &gt;=&gt; personHandler
            RequestErrors.NOT_FOUND &quot;Not found&quot;
        ]</code></pre>

In this example we have a typical F# record type called `Adult`. The `Adult` type has an override for its `ToString()` method to output something more meaningful than .NET's default and an additional member called `HasErrors()` which checks if the provided data is correct according to the application's business rules (e.g. an adult must have an age of 18 or over).

There's a few problems with this implementation though. First you must know that the `BindQueryString<'T>` extension method is a very loose model binding function, which means it will create an instance of type `Adult` even if some of the mandatory fields (non optional parameters) were not present in the query string (or badly formatted). While this "optimistic" model binding approach has its own advantages, it is not very idiomatic to functional programming and requires additional `null` checks in subsequent code.

Secondly the model validation has been baked into the `personHandler` which is not a big problem at first, but means that there's a lot of boilerplate code to be written if an application has more than just one model to work with.

Giraffe 1.1.0 introduces [new http handler functions](https://github.com/giraffe-fsharp/Giraffe/blob/master/RELEASE_NOTES.md#110) which make model binding more functional. The new `tryBindQuery<'T>` http handler is a stricter model binding function, which will only create an instance of type `'T` if all mandatory fields have been provided by the request's query string. It will also make sure that the provided data is in the correct format (e.g. a numeric value has been provided for an `int` property of the model) before returning an object of type `'T`:

<pre><code>[&lt;CLIMutable&gt;]
type Adult =
    {
        FirstName  : string
        MiddleName : string option
        LastName   : string
        Age        : int
    }
    override this.ToString() =
        sprintf &quot;%s %s&quot;
            this.FirstName
            this.LastName

    member this.HasErrors() =
        if this.Age &lt; 18 then Some &quot;Person must be an adult (age &gt;= 18).&quot;
        else if this.Age &gt; 150 then Some &quot;Person must be a human being.&quot;
        else None

module WebApp =
    let adultHandler (adult : Adult) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) -&gt;
            match adult.HasErrors() with
            | Some msg -&gt; RequestErrors.BAD_REQUEST msg
            | None     -&gt; text (adult.ToString())

    let parsingErrorHandler err = RequestErrors.BAD_REQUEST err

    let webApp _ =
        choose [
            route &quot;/person&quot; &gt;=&gt; tryBindQuery&lt;Adult&gt; parsingErrorHandler None adultHandler
            RequestErrors.NOT_FOUND &quot;Not found&quot;
        ]</code></pre>

The `tryBindQuery<'T>` requires three parameters. The first is an error handling function of type `string -> HttpHandler` which will get invoked when the model binding fails. The `string` parameter in that function will hold the specific model parsing error message. The second parameter is an optional `CultureInfo` object, which will get used to parse culture specific data such as `DateTime` values or floating point numbers. The last parameter is a function of type `'T -> HttpHandler`, which will get invoked with the parsed model if model parsing was successful.

By using `tryBindQuery<'T>` there is no danger of encountering a `NullReferenceException` or the need of doing additional `null` check any more. By the time the model has been passed into the `adultHandler` it has been already validated against any data contract violations (e.g. all mandatory fields have been provided, etc.).

At this point the semantic validation of business rules is still embedded in the `adultHandler` itself. The `IModelValidation<'T>` interface can help to move this validation step closer to the model and make use of a more generic model validation function when composing the entire web application together:

<pre><code>[&lt;CLIMutable&gt;]
type Adult =
    {
        FirstName  : string
        MiddleName : string option
        LastName   : string
        Age        : int
    }
    override this.ToString() =
        sprintf &quot;%s %s&quot;
            this.FirstName
            this.LastName

    member this.HasErrors() =
        if this.Age &lt; 18 then Some &quot;Person must be an adult (age &gt;= 18).&quot;
        else if this.Age &gt; 150 then Some &quot;Person must be a human being.&quot;
        else None

    interface IModelValidation&lt;Adult&gt; with
        member this.Validate() =
            match this.HasErrors() with
            | Some msg -&gt; Error (RequestErrors.BAD_REQUEST msg)
            | None     -&gt; Ok this

module WebApp =
    let textHandler (x : obj) = text (x.ToString())
    let parsingErrorHandler err = RequestErrors.BAD_REQUEST err
    let tryBindQuery&lt;'T&gt; = tryBindQuery&lt;'T&gt; parsingErrorHandler None

    let webApp _ =
        choose [
            route &quot;/person&quot; &gt;=&gt; tryBindQuery&lt;Adult&gt; (validateModel textHandler)
        ]</code></pre>

By implementing the `IModelValidation<'T>` interface on the `Adult` record type we can now make use of the `validateModel` http handler when composing the `/person` route. This functional composition allows us to entirely get rid of the `adultHandler` and keep a clear separation of concerns.

First the `tryBindQuery<Adult>` handler will parse the request's query string and create an instance of type `Adult`. If the query string had badly formatted or missing data then the `parsingErrorHandler` will be executed, which allows a user to specify a custom error response for data contract violations. If the model could be successfully parsed, then the `validateModel` http handler will be invoked which will now validate the business rules of the model (by invoking the `IModelValidation.Validate()` method). The user can specify a different error response for business rule violations when implementing the `IModelValidation<'T>` interface. Lastly if the model validation succeeded then the `textHandler` will be executed which will simply use the object's `ToString()` method to return a `HTTP 200` text response.

All functions are generic now so that adding more routes for other models is just a matter of implementing a new record types for each model and registering a single route in the web application's composition:

<pre><code>let webApp _ =
    choose [
        route &quot;/adult&quot; &gt;=&gt; tryBindQuery&lt;Adult&gt; (validateModel textHandler)
        route &quot;/child&quot; &gt;=&gt; tryBindQuery&lt;Child&gt; (validateModel textHandler)
        route &quot;/dog&quot;   &gt;=&gt; tryBindQuery&lt;Dog&gt;   (validateModel textHandler)
    ]</code></pre>

Overall the new model binding and model validation API aims at providing a more functional counter part to [MVC's model validation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation), except that Giraffe prefers to use functions and interfaces instead of the `System.ComponentModel.DataAnnotations` attributes. The benefit is that data attributes are often ignored by the rest of the code while a simple validation function can be used from outside Giraffe as well. F# also has the benefit of having a better type system than C#, which means that things like the `[<Required>]` attribute have little use if there is already an `Option<'T>` type.

Currently this new improved way of model binding in Giraffe only works for query strings and HTTP form payloads via the `tryBindQuery<'T>` and `tryBindFrom<'T>` http handler functions. Model binding functions for JSON and XML remain with the "optimistic" parsing model due to the underlying model binding libraries (JSON.NET and `XmlSerializer`), but a future update with improvements for JSON and XML is planned as well.

In total you have the following new model binding http handlers at your disposal with Giraffe 1.1.0:

<table>
    <tr>
        <th>HttpHandler</th>
        <th>Description</th>
    </tr>
    <tr>
        <td><code>bindJson<'T></code></td>
        <td>Traditional model binding. This is a new http handler equivalent of `ctx.BindJsonAsync<'T>`.</td>
    </tr>
    <tr>
        <td><code>bindXml<'T></code></td>
        <td>Traditional model binding. This is a new http handler equivalent of `ctx.BindAsync<'T>`.</td>
    </tr>
    <tr>
        <td><code>bindForm<'T></code></td>
        <td>Traditional model binding. This is a new http handler equivalent of `ctx.BindFormAsync<'T>`.</td>
    </tr>
    <tr>
        <td><code>tryBindForm<'T></code></td>
        <td>New improved model binding. This is a new http handler equivalent of a new `HttpContext` extension method called `ctx.TryBindFormAsync<'T>`.</td>
    </tr>
    <tr>
        <td><code>bindQuery<'T></code></td>
        <td>Traditional model binding. This is a new http handler equivalent of `ctx.BindQueryString<'T>`.</td>
    </tr>
    <tr>
        <td><code>tryBindQuery<'T></code></td>
        <td>New improved model binding. This is a new http handler equivalent of a new `HttpContext` extension method called `ctx.TryBindQueryString<'T>`.</td>
    </tr>
    <tr>
        <td><code>bindModel<'T></code></td>
        <td>Traditional model binding. This is a new http handler equivalent of `ctx.BindModelAsync<'T>`.</td>
    </tr>
</table>

The new model validation API works with any http handler which returns an object of type `'T` and is not limited to `tryBindQuery<'T>` and `tryBindFrom<'T>` only.

## Roadmap overview

To round up this blog post I thought I'll quickly give you a brief overview of what I am planning to tackle next.

The next release of Giraffe is anticipated to be version 1.2.0 (no date set yet) which will mainly focus around improved authentication and authorization handlers (policy based auth support), better CORS support and hopefully better Anti-CSRF support.

After that if nothing else urgent comes up I shall be free to go over two bigger PRs in the Giraffe repository which aim at providing a [Swagger integration API](https://github.com/giraffe-fsharp/Giraffe/pull/218) and a [higher level API of working with web sockets](https://github.com/giraffe-fsharp/Giraffe/pull/182) in ASP.NET Core.