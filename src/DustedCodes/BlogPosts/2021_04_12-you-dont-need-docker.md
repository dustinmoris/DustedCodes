<!--
    Tags: docker containers microservices
-->

# You don’t need Docker

<div class="tip"><strong>Note:</strong> In this blog post I use the terms Docker and containers interchangeably. I know that Docker is only one of many container technologies and not always the best suited one (e.g. Kubernetes), but for the purpose of this blog post I don't differentiate between them.</div>

*<span style="font-size: 1.5em;">&bdquo;</span>You don’t need Docker. I started my business on a small server under my desk. It took me more than 10 years before I reached the scale where I needed something like Docker. You’ll be fine for a very long time before you’ll have to worry about it!<span style="font-size: 1.5em;">&ldquo;</span>*

![You don't need Docker](https://cdn.dusted.codes/images/blog-posts/2021-03-26/you-dont-need-docker.png)

Who has never heard someone say something like this?

[I hear it all the time](https://dev.to/inductor/do-you-really-need-docker-or-kubernetes-in-your-system-11nk). At least once a week I see someone [tweet](https://twitter.com/FransBouma/status/1216736461166383105?s=20) or [blog](https://medium.com/@chintanaw/no-you-dont-need-cloud-docker-no-kubernetes-hell-no-ae2e422d0942) about how [Docker is not really needed](https://launchyourapp.meezeeworkouts.com/2021/03/why-we-dont-use-docker-we-dont-need-it.html?m=1) and how they managed to get away without it. To be honest they are not wrong. Nobody really *needs* Docker, but then again “need” is a very strong word. The real question is “Do you want Docker”?

## A different world

It is true, many successful websites and web applications started without Docker. Many also started without the “Cloud”. Some probably even started without NoSQL databases (there was a time when MySQL and Oracle were king), no Redis, no SendGrid or MailChimp, no Stripe or Braintree, no Angular or React, definitely no Vue, no serverless functions, no queues and distributed systems, no CSS frameworks, no CI/CD pipelines and probably not even virtual machines! Heck, some websites probably didn't even use jQuery or JavaScript to begin with!

However, would anyone really want to start a new internet business without these tools today? I don't think so, at least not if they aim for success!

It's easy to forget but when companies like MySpace or Facebook started the world was a very different place. We had no social media (that’s kind of obvious from this example), no iPads and iPhones, no smartwatches, no home speakers, we had no fibre optic cables leading to our homes and we didn’t have mobile broadband either. The aspiration of internet domination wasn’t a thing yet. Even when Facebook already reached huge success they were still only one of many other social networks on the web. We had many individual (often national) versions of something similar to Facebook for a very long time before the world wide web became a little bit less wide again. Internet usage was very different too. [Psy didn't even break YouTube yet](https://www.telegraph.co.uk/technology/news/11272577/How-South-Korean-pop-star-Psy-broke-YouTube.html).

The world, our relationship with the internet and people's expectations were completely different than what they are today.

### Instant scale was not a threat

Do you remember [online bulletin boards (BBS)](https://en.wikipedia.org/wiki/Bulletin_board_system)? Before Reddit we had many independent self-hosted internet forums. There was a time where there was no WordPress or Medium. The internet started off as a truly decentralised web with many independent websites, blogs, communities and even multiple search engines before Google took over. Internet users didn't all congregate in the same places then.

**Websites had time to grow**. The danger for an indy blog seeing traffic spikes from 3 users per week to tens of thousands of users in a single day was just not a credible threat. Today it doesn't matter how fringe or unknown a website is, any page could suddenly end up on Hacker News and learn what it means to get the infamous [Hacker News hug of death](https://www.indiehackers.com/post/the-hacker-news-hug-50-000-unique-visitors-in-18-hours-65977e0636). Maybe a few years ago it was possible to delay the thought of scale to a later stage, but today this is not possible if one wants to reap the benefits of sudden success.

### A more patient crowd

The first time I used an instant messenger was at the time of [ICQ](https://en.wikipedia.org/wiki/ICQ). The first time I downloaded music was from [Napster](https://en.wikipedia.org/wiki/Napster). Then I switched to [KaZaA](https://en.wikipedia.org/wiki/Kazaa), then [LimeWire](https://en.wikipedia.org/wiki/LimeWire), then [eDonkey](https://en.wikipedia.org/wiki/EDonkey2000) and later to [Giganews](https://en.wikipedia.org/wiki/Giganews) which was a popular [Usenet](https://en.wikipedia.org/wiki/Usenet) at that time. What they all had in common was the true nature of a decentralised web. They were all built on so called [peer-to-peer networks](https://en.wikipedia.org/wiki/Peer-to-peer). When my friends went offline then I couldn't send them a message any more. Messages just wouldn't arrive. There would be no connection and it would time out. When enough &quot;peers&quot; turned their computers off then my downloads would pause. It was totally normal to download content over a period of multiple days if not weeks. There was absolutely no expectation for things to happen in an instant moment.

Nowadays nobody would accept a download to take longer than a few seconds let alone a couple weeks. Patience has come down and expectations went up high. What was once a luxury experience is now the baseline bar. If a video doesn't stream in at least 1080p then it might as well never happen. If the quality is not right then people turn away. Startups, indy hackers, open source projects and even hobbyists cannot afford to offer a degraded service if they want to get traction in current times.

### The internet was a toy

When I got my first computer the internet was nothing more than a toy. My relationships with my family and friends did not rely on the availability of WhatsApp. Jobs were not impacted if StackOverflow, GitHub or Slack were a bit slow. Today I can't even book a doctor's appointment without going online.

As we have become increasingly more dependent on the web, service providers have gained a higher responsibility in keeping their services alive. Today it's rather questionable if a business can still offer a meaningful SLA with a server under someone's desk.

### Tolerance towards failure

Tolerance towards failure is another benefit which web applications had in the past. Today not so much anymore. Nobody expects Uber to be down. Binge watching is only possible because Netflix never goes offline. Music never stops playing when you're on Spotify. And if it did then people would lash out.

We've become so used to the high quality and availability of services that no glitches in the system go unnoticed anymore. No failure, no scalability issue and no data loss get past users without people grinding their teeth or writing angry tweets. Every incident has a lasting effect and can limit one's future potential of growth. For example, I have never hosted anything on GitLab myself but I sure know that they are infamous for [being](https://github.com/sameersbn/docker-gitlab/issues/13) [so](https://serverfault.com/questions/1049621/gitlab-push-very-slow-gitlab-ce) [awfully](https://gitlabfan.com/why-gitlab-is-slow-and-what-you-can-do-about-it-bca9d61405bd) [slow](https://stackoverflow.com/questions/43226191/frequently-our-gitlab-is-getting-slow) or losing [production data](https://gitlab.developers.cam.ac.uk/uis/devops/devhub/docs/-/wikis/reports/29th-March-2019-Incident-Report) without full recourse.

### The network effect

Although everyone likes some quick gains, nobody likes to see their business outgrow their own ability of keeping up with demand. **Scale plays a huge part in that realm.** We are so interconnected that unless someone launches a product in a private invite-only group they won't be able to predict (or control) how fast they will grow. The internet has its own mind and nobody knows who will be famous tomorrow and what will go viral the day after. An innocent tweet, a [short post on Reddit](https://remoteclan.com/s/27ihu5/my_product_scale_went_viral_150_000_views) or a routine launch on Product Hunt can [shift a new startup from 0 to 10,000 users](https://medium.com/@vinayh/0-10-000-users-how-openvid-launched-on-product-hunt-575ff9ecf7a1) in the span of 3 months. That level of virality is insane. Imagine having 10,000 willing customers on your doorstep and they can't sign in because someone told you that you won't have to think about scale for a good while. Don't become a victim of your own success.

## Do you <del>need</del> want Docker?

Nobody really needs Docker (or containers per se) and I'm not going to claim that containers are the perfect silver bullet to all the issues listed above, but it remains an incredibly powerful tool which can address many of today's challenges in a very time and cost effective way. Sure there is an upfront investment to be made in learning container technology and putting it into practice, but by no means is it any harder or more time intensive than learning a new CSS framework or the latest flavour of JS. If anything, skills like Docker are much broader applicable and more transferable between programming languages, tech stacks and jobs.

Containers make builds predictable, they make deployments reliable and they make horizontal scaling a breeze. Containers are a great way of providing stable and backwards compatible APIs whilst keeping code complexity low. Containers can reduce infrastructure cost by running multiple applications on the same box. They can accelerate a team's productivity by running different feature branches at the same time and launch testing environments independent of hardware. Containers make blue green deployments mainstream and help to keep downtime low.

The question is, why would you not want Docker?