<!--
    Tags: dotnet dotnet-core aspnet aspnet-core
-->

# .NET Basics

![.NET for Dummies](https://cdn.dusted.codes/images/blog-posts/2021-05-06/dotnet-for-dummies-2.png)

So you want to learn .NET but you are [confused about the differences between .NET Framework and .NET Core](https://www.reddit.com/r/csharp/comments/n5av93/do_i_need_to_learn_asp_net_before_starting_asp/) or what the various versions of ASP.NET are or what the relationship is between C# and F#? If that is the case then you came to the right place. This guide will cover all the basics of .NET and shed some light on the various acronyms and buzz words behind it!

If you are new to .NET and you want to get a holistic overview of the entire platform and what parts really drive the framework and how they relate to each other then look no further because this blog post will cover them all!

<div class="tip"><p><strong>Disclaimer:</strong> I am a (mostly) self thaught developer and a non native English speaker. I have written this guide with the utmost care and to the best of my knowledge, but there is a good chance that I could have made a mistake or misrepresented some details in the guide below. If you find an issue then please be polite and let me know in the comments underneath or email me directly at hello [at] dusted.codes. I appreciate any type of feedback and will do my best to correct any mistakes as soon as I can. Thank you.</p></div>

## Table of contents

- [What is .NET?](#what-is-net)
    - [C# vs. F# vs. VB.NET](#c-vs-f-vs-vbnet)
    - [IL Code and the CLI](#il-code-and-the-cli)
    - [SDK and Runtime (CLR)](#sdk-and-runtime-clr)
    - [So what is .NET?](#so-what-is-net)
- [What is .NET Framework?](#what-is-net-framework)
- [What is Mono?](#what-is-mono)
- [What is .NET Core?](#what-is-net-core)
- [What is .NET Standard?](#what-is-net-standard)
- [Why is there no .NET Core 4?](#why-is-there-no-net-core-4)
- [What is ASP.NET?](#what-is-aspnet)
- [What is ASP.NET Core?](#what-is-aspnet-core)
- [Where do I start?](#where-do-i-start)
- [Who is dotnet-bot?](#who-is-dotnet-bot)
- [Why is everything .NET?](#why-is-everything-net)
- [Useful links](#useful-links)

## What is .NET?

.NET is the top level name of a Microsoft technology used for building software. It is a twenty year old platform which has seen a lot of change and innovation over the years and which spans across many different domains. .NET can be used to develop web applications, games, IoT, machine learning, Desktop and mobile applications and much more. The term ".NET" is often used in a very far reaching meaning and used synonymously for a wide range of smaller parts of the platform such as the original .NET Framework, the newer .NET Core, or languages such as C#, F# and VB.NET.

When people talk about ".NET" they could mean anything from ASP.NET to PowerShell. In order to understand .NET one has to look at all the different pieces which make up the platform and look at them individually to fully make of them sense.

<div class="tip"><p><strong>Tip:</strong> The official website for .NET is <a href="https://dotnet.microsoft.com" target="_blank">dotnet.microsoft.com</a>, but a much more memorable URL to all things .NET is <a href="https://dot.net" target="_blank">dot.net</a>, which will redirect a beginner to the most up to date resources around the Microsoft .NET platform.</p></div>

### C# vs. F# vs. VB.NET

First of all .NET is not a programming language. It's a development platform. In fact, you can use three different officially supported programming languages to develop with .NET:

- [C#](https://dotnet.microsoft.com/learn/csharp) (An object oriented language, very similar to Java)
- [F#](https://dotnet.microsoft.com/learn/fsharp) (A functional first language, similar to OCaml)
- [VB.NET](https://docs.microsoft.com/en-gb/dotnet/visual-basic/) (An object oriented language which is the successor of VB6)

VB.NET and C# were the first officially supported languages for .NET. VB.NET is the successor of Visual Basic 6.0 and whilst extremely similar in syntax yet a different language and incompatible with the classic version of Visual Basic.

C# is a C-like object oriented language which first borrowed a lot of its early concepts from Java and was initially seen as an imitation of it. Today things stand very differently and C# is often being praised for its innovation and modern features which Java lacks behind. The name [C# is a word play from taking C++ and adding two more + signs](https://www.donnfelker.com/how-did-c-get-its-name/) to it, so that it forms the hash character, or as .NET developers like to call it, the "sharp" sign.

<div class="tip"><p><strong>Fun fact:</strong> Initially C# was developed under the name &quot;Cool&quot; which stood for <strong>C</strong>-like <strong>O</strong>bject <strong>O</strong>riented <strong>L</strong>anguage but then was renamed to C# for trademark reasons.</p></div>

A few years after .NET's initial release Microsoft Research developed a completely new language called F#. Unlike C# and VB.NET "F-Sharp" was designed as a functional first multi paradigm language which took a lot of inspiration from Ocaml, Erlang, Python, Haskell and Scala at the time. For many years F# has been the leading source of inspiration for new features in C# and was the first .NET language to introduce features such as Linq, Async and pretty much all of the new features starting from C# 7 and onwards. It was also the first language to go open source before all of .NET became public.

All three programming languages are [statically typed languages](https://en.wikipedia.org/wiki/Type_system#Static_and_dynamic_type_checking_in_practice) which means that the compiler will provide type safety checks during development and compilation. Those type safety checks can prevent many hard to catch runtime errors which could occur otherwise.

The opposite to a statically typed language is a dynamic one. Famous examples of dynamic languages are Python or JavaScript. For example, in C# one cannot accidentally assign a string value to a float variable but in JavaScript this would be perfectly fine.

<div class="tip"><p><strong>Note:</strong> C# has the <code>dynamic</code> keyword which allows a developer to introduce dynamic behaviour into the language, but it remains an extremely rarely used feature which has been mostly reserved to excpeptional cases where interop with other systems is required.</p></div>

### IL Code and the CLI

A term which often gets mentioned alongside .NET is so called "IL Code". IL code stands for intermediate language code and is the code to which C#, F# and VB.NET get translated to when an application gets compiled.

In .NET it is perfectly possible (and not that uncommon) to have multiple projects of different languages mixed into a single solution. For example one can have several F# projects sitting alongside C# and talk to each other, forming a larger application.

This is possible because the CLI (Common Language Infrastructure, not to be mistaken with the "command line interface") is a language agnostic interface which allows code to be executed on different architectures without having the code to be rewritten for each specific platform.

At this point this might sound a little bit confusing but it will become much clearer when I will explain the .NET CLR in the next part.

<div class="tip"><p><strong>Tip:</strong> One can use <a href="https://sharplab.io" target="_blank">sharplab.io</a> to translate .NET code into IL code and get a glimpse into the inner workings of the compiler.</p></div>

### SDK and Runtime (CLR)

Software programmers write applications with the help of very high level languages which have human readable constructs such as `if`, `else`, `return`, `while`, `foreach`, `public`, `private` and so on. Of course this is not how a binary machine works and therefore every application code which was written in a high level programming language must get translated into native machine code at some stage in its life. We can broadly characterise a programming language into "**compiled**" and "**interpreted**" languages and "**managed**" and "**unmanaged**" code.

#### Compiled vs. Interpreted

An interpreted language is a programming language where the code which gets written by a developer is the final code which gets shipped as the application. For example, in PHP a developer doesn't compile `.php` files into something else. PHP files are the final artefact which get shipped to a web server and only when an incoming request hits the server the PHP engine will read all the `.php` files and interpret the code "just in time" to translate it into lower level machine code. If a developer made a mistake then they will not know until the code gets executed. This has the benefit that a developer can quickly modify raw `.php` files and get a quick feedback loop during development but on the other hand it means that many errors will not be found until the server runs the code. Typically an interpreted language also takes a slight performance hit because the runtime has to do the entire compilation at the time of execution.

In contrast a compiled language does all of the compilation work (or parts of it) ahead of time. For example Go requires code to be compiled directly into native machine code before an application can run. This means that a developer will be notified by the compiler about any potential errors well ahead of execution, but equally it means that changes to the code require an additional step during development before they can get tested.

.NET is also a compiled language, but unlike Go or Rust it doesn't compile directly into native machine code but into the intermediate language (IL Code). The IL code is the artefact which gets shipped as the final application and the .NET runtime will do the final compilation step from IL code to native machine code using the JIT (just in time compiler) at runtime. This is why .NET applications get deployed as `.dll` files and not raw `.cs`, `.fs` or `.vb` files. This is also how C# and F# and VB.NET can talk to each other because they communicate at the IL level and not before. The model which .NET follows is popular with many other programming languages and provides some notable benefits. It is usually much faster than interpreted code because much of the compilation is done well in advance, but then still slightly slower than low level languages such as Rust or C++ which can compile directly into native machine instructions.

The important take away is to understand why .NET has `.dll` files and that .NET requires a runtime to do the final compilation from IL code to machine code with the help of the JIT. This runtime is called the "CLR" (common language runtime) in .NET.

<div class="tip"><p><strong>Fun fact:</strong> The source code compiler for C# and VB.NET is called "Roslyn", whereas <a href="https://github.com/dotnet/fsharp/blob/main/docs/compiler-guide.md" target="_blank">F# has its own compiler</a> called the <code>Fsharp.Compiler.Service</code> and a console application called <code>fsc</code> (fsharp compiler) which can be used to invoke the <code>FSharp.Compiler.Service</code> from the command line.</p></div>

#### Managed vs. Unmanaged

Managed code is simply code which requires the execution under the [CLI (Common Language Infrastructure)](#il-code-and-the-cli). Unmanaged code is code which runs outside the CLI. Without going too far into detail the point is that in theory one can invent any new framework (or language) which can run on .NET as long as it implements the CLI. If that is the case then [the language can be translated into IL code and get executed under the CLR (Common Language Runtime)](https://en.wikipedia.org/wiki/List_of_CLI_languages). All these abstractions make it possible to not only have more than one language for .NET but also multiple frameworks such as .NET Framework, .NET Core or Mono (more on this later).

<div class="tip"><p><strong>Fun fact:</strong> Microsoft has also developed a dynamic language runtime (DLR) which runs on the CLR and therefore can support dynamic languages on top of .NET. The most notable examples are <a href="https://ironpython.net" target="_blank">IronPython</a> and <a href="http://ironruby.net" target="_blank">IronRuby</a>, which are implementations of the Python and Ruby programming language for .NET.</p></div>

#### SDK vs. Runtime

Now that the purpose of the CLR (.NET runtime) is a bit clearer one can also explain why .NET is available as two different installations:

- **SDK** (Software Development Kit with the runtime)
- **Runtime** (only)

The runtime is what an end user's machine or a server must have installed in order to run a .NET application. The SDK is the software development tool chain which allows programmers to develop .NET applications. In layman terms, the SDK is what produces a `.dll` file and the runtime is what can run it.

This is why the [official .NET page](https://dotnet.microsoft.com/download/dotnet/6.0) offers both options to download:

![.NET 5 Download Page](https://cdn.dusted.codes/images/blog-posts/2021-05-06/dotnet-download-page-2.png)

Software developers must download the .NET SDK in order to build .NET applications, but a web server or end user machine should only install the .NET runtime which is the much smaller installation.

Another point which becomes very clear from the download page above is the loose relationship between the different .NET languages. As one can see C#, F# and VB.NET are at complete different stages in their life and therefore versioned differently. The languages can evolve independently and introduce new language features to their own liking as long as the associated compiler translates the source code into valid IL.

A careful observer might have also noticed the disparity between the .NET version, the .NET SDK and the .NET runtime. The official .NET version normally refers to the .NET runtime version, because that is essentially the final execution runtime which needs to be installed on a machine. The SDK can have a different version because the development tool chain can improve faster than the runtime itself and support new features and better development workflows whilst still targeting the same version of .NET.

<div class="tip"><p><strong>Side note:</strong> Not every programming language requires an SDK and runtime. For example languages such as <a href="https://www.rust-lang.org" target="_blank">Rust</a> or <a href="https://golang.org" target="_blank">Go</a>, which directly compile into native machine code, don't require a runtime. These languages only have a <a href="https://golang.org/dl/" target="_blank">single download option</a> available which normally represents the SDK for building software. Coming from one of these languges can make .NET feel unusual, but in essence .NET is not any different from for example Java, which also has the JDK (Java Development Kit) for building software and the JRE (Java Runtime Environment) for running it.</p></div>

#### Summary of .NET components

![.NET Compilation Steps](https://cdn.dusted.codes/images/blog-posts/2021-05-06/dotnet-compilation-steps.svg)

### So what is .NET?

Coming back to the original question of what is .NET? .NET is the combination of all of the different parts which have been described above. It's a platform which consists of languages, a CLI, the runtime (CLR) and an SDK for building software.

To make matters worse, .NET comes in three official versions:

- .NET Framework
- Mono
- .NET Core

Even though .NET Framework, .NET Core and Mono are officially labelled as "frameworks", these flavours of .NET are simply different implementations of the CLI.

<div class="tip">
<strong>Fun fact:</strong>
<p>In theory all three frameworks come with slightly different CLRs:</p>
<ul>
<li>CLR (Original .NET Framework CLR)</li>
<li>Mono CLR (Mono's implementation of the CLR)</li>
<li>CoreCLR (The actual name of .NET Core's CLR)</li>
</ul>
<p>The CLR is the platform specific part of .NET since it has to translate platform agnostic IL code into an architecture's specific machine code. This is why the .NET SDK and Runtime downloads come in so many different versions.</p><p>In order to support .NET on a completely new architecture (such as Apple Silicon for example) Microsoft only has to build a new architecture specific CLR (e.g. for macOS Arm64) and the rest will continue to work.</p>
</div>

## What is .NET Framework?

.NET Framework is the original .NET. It's the platform which was first released twenty years ago and which only worked on Windows and was regularly updated and shipped as part of Windows.

.NET Framework came in these major versions:

- .NET Framework 1.0
- .NET Framework 1.1
- .NET Framework 2.0
- .NET Framework 3.0
- .NET Framework 3.5
- .NET Framework 4.0
- .NET Framework 4.5
- .NET Framework 4.6
- .NET Framework 4.7
- .NET Framework 4.8

[.NET Framework 4.8](https://devblogs.microsoft.com/dotnet/announcing-the-net-framework-4-8/) was the last official version of .NET Framework and has been superseded by .NET 5 since then. There will be no new version of .NET Framework any more and all future development is conducted on .NET Core (renamed to .NET 5) and its future versions.

The original .NET Framework is what most people think of when they have negative connotations towards .NET. It was tightly coupled to Windows, the CLR could only run on Windows or Windows Server with IIS, it required Visual Studio to develop on it and it had absolutely no cross platform support. It was also relatively slow in execution and slow to evolve.

Overall it worked well on Windows but started to increasingly lack capabilities to meet modern software development demands. 

## What is Mono?

When Microsoft was still at war with Linux and had absolutely no appetite to support .NET on any other platform than Windows a crazy-genius person called [Miguel de Icaza](https://twitter.com/migueldeicaza) decided to develop the [Mono Project](https://www.mono-project.com) - an open source cross platform alternative to .NET Framework.

Mono allowed Linux, macOS and other Unix developers to write and execute .NET applications on non Windows systems. The Mono project was led by Miguel as a collaborative open source project and attracted many supporters and even investors (through Xamarin) from all around the world. Despite its initial struggles and not having had any help from Microsoft, which meant it was always lagging slightly behind the latest version of .NET Framework, Mono still managed to become a very successful standalone framework which became very popular with the ALT.NET (alternative .NET) movement and matured incredibly well over the years.

<div class="tip"><p><strong>Fun fact:</strong> Mono was only possible because it implemented the <a href="#il-code-and-the-cli">Common Language Infrastructure (CLI)</a> which Microsoft released as an open standard (ECMA-335) in December 2000.</p></div>

Only in 2016 when Microsoft changed its internal culture and started to adopt a more favourable relationship with the open source community they decided to acquire [Xamarin](https://xamarin.com/) (the company which owned Mono)  and hired Miguel de Icaza as the lead.

Today Mono is part of the official .NET eco system and powers strategic products such as [Microsoft's mobile development toolkit](https://dotnet.microsoft.com/apps/xamarin) and [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor), a .NET WASM (web assembly) runtime for running .NET in the browser.

## What is .NET Core?

.NET Core is Microsoft's latest reinvention of the legacy .NET Framework with the promise of true cross platform support and improved performance. Initially .NET Core started as a complete re-write of the .NET Framework and was initially focused on the "core" parts of the framework which were needed for running web and console applications.

Since the [release of .NET Core 1.0](https://devblogs.microsoft.com/dotnet/announcing-net-core-1-0/) Microsoft has steadily iterated over the product and eventually caught up with the original .NET Framework which led to the renaming of .NET Core to simply ".NET" again. The current version of .NET Core is called ".NET 5" and .NET 6 will be released later this year.

.NET Core has truly lived up to its promises and revived the entire .NET eco system from the first day. It runs on Linux, macOS and Windows, it is being [openly developed on GitHub](https://github.com/dotnet/core) and runs [under an OSS license](https://github.com/dotnet/core/blob/main/LICENSE.TXT), it's a more light-weight standalone product which is decoupled from Windows and incredibly fast in comparison to .NET Framework.

<div class="tip">
<p><strong>Fun fact:</strong></p>
<p>.NET Core's CLR is called CoreCLR and has its own JIT which is called <a href="https://devblogs.microsoft.com/dotnet/ryujit-the-next-generation-jit-compiler-for-net/" target="_blank">RyuJIT</a>. Both projects are also open source and available on GitHub:</p>
<ul>
<li><a href="https://github.com/dotnet/runtime/tree/main/src/coreclr" target="_blank">CoreCLR</a></li>
<li><a href="https://github.com/dotnet/runtime/tree/main/src/coreclr/jit" target="_blank">RyuJIT</a></li>
</ul>
<p>The term Ryu means dragon in Japanese and is a reference to a book about compilers famously known as the <a href="https://en.wikipedia.org/wiki/Compilers:_Principles,_Techniques,_and_Tools" target="_blank">"Dragon Book"</a>.</p>
</div>

Today .NET Core represents the foundation of all new innovation in .NET and is being released on a yearly schedule, with every second year producing an LTS (long term support) version (.NET 6 going to be the next one).

Without doubt, any new .NET developer should start with .NET 5 or higher when learning .NET.

## What is .NET Standard?

[.NET Standard is basically a short lived invention from the past](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/), but because it still lingers around many corners of the internet it is worth quickly touching on as well.

Before .NET 5 became the unification of .NET Framework and .NET Core Microsoft created a specification called the ".NET Standard" which was meant to help developers to build Framework and Core compatible applications. .NET Standard was not a framework itself, but just a blueprint (specification) of available APIs. 

It worked as following, the higher the version of .NET Framework was, the higher it would implement a version of .NET Standard. The same was true for .NET Core. This meant that a .NET developer could target a specific version of .NET Standard and then be confident that it would be compatible with certain versions of .NET Framework and .NET Core.

It was a worthwhile idea but unfortunately has always caused some confusion with .NET developers and finally got phased out with the unification of .NET 5. 

## Why is there no .NET Core 4?

.NET 5 became the first version of .NET to unify .NET Framework and .NET Core. As such it had to pick a version number which would reflect the natural progression of both frameworks and had to be higher than .NET Core 3.1 and .NET Framework 4.8. Simple as that.

<div class="tip"><p><strong>Tip:</strong> You can visit <a href="https://versionsof.net" target="_blank">versionsof.net</a> to get an overview of all existing versions of .NET, including .NET Framework, .NET Core and Mono. It also highlights which versions are in long term support.</p></div>

## What is ASP.NET?

ASP.NET is the name of .NET's web platform. It is a collection of .NET libraries to build rich web applications and comes with .NET Framework. ASP.NET inherited its name from [ASP (Active Server Pages)](https://en.wikipedia.org/wiki/Active_Server_Pages), which was Microsoft's initial server side scripting language for dynamic web pages. ASP was an interpreted language just like PHP and pretty much a Redmond copy of it. ASP.NET on the other hand is an object oriented framework which compiles into IL code like everything else in .NET Framework. It was first released with .NET Framework 1.0 and is only available on Windows.

Much like the rest of .NET Framework, ASP.NET is mostly seen as a legacy platform, which is tightly coupled to Windows and requires hosting in IIS (Internet Information Services) - a proprietary Microsoft web server.

## What is ASP.NET Core?

For a long time Microsoft was bleeding existing developers to new emerging technologies, such as Node.js, Docker or the Cloud. These tools were purposefully built to tackle the problems of modern web demand and for the most part incompatible with Microsoft's Windows centric ASP.NET. As a result Microsoft decided to develop a new version of ASP.NET with the release of .NET Core. The aim was to provide a more light weight, composable and cross platform compatible web platform which could compete with other technologies on a level playing field. ASP.NET Core was released with the first version of .NET Core and remained the main focus of its initial releases. It played a huge role in the success of .NET Core and the adoption of .NET by a whole new generation of developers, scoring consistently high in [StackOverflow's yearly developer surveys](https://insights.stackoverflow.com/survey/2020).

ASP.NET Core is the future of web in .NET and often forms the baseline library for many other web frameworks too. One of those is [ASP.NET Core MVC](https://dotnet.microsoft.com/apps/aspnet/mvc), an object oriented model-view-controller framework sitting on top of ASP.NET Core, which allows developers to build rich web applications in an object oriented class driven approach. Other notable ASP.NET Core web frameworks are [Carter](https://github.com/CarterCommunity/Carter), [Giraffe](https://github.com/giraffe-fsharp/Giraffe), [Saturn](https://saturnframework.org), [Freya](https://github.com/xyncro/freya) or [Falco](https://github.com/pimbrouwers/Falco/), which have a slightly more light-weight and functional nature to building web applications in .NET.

In addition there are also .NET web frameworks which don't require ASP.NET Core at all. Famous examples of standalone web frameworks are [WebSharper](https://websharper.com) or [Suave](https://suave.io).

Last but not least ASP.NET Core can also be used completely on its own in a more bare metal approach. It is a very popular choice and something which the ASP.NET Core team is currently focused on and will probably evangelise in the future more.

## Where do I start?

Unless someone has a very good reason not to, everyone should start with .NET Core (now .NET 5). As a developer one should download the [latest .NET SDK](https://dotnet.microsoft.com/download) and familiarise themselves with the `dotnet` command line tool.

The most important commands are `dotnet build`, which will restore dependencies and build an application, `dotnet run` which will build and launch an application and `dotnet test` which can be used to run some unit tests.

The easiest way to get started is by creating a simple console application:

```
dotnet new console
```

If someone wants to jump straight into web development I'd recommend to begin with an empty ASP.NET Core application:

```
dotnet new web
```

This is a good starting point to slowly explore ASP.NET Core as a whole and learn about the architecture of the framework and how to compose bigger applications.

A great resource to learn ASP.NET Core and find out about different project styles is [practical-aspnetcore](https://github.com/dodyg/practical-aspnetcore).

## Who is dotnet-bot?

When Microsoft started to open source .NET they didn't want that all of the existing source will get attributed to a single person. Consequently Microsoft created a new GitHub user called [dotnet-bot](https://github.com/dotnet-bot) as a placeholder for the [initial commit](https://github.com/dotnet/coreclr/commit/ef1e2ab328087c61a6878c1e84f4fc5d710aebce). Later it has evolved into .NET's official mascot:

![dotnet-bot](https://cdn.dusted.codes/images/blog-posts/2021-05-06/dotnet-bot.svg)

You can design your own dotnet-bot mod at [mod-dotnet-bot.net](https://mod-dotnet-bot.net/create-your-bot/) and also find existing swag in the [official .NET Foundation repository](https://github.com/dotnet-foundation/swag).

## Why is everything .NET?

At last one might ask why is everything called something something .NET? Isn't that causing a lot of confusion and part of the reason why guides like this have to be written in the first place? Well yes, but it's also important to remember that Microsoft is still a big corporation after all. That means that things have to be complicated enough to justify corporate prices and policies and Microsoft also doesn't want to disenfranchise all the corporate consultants who have made an entire career out of distilling everything .NET.

Just think of all the certifications alone!

![Laughing at my own joke ;)](https://cdn.dusted.codes/images/blog-posts/2021-05-06/joke.gif)

Jokes aside, there is no real reason. Someone at Microsoft probably really likes .NET as a name and because they are boss everything will remain and continue to be .NET until they eventually retire :)

## Useful links

Finally a list of some useful links:

- [.NET Homepage](https://dot.net)
- [C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [F# Documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/)
- [VB.NET Documentation](https://docs.microsoft.com/en-us/dotnet/visual-basic/)
- [NuGet.org](https://nuget.org) (npm for .NET)
- [.NET Foundation](https://dotnetfoundation.org)
- [Versions of .NET](https://versionsof.net)
- [Themes of .NET](https://themesof.net) (High level topics which the .NET team is working on)
- [Discover .NET](https://discoverdot.net) (.NET community resources)
- [BuiltWithDot.Net](https://builtwithdot.net) (Collection of projects which have been built with .NET)