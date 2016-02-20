<!--
    Published: 2016-02-08 00:51
    Author: Dustin Moris Gorski
    Title: SHA-256 is not a secure password hashing algorithm
    Tags: security password-hashing cryptography brute-force-attacks
-->
[SHA-256](https://en.wikipedia.org/wiki/SHA-2#Cryptanalysis_and_validation) is not a secure password hashing algorithm. [SHA-512](https://en.wikipedia.org/wiki/SHA-2#Cryptanalysis_and_validation) neither, regardless of how good it has been salted. Why not? Because both can be computed in the billions per minute with specialised hardware. If you are surprised to hear that you should continue reading...

## What makes a good password hashing algorithm?

A password hash is the very last line of defence. Its only purpose is to prevent an attacker from gaining total control of a user when all other measures of security have been broken. This usually means to prevent the attacker from using the compromised data to access users' data on other websites, which could happen [when a user re-uses a password](http://www.troyhunt.com/2011/06/brief-sony-password-analysis.html). It is extremely important that a good hashing algorithm will resist all attempts of cracking it, at least for a significant period of time.

Since the attacker is in control of the raw user data there is nothing which can be done to prevent a crude brute force attack. However, this is not an easy undertaking and there are measures which can be put into place to prolong the attack and jeopardise its feasibility.

A good password hashing algorithm removes the slightest chance of a shortcut, leaving a brute force attack as the only attack surface and puts other barriers in place.

### One-way functions

First of all this means that a password must **always** be stored with a cryptographic one-way function. If a password has been encrypted with an algorithm which allows decryption then there is no guarantee that an attacker has not already gained access to the secret key and immediately bypassed all gates of security.

Therefore encryption algorithms such as [AES](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard) and [RSA](//dusted.codes/the-beauty-of-asymmetric-encryption-rsa-crash-course-for-developers) are not secure storage mechanisms for a password. The use of a one-way hash function is mandatory.

### Pre-image and collision attacks

A password hash also needs to resist so called [pre-image](https://en.wikipedia.org/wiki/Preimage_attack) and [collision attacks](https://en.wikipedia.org/wiki/Collision_attack). In simple words it should not be possible to methodically find a value which can be computed to a given hash value. This crosses out hash functions such as [MD5 and SHA-1 which have been proven to be vulnerable](https://en.wikipedia.org/wiki/Collision_attack#Classical_collision_attack) to such attacks.

### Lookup tables and password salting

Another shortcut are [lookup tables](https://en.wikipedia.org/wiki/Rainbow_table). A lookup table is a pre-computed table with hash values derived from commonly used passwords and dictionary entries. An attacker can easily match up a lookup table with the compromised hash values and look up the underlying plain text password. This is where the [concept of salting](https://en.wikipedia.org/wiki/Salt_(cryptography)) comes into play.

A salt is a piece of text of certain length and complexity which is added to the original value before computing a hash. The idea is that the salt itself is random enough to generate a hash which will not exist in a pre-computed lookup table.

The salt is usually stored in plain text next to the hash value. This is required to allow a genuine login scenario with the original password. It doesn't matter if an attacker can see the salt, because it still invalidates a pre-computed lookup table.

### Random salt per user

It is good practice to generate a random salt per user. If the same salt has been shared among all users then an attacker can quickly generate a new lookup table and is back at square one. However, if every user has an individual salt then it becomes significantly more difficult.

Additionally a random salt per user prevents the use of reverse lookup tables. A reverse lookup table is similar to a lookup table except that it matches up the password of multiple users at once. This is possible because many users pick the same (simple) password without knowing it.

### Key-stretching algorithms

By using salts and eliminating the possibility of pre-computed lookup tables an attacker is forced to go down the route of a brute force attack. Even though it is extremely difficult it is not impossible. High end hardware with [fast GPU can compute billions of hashes per minute](http://www.zdnet.com/article/25-gpus-devour-password-hashes-at-up-to-348-billion-per-second/).

How can one protect a password from being brute force attacked like this? The idea is to slow down the hashing function. This technique is called [key stretching](https://en.wikipedia.org/wiki/Key_stretching) and is a specially crafted algorithm which is very hardware intensive. Such algorithms usually come with an iteration factor which needs to be carefully adjusted to the hardware used on a web server. **This is the currently recommended way of storing passwords**.

Popular key-stretching algorithms are:

- [PBKDF2](https://en.wikipedia.org/wiki/PBKDF2)
- [bcrypt](https://en.wikipedia.org/wiki/Bcrypt)
- [scrypt](https://en.wikipedia.org/wiki/Scrypt)

The .NET framework has native built in support for PBKDF2 which comes in form of the [Rfc2898DeriveBytes](https://msdn.microsoft.com/en-gb/library/system.security.cryptography.rfc2898derivebytes%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) class. There is also an open source library for [bcrypt in .NET](https://bcrypt.codeplex.com/).

### Summary

As you can see good password hashing is more than just sticking a salt at the end of a password and shoving it into the SHA-256 hash function. In practical terms this is as bad as using MD5.

Correct password hashing is not too complicated either, but if it could be avoided all together it would be even better. There is always the option of defering the password handling to a 3rd party by using single sign-on options from trusted authorities such as Google, Facebook or Twitter.

If you have to do it yourself you should follow the guidelines from above and use a key derivation algorithm (=key stretching) in combination with a random salt per user and stick with the native implementation. Don't try to create your own algorithm as you will only get it wrong and end up with a hash function which can be easily parallelised on the CPU and potentially make it even worse.

Last but not least you should always encourage your users to chose strong and unique passwords and [not limit them in doing so](http://www.troyhunt.com/2011/03/only-secure-password-is-one-you-cant.html).