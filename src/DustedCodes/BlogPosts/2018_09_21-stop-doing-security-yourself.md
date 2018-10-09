<!--
	Tags: security cryptography
-->

# Stop doing security (yourself)

Security has a very special place in software engineering. It is very different than other disciplines, because it is a problem space which cannot be approached with the same mindset as other problem spaces in our trade.

What makes security stand out so much is that it doesn't follow the same principles as the rest of software development. There is no different set of opinions. There is only one right opinion and the rest. There is not many ways how to achieve a secure something. There's only one very narrow, very particular and very unforgiving way of how to achieve a secure system (based on latest knowledge) and if you don't follow these steps very precisely then you'll end up with something which has more holes than a swiss cheese.

The other difference (between security and the rest of software engineering) is that you cannot delegate that competency to a single person. Your organisation might have one software architect, maybe one principal developer, maybe one Java or one database specialist, but if you have only one security expert then you have a problem. Security is such a complex topic, that even the most knowledgeable person on the planet will not know everything that is important.

Unfortunately, by nature, a system can never be assumed (or claimed) to be 100% secure, because there is no physics on this planet which could back this up. Therefore, the best and really only way which an organisation can use to its defence is to get their security model in front of as many eyes as possible. This is not an opinion, but a fact.

The most secure encryption is not the one which has been developed by the most knowledgeable person, but the one which has been reviewed, hacked and revised by the most people. There is a reason why all sophisticated cryptography algorithms are open to the public. Cryptographers deliberately want their work to be exposed to the largest possible audience and have it validated by them, because even they don't know how secure it is until it's been tested. This also explains why we have organisations specialising in penetration testing or why big organisations run public bug bounty programs.

With that in mind, my universal recommendation to any organisation which is not deeply involved in the InfoSec community is always to not do security by yourself. This can really not be stressed enough, but if you are not an industry leading expert in security, then don't even think about implementing your own password hasing algorithm, don't secure your API with a custom built authentication scheme and please don't build your own identity provider.

There is three reasons why:

1. You'll get it wrong
2. You don't have to do it, because others have already done it for you
3. You will get it wrong

If this little pep talk hasn't been convincing enough yet then there is another crucial reason why you shouldn't do security yourself: Security is out of your control and you'll probably live better by not having to deal with it.

Imagine your development team has made the choice to use Amazon's DynamoDb as their main data persistence layer. Two years down the line Amazon releases a new, improved and more state-of-the-art NoSQL database, but the development team doesn't find out about it until a few months later when one of the team members hears about it at a conference. After the conference the team sits together and decides to migrate to the new NoSQL database, but they won't do it for another six months, because they simply don't have the time and resources right now. Half of the team is on summer holiday, one person is sick, the development manager is on her honeymoon (so they lack formal approval anyway) and the person who originally came up with the current database architecture has left the company last year. It will take some time to come up with a good migration strategy and roll out in an efficient way. Luckily that is not an issue, because the current NoSQL database still works perfectly well and there's no immediate pressure to make a hasty change. The team is not bothered, nobody is stressing out and everyone enjoys their holidays before the team tackles the project later in the year.

Now imagine the same scenario but replace "database" with "identity provider" and replace "state-of-the-art" with "not full of security holes" and the whole story looks very different.

If someone drops a [zero day](https://en.wikipedia.org/wiki/Zero-day_(computing)) vulnerability which affects your system then time is against you before you have even realised it. At this point you are already on the losing end and the problem which you're trying to tackle is not prevention, but damage control. The longer it will take you to fix, update and roll out a new version of your affected software the more likely you are going to be hit hard by this situation.

It will add very little consolation knowing that this whole disaster hasn't even been your fault. You might just have used a cryptographic implementation which was considered secure yesterday, but today you woke up to the news that some hackers have published a white paper on how to crack it in less than an hour. This was not a targeted attack against you, your business or your customers. This was simply a new discovery which has been dumped on the security community without much thought and now you and half of the world is affected by it. Before you think this type of stuff doesn't happen, let me quickly remind you of incidents like [Heartbleed](http://heartbleed.com/), [Cloudbleed](https://en.wikipedia.org/wiki/Cloudbleed), [Meltdown](https://meltdownattack.com/), [Spectre](https://spectreattack.com/), [WannaCry](https://en.wikipedia.org/wiki/WannaCry_ransomware_attack), [and so on](https://krebsonsecurity.com/tag/zero-day/).

In order to survive such a scenario you'll want to have certain things in place:

- You'd hope to hear about the news before it's too late. This usually requires someone being extremely active in the InfoSec community, following multiple industry leading experts on Twitter, reading InfoSec blogs and being subscribed to InfoSec related RSS feeds and mailing lists.
- You have a security response team which has the skillset, communication channels and authority within your organisation to deal with this matter as fast as possible
- You have an extremely fast development life cycle. Your security response team can get the latest version of a project, make the necessary changes, get sufficient test coverage and deployment to your production systems turned around in as little time as possible
- Your security response team is ideally available around the clock. You don't know when the news might come to light and given the huge time differences between different places it can be mission critical to have security employees working around the clock, or at least be prepared for an uncomfortable situation in the middle of the night. Your security team might wake up one or two hours away from the office and needs to have sufficient access to deal with such a situation away from their office desk.

Unless your business meets all of these requirements it might be flatout irresponsible to even think of writing your own custom security software if you don't have the means to deal with issues that come with it.

Now my main point is not to scare you of writing your own software, but to create some awareness that in the context of security you are probably better off by deferring these responsibilities to a third party which is a specialist in this field. Someone who lives and breathes security every day is much better equipped to deal with all the unknowns which we're confronted with every day.