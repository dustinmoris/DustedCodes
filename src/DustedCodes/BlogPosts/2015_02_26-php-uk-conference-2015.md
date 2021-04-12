<!--
    Tags: php-uk versioning hhvm
    Type: HTML
-->

# PHP UK Conference 2015

<p>Last week was my first time at the <a href="http://phpconference.co.uk/">PHP UK Conference</a> in London. As a .NET developer who is very new to the PHP community I didn't have any particular expectations, but I think this year was a great time to be there!</p>
<h2>The Venue</h2>
<img src="https://cdn.dusted.codes/images/blog-posts/2015-02-26/16626982306_ed7994ed96_o.jpg" alt="PHP UK Conference 2015 - The Venue, Image by Dustin Moris Gorski" class="half-width"><img src="https://cdn.dusted.codes/images/blog-posts/2015-02-26/16445626857_00f842eb7e_o.jpg" alt="PHP UK Conference 2015 - Open Bar, Image by Dustin Moris Gorski" class="half-width">
<p>The conference took place on Thursday and Friday (apparently for the first time, because in previous years it was Friday and Saturday) at <a href="http://www.thebrewery.co.uk/">The Brewery</a>, which is a great venue at a very central location in the city of London.</p>
<p>Also worth noting is that this year was the 10<sup>th</sup> anniversary of the event with a record high of more than 700 attendees coming from many different countries around the world. The hosting was excellent, food and drinks were available throughout the entire day and in the evenings they had many hundreds of free beer up for grab to celebrate this occasion.</p>
<p>As if this was not enough, the organisers even rented out the <a href="http://www.allstarlanes.co.uk/venues/brick-lane/karaoke/">All Star Lanes in Brick lane</a> to continue the celebration with some free bowling, free karaoke, more beer, more food and a cake on Thursday night.</p>
<p>I have to admit that due to a light cold I didn't make the most out of it, but I still had a fantastic time!</p>
<h2>The Tracks</h2>
<img src="https://cdn.dusted.codes/images/blog-posts/2015-02-26/16651908382_77919e91e6_o.jpg" alt="PHP UK Conference 2015 - Closing Keynote, Image by Dustin Moris Gorski" class="half-width"><img src="https://cdn.dusted.codes/images/blog-posts/2015-02-26/16651888422_9c337e1d3f_o.jpg" alt="PHP UK Conference 2015 - Opening Keynote, Image by Dustin Moris Gorski" class="half-width">
<p>Both days started off with two great key notes. The first keynote was by <a href="https://twitter.com/coderabbi">coderabbi</a>, who walked us through some of his own experiences, spoke about code reviews and peer coding and eventually spread his wisdom over a packed room.</p>
<p>On Friday <a href="https://twitter.com/miss_jwo">Jenny Wong</a>, a developer and (according to her own words) a "community junkie" kicked off the second day with a very light hearted and extremely inspirational speech about bringing developer communities together.</p>
<h3>Integrating Communities</h3>
<p>The message which I took away is, that it doesn't matter who you are, which technology you use or how experienced you are. We are all connected through the same passion which brought us together - the passion for coding. Sadly we are in an industry where people burn out every day. Rather than belittling and laughing at other developers (even if it is a Wordpress developer :)) we should help each other and help this community to grow and not shrink.</p>
<p>Definitely one of the sessions which got stuck most with me!</p>
<p>Some other sessions which I really enjoyed were "Composer Best Practices", "Application Logging & Logstash", "Modern Software Architectures" and "HHVM at Etsy".</p>
<p>They were perhaps not the most technically advanced sessions, but they all had a great speaker, who was able to present something interesting from a personal experience and give some insights on the topic which were beyond the usual literature which you'll find in books or on the internet.</p>
<p>I won't rehash all of them in this blog post, but I picked a few where I wanted to share some interesting take aways!</p>
<h3>Versioning</h3>
<p>In the session about Composer best practices <a href="http://seld.be/">Jordi Boggiano</a> started off by explaining the meaning and importance of <a href="http://semver.org">semantic versioning</a>.</p>
<p>We all know what a build number looks like. It consists at least of three numbers, separated by a dot, where the parts usually stand for the major number, the minor number and a patch or sometimes referred to as the build number.</p>
<p>e.g.: <strong>{major}.{minor}.{patch}</strong></p>
<p>What semantic versioning does is to give a clearly defined meaning to each of these numbers with the aim to ease the upgrade and migration pain when developers use 3<sup>rd</sup> party code libraries.</p>
<p>In a nutshell, major stands for compatibility breaks (even the smallest!), minor stands for new features (without affecting other features) and patch denotes bug fixes (for existing features). Sounds simple, but this convention really makes a difference when you are dealing with someone else's code and you want to understand the implications of a NuGet package update or similar.</p>
<p>It definitely changed my view on versioning artifacts and if this was new to you as well, then I hope it makes as much sense to you as it did to me :).</p>
<h3>HHVM and PHP 7</h3>
<p>The other talk I wanted to quickly share with you was HHVM at <a href="https://www.etsy.com/">Etsy</a> by <a href="https://twitter.com/jazzdan">Dan Miller</a>.</p>
<p>For those who don't know what <a href="http://hhvm.com/">HHVM</a> is, it is a virtual machine which compiles PHP code down into native C++ for better performance. HHVM is written by Facebook in <a href="http://hacklang.org/">Hack</a> and PHP, and Hack itself is written by Facebook as well.</p>
<p>HHVM stands for HipHop Virtual Machine and is a replacement for the initial HipHop compiler. The major difference is that HHVM is a just-in-time compiler, while HipHop wasn't.</p>
<h4>Why a just-in-time compiler for PHP?</h4>
<p>Now you probably ask yourself why did Facebook prefer a just-in-time compiler over the other? While there are lots of reasons for it, one of the perhaps minor, but interesting ones was the difficulty to introduce HipHop into the existing development culture. PHP developers are used to drop in replace a file on a server and know that the changes have taken immediate effect. Now with a compilation step in between this was not possible anymore and was a rather huge shift among engineers. A just-in-time compiler on the other hand allowed developers to continue their work flow as they were used to.</p>
<h4>HHVM points of interest</h4>
<p>Another few interesting things I have learned:</p>
<ul>
    <li>Facebook claims that HHVM increases the throughput between 3 to 6 times. These benchmarks were taken with a much older PHP version though and Etsy's experience was more towards 3x in comparison with PHP 5.</li>
    <li>HHVM is used by other big companies like Wikipedia and Baido as well.</li>
    <li>HHVM is extremely solid. Etsy had no issues with HHVM whatsoever and did not encounter any bugs, even though their entire code base is in PHP and they expected at least some weird PHP constraints causing errors - but to their pleasant surprise, they didn't.</li>
    <li>BUT, they found many issues with 3<sup>rd</sup> party modules for HHVM. They had to fix some pretty major bugs for the Memcached and MySQL modules to get them running.</li>
    <li>If a HHVM module is not yet in use by another big company, then expect to invest a fair amount of time for bug fixing.</li>
    <li>Facebook seems to be very enganged with the project. There were stories told at the conference where people reported a bug and it has been fixed only a few hours later over night. This is kind of cool stuff and music in a developer's ear :)!</li>
    <li>
        HHVM offers a variety of other extremely useful features like:
        <ul>
            <li>
                <strong>HHVM Debugger</strong><br />
                Allows you to set conditional break points, e.g.: only hit the break point for useid=123.
            </li>
            <li>
                <strong>sgrep</strong><br />
                Great tool for static code analysis which offers a simpler syntax than conventional regex.<br />
                e.g.: <code>sgrep -e 'X &amp;&amp; X'</code> will return all the code lines where the left- and right hand statement of a logical AND operator is the same.
            </li>
            <li>
                <strong>spatch</strong><br />
                Great tool for refactoring your code.<br />
                Good PHP IDEs will offer you refactoring tools as well, but they all rely on text search and replace, why they won't give you 100% confidence that all changes have been made properly in your entire code base.<br />
                e.g.: Remove 2<sup>nd</sup> argument from a method, etc.
            </li>
        </ul>
    </li>
    <li>And last but not least: Some early performance benchmarks between HHVM and PHP 7 showed that PHP 7 gets very close to HHVM. In one benchmark it even outperformed HHVM, but they didn't dived too much into the details and the quality of these figures, so please don't pin this on the wall yet.</li>
</ul>
<h2>Summary</h2>
<p>All in all the PHP UK conference was an amazing event and I am glad I had the opportunity to be part of it! Will I go next year again? It is definitely on my list! Hopefully I will see you next year PHP folks!</p>