<!--
    Published: 2015-12-05 22:15
    Author: Dustin Moris Gorski
    Title: Design, test and document RESTful APIs using RAML
    Tags: raml restful-api design testing
-->
Building a RESTful API is easy. Building a RESTful API which is easy to consume is more difficult. There are three key elements which make a good API:

-   Intuitive design
-   Good documentation
-   Documentation which actually matches the implementation

Intuitive API design is obviously very important, but equally important and often neglected is good and complete documentation which makes it easier to build against your API.

Having said that we all know how difficult it is to keep documentation up to date. It is very loosely coupled to the actual implementation and without any enforcement other than a human trial and review process.

In this article I would like to demonstrate how we can close this gap by building a RESTful API with a design- and test first approach using [RAML](http://raml.org/).

I will showcase an entire end to end scenario by building a simple demo API and covering the following steps:

1.   Design an API using RAML
2.   Generate a client from the RAML definition
3.   Write failing tests using the auto-generated client
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

For further reading I would recommend to go through the official RAML tutorials explaining the basic concepts and more advanced features on your own pace:

-   [RAML 100 Tutorial (Basics)](http://raml.org/developers/raml-100-tutorial)
-   [RAML 200 Tutorial (Advanced)](http://raml.org/developers/raml-200-tutorial)

Now that you know what RAML is I will jump straight into the first part where I'll be using RAML to design an API:

## 1. Design an API using RAML

As I mentioned earlier, for the purpose of this demo I would like to build a very rudimental parcel delivery API with the following two endpoints:

-   ***GET** /status/{parcelId}* will return the status of a parcel
-   ***PUT** /status/{parcelId}* will update the status of a parcel

RAML is a YAML based language and designed for human readability. The beauty of this is that you can write RAML in any basic editor without any fancy syntax highlighting and it will be still easy to read and understand.

However, there is a really good [Atom](https://atom.io/) plugin called [API Workbench](http://apiworkbench.com/) which I am using to kick start my API:

<pre><code>#%RAML 0.8
title: Parcel Delivery API
version: v1
baseUri: http://localhost/raml-demo/{version}
protocols: [ HTTP, HTTPS ]</code></pre>

*Sidenote:
I will not go through every single line of the spec, but give some context so that you get the idea of where I am heading with this API.*

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

As you can see there is probably not a lot I have to explain. The GET operation expects a successful response with the 200 HTTP status code and a JSON payload as defined above. Note how RAML allows me to provide an example alongside the schema for better understanding and later validation. I find it really nice and it will be useful at a later point.

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

Because this applies to both operations, the GET and the PUT on my endpoint, I will define those additional error responses as a so called [trait](http://docs.raml.org/specs/1.0/#raml-10-spec-resource-types-and-traits) which can be shared by multiple endpoints in RAML:

<pre><code>...
traits:
  - requiresValidParcelId:
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

It is the same principle as before and nothing new to the way how we defined the success response above, only that it has been moved into a separate section in the RAML document which allows re-use in mutliple endpoints.

Last but not least I need to hook up my endpoint with the trait:

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

RAML has a lot more to offer than what I showed in this basic example, but hopefully this gives you a rough idea of how easy and powerful it can be!

One of the great things is that you can auto-generate a client from a RAML spec, which brings me to the second part of this blog series.

## 2. Generate a client from the RAML definition

Now that I have a detailed specification of what my API should look like it is time to spin up Visual Studio and get to the juicy bits.

First I have created an empty test project and included the RAML file (api.raml) in a solution folder to keep everything together:

<a href="https://www.flickr.com/photos/130657798@N05/23265040193/in/dateposted-public/" title="RAML-Demo-Solution-Tree"><img src="https://farm1.staticflickr.com/734/23265040193_87ebcfdf49_o.png" alt="RAML-Demo-Solution-Tree"></a>

For the next part I have to install the official [RAML Tools for .NET](https://github.com/mulesoft-labs/raml-dotnet-tools) Visual Studio extension:

<a href="https://www.flickr.com/photos/130657798@N05/23865723396/in/dateposted-public/" title="RAML-Demo-Visual-Studio-RAML-Extension"><img src="https://farm6.staticflickr.com/5739/23865723396_88003023fd_o.png" alt="RAML-Demo-Visual-Studio-RAML-Extension"></a>

After a successful install I have now an additional context menu when I right click the &quot;References&quot; item underneath my test project:

<a href="https://www.flickr.com/photos/130657798@N05/23596120010/in/dateposted-public/" title="RAML-Demo-Add-RAML-Reference"><img src="https://farm1.staticflickr.com/584/23596120010_2895ee90f2_o.png" alt="RAML-Demo-Add-RAML-Reference"></a>

A click on that pops up a pretty much self-explaining dialog:

<a href="https://www.flickr.com/photos/130657798@N05/23783503252/in/dateposted-public/" title="RAML-Demo-Add-RAML-Reference-Dialog"><img src="https://farm1.staticflickr.com/769/23783503252_f6263a16ef_o.png" alt="RAML-Demo-Add-RAML-Reference-Dialog"></a>

I selected upload and then navigated to the api.raml inside my solution folder. After confirmation I was presented with the Import RAML dialog:

<a href="https://www.flickr.com/photos/130657798@N05/23865723466/in/dateposted-public/" title="RAML-Demo-Create-Client"><img src="https://farm6.staticflickr.com/5778/23865723466_db487ddba7_o.png" alt="RAML-Demo-Create-Client"></a>

The import process automatically detected my single endpoint (/status/{parcelId}) and the only thing I wanted to change was the default client name to be &quot;ParcelDeliveryApiClient&quot; in case I have to import another client at a later time.

Hitting the Import button finished the remaining work and once completed I had the newly created reference showing up in my project files alongside a copy of the original RAML file:

<a href="https://www.flickr.com/photos/130657798@N05/23596119830/in/dateposted-public/" title="RAML-Demo-RAML-References-in-Project"><img src="https://farm6.staticflickr.com/5662/23596119830_abde113f83_o.png" alt="RAML-Demo-RAML-References-in-Project"></a>

Done. Wasn't this extremley straight forward or what?

Not convinced yet? Let's explore the auto-generated client by writing some tests now!

## 3. Write failing tests using the auto-generated client

