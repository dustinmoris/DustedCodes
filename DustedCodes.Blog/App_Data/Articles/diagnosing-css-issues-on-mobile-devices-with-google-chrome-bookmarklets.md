<!--
    Published: 2015-12-31 01:15
    Author: Dustin Moris Gorski
    Title: Diagnosing CSS issues on mobile devices with Google Chrome bookmarklets
    Tags: css google-chrome
-->
Yesterday, when I was browsing my blog on my mobile phone I discovered a small CSS issue on one of the pages. One of my recent blog posts had a horizontal scrollbar which shouldn't have been there. A page element caused an overflow, but it was not visible which element was responsible for it.

When I tried to diagnose the issue on my computer I struggled to replicate it to the same extent as it was present on my mobile phone. I was too lazy to go through the entire page source to search for the needle in the haystack and decided to quickly create a [Google Chrome bookmarklet](https://support.google.com/chrome/answer/95745?hl=en) to help me with the investigation.

First I had to make the invisible visible.

With a little bit of Google's help and playing around in the Google Chrome Console (<kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>J</kbd>) I put this little JavaScript snippet together:

<pre><code>[].forEach.call(
    document.getElementsByTagName("*"),
    function(e) { 
        e.style.outline = "1px solid red";
});</code></pre>

When I execute this in the console it will outline every element on the page:

<a href="https://www.flickr.com/photos/130657798@N05/23989595201/in/dateposted-public/" title="page-with-outlined-elements"><img src="https://farm6.staticflickr.com/5783/23989595201_f7b29a924a_c.jpg"alt="page-with-outlined-elements"></a>

With this it should be easy to spot the overflowing element. Now I had to find a way to execute it inside the mobile version of Google Chrome.

For this purpose I created a new bookmarklet on my Desktop, which then got automatically synchronised to my phone.

A bookmarklet in Google Chrome allows you to execute JavaScript code on an already rendered page.

This is how you create it:

1.  <kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>O</kbd> in Google Chrome
2.  Right click &quot;Bookmarks bar&quot; on the left
3.  Click Add Page
4.  Pick a random name, e.g.: Outline Elements
5.  Type in the following snippet:

    `javascript: [].forEach.call(document.getElementsByTagName("*"), function(e) {e.style.outline = "1px solid red";});
  `
6.  Done!

When I click on the newly created bookmarklet I will get the same result as if I had executed the snippet in the console.

Only seconds later it appeared on my phone as well:

<a href="https://www.flickr.com/photos/130657798@N05/23447431843/in/dateposted-public/" title="mobile-google-chrome-bookmarks-bar"><img class="half-width" src="https://farm2.staticflickr.com/1480/23447431843_deb816c10b_o.png" alt="mobile-google-chrome-bookmarks-bar"></a>

However, when I click on the bookmarklet from the bookmarks menu on my phone everything freezes and nothing happens. It turns out that I have to execute it from the address bar.

Just start typing the name of your bookmarklet and Google Chrome will auto-suggest the rest:

<a href="https://www.flickr.com/photos/130657798@N05/23991652851/in/dateposted-public/" title="outline-elements-bookmarklet-in-mobile-google-chrome"><img class="half-width" src="https://farm6.staticflickr.com/5781/23991652851_1d9acee307_o.png" alt="outline-elements-bookmarklet-in-mobile-google-chrome"></a>

Executing it from the address bar delivers the correct result:

<a href="https://www.flickr.com/photos/130657798@N05/24074392275/in/dateposted-public/" title="page-with-outlined-elements-on-mobile-phone"><img class="half-width" src="https://farm2.staticflickr.com/1685/24074392275_2446d6a4fd_o.png" alt="page-with-outlined-elements-on-mobile-phone"></a>

This little trick quickly helped me to find the overflowing element on my phone without having to modify the original website. I use the same technique to remove advertising banners and other blocking content on several websites which normally don't display the entire content when you are not logged in (or a paying customer).