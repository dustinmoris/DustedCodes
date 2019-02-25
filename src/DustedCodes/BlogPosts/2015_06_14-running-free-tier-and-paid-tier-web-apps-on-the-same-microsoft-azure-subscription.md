<!--
    Tags: microsoft-azure app-hosting-plan
    Type: HTML
-->

# Running free tier and paid tier web apps on the same Microsoft Azure subscription

<p>Last week I noticed a charge of ~ &pound;20 by MSFT AZURE on my bank statement and initially struggled to work out why I was charged this much.</p>
<p>I knew I'd have to pay something for this website, which is hosted on the shared tier in Microsoft Azure, but according to <a href="http://azure.microsoft.com/en-us/pricing/calculator/">Microsoft Azure's pricing calculator</a> it should have only come to &pound;5.91 per month:</p>
<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2015-06-14/18821999662_b71b95637e_o.png" alt="Windows Azure Shared Pricing Tier">

<p>After a little investigation I quickly found the issue, it was due to a few on and off test web apps which were running on the shared tier as well.</p>
<p>This was clearly a mistake, because I was confident that I created all my test apps on the free tier, but as it turned out, after I upgraded my production website to the shared tier all my other newly created apps were running on the shared tier as well.</p>

<p>I simply didn't pay close attention during the creation process:</p>
<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2015-06-14/18829751471_b072e0ceaa_o.png" alt="Windows Azure Create new Web App">

<p>Evidentially every new web app gets automatically assigned to my existing app service plan, which I upgraded to the shared tier.</p>
<p>Luckily I learned my lesson after the first bill. However my initial attempt to switch my test apps back to the free tier was not as simple as I thought it would be. I cannot scale one app individually without affecting all other apps on the same plan:</p>
<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2015-06-14/18640926409_dbf2790205_o.png" alt="Windows Azure change pricing tier">

<p>The solution is to create a new app service plan and assign it to the free tier.</p>

<p>You can do this either when creating a new web app, by picking "Create new App Service plan" from the drop down:</p>
<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2015-06-14/18204493134_e04eba21dd_o.png" alt="Windows Azure Create new App Service plan">

<p>Or when navigating to the new Portal, where you have the possibility to manage your app service plans:</p>
<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2015-06-14/18821999642_d779125c72_o.png" class="half-width" alt="Windows Azure switch to Azure Preview Portal">
<img src="https://storage.googleapis.com/dustedcodes/images/blog-posts/2015-06-14/18640926369_1f679d0f4f_o.png" class="half-width" alt="Windows Azure New Portal App Service Plans Menu">

<p>This wasn't difficult at all, but certainly a mistake which can easily happen to anyone who is new to Microsoft Azure.</p>
<p>Another very useful thing to know is that if you choose the same data centre location for all your app service plans, then you can easily move a web app from one plan to another. This could be very handy when having different test and/or production stages (Dev/Staging/Production).</p>