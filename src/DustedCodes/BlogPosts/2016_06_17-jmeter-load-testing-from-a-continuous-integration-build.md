<!--
    Tags: jmeter load-testing
-->

# JMeter Load Testing from a continuous integration build

In the last two weeks I have been doing a series of load tests and the tool I've been using was [Apache JMeter](http://jmeter.apache.org/). JMeter is an open source, cross platform load testing tool written in Java. Unlike the [Apache benchmarking tool](https://httpd.apache.org/docs/2.4/programs/ab.html) (aka AB) [JMeter has been specifically designed to load test](http://stackoverflow.com/a/10264501/1693158) static and dynamic web services with high accuracy. Running those tests manually in the beginning of a new project might be fun, but if you have a high traffic web service which needs periodic testing then integrating those tests into your CI system might be a good idea.

In this short blog post I will share a few lessons I have learned along the way...

## Creating a JMeter test plan (.jmx file)

The first step would be to create a test plan which will represent the type of load you would like to test against. This should match the load you anticipate in your production system as much as possible. If you have multiple instances running behind a load balancer then you can test the load of one instance only. Also the maximum load your server can handle will depend on the size of the instance under test. If your production and test instances don't match up then you might have difficulties to draw meaningful conclusions after a test run.

Creating a JMeter test plan should be the only time you use the GUI to execute your tests. The GUI consumes many resources which will have an impact on your test run and could even limit you on the maximum load you'll be able to throw at your service. If you have never created a JMeter test plan then check out this great [introduction tutorial which will walk you through the basic architecture of a JMeter test plan](http://jmeter.apache.org/usermanual/build-web-test-plan.html).

Note that [listeners](http://jmeter.apache.org/usermanual/component_reference.html#listeners) and [assertions](http://jmeter.apache.org/usermanual/component_reference.html#assertions) are very resource expensive elements and should be avoided in your test plan wherever possible. After a test run you can open the results file with any of the provided listeners and analyse the data retrospectively. For example if you don't log the HTTP response data in the test results file, which is generally recommended to minimise resource consumption, then you might need to keep a [response assertion](http://jmeter.apache.org/usermanual/component_reference.html#Response_Assertion) enabled to check if a HTTP request was successful or not. In contrast you can probably disable all [duration assertions](http://jmeter.apache.org/usermanual/component_reference.html#Duration_Assertion), because you will most likely log the latency of each request and therefore be able validate this metric afterwards.

## Allocating enough memory in your JVM

If you run very large load tests then you might experience a Java `OutOfMemory` exception. This is not uncommon considering that the default memory allocation is only 512MB large. A good rule of thumb is to set the Java heap size to ~80% of your available memory. If you intend to set the heap size to a value greater than 2GB then you will have to install the [64-bit version of the JRE](http://www.oracle.com/technetwork/java/javase/downloads/jre8-downloads-2133155.html).

You can change the heap size allocation by opening the `jmeter.bat` file inside the `/bin` folder and edit the following line:

<pre><code>set HEAP=-Xms512m -Xmx512m</code></pre>

In this example I set the Java heap size to a maximum value of 4GB:

<pre><code>set HEAP=-Xms512m -Xmx4096m</code></pre>

## Configuring JMeter properties

Another good way of optimising your load tests is to configure JMeter to only save the data which is required for later analysis. This again will reduce the overall resource consumption and allow you to run much larger tests.

Inside the `/bin` folder there is a `jmeter.properties` file, which holds all the default properties for JMeter. You should not modify this file directly, because otherwise you would lose your custom settings when upgrading to a newer version. Instead it's recommended to create a `user.properties` file in the same folder (or open the pre-existing empty one) and save all custom settings in this place. Configuration settings inside the `user.properties` file have higher precedence over `jmeter.properties`.

Search for the values beginning with `jmeter.save.saveservice.` inside the `jmeter.properties` file. These properties specify the results file configuration and let you customise what data will be stored during a test run. Go through each of those lines and decide whether you want to keep the default value or change it. In the latter case copy the original line and save it with your custom value inside the `user.properties` file.

For instance if you are mostly interested in the average throughput, various latencies and the error rate then you could tailor the JMeter properties to those values:

<pre><code>jmeter.save.saveservice.output_format=csv
jmeter.save.saveservice.assertion_results_failure_message=false
jmeter.save.saveservice.assertion_results=none
jmeter.save.saveservice.data_type=false
jmeter.save.saveservice.label=true
jmeter.save.saveservice.response_code=true
jmeter.save.saveservice.response_data=false
jmeter.save.saveservice.response_data.on_error=false
jmeter.save.saveservice.response_message=false
jmeter.save.saveservice.successful=true
jmeter.save.saveservice.thread_name=true
jmeter.save.saveservice.time=true
jmeter.save.saveservice.subresults=false
jmeter.save.saveservice.assertions=false
jmeter.save.saveservice.latency=true
jmeter.save.saveservice.connect_time=false
jmeter.save.saveservice.samplerData=false
jmeter.save.saveservice.responseHeaders=false
jmeter.save.saveservice.requestHeaders=false
jmeter.save.saveservice.encoding=false
jmeter.save.saveservice.bytes=true
jmeter.save.saveservice.url=false
jmeter.save.saveservice.filename=false
jmeter.save.saveservice.hostname=true
jmeter.save.saveservice.thread_counts=true
jmeter.save.saveservice.sample_count=true
jmeter.save.saveservice.idle_time=true
jmeter.save.saveservice.timestamp_format=ms
jmeter.save.saveservice.default_delimiter=,
jmeter.save.saveservice.print_field_names=true</code></pre>

One setting which is worth pointing out is the following:

<pre><code>jmeter.save.saveservice.output_format=csv</code></pre>

This lets you specify the format of the results file. By default it is set to `csv` and I would recommend you to keep it this way, unless you want to store the response data in your results. In that case you'll have to change it to `xml`.

However, storing results in CSV has a few advantages:

- JMeter is quicker in saving data in CSV than in XML
- It might be easier to analyse data with 3<sup>rd</sup> party tools such as MS Excel
- Even though it's possible with XML, it is much easier to merge multiple results files from a distributed test run in CSV format

## Running JMeter from the command line

Runnning JMeter from the command line is not only the recommended way of running your tests, but also the best to execute them from an automated build step.

To run in non GUI mode you have to invoke the `jmeter.bat` file with the `-n` switch. Using the `-t` parameter lets you specify a test plan and the `-l` parameter the location of the results file:

<pre><code>jmeter -n -t MyTestPlan.jmx -l Results.jtl</code></pre>

When the test run completes JMeter will exit with code 0. You can check for that code when invoking the load test from an automated step. The results file will contain all information for subsequent test analysis. When JMeter encounters an error you can inspect the `jmeter.log` file for more information.

## Combining multiple results files from a distributed test run

Sometimes the load you want to run is so big that you have to split the execution across two or more JMeter instances, because one instance is unable to simulate the load alone. In this case you can either use the [JMeter client to control multiple remote instances](http://jmeter.apache.org/usermanual/remote-test.html) or configure your CI system do it yourself. Either way it will work the exact same way. The JMeter test plan will have to be executed on each individual instance and will produce a separeate results file for you. When all tests finished you'll have to collect the results files and merge them into one single file for analysis.

If you're using the CSV format you can simply append the contents from one file to the end of another. Merging multiple CSV results files is usually as simple as a few copy paste bash commands. However, if you were using the XML format then you will have to append the data in the right place which will require [a bit of additional scripting](http://stackoverflow.com/a/35783873/1693158).

## Analysing the results file (.jtl)

You can always open a JMeter results file with one of the existing listeners from the JMeter GUI. If your results were saved in CSV format then you can also open them in a spreadsheet or almost any other statistical software.

If you want to analyse and evaluate your results from an automated build then you will have to do some more scripting. It mostly depends what type of analysis you would like to perform, but in most cases you will be able to extract the relevant information with only a few simple commands.

For that purpose you could use PowerShell as the native language on Windows machines or if you are looking for something more portable then you could write a small C# or F# library, which can be either invoked directly from tools like [CAKE](http://cakebuild.net/), [FAKE](http://fsharp.github.io/FAKE/), the [FSI](https://msdn.microsoft.com/en-us/visualfsharpdocs/conceptual/fsharp-interactive-%5Bfsi.exe%5D-reference) or from a console application build on Mono. Just to give you a taster of how simple that can be you an check out [this small F# application](https://github.com/dustinmoris/JMeterResultsAnalyser).

## BlazeMeter for everyone

If everything from above sounds like too much work and you look for a more hassle free, extremely well functioning and feature rich solution then I can highly recommend you to sign up at [BlazeMeter](https://www.blazemeter.com/) and let your tests run in the cloud by some real experts.