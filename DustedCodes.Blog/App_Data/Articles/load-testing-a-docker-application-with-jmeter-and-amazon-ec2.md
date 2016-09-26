<!--
    Published: 2016-09-25 19:07
    Author: Dustin Moris Gorski
    Title: Load testing a Docker application with JMeter and Amazon EC2
	Tags: jmeter docker aws cloud
-->
A couple of months ago I blogged about [JMeter load testing from a continuous integration build](https://dusted.codes/jmeter-load-testing-from-a-continuous-integration-build) and gave a few tips and tricks on how to get the most out of automated load tests. In this blog post I would like to go a bit more hands on and show how to manually load test a Docker application with JMeter and the help of [Amazon web services]().

I will be launching two [Amazon EC2 instances]() to conduct a single load test. One instance will host the [Docker]() application and the other the [JMeter load test tool](). The benefit of this setup is that Docker and JMeter have their own dedicated resources and I can load test the application in isolation. It also allows me to quickly tear down the Docker instance and vertically scale it up or down and measure the impact of it.

## Launching a Docker VM

First I will create a new EC2 instance to host the Docker container. The easiest way of doing this is to go through the online wizard and select the Ubuntu 14.04 base image and paste the following bash script into the user data field to automatically pre-install the Docker service during the launch up:

<pre><code>#!/bin/bash

# Install Docker
sudo apt-get update
sudo apt-get install apt-transport-https ca-certificates
sudo apt-key adv --keyserver hkp://p80.pool.sks-keyservers.net:80 --recv-keys 58118E89F3A912897C070ADBF76221572C52609D
sudo bash -c 'echo "deb https://apt.dockerproject.org/repo ubuntu-trusty main" >> /etc/apt/sources.list.d/docker.list'
sudo apt-get update
sudo apt-get install linux-image-extra-$(uname -r) -y
sudo apt-get install apparmor
sudo apt-get install docker-engine -y
sudo service docker start

# Run [your] Docker container
sudo docker run -p 8080:8888 dustinmoris/docker-demo-nancy:0.2.0
</code></pre>

At the end of the script I added a `docker run` command to auto start the container which runs my application under test. Replace this with your own container when launching the instance.

![](aws-launch-ec2-advanced-details.png)

Simply click through the rest of the wizard and a few minutes later you should have a running Ubuntu VM with Docker and your application container running inside it.

Make sure to map a port from the container to the host and open this port for inbound traffic. For example if I launched my container with the flag `-p 8080:8888` then I need to add the port 8080 to the inbound rules of the security group associated with this VM.

## Launching a JMeter VM

Next I follow through the wizard a second time to create an instance with JMeter installed on it. Just as before I use the Ubuntu 14.04 base image and the user data field to add a bit of bash commands to get everything pre-installed during launch time:

<pre><code>#!/bin/bash

# Install Java 7
sudo apt-get install openjdk-7-jre-headless -y

# Install JMeter
wget -c http://ftp.ps.pl/pub/apache//jmeter/binaries/apache-jmeter-3.0.tgz -O jmeter.tgz
tar -xf jmeter.tgz</code></pre>

Don't forget to open the default SSH port 22 in the security group of the JMeter instance.

Only a short time later I should have two successfully created VMs with Docker and JMeter being fully operational and ready to run some load tests.

## Running JMeter tests

Running load tests from the JMeter instance is fairly straight forward from here. I am going to remote connect to the JMeter instance, copy a JMeter test file onto the machine and then launch the JMeter command line tool to run the load tests remotely. Afterwards I will download the JMeter results file and analyse the test data in my local JMeter GUI.

### Download PuTTY SSH client tools

From here on I will describe the steps required to remote connect from a Windows desktop, which might be slightly different than what you'd have to do to connect from a Unix based system. However, most things are very similar and and it should not be too difficult to follow the steps from a Mac or Linux as well.

In order to SSH from Windows to a Linux VM you will have to download the [PuTTY SSH client](http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html). Whilst you are on the download page you might also download the [PSCP]() and [PuTTYgen]() tools. One will be needed to securely transfer files between your Windows desktop and the Linux VM and the other to convert the SSH key from the `.pem` file format to `.ppk`.

### Convert SSH key from .pem to .ppk

Before we can do anything else we have to convert the SSH key which has been associated with the VM from the `.pem` file format into `.ppk`.

1. Open the `puttygen.exe`
2. Click on the &quot;Load&quot; button and locate the `.pem` SSH key file
3. Select the SSH-2 RSA option
4. Click on &quot;Save private key&quot; and save the key as a `.ppk` file

Once completed you can use the new key file with the PuTTY SSH client to remote connect to the EC2 instance.

### Remote connect to the EC2 instance

1. Open the `putty.exe`
2. Type the public IP of the EC2 instance into host name field
3. Prepend `ubuntu@` to the IP address in the host name field (this is not necessarily required, but speeds up the login process later on)
4. On the left hand side expand the &quot;SSH&quot; tree node and then select &quot;Auth&quot;
5. Browse for the `.ppk` private key file
6. Go back to &quot;Session&quot; tree node
7. Type in a memorable name into the &quot;Saved Sessions&quot; field and click &quot;Save&quot;
8. Finally click on the &quot;Open&quot; button and a terminal window will open and it should be able to successfully connect to the VM

At this point you should be remote connected to the JMeter EC2 instance and able to run any commands on the machine.

### Copy JMeter test file to the VM

1. 