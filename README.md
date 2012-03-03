# DNS-O-Matic Client .NET #

DNS-O-Matic Client .NET is an open source dynamic dns hostname updater client for [DNS-O-Matic](http://www.dnsomatic.com).  It is written in C# and is built on the .NET Framework 4.

DNS-O-Matic is a service that enables a user to install a single updater client on their computer, and update hostnames at one or more of the [dozens](http://www.dnsomatic.com/wiki/supportedservices) of dynamic dns providers that DNS-O-Matic supports.

**How Do I Run DNS-O-Matic Client .NET?**

DNS-O-Matic Client .NET can be run in two different configurations:

1. As a console application.  DNS-O-Matic credentials and hostnames are supplied in the application configuration file, and the console application can be run manually, or via a scheduled task.

2. As a Windows service.  The service can be installed from the command line (a batch script is provided), or via a Windows Setup.exe application.  When installed via the command line, DNS-O-Matic credentials and hostnames are supplied in the application configuration file.  When installed via the Setup.exe, the installation wizard prompts for DNS-O-Matic credentials and hostnames.

**How Often Does the DNS-O-Matic Client .NET Update My IP Addresses?**

When run as a Windows Service, the updater checks if the IP Address has changed every 5 minutes.  If the IP Address has changed since the last check, the client will updated the IP Address with the DNS-O-Matic service, otherwise no update is sent to DNS-O-Matic.

----------


**Why Use the DNS-O-Matic Service?**

1. A user can pick an updater client that they trust, or with features that they need, that will work even when they swith dynamic dns providers.
2. A user can update hostnames at different dynamic dns providers, without having to install multiple clients on the same computer.
3. DNS-O-Matic user can enable the service to send them email notification when IP Addresses change, or when there are errors updating their hostnames.  This is something that many updater clients or dynamic dns services do not provide.

**Why another DNS-O-Matic Client?**

1. There is no official DNS-O-Matic client.
2. Other existing clients are primarily closed source.
3. There was no other open source client written in for the .NET Framework.

**Why an Open Source DNS-O-Matic Client?**

1. Developers have the ability to extend or improve functionality as needed.
2. Individuals and IT departments need to be able to trust the code that is running on their machines.  Without an official client the best alternative is an open source solution.