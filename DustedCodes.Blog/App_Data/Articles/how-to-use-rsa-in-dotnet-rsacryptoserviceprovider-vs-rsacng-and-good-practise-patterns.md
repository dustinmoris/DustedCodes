<!--
    Published: 2015-08-13 20:08
    Author: Dustin Moris Gorski
    Title: How to use RSA in .NET: RSACryptoServiceProvider vs. RSACng and good practise patterns
    Tags: dotnet rsa security asymmetric-encryption cryptography
-->
<p>In my last blog post I wrote a little <a href="http://dusted.codes/the-beauty-of-asymmetric-encryption-rsa-crash-course-for-developers">crash course on RSA and how it works</a> without looking into any specific language implementations. Today I'd like to explore the native implementations of .NET and the new <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacng(v=vs.110).aspx">RSACng</a> class which has been introduced with <a href="https://msdn.microsoft.com/library/ms171868.aspx#v46">.NET Framework 4.6</a>.</p>

<p>In .NET 4.6 you'll find three native RSA classes:</p>
<ol>
	<li><a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsa(v=vs.110).aspx">RSA</a></li>
	<li><a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(v=vs.110).aspx">RSACryptoServiceProvider</a></li>
	<li><a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacng(v=vs.110).aspx">RSACng</a></li>
</ol>

<h2>RSA Class</h2>
<p>The RSA class in <em>System.Security.Cryptography</em> is an abstract class which cannot be instantiated itself. It is the base class for all other RSA implementations and exists since .NET 1.1 in the mscorlib assembly.</p>
<p>It derives from the abstract class <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.asymmetricalgorithm(v=vs.110).aspx">AsymmetricAlgorithm</a>, which itself implements <a href="https://msdn.microsoft.com/en-us/library/system.idisposable(v=vs.110).aspx">IDisposable</a>. This means every instance of an RSA implementation should be disposed after its usage to free up memory as soon as possible.</p>
<p>The base class defines many methods, but most likely you will be interested in one of these which come with the default RSA contract:</p>
<ol>
	<li>Encrypting data</li>
	<li>Decrypting cipher data</li>
	<li>Signing data</li>
	<li>Signing a hash of data</li>
	<li>Validating signed data</li>
	<li>Validating a signed hash</li>
	<li>A factory method to instantiate an implementation of RSA</li>
</ol>

<h3>Differences between .NET Frameworks</h3>

<h4>.NET 3.5 and earlier</h4>
<p>In .NET 3.5 and earlier the RSA class was much smaller than it is today. It didn't have a contract for signing and validating data and only exposed two methods for encyrpting and decrypting a value:</p>
<ul>
	<li><a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsa.encryptvalue(v=vs.90).aspx"><code>public abstract byte[] EncryptValue(byte[] rgb)</code></a></li>
	<li><a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsa.decryptvalue(v=vs.90).aspx"><code>public abstract byte[] DecryptValue(byte[] rgb)</code></a></li>
</ul>
<p>Also notice how the methods accept a data object but no indication of which padding system should be used. Using an <a href="http://rdist.root.org/2009/10/06/why-rsa-encryption-padding-is-critical/">encyrption padding is critical to the security of your RSA implementation</a> and therefore is somewhat lacking in the base contract.</p>

<h4>After .NET 4.0</h4>
<p>Starting with the .NET 4.0 framework the RSA class has been significantly extended. In addition to all the signing methods it received two new methods for encrypting and decrypting a message:</p>
<ul>
	<li><code>public virtual byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)</code></li>
	<li><code>public virtual byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)</code></li>
</ul>
<p>Interestingly they are not mentioned in <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsa(v=vs.100).aspx">the official MSDN documentation</a> on the web, however when I decompile .NET 4.0's mscorlib I can see the two virtual methods:</p>
<a href="https://www.flickr.com/photos/130657798@N05/20377339999" title="Encrypt and Decrypt methods in .NET C# RSA Class by Dustin Moris Gorski, on Flickr"><img src="https://farm6.staticflickr.com/5701/20377339999_8cd6511ee9_o.png" alt="Encrypt and Decrypt methods in .NET C# RSA Class"></a>

<p>This was a great addition for two reasons in particular:</p>
<ol>
    <li>It allows you to specify which padding should be used.</li>
    <li>It matches the implementation of the RSACryptoServiceProvider. This is very nice, because the RSACryptoServiceProvider never bothered to implement the EncryptValue and DecryptValue methods and made it impossible to program against the native RSA contract in previous .NET versions.</li>
</ol>

<h3>Factory methods</h3>
<p>The RSA class also implements two static factory methods to create an instance of RSA:</p>
<ol>
    <li><a href="https://msdn.microsoft.com/en-us/library/7taa5dzy(v=vs.110).aspx"><code>public static RSA Create()</code></a></li>
    <li><a href="https://msdn.microsoft.com/en-us/library/5ws2s1f6(v=vs.110).aspx"><code>public static RSA Create(string algName)</code></a></li>
</ol>
<p>In all versions of .NET the default implementation is the RSACryptoServiceProvider:</p>
<pre><code>using (var rsa = RSA.Create())
{
    Console.WriteLine(rsa.GetType().ToString());
    // Returns System.Security.Cryptography.RSACryptoServiceProvider
}</code></pre>

<h4>Overriding the default implementation</h4>
<p>The factory methods are designed to work with the <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.cryptoconfig(v=vs.110).aspx">CryptoConfig</a> setting from the machine.config file.</p>
<aside><em>System.Security.Cryptography.CryptoConfig</em> is a machine wide setting and not supported in the web.config on an application level.</aside>
<p>In order to override the default implementation you need to <a href="https://msdn.microsoft.com/en-us/library/vstudio/693aff9y(v=vs.100).aspx">map a friendly algorithm name to a specific cryptography class</a>.</p>
<p>Be aware that you have different machine.config files for each architecture and .NET framework:</p>
<ul>
    <li><strong>32-bit</strong><br />%windir%\Microsoft.NET\Framework\<em>{.net version}</em>\config\machine.config</li>
    <li><strong>64-bit</strong><br />%windir%\Microsoft.NET\Framework64\<em>{.net version}</em>\config\machine.config</li>
</ul>
<p>For example if you have a 64-bit application built with .NET 4.0 you need to modify the machine.config under this path:<br /><strong>%windir%\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config</strong></p>
<p>For this demo I will map &quot;SomeCustomFriendlyName&quot; to the native RSACng class:</p>
<pre><code>&lt;mscorlib&gt;
  &lt;cryptographySettings&gt;
    &lt;cryptoNameMapping&gt;
      &lt;cryptoClasses&gt;
        &lt;cryptoClass
         <strong>MyRSAImplementation</strong>=&quot;System.Security.Cryptography.RSACng,
          System.Core, Version=4.0.0.0, Culture=neutral,
          PublicKeyToken=b77a5c561934e089&quot; /&gt;
      &lt;/cryptoClasses&gt;
      &lt;nameEntry 
        name=&quot;<strong>SomeCustomFriendlyName</strong>&quot;
        class=&quot;<strong>MyRSAImplementation</strong>&quot; /&gt;
    &lt;/cryptoNameMapping&gt;
  &lt;/cryptographySettings&gt;
&lt;/mscorlib&gt;</code></pre>

<p>Now when I run this...</p>
<pre><code>using (var rsa = RSA.Create("SomeCustomFriendlyName"))
{
    Console.WriteLine(rsa.GetType().ToString());
}</code></pre>
<p>...it will output &quot;System.Security.Cryptography.RSACng&quot;.</p>
<p>If you don't specify a friendly algorithm name then the default <code>Create()</code> method will call <code>RSA.Create("System.Security.Cryptography.RSA")</code> under the covers.</p>
<p>Hence adding another nameEntry for &quot;System.Security.Cryptography.RSA&quot;:</p>
<pre><code>&lt;mscorlib&gt;
  &lt;cryptographySettings&gt;
    &lt;cryptoNameMapping&gt;
      &lt;cryptoClasses&gt;
        &lt;cryptoClass
          MyRSAImplementation=&quot;System.Security.Cryptography.RSACng,
          System.Core, Version=4.0.0.0, Culture=neutral,
          PublicKeyToken=b77a5c561934e089&quot; /&gt;
      &lt;/cryptoClasses&gt;
      &lt;nameEntry 
        name=&quot;SomeCustomFriendlyName&quot;
        class=&quot;MyRSAImplementation&quot; /&gt;
      <strong>&lt;nameEntry 
        name=&quot;System.Security.Cryptography.RSA&quot;
        class=&quot;MyRSAImplementation&quot; /&gt;</strong>
    &lt;/cryptoNameMapping&gt;
  &lt;/cryptographySettings&gt;
&lt;/mscorlib&gt;</code></pre>
<p>...will now also output &quot;System.Security.Cryptography.RSACng&quot; when I run <code>RSA.Create().GetType().ToString()</code>.</p>

<h2>RSACryptoServiceProvider vs. RSACng</h2>

<h3>Where to find</h3>
<p>RSACryptoServiceProvider exists in mscorlib since .NET 1.1 while RSACng is a very recent addition with .NET 4.6 and lives in System.Core.</p>
<p><a href="https://github.com/dotnet/corefx">CoreFX is open source</a> and you can browse any implementation as well as the <a href="https://github.com/dotnet/corefx/blob/6b2d60061c87a2a3b0d11adafa1311ae18206259/src/System.Security.Cryptography.Cng/src/System/Security/Cryptography/RSACng.cs">RSACng class on GitHub</a>.</p>

<h3>Implementation</h3>
<p>Both classes are sealed and derive from the base RSA class and implement their members. However, both classes throw a NotSupportedException when calling EncryptValue and DecryptValue. You are forced to use the Encrypt and Decrypt methods which accept a padding.</p>
<p>Both classes are <a href="https://en.wikipedia.org/wiki/Federal_Information_Processing_Standards">FIPS</a> compliant!</p>
<p>Also worth mentioning is that both classes use the OS's underlying CSP (Crypto Service Provider) and CNG (Cryptography Next Generation) providers which are unmanaged code and should have considerably similar performance.</p>

<h3>Difference</h3>
<p>In short, the CNG implementation is...</p>
<blockquote>
    <p>...the long-term replacement for the CryptoAPI. CNG is designed to be extensible at many levels and cryptography agnostic in behavior.</p>
    <footer><cite><a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa376210(v=vs.85).aspx">Cryptography API: Next Generation</a>, MSDN Microsoft</cite></footer>
</blockquote>
<p>One difference which you will quickly notice is that the key in RSACng is managed by the <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.cngkey(v=vs.110).aspx">CngKey class</a> and can be injected into the constructor of RSACng, while RSACryptoServiceProvider is tied to its own key operations.</p>

<h3>Support</h3>
<p>CNG is supported beginning with Windows Server 2008 and Windows Vista. RSACng requires .NET framework 4.6 (or higher).</p>
<p>The Crypto API was first introduced in Windows NT 4.0 and enhanced in subsequent versions.</p>

<h2>Good practise patterns</h2>
<p>After all it doesn't really matter which implementation you choose if you abstract it away and program against a contract. This is good practise anyway and luckily .NET offers the abstract RSA class for this purpose out of the box.</p>
<h3>Constructor injection vs. Factory method</h3>
<p>Constructor injection is the de facto standard pattern for dependency injection in most cases. However, when you deal with an object which implements the IDisposable interface then constructor injection can be responsible for keeping the object longer alive than it needs to be.</p>
<p>Image I have this class:</p>
<pre><code>public class MyClass
{
    private readonly RSA _rsa;
	
    public MyClass(RSA rsa)
    {
        _rsa = rsa;
    }
	
    public void sendSecureMessage(string message)
    {
        byte[] data;
        // Convert the message into data
		
        _rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
		
        // More business logic
    }
}</code></pre>
<p>Whatever happens after <code>_rsa.Encrypt(...)</code> doesn't require the RSA object any longer and it should get disposed immediately.</p>
<p>I could call <code>_rsa.Dispose();</code> afterwards, but this would be a bad code smell, because:</p>
<ol>
    <li>MyClass is not the owner of the RSA instance. It didn't create it and therefore shouldn't dispose it.</li>
    <li>The RSA instance could have been injected somewhere else as well, therefore disposing it would cause a bug.</li>
    <li>There is a <a href="http://dailydotnettips.com/2014/01/15/benefit-of-using-in-dispose-for-net-objects-why-and-when/">benefit of the using statement</a> and the constructor injection pattern doesn't allow me to make use of it</li>
</ol>
<p>This means we are better of by using a different dependency injection pattern. In this instance the factory pattern is more suitable:</p>
<pre><code>public interface IRSAFactory
{
    RSA CreateRSA();
}

public class MyClass
{
    private readonly IRSAFactory _rsaFactory;
	
    public MyClass(IRSAFactory rsaFactory)
    {
        _rsaFactory = rsaFactory;
    }
	
    public void SendSecureMessage(string message)
    {
        byte[] data;
        // Convert the message into data
		
        using (var rsa = _rsaFactory.CreateRSA())
        {
            rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }
		
        // More business logic
    }
}</code></pre>
<p>I have created an IRSAFactory interface which defines a contract to create a new instance of RSA. Now I can inject this factory into MyClass and conveniently create a new RSA object on the fly and encapsulate it with the using statement to properly dispose it afterwards.</p>

<h3>Why not using the native RSA.Create() factory method?</h3>
<p>What is wrong with the native factory method? Why didn't I just do this:</p>
<pre><code>public class MyClass
{
    public void SendSecureMessage(string message)
    {
        byte[] data;
        // Convert the message into data
		
        using (var rsa = RSA.Create())
        {
            rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }
		
        // More business logic
    }
}</code></pre>
<p>Theoretically this is absolutely fine, but it makes it more difficult to provide an alternative implementation in my unit test suite, because the native factory is designed to work with the machine.config and cannot be changed programmatically!</p>
<h3>Wrapper for RSA</h3>
<p>Alternatively I could have created a wrapper for the RSA class like this:</p>
<pre><code>public abstract class RSAWrapper : RSA
{
    private static RSA _overridenDefaultRSA = null;

    public static void OverrideDefaultImplementation(RSA rsa)
    {
        _overridenDefaultRSA = rsa;
    }

    public static new RSA Create()
    {
        return _overridenDefaultRSA ?? RSA.Create();
    }
}</code></pre>
<p>...and then use it like the original class:</p>
<pre><code>public class MyClass
{
    public void SendSecureMessage(string message)
    {
        byte[] data;
        // Convert the message into data
		
        using (var rsa = <strong>RSAWrapper</strong>.Create())
        {
            rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }
		
        // More business logic
    }
}</code></pre>
<p>This would allow me to override the default implementation from the machine.config by calling into <code>OverrideDefaultImplementation(RSA rsa)</code> from my unit tests.</p>
<p>I personally prefer the first approach though, for the following reasons:</p>
<ol>
    <li>By injecting an IRSAFactory object into MyClass it is obvious from the outside which dependencies MyClass has</li>
    <li>It allows me to provide different mocks and stubs for different unit tests while running them in parallel. This would be very tricky with the static factory method.</li>
    <li>At some point I'd have to initialise the RSA key by using <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsa.fromxmlstring(v=vs.110).aspx">RSA.FromXmlString</a> or <a href="https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsa.importparameters(v=vs.110).aspx">RSA.ImportParameters</a>. With the IRSAFactory (which is an abstract factory btw) I could provide additional methods to do this and test the interaction between them as well.</li>
</ol>
<p>I hope this was useful and that I could shed more light on RSA in .NET.</p>
<p>There are a lot of resources on the internet showing how to use the RSACryptoServiceProvider and therefore I didn't want to re-iterate over the same topic again and focus more on some patterns beyond the default examples.</p>
<p>Using the RSACng class is very similar and the official MSDN documentation gives good examples on both as well.</p>
<p>If all dependent classes work with the abstarct RSA type then the only difference between both classes boils down to their composition in your factory.</p>