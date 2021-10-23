<!--
    Tags: dotnet aspnet-core oss
-->

# Can we trust Microsoft with Open Source?

Oh boy, what a week of .NET drama again. Not bored yet? Read on, but for this one you’ll need some stamina, because this one is different.

Before I start with my actual blog post let me give you a short disclaimer. This is **not** an issue of some outspoken or sceptical community members making a fuss out of nothing. I know it is easy to see it this way with incomplete information, but trust me on this one, you couldn’t be more wrong if you believed that's the case this time. This is a **bigger issue**, an issue internally playing out at Microsoft right now as we speak. Many people from Microsoft itself, who have been working really hard to build trust with the OSS community and market .NET as a real viable OSS platform are struggling right now. These are your .NET heroes who you probably dearly cherish and these folks are currently unable to publicly speak their minds. They struggle and they want YOU to speak for them. In fact, they want you to speak out right now and make your voice heard!

You don’t believe me? Don’t take my word for it, but please read in-between the lines:

[![Tweet 1](https://cdn.dusted.codes/images/blog-posts/2021-10-23/tweet-1.png)](https://twitter.com/shanselman/status/1451376901579182082)
￼
[![Tweet 2](https://cdn.dusted.codes/images/blog-posts/2021-10-23/tweet-2.png)](https://twitter.com/shanselman/status/1451737603942739974)

[![Tweet 3](https://cdn.dusted.codes/images/blog-posts/2021-10-23/tweet-3.png)](https://twitter.com/davidfowl/status/1451759897708666881)
￼
[![Tweet 4](https://cdn.dusted.codes/images/blog-posts/2021-10-23/tweet-4.png)](https://twitter.com/DamianEdwards/status/1451015493872087045)

[![Tweet 5](https://cdn.dusted.codes/images/blog-posts/2021-10-23/tweet-5.png)](https://twitter.com/condrong/status/1451754645563457537)

## The hot "Hot Reload" issue￼

Here is a short summary of what has happened…

For the majority of the last year the .NET Team was hugely focused on improving the “inner dev loop” in .NET. You might have heard many prominent .NET figures use exactly those words on countless public forums, live streams, conferences and public talks. It was one of the [.NET Team’s highest priority items for .NET 6](https://themesof.net):

![Themes of .NET](https://cdn.dusted.codes/images/blog-posts/2021-10-23/themes-of-dotnet.png)

As such the team worked hard on making lots of great improvements to .NET, the SDK and the tooling around it. One of those big features was “Hot Reload” in the `dotnet watch` tool. I watched Scott Hunter give a talk and early demos of this feature many months ago when .NET 6 was still in baby shoes.

Hot reload wasn’t a fringe feature that might or might not have made it into the release of .NET 6. It was literally one of the flagship features that was in the making for a long time, **and it was complete**. After all, you don’t accumulate [1000s of lines of perfectly fine code](https://github.com/dotnet/sdk/pull/22217) by accident.

[![Hot Reload Tweet](https://cdn.dusted.codes/images/blog-posts/2021-10-23/hot-reload-tweet.png)](https://twitter.com/davidfowl/status/1392367324586418176?s=20)
￼
So what happened to it? Well, [someone at Microsoft who wields great power](https://www.linkedin.com/in/julia-liuson-6703441/) has made the decision that features such as hot reload cannot be given away for free as part of the open source .NET SDK anymore. These features must be reserved to proprietary commercial products such as Visual Studio. In fact, there is a bigger internal strategy being formed at Microsoft to make Visual Studio the main IDE for .NET again, because some people at Microsoft are annoyed that Visual Studio Code and other third party tools have been undermining Visual Studio for way too long now. As a result the [hot reload feature was ripped out of the SDK](https://github.com/dotnet/sdk/pull/22217) in a last minute effort, breaking all promises and conventions of Microsoft’s release candidate policies and [announcing that hot reload will be a Visual Studio only feature](https://devblogs.microsoft.com/dotnet/update-on-net-hot-reload-progress-and-visual-studio-2022-highlights/) going forward.

I know what you think. You think I’m crazy and that I'm trying to spread some conspiracy theories here, but trust me on this one that I am not wrong. This is exactly what is happening behind the scenes at Microsoft right now and the .NET team wants you to know it even though they can't say it. It’s not by accident that we’ve seen a fairly “coordinated” effort by Microsoft employees [leaking a lot of internal infighting with the Verge](https://www.theverge.com/2021/10/22/22740701/microsoft-dotnet-hot-reload-removal-decision-open-source) and [other media outlets](https://www.theregister.com/2021/10/22/microsoft_net_hot_reload_visual_studio/) yesterday.

The issue goes even deeper. Not only will hot reload not make it into .NET 6 or any future version of .NET, the entire `dotnet watch` tool will be discontinued in an effort to push Visual Studio as a viable product. This was not publicly announced yet and don’t count on anyone from Microsoft to publicly admit that, but if you [pay close attention](https://github.com/dotnet/sdk/pull/22217/files#r733047263) or if you happen to talk to a Microsoft developer privately over a coffee then you might come to that conclusion exactly.

Why the change of direction now? The truth is this hasn’t just happened now. In fact Microsoft has already started to make such subtle moves in the past. For example the [Python extension for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-python.vscode-pylance) was never open sourced and released as a proprietary product to begin with. The once open source cross platform IDE [MonoDevelop](https://www.monodevelop.com) was hard forked by Microsoft and rebranded as "Visual Studio for Mac". All improvements and feature work since then have been proprietary and closed source. Yes, that is right, Visual Studio for Mac is a closed source version of the formerly open source MonoDevelop IDE after Microsoft acquired Xamarin. There was no need to turn an already long standing open source project into a proprietary commercial product. Especially since MonoDevelop was available on Linux and Mac and Visual Studio for Mac, as the name suggests, isn't. These subtle moves and many others which went mostly unnoticed have emboldened certain people at Microsoft to further pursue a less open strategy again.

I'm glad I am not the only one who noticed...

[![Microsoft changing licenses to prevent debugging outside Visual Studio](https://cdn.dusted.codes/images/blog-posts/2021-10-23/debugging-tweet.png)](https://twitter.com/hhariri/status/1451841350123597829?s=200)

Rumours say more is to follow.

Visual Studio Code has been one of the most successful products which Microsoft has ever released. It has become staple to every software developer around the globe. It was a success to everyone except Visual Studio. Have you ever noticed that .NET - a Microsoft owned product - is the worst supported programming platform on Visual Studio Code - which also happens to be another Microsoft owned product? [I’ve been complaining about this for years](https://dusted.codes/dotnet-for-beginners), because to me Visual Studio Code represents the future of .NET and a new gateway into growing the .NET platform beyond traditional die-hard Windows fans. However, Microsoft has been purposefully underfunding [OmniSharp](https://github.com/OmniSharp) for years in an effort to push developers towards Visual Studio again.

Sure, you might think this is not a big deal and completely up to Microsoft to decide where they want to allocate their resources, and normally I would agree, but what if I tell you that internally at Microsoft employees are being actively punished by management if they contribute improvements or bug fixes to OmniSharp (the OSS .NET plugin for VS Code) which is seen as further undermining Visual Studio?

Ouch, that is a big accusation and hard to believe, so why don't you reach out to one of your favourite Microsoft employees who work on .NET or Roslyn or another OSS product under .NET and ask them for a comment? They most likely won’t admit it, because they are not allowed to, but more importantly **they also won’t deny it**.

As Damian Edwards said before, good that nobody let the cat out of the bag ;)

![Microsoft org chart](https://cdn.dusted.codes/images/blog-posts/2021-10-23/microsoft-org-chart.png)

The big issue here is that Microsoft has an internal struggle going on at the moment. On one hand they want to be seen as a new version of Microsoft who loves Open Source, but on the other hand they want to actively block advances in OSS projects like the .NET SDK which could undermine their own commercial offerings. Microsoft<sup>*</sup> doesn’t want features like hot reload to make it into the SDK. They won’t develop cool features like these any more and most frighteningly, they won’t accept pull requests or community contributions which could add these features back into the SDK - and that is a bleak outlook for the open source community in .NET.

*) Microsoft outside the .NET team

So here is my simple question…

## Can we trust Microsoft with OSS?

I am not sure. It takes years to build trust and only a few moments to lose it all. Microsoft is a huge organisation and we as outsiders often get to see only a handful of selected people being tasked to spread a certain message via their huge followings in order to create a public image in favour of Microsoft, but what if those people leave?

Do you trust Microsoft with Open Source or do you actually trust people like Jon Galloway, Scott Hanselman, Scott Hunter, Guido van Rossum, David Fowler, Damian Edwards, Miguel de Icaza and a handful of other OSS champions who have been pushing the OSS message internally from the bottom up? What if these people leave .NET? Will Microsoft continue to play nicely with the community?

Would it worry you if Scott Hunter was moved away from .NET and someone new from a different division at Microsoft would have taken over? Remember your answer when you find out in which part of Microsoft Scott Hunter works today.

## What can we do?

This is a call to action to all .NET community members out there. Anyone with a tiny bit of clout make your voice heard. [Comment and upvote the issues on GitHub](https://github.com/dotnet/sdk/issues/22247). [Tweet your dissatisfaction](https://twitter.com/haacked/status/1451580844578000898?s=20) and make sure to tag high profile folks at Microsoft so they see what you think! Send Microsoft a clear message that this undermines the trust which we’ve lent them over the last few years and that the betrayal of this trust can have long term consequences. This is not a threat by any means but really just the reality of the matter. Many developers in technical leadership roles and with hiring powers in their respective organisations have only stayed on .NET due to the big changes which came with .NET Core. People love .NET for what it is today, a more open, cross platform and community led platform. If Microsoft starts to change their hearts again then people will as well.

Please help to send the right message and make your voice heard!