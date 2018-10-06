<!--
    Tags: aspnet-core aspnet dotnet
-->

# Understanding ASP.NET Core 1.0 (ASP.NET 5) and why it will replace Classic ASP.NET

ASP.NET has quite some years on its shoulders now. Fourteen years to be precise. I only started working with ASP.NET in 2008, but even that is already 8 years ago. Since then the framework went through a steady evolutionary change and finally led us to its most recent descendant - [ASP.NET Core 1.0](https://get.asp.net/).

ASP.NET Core 1.0 is not a continuation of ASP.NET 4.6. It is a whole new framework, a side-by-side project which happily lives alongside everything else we know. It is an actual re-write of the current ASP.NET 4.6 framework, but much smaller and a lot more modular.

Only recently [Scott Hanselman](https://twitter.com/shanselman) announced the [final name will be ASP.NET Core 1.0](http://www.hanselman.com/blog/ASPNET5IsDeadIntroducingASPNETCore10AndNETCore10.aspx) after we've got to know it under ASP.NET 5, ASP.NET vNext and Project K.

Some people claim that many things remain the same, but this is not entirely true. Those people mostly refer to MVC 6 which is a whole separate framework which can be plugged into ASP.NET, but doesn't have to. While MVC 6 remains very familiar, ASP.NET Core 1.0 is a big fundamental change to the ASP.NET landscape.

If you are a follower of the [live community standups](https://live.asp.net/) then you might have heard [Damian Edwards](https://twitter.com/DamianEdwards) saying how the team gets a lot of pressure (from above) to get an RTM out of the door. I am not surprised and can understand why ASP.NET Core 1.0 is strategically so important to Microsoft. It is probably a lot more vital to the future of .NET than we might think of it today.

## ASP.NET Core 1.0 - What has changed?

A better question would be what has not changed. ASP.NET Core 1.0 is a complete re-write. There is no `System.Web` anymore and everything which came with it.

ASP.NET Core 1.0 is open source. It is also cross platform. Microsoft invests a lot of money and effort into making it truly cross platform portable. This means there is a new [CoreCLR](https://github.com/dotnet/coreclr) which is an alternative to [Mono](http://www.mono-project.com/) now. You can develop, build and run an ASP.NET Core 1.0 application either on Mono or the CoreCLR on a Mac, Linux or Windows machine. This also means that Windows technologies such as PowerShell are abandoned from ASP.NET Core 1.0. Instead Microsoft heavily integrates Node.js which can be utilized to run pre- and post build events with [Grunt](http://gruntjs.com/) or [Gulp](http://gulpjs.com/).

It is also the reason why things like the `.csproj` file got replaced by the `project.json` file and why all the new [framework libraries ship as NuGet packages](https://www.nuget.org/profiles/dotnetframework?showAllPackages=True). This was the only way to make development on a Mac a first class citizen.

But Microsoft went even further. Part of a great development experience is the editor of choice. Visual Studio was privileged to Windows users only. With [Visual Studio Code](https://code.visualstudio.com/) Microsoft created a decent IDE for everyone now. Initially it was proprietary software but quickly became open source as well.

There are many more changes being made, but the common theme remains the same. Microsoft is dead serious about going open source and cross platform. Personally I think this is great. All of this is an amazing change and crucial to the long term success of ASP.NET.

## ASP.NET Core 1.0 - Why did everything change?

One might wonder why this new direction towards the Mac and Linux community? Why does Microsoft invest so much money in attracting non-Windows developers? Visual Studio Code doesn't cost them anything, it is unlikely that they will use MS SQL server in their projects and there is a high chance that these web applications will end up somewhere on a Linux box in Amazon Web Services or the Google Cloud Platform. After all these are the technologies which non-Windows users are more familiar with.

My guess is that all of this doesn't matter for now. The truth is that an ASP.NET developer who cannot be monetized is still better than a non ASP.NET developer. This is particularly very true if you think that the .NET community is shrinking (relatively). This is just my own speculation, but I think Microsoft fears losing .NET developers, which means they are subsequently losing people who are more willing to pay for other Microsoft products such as MS SQL Server or Microsoft Azure.

If you are a .NET developer you might think this sounds crazy, but think of it from a different angle. Windows Desktop application development is slowly dying. There is no denial to that. Left is the mobile market and the web. Windows phones and tablets are still [a drop on a hot stone](http://www.winbeta.org/news/windows-phone-market-share-drops-1-7-percent) in comparison to the market shares of iOS and Android. This leaves the web as a last resort. Now the web is an interesting one. After Silverlight's death ASP.NET is the only Microsoft product which competes with other web technologies such as Node, Ruby, Python, Java and more. This is a though battle for ASP.NET, because up until now you had to be a Windows user to be able to develop web applications with ASP.NET.

### Lack of portability

In the last few years this problem has become even more prominent with many [new languages gaining more popularity](http://www.infoworld.com/article/2840235/application-development/9-cutting-edge-programming-languages-worth-learning-next.html) and putting ASP.NET into the shadows.

The biggest problem is that the .NET framework and ASP.NET are not cross platform compatible. As a web developer you are writing applications which can be understood by any browser, any OS and any device which is connected to the web. There are no limitations, but with ASP.NET you can only develop from a Windows machine. That doesn't make much sense when you think about it.

This limitation has an impact on the adoption of ASP.NET on several levels. Recruitment is a good example. There is a massive [shortage of good software developers](http://techcrunch.com/2015/06/09/software-is-eating-the-job-market/) at the moment. Ask Ayende [how hard it is to recruit](https://ayende.com/blog/172899/recruiting-good-people-is-hard) a new talent. Imagine how much harder it is if you limit your talent pool to Windows users only? Not only do you waste more time and resources on the recruitment process itself, but also have to pay higher salaries for developers where the demand is higher than the supply.

It can be difficult for companies which are heavily committed to the .NET stack to change directions now, but what about startups? Many of today's biggest internet businesses were born out of small startups. They use free open source technologies such as PHP, Ruby, Python, Java or Node.js. This has a double negative effect for Microsoft. Not only did they lose the opportunity to sell ASP.NET, but they also send out the message that if you want to build a successful business you pick an open stack over proprietary software.

ASP.NET is probably one of the feature richest and fastest technologies you can find, but why would a startup care about this in the beginning? If they do well they can deal with this stuff later and if it doesn't go well then its good they didn't have to pay for a Microsoft license, right?

### Chasing behind innovation

Another major implication of not being cross platform compatible is that current ASP.NET 4.6 developers are missing out on big innovations which are not immediately available on the Windows platform. Over the last years Microsoft was chasing after many innovations by providing its own version to the .NET community, but not always with success (Silverlight, AppFabric Cache, DocumentDb, Windows Phone, etc.). This is not a sustainable model.

As a result many ASP.NET developers live in silos today. We are at a point where Microsoft cannot keep up with the vast amount of technology anymore and ASP.NET developers miss out on big innovations such as containers and Docker and don't even realize it, because they know very little to nothing about it. This is a dangerous place to be.

Cross platform compatibility is more than just a fad. It is the key to innovation today and the only way to stay on top of the game!

So how does Classic ASP.NET fit into this new world? Not much to be honest. ASP.NET 4.6 has a really though time to keep up with this fast moving environment.

Except we have ASP.NET Core 1.0 now...

## ASP.NET Core 1.0 - Reviving ASP.NET

This is where ASP.NET Core 1.0 comes into the limelight. It is built on the same core principles which helped other languages to popularity:

-   Free and open source
-   Cross platform compatible
-   Ease of access
-   Thin, fast, modular and extensible

On the plus side ASP.NET Core 1.0 can be developed with some of the greatest languages available right now, thinking of C# and F# in particular! This will stick out ASP.NET Core from other competitive frameworks.

 What will happen to ASP.NET 4.6? I don't know, but I would argue that ASP.NET 4.6 is a dead horse in the long run. There is very little value in betting any more money on it. Microsoft wouldn't say this yet, but it is pretty obvious. ASP.NET Core 1.0 is the new successor and the only viable solution to address the aforementioned problems.

ASP.NET 4.6 will be soon remembered as Classic ASP.NET. It will not entirely disappear, just like Classic ASP has never fully disappeared, but new development will likely happen in ASP.NET Core going forward. I find it extremely exciting and the benefits of ASP.NET Core are too compelling to not switch over as soon as possible.

The only thing we need to hope for is that Microsoft will not become impatient now and mess up the release with an immature product which will cause more churn than attraction. Microsoft, please take the time to bake something to be proud of!