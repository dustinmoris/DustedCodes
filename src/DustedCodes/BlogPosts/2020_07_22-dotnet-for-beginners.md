<!--
    Tags: dotnet dotnet-core csharp fsharp vbnet
-->

# .NET for Beginners

Last night I came across this question on Reddit:

<blockquote>
    <header><h5><a href="https://www.reddit.com/r/csharp/comments/hkmnue/i_am_just_starting_to_learn_c_and_i_am_confused/">I am just starting to learn C# and i am confused about .NET framework & .NET Core</a></h5></header>
    <p>Hello, I am just starting to learn c# and i am about to start with a course on Udemy by Mosh Hamedani called "C# Basics for Beginners: Learn C# Fundamentals by Coding".
       I can see that he is using .NET Framework but i have read that .NET Core is newer and is the future? I am really not sure where to start and would appreciate if anyone could help me. I would like to learn C# to have a better understanding with OOP and to learn a programming language to help with my University course. Thank you</p>
</blockquote>

First of all congratulations to the author for learning C# which is a great choice of programming language and for putting in the extra effort into better understanding the concepts of OOP (object oriented programming)! I genuinely hope that they'll have a great experience learning .NET and most importantly that they'll enjoy picking up some new skills and have fun along the way! I remember when I started to code (more than 20 years ago) and how much fun it was for me! It is also great to see how many people have replied with really helpful answers and provided some useful guidance to the original thread! It is a true testament to how supportive and welcoming the .NET community is! However, despite these positive observations I'm still a little bit gutted that such a question had to be even asked in the first place. I don't imply any fault on the author themselves - quite contrary - I actually really sympathise with them and can understand the sort of struggles which a new C# or F# developer may face. Questions like these really demonstrate how complex .NET has become over the years. It is a good time to take a step back and reflect!

## High cognitive entry barrier

.NET has a high cognitive entry barrier. What I mean by this is that a new developer has to quickly familiarise themselves with rather boring and complex topics way too early in their educational journey. In particular questions around the differences between .NET, .NET Core, Mono, Xamarin, and the relation between C#, F#, VB.NET and what a target framework or a runtime is add very little to almost no benefit to one's initial learning experience. Most newcomers just want to read a tutorial, watch a video or attend a class and then be able to apply their newly learned skills in an easy and uncomplicated way. Ideally educational material such as purchased online courses or books from more than a few months ago should still be relevant today. Unfortunately neither of these are true for .NET. As a matter of fact it's almost given that any content which has been written more than a year ago is largely outdated today. Just think about the [upcoming release of .NET 5](https://devblogs.microsoft.com/dotnet/introducing-net-5/) and how it will invalidate most of the lessons taught just a few months ago. Another prominent example is the very short lived [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard). Once hugely evangelised and now de-facto dead. I personally think Microsoft, the .NET team and to some extent the wider .NET community has failed in making .NET a more beginner friendly language. I say this from a good place in my heart. Programming is an extremely powerful skill which can allow people to uplift themselves from all sorts of backgrounds and the harder we make it for beginners, the more exclusive we make the entry to our profession. I think the success of the next ten years will be defined by how accessible we make .NET to a whole new population of developers and I think there's some real work to be done in order to improve the current situation!

## What about others?

 Before you think that the current complexity is normal in programming and nothing endemic to .NET itself then I'm afraid to prove you wrong. [Take a look at PHP](https://www.php.net). PHP has evolved a lot over the years and while it has introduced more complicated object oriented concepts and improved on its performance, it hasn't lost the beauty of its original simplicity at all. PHP is a great programming language to learn. Beginner content exist like sand on a beach. The online community, forums, Reddits and conferences are second to none. Most importantly, a student could pass on a several years old PHP book to another person and they would still get a lot of value from it. The language might have moved on, but many things which were taught on a beginner level are still 100% accurate today. This is a huge advantage which gets easily forgotten by more advanced developers like myself. Furthermore the distinction between language and the interpreter is not something which a new PHP developer has to understand. A new beginner doesn't have to wrap their head around such relatively unimportant topics to get started. They just know that they've installed PHP and it works.

Another great example (and there are many great examples) is [Go](https://golang.org). Go is not a new language at all. It's [been around for more than 10 years](https://opensource.googleblog.com/2009/11/hey-ho-lets-go.html) and despite huge improvements to the language, the compiler and the standard library it has remained faithful to its original simple design. Similar to PHP a new developer doesn't have to think about complicated nuances between "target frameworks", weird gotchas if they write Go in one IDE or another and they certainly don't have to look up a complicated version matrix to understand the intricate relations between an SDK, the runtime and newly available language features. There is just one version which maps to a well defined list of features, bug fixes and improvements in Go. It's documentation is simple and easy to comprehend. It is a very beginner friendly language.

## Improving .NET

Why is .NET so complicated? Well, the answer is perhaps not that easy itself. It's mostly history, legacy baggage, some bad decisions in the past and what I believe an overly eager desire (almost urgency) to change things for changes' sake. I will try to name a few selected issues from my personal observation, describe the problems which I have perceived and try to provide some constructive suggestions which I think could be a good improvement for the future.

Let me introduce you to the **6 Sins of .NET**:

- [Language Spaghetti](#language-spaghetti)
- [Version Overflow](#version-overflow)
- [.NET Everywhere](#net-everywhere)
- [All Eyez on Me](#all-eyez-on-me)
- [Architecture Break Down](#architecture-break-down)
- [Name Overload](#name-overload)

### Language Spaghetti

If a person sets out to learn C# (like the author from the question above), what do they learn? Is it C# or .NET? The answer is both. C# doesn't exist without .NET and you cannot program .NET without C# (or F# or VB.NET for that matter). This is not a problem in itself, but certainly where some of the issues begin. A new beginner doesn't just learn C# but also has to learn the inner workings of .NET. Things get even more confusing when C# isn't the initial .NET language to learn. Supposedly the loose relationship between .NET languages shouldn't really matter because they compile into IL and become cross compatible. [Except when they don't](https://github.com/dotnet/vblang/issues/300):

[![C# unmanaged constraint language leak](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/csharp-unmanaged-constraint-leak.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/csharp-unmanaged-constraint-leak.png)

The [BCL (Base Class Libraries)](https://docs.microsoft.com/en-us/dotnet/standard/framework-libraries#base-class-libraries) provide the foundation for all three languages, yet they are only written in C#. That's not really an issue unless entire features were written with only one language in mind and are extremely cumbersome to use from another. For example, F# still doesn't have [native support for `Task` and `Task<T>`](https://github.com/fsharp/fslang-suggestions/issues/581), converting between `System.Collection.*` classes and F# types is a painful undertaking and F# functions and .NET `Action<T>` objects don't map very well.

The most frustrating thing though is when [changes to one language](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references) force [additional complexity on another](https://github.com/fsharp/fslang-suggestions/issues/577).

Interop issues between those three languages are a big burden on new developers, particularly when they start with something else but C#. However, as painful as this might be, interop problems are not the only complexity which a new beginner has to face. Try to explain to a new C# user in a meaningful way when they should use [inheritance with standard classes](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance), [interfaces](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/), [abstract classes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/abstract-and-sealed-classes-and-class-members) or [interfaces with default method implementations](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/default-interface-methods-versions). The differences between those options have been so watered down that one cannot explain the distinctions in a single coherent answer anymore.

Take this [StackOverflow question](https://stackoverflow.com/questions/2570814/when-to-use-abstract-classes) for an example. Nothing which has been described in the accepted (and most upvoted) answer below isn't also true for interfaces with default method implementations today:

[![Abstract class question on StackOverflow](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/abstract-class-question-stack-overflow.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/abstract-class-question-stack-overflow.png)

Another great example is the growing variety of C# data types. When is it appropriate to create a [data class](https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5), an [immutable data class](https://www.c-sharpcorner.com/article/all-about-c-sharp-immutable-classes2/), a [mutable struct](http://mustoverride.com/tuples_structs/), an [immutable struct](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/struct#readonly-struct), a [tuple class](https://docs.microsoft.com/en-us/dotnet/api/system.tuple?view=netcore-3.1), the new concept of [named tuples](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples) or the upcoming [record type](https://www.stevefenton.co.uk/2020/05/csharp-9-record-types/)?

Seasoned .NET developers are very opinionated about when to use each of these types and depending on who a new beginner will ask, or which StackOverflow thread they'll read, they will most likely get very different and often contradicting advice. Anyone who doesn't think that this is a massive problem must be in serious denial. Learning a programming language is hard enough on its own. Learning a programming language where two mentors (who you might look up to) give you contradicting advice is even harder.

The irony is that many of the newly added language and framework features are supposed to make .NET easier to learn:

[![Make .NET easier](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-easier.png)](https://twitter.com/shanselman/status/1281856685657616384)

*(BTW, I have been a huge proponent of dropping the `.csproj` and `.sln` files for a very long time but previously Microsoft employees defended them as if someone offended their family, so it's nice to finally see some support for that idea too! :))*

Don't get me wrong, I agree with Scott that \*this particular feature\* will make writing a first hello world app a lot easier than before, however, our friend Joseph Woodward makes a good point that nothing comes for free:

[![With great power comes great responsibility](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-reboot-3.png)](https://twitter.com/joe_mighty/status/1281899362281566208)

And he's not alone with this idea:

[![.NET is hard](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-hard-2.png)](https://twitter.com/buhakmeh/status/1281985930279223298)

[![.NET is hard](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-hard-3.png)](https://twitter.com/FransBouma/status/1282059062247555082)

It's not just about learning how to write C#. A huge part of learning .NET is reading other people's code, which is also getting inherently more difficult as a result:

[![.NET is hard](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-hard-4.png)](https://twitter.com/1amjau/status/1281908190167347200)

Whilst I do like and support the idea of adding more features to C#, I cannot ignore the fact that it also takes its toll.

Some people raised a good point that it might be time to consider making some old features obsolete:

[![.NET is hard](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-reboot-2.png)](https://twitter.com/RustyF/status/1281860368369963008)

[![.NET is hard](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/tweet-dotnet-reboot-1.png)](https://twitter.com/RehanSaeedUK/status/1281943982189228032)

Whatever one's personal opinion is, &quot;feature bloat&quot; is certainly becoming a growing concern in the C# community and Microsoft would be stupid not to listen or at least take some notes.

Given that F# is already a [functional first multi paradigm language](https://dusted.codes/why-you-should-learn-fsharp), and C# is definitely heading towards that direction too, maybe one day there's a future opportunity to consolidate both languages into one? (YES, I dared to suggest it!) Either that, or Microsoft should establish 100% feature parity so that interop is seamless between the languages and the only differentiating factor remains in their syntax - one geared towards a functional first experience and the other towards the object oriented equivalent.

### Version Overflow

As mentioned above, all three .NET languages evolve independently from .NET. C# is heading towards [version 9](https://devblogs.microsoft.com/dotnet/welcome-to-c-9-0/), F# is approaching [version 5](https://devblogs.microsoft.com/dotnet/announcing-f-5-preview-1/) and VB.NET is already on [version 16](https://docs.microsoft.com/en-us/dotnet/visual-basic/getting-started/whats-new#visual-basic-160). Meanwhile .NET Framework is on [version 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) and .NET Core on [version 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1). Don't even get me started on [Mono](https://www.mono-project.com), [Xamarin](https://dotnet.microsoft.com/apps/xamarin) or [ASP.NET](https://docs.microsoft.com/en-us/aspnet/core).

Now I understand that all these things are very different and I'm comparing apples with oranges, but how is a new developer supposed to know all of that? All these components are independent and yet correlated enough to overwhelm a new developer with screens like this:

[![.NET Core Versions](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-core-versions.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-core-versions.png)

Even .NET developers with 5+ years of experience find the above information hard to digest. I know this for a fact because I ask this question in interviews a lot and it's rare to get a correct explanation. The problem is not that this information is too verbose, or wrong, or unnecessary to know, but rather the unfortunate case that it's just how big .NET has become. In fact it's even bigger but I believe that the .NET team has already tried their best to condense this screen into the simplest form possible. I understand the difficulty - if you've got a very mature and feature rich platform then there's a lot to explain - but nevertheless it's not a particularly sexy look.

In contrast to .NET this is the cognitive load which is thrown at a beginner in Go:

[![Go Versions](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/go-versions.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/go-versions.png)

It's much simpler in every way. Admittedly it's not an entirely fair comparison because Go gets compiled directly into machine code and therefore there isn't a real distinction between SDK and runtime, but my point is still the same. There is certainly plenty of room for improvement and I don't think what we see there today is the best we can do.

Maybe there's some value in officially aligning language, ASP.NET Core and .NET (Core) versions together and ship one coherent release every time? Fortunately .NET 5 is the right step in that direction but in my opinion there's still more to do!

### .NET Everywhere

Now this one will probably hit some nerves, but one of the \*big\* problems with .NET is that Microsoft is obsessed with the idea of [.NET Everywhere](https://www.hanselman.com/blog/NETEverywhereApparentlyAlsoMeansWindows311AndDOS.aspx). Every [new development](https://visualstudiomagazine.com/articles/2020/06/30/uno-visual-studio.aspx) aims at [unifying everything](https://devblogs.microsoft.com/dotnet/introducing-net-multi-platform-app-ui/) into [one big platform](https://dotnet.microsoft.com/learn/dotnet/what-is-dotnet), catering for [every](https://dotnet.microsoft.com/apps/xamarin/mobile-apps) [single](https://dotnet.microsoft.com/apps/iot) [possible](https://dotnet.microsoft.com/apps/gaming) [use](https://dotnet.microsoft.com/apps/cloud) [case](https://dotnet.microsoft.com/apps/desktop) [imaginable](https://dotnet.microsoft.com/apps/machinelearning-ai):

[![.NET Everywhere](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-5-everywhere.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-5-everywhere.png)

*(Thanks to the courtesy of [Ben Adams](https://twitter.com/ben_a_adams) I've updated the graphic to represent the [full picture of .NET](https://twitter.com/ben_a_adams/status/1286144227819257856). Ben created this image for the purpose of his own blog which you can read on [www.ageofascent.com/blog](https://www.ageofascent.com/blog/).)*

In many ways it makes a lot of sense, but the angle taken is causing more harm than help. While it makes a lot of sense to walk on a stage and boast to potential customers that your product can solve all their existing and future problems, it's not always the best approach when you actually want to on-board new developers from all sorts of different industries.

Unifying the entire stack into a single .NET platform doesn't come without a price. For years things have been constantly moved around, new things have been created and others have been dropped. Only recently I had to re-factor my ASP.NET Core startup code yet again:

[![.NET Core 3.1 - Refactoring web host to generic host](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-generic-web-host-builder.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-generic-web-host-builder.png)

Every attempt at unifying things for unification's sake makes simple code more verbose. Without a doubt the previous code was a lot easier to understand. I applaud the concept of a [generic host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1), but one has to wonder how often does a developer actually want to combine two servers into one? In my opinion there must be a real benefit in order to justify complexity such as having a builder inside another builder! How often does a developer want to \*create\* a web server and not \*run\* it as well? I'm sure these edge cases do exist, but why can't they be hidden from the other 99.9% of use cases which normally take place?

As nice and useful as the builder pattern may be, and whatever architectural benefits the lambda expression might give, to a C# newbie who just wants to create a hello world web application this is insane!

Remember, this is the equivalent in Go:

```
router := ... // Equivalent to the Startup class in .NET

if err := http.ListenAndServe(":5000", router); err != nil {
    // Handle err
}
```

Microsoft's &quot;Swiss Army Knife&quot; approach creates an unnecessary burden on new .NET developers in many different ways.

For example, here's the output of all the default .NET templates which get shipped as part of the .NET CLI (minus the Giraffe one):

[![.NET Project Templates](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-new-command.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-new-command.png)

They barely fit on a single screen. Again, it's great that Microsoft has Blazor as an answer to WASM, or that they have WPF as an option for Windows, but why are these things shipped together as one big ugly thing? Why can't there just be a template for a console app or class library and then some text which explains how to download more? This is a classic example where &quot;.NET Everywhere&quot; is getting into most users' way!

Speaking of fitting things into a single screen...

[![Visual Studio Chaos](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/visual-studio-chaos.png)](https://twitter.com/dylanbeattie/status/832326857798348800)

Whilst the above tweet was comical in nature, it's not far from the truth.

My honest constructive feedback is **\*less is more\***!

It's rarely the case that a new .NET developer wants to be drowned in a sea of options before they can get started with some basic code. Most newcomers just want to set up the bare bones minimum to get up and running and write their first lines of code. They don't even care if it's a console app or an ASP.NET Core application.

I totally support the idea of a single .NET which can cater to all sorts of different needs, but it must be applied in a much less overwhelming way. Visual Studio's new project dialogue doesn't need to match Microsoft's marketing slides and the `dotnet new` command doesn't have to ship a template for every single type of app. It doesn't happen very often that a developer is first tasked to work on a line of business web application, then told to build a Windows forms store app and later asked to build a game. Absolutely nobody needs twenty different templates which span across ten different industries on their machine.

My advice would be to optimise the .NET experience for beginners and not worry about advanced developers at all. There's a reason why advanced users are called advanced. If a senior developer wants to switch from iOS to IoT development then they will know where to find the tools. Currently the .NET CLI ships FIFTEEN!!! different web application templates for a user to pick. How is a new C# developer supposed to decide on the right template if even experienced .NET developers scratch their head? Microsoft must understand that not bloating every tool or IDE with a million different options doesn't mean that users don't understand that these options do exist.

In my opinion the whole mentality around &quot;.NET Everywhere&quot; is entirely wrong. It should be a positive side effect and not the goal.

### All Eyez on Me

Another problem which I have observed is Microsoft's fear of being ignored. Microsoft has a solution for almost every problem which a developer might face. It's an amazing achievement and something to be really proud of (and I really mean it), but at the same time Microsoft has to learn how to give developers some space.

Microsoft does not miss a single opportunity to advertise a whole range of products when someone just looks at one. Doing .NET development? Oh look, here is Visual Studio which can help you with that! Oh and by the way you can deploy directly to IIS! In case you wonder, IIS comes with Windows Server which also runs in Azure! Aha, speaking of Azure, did you know that you can also click this button which will automatically create a subscription and deploy to the cloud? In case you don't like right-click-publish we also have Azure DevOps, which is a fully featured CI/CD pipeline! Of course there's no pressure, but if you \*do sign up now\* then we'll give you free credits for a month! Anyway, it's just a &quot;friendly reminder&quot; so you know that in theory we offer the full package in case you need it! C'mon look at me, look at me, look at me now!

[![Look at me - Attention seeker](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/look-at-me.gif)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/look-at-me.gif)

Again, I totally understand why Microsoft does what they do (and I'm sure there's a lot of good intention there), but it comes across the complete wrong and opposite way.

No wonder that the perception of .NET hasn't changed much in the outside world:

[![.NET coming across the wrong way](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/dotnet-microsoft-bloatware.png)](https://twitter.com/blazorguy/status/1279092538490736640)

What Microsoft really tries to say is:

> Hey folks, look we're different now, we are cross platform, we run everywhere and we want you to have a great experience. Here's a bunch of things which can help you on your journey.

Unfortunately what users \*actually\* understand is:

> Hey, so .NET is a Microsoft product and it mostly only works with other Microsoft products, so here's a bunch of stuff which you will have to use!

On one hand Microsoft wants to create this new brand of an open source, cross platform development tool chain and yet on another they push Visual Studio and Azure whenever they talk .NET. This is sending mixed messages and confusing new developers, detracting from .NET's actual brilliance and doing a massive disservice to the entire development platform. The frustrating thing is that the more .NET is becoming open to the world, the more Microsoft is pushing for their own proprietary tools. Nowadays when you watch some of the most iconic .NET employees giving a talk on .NET then it's only 50% .NET and the rest advertising Windows, Edge and Bing. This is not what most .NET developers came to see and it doesn't happen in other programming communities such as Node.js, Rust or Go either. Besides that, if someone constantly advertises every new Microsoft flavour of the month then they also lose developer credibility over time.

The other thing is that [questions and answers like these](https://www.reddit.com/r/csharp/comments/htlgsr/vs_or_vs_code_problem/) need to stop:

[![Visual Studio vs. Visual Studio Code](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/visual-studio-vs-visual-studio-code-question.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/visual-studio-vs-visual-studio-code-question.png)

This question and particularly the answer are really bad, because they demonstrate that whilst C# and .NET are not necessarily tied to Visual Studio and Windows anymore, they still remain the most viable option to date. This sentiment is not good but unfortunately true. I know from my own experience that the [Visual Studio Code plugin for C#](https://code.visualstudio.com/Docs/languages/csharp) is nowhere near as good as it should be. The same applies to F#. Why is that? It's not that Visual Studio Code is less capable than Visual Studio, but rather a decision by Microsoft to give it a lower priority and a lack of investment. I don't need to use [JetBrains GoLand](https://www.jetbrains.com/go/) in order to be productive in Go, but I have to use [Rider](https://www.jetbrains.com/rider/) for .NET.

Microsoft needs to decouple .NET from everything else and make it a great standalone development environment if they want to compete with the rest.

C#, F# and .NET will always be perceived as a very Microsoft and Windows centric development environment when even the [official .NET documentation](https://docs.microsoft.com/en-us/dotnet/) page confirms a critic's worst fears:

[![.NET Documentation](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/official-dotnet-docs.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/official-dotnet-docs.png)

### Architecture Break Down

Go is 10 years old.

.NET Core is 4 years old - not even half the age of Go.

Go is currently on version 1.14 and .NET Core is already on its third major version. Both have been written from the ground up, but Microsoft has had arguably more experience developing .NET Core given that it was re-written from scratch with the knowledge of more than 14 years of supporting .NET Framework. How on earth did .NET Core end up with so many architectural changes that it is already on version 3.x?

Microsoft prides itself with developing .NET Core extremely fast and offering a comprehensive out of the box solution, but it is one of the most unstable and tiresome development environments which I have worked with in recent years. It is becoming increasingly exhausting trying to keep up with .NET Core's constant change. Microsoft would implement one thing one day and then completely replace it the other. Just think about the constant changes to the `Startup.cs` class, or the ever evolving `HttpClient` and its related helper types, the invention and demise of the JSON project file, .NET Standard or various JSON serialisers. The list goes on. Features such as CORS, routing and authorisation keep changing as more code gets rewritten and pushed down the pipeline and more types are being made obsolete from the `Microsoft.AspNetCore.*` namespace and replaced with new ones emerging in `Microsoft.Extensions.*`.

It's hard to keep up with .NET as an experienced developer let alone as a beginner. This constant change significantly reduces the lifespan and usefulness of books, videos, online tutorials, StackOverflow questions, Reddit threads and community blog posts. It's not only making .NET by an order of magnitude harder to learn but also financially less viable than others.

Good software and framework architecture should provide a stable foundation which is open for extension, but closed for change (sound familiar?). There was no need to implement v1 of ASP.NET Core in such a way that it now requires constant architectural change in order to support new innovation. Why has endpoint routing not been build on day one? Why does Microsoft not provide an adequate and feature complete replacement for `Newtonsoft.Json` before they released and encouraged the usage of `System.Text.Json`? Why are light weight and easy to understand routing handlers such as `MapGet` only an afterthought? Why is it that Microsoft never creates a GitHub issue for an existing successful .NET OSS project when they need something similar (maybe with a few changes or improvements) and rather invent their own competing in-house product which \*always\* causes pain in the community and indirectly forces users to re-write their codebase yet again?

There is no actual need to do any of this, only self imposed deadlines which force Microsoft to release ill written software, badly thought out framework features and an unnecessary burden on its current developer community. This constant change is extremely painful to say the least. It's the single worst feature of .NET and the main reason why I honestly couldn't recommend .NET to a programming novice in good faith.

There is really not much else to say other than **\*slow - down\***. I wish the .NET and ASP.NET Core teams would take this criticism (which isn't new) more serious and realise how bad things have become. I know I keep banging about Go, but surely there is some valuable lesson to learn given how popular and successful Go has become in a relatively short amount of time? Maybe Go is too simple in comparison to .NET, but maybe the current pace of .NET is not the right approach either? It's important to remember that a less breaking .NET would pose such a smaller mental and financial burden on new developers from all across the world!

### Name Overload

This blog post wouldn't be complete without mentioning Microsoft's complete failure of naming .NET properly in a user and beginner friendly way. I mean what is &quot;.NET&quot; anyway? First and foremost it's a TLD, which has nothing to do with Microsoft! Secondly there is no clear or uniform way of spelling .NET. Is it &quot;.NET&quot; or &quot;dot net&quot;? Maybe it was &quot;DOTNET&quot; or it could be &quot;dot.net&quot; like the newly registered domain [dot.net](https://dot.net)? My friends still tease me by calling it &quot;DOT NOT&quot; whenever I mention it to them!

Finally when there was an opportunity to correct this long standing mistake by re-writing the entire platform and possibly giving it a new name then Microsoft decided to call it &quot;.NET Core&quot;. If anyone thought it couldn't get any worse than Microsoft surely didn't disappoint! I cannot think of a more internet unfriendly name than &quot;.NET Core&quot;. How do you even hashtag this? I've seen it all ranging from [#dotnet](https://twitter.com/hashtag/dotnet) to [#dot-net](https://twitter.com/hashtag/dot-net), [#dot-net-core](https://twitter.com/hashtag/dot-net-core), [#dotnet-core](https://twitter.com/hashtag/dotnet-core), [#netcore](https://twitter.com/hashtag/netcore), [#net-core](https://twitter.com/hashtag/net-core), [#dot-netcore](https://twitter.com/hashtag/dot-netcore) and [#dotnetcore](https://twitter.com/hashtag/dotnetcore).

I think everyone can agree that objectively speaking &quot;.NET Core&quot; was never a great name. Needless to say that &quot;.NET Core&quot; also completely messes with the internet history for &quot;.NET Framework&quot;, which is exactly what everyone predicted before.

At least Microsoft is consistent with its naming. There's something comical in the fact that the only three supported languages in .NET are called CSharp, F# and VB.NET. Or was it C#, F Sharp and Visual Basic? Anyway, it was some combination of the three!

## Final words

C#, F# and the whole of .NET is a great development platform to code, but it has also become overly complex which is holding new developers back. I've been working with it for many years and mostly enjoyed myself, however I won't lie and say that things haven't gotten a little bit out of hand lately. There is something to tell that after having .NET for 20 years the programming community still hasn't seen anything new or noteworthy since the creation of [stackoverflow.com](https://stackoverflow.com):

[![Famous .NET website question on Quora](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/famous-dotnet-websites.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-07-05/famous-dotnet-websites.png)

Meanwhile we've seen very prominent products being built with other languages spanning across multiple domains such as developer technologies ([Docker](https://www.docker.com), [Kubernetes](https://kubernetes.io), [Prometheus](https://github.com/prometheus)) to smaller static website generators ([Hugo](https://gohugo.io)) or some of the most successful FinTech startups ([Monzo](https://monzo.com)) in the world.

.NET is a great technology for experienced developers who grew up with the platform as it matured, but I'm not sure if I'd still enjoy learning it today as much as I did in 2008. Whilst the complexity allows me to charge great fees to clients for writing software in .NET, I'd probably not recommend it to a friend who wants to learn how to code or I wouldn't use it for building my own startup from the grounds up.

The future success of .NET will be based on the developers which it can attract today.

The success of .NET in ten years will be based on the decisions made today.

I hope those decisions will be made wisely!