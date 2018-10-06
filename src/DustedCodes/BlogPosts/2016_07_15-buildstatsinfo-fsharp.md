<!--
	Tags: fsharp suave docker aws
-->

# BuildStats.info |> F#

After working on the project on and off for a few months I finally found enough time to finish the migration to F#, Suave and Docker of [BuildStats.info](https://buildstats.info/).

The migration was essentially a complete rewrite in F# and a great exercise to learn F# and [Suave.io](https://suave.io/) as part of a small side project which runs in a production environment now. Working with F# and Suave was so much fun that I'm already planning to develop a couple more small projects in the very near future, but more on this at another time.

Apart from migrating to F# and Suave I had also [dockerised the application](https://hub.docker.com/r/dustinmoris/ci-buildstats/) and switched my hosting from an [Azure Web App](https://azure.microsoft.com/en-gb/services/app-service/web/) to [Amazon EC2 Container Service](https://aws.amazon.com/ecs/), because it is considerably cheaper than Microsoft's [ACS](https://azure.microsoft.com/en-gb/services/container-service/) at the time of writing. I was also considering Docker Cloud and Google Container Service, but the fact that I can run a micro instance in Amazon for free for 12 months was the deciding factor which pushed me towards AWS and I am very happy so far.

Small side projects like this are always a great opportunity to try and learn new technologies beyond a simple hello world application and with that in mind I also decided to [document the service endpoint with RAML](https://github.com/dustinmoris/CI-BuildStats/blob/master/api.raml), which is a very intuitive language for describing web service APIs. [RAML was not entirely new to me](https://dusted.codes/design-test-and-document-restful-apis-using-raml-in-dotnet), but it was the first time I used version 1.0 and some of its new features.

Last but not least I also switched my CI system from [AppVeyor](https://www.appveyor.com/) to [TravisCI](https://travis-ci.org/dustinmoris/CI-BuildStats). I wasn't really planning to do this, but because I wanted to build the application with Mono on Linux I had to make that transition as well. Nevertheless I am still a big fan of AppVeyor and will continue using it as my primary CI sytem for all Windows based builds and Travis is just as great as well.

A lot of (good) things have been going on in my private life in the last 6 months and I didn't get as much time to blog and work on side projects as I wanted to do, but now that things have gotten a bit calmer again I hope to get more time to keep this blog more updated again and talk about all the stuff that I am doing every day.

Wish you all a great weekend and stay tuned :)