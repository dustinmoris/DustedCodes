<!--
	Tags: slack suave fsharp
-->

# Creating a Slack bot with F# and Suave in less than 5 minutes

Slack has quickly gained a lot of popularity and became one of the leading team communication tools for developers and technology companies. One of the main compelling features was the great amount of integrations with other tools and services which are essential for development teams and project managers. Even though the [list of apps](https://slack.com/apps) seems to be huge, sometimes you need to write your own custom integration and Slack wants it to be as easy and simple as possible. How easy and how fast this can be done I will show you in this blog post.

## A simple slash command

In most cases you will probably need to create one or more new slash commands which can be used by slack users to perform some actions.

For this tutorial let's assume I would like to create a new slash command to hash a given string with the SHA-512 algorithm. I would like my slack users to be able to type `/sha512 <some string>` into a channel and a slack bot to reply with the correct hash code.

The easiest way to achieve this is to create a new web service which will perform the hashing of the string and integrate it with the [Slash Commands API](https://api.slack.com/slash-commands).

## Building an F# web service which integrates with Slash commands

Let's begin with the web service by creating a new F# console application and installing the [Suave web framework](https://www.nuget.org/packages/Suave) NuGet package.

The [Slash Commands API](https://api.slack.com/slash-commands) will make a HTTP POST request to a configurable endpoint and submit a bunch of data
which will provide all relevant information to perform our action. First I want to model a `SlackRequest` type which will represent the incoming POST data from the Slash Commands API:

<pre><code>type SlackRequest =
    {
        Token       : string
        TeamId      : string
        TeamDomain  : string
        ChannelId   : string
        ChannelName : string
        UserId      : string
        UserName    : string
        Command     : string
        Text        : string
        ResponseUrl : string
    }</code></pre>

For this simple web service the only two relevant pieces of information are the token and the text which get submitted. The token represents a secret string value which can be used to validate the origin of the request and the text value represents the entire string which the user typed after the slash command. For example if I type `/sha512 dusted codes` then the text property will contain `dusted codes` in the POST data.

Inside this record type I'm also adding a little helper function to extract the POST data from a `Suave.Http.HttpContext` object:

<pre><code>static member FromHttpContext (ctx : HttpContext) =
    let get key =
        match ctx.request.formData key with
        | Choice1Of2 x  -&gt; x
        | _             -&gt; ""
    {
        Token       = get "token"
        TeamId      = get "team_id"
        TeamDomain  = get "team_domain"
        ChannelId   = get "channel_id"
        ChannelName = get "channel_name"
        UserId      = get "user_id"
        UserName    = get "user_name"
        Command     = get "command"
        Text        = get "text"
        ResponseUrl = get "response_url"
    }</code></pre>

Next I'll create a function to perform the actual SHA-512 hashing:

<pre><code>let sha512 (text : string) =
    use alg = SHA512.Create()
    text
    |&gt; Encoding.UTF8.GetBytes
    |&gt; alg.ComputeHash
    |&gt; Convert.ToBase64String</code></pre>

Finally I will create a new Suave WebPart to handle an incoming web request and register it with a new route `/sha512` which listens for POST requests:

<pre><code>let sha512Handler =
    fun (ctx : HttpContext) -&gt;
        (SlackRequest.FromHttpContext ctx
        |&gt; fun req -&gt;
            req.Text
            |&gt; sha512
            |&gt; OK) ctx

let app = POST &gt;=&gt; path &quot;/sha512&quot; &gt;=&gt; sha512Handler

[&lt;EntryPoint&gt;]
let main argv =
    startWebServer defaultConfig app
    0</code></pre>

With that the entire web service - even though very primitive - is completed. The entire implementation is less than 60 lines of code:

<pre><code>open System
open System.Security.Cryptography
open System.Text
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

type SlackRequest =
    {
        Token       : string
        TeamId      : string
        TeamDomain  : string
        ChannelId   : string
        ChannelName : string
        UserId      : string
        UserName    : string
        Command     : string
        Text        : string
        ResponseUrl : string
    }
    static member FromHttpContext (ctx : HttpContext) =
        let get key =
            match ctx.request.formData key with
            | Choice1Of2 x  -&gt; x
            | _             -&gt; &quot;&quot;
        {
            Token       = get &quot;token&quot;
            TeamId      = get &quot;team_id&quot;
            TeamDomain  = get &quot;team_domain&quot;
            ChannelId   = get &quot;channel_id&quot;
            ChannelName = get &quot;channel_name&quot;
            UserId      = get &quot;user_id&quot;
            UserName    = get &quot;user_name&quot;
            Command     = get &quot;command&quot;
            Text        = get &quot;text&quot;
            ResponseUrl = get &quot;response_url&quot;
        }

let sha512 (text : string) =
    use alg = SHA512.Create()
    text
    |&gt; Encoding.UTF8.GetBytes
    |&gt; alg.ComputeHash
    |&gt; Convert.ToBase64String

let sha512Handler =
    fun (ctx : HttpContext) -&gt;
        (SlackRequest.FromHttpContext ctx
        |&gt; fun req -&gt;
            req.Text
            |&gt; sha512
            |&gt; OK) ctx

let app = POST &gt;=&gt; path &quot;/sha512&quot; &gt;=&gt; sha512Handler

[&lt;EntryPoint&gt;]
let main argv =
    startWebServer defaultConfig app
    0</code></pre>

Now I just need to build, ship and deploy the application.

## Configuring Slash Commands

Once deployed I am ready to add a new Slash Commands integration.

1. Go into your team's Slack configuration page for custom integrations.
  <br/>e.g.: `https://{your-team-name}.slack.com/apps/manage/custom-integrations`

2. Pick Slash Commands and then click on the "Add Configuration" button:

<img class="half-width" src="https://cdn.dusted.codes/images/blog-posts/2016-08-22/29087335371_13517d5f78_o.png" alt="slack-slash-commands-add-configuration, Image by Dustin Moris Gorski">

3. Choose a command and confirm by clicking on "Add Slash Command Integration":

<img src="https://cdn.dusted.codes/images/blog-posts/2016-08-22/29087334921_64c34738d3_o.png" alt="slack-slash-commands-choose-a-command, Image by Dustin Moris Gorski">

4. Finally type in the URL to your public endpoint and make sure the method is set to POST:

<img src="https://cdn.dusted.codes/images/blog-posts/2016-08-22/29087335691_dd7ae72d98_o.png" alt="slack-slash-commands-integration-settings, Image by Dustin Moris Gorski">

5. Optionally you can set a name, an icon and additional meta data for the bot and then click on the "Save Integration" button.

Congrats, if you've got everything right then you should be able to go into your team's Slack channel and type `/sha512 test` to get a successful response from your newly created Slack integration now.

If you are interested in a more elaborate example with token validation and Docker integration then check out my [glossary micro service](https://github.com/dustinmoris/Glossiator/blob/master/Glossiator/Program.fs).