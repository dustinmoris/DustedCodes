<!--
	Tags: documentation oss giraffe
-->

# Open Source Documentation

Since January 2017, which soon will be two years ago, I've been maintaining an open source project called [Giraffe](https://github.com/giraffe-fsharp/giraffe). Giraffe is a functional web framework for F# which allows .NET developers to build rich web applications on top of Microsoft's [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) web framework in a functional first approach. Given that [Giraffe](https://github.com/giraffe-fsharp/Giraffe) is targeted at .NET web developers, who practise F# and who also like to use ASP.NET Core as their underlying web stack (as opposed to maybe [WebSharper](https://websharper.com/), [Suave](https://suave.io/), [Freya](https://freya.io/) or others) I would consider Giraffe a fairly niche product.

However as niche as it may be, it still attracted a reasonable amount of developers who use it in a personal or professional capacity every day and as such documentation has become an integral part of maintaining the project from the get go.

As someone who has never maintained an open source project before I didn't really have much experience with this topic and therefore went with a very straightforward, simple and sort of lazy solution in the beginning. I put all initial documentation into the `README.md` file inside my Git repository.

Almost two years-, nearly 50 releases-, 47 total contributors-, more than 800 GitHub stars and more than 100 merged pull requests later the project's simple documentation approach hasn't changed much, and to be honest I have very little motivation to do something about it.

The main reason why I'm not particularly excited about migrating [Giraffe](https://github.com/giraffe-fsharp/Giraffe)'s documentation to a different place is because I actually believe that the current way of how documentation is handled by Giraffe is  a perfectly well working solution for its users, its contributors and myself.

In Giraffe the entire documentation - a full reference of what the web framework can or cannot do and how it integrates and operates within the ASP.NET Core pipeline - is placed in a single [`DOCUMENTATION.md`]() file inside the [project's Git repository](), right next to the [`README.md`]() file.

The documentation is not short either. There's quite a lot of content available and somehow this hasn't become a problem yet. The fact that all of the documentation is placed  in a single large file has proven to be extremely advantagous if anything else.

Someties people ask me why I don't move the docs to a wiki page, like [GitHub's wiki feature](), or use [GitHub pages]() or perhaps even a third party tool like [Read the docs](https://readthedocs.org/) and my answer has always been the same: Because they all suck for documentation!

I find them particularly bad for documentation because they often fail to sufficiently address one of the two most important aspects of what makes good documentation in my opinion:

1. Help your users
2. Stay up to date (aka provide accurate information)

The first point is without doubt the most important aspect of all. If a project's documentation doesn't address 1.) sufficiently, then there's no point in even having documentation at all.

## A single `DOCUMENTATION.md` file helps your users

### Discovery

Before users can read (and hopefully benefit from) your documentation they need to be able to find it in the first place. Having all of your documentation in a single `DOCUMENTATION.md` file makes that discovery process a lot easier.

First the file is labelled in big capital letters stating "DOCUMENTATION". This is definitely a good start. Secondly it is right next to a file called `README.md`, which is a pretty well established (and understood) concept in the open source community. The chances that someone will find the `DOCUMENTATION.md` file which resides right next to the `README.md` file is considerably high I would say. An additional reference from the `README.md` file pointing to the `DOCUMENTATION.md` file often helps to eliminate any leftover chances of someone not finding my project's documentation.

Furthermore the discovery process is massively boosted by the fact that the `DOCUMENTATION.md` file is stored inside the project's Git repository. Given that most open source projects are hosted by one of the big Git hosting providers (GitHub, BitBucket, GitLab, etc.) there's a high probability that the `DOCUMENTATION.md` file will end up very high at a search engine's results page - and let's face it, this is probably how the majority of your users will search for documentation in the first place anyway. It will probably even outrank any custom homepage or wiki page which is rarely a coincidence. GitHub's, BitBucket's and other Git hosting provider's main business model is to provide a user friendly hosting platform for your open source projects as well as a user friendly platform for your own users to easily discover, browse and contribute to your project. These platforms have discoverability at their heart and are extremely well SEO optimised. If my project's `DOCUMENTATION.md` file can benefit from that optimisation then I'm all up for it!

### Well understood structure

Once users find the link to the `DOCUMENTATION.md` file and click on it they will be presented with the actual content. At this point it is the maintainer's responsibility to make sure that the content is written and presented in such a way that it addresses the needs of its readers.

The ease of use and the initial experience of your users will often be determined by how familiar and comfortable they are with your documentation's structure. Wiki pages, GitHub pages, custom homepages, Read the docs pages and other third party tools have all their own take on how to structure a well laid out documentation. Is the menu on the top? Maybe on the left or right? Does it slide or collapse and where does it go when I open it on a mobile device with a much smaller screen size? Does it even have a menu? These questions seem extremely trivial, but they are often responsible for frustrations amongst users when they are not implemented in a good way.

I once had to read the documentation of a third party software which had so many menu items that the menu exceeded my screen's vertical length. Unfortunately at that time there was an issue with the website which didn't let me to scroll beyond the last visible item on the screen and I had to open my browser's developer tools to read the remaining items directly from the source code and open the links manually in a new tab. Needless to say that this was a terrible experience.

I'm sure that this issue has been fixed by now and I'm not saying that all third party tools have such a bad user experience, but regardless of what their actual UX is, each of them has a slightly different approach on how they structure their content. This is all good in the context of normal (commercial) websites, but it often forgets that documentation is much simpler than the usual website on the internet. Documentation doesn't require half of the things which a normal website can do today. Documentation is a read only exercise. Most importantly, documentation already has an ancient universally understood structure which every human is very likely to be familiar (and comfortable) with: The table of contents.

<img src="https://storage.googleapis.com/dusted-codes/images/book-table-of-contents.png" alt="Table of contents inside a book" class="three-quarters" />

A table of contents is so simple and effective that it is still used across all various industries for any content which happens to be larger than a single page. Magazines, books, catalouges, contracts and manuals of all sorts of kind use a table of contents in order to structure their content in a user friendly way.

A table of contents let's one structure a large document into smaller pieces without having to divide the content into multiple pages. If it works for print, e-books or large PDFs, then I don't see why it wouldn't work for a project's `DOCUMENTATION.md` file which is hosted on the web:

<img src="https://storage.googleapis.com/dusted-codes/images/giraffe-table-of-contents.png" alt="Table of contents for open source project" class="three-quarters" />

Instead of having to break up an online documentation into multiple pages which need to be maintained individually by a person or team, a table of contents allows one to have a single large `DOCUMENTATION.md` file which can be maintained a lot easier without losing the convenience of a structured document.

There's also no ambiguity where a user will find the menu (= table of contents). It's always at the top (or the front) of a document, regardless if it's been opened on a large screen, a small mobile device or if it's printed on paper.

### Search is king

Arguably the most important feature of good documentation (apart from the content itself) is the capability of quickly finding a certain piece of information. I would even go as far as to say that a website's search capability make or break a good documentation.

This again sounds like an extremely trivial problem and yet I feel like we're constantly getting it wrong to a point where a user can never really know whether they can trust a website's search box or not. Will the search give me meaningful suggestions when I type? Will it suggest relevant content even when I mistype something? How does the algorithm prioritise results? What key words do I need to search for? When I type "nodejs" will it also show me results which has "node.js" in the title? I'd hope so, but I can never be 100% sure.

For example there's a huge difference in the quality of results which I get from searching a programming question on Google or directly via [StackOverflow.com](https://stackoverflow.com). I want to get an answer from [StackOverflow.com](https://stackoverflow.com), but I still chose to search for it via Google because I know I'll get much better results.

The loss of trust in a website's search box has a deep implication on the user experience. If I'm browsing a website and I want to search for a specific topic which I am interested in and I don't get a perfect match to my first query then I'm rarely satisfied with my initial results. Often I will start altering my search query several times before accepting that the topic which I'm looking for might not be covered by the current state of documentation. Most likely I will even go to Google and try several search queries there before making any conclusions.

Another realisation which I have made is that nowadays it doesn't even matter how good the actual search of a website is. The mistrust in search boxes is so engrained in users' behaviour that even if a wiki page (or any other tool) has a perfectly functioning search box users will still go to a search engine and double check their results. It's kind of sad that users have been trained over time that the best way to search for any information is to leave the actual webstie where they want to get the information from and search for it on another estate.

The only search which I have found is more trusted than Google's search box is the browser's built-in text search (often quickly accessed through the <kbd>CTRL+F</kbd> or <kbd>CMD+F</kbd> shortcut). A single page `DOCUMENTATION.md` file can be easily and confidently searched by simply using a browser's built-in full text search - and regardless of what the results are - users know to trust it. From my experience this is the only time where a user *actually* trusts their initial search results and almost *never* re-validates via Google or any other search engine. In terms of user experience this is a huge benefit for keeping all documentation in a single large file. It makes finding information easy, predictable, trustworthy and extremely fast!

### Other benefits for users

There's quite a few other benefits which users get from a single `DOCUMENTATION.md` file:

- Search works offline. Having the documentation open in one tab allows a user to make effective use of it even when there is a loss of connectivity (e.g. sitting on a train, plane, airport, etc.).
- Downloadable - The entire documentation can be downloaded with a single click and is usable as it is. This is particularly true for a Markdown file which is already human readable without any extra software. Given that most users probably have some sort of editor which can nicely format Markdown files makes it even better.
- Print friendly - Some people like to have a print-out of the documentation. A single `DOCUMENTATION.md` file is extremely print friendly, since it doesn't need anything else in order to be user friendly for offline consumption.
- Looks good on any screen size. A single large documentation Markdown file is extremely friendly towards all sort of screen sizes.

## A single `DOCUMENTATION.md` file makes it easy to maintain

The most annoying thing about documentation is that it is extremely difficult to keep up to date if nobody really owns this responsibility. For an open source project this normally means that the lead maintainer has to constantly update the documentation after a new release has been published, which often proves to be a process which doesn't scale very well.

If all of the documentation is hosted in a single `DOCUMENTATION.md` file inside a project's Git repository then this responsibility can be easily shared with other contributors. There is a huge benefit in being able to search and replace function names and other code samples as part of the normal re-factoring process from an IDE. Without even having to actively think about documentation the normal search and replace feature in an IDE will automatically include any findings from the `DOCUMENTATION.md` too. It is also extremely useful to have the documentation being closely linked with the various branches of a project. This allows contributors and other maintainers to keep the documentation up to date as and when they make changes on a specific branch. Heck it can even be required to update the documentation as part of a pull request which gives the core maintainer a huge power over distributing this responsibility to other contributors as well.

Apart from never being out of sync another nice token is that there is never a delay in publishing the updated version of the documentation and the actual product release itself. As soon as a release is crafted and everything is merged back into master (from where the build will automatically deploy the latest version) the documentation has been merged into master as well and therefore become the latest updated iteration which matches the released code.

## Overall experience

Personally I've not found a single downside of having a single large documentation file written up in Markdown and kept close to my code inside my project's Git repository yet. It has proven to be an extremely powerful pattern which allows me to easily keep [Giraffe](https://github.com/giraffe-fsharp/Giraffe)'s documentation up to date, well maintained and extremely user friendly for everyone who's been using it so far.

I don't pretend that I've invented something new here or that I'm the first one doing this, but I simply wanted to state all the benefits which I have actively thought about when taking the decision to follow this pattern and I thought it would be worth sharing as I think other project's could certainly benefit of better documentation too.