<!--
    Tags: giraffe template aspnet-core fsharp
-->

# Extending the Giraffe template with different view engine options

This is going to be a quick but hopefully very useful tutorial on how to create a more complex template for the `dotnet new` command line tool.

If you have ever build a [Giraffe](https://github.com/giraffe-fsharp/Giraffe) web application then you've probably started off by installing the [giraffe-template](https://github.com/giraffe-fsharp/giraffe-template) into your .NET CLI and then created a new boilerplate application by running the `dotnet new` command:

<pre><code>dotnet new giraffe</code></pre>

(*There is currently a [bug with the .NET CLI](https://github.com/dotnet/templating/issues/1373) which forces you to specify the `-lang` parameter.*)

Previously this would have created a new Giraffe web application which would have had the `Giraffe.Razor` NuGet package included and a default project structure with MVC's famous Razor view engine.

Since today (after you've [updated the giraffe-template](https://github.com/giraffe-fsharp/giraffe-template#updating-the-template) to the latest version) you can choose between three different options:

- `giraffe` - Giraffe's default view engine
- `razor` - MVC Razor views
- `dotliquid` - DotLiquid template engine

The `dotnet new giraffe` command optionally supports a new parameter called `--ViewEngine` or short `-V` now:

<pre><code>dotnet new giraffe -ViewEngine razor</code></pre>

If you are unsure which options are available you can always request help by running:

<pre><code>dotnet new giraffe --help</code></pre>

The output of the command line will display all available options and supported values as well:

<pre><code>Giraffe Web App (F#)

Author: Dustin Moris Gorski, David Sinclair and contributors

Options:
  -V|--ViewEngine
        giraffe    - Default GiraffeViewEngine
        razor      - MVC Razor views
        dotliquid  - DotLiquid template engine

        Default: giraffe</code></pre>

If you do not specify a view engine then the `dotnet new giraffe` command will automatically create a new Giraffe web application with the default `GiraffeViewEngine` engine.

## Creating multiple project templates as part of one dotnet new template

There are many ways of how I could have programmed these options into the Giraffe template, but none of them are very obviously documented in one place. The [documentation of the dotnet templating engine](#templating-engine-documentation)<sup>1</sup> is fairly scattered across multiple resources and hard to understand if you have never worked with it before. Part of today's blog post I thought I'll quickly sum up one of the options which I believed was the cleanest and most straight forward one.

Each view engine has a significant impact on the entire project structure, such as NuGet package dependencies, folder structure, code organisation and files which need to be included. I didn't want to hack around with `#if` - `#else` switches and introduce complex add-, modify- or delete rules and consequently decided that the easiest and least error prone way would be to create a [complete independent template for each individual view engine](https://github.com/giraffe-fsharp/giraffe-template/tree/master/src/content) first:

<pre><code>src
+-- giraffe-template.nuspec
|
+-- content
    |
    +-- .template.config
    |   +-- template.json
    |
    +-- DotLiquid.Template
    |   +-- Views
    |   +-- WebRoot
    |   +-- Program.fs
    |   +-- AppNamePlaceholder.fsproj
    |
    +-- Razor.Template
    |   +-- Views
    |   +-- WebRoot
    |   +-- Program.fs
    |   +-- AppNamePlaceholder.fsproj
    |
    +-- Giraffe.Template
        +-- WebRoot
        +-- Program.fs
        +-- AppNamePlaceholder.fsproj</code></pre>

I split the content of the Giraffe template into three distinctive sub templates:

- `DotLiquid.Template`
- `Razor.Template`
- `Giraffe.Template`

As you can see from the diagram there's still only one `.template.config\template.json` file at the root of the `content` folder and only one `giraffe-template.nuspec` file.

The benefit of this structure is very simple. There is a clear separation of each template and each template is completely independent of the other templates which makes maintenance very straight forward. I can work on each template as if they were small projects with full Intellisense and IDE support and being able to build, run and test each application.

The next step was to create the `--ViewEngine` parameter inside the `template.json` file:

<pre><code>"symbols": {
    "ViewEngine": {
      "type": "parameter",
      "dataType": "choice",
      "defaultValue": "giraffe",
      "choices": [
        {
          "choice": "giraffe",
          "description": "Default GiraffeViewEngine"
        },
        {
          "choice": "razor",
          "description": "MVC Razor views"
        },
        {
          "choice": "dotliquid",
          "description": "DotLiquid template engine"
        }
      ]
    }
  }</code></pre>

All I had to do was to define a new symbol called `ViewEngine` of type `parameter` and data type `choice`. Then I specified all supported options via the `choice` array and set the `giraffe` option as the default value.

Now that the `ViewEngine` parameter has been created I was able to use it from elsewhere in the specification. The `sources` section of a `template.json` file denotes what source code should be installed during the `dotnet new` command. In Giraffe's case this was very easy. If the `giraffe` option has been selected, then the source code shall come from the `Giraffe.Template` folder and the destination/target folder should be the root folder of where the `dotnet new` command is being executed from. The same logic applies to all the other options as well:

<pre><code>"sources": [
    {
      "source": "./Giraffe.Template/",
      "target": "./",
      "condition": "(ViewEngine == \"giraffe\")"
    },
    {
      "source": "./Razor.Template/",
      "target": "./",
      "condition": "(ViewEngine == \"razor\")"
    },
    {
      "source": "./DotLiquid.Template/",
      "target": "./",
      "condition": "(ViewEngine == \"dotliquid\")"
    }
  ]</code></pre>

With this in place I was able to create a new `giraffe-template` NuGet package and deploy everything to the official NuGet server again.

This is literally how easy it is to support distinct project templates from a single dotnet new template.

## Different templates with same groupIdentifier

Another very similar, but in my opinion less elegant way would have been to create three different `template.json` files and use the `groupIdentifier` setting in connection with the `tags` array to support three different templates as part of one. Unfortunately this option doesn't seem to be very well supported from the .NET CLI. Even though it works, the .NET CLI doesn't display any useful error message when a user makes a mistake or when someone types `dotnet new giraffe --help` into the terminal. It also doesn't allow a default value to be set which made it less attractive overall. I would only recommend to go with this option if you need to [provide different templates based on the selected .NET language](https://github.com/dotnet/dotnet-template-samples/tree/master/06-console-csharp-fsharp), in which case it works really well again.

If you have any further questions or you would like to know more about the details of the Giraffe template then you can visit the [giraffe-template GitHub repository](https://github.com/giraffe-fsharp/giraffe-template) for further reference.

This blog post is part of the [F# Advent Calendar in English 2017](https://sergeytihon.com/2017/10/22/f-advent-calendar-in-english-2017/) blog series which has been kindly organised by [Sergey Tihon](https://twitter.com/sergey_tihon). Hope you all enjoyed this short tutorial and wish you a very Merry Christmas!

<h4 id="templating-engine-documentation">1) Templating engine documentation</h4>

Various documentation for the `dotnet new` templating engine can be found across the following resources:

- [Custom templates for dotnet new](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)
- [dotnet-template-samples](https://github.com/dotnet/dotnet-template-samples) (GitHub repo with a lot of useful examples)
- [How to create your own templates for dotnet new](https://blogs.msdn.microsoft.com/dotnet/2017/04/02/how-to-create-your-own-templates-for-dotnet-new/) (Great blog post)
- [Available templates for dotnet new](https://github.com/dotnet/templating/wiki/Available-templates-for-dotnet-new) (Community built templates)
- [dotnet tempalting engine](https://github.com/dotnet/templating) (Official dotnet templating engine GitHub repository)