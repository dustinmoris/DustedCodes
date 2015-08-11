<!--
    Published: 2015-03-04 02:28
    Author: Dustin Moris Gorski
    Title: Making Font Awesome awesome - Using icons without i-tags
    Tags: font-awesome css
-->
<p><a href="http://fortawesome.github.io/Font-Awesome/">Font Awesome</a> is truly an awesome library. It is widely used across many websites and has made a web developer's life so much easier!</p>
<p>There is one thing though which I don't find that awesome about it! It pollutes your HTML with styling markup!</p>
<p>Let me explain the problem with an example...</p>
<h2>The Problem</h2>
<p>The default way to include an icon into your website is by adding an &lt;i&gt; tag into your HTML code; like this:</p>
<pre><code>&lt;i class=&quot;fa fa-car&quot;&gt;&lt;/i&gt;</code></pre>
<p>
    For example, the <a href="http://fortawesome.github.io/Font-Awesome/icon/car/">car icon</a> will render into:<br /><i class="fa fa-car"></i>
</p>
<p>This is great, but now you end up with many empty &lt;i&gt; tags in your HTML code, which have no structural purpose in the document whatsoever. Even worse, these tags are tightly coupled to a concrete theming technology (Font Awesome).</p>
<p>This is not so great and I personally think HTML markup should be theme agnostic.</p>

<h2>Solutions out in the wild</h2>
<p>A quick Google search brought up some solutions to the problem. None of them without any drawback though.</p>
<h3>#1 Shipping a modified copy of Font Awesome</h3>
<p>By providing your own copy of Font Awesome you'd obviously be able to do what you like and easily solve the problem, however this appraoch has some major issues:</p>
<ol>
    <li>I don't want to maintain a custom copy of Font Awesome</li>
    <li>The library is quite big and I really want to benefit from the public CDN</li>
</ol>
<h3>#2 Add the classes to the target element</h3>
<p>The next best suggestion was to attach the classes to the target tag instead of the &lt;i&gt; tag.<br />For example:</p>
<pre><code>&lt;a class=&quot;fa fa-car&quot; href=&quot;#&quot;&gt;This is a link&lt;/a&gt;</code></pre>
<p>Result:<br /><a class="fa fa-car" href="#">This is a link</a></p>
<p>As you can see this works, but Font Awesome has become the font for the entire anchor tag now. The browser ends up rendering the text in the default font, which mostly comes from the serif family and is rarely what you want.</p>
<h3>#3 Setting the unicode character in your custom CSS</h3>
<p>The most popular solution was to set the Unicode character for the content property of the ::before attribute:</p>
<pre><code>a:before {
    font-family: FontAwesome;
    content: &quot;\f1b9&quot;;
    display: inline-block;
    padding-right: 3px;
    vertical-align: middle;
}
</code></pre>
<p>The result will be visually perfect, but there are a few things which I don't like about this appraoch:</p>
<ol>
    <li>I have to add custom CSS for each tag which I want to decorate</li>
    <li>The unicode character can change</li>
    <li>It is not readable! CSS code is still code and deserves the same best practices like any other code</li>
</ol>

<h2>Analysing the Font Awesome CSS classes</h2>
<p>After I wasn't really satisfied with any of the proposed solutions I tried to find a maybe better one?</p>
<h3>Let's break it down</h3>
<p>Each icon consists of two CSS classes - the shared &quot;fa&quot; class and an icon-specific class like &quot;fa-car&quot;. Setting only the icon-specific class will result in a not very meaningful unicode character:</p>
<pre><code>&lt;a class=&quot;fa-car&quot; href=&quot;#&quot;&gt;This is a link&lt;/a&gt;</code></pre>
<p>Result:<br /><a class="fa-car" href="#">This is a link</a></p>
<p>The icon isn't what we want, but at least the tag's original font remains as is. Using the Google Chrome developer tools I can quickly confirm that the icon-specific class is not doing any harm to the original tag:</p>
<a href="https://www.flickr.com/photos/130657798@N05/16710024065" title="CSS source code of the Font Awesome car icon by Dustin Moris Gorski, on Flickr"><img src="https://farm9.staticflickr.com/8676/16710024065_9226643bf3_o.png" alt="CSS source code of the Font Awesome car icon"></a>
<p>Evidently this class only adds the content to the ::before attribute of the target element. The conclusion is that the actual styling gets applied via the &quot;fa&quot; class:</p>
<a href="https://www.flickr.com/photos/130657798@N05/16523945879" title="CSS source code of the Font Awesome fa class by Dustin Moris Gorski, on Flickr"><img src="https://farm9.staticflickr.com/8571/16523945879_3588abcda2_o.png" alt="CSS source code of the Font Awesome fa class"></a>
<p>Now this makes sense. The content from the ::before attribute gets rendered inside the original tag and therefore also picks up the styling from the fa class.</p>
<p>Everything from the fa class could equally go into the icon class as part of the ::before attribute, but I can see why the Font Awesome team has extracted it into a shared class, because it is the same for every icon and would be otherwise a maintenance nightmare.</p>

<h2>The ultimate solution (or at least the best I came up with)</h2>
<p>After inspecting the code we can work out that all it requires is to create one and only one more class in a custom style sheet to fix the dilemma:</p>
<pre><code>.icon::before {
    display: inline-block;
    margin-right: .5em;
    font: normal normal normal 14px/1 FontAwesome;
    font-size: inherit;
    text-rendering: auto;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    transform: translate(0, 0);
}</code></pre>
<p>It is a copy-paste of the original fa class with 3 changes to it:</p>
<ol>
    <li>I named the class &quot;icon&quot;</li>
    <li>I attached the ::before attribute</li>
    <li>I added a small margin-right, so that the icon doesn't stick to the original tag</li>
</ol>
<p>With this little trick it seems like I am able to tick all the boxes:</p>
<ul>
    <li>I can continue to use the original Font Awesome CDN library</li>
    <li>I don't need to add empty i-tags</li>
    <li>It doesn't interfere with other CSS on targeted elements</li>
    <li>I don't have to make a custom CSS change for each individual icon (unlike the unicode appraoch)</li>
    <li>The code is readable!</li>
</ul>
<p>
    Now it can be used like this:<pre><code>&lt;a class=&quot;icon fa-car&quot; href=&quot;#&quot;&gt;This is a link&lt;/a&gt;</code></pre>
</p>
<p>
    Result:<br /><a class="icon fa-car" href="#">This is a link</a>
</p>
<p>Especially the fact that I don't have to use the unicode characters is very appealing. When I read the code &quot;icon fa-car&quot; it remains self-explaining what the rendered result will look like!</p>
<h2>Conclusion</h2>
<p>At the moment I haven't found a drawback with this appraoch, but there might be something which I have overlooked. If you know of any issues or if you know an even better appraoch, then please let me know! Feedback is much appreciated!</p>