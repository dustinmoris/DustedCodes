<!--
Tags: aspnet mvc-5 csharp-6 razor
-->

# Using C# 6 features in ASP.NET MVC 5 razor views

<p>Recently I upgraded my IDE to Visual Studio 2015 and made instant use of many new C# 6 features like the <a href="https://msdn.microsoft.com/en-us/library/dn986596.aspx">nameof keyword</a> or <a href="https://msdn.microsoft.com/en-us/library/dn961160.aspx">interpolated strings</a>.
</p>
<p>It worked (and compiled) perfectly fine until I started using C# 6 features in ASP.NET MVC 5 razor views:</p>
<a href="https://www.flickr.com/photos/130657798@N05/20768813781" title="Feature not available in C# 5 message by Dustin Moris Gorski, on Flickr"><img src="https://farm6.staticflickr.com/5658/20768813781_9d305e366b_o.png" alt="Feature not available in C# 5 message"></a>

<blockquote>
    <p>Feature 'interpolated strings' is not available in C# 5. Please use language version 6 or greater.</p>
</blockquote>

<p>The project compiles fine, but intellisense underlines my interpolated string in red and tells me that I can't use this feature in C# 5. Well I know that myself, but the real question is why does it think it is C# 5?</p>

<h2>It is the compiler's fault</h2>
<p>I knew I didn't have to change the .NET Framework version to .NET 4.6, because it is a language feature and not a .NET framework feature. The compiler is responsible for translating my C# 6 code into IL code which is supported by the framework.</p>

<p>However, saying that I don't get any errors at compilation time even though I made a lot of use of C# 6 features all over my project.</p>
<p>Maybe it is an intellisense bug in Visual Studio 2015? Not really, because when I start my project I get a yellow screen of death which matches the intellisense error:</p>
<a href="https://www.flickr.com/photos/130657798@N05/20575504479" title="Interpolated String Runtime Error in ASP.NET MVC 5 by Dustin Moris Gorski, on Flickr"><img src="https://farm1.staticflickr.com/566/20575504479_95b11bae10_o.png" alt="Interpolated String Runtime Error in ASP.NET MVC 5"></a>

<h3>ASP.NET Runtime compiler</h3>
<p>The problem is at runtime when ASP.NET tries to compile the razor view. ASP.NET MVC 5 uses the <a href="https://msdn.microsoft.com/en-us/library/system.codedom.compiler.codedomprovider(v=vs.110).aspx">CodeDOM Provider</a> which doesn't support C# 6 language features.
</p>

<h2>Solutions</h2>
<p>There are two solutions to fix the problem:</p>
<ol>
    <li>Upgrade your application to MVC 6 (which is still in beta at the time of writing)</li>
    <li>Reference the <a href="https://github.com/dotnet/roslyn">Roslyn compiler</a> in your project by using the <a href="https://msdn.microsoft.com/en-us/library/y9x69bzw(v=vs.110).aspx">compiler element</a> in your web.config</li>
</ol>

<p>The 2nd option is as easy as installing the <a href="https://www.nuget.org/packages/Microsoft.CodeDom.Providers.DotNetCompilerPlatform/">CodeDOM Providers for .NET Compiler</a> nuget package.
</p>
<p>It replaces the CodeDOM provider with the new .NET compiler platform (aka Roslyn) compiler as a service API. After installing the nuget package in your MVC 5 project you will be able to use C# 6 features in razor views as well!</p>