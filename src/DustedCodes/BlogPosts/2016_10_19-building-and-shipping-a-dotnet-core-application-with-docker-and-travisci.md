<!--
    Tags: dotnet-core travisci docker
-->

# Building and shipping a .NET Core application with Docker and TravisCI

With the .NET Core ecosystem slowly maturing since the [first official release](http://www.hanselman.com/blog/NETCore10IsNowReleased.aspx) this year I started to increasingly spend more time playing and building software with it.

I am a big fan of managed CI systems like [AppVeyor](https://www.appveyor.com/) and [TravisCI](https://travis-ci.org/) and one of the first things I wanted to work out was how easily I could build and ship a [.NET Core](https://www.microsoft.com/net/core#windows) application with one of these tools. This was a major consideration for me because I would have been less interested in building a .NET Core app if the deployment story wasn't great yet and I am not very keen in building my own CI server as I don't think this is the best use of a developer's time. Luckily I was very happy to find out that the deployment experience and integration with TravisCI is extremely easy and intuitive, which is what I will be trying to cover in this blog post today.

Up until now I was more or less tied down to AppVeyor as the only vendor which uses Windows Server VMs for its build nodes and therefore the only viable option of building full .NET framework applications. TravisCI and [other popular CI platforms](https://circleci.com/) use Linux nodes for their build jobs and .NET support was limited to the [Mono framework](http://www.mono-project.com/) at most. However, with .NET Core being the first officially Microsoft supported cross platform framework my options have suddenly increased from one to many. TravisCI already offered a good integration with Mono and now that [.NET Core is part of their default offering](https://docs.travis-ci.com/user/languages/csharp/#Testing-Against-Mono-and-.NET-Core) I was keen to give it a shot.

In this blog post I will be covering what I believe is a typical deployment scenario for a .NET Core application which will be shipped as a Docker image to either the [official Docker Hub](https://hub.docker.com/) or a private registry.

## 1. Creating a .NET Core application

First I need to create a .NET Core application. For the purpose of this blog post I am just going to create a default hello world app and you can skip this step for the most part if you are already familiar with the framework. For everyone else I will quickly skim through the creation of a new .NET Core application.

Let's open a Windows command line prompt and navigate to `C:\temp` and create a new folder called `NetCoreDemo`:

<pre><code>cd C:\temp
mkdir NetCoreDemo
cd NetCoreDemo</code></pre>

Inside that folder I can run `dotnet new --type console` to create a new hello world console application:

<img src="https://cdn.dusted.codes/images/blog-posts/2016-10-19/30273420421_c02db77a5e_o.png" alt="dotnet-new-console-app, Image by Dustin Moris Gorski" class="two-third-width">

For a full reference of the `dotnet new` command check out the [official documentation](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-new).

If you don't have the .NET Core CLI available you need to install the [.NET Core SDK for Windows](https://www.microsoft.com/net/core#windows) (or your operating system of choice).

After the command has completed I can run `dotnet restore` to restore all dependencies followed by a `dotnet run` which will build and subsequently start the hello world application:

<img src="https://cdn.dusted.codes/images/blog-posts/2016-10-19/30273420621_c451bc8473_o.png" alt="dotnet-restore-and-run, Image by Dustin Moris Gorski">

This is literally all I had to do to get a simple C# console app running and therefore will stop at this point and move on to the next part where I will set up a build and deployment pipeline in TravisCI.

If you want to learn more about building .NET Core applications then I would highly recommend to check out the [official ASP.NET Core tutorials](https://docs.asp.net/en/latest/tutorials/index.html) or read [other great blog posts](https://www.asp.net/community/articles) by developers who have covered this topic extensively.

## 2. Setting up TravisCI for building a .NET Core application

If you are not familiar with TravisCI yet (or a similar platform), then please follow the instructions to [set up TravisCI with your source control repository](https://docs.travis-ci.com/user/for-beginners) and add a `.travis.yml` file to your project repository. This file will contain the entire build configuration for a project.

The first line in the `.travis.yml` file should be the `language` declaration. In our case this will be `language: csharp` which is the correct setting for any .NET language (including VB.NET and F#).

Next we need to set the correct environment type.

The standard TravisCI build environment runs on an [Ubuntu 12.04 LTS Server Edition 64 bit](https://docs.travis-ci.com/user/ci-environment/#Virtualization-environments) distribution. This is no good for us because [.NET Core only supports Ubuntu 14.04 or higher](https://www.microsoft.com/net/core#ubuntu). Fortunately there is a new [Ubuntu 14.04 (aka Trusty) beta environment](https://docs.travis-ci.com/user/trusty-ci-environment/) available. In order to make use of this new beta environment we need to enable `sudo` and set the `dist` setting to `trusty`:

<pre><code>sudo: required
dist: trusty</code></pre>

Next I want to specify what version of Mono and .NET Core I want to have installed when running my builds. At the moment I am only interested in .NET Core so I am going to skip Mono and set the `dotnet` setting to the currently latest SDK:

<pre><code>language: csharp
sudo: required
dist: trusty
<strong>mono: none
dotnet: 1.0.0-preview2-003131</strong></code></pre>

The next step is not required nor necessarily recommended, but more of my personal preference to disable the [.NET Core Tools Telemetry](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/telemetry) by setting the `DOTNET_CLI_TELEMETRY_OPTOUT` environment variable to `1` during the `install` step of the TravisCI lifecycle:

<pre><code>install:
  - export DOTNET_CLI_TELEMETRY_OPTOUT=1</code></pre>

After that I have to set access permissions for two script files in the `before_script` step:

<pre><code>before_script:
  - chmod a+x ./build.sh
  - chmod a+x ./deploy.sh</code></pre>

The [chmod command](https://en.wikipedia.org/wiki/Chmod) changes the access permissions of my build and deployment script to allow execution by any user on the system. TravisCI recommends to set `chmod ugo+x` which is effectdively the same as `chmod a+x`, where `a` is a shortcut for `ugo`.

Following `before_script` I am going to set the `script` step which is responsible for the actual build instructions:

<pre><code>script:
  - ./build.sh</code></pre>

At last I am giong to define a `deploy` step as well, which will automatically trigger only after the `script` setp has successfully completed:

<pre><code>deploy:
  - provider: script
    script: ./deploy.sh $TRAVIS_TAG $DOCKER_USERNAME $DOCKER_PASSWORD
    skip_cleanup: true
    on:
      tags: true</code></pre>

Here I am essentially calling a second script called `deploy.sh` and passing in three environment variables which I will explain in a moment. Additionally I defined the trigger to deploy for tags only. You can set up [different deploy conditions](https://docs.travis-ci.com/user/deployment#Conditional-Releases-with-on%3A), but in most cases you either want to deploy on each push to `master` or when a commit has been tagged. I chose the latter, because sometimes I want to publish an alpha or beta version of my application which is likely to be on a different branch than `master` and therefore the tag condition made more sense in my case.

The `TRAVIS_TAG` variable is a [default environment variable](https://docs.travis-ci.com/user/environment-variables/#Default-Environment-Variables) which gets set by TravisCI for every build which has been triggered by a tag push and will contain the string value of the tag. `DOCKER_USERNAME` and `DOCKER_PASSWORD` are two custom [environment variables which I have set through the UI](https://docs.travis-ci.com/user/environment-variables/#Defining-Variables-in-Repository-Settings) to follow TravisCI's recommendation to keep sensitive data secret:

<img src="https://cdn.dusted.codes/images/blog-posts/2016-10-19/30401215765_13d4f6937d_o.png" alt="travisci-settings-page, Image by Dustin Moris Gorski">

Another option would have been to [encrypt environment variables](https://docs.travis-ci.com/user/environment-variables/#Encrypting-environment-variables) in the `.travis.yml` file to keep those values secret. Both options are valid as far as I know and it is up to you which one you prefer.

#### Tip:

If you have to store access credentials to 3rd party platforms like a private registry or the official Docker Hub inside TravisCI then it is highly recommended to register a dedicated user for TravisCI and add that user as an additional collaborator to your Docker Hub repository, so that you can easily limit or revoke access when required:

<img src="https://cdn.dusted.codes/images/blog-posts/2016-10-19/30347448781_1f15e0ded6_o.png" alt="docker-hub-collaborators, Image by Dustin Moris Gorski" class="two-third-width">

After defining the `script` and `deploy` step I am basically done with the `.travis.yml` file.

Note that I purposefully didn't choose to place the individual build and deployment instructions directly into the `script` step, because I wanted to seperate out the actual build instructions from the TravisCI configuration.

This has a few advantages:
- There is a clear distinction between environment setup and the actual build steps which are required to build and deploy the project. The `.travis.yml` file is the definition for the build environment and the `build.sh` and `deploy.sh` script files are the recipe to build and deploy an application.
- The build and deploy scripts are completely independent from the CI platform and I could easily switch the CI provider at any given time.
- The actual build and deployment scripts can be executed from anywhere. Both are a generic bash script which developers can run on their personal machines to build, test and deploy a project.

The last point is probably the most important in my view. Even though managed CI systems are super easy to integrate with, it can be a pain if you are tied down to a particular provider. Imagine you have a new developer joining your team and the first question they ask is how to build your project. It would be a pain to tell them to open up the `.travis.yml` file and follow all the instructions manually if you could just tell them to run `build.sh` and it will work.

If I put everything together then the final `.travis.yml` file will look something like this:

<pre><code>language: csharp
sudo: required
dist: trusty
mono: none
dotnet: 1.0.0-preview2-003131
install:
  - export DOTNET_CLI_TELEMETRY_OPTOUT=1
before_script:
  - chmod a+x ./build.sh
  - chmod a+x ./deploy.sh
script:
  - ./build.sh
deploy:
  - provider: script
    script: ./deploy.sh $TRAVIS_TAG $DOCKER_USERNAME $DOCKER_PASSWORD
    skip_cleanup: true
    on:
      tags: true</code></pre>

One last thing that I wanted to mention is that even though I said we are going to use Docker to deploy the project I didn't have to [specify Docker as an extra service](https://docs.travis-ci.com/user/docker/) anywhere in the `.travis.yml` file. This is because unlike the standard TravisCI environment the Trusty beta environment comes with Docker pre-configured out of the box.

## 3. Building and deploying a .NET Core app from a bash script

Now that the build environment is set up in the `.travis.yml` file and we deferred the entire build and deployment logic to external bash scripts we have to actually create those scripts to complete the puzzle.

#### build.sh

The `build.sh` script is going to be very quick:

<pre><code>#!/bin/bash
set -ev
dotnet restore
dotnet test
dotnet build -c Release</code></pre>

The first line is not necessarily required, but it is good practice to include `#!/bin/bash` at top of the script so the shell knows which interpreter to run. The second line tells the shell to exit immediately if a command fails with a non zero exit code (`set -e`) and to print shell input lines as they are read (`set -v`).

The last three commands are using the normal `dotnet` CLI to [restore](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-restore), [build](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-build) and [test](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-test) the application.

#### deploy.sh

The `deploy.sh` script is going to be fairly easy as well. The first two lines are going to be the same as in `build.sh` and then I am assigning the three parameters that we are passing into the script to named variables:

<pre><code>#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3</code></pre>

Next I am going to use the `dotnet` CLI [publish](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-publish) command to package the application and all of its dependencies into the publish folder:

<pre><code>dotnet publish -c Release</code></pre>

Now that everything is packaged up I can use the `docker` CLI to build an image with the supplied tag and the `latest` tag:

<pre><code>docker build -t repository/project:$TAG bin/Release/netcoreapp1.0/publish/.
docker tag repository/project:$TAG repository/project:latest</code></pre>

Make sure that `repository/project` matches your own repository and project name.

Lastly I have to authenticate with the official Docker registry and push both images to the hub:

<pre><code>docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push repository/project:$TAG
docker push repository/project:latest</code></pre>

And with that I have finished the continuous deployment setup with Docker and TravisCI. The final `deploy.sh` looks like this:

<pre><code>#!/bin/bash
set -ev

TAG=$1
DOCKER_USERNAME=$2
DOCKER_PASSWORD=$3

# Create publish artifact
dotnet publish -c Release src

# Build the Docker images
docker build -t repository/project:$TAG src/bin/Release/netcoreapp1.0/publish/.
docker tag repository/project:$TAG repository/project:latest

# Login to Docker Hub and upload images
docker login -u="$DOCKER_USERNAME" -p="$DOCKER_PASSWORD"
docker push repository/project:$TAG
docker push repository/project:latest</code></pre>

#### Tip:

Some projects follow a naming convention where version tags begin with a lowercase `v` in git, for example `v1.0.0`, but want to remove the `v` from the Docker image tag. In that case you can use this additional snippet to create a variable called `SEMVER` which will be the same as `TAG` without the leading `v`:

<pre><code># Remove a leading v from the major version number (e.g. if the tag was v1.0.0)
IFS='.' read -r -a tag_array <<< "$TAG"
MAJOR="${tag_array[0]//v}"
MINOR=${tag_array[1]}
BUILD=${tag_array[2]}
SEMVER="$MAJOR.$MINOR.$BUILD"</code></pre>

Place that snippet after the `dotnet publish` command in the `deploy.sh` and use `$SEMVER` instead of `$TAG` when building and publishing the Docker images.

If you want to see a full working example you can check out [one of my open source projects](https://github.com/dustinmoris/NewsHacker) where I use this setup to publish a Docker image of an F# .NET Core application.