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

In this article I would like to demonstrate how we can close this gap by building a RESTful API with a design- and test first appraoch using [RAML](http://raml.org/).

I will showcase an entire end to end scenario by building a simple demo API and run you through the following steps:

1.   Design an API using RAML
2.   Generate a client from the RAML definition
3.   Write failing tests using the auto generated client
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

For further reading I would highly recommend to go through the official RAML tutorials to learn all the basic concepts and more advanced features on your own pace:

-   [RAML 100 Tutorial (Basics)](http://raml.org/developers/raml-100-tutorial)
-   [RAML 200 Tutorial (Advanced)](http://raml.org/developers/raml-200-tutorial)

Now that you know what RAML is I will jump straight into the first part where I'll be using RAML to design an API:

## 1. Design an API using RAML

As I mentioned earlier, for the purpose of this demo I would like to build a very rudimental parcel delivery API with the following two endpoints:

-   GET /status/{parcelId} will return the status of a parcel
-   PUT /status/{parcelId} will update the status of a parcel

RAML is a YAML based language and designed for human readability. The beauty of this is that you can write RAML in any basic editor without fancy syntax highlighting and it will be still easy to read and understand.

However, there is a really good [Atom](https://atom.io/) plugin called [API Workbench](http://apiworkbench.com/) which I am using to kick start my API:

<pre><code>#%RAML 0.8
title: Parcel Delivery API
version: v1
baseUri: http://localhost/raml-demo/{version}
protocols: [ HTTP, HTTPS ]</code></pre>

*Sidenote:
I will not go through every single line of the spec, but give you some context so that you get the idea of where I am heading with this API.*

At the top of the document I specified the RAML version, followed by the title of the API, the version and the basic URI with a version placeholder. This will allow me to introduce breaking changes in the future. The API shall also be called from both protocols, HTTP and HTTPS.

Next I define a single endpoint to set and retrieve status information for a given parcel ID:

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

As you can see there is probably not a lot I have to explain. The GET operation expects a successful response with the 200 HTTP status code and a JSON payload as defined above. Note how RAML allows me to provide an example alongside the schema for better understanding and later validation.

I am pleased with this and apply something similar for the PUT operation:

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

The only difference is that the PUT operation has to supply a JSON object in the HTTP body and expects the [201 status code](http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.2.2) as a success indicator.

This is 