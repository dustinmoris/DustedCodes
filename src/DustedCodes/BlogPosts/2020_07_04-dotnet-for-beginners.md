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

First of all congratulations to the author for learning C# which is a great choice of programming language and for the extra effort being put in to better understand the concepts of OOP (object oriented programming)! I genuinely hope that they'll have a great experience learning .NET and most importantly that they'll enjoy picking up some new skills! I remember when I started to code more than 20 years ago and how much fun it was for me! It is also great to see how many people have replied with really helpful answers and provided some useful guidance to the original thread! It is a true testament to how supportive and welcoming the .NET community is! However, despite these positive observations I'm still a little bit gutted that such a question had to be even asked in the first place. I don't imply any fault on the author themselves - quite contrary - I actually really sympathise with them and can see the sort of struggles which a new C# or F# developer may face. Questions like these really demonstrate how complex .NET has become over the years. It is a good time to take a step back and reflect!

## High cognitive entry barrier

.NET has a high cognitive entry barrier. What I mean by this is that a new developer has to quickly familiarise themselves with rather boring and complex topics way too early in their educational journey. In particular questions around the differences between .NET, .NET Core, Mono, Xamarin, C#, F#, VB.NET, what a target framework is, a runtime, and so on, add very little to almost no benefit to one's initial learning experience. Most newcomers just want to read a tutorial, watch a video or attend a class and then be able to apply their newly learned skills in an easy and uncomplicated way. Ideally educational material such as purchased online courses or books from more than a few months ago should still be relevant today. Unfortunately neither of these things are true for .NET. They rarely are. As a matter of fact it's almost given that any content which has been written more than a year ago is largely outdated today. Just think about the upcoming release of .NET 5 and how it will invalidate most of the lessons taught just a few months ago. Another prominent example is the very short lived .NET Standard. Once hugely evangelised and now de-facto dead. I personally think Microsoft, the .NET team and to some extent the wider .NET community has somewhat failed in making .NET a much more beginner friendly language. I say this from a good place in my heart. Programming is an extremely powerful skill which can allow people to uplift themselves from all sorts of backgrounds and I speak from personal experience myself. The harder we make it for beginners, the more exclusive we make our profession. I think the success of the next ten years will be defined by how easy we make .NET to a whole new population of developers and I think there's some real work for us to do in order to improve the current situation!

## What about others?
 
 Before you think that the current complexity is normal in tech and nothing endemic to .NET itself then I'm afraid to prove you wrong. [Take a look at PHP](https://www.php.net). PHP has evolved a lot over the years and while it has introduced more complicated object oriented programming paradigms and improved on its performance, it hasn't lost the sense of its original simplicity at all. PHP is a great programming language to start. Learning content exist like sand on a beach. The online community, forums, Reddits and conferences are second to none. Most importantly, a student could pass on a PHP book to their younger sibling after years of studying it themselves and the next person would still gain a lot of value from it. The language might have moved on, but many things which were written down, particularly on a beginner level, are still 100% accurate today. This is a huge benefit which gets easily forgotten by more advanced developers like myself. Furthermore the distinction between PHP (as in the language) and the interpreter itself is negligent. A new beginner doesn't have to wrap their head around such topics at all. They just know that they installed PHP and it works.
 
Another great example (and there are many great examples) is [Go](https://golang.org). Go is not a new language at all. It's [been around for more than 10 years](https://opensource.googleblog.com/2009/11/hey-ho-lets-go.html) and despite huge improvements to the language, the compiler and the standard libraries it has remained faithful to its original simple design. Similar to PHP a new developer doesn't have to think about complicated nuances between "target frameworks", weird gotchas if they write Go in this IDE or that IDE and certainly they don't have to look up a complicated version matrix which explains the intricate relations between the SDK, the runtime and newly available language features. There is just one version which maps to a well defined list of Go features, bug fixes and improvements. It's documentation is simple and easily comprehensible. It is very beginner friendly.

## Improving .NET

Why is .NET so complicated? Well, the answer is not that easy itself :). It's mostly history, legacy baggage, some bad decisions and what I believe is an eager urgency to push boundaries for boundaries' sake. I will try to name a few selected issues from my personal observation, describe the problems perceived and what I think could be good improvements for the future.
 
### Languages &amp; Versions

If a person sets out to learn C# like the author from the question above, what do they learn? Is it C# or .NET? The answer is both. C# doesn't exist without .NET and you cannot program in .NET without one of the languages, which is C#, F# or VB.NET. This is not necessarily a problem yet, but it's where all the troubles begins.

First the languages evolve independently from .NET. Whilst this has many advantages it also makes things unnecessarily confusing. C# is heading towards [version 9](https://devblogs.microsoft.com/dotnet/welcome-to-c-9-0/), F# is approaching [version 5](https://devblogs.microsoft.com/dotnet/announcing-f-5-preview-1/) and VB.NET is already on [version 16](https://docs.microsoft.com/en-us/dotnet/visual-basic/getting-started/whats-new#visual-basic-160). Meanwhile .NET Framework is on [version 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) and .NET Core on [version 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1). Don't even get me started on [Mono](), [Xamarin]() or [ASP.NET]().

I appreciate that I'm comparing oranges with apples, but how is a new developer supposed to know that? Especially when this is what they see on the official download page:



### The Name

No matter what you think, how used you got to it or how much you subjectively like it, .NET is not a great name.

### The Frameworks

### Pushing boundaries