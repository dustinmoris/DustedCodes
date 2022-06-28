<!--
    Tags: oss nuget npm github
-->

# Fund OSS through package managers

Open source software has become an integral part of our lives. Personally I don’t know a single app, website or other digital product which doesn’t make use of at least one open source project in their stack. We all know the value and importance of open source software and yet [many open source maintainers](https://github.com/SixLabors/ImageSharp/discussions/2151) struggle with its sustainability. A big part of that struggle is the inability to cover the cost for the time and effort put into developing and maintaining a successful OSS project over a long period of time. Of course, very few if any software developers start an OSS project with the intent to make an income from it, not least because it would be a very bad business idea, but if a project grows in users and (user) demands over a long period of time then it inevitably turns from an occasional I-do-what-I-want hobby into something more akin to a real world job.

Unsurprisingly nobody likes to put in hard work for free. Unfortunately most OSS developers never manage to make a dime from their projects.

There may be many reasons why OSS maintainers find it difficult to get compensated for their hard work, but fundamentally I believe that the issue lies within the fact that there's no easy way to pay for OSS. Of course we have a [few fringe projects](https://ko-fi.com) which [try to fill the void](https://www.buymeacoffee.com), but [none of them](https://en.tipeee.com) are integrated into the daily tools which developers rely on every day. It requires some good will and someone going out of their way in order to make a financial contribution towards OSS. Ideally it should be the other way around.

## Donations don’t work

Personally I don't think that the model of donations work that well. Initiatives like [Patreon](https://www.patreon.com), [OpenCollective](https://opencollective.com) or [GitHub Sponsors](https://github.com/sponsors) sound very noble and for very few people it might even generate enough income to compensate their work, but for the vast majority of OSS developers it just doesn’t help at all.

The problem with donations is that it promotes the wrong idea. It suggests that OSS is free by nature and only if a user feels charitable enough they might contribute a dollar or two to show some support. Fundamentally there is no set expectation to donate towards OSS and as a result most consumers won't pay anything at all.

It is important to understand that users are not to blame. They simply follow a social contract which has been understood by people since the beginning of time.

### Digital street artists

Free open source projects are the digital equivalent of real world street artists.

We’ve all been to a town square with (free) street performances before. Only a small fraction of the audience will throw in a coin at the end of the show. Nobody thinks of you badly if you don't. After all nobody asked the street performers to put on a show. In most cases street artists will even begin their performance without an audience to start with. Only if the street artist performs well enough people will start to slowly congregate around them. It is not that different to how most open source projects operate today. The social contract is the same. It is the street artists who are grateful for the (voluntary) donations from their audience and not the other way around.

### As a charity you need to beg

I often hear the argument that the entire software industry relies on OSS projects and therefore everyone should pay. I agree with the sentiment (to some extent) but then donations are the wrong model to achieve this goal.

Let’s look at another example from a different field. Five out of ten adults (in the developed world) will develop cancer at some point in their life. Yet less than one out of ten people will ever donate a single penny towards cancer research during their time. Nine out of ten people will be affected by cancer indirectly at the minimum. Despite cancer being a major issue in the developed world only very few people choose to voluntarily contribute towards the cause.

On the other hand people contribute towards a million other medical treatments and research projects by paying national insurance and income tax. The difference? It's done automatically via existing systems which we have in place.

What about Wikipedia? How many people use Wikipedia every day versus how many people choose to donate at least once a year? My point is that a user's willingness to pay *voluntarily* for a product or service is rarely related to its usefulness. Frequently people simply choose the path of least resistance. In the case of a free open source project that means no financial support at all.

Neither Cancer Research UK or Wikipedia would exist if they didn't spend a lot of time and energy every year to actively beg people for their support. This is not a model which we can reasonably expect from an average OSS developer.

Donations simply don't work.

### Toxic expectations

Sometimes donations can be toxic too. As mentioned before, a donation is seen as a charitable act by the user towards the OSS maintainer. Open source developers often feel very grateful for their users' support and don't want to let them down. That feeling of gratitude and self imposed expectations can put maintainers under immense pressure too. It's hard to prioritise your own personal needs if someone who voluntarily pays for your coffee asks for your time. Even if one's supporters are the most understanding, kind and friendly people, it can be still extremely mentally taxing if one thinks that they're letting their supporters down. It takes a mentally strong individual to not fall into this trap. I wouldn't be surprised if it is often the mental strain rather than the physical work which drives OSS maintainers into the burnout stage.

Of course the situation isn't helped by the fact that some donors actually believe that their financial support entitles them to special treatment in return. The highest paying customer often demands the best table in the house. If someone tips their taxi driver well they probably expect help with their luggage also. I am not saying that this is right or wrong, but simply stating the fact that it's common enough for many people to believe that their donations buy them a favour too.

Whether it is intentional or not, the model of donations (or often camouflaged as "sponsorships") can put open source maintainers on the back foot and sometimes even be felt as more damaging than helpful under certain circumstances. Those concerns should not be taken lightly and I wholeheartedly believe that donations (or sponsorships) are a complete inappropriate way to sustain open source.

## Convenience drives behaviour

It turns out that the vast majority of cinema goers don't mind to pay for films at all. There was a time when people were not so sure. In the late 90s and early 2000s ripping movies off the internet was so widely spread that one might have thought the opposite was true. However, the problem was never people's willingness to pay for films. Initially it just happened to be so much easier to download a movie illegally off the internet than buying a legitimate DVD. Today the tables have turned. Finding and ripping movies off the internet takes so much more effort than buying them on the Google or Apple store. Amazon Prime, Netflix, Disney+, Google Play or Apple TV have made film watching so incredibly convenient that for 99.9% of consumers any other alternative is just not worth their time.

We are a very busy society and convenience drives how we behave.

I believe that we should take this valuable lesson to the open source ecosystem an apply it in a similar way!

## Package managers reimagined

Package managers are the central point of consuming third party OSS packages. It is the most straightforward and convenient way to publish, install or update an open source library for most programming languages. Package managers know everything about both sides of the transaction. They know who publishes a package and who consumes it. They know how many times a package gets installed in total or per party. They are already an established platform that developers could hardly live without today. They have fantastic and simple to use CLI tooling which could be easily extended to help with the sustainability of open source.

### Introduce sign-in and pricing models

Firstly, package managers such as npm or NuGet should introduce the ability for users to sign-in with an account. It should be an optional feature so that anonymous access remains the default out of the box experience. However, this could enable package authors to decide if their package may be consumed by anonymous users or authenticated user only. In the simplest form nothing would change. A package author could simply specify that their OSS library can be consumed by anyone without any limitations at all, just like it is today. However, in addition they could start introducing pricing models based on a variety of options:

- Disable anonymous access (only logged in users can install a package)
- Allow first X amount of installs per user for free (e.g. for exploration)
- If user installs package more than X-times they require to purchase a license

If for whatever reason a package is prohibited from being installed then the CLI should output the reason just like it does for a variety of non commerical reasons already today.

Examples:

```
> Please sign in to install package X
```

```
> You've exceeded Y free installs for package X. Choose one of the following license options to continue:

1. $49 for a lifetime license
2. Don't install package
```

Purchasing a license should be as straightforward as buying a subscription in the Apple Store. Those licensing fees could be yearly, monthly or a one off fee. I haven't thought of all the possibilities but over time package platforms should evolve to give their users increasing flexibility to pick the right model for themselves.

### Flexible pricing

Other ideas would be to limit the installation of a package only in certain scenarios. For example one may consume a package anonymously and without any limitations as long as they consume it from their development machine, but they may need a license when installing it from a CI build.

Perhaps a package remains entirely free but users who want to have early access to bleeding edge features will require a license for the consumption of pre-release versions. Maybe a user that is a verified student will not be restricted at all.

Another option could be to offer the same package licensed under different conditions. A free download could be available for a GPL licensed version and differently licensed version for a fee. The possibilities are endless and each package author could pick an appropriate model between them and their user base.

### Make license acquisition easy via CLI

The biggest challenge with introducing a commercial layer into the already existing complexity of package management is to not make it so cumbersome that it will become unusable. The key is that the vast majority (like 99%) of use cases should become satisfiable via the CLI. Nobody wants to have to open the browser, log into some platform and start entering card details or go through checkout screens in order to use a third party library. Package managers would have to capture a user's payment details once in advance and make the whole experience almost unnoticeable. There is already a good precedence for this model, although it might not be 100% transferable to package management it would be a good point to start with. I can already provision, update or extend expensive cloud infrastructure from the comfort of my CLI and at no point am I confronted with pesky payment or checkout forms. I believe a very similar experience could be introduced into package management in order to make OSS more sustainable!

### Downstream dependencies

Downstream dependency management could pose another challenge which needs to be carefully thought through. Again, there are many ways how this could be solved, but in its simplest form one could say that the end user must hold a valid (free or commercial) license for all downstream dependencies. Personally this feels the most straightforward and least ambiguous model and also the easiest to start with from a package management platform's point of view.

### Users can still circumvent and cheat

What if users cheat?

Yeah so what? There will always be a minority that will go out of their way to circumvent whatever restrictions have been imposed on them. However, they will have to jump through annoying hoops and take a path of constant resistance in order to achieve their goal. They will not be able to rely on the leading package managers for their language of choice. They will not be able to receive frictionless updates to their dependencies. They might have to run their own hosting or pay a fee elsewhere to create a back alley dependency channel. They will have to jump through additional hoops to stay anonymous or risk getting sued by the original package author or a big platform like npm itself.

If the legitimate path of acquiring a third party OSS package has been deeply engrained into developer's every day tools and made ridiculously easy then most programmers will simply not bother to go against the grain. It has worked for books, music and movies and there is little reason to believe that it won't work for the open source community either.

## Final thoughts

Apart from package managers other platforms can play an active role in promoting the sustainability of OSS as well.

GitHub for example could allow repository owners to disable issue creation unless a user pays a minor fee. Users who submitted a successful pull request which later gets merged could get a credit towards their account which in turn can be used as payment as well.

Maybe GitHub can create a new tab for "development requests" to track user desired work. Those could be new features, creating sample projects or expanding in documentation. Repository owners can then set a funding goal for each development request and only commence with the work when the goal was successfully met.

I don't want to claim that I have thought of all the potential pros and cons for each those ideas but I would like to bring two major points home before I will wrap up with my post:

1. There is no shortage of creative ideas which could immensely improve the sustainability of OSS.
2. It is the platforms which developers use every day that must get involved. Paying for OSS is only awkward because it has not been normalised through the platforms which we use yet.

GitHub, GitLab, npm, NuGet, etc. the ball is in your court!