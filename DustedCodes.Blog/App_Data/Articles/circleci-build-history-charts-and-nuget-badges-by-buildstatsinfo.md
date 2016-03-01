<!--
    Published: 2016-02-29 23:45
    Author: Dustin Moris Gorski
    Title: CircleCI build history charts and NuGet badges by Buildstats.info
    Tags: circleci nuget github svg
-->
Quick update on [Buildstats.info](https://buildstats.info/). Two weeks ago I added [CircleCI](https://circleci.com/) to the list of  supported CI systems for the [build history chart](https://github.com/dustinmoris/CI-BuildStats#build-history-chart) and last weekend I implemented a new badge for [NuGet packages](https://github.com/dustinmoris/CI-BuildStats#nuget-badge) too.

CircleCI is the third continuous integration system which is supported by the build history chart now. AppVeyor and TravisCI are the other two. If you have a public open source project which is built by one of those systems then you might want to check out the official [documentation for the build history chart](https://github.com/dustinmoris/CI-BuildStats). Its quite cool and lets you create SVG badges like the one I did for my blog:

[![Build History Chart](https://buildstats.info/appveyor/chart/dustinmoris/dustedcodes?branch=master)](https://ci.appveyor.com/project/dustinmoris/dustedcodes/history?branch=master)

On a complete separate note I also added a new SVG badge for [NuGet packages](https://github.com/dustinmoris/CI-BuildStats#nuget-badge).

I did not think of adding NuGet support in the beginning, but since [Shields.io](http://shields.io/) NuGet badges are broken for more than 2 weeks now I had to look for an alternative:

<a  href="https://www.flickr.com/photos/130657798@N05/25255668592/in/datetaken/" title="shields.io-broken-nuget-badges"><img src="https://farm2.staticflickr.com/1630/25255668592_5362a02717_o.png" alt="shields.io-broken-nuget-badges" class="half-width"></a>

The [issue has been reported](https://github.com/badges/shields/issues/655), but it does not look like it might get fixed any time soon.

For my own projects I like to display the current version of my NuGet packages as well as the total number of downloads. With Shields.io I had to use two individual badges, but with Buildstats.info I can display both in one:

[![NuGet Badge for NUnit](https://buildstats.info/nuget/nunit)](https://www.nuget.org/packages/NUnit/)

This is a first version which satisfied my own personal needs, but will likely expand over the coming weeks providing more functionality for other users as well.

I have a few ideas, but if you are looking for something in particular then please feel free to [file a feature request](https://github.com/dustinmoris/CI-BuildStats/issues) on the public GitHub repository.