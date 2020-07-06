<!--
    Tags: dotnet dotnet-core csharp fsharp vbnet
-->

# .NET for Beginners

Last night I came across this question on Reddit:

<blockquote>
    <header><h5><a href="https://www.reddit.com/r/csharp/comments/hkmnue/i_am_just_starting_to_learn_c_and_i_am_confused/">I am just starting to learn C# and i am confused about .NET framework & .NET Core</a></h5></header>
    <p>Hello, I am just starting to learn c# and i am about to start with a course on Udemy by Mosh Hamedani called "C# Basics for Beginners: Learn C# Fundamentals by Coding".
       I can see that he is using .NET Framework but i have read that .NET Core is newer and is the future? I am really not sure where to start and would appreicate if anyone could help me. I would like to learn C# to have a better understanding with OOP and to learn a programming language to help with my Univeristy course. Thank you</p>
</blockquote>

First of all congratulations to the author for learning C# which is a great choice of programming language and for putting in the extra effort into better understanding the concepts of OOP (object oriented programming)! I genuinely hope that they'll have a great experience learning .NET and most importantly that they'll enjoy picking up some new skills and have fun along the way! I remember when I started to code more than 20 years ago and how much fun it was for me! It is also great to see how many people have replied with really helpful answers and provided some useful guidance to the original thread! It is a true testament to how supportive and welcoming the .NET community is! However, despite these positive observations I'm still a little bit gutted that such a question had to be even asked in the first place. I don't imply any fault on the author themselves - quite contrary - I actually really sympathise with them and can see the sort of struggles which a new C# or F# developer may face. Questions like these really demonstrate how complex .NET has become over the years. It is a good time to take a step back and reflect!

## High cognitive entry barrier

.NET has a high cognitive entry barrier. What I mean by this is that a new developer has to quickly familiarise themselves with rather boring and complex topics way too early in their educational journey. In particular questions around the differences between .NET, .NET Core, Mono, Xamarin, C#, F#, VB.NET, what a target framework is, a runtime, and so on, add very little to almost no benefit to one's initial learning experience. Most newcomers just want to read a tutorial, watch a video or attend a class and then be able to apply their newly learned skills in an easy and uncomplicated way. Ideally educational material such as purchased online courses or books from more than a few months ago should still be relevant today. Unfortunately neither of these are true for .NET. As a matter of fact it's almost given that any content which has been written more than a year ago is largely outdated today. Just think about the [upcoming release of .NET 5](https://devblogs.microsoft.com/dotnet/introducing-net-5/) and how it will invalidate most of the lessons taught just a few months ago. Another prominent example is the very short lived [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard). Once hugely evangelised and now de-facto dead. I personally think Microsoft, the .NET team and to some extent the wider .NET community has failed in making .NET a more beginner friendly language. I say this from a good place in my heart. Programming is an extremely powerful skill which can allow people to uplift themselves from all sorts of backgrounds and I speak here from my own personal experience. The harder we make it for beginners, the more exclusive we make the entry to our profession. I think the success of the next ten years will be defined by how accessible we make .NET to a whole new population of developers and I think there's some real work for us to be done in order to improve the current situation!

## What about others?

 Before you think that the current complexity is normal in tech and nothing endemic to .NET itself then I'm afraid to prove you wrong. [Take a look at PHP](https://www.php.net). PHP has evolved a lot over the years and while it has introduced more complicated object oriented programming paradigms and improved on its performance, it hasn't lost the beauty of its original simplicity at all. PHP is a great programming language to start. Learning content exist like sand on a beach. The online community, forums, Reddits and conferences are second to none. Most importantly, a student could pass on a PHP book to their younger sibling after years of studying it and the next person would still gain a lot of value from it. The language might have moved on, but many things which were written down, particularly at a beginner level, are still 100% accurate today. This is a huge benefit which gets easily forgotten by older developers like myself. Furthermore the distinction between PHP (as in the language) and the interpreter is negligent. A new beginner doesn't have to wrap their head around such topics at all. They just know that they've installed PHP and it works as advertised.

Another great example (and there are many great examples) is [Go](https://golang.org). Go is not a new language at all. It's [been around for more than 10 years](https://opensource.googleblog.com/2009/11/hey-ho-lets-go.html) and despite huge improvements to the language, the compiler and the standard libraries it has remained faithful to its original simple design. Similar to PHP a new developer doesn't have to think about complicated nuances between "target frameworks", weird gotchas if they write Go in this IDE or that IDE and certainly they don't have to look up a complicated version matrix which explains the intricate relations between the SDK, the runtime and newly available language features. There is just one version which maps to a well defined list of Go features, bug fixes and improvements. It's documentation is simple and easily comprehensible. It is very beginner friendly.

## Improving .NET

Why is .NET so complicated? Well, the answer is not that easy itself :). It's mostly history, legacy baggage, some bad decisions along the way and what I believe an overly eager desire (almost urgency) to push boundaries for boundaries' sake. I will try to name a few selected issues from my personal observation, describe the problems which I have perceived and try to provide some constructive suggestions which I think could be a good improvement for the future.

Let me introduce you to the 6 sins of .NET:

- [Language Mash Up](#language-mash-up)
- [Version Overflow](#version-overflow)
- [.NET Everywhere](#net-everywhere)
- [Fear of being ignored](#fear-of-being-ignored)
- [Pushing Overboard](#pushing-overboard)
- [Bad Naming](#bad-naming)

### Language Mash Up

If a person sets out to learn C# (like the author from the question above), what do they learn? Is it C# or .NET? The answer is both. C# doesn't exist without .NET and you cannot program .NET without C# (or F# or VB.NET). This is not a problem in itself, but certainly where some of the issues begin. A new beginner doesn't just learn C# but also has to learn the inner workings of .NET. Things get even more confusing when C# isn't the chosen language to start with. The loose relationship between .NET languages doesn't really matter because they compile into IL and become cross compatible. [Except when they don't](https://github.com/dotnet/vblang/issues/300):

[![C# unmanaged constraint language leak](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/csharp-unmanaged-constraint-leak.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/csharp-unmanaged-constraint-leak.png)

The [BCL (Base Class Libraries)](https://docs.microsoft.com/en-us/dotnet/standard/framework-libraries#base-class-libraries) provide the foundation for all three languages, yet they are entirely written in C# only. That's not really an issue, unless entire features were only written with one language in mind (C#) and are extremely cumbersome to use from another. F# still doesn't have [native support for `Task` and `Task<T>`](https://github.com/fsharp/fslang-suggestions/issues/581), converting between `System.Collection.*` classes and F# types is a painful undertaking and F# functions and .NET `Action<T>` objects don't map very well.

The most frustrating thing though is when [changes to one language](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references) force [additional complexity on another](https://github.com/fsharp/fslang-suggestions/issues/577).

Interop between those three languages is a big burden on new developers, particularly when they start in F# and constantly struggle to make successful use of everyday BCL types. However, as painful as this might be, the interop issues are not the only complexity which a new starter has to deal with. Try to explain to a new C# user in a meaningful way when they should be using [inheritance with standard classes](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance), [interfaces](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/), [abstract classes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/abstract-and-sealed-classes-and-class-members) or [interfaces with default method implementations](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/default-interface-methods-versions). The distinctions have been narrowed down to very specific niche problems, which cannot be explained in a single coherent answer anymore.

Take this [StackOverflow question](https://stackoverflow.com/questions/2570814/when-to-use-abstract-classes) as an example. Nothing which has been described in the accepted (and most upvoted) answer below isn't also true for interfaces with default method implementations:

[![Abstract class question on StackOverflow](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/abstract-class-question-stack-overflow.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/abstract-class-question-stack-overflow.png)

Another issue is the growing variety of C# data types. When is it appropriate to create a data class, an immutable data class, a mutable struct, an immutable struct or a new [record type](https://www.stevefenton.co.uk/2020/05/csharp-9-record-types/)? Seasoned .NET developers are very opinionated about when to use which of those types and depending on who a beginner will ask, or which StackOverflow thread they'll read, they are likely going to get very different and often contradicting advice.

Non C# beginners have an even harder time to find meaningful information since most content is written for C# developers only.

Given that F# is already a [functional first multi paradigm language](https://dusted.codes/why-you-should-learn-fsharp), and C# is steaming towards that same goal too, maybe there's an opportunity to either merge both languages (I dared to say it) or at least create 100% feature parity in the future, so that the only differentiating factor remains in their syntax. F# for a functional first experience and C# for the object oriented equivalent.

### Version Overflow

As seen above, all three .NET languages evolve independently from .NET. Whilst this has many advantages on one hand it also makes things unnecessarily confusing on another. C# is heading towards [version 9](https://devblogs.microsoft.com/dotnet/welcome-to-c-9-0/), F# is approaching [version 5](https://devblogs.microsoft.com/dotnet/announcing-f-5-preview-1/) and VB.NET is already on [version 16](https://docs.microsoft.com/en-us/dotnet/visual-basic/getting-started/whats-new#visual-basic-160). Meanwhile .NET Framework is on [version 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) and .NET Core on [version 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1). Don't even get me started on [Mono](https://www.mono-project.com), [Xamarin](https://dotnet.microsoft.com/apps/xamarin) or [ASP.NET](https://docs.microsoft.com/en-us/aspnet/core).

Now I understand that all these things are very different and I'm comparing apples with oranges, but how is a new developer supposed to know all of that given the sheer amount of highly correlated (and yet different) things? It's not easy to digest and particularly overwhelming if a beginner is presented with a screens like this:

[![.NET Core Versions](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-core-versions.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-core-versions.png)

Let's be honest, even .NET developers with 5+ years of experience don't find the above information easy to comprehend. I know that for a fact, because I ask that question in interviews a lot and it's rare to get a correct explanation. The problem is not that it's too verbose, or wrong, or unnecessary information, but rather the unfortunate case that this is just how big .NET has become. In fact it's even bigger but I believe that the .NET team has already tried their best to condense this screen into the simplest form possible. I understand the difficulty - if you've got a very mature and feature rich platform then there's a lot to explain - but nevertheless it's not a particularly good look.

In contrast to .NET this is the cognitive load which is thrown at a beginner in Go:

[![Go Versions](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/go-versions.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/go-versions.png)

It's much simpler in every way. Admittedly it's not an entirely fair comparison because Go gets compiled directly into machine code and therefore there isn't a distinction between SDK and runtime, but my point still stands: there is plenty of opportunity to make things simpler!

I don't have a concrete idea on how to improve .NET's situation in this regard, but I don't think that what we see here today is the best we can do. Maybe there's some value in officially aligning languages, ASP.NET Core and .NET (Core) versions together and ship one coherent release every time?

### .NET Everywhere

Now this one will probably hit some nerves. One of the \*big\* problems with .NET is that Microsoft is obsessed with the vision of [.NET Everywhere](https://www.hanselman.com/blog/NETEverywhereApparentlyAlsoMeansWindows311AndDOS.aspx). Every [new development](https://visualstudiomagazine.com/articles/2020/06/30/uno-visual-studio.aspx) aims at [unifying everything](https://devblogs.microsoft.com/dotnet/introducing-net-multi-platform-app-ui/) into [one big platform](https://dotnet.microsoft.com/learn/dotnet/what-is-dotnet), catering for [every](https://dotnet.microsoft.com/apps/xamarin/mobile-apps) [single](https://dotnet.microsoft.com/apps/iot) [possible](https://dotnet.microsoft.com/apps/gaming) [use](https://dotnet.microsoft.com/apps/cloud) [case](https://dotnet.microsoft.com/apps/desktop) [imaginable](https://dotnet.microsoft.com/apps/machinelearning-ai):

[![.NET Everywhere](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-everywhere.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-everywhere.png)

In many ways it makes a lot of sense, but the angle taken is causing more harm than help. While it makes a lot of sense to walk on a stage and boast to potential customers that your product can solve all their existing and future problems, it's not always the best approach when you actually want to on-board thousands of new developers.

Unifying the entire stack into a single .NET platform takes its toll. For years things have been constantly moved around, new things have been created and others have been dropped. Only recently I had to re-factor my ASP.NET Core startup code again:

[![.NET Core 3.1 - Refactoring web host to generic host](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-generic-web-host-builder.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-generic-web-host-builder.png)

Every attempt to unify things for unification's sake makes simple code more verbose. Without a doubt the previous code was a lot easier to understand. I applaud the concept of a [generic host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1), but one has to wonder how often does a developer actually want to combine two servers into one to justify the added complexity by having a builder inside another builder? How often does a developer want to \*create\* a web server but not \*run\*? I'm sure these edge cases do exist, but why can't they be hidden from the other 99.9% of user scenarios?

As nice and useful as the builder pattern may be, and whatever architectural benefits the lambda expression might give, to a C# newbie who just wants to create a hello world web application this is insane!

Remember, this is the equivalent in Go:

```
router := ... // Equivalent to the Startup class in .NET

if err := http.ListenAndServe(":5000", router); err != nil {
    // Handle err
}
```

Microsoft .NET's Swiss Army Knife approach creates an unnecessary burden on new developers on many different levels.

Here's the output of all the default .NET templates which get shipped as part of the .NET CLI (minus the Giraffe template):

[![.NET Project Templates](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-new-command.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-new-command.png)

They barely fit on a single screen.

Speaking of fitting things onto a single screen...

[![Visual Studio Chaos](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/visual-studio-chaos.png)](https://twitter.com/dylanbeattie/status/832326857798348800)

My honest constructive feedback is **\*less is more\***!

It's rarely the case that a new .NET developer wants to be confronted with a whole lot of options before they can start to write some basic code. Most newcomers just want to get the bare bones minimum set up to get up and running.

I totally support the idea of a single .NET platform which can cater to all needs, but I'd clearly separate the marketing material from the developer centred things. Visual Studio's new project dialogue doesn't have to match Microsoft's marketing slides and the `dotnet new` command doesn't have to

Optimise first time experiences for beginners, not for advanced users. There's a reason why advanced developers are called advanced. If they need any extras they will know where to find them.

### Fear of being ignored

Another problem which I have observed over the years is Microsoft's fear of being ignored. Microsoft has a solution for almost everything. It's an amazing achievement and something to be really proud of (and I really mean it), but at the same time Microsoft has to learn to give developers some space.

Microsoft does not miss a single opportunity to advertise a range of products when you look at one. You want to do .NET development? Oh look, here is Visual Studio, oh and by the way you can deploy to IIS, oh and also Azure if you plan on running in the cloud. How are you going to deploy there? Well, we have Azure DevOps, a super cool feature rich CI/CD pipeline. You don't have to use it, but just letting you know that it's there! Why don't you have a look at aaaaaaaall of these things whilst you're just interested in .NET, after all you never know, you might like some of it, right?

[![Look at me - Attention seeker](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/look-at-me.gif)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/look-at-me.gif)

Again, I totally get why Microsoft present itself that way. I'm sure it comes from a good place wanting to help developers rather than an evil plan to up-sell at every occasion, but unfortunately it doesn't come across this way.

What Microsoft tries to say is "Hey, look we're different now, we are cross platform, we run everywhere and we want you to have a great experience. Here's a bunch of things which can help you on your first day".

Unfortunately what new users understand is "Hey, so .NET is a Microsoft product and if you really want to have a smooth experience then you must use other Microsoft products".

This is why the perception of .NET is changing slowly. It's not that the "we run everywhere" message hasn't reached, it's just that when developers come and explore they get a very different impression from what they hoped to see.

My suggestion is to de-clutter all developer visited pages, home screens, hero images and so on from anything which isn't directly related to the product in focus. When a new beginner visits the pages for .NET then don't bother them with Visual Studio straight away. Trust in the excellence of your own products and have the confidence that when a new developer starts to look for an IDE they will come across yours too.

### Pushing Overboard

### Bad Naming

No matter what you think, how used you got to it or how much you subjectively like it, .NET is not a great name.

## Conclusion

C#, F# and the whole of .NET is a great development platform. I've been working with it for many years and mostly enjoyed myself, however I won't lie and pretend that things have gotten fairly complicated recently. I count myself as extremely lucky to have started with .NET when things were a lot simpler back then and I'm not sure how I would feel as a new beginner today. The entire tech industry is becoming more complicated and whilst our field is one of the highest in demand it is also one of the steepest to learn. I want .NET to become easier again, so that the next generation of C#, F# and VB.NET developers can have the same wonderful experience as I did. Some things have gotten significantly better, but others worse. The issues which I have described above are some of the biggest sticking points at the moment, at least from my limited observation. These problems make it hard for new people to start in .NET and they make experienced developers doubt whether .NET is a good platform to recommend. .NET is a lot of fun, but to many it currently feels like more pay than play.