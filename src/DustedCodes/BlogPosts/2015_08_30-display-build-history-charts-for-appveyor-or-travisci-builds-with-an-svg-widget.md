<!--
    Tags: appveyor travisci github svg
    Type: HTML
-->

# Display build history charts for AppVeyor or TravisCI builds with an SVG widget

<p>If you ever browsed a popular GitHub repository (like <a href="https://github.com/nunit/nunit">NUnit</a> or <a href="https://github.com/twbs/bootstrap">Bootstrap</a>) then you must have seen many of the available SVG badges which can be used to decorate a repository's README file.
</p>
<p>While some repositories keep it very simple:</p>
<img src="https://cdn.dusted.codes/images/blog-posts/2015-08-30/20384154514_4e48fdc582_o.png" alt="NUnit Project Badges, Image by Dustin Moris Gorski">

<p>Others can be quite fancy:</p>
<img src="https://cdn.dusted.codes/images/blog-posts/2015-08-30/20996898652_6205e41d46_o.png" alt="Bootstrap Project Badges, Image by Dustin Moris Gorski">

<p>These little widgets (or often called badges) are more of a gimmick rather than anything useful, but we love them because they give us an opportunity to visually highlight statistics or achievements which we are proud of.</p>

<p>Having a few of <a href="https://github.com/dustinmoris">my own open source projects</a> I also wanted to decorate my README files with one or more fancy widgets.</p>
<p>I quite like the little build charts which you get in VisualStudio's Team Explorer and I thought I could create something similar for my <a href="http://www.appveyor.com/">AppVeyor</a> builds myself:</p>
<img src="https://ci-buildstats.azurewebsites.net/appveyor/chart/dustinmoris/dustedcodes" alt="Build history for Dusted Codes"/>

<p>A couple days later I also added support for <a href="https://travis-ci.org/">TravisCI</a> builds, uploaded everything to GitHub and hosted the widget in Windows Azure.</p>

<p>You can see it in action in <a href="https://github.com/dustinmoris/DustedCodes">one of my public GitHub repositories</a>.</p>

<p>Other repositories can use the widget as well by simply providing their own account and project name in the widget's URL. Additionally you can specify the number of builds to be shown and a switch to display or hide the build statistics.</p>

<p>For a complete up to date feature list and concrete code examples please visit the <a href="https://github.com/dustinmoris/CI-BuildStats">official project page</a>.
</p>

<p>Ideas, contributions, bug reports or any type of feedback is welcome!</p>