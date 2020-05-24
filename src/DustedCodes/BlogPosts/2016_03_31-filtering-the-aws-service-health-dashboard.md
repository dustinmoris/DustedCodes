<!--
    Tags: aws google-chrome
-->

# Filtering the AWS Service Health Dashboard

If you run anything on [Amazon Web Services](https://aws.amazon.com/) in production then you probably know the [AWS Service Health Dashboard](http://status.aws.amazon.com/) very well.

Sometimes when you experience a service disruption you might find yourself scrolling through the page and look for an update for one of your affected resources and then you probably wish that the icons were a little bit more distinguishable between healthy and previously unhealthy services:

<img class="two-third-width" src="https://storage.googleapis.com/dusted-codes/images/blog-posts/2016-03-31/26168064985_56b3320748_o.png" alt="aws-service-health-dashboard, Image by Dustin Moris Gorski">

Unless you have perfect eagle sight you probably struggle to quickly filter the page with your plain eye and could use some help to better visualise good from bad icons. At least this is how I feel and therefore I created this little JavaScript snippet to blend out healthy icons from the page, leaving only the bad ones visible:

<pre><code>[].forEach.call(document.querySelectorAll('[src="/images/status0.gif"]'), function(e) {e.style.display = "none";});</code></pre>

<img class="two-third-width" src="https://storage.googleapis.com/dusted-codes/images/blog-posts/2016-03-31/26168064855_c38f2f124e_o.png" alt="aws-service-health-dashboard-filtered, Image by Dustin Moris Gorski">

You can either copy this into your browser's console and run it directly from there or [create a permanent bookmarklet](https://dusted.codes/diagnosing-css-issues-on-mobile-devices-with-google-chrome-bookmarklets) for easy access in the future.