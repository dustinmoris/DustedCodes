<!--
    Published: 2015-05-01 00:35
    Author: Dustin Moris Gorski
    Title: Effective defense against distributed brute force attacks
    Tags: security brute-force-attacks
-->
<p>Protecting against brute force attacks can be a very tricky task.</p>
<p>Recently I was curious if there are any best practices to protect a website from distributed brute force attacks and I found a lot of interesting solutions:</p>

<h2>Lock an account after X failed login attempts</h2>
<p>The first method I found was very trivial. If a user reaches a certain limit of failed login attempts  the website locks down the account and refuses any further access.</p>
<p>A genuine user can unlock his or her account by requesting a recovery link via email or by changing their password via the password reset function.</p>
<h3>Problems with this pattern</h3>
<ul>
    <li>Introduces a targeted DOS attack surface. An attacker could easily lock out an account by purposefully providing the wrong password several times to either to block an account from using the service entirely or to force the user into a recovery path, where the attacker might have found further vulnerabilities.</li>
    <li>Doesn't protect against more sophisticated attacks (typically an attacker would pick the most common password an try it on all accounts, then pick the second most common password, etc.)</li>
    <li>Introduces a potential enumeration attack. An attacker can purposely provide a wrong password and determine if a certain email address/username exists if the account gets locked or not.</li>
</ul>

<h2>Blocking IP Addresses with too many failed login attempts</h2>
<p>This one is fairly simple and well known. If a certain IP address had too many failed login attempts, then further access from this IP address is denied.</p>
<h3>Problems with this pattern</h3>
<ul>
    <li>It doesn't help against a distributed brute force attack.</li>
    <li>Opens the door for another DOS attack.</li>
    <li>There is a good chance that users behind a shared network will lock themselves out if enough users type in a wrong password within a short period of time.</li>
</ul>

<h2>Whitelist-/Blacklisting IP Addresses</h2>
<p>The idea is that a user can limit access to his/her account based on IP address rules. It can be as simple as allowing access to one single IP address, multiple addresses or more complex rules around IP address ranges or subnets.</p>
<h3>Problems with this pattern</h3>
<ul>
    <li>Impractical for most websites or web services.</li>
    <li>This pattern requires a user to put effort into security configuration instead of being secure by default.</li>
    <li>Can become a maintenance nightmare.</li>
</ul>

<h2>Increase artificially the login time after each failed attempt</h2>
<p>This one I found very creative. Each failed login attempt causes the next failed login request to take longer by a factor X. A successful login will proceed in normal speed at any point of time. This allows a website to throttle a distributed brute force attack while providing good experience for a genuine user.</p>
<h3>Problems with this pattern</h3>
<ul>
    <li>If a genuine user makes a mistake shortly after an attack, they might end up with a long response time.</li>
    <li>The website ends up unnecessarily wasting threads. This can result in a potential DOS attack again!?</li>
</ul>

<h2>Implement a challenge like a CAPTCHA</h2>
<p>This appraoch is trying to stop automated bots from brute forcing an account by implementing a challenge, which supposedly can only be accomplished by a human. Captchas are a very popular solution, but there are many other creative approches to filter humans from machines which work on the same assumption.</p>
<h3>Problems with this pattern</h3>
<ul>
    <li>Bad user experience for the geuine user.</li>
    <li>Computer learning and social engineering make it a tough challenge to come up with a good filter.</li>
</ul>

<h2>Additional verification step</h2>
<p>Digital signatures, two factor authentication and many other patterns require an additional step of verification. They are highly effective against brute force attacks, but have their own down sides and might be impractical for many web services.</p>

<h2>Combination of patterns?</h2>
<p>Quickly you will find that one pattern on it's own might not do the trick. I tried to think of a good combination of patterns and potential pros and cons attached to them and my best idea was the following:</p>
<h3>Monitor the average fail rate and CAPTCHAs</h3>
<p>The website determines a natural rate of login failure over a certain period of time. Once this metric has been established it starts monitoring and counting failed login attempts going forward. When the number of failed login attempts significantly deviates from the natural rate then a CAPTCHA will be displayed on all subsequent login requests.</p>
<p>If the rate recovers then the CAPTCHA will be hidden from the login screen again. A very transparent website could even show a notification to the user explaining why the CAPTCHA is being displayed and remind the user to set a strong password if not done yet.</p>
<h4>Pros</h4>
<ul>
    <li>Effective against any type of brute force attack?</li>
    <li>Good user experience.</li>
</ul>
<h4>Cons</h4>
<ul>
    <li>Might be difficult to establish the initial variables.</li>
</ul>

<h2>Strict password policy</h2>
<p>Another very viable approach is to simply not fight brute force attacks at all. Make sure your users have strong passwords and make brute force attempts rather harmless.</p>
<p>A good password policy is probably a good idea in any case. As always, security comes in layers.</p>

<p>If you know any other effective defense systems against distributed brute force attacks I'd be interested in hearing them!</p>
