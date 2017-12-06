<!--
    Published: 2017-12-06 19:00
    Author: Dustin Moris Gorski
    Title: Evolving my open source project from a one man repository to a proper organisation
    Tags: giraffe oss aspnet-core fsharp
-->
Over the last couple of months I have been pretty absent from my every day life, such as keeping up with Twitter, reading and responding to emails, writing blog posts, working on my business and maintaining the Giraffe web framework. It was not exactly what I had planned for, but my wife and I decided to take a small break and [wonder through South America](https://www.instagram.com/dustedtravels/) for a bit before coming back for Christmas again. It was a truly amazing experience, but as much as travelling was fun, it was not great for [my open source project Giraffe](https://github.com/giraffe-fsharp/Giraffe) which was just about to pick up momentum after the huge exposure via [Scott Hanselman](https://www.hanselman.com/blog/AFunctionalWebWithASPNETCoreAndFsGiraffe.aspx)'s and the [Microsoft F# team](https://blogs.msdn.microsoft.com/dotnet/2017/09/26/build-a-web-service-with-f-and-net-core-2-0/)'s blog posts (huge thanks btw, feeling super stoked and grateful for it!).

Initially I did not plan to slow down on Giraffe, but it turns out that trying to get a decent internet connection and a few hours of quality work in between of adventure seeking, 16 hour bus journeys, one flight every 3 days on average, hostels, hotels, hurricanes and multi day treks in deserts and mountains is not that easy after all :) (who thought ey?).

As a result issues and pull requests started to pile up quicker than I was able to deal with them and things got a bit stale. Luckily there were a few OSS contributors who did a fantastic job in picking up issues, replying to questions, fixing bugs and sending PRs for new feature requests when I was simply not able to do so - which meant that luckily things were moving at least at some capacity while I was away (a very special thanks to [Gerard](https://twitter.com/gerardtoconnor) who has helped with the entire project beyond imagination and has been a huge contributor to Giraffe altogether; I think it's fair to say that Giraffe wouldn't be the same without his endless efforts).

However, even though the community stepped up during my absence I was still the only person who was able to approve PRs and get urgent bug fixes merged before pushing a new release to NuGet, and that understandably caused frustrations not only for users, but also for the very people who were kind enough to help maintaining it.

As the owner of the project and someone who really believes in the future of Giraffe it became very apparent that this was not acceptable going forward and things had to change if I really wanted Giraffe to further grow and succeed in the wider .NET eco system. So when [the community asked me to add additional maintainers](https://github.com/giraffe-fsharp/Giraffe/issues/152) to the project I did not even hesitate for a second and decided that it was time to evolve the project from a one man repository to a proper OSS organisation which would allow more people having a bigger impact and control of the future of Giraffe.

## An OSS project is only as good as its community

I created [`giraffe-fsharp`](https://github.com/giraffe-fsharp) on GitHub and moved the [Giraffe repository](https://github.com/giraffe-fsharp/Giraffe) from my personal account to its new home. Furthermore I have initially added three more contributors to the organisation who now all have the required permissions to maintain Giraffe without my every day involvement. This doesn't mean that I don't want to work on Giraffe any longer or that I want to work less on it, but it means that I just wanted to **remove myself as the single point of failure**, which is very important for a number of reasons.

First there is the obvious point that if I'd disappear out of the blue for whatever reason it would have a huge impact on anyone who has trusted in the project and its longevity. If I'd be a large company where changing tech can be very expensive then I'd personally not be able to justify the usage of a project which could literally drop dead over night. It is a real concern which I understand and therefore try to address this with the transition to a proper OSS organisation. It's a first step to mitigate this very real risk and a commitment from my side to do whatever I can to keep Giraffe well and alive.

Secondly I can simply not imagine that I as a single person could possibly grow the project better or faster than a larger collective of highly talented developers who are motivated to help me. I would be stupid to refuse this offer and it's in my personal interest to help them to help me.

Thirdly and most importantly I don't want to lose the fun and joy of working on Giraffe. I strongly believe that [.NET Core](https://dot.net) is an excellent platform and a future proof technology. I also believe that functional programming is yet to have its big moment in the .NET world and with [F# being the only functional first language](http://fsharp.org/) I think the potential for Giraffe is probably bigger than I can think of today. I think it's only a matter of time before its usage will outgrow my own physical capability of maintaining it and the last thing I want to happen is to burn out, have an OSS fatigue or completely lose motivation for it. The only way to avoid this from happening is by accepting the help of others and delegating more responsibilities to other people who share the same vision and have an interest in the project's success.

Therefore it makes me proud to have met these people and being able to expand Giraffe into a more structured organisation which I believe is the right way of addressing all these issues effectively.

## More people need more structure

Now that there's more people using Giraffe and more people helping to maintain it I think the next important step is to further expand on a work flow which aims at providing at a minimum the same quality at which I would have maintained the project myself.

As such I have set up the following process to maximise quality, reduce risk and give orgaisations of all size the confidence of trusting into Giraffe:

- All development work has to happen on individual branches (enforced via GitHub)
- The `develop` branch **must be at a releasable state** at all times
- Only a finished bug fix or feature enhancement can be pushed via a pull request into `develop` (enforced via GitHub)
- Each PR against `develop` must be formally reviewed by at least one other core maintainer (enforced via GitHub)
- If enough PRs has flown into `develop` and the team wants to schedule a new release then a pull request can be made against `master`
- Only an owner (currently me) has permissions to approve a PR on `master` and hence triggering a new automated release to NuGet (enforced via GitHub)

I know this might seem like a lot of process for a fairly new project, but I think it is very important to establish a good working structure early on to keep the quality high no matter how big the project will grow in the future. This process guarantees that at least 3 separate pair of eyes (two core maintainers and one owner) will review every single line of code before it makes into an official release. At the same time this process should hopefully allow a frictionless collaboration between core maintainers up until the point of an official release without the necessity of my involvement. Things like a vacation, illness or other forms of temporary unavailability should not be an issue any longer.

## Never stop growing

All of this is only a first step of what I hope will be a very long future for Giraffe. Nothing is set in stone and if we discover better or more efficient ways of working together then things might change in the future and I will most certainly blog about it in a follow up post again.

One other thing which is perhaps worth noting is that I also plan to join the [.NET Foundation](https://www.dotnetfoundation.org/) or the [F# Foundation](http://foundation.fsharp.org/) in the near future. I'll talk more about this in a follow up blog post as well!

## Can I join giraffe-fsharp?

Short answer is yes, but you have to have contributed to the project as an outside collaborator first and shown an interest and familiarity with the project before getting an invite to become a member of the organisation. I certainly don't see an upper limit of motivated people who want to help and any form of contribution is more than welcome. If you like Giraffe and want to participate in it then feel free to go and [check out the code](https://github.com/giraffe-fsharp/Giraffe/fork), [discuss your ideas via GitHub issues](https://github.com/giraffe-fsharp/Giraffe/issues/new) and send PRs for approved feature requests which will definitely see your code (after reviews) merged into the repository.

Please also be aware that any member of [giraffe-fsharp](https://github.com/orgs/giraffe-fsharp/people) must have two factor authentication enabled on their account. I am big on security and I don't see why an OSS project should be treated any less serious than a project which would be run by a private company.

I hope this was a helpful insight into where I see the project going, what has happend lately and which steps I am taking to get Giraffe out of infancy and make it a serious contender among all other web frameworks in the wild (and not just .NET ;))!

Thanks to everyone who has helped, blogged, used or simply talked about Giraffe! There's no better feeling than knowing that people use and like something you've built and I now it's time that Giraffe becomes not only mine but a community driven project which we can grow together.

P.S.: If you ever have a chance to pack your stuff and travel the world then I'd highly recommend you to do so! It's one of the most amazing things one can do in life and probably for many people a one in a lifetime experience. Go and explore different places, meet people, learn about new cultures and experience life from a different perspective. It's eye opening and very educational on so many levels! [Follow me on Instagram](https://www.instagram.com/dustedtravels/) for inspiration ;)