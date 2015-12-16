<!--
    Published: 2015-12-05 22:15
    Author: Dustin Moris Gorski
    Title: Using RAML to design, test and document RESTful APIs
    Tags: raml restful-api design testing
-->
Building a RESTful API is easy. Building a RESTful API which is easy to consume is more difficult. There are three key elements which make a good API:

-   Intuitive design
-   Good documentation
-   Documentation which matches the implementation

While good API design is extremely important I would argue that accurate and complete documentation is even more important, because it makes an even less intuitive API easier to use than a well designed API with incomplete or wrong documentation.

Having said that we all know how difficult it is to keep documentation up to date. Its too easy to get it out of sync with ongoing development and sometimes we don't even realise the subtle differences between the actual implementation and what we have formally recorded.

In this article I will demonstrate how we can build RESTful APIs with a design- and test-first appraoch and tighten the relationship between implementation and documentation.

## RAML

[RAML](http://raml.org/) stands for RESTful API Modeling Language and this is exactly what it delivers.

If you are familiar with [Swagger](http://swagger.io/) or [API Blueprint](https://apiblueprint.org/) then this should be familiar, except that RAML is designed to be human readable and remarkably easy to use.

At the time of writing there are two public specifications:

- [RAML 0.8](https://github.com/raml-org/raml-spec/blob/master/raml-0.8.md)
- [RAML 1.0 RC](http://docs.raml.org/specs/1.0/)

In this blog post I will use RAML 0.8.

### A simple example

The aim of this blog post is to showcase the relation between RAML and an API rather than the richness of RAML itself. For that reason I will build a very simple parcel delivery status API with the following two endpoints:

- GET /{parcelId} will return the status of a parcel
- PUT /{parcelId} will update the status of a parcel

## Design and document your API with RAML

I will start off by defining the basics of my new API:

<pre><code>#%RAML 0.8
title: Parcel Delivery Status API
version: v1
baseUri: http://localhost/pds/{version}</code></pre>

At the top of my document I specified the RAML version, followed by the title of my API, the version and the basic URI with the version placeholder.