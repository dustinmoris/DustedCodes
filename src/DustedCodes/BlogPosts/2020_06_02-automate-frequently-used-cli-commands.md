<!--
    Tags: devops kubernetes docker productivity
-->

# Automate frequently used CLI commands

Welcome back to my blog! I haven’t written anything here for more than a year now, which is something I very much regret, but I mostly blame real life for it! Luckily due to the COVID-19 pandemic everything has slowed down a bit and I get to spend more time on my blog, side projects and some writing again. It’s always hard to get back to something after a long break, so I thought I'd start with a short blog post on a little productivity tip to break the current blogging hiatus!

Every developer has a range of commonly used CLI commands which they have to run frequently in order to execute some everyday tasks. These commands often have a bunch of additional flags and arguments attached to them which are really hard to remember if you only need to run them once every so often. One way of making it easier to run these commands is to [configure short Aliases](https://thorsten-hans.com/5-types-of-zsh-aliases) which are much easier to remember if you're using something like [zsh](https://www.zsh.org/) or [bash](https://www.gnu.org/software/bash/) as your preferred shell (aliases also work in the [KornShell](http://www.kornshell.org/) and [Bourne shell](https://en.wikipedia.org/wiki/Bourne_shell) too). However, if you don't have a fancy Unix shell because you work on a restricted environment, an older version of Windows or you don't have the option of installing [WSL (Windows Subsystem for Linux)](https://docs.microsoft.com/en-us/windows/wsl/) on your Windows machine then there's another option which luckily works across most operating systems regardless which version or shell you're using!

## Good old shell/batch scripts

Creating a shell (or batch) script with an easy to remember name and placing it in a directory which has been added to your OS's `PATH` environment variable is a very simple and easy way of achieving pretty much the same for the majority of automation tasks!
 
First create a new folder called something like `useful-scripts` in your home directory:
 
###### Windows
 
```
mkdir %userprofile%\useful-scripts
```

###### Unix

```
mkdir $HOME/useful-scripts
```

This will create a new directory called `/useful-scripts` under... 

- `/users/<username>/useful-scripts` on Linux or macOS
- `C:\Users\<username>\useful-scripts` on Windows

Then you have to add this path to your `PATH` environment variable and voila, any shell or batch script which will be placed inside this folder will be available as if it was a command itself.

#### PATH on Windows

Note that when you're changing the `PATH` variable on Windows the `set PATH=%PATH%;C:\Users\<username>\useful-scripts` command will only set the path for the current terminal session and the change will not persist until the next one. In Windows 7 or later you can use the `setx` command to permanently set the `PATH` variable, however it's not very intuitive, because the command `setx PATH %PATH%;C:\Users\<username>\useful-scripts` will merge all values from your system wide `PATH` into your user specific `PATH` variable. Using `setx /M` will do the opposite, merge your user specific `PATH` values into the system wide `PATH` variable. Both are not recommended as it may cause unwanted side effects.

The GUI still remains the easiest way of permanently editing your `PATH` variable in Windows today.

Alternatively you can use the following PowerShell script too:

```
$currentPath = [Environment]::GetEnvironmentVariable("Path", "Machine")
$usefulScripts = "C:\Users\<username>\useful-scripts"
[Environment]::SetEnvironmentVariable("Path", $currentPath + ";" + $usefulScripts, "Machine")
```

#### PATH on Linux and macOS

Adding `/users/<username>/useful-scripts` to the `PATH` environment variable on Linux or macOS is fairly straight forward. You can either [add it to the profile of your preferred shell](https://stackabuse.com/how-to-permanently-set-path-in-linux/) or set it system wide by adding it to `/etc/profile.d` on Linux or `/etc/paths.d` on macOS.

## Useful Scripts

Now let's explore some example scripts which might be useful to add to your newly created directory.

### Switching kubectl context

A command which I frequently use is to authenticate the `kubectl` CLI with a specific Kubernetes cluster. I manage more than one Kubernetes cluster in Microsoft Azure as well as a few other ones in the Google Cloud Platform. Authenticating with an AKS cluster requires to run the following command:

```
az aks get-credentials --name <cluster-name> --resource-group <resource-group-name>
```

Authenticating with a cluster running inside the GKE requires a similar command:

```
gcloud container clusters get-credentials <cluster-name> --region <region-name>
```

If you manage multiple AKS clusters under different Azure subscriptions, or multiple GKE clusters under different Google Cloud projects then you'll also need to authenticate with the desired subscription/project before executing the `get-credentials` command:

```
gcloud config set project <gcp-project-name>
```

Having to type out all these commands every single time when wanting to connect to a specific cluster is a very tedious task and a good opportunity to shorten them into a more memorable abbreviation.

For example I'd like to only have to type `gcp-cluster-1` if I want to authenticate with `cluster-1` in GCP and only have to type `aks-cluster-a` when I'd like to authenticate with `cluster-a` in Azure.  

Creating the following scripts inside `useful-scripts` will exactly enable this:

###### /users/&lt;username&gt;/useful-scripts/gcp-cluster-1

```
gcloud config set project <project-1>
gcloud container clusters get-credentials <cluster-1> --region <region-name>
```

###### /users/&lt;username&gt;/useful-scripts/gcp-cluster-2

```
gcloud config set project <project-2>
gcloud container clusters get-credentials <cluster-2> --region <region-name>
```

###### /users/&lt;username&gt;/useful-scripts/aks-cluster-a

```
az account set --subscription <subscription-name>
az aks get-credentials --name <cluster-1> --resource-group <resource-group-name>
```

Note that on Windows you'll have to add the `.bat` file extension for it to work and on macOS or Linux you'll have to give the execute permission to the newly created scripts:

```
chmod +x /users/<username>/useful-scripts/*
```

### Cleaning up Docker containers and images

Another immensely useful script is to clean up old Docker containers and remove unused Docker images from your machine. If you frequently create and run Docker images locally then you will quickly accumulate many unused containers and unused or outdated images over time.

In older versions of Docker it was a bit of a mouthful to find and remove old Docker containers and images, but even with the latest Docker APIs and the availability of the `prune` command it's still nice to condense them into a single command:

###### Delete old containers (before Docker 1.13)

```
docker rm $(docker ps -a -q)
```

###### Delete untagged images (before Docker 1.13)

```
docker rmi $(docker images -a | grep "^<none>" | awk '{print $3}')
```

###### Delete old containers (Docker >= 1.13)

```
docker container -prune -f
```

###### Delete untagged images (Docker >= 1.13)

```
docker image prune -a -f
```

It can be more convenient to put both statements into a single `docker-cleanup` script:

```
docker container -prune -f
docker image prune -a -f
```

### Output all custom scripts

Creating these sort of helper scripts is a nice way of simplifying and speeding up every day workflows. One script which I always like to include is an `sos` script which will output all other scripts in case I have ever forgetten any of the other scripts :)

###### sos.bat on Windows

```
dir %USERPROFILE%\useful-scripts
```

###### sos on Linux or macOS

```
ls $HOME/useful-scripts
```

This will enable me to run `sos` in order to get a list of all other available &quot;commands&quot;.

