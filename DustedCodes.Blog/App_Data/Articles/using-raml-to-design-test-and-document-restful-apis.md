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

Having said that we all know how difficult it is to keep documentation up to date. It is very loosely coupled to the actual implementation without any enforcement other than a human trial and error process.

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

For further reading I would recommend to go through the official RAML tutorials explaining the basic concepts and more advanced features in your own time and at your own pace:

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
I will not go through every single line of the spec, but try to provide enough context so you can easily follow the examples.*

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

After this I define the contract for the GET operation, which shall return the current status of a parcel:

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

Now I am almost done designing the API. The only thing I would like to add is to describe what happens if a consumer provides a wrong parcel ID.

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

If you had paid close attention you might have noticed the &quot;schema: ErrorMessage&quot; in the declaration of the response body.

Shared schemas is another feature of RAML. In this case I have defined a schema called &quot;ErrorMessage&quot; which describes the payload in the error response:

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

It is the same principle as to how I defined the success response in my previous example, with the only difference that this one is declared in a separate section within the RAML document which allows re-use in mutliple endpoints.

At last I need to hook up the status endpoint with the trait:

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

*The full [RAML document](https://github.com/dustinmoris/RAML-Demo/blob/master/api.raml) can also be found in the [RAML-Demo repository](https://github.com/dustinmoris/RAML-Demo/) on GitHub*

RAML has a lot more to offer than what I showed in this basic example, but hopefully this gives you a rough idea of how intuitive and powerful it can be!

One of my favourite features is that you can auto-generate a .NET client from a RAML file, which brings me to the second part of this blog post.

## 2. Generate a client from the RAML document

Now that I have a detailed specification of what my API should look like it is time to open up Visual Studio and get my hands dirty.

First I create an empty test project and include the RAML file (api.raml) in a solution folder to keep everything together:

<a href="https://www.flickr.com/photos/130657798@N05/23265040193/in/dateposted-public/" title="RAML-Demo-Solution-Tree"><img src="https://farm1.staticflickr.com/734/23265040193_87ebcfdf49_o.png" alt="RAML-Demo-Solution-Tree"></a>

For the next part I have to install the official [RAML Tools for .NET](https://github.com/mulesoft-labs/raml-dotnet-tools) Visual Studio extension:

<a href="https://www.flickr.com/photos/130657798@N05/23865723396/in/dateposted-public/" title="RAML-Demo-Visual-Studio-RAML-Extension"><img src="https://farm6.staticflickr.com/5739/23865723396_88003023fd_o.png" alt="RAML-Demo-Visual-Studio-RAML-Extension"></a>

After a successful install I have an additional context menu when I right click the &quot;References&quot; item underneath my test project:

<a href="https://www.flickr.com/photos/130657798@N05/23596120010/in/dateposted-public/" title="RAML-Demo-Add-RAML-Reference"><img src="https://farm1.staticflickr.com/584/23596120010_2895ee90f2_o.png" alt="RAML-Demo-Add-RAML-Reference"></a>

A click on that menu item pops up a pretty much self-explaining dialog:

<a href="https://www.flickr.com/photos/130657798@N05/23783503252/in/dateposted-public/" title="RAML-Demo-Add-RAML-Reference-Dialog"><img src="https://farm1.staticflickr.com/769/23783503252_f6263a16ef_o.png" alt="RAML-Demo-Add-RAML-Reference-Dialog"></a>

I select the upload option and navigate to the api.raml inside my solution folder. After confirmation I am presented with an Import RAML dialog:

<a href="https://www.flickr.com/photos/130657798@N05/23865723466/in/dateposted-public/" title="RAML-Demo-Create-Client"><img src="https://farm6.staticflickr.com/5778/23865723466_db487ddba7_o.png" alt="RAML-Demo-Create-Client"></a>

The import process automatically detects my single endpoint (/status/{parcelId}) and the only thing I had to change was the default client name to be &quot;ParcelDeliveryApiClient&quot; in case I wanted to import another API at a later point.

Hitting the Import button finishes the remaining work and once completed I am seeing a new API reference in my project tree:

<a href="https://www.flickr.com/photos/130657798@N05/23596119830/in/dateposted-public/" title="RAML-Demo-RAML-References-in-Project"><img src="https://farm6.staticflickr.com/5662/23596119830_abde113f83_o.png" alt="RAML-Demo-RAML-References-in-Project"></a>

This was a very smooth and painless import and if successful I should be able to instantiate an object of `ParcelDeliveryApiClient` in a new class file:

<a href="https://www.flickr.com/photos/130657798@N05/23797380272/in/dateposted-public/" title="RAML-Demo-Aut-Generated-Client-in-Code"><img src="https://farm6.staticflickr.com/5833/23797380272_a633140864_o.png" alt="RAML-Demo-Aut-Generated-Client-in-Code"></a>

Amazing, let's explore the auto-generated client by writing some tests next!

## 3. Write tests using the auto-generated client

Using the client I can write integration tests against a real endpoint. At the moment I don't have a working API running anywhere so I define a provisional URI and a couple more parameters for my first test:

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

Next I initialize an instance of the client and send a PUT request for the parcel ID 123456:

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

The amazing thing is that the entire client code has been auto-generated when importing the RAML file. Every member of the class such as the &quot;StatusParcelId&quot; or the &quot;Put(...)&quot; method, as well as the &quot;StatusParcelIdPutRequestContent&quot; DTO class have been auto-magically created for me.

Remember, all I have done was to describe my API using RAML and with a few additional clicks in Visual Studio I am now able to write full fletched integration tests against an API which doesn't even exist yet!

I find this pretty cool.

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

For this integration test I'd like to add one more check:

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

After the PUT I am firing a GET with the same parcel ID and expect another successful response with the updated status.

When I run this test it will fail on the first assert, because there is nothing behind that endpoint yet, but I am going to fix this very soon.

This test obviously doesn't cover everything from the RAML document, but at this point it should be clear that with the auto-generated client I can test every aspect of my API without having to write any client code myself.

### Coupling the RAML file to the API

So how is this better than a normal integration test? The key benefit is that the client is a 1:1 replica of the RAML file. If the API changes I will have to update the client as well which subsequently forces me to update the RAML first.

Besides that it took me only 10 seconds to generate a perfect abstraction of my API which can be used for more than just writing tests.

## 4. Implement the API to satisfay the tests

I have to admit this part has very little to do with RAML, but I thought it would be good to provide a full end to end example as part of this blog post.

For that reason I will make it quick and fast forward this step by providing a quick and dirty implementation which should satisfy the integration test from above:

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

*By the way, you can find the [entire sample code of this blog post](https://github.com/dustinmoris/RAML-Demo) hosted on GitHub as well.*

After cheating myself through step 4 let's move on to the next and final part of this article and look at another important aspect of building RESTful APIs.

## 5. Document and review an API using the Anypoint Platform

Before going any deeper let's quickly recap what I've done so far:

-   I designed an API using RAML
-   Used the RAML Tools for .NET to auto-generate a client
-   Wrote an integration test with the generated client
-   Implemented enough of the API to satisfy my test

It feels like I am almost done. So what about documentation now? Well RAML is already human readable, it is accurate and tested against my actual API and all my tests are passing, so am I actually done?

No, not yet, there are two more issues I have to solve:

-   At the moment the RAML document is only in my source control and likely not accessible to external consumers
-   Integration tests are great to give me confidence, but my stakeholders don't understand those green and red lights in Visual Studio (or CI system) and likely want to verify the API for themselves

This is where I find the [Anypoint Platform](https://anypoint.mulesoft.com/apiplatform/) extremely useful!

### Anypoint Platform

Among many other features [Anypoint](https://anypoint.mulesoft.com/) allows me to document and publish my API with an interactive designer (much like the API Workbench but even richer) and create a live Portal at no cost.

The designer is exceptionally well done. It offers many features like syntax highlighting, intellisense, instant RAML validation and auto-suggestion of available nodes:

<a href="https://www.flickr.com/photos/130657798@N05/23881341396/in/dateposted-public/" title="RAML-Demo-Anypoint-Designer-Editor"><img src="https://farm1.staticflickr.com/665/23881341396_87c12df3f3_o.png" alt="RAML-Demo-Anypoint-Designer-Editor"></a>

<a href="https://www.flickr.com/photos/130657798@N05/23548329229/in/dateposted-public/" title="RAML-Demo-Anypoint-Designer-Suggested-Nodes"><img src="https://farm1.staticflickr.com/565/23548329229_70792a7a3b_o.png" alt="RAML-Demo-Anypoint-Designer-Suggested-Nodes"></a>

Another brilliant feature is the interactive preview when editing a RAML file. It visually displays every aspect of your RAML in a beautiful interface, like all possible responses in this example:

<a href="https://www.flickr.com/photos/130657798@N05/23611739780/in/dateposted-public/" title="RAML-Demo-Anypoint-Designer-Preview-Responses"><img src="https://farm1.staticflickr.com/570/23611739780_4be138f177_o.png" alt="RAML-Demo-Anypoint-Designer-Preview-Responses"></a>

It even goes as far as allowing me to interact with a mocked version while working on the RAML:

<a href="https://www.flickr.com/photos/130657798@N05/23881341456/in/dateposted-public/" title="RAML-Demo-Anypoint-Designer-Preview"><img src="https://farm6.staticflickr.com/5786/23881341456_e44ba5d047_o.png" alt="RAML-Demo-Anypoint-Designer-Preview"></a>

When I click the Try It button it displays me a form with all relevant parameters and auto-populates the fields with the values from the examples in my RAML:

<a href="https://www.flickr.com/photos/130657798@N05/23907433735/in/dateposted-public/" title="RAML-Demo-Anypoint-Designer-TryIt-Request"><img src="https://farm6.staticflickr.com/5791/23907433735_0847e4e5c9_o.png" alt="RAML-Demo-Anypoint-Designer-TryIt-Request"></a>

After I finished designing my API I published it into a Live Portal which is publicly accessible (can be private as well), where users and stakeholders can try the API against my live service.

Try it yourself by executing some PUT and GET requests via the [Live Portal of my demo API](https://anypoint.mulesoft.com/apiplatform/dustinmoris#/portals/organizations/1c966d9b-793c-46bc-a87a-427b9a4a9b4a/apis/45625/versions/47291).

Any technical or non-technical person can review the API and validate if it works as expected and I as a developer cannot claim that a feature is done if it is not live in the Portal.

This is how I envision API development in an agile environment.