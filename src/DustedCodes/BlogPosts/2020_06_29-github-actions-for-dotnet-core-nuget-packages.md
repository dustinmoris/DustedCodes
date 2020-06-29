<!--
    Tags: devops github dotnet-core ci-cd nuget
-->

# GitHub Actions for .NET Core NuGet packages

Last weekend I migrated the [Giraffe web framework](https://github.com/giraffe-fsharp/Giraffe) from [AppVeyor](https://www.appveyor.com) to [GitHub Actions](https://github.com/features/actions). It proved to be incredibly easy to do so despite me having some very specific requirements on how I wanted the final solution to work and that it should be flexible enough to apply to all my other projects too. Even though it was mostly a very straight forward job, there were a few things which I learned along the way which I thought would be worth sharing! 

Here's a quick summary of what I did, why I did it and most importantly how you can apply the same GitHub workflow to your own .NET Core NuGet project as well!

## Overview

- [CI/CD pipeline for .NET Core NuGet packages](#cicd-pipeline-for-net-core-nuget-packages)
    - [Branch and pull request trigger](#branch-and-pull-request-trigger)
    - [Test on Linux, macOS and Windows](#test-on-linux-macos-and-windows)
    - [Create build artifacts](#create-build-artifacts)
    - [Push nightly releases to GitHub packages](#push-nightly-releases-to-github-packages)
    - [GitHub release trigger for official NuGet release](#github-release-trigger-for-official-nuget-release)
    - [Drive NuGet version from Git Tags](#drive-nuget-version-from-git-tags)
    - [Speed](#speed)
- [Environment Variables](#environment-variables)
- [The End Result](#the-end-result)
    - [Four stages of a release](#four-stages-of-a-release)
    - [Workflow YAML](#workflow-yaml)

## CI/CD pipeline for .NET Core NuGet packages

First, let's look at the requirements which I set out for my final CI/CD pipeline to meet. Each of these points has a specific purpose which I think is applicable to most .NET Core NuGet projects and therefore explaining in more detail.

### Branch and pull request trigger

CI builds are the first formal check as part of the software development feedback loop which don't come from a developer's machine itself. They are reproducible and reliable feedback and arguably cheap to run in the cloud. As such CI builds should run as frequently as possible so that new errors can be flagged up as soon as they occur.

On this premise I decided that each commit, regardless if it happened on a `feature/*`, `hotfix/*` or other branch, should trigger a CI build. Pull requests should trigger a CI build as well. It's a great way of validating the changes of a PR before deciding whether to merge. As a matter of fact, it's highly recommended to enforce this rule through GitHub itself.

In GitHub, under **Settings** and then **Branches**, one can set up [branch protection rules](https://help.github.com/en/github/administering-a-repository/configuring-protected-branches) for a repository:

[![GitHub Branch Protection Rules](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-branch-protection-rules.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-branch-protection-rules.png)

*Note that the available CI options get automatically updated whenever a CI pipeline is executed and therefore might not show up before the first workflow run has completed.* 

We can configure a GitHub Action to trigger builds for commits and pull requests on all branches by providing the `push` and `pull_request` option and leaving the branch definitions blank:

```yaml
on:
  push:
  pull_request:
```   

### Test on Linux, macOS and Windows

.NET Core is cross platform compatible and so it's not a surprise that a NuGet library is expected work on Linux, macOS and Windows as well.

Running a CI job against multiple OS versions can be configured via a build matrix:

```yaml
jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ ubuntu-latest, windows-latest, macos-latest ]
``` 

In this example I've named the &quot;build&quot; job `build`, which is an arbitrary value and can be changed to anything a user wants.

### Create build artifacts

A build artifact is downloadable output which can be created and collected on each CI run. It can be anything from a single file to an entire folder full of binaries. In the case of a .NET Core NuGet library it is a very useful feature to create a super early version of a NuGet package as soon as a build has finished: 

[![GitHub Action Build Artifacts](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-build-artifacts.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-build-artifacts.png)

In combination with pull request triggers this is a super handy way of giving OSS contributors and OSS maintainers an easy way of downloading and testing a NuGet package as part of a PR.

It is also a nice way of letting users download and consume a &quot;super early semi official&quot; NuGet package which came from the project's official CI pipeline when someone is in absolute desperate need of applying a fix before an official release or pre-release has been created.

In GitHub a NuGet artifact can be easily created by first running the `dotnet pack` command as part of an earlier build step and subsequently using the `upload-artifact@v2` action to upload the newly created `*.nupkg` as an artifact:

```yaml
jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ ubuntu-latest, windows-latest, macos-latest ]
    steps:
      ...
      ...
      - name: Pack
        if: matrix.os == 'ubuntu-latest'
        run: dotnet pack -v normal -c Release --no-restore --include-symbols --include-source -p:PackageVersion=$GITHUB_RUN_ID src/$PROJECT_NAME/$PROJECT_NAME.*proj
      - name: Upload Artifact
        if: matrix.os == 'ubuntu-latest'
        uses: actions/upload-artifact@v2
        with:
          name: nupkg
          path: ./src/${{ env.PROJECT_NAME }}/bin/Release/*.nupkg
```  

In the example above I'm using the pre-defined `GITHUB_RUN_ID` environment variable to specify the NuGet package version and a custom defined environment variable called `PROJECT_NAME` to specify which .NET Core project to pack and publish as an artifact. This has the benefit that the same GitHub workflow definition can be used across multiple projects with very minimal initial setup.

One might have also noticed that I used a wildcard definition for the project file extension `.*proj`. This has the additional benefit that the `dotnet pack` command will work for all types of .NET Core projects, which are `.vbproj`, `.csproj` and `.fsproj`.

Lastly I had to use the version 2 (`@v2`) of the `upload-artifact` action in order to use wildcard definitions in the artifact's `path` specification. If you run into a &quot;missing file&quot; error when trying to upload an artifact then make sure that you're using the latest version of this action. Before version 2 wildcards were not supported yet.

On another note, the `if: matrix.os == 'ubuntu-latest'` condition as part of the `Pack` and `Upload Artifact` steps has no special purpose except limiting the artifact upload to a single run from the previously defined build matrix. A single artifact upload is sufficient (the NuGet package doesn't change based on the environment where it has been packed) and therefore I simply chose `ubuntu-latest` because Ubuntu happens to be the fastest executing environment and therefore helps to keep the overall build time as low as possible. Windows workers seem to take generally longer to start than macOS or Ubuntu. 

### Push nightly releases to GitHub packages

You might have heard of the term &quot;Nightly Build&quot; before. A nightly build (or what I like to call a bleeding edge pre-release build) is a proper (formal) deployment of a build artifact to a place which makes general consumption almost as intuitive as an official release.

In the context of a NuGet package a &quot;nightly release&quot; is a NuGet library which normally gets pushed to a public NuGet feed which is just like the official [NuGet Gallery](https://www.nuget.org), but not the gallery itself. This is a common pattern amongst .NET Core libraries because developers can configure more than one NuGet feed in their project via a `NuGet.config` file (see the [NuGet.config reference](https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file) for more information) and therefore consume a nightly build package the same way as an official release. Most commonly I've seen self hosted [ProGet](https://inedo.com/proget) feeds or cloud hosted [MyGet](https://www.myget.org) feeds to distribute &quot;nightly builds&quot; alongside the official NuGet gallery. However, GitHub's relatively new [Packages](https://github.com/features/packages) feature makes an attractive alternative.

Setting up a nightly build pipeline to [GitHub packages](https://github.com/features/packages) is fairly easy:

```yaml
jobs:
  build:
    ...
    ...
  prerelease:
    needs: build
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    steps:
      - name: Download Artifact
        uses: actions/download-artifact@v1
        with:
          name: nupkg
      - name: Push to GitHub Feed
        run: |
          for f in ./nupkg/*.nupkg
          do
            curl -vX PUT -u "$GITHUB_USER:$GITHUB_TOKEN" -F package=@$f $GITHUB_FEED
          done
```

Unlike build artifacts nightly releases are not something which one would want to do on every build run. It makes sense to limit the creation of a pre-release/nightly deployment to a trigger which is at least one step closer to an official release than a casual git commit or a random pull request. If one uses [Git flow](https://nvie.com/posts/a-successful-git-branching-model/) or another similar branching strategy then the `develop` branch can be a natural gate keeper for a nightly release:

```yaml
if: github.ref == 'refs/heads/develop'
```

Anything which gets pushed into the `develop` branch is per definition on the road map for the next official release and therefore a good trigger for a nightly build.

I've created a complete separate job called `prerelease` for this purpose alone. Just like the `build` job before, this name is completely random and can be changed to something entirely else. In addition the `prerelease` job should only execute after a successful `build` run:

```yaml
needs: build
```

If I hadn't specified this then GitHub would try to run multiple jobs in parallel which is not desired in this case.

The following two `steps` are fairly self explanatory:

```yaml
steps:
  - name: Download Artifact
    uses: actions/download-artifact@v1
    with:
      name: nupkg
  - name: Push to GitHub Feed
    run: |
      for f in ./nupkg/*.nupkg
      do
        curl -vX PUT -u "$GITHUB_USER:$GITHUB_TOKEN" -F package=@$f $GITHUB_FEED
      done
```

First I'm using the `download-artifact@v1` action to obtain the artifact which has been uploaded under the name `nupkg` by the previous job. Then I use cURL to make a HTTP PUT request directly to GitHub's HTTP API in order to upload the downloaded `*.nupkg` package to the specified feed.

The name of the feed is determined through the `GITHUB_FEED` environment variable (more on this later). The `GITHUB_TOKEN` is a pre-defined environment variable which every GitHub Action has automatically created for. The `GITHUB_USER` variable is another global setting where I've set my GitHub username in one place.

#### GitHub packages issue with NuGet

Now one might wonder why I used a `curl` command to interact with GitHub's HTTP API if I could have used `dotnet nuget push` or `nuget push` instead? The short answer is because both of these CLI commands don't work with GitHub's packages feed today.

*The `dotnet nuget push` command only works if the worker image is set to `windows-latest`, however, because the start-up time of a Windows worker is significantly longer than `ubuntu-latest` I rather trade a little bit of &quot;cURL complexity&quot; for an overall faster CI/CD pipeline. It is a personal choice and a trade off which I'm happy to make in this particular case (more on the benefit of speed later).*

#### GitHub Packages Feed

If everything went to plan then the NuGet packages will get uploaded to the user's or organisation's own GitHub packages feed:

[![GitHub Packages Feed](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-packages-feed.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-packages-feed.png)

The packages are tagged with the `GITHUB_RUN_ID` (unless it was a GitHub release):

[![GitHub package versions](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-package-versions.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-package-versions.png)

This is by design. It makes it very easy to associate a certain package version to a specific nightly run. It also makes it very obvious that a package version is a nightly build and not an official release and it's easy to know when a newer version is available since the `GITHUB_RUN_ID` is an incremental counter.

### GitHub release trigger for official NuGet release

GitHub has a wonderful [concept of releases](https://help.github.com/en/github/administering-a-repository/about-releases), which is an extra layer on top of git tags and which provide a nice UI to create, manage and view a release:

[![GitHub Release](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-view-release.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-view-release.png)

Personally I like to use GitHub releases as a formal and conscious step to create, document and publish a NuGet package. For that reason I've added the `release` option as an additional CI trigger:

```yaml
on:
  push:
  pull_request:
  release:
    types:
      - published
```

A GitHub release can have multiple trigger types such as a draft (e.g. `created`) or an edit (`edited`), a delete (`deleted`) and many more. A deployment should only get kicked off when an actual release has been published and therefore the `published` type has to be specified explicitly.

If a repository doesn't use GitHub releases then one can add normal git tags as a CI trigger instead. Git tag triggers would also invoke for a GitHub release (which uses git tags behind the scene), but they would also run whenever a developer pushes a git tag manually from their client.

In order to kick off a deployment for a GitHub release I created a job called `deploy`:

```yaml
deploy:
  needs: build
  if: github.event_name == 'release'
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v2
    - name: Setup .NET Core
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 3.1.301
    - name: Create Release NuGet package
      run: |
        ...
        ...
        dotnet pack -v normal -c Release --include-symbols --include-source -p:PackageVersion=$VERSION -o nupkg src/$PROJECT_NAME/$PROJECT_NAME.*proj
    - name: Push to GitHub Feed
      run: |
        for f in ./nupkg/*.nupkg
        do
          curl -vX PUT -u "$GITHUB_USER:$GITHUB_TOKEN" -F package=@$f $GITHUB_FEED
        done
    - name: Push to NuGet Feed
      run: dotnet nuget push ./nupkg/*.nupkg --source $NUGET_FEED --skip-duplicate --api-key $NUGET_KEY
```

Similar to the `prerelease` job the `deploy` job also requires the `build` job to finish first and additionally checks if the CI run was triggered by a GitHub release event:

```yaml
deploy:
  needs: build
  if: github.event_name == 'release'
```

The actual `deploy` steps are very similar to everything else we've done so far. First it pulls the latest changes via the `checkout@v2` action, then it installs the .NET Core SDK version `3.1.301` with the help of `setup-dotnet@v1` and finally it runs a `dotnet pack` command with the official release version which is specified via the `VERSION` variable (more on this in the next section).

At last I'm using cURL to push to the GitHub packages feed again and the normal `dotnet nuget push` command to publish the final release to the official NuGet feed too.

### Drive NuGet version from Git Tags

As mentioned above, the final NuGet version is determined through the `VERSION` variable. This variable doesn't exist and has to be created manually. Most .NET Core projects specify the package version in their `*.csproj`/`*.fsproj` file through the `<Version>`, `<VersionSuffix>` or `<PackageVersion>` property (if you don't know the differences check out [Andrew Lock's blog post](https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/) for further information). The downside of this is that the project's version has to be kept manually in sync with the GitHub release or git tag version and that it's mostly just meaningless metadata carried along in the project file which is not required until a new release is being published.

In my opinion a much better approach is to entirely remove all version properties from a .NET Core project file and derive the final package version from the submitted git tag during deployment. This is mostly better because there is never a risk of the NuGet package version being out of sync with the provided git tag version. As a developer you probably know that any manual sync is deemed to fail, so why put that strain on ourselves if we can do without it!

Luckily obtaining the git tag version from within a GitHub action is fairly easy. Assuming that a release is being tagged in the format of `vX.X.X` this bash script will extract the actual version from the `GITHUB_REF` variable:

```yaml
- name: Create Release NuGet package
  run: |
    arrTag=(${GITHUB_REF//\// })
    VERSION="${arrTag[2]}"
    VERSION="${VERSION//v}"
    dotnet pack -v normal -c Release --include-symbols --include-source -p:PackageVersion=$VERSION -o nupkg src/$PROJECT_NAME/$PROJECT_NAME.*proj
```

For example, if I tagged a commit with `v1.2.3`, then the `GITHUB_REF` variable would contain `refs/tags/v1.2.3`.

The code `arrTag=(${GITHUB_REF//\// })` converts all forward slash `/` characters into a whitespace and subsequently splits the `GITHUB_REF` variable by the whitespace character into an array called `arrTag`:

```
arrTag[0]: refs
arrTag[1]: tags
arrTag[2]: v1.2.3
```

The next couple lines grab the version tag from the third array element (second index) and later strip the leading `v` character from the value: 

```
VERSION="${arrTag[2]}"
VERSION="${VERSION//v}"
```

If the git tag didn't include the `v` character (e.g. just `1.2.3`) then the second line can be removed.

In the end the `VERSION` variable holds the correct release version of the imminent deployment and can be used to tag the final NuGet package as part of the `dotnet nuget pack` command:

```
dotnet pack -c Release --include-symbols --include-source -p:PackageVersion=$VERSION -o nupkg src/$PROJECT_NAME/$PROJECT_NAME.*proj
```

This is a very effective way of correctly versioning NuGet releases and keeping them automatically synced with GitHub releases (or manual git tags). It also enforces that a release can only happen when a proper git tag has been created.  

### Speed

Speed is paramount in a good CI/CD pipeline. The longer a single run takes, the more likely it is that multiple triggers will result in long queues of individual CI runs stacking up and therefore preventing developers from getting a fast developer feedback loop.

There's a few things which can be done to speed up a .NET Core NuGet pipeline.

#### Ubuntu over Windows

All jobs use the `ubuntu-latest` worker image except the first `build` job which uses a build matrix of three different OS versions to build and test against all major environments. Ubuntu workers start faster than others and therefore should be preferred over Windows images.

#### Avoid redundant dotnet restores

The `build` job has been optimised to not repeat the `dotnet restore` step unnecessarily by making use of the `--no-restore` and `--no-build` flags where possible:

```yaml
- name: Checkout
  uses: actions/checkout@v2
- name: Setup .NET Core
  uses: actions/setup-dotnet@v1
  with:
    dotnet-version: 3.1.301
- name: Restore
  run: dotnet restore
- name: Build
  run: dotnet build -c Release --no-restore
- name: Test
  run: dotnet test -c Release --no-build
```

#### Avoid redundant NuGet caching

Setting the environment variable `DOTNET_SKIP_FIRST_TIME_EXPERIENCE` to `true` means that we can prevent the .NET CLI from wasting time on redundant package caching.

#### Turn off telemetry

Maybe not a huge gain, but surely turning off the .NET telemetry by setting the `DOTNET_CLI_TELEMETRY_OPTOUT` environment variable to `true` will shave off another few (milli)seconds.

#### Avoid pulling in extra dependencies

Not having to install extra utilities on a worker image means that the CI run doesn't have to waste extra time setting up additional tools. For example, instead of installing the standalone NuGet CLI one can use `dotnet nuget` which comes out of the box when .NET Core is set up as a dependency. Another example is to use `curl` when it already exists instead of pulling in another HTTP client for negligent benefits.

#### Bash over PowerShell

Running `bash` scripts is significantly faster than running PowerShell (`pwsh`), because PowerShell takes longer to load. Luckily all script blocks in GitHub actions are set to `bash` by default unless specified otherwise. Try to avoid using PowerShell scripts if not necessarily required (e.g. using a little bit more `bash` instead of fancier `pwsh` Cmdlets for establishing the NuGet release version as seen above).

Overall these micro improvements mean that an incoming pull request takes approximately two minutes to successfully build against the entire build matrix and produce a NuGet artifact as well:

[![GitHub Action Build time for Giraffe](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-action-run-time.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-action-run-time.png)

## Environment Variables

Now that the majority of the GitHub Action has been explained in detail we're just missing one final piece to complete the puzzle. Throughout this blog post I've been frequently referring to various environment variables which the script assumes to exist so that it is not specifically tied to a single project but rather applicable to many.

Some of those environment variables get created automatically by GitHub itself, but others have to be set up manually, which can be done either at the job level or globally at the top of the file:

```yaml
env:
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true

  # Project name to pack and publish
  PROJECT_NAME: Giraffe

  # GitHub Packages Feed settings
  GITHUB_FEED: https://nuget.pkg.github.com/giraffe-fsharp/
  GITHUB_USER: dustinmoris
  GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}

  # Official NuGet Feed settings
  NUGET_FEED: https://api.nuget.org/v3/index.json
  NUGET_KEY: ${{ secrets.NUGET_KEY }}
```

The `PROJECT_NAME` variable is set to the .NET Core project name which is meant to get packed and published by this script. The script assumes that the solution follows the widely adopted folder structure of:

```
src/
+-- MyProject/
    +-- MyProject.csproj
tests/
+-- MyProject.Tests/
    +-- MyProject.Tests.csproj
MySolution.sln
```

In this example the `PROJECT_NAME` variable must be set to `MyProject`. Don't worry, if the solution contains more helper projects. Everything will get built and tested by the script. The `build` job executes a `dotnet build` and `dotnet test` from the root level of the repository, which means that it will pick up all projects from the repo's `.sln` file.

The `GITHUB_FEED` variable is a convenience pointer to the user's or organisation's GitHub feed. The `GITHUB_USER` variable requires a username of someone who has sufficient permissions to publish to the feed. As mentioned before, the `GITHUB_TOKEN` variable is auto-created by GitHub, which is why it's being assigned directly from `${{ secrets.GITHUB_TOKEN }}`.

Finally the `NUGET_FEED` variable points towards the official NuGet gallery and the `NUGET_KEY` variable receives a private secret which must be set up manually either at the project or organisation level of the GitHub repository:

[![GitHub Secrets](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-secrets.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/github-secrets.png)

I configured the `NUGET_KEY` as an organisation wide secret, which means I don't have to set up any additional secrets for each repository any more.

If this rings your security bells then you are not entirely wrong. If you wonder if a malicious attacker could modify the GitHub workflow yaml file as part of a pull request and force bad code into the public domain then let me assure you that this won't be possible. GitHub makes it very clear that **it doesn't pass** secrets to workflows which were triggered by a pull request from a fork:

> Secrets are not passed to workflows that are triggered by a pull request from a fork

The value for the `NUGET_KEY` secret has to be generated on [www.nuget.org](https://www.nuget.org):

[![NuGet API Key](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/nuget-key.png)](https://storage.googleapis.com/dusted-codes/images/blog-posts/2020-06-28/nuget-key.png)

## The End Result

The end result is a pretty elaborate CI/CD pipeline. All commits and pull requests will trigger a new run against a Linux, macOS and Windows environment and build and test the code across all these platforms. Additionally each build will produce a NuGet package as an artifact which can be downloaded and added to a local NuGet feed for test purposes or urgent matters. When features and bug fixes get eventually merged into the `develop` branch it will kick off a nightly build and publish an early pre-release version into the organisation's own GitHub packages feed. Finally when a release is being published an official NuGet package will be produced and not only pushed to the Github packages feed, but also to the official NuGet gallery. Nightly build packages are tagged with the workflow run ID and official packages with the associated git tag version. Everything is optimised for speed.

### Four stages of a release

In total this set up supports the four stages of a release:

#### 1. YOLO Release

Builds create a NuGet artifact, which is what I like to call the YOLO (you only live once) release. It's meant for super early testing or people who just don't give a damn :).

#### 2. Nightly Builds

Merges into the `develop` branch will create an official nightly build which gets pushed into GitHub packages. It's still bleeding edge, but slightly more mature than the YOLO release.

#### 3. Official pre-release packages

The next step in the release pipeline is an official pre-release package. It is basically the same as an official release package except that the git version tag follows the pre-release convention (e.g. `v2.0.0-beta-23`). Those packages are being released into the public NuGet gallery, but clearly marked as not a proper release candidate.

#### 4. Official release packages

Finally the last release stage is a proper stable release. Same as before, it's triggered by a git tag, but this time with a stable release version (e.g. `v2.0.0`).

### Workflow YAML

The final GitHub Action file where all the individual pieces are put together can be viewed on the official [Giraffe repository](https://github.com/giraffe-fsharp/Giraffe/blob/master/.github/workflows/build.yml).

Anyone is free to copy the `build.yml` file, apply custom changes to the environment variables at the top of the file and deploy it to their own .NET Core NuGet repository (it should just work)!

If the link above doesn't work or cannot tbe viewed then the entire `build.yml` file can also be seen in the script below:

###### build.yml

```yaml
name: .NET Core
on:
  push:
  pull_request:
  release:
    types:
      - published
env:
  # Stop wasting time caching packages
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
  # Disable sending usage data to Microsoft
  DOTNET_CLI_TELEMETRY_OPTOUT: true
  # Project name to pack and publish
  PROJECT_NAME: Giraffe
  # GitHub Packages Feed settings
  GITHUB_FEED: https://nuget.pkg.github.com/giraffe-fsharp/
  GITHUB_USER: dustinmoris
  GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
  # Official NuGet Feed settings
  NUGET_FEED: https://api.nuget.org/v3/index.json
  NUGET_KEY: ${{ secrets.NUGET_KEY }}
jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ ubuntu-latest, windows-latest, macos-latest ]
    steps:
      - name: Checkout
        uses: actions/checkout@v2
      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.1.301
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build -c Release --no-restore
      - name: Test
        run: dotnet test -c Release
      - name: Pack
        if: matrix.os == 'ubuntu-latest'
        run: dotnet pack -v normal -c Release --no-restore --include-symbols --include-source -p:PackageVersion=$GITHUB_RUN_ID src/$PROJECT_NAME/$PROJECT_NAME.*proj
      - name: Upload Artifact
        if: matrix.os == 'ubuntu-latest'
        uses: actions/upload-artifact@v2
        with:
          name: nupkg
          path: ./src/${{ env.PROJECT_NAME }}/bin/Release/*.nupkg
  prerelease:
    needs: build
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    steps:
      - name: Download Artifact
        uses: actions/download-artifact@v1
        with:
          name: nupkg
      - name: Push to GitHub Feed
        run: |
          for f in ./nupkg/*.nupkg
          do
            curl -vX PUT -u "$GITHUB_USER:$GITHUB_TOKEN" -F package=@$f $GITHUB_FEED
          done
  deploy:
    needs: build
    if: github.event_name == 'release'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET Core
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 3.1.301
      - name: Create Release NuGet package
        run: |
          arrTag=(${GITHUB_REF//\// })
          VERSION="${arrTag[2]}"
          echo Version: $VERSION
          VERSION="${VERSION//v}"
          echo Clean Version: $VERSION
          dotnet pack -v normal -c Release --include-symbols --include-source -p:PackageVersion=$VERSION -o nupkg src/$PROJECT_NAME/$PROJECT_NAME.*proj
      - name: Push to GitHub Feed
        run: |
          for f in ./nupkg/*.nupkg
          do
            curl -vX PUT -u "$GITHUB_USER:$GITHUB_TOKEN" -F package=@$f $GITHUB_FEED
          done
      - name: Push to NuGet Feed
        run: dotnet nuget push ./nupkg/*.nupkg --source $NUGET_FEED --skip-duplicate --api-key $NUGET_KEY
```

