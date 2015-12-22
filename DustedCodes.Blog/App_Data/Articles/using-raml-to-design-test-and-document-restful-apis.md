<!--
    Published: 2015-12-05 22:15
    Author: Dustin Moris Gorski
    Title: Design, test and document RESTful APIs using RAML in .NET
    Tags: raml restful-api design testing dotnet
-->
Building a RESTful API is easy. Building a RESTful API which is easy to consume is more difficult. There are three key elements which make a good API:

-   Intuitive design
-   Good documentation
-   Documentation which actually matches the implementation

Intuitive API design is obviously very important, but equally important and often neglected is good and complete documentation which makes it easier to build against your API.

Having said that we all know how difficult it is to keep documentation up to date. It is very loosely coupled to the actual implementation and without any enforcement other than a human trial and error process.

In this article I would like to demonstrate how we can close this gap by building a RESTful API with a design- and test first approach using [RAML](http://raml.org/).

I will showcase an entire end to end scenario by building a simple demo API and covering the following steps:

1.   Design an API using RAML
2.   Generate a client from the RAML document
3.   Write tests using the auto-generated client
4.   Implement the API to satisfay the tests
5.   Document and review an API using the Anypoint Platform

But first let me briefly introduce you to RAML:

## RAML

[RAML](http://raml.org/) stands for RESTful API Modeling Language and *this* is exactly what it delivers.

If you have worked with [Swagger](http://swagger.io/) or [API Blueprint](https://apiblueprint.org/) before then this should be familiar, except that RAML is designed to be human readable and remarkably easy to use.

At the time of writing there are two public specifications:

-   [RAML 0.8](https://github.com/raml-org/raml-spec/blob/master/raml-0.8.md)
-   [RAML 1.0 RC](http://docs.raml.org/specs/1.0/)

In this blog post I will be using RAML 0.8 and assume that you are familiar enough with the spec to follow my simple examples as part of the demo.

For further reading I would recommend to go through the official RAML tutorials explaining the basic concepts and more advanced features in your own time and your own pace:

-   [RAML 100 Tutorial (Basics)](http://raml.org/developers/raml-100-tutorial)
-   [RAML 200 Tutorial (Advanced)](http://raml.org/developers/raml-200-tutorial)

Now that you know what RAML is I will jump straight into the first part where I'll be using RAML to design an API:

## 1. Design an API using RAML

As I mentioned earlier, for the purpose of this demo I would like to build a very rudimental fake parcel delivery API with the following two endpoints:

-   ***GET** /status/{parcelId}* will return the status of a parcel
-   ***PUT** /status/{parcelId}* will update the status of a parcel

RAML is a [YAML](http://www.yaml.org/) based language and designed for human readability. The beauty of this is that you can write RAML in any basic editor without any fancy syntax highlighting and it will still be easy to read and understand.

However, there is a really good [Atom](https://atom.io/) plugin called [API Workbench](http://apiworkbench.com/) which I am using to kick start my API:

<pre><code>#%RAML 0.8
title: Parcel Delivery API
version: v1
baseUri: http://localhost/raml-demo-api/{version}
protocols: [ HTTP, HTTPS ]</code></pre>

*Sidenote:
I will not go through every single line of the spec, but give some context so that you get the idea of where I am going with this API.*

At the top of the document I specified the RAML version, followed by the title of the API, the version and the basic URI with a version placeholder. This will allow me to introduce breaking changes in the future. The API shall also be called from both protocols, HTTP and HTTPS.

Next I define a single endpoint to set and get status information for a given parcel ID:

<pre><code>...
/status/{parcelId}:
  displayName: Parcel Status Information
  uriParameters:
    parcelId:
      displayName: Parcel ID
      type: string
      required: true
      minLength: 6
      maxLength: 6
      example: 123456</code></pre>

Now it is time to define the contract for the GET operation, which shall return the current status of a parcel:

<pre><code>...
  get:
    description: Retrieves the current status for the specified parcel ID.
    responses:
      200:
        description: Current status.
        body:
          application/json:
            schema: |
              {
                "$schema": "http://json-schema.org/draft-04/schema#",
                "title": "Delivery Status",
                "type": "object",
                "properties": {
                  "status": {
                    "description": "The current status of the delivery.",
                    "type": "string"
                  },
                  "updated": {
                    "description": "The date time the last status update.",
                    "type": "string"
                  }
                }
              }
            example: |
              {
                "status": "Parcel is out for delivery.",
                "updated": "2015-12-09T16:53:19.5168335+00:00"
              }</code></pre>

As you can see there is probably not much I have to explain. The GET operation expects a successful response with the 200 HTTP status code and a JSON payload containing the current status. Note how RAML allows me to provide an example alongside the schema for better understanding and later validation. This will be particularly useful at a later point.

I am pleased with this and therefore apply something similar for the PUT operation:

<pre><code>...
  put:
    description: Creates or updates the status for the specified parcel ID.
    body:
      application/json:
        schema: |
          {
            "$schema": "http://json-schema.org/draft-04/schema#",
            "title": "Status Update",
            "type": "object",
            "properties": {                 
              "status": {
                "description": "The new status update message.",
                "type": "string"
              }
            }
          } 
        example: |
          {
            "status": "Delivered and signed by customer."
          }
    responses:
      201:
        description: The status has been successfully updated.</code></pre>

The only difference is that the PUT operation has to supply a JSON object in the HTTP body and expects the [201 status code](http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.2.2) on success.

With this I am almost done designing the API. The only other thing I would like to add is to describe what happens if a consumer provides a wrong parcel ID.

The supplied parcel ID might either not exist or be in the wrong format.

Because this applies to both operations, the GET and the PUT on my endpoint, I will define two additional error responses as a so called [trait](http://docs.raml.org/specs/1.0/#raml-10-spec-resource-types-and-traits) which can be shared by multiple endpoints in RAML:

<pre><code>...
traits:
  <strong>- requiresValidParcelId:</strong>
      usage: |
        Apply this to any method which requires a valid Parcel ID in the request.
      responses:
        406:
          description: Parcel ID was in incorrect format.
          body:
            application/json:
              <strong>schema: ErrorMessage</strong>
              example: |
                {
                  "message": "Parcel ID has to be 6 characters long and may only contain digits."
                }
        404:
          description: Could not find the specified parcel ID.
          body:
            application/json:
              <strong>schema: ErrorMessage</strong>
              example: |
                {
                  "message": "Parcel ID not found."
                }</code></pre>

Every trait has a unique name. I named this one &quot;requiresValidParcelId&quot;.

If you've paid close attention then you might be asking yourself what &quot;schema: ErrorMessage&quot; means in the declaration of the response body.

Shared schemas is another feature of RAML. In this case I defined a schema called &quot;ErrorMessage&quot; which describes the payload in the error response:

<pre><code>...
schemas:
  - ErrorMessage: |
      {
        "$schema": "http://json-schema.org/draft-04/schema#",
        "title": "Error Message",
        "type": "object",
        "properties": {                 
          "message": {
            "description": "The error message of the error.",
            "type": "string"
          }
        }
      }</code></pre>

It is the same principle as before and nothing new to the way how we defined the success response before, only that it has been moved into a separate section in the RAML document which allows re-use for mutliple endpoints.

Last but not least I need to hook up the status endpoint with the trait:

<pre><code>...
/status/{parcelId}:
  displayName: Parcel Status Information
  uriParameters:
    parcelId:
      displayName: Parcel ID
      type: string
      required: true
      minLength: 6
      maxLength: 6
      example: 123456
  <strong>is: [ requiresValidParcelId ]</strong></code></pre>

The entire end result looks as follows:

<pre><code>#%RAML 0.8
title: Parcel Delivery API
version: v1
baseUri: https://raml-demo-api.azurewebsites.net/{version}
protocols: [ HTTP, HTTPS ]
schemas:
  - ErrorMessage: |
      {
        "$schema": "http://json-schema.org/draft-04/schema#",
        "title": "Error Message",
        "type": "object",
        "properties": {                 
          "message": {
            "description": "The error message of the error.",
            "type": "string"
          }
        }
      }
traits:
  - requiresValidParcelId:
      usage: |
        Apply this to any method which requires a valid Parcel ID in the request.
      responses:
        406:
          description: Parcel ID was in an incorrect format.
          body:
            application/json:
              schema: ErrorMessage
              example: |
                {
                  "message": "Parcel ID has to be 6 characters long and may only contain digits."
                }
        404:
          description: Could not find the specified parcel ID.
          body:
            application/json:
              schema: ErrorMessage
              example: |
                {
                  "message": "Parcel ID not found."
                }
/status/{parcelId}:
  displayName: Parcel Status Information
  uriParameters:
    parcelId:
      displayName: Parcel ID
      type: string
      required: true
      minLength: 6
      maxLength: 6
      example: 123456
  is: [ requiresValidParcelId ]
  get:
    description: Retrieves the current status for the specified parcel ID.
    responses:
      200:
        description: Current status.
        body:
          application/json:
            schema: |
              {
                "$schema": "http://json-schema.org/draft-04/schema#",
                "title": "Delivery Status",
                "type": "object",
                "properties": {
                  "status": {
                    "description": "The current status of the delivery.",
                    "type": "string"
                  },
                  "updated": {
                    "description": "The date time the last status update.",
                    "type": "string"
                  }
                }
              }
            example: |
              {
                "status": "Parcel is out for delivery.",
                "updated": "2015-12-09T16:53:19.5168335+00:00"
              }
  put:
    description: Creates or updates the status for the specified parcel ID.
    body:
      application/json:
        schema: |
          {
            "$schema": "http://json-schema.org/draft-04/schema#",
            "title": "Status Update",
            "type": "object",
            "properties": {                 
              "status": {
                "description": "The new status update message.",
                "type": "string"
              }
            }
          } 
        example: |
          {
            "status": "Delivered and signed by customer."
          }
    responses:
      201:
        description: The status has been successfully updated.</code></pre>

*I have uploaded [the same RAML](https://github.com/dustinmoris/RAML-Demo/blob/master/api.raml) to GitHub as well.*

RAML has a lot more to offer than what I showed in this basic example, but hopefully this gives you a rough idea of how intuitive and powerful it can be!

One of the great things is that you can auto-generate a client from a RAML spec, which brings me to the second part of this blog post.

## 2. Generate a client from the RAML document

Now that I have a detailed specification of what my API should look like it is time to spin up Visual Studio and get to the juicy bits.

First I create an empty test project and include the RAML file (api.raml) in a solution folder to keep everything together:

<a href="https://www.flickr.com/photos/130657798@N05/23265040193/in/dateposted-public/" title="RAML-Demo-Solution-Tree"><img src="https://farm1.staticflickr.com/734/23265040193_87ebcfdf49_o.png" alt="RAML-Demo-Solution-Tree"></a>

For the next part I have to install the official [RAML Tools for .NET](https://github.com/mulesoft-labs/raml-dotnet-tools) Visual Studio extension:

<a href="https://www.flickr.com/photos/130657798@N05/23865723396/in/dateposted-public/" title="RAML-Demo-Visual-Studio-RAML-Extension"><img src="https://farm6.staticflickr.com/5739/23865723396_88003023fd_o.png" alt="RAML-Demo-Visual-Studio-RAML-Extension"></a>

After a successful install I have now an additional context menu when I right click the &quot;References&quot; item underneath my test project:

<a href="https://www.flickr.com/photos/130657798@N05/23596120010/in/dateposted-public/" title="RAML-Demo-Add-RAML-Reference"><img src="https://farm1.staticflickr.com/584/23596120010_2895ee90f2_o.png" alt="RAML-Demo-Add-RAML-Reference"></a>

A click on that pops up a pretty much self-explaining dialog:

<a href="https://www.flickr.com/photos/130657798@N05/23783503252/in/dateposted-public/" title="RAML-Demo-Add-RAML-Reference-Dialog"><img src="https://farm1.staticflickr.com/769/23783503252_f6263a16ef_o.png" alt="RAML-Demo-Add-RAML-Reference-Dialog"></a>

I select the upload option and then navigate to the api.raml inside my solution folder. After confirmation I was presented with an Import RAML dialog:

<a href="https://www.flickr.com/photos/130657798@N05/23865723466/in/dateposted-public/" title="RAML-Demo-Create-Client"><img src="https://farm6.staticflickr.com/5778/23865723466_db487ddba7_o.png" alt="RAML-Demo-Create-Client"></a>

The import process automatically detects my single endpoint (/status/{parcelId}) and the only thing I had to change was the default client name to be &quot;ParcelDeliveryApiClient&quot; in case I wanted to import another API at a later time.

Hitting the Import button finishes off the remaining work and once completed I had a new API reference in my project tree:

<a href="https://www.flickr.com/photos/130657798@N05/23596119830/in/dateposted-public/" title="RAML-Demo-RAML-References-in-Project"><img src="https://farm6.staticflickr.com/5662/23596119830_abde113f83_o.png" alt="RAML-Demo-RAML-References-in-Project"></a>

Done! The import was extremley smooth and now I should have a new ParcelDeliveryApiClient class which enables me to run all operations against my API as described in the RAML spec:

<a href="https://www.flickr.com/photos/130657798@N05/23797380272/in/dateposted-public/" title="RAML-Demo-Aut-Generated-Client-in-Code"><img src="https://farm6.staticflickr.com/5833/23797380272_a633140864_o.png" alt="RAML-Demo-Aut-Generated-Client-in-Code"></a>

Not convinced yet? Let's explore the auto-generated client by writing some tests in the next step!

## 3. Write tests using the auto-generated client

Using the client I can write integration tests against a real endpoint. At the moment I don't have a working API running anywhere so I am making up a random endpoint URI and define other parameters for a first test:

<pre><code>[TestFixture]
public class ParcelDeliveryApiTests
{
    [Test]
    public async Task IntegrationTest()
    {
        const string endpoint = "http://localhost/raml-demo-api/v1/status";
        const string parcelId = "123456";
        const string status = "Parcel is out for delivery.";
    }
}</code></pre>

Next I can initialize an instance of the client and fire a PUT request with a status update for parcel ID 123456:

<pre><code>[Test]
public async Task IntegrationTest1()
{
    const string endpoint = "http://localhost/raml-demo-api/v1/status";
    const string parcelId = "123456";
    const string status = "Parcel is out for delivery.";

    <strong>var parcelDeliveryApiClient = new ParcelDeliveryApiClient(endpoint);

    var putResult =
        await parcelDeliveryApiClient.StatusParcelId.Put(
            new StatusParcelIdPutRequestContent
            {
                Status = status
            },
            parcelId);</strong>
}</code></pre>

Everything you see here has been auto-generated when importing the RAML file. Every member of the client class such as the &quot;StatusParcelId&quot; object and the &quot;Put(...)&quot; method, as well as the &quot;StatusParcelIdPutRequestContent&quot; DTO class have been created for me.

Remember, all I have done so far was to describe my API using RAML and with a few additional clicks in Visual Studio I am already able to write full fletched integration tests against an API which does not even exist yet!

Not bad ey? Let's continue... :)

The expected result is HTTP status code 201:

<pre><code>[Test]
public async Task IntegrationTest()
{
    const string endpoint = "http://localhost/raml-demo-api/v1/status";
    const string parcelId = "123456";
    const string status = "Parcel is out for delivery.";

    var parcelDeliveryApiClient = new ParcelDeliveryApiClient(endpoint);

    var putResult =
        await parcelDeliveryApiClient.StatusParcelId.Put(
            new StatusParcelIdPutRequestContent
            {
                Status = status
            },
            parcelId);

    <strong>Assert.AreEqual(HttpStatusCode.Created, putResult.StatusCode);</strong>
}</code></pre>

This is an integration test and not a unit test, so why stop here and not continue with one more check?

<pre><code>[Test]
public async Task IntegrationTest()
{
    const string endpoint = "http://localhost/raml-demo-api/v1/status";
    const string parcelId = "123456";
    const string status = "Parcel is out for delivery.";

    var parcelDeliveryApiClient = new ParcelDeliveryApiClient(endpoint);

    var putResult =
        await parcelDeliveryApiClient.StatusParcelId.Put(
            new StatusParcelIdPutRequestContent
            {
                Status = status
            },
            parcelId);

    Assert.AreEqual(HttpStatusCode.Created, putResult.StatusCode);

    <strong>var getResult = await parcelDeliveryApiClient.StatusParcelId.Get(parcelId);

    Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode);
    Assert.AreEqual(status, getResult.Content.StatusParcelIdGetOKResponseContent.Status);</strong>
}</code></pre>

After the PUT I am firing a GET with the same parcel ID and expect to retrieve another successful response with the updated status.

When I run this test it will fail on the first assert, because it cannot find the endpoint yet, but I am going to fix this very soon.

This test obviously doesn't cover everything from the RAML document, but at this point it should be clear that with the auto-generated client I can test every aspect of my API without having to write any client code myself.

### Coupling the RAML file to the API

So how is this better than a normal integration test for my API? The key benefit is that the client is a 1:1 replica of the RAML file. If the API changes I will have to update the client in my tests as well which subsequently forces me to update the RAML file.

Besides that it took me only 10 seconds to generate a perfect abstraction of my API which can be used for more than just writing tests.

## 4. Implement the API to satisfay the tests

I have to admit this part is a bit dry and has very little to do with RAML itself. I thought it would be good though to provide a full end to end example of how I envision a better approach of designing and building a RESTful API in .NET.

For that reason I will make it quick and fast forward most of this step and provide a quick and dirty implementation of the API which satisfies the integration test from above:

<pre><code>public StatusModule() : base("/v1/status")
{
    Get["/{parcelId}"] = ctx =>
    {
        if (!Statuses.ContainsKey(ctx.parcelId))
            return HttpStatusCode.NotFound;

        return new JsonResponse(
            Statuses[ctx.parcelId], 
            new DefaultJsonSerializer());
    };

    Put["/{parcelId}"] = ctx =>
    {
        dynamic obj = JsonConvert.DeserializeObject(Request.Body.AsString());

        Statuses[ctx.parcelId] = new StatusInformation
        {
            Status = obj.status.Value,
            Updated = DateTime.UtcNow.ToString("o")
        };

        return HttpStatusCode.Created;
    };
}</code></pre>

This snippet doesn't implement the entire API, but enough to make the test go green.

*By the way, you can find the [entire sample code of this blog post](https://github.com/dustinmoris/RAML-Demo) hosted on GitHub.*

After cheating myself through step 4 let's move on to the final part of this article and look at one more important facet of working with RAML.

## 5. Document and review an API using the Anypoint Platform

Before going into any further details let's quickly recap what I've done so far:

-   I designed an API using RAML
-   Used the RAML Tools for .NET to auto-generate a client
-   Wrote an integration test with that client
-   Implemented the API to satisfy my test

Not sure what you think, but it feels to me like I am done. You ask what about documentation? Well RAML is already human readable, it is accurate and tested against my actual API and all my tests are passing, so I am actually done! Right?

Theoretically yes, but practically no. There are two more issues I have to solve:

-   At the moment my RAML document is only in my source control and likely not accessible to external consumers
-   Integration tests are great to give me confidence, but my stakeholders don't understand those green and red lights in Visual Studio (or CI system) and likely want to verify that the API works for themselves

Fair enough... not a problem though. This is where I find the [Anypoint Platform](https://anypoint.mulesoft.com/apiplatform/) extremely useful!

### Anypoint Platform

Among many other features [Anypoint](https://anypoint.mulesoft.com/) allows me to document and publish my API with an interactive designer (much like the API Workbench in Atom, but even feature richer) and create a live Portal at no cost.

