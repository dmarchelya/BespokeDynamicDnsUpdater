# Bespoke Dynamic DNS Updater #

Bespoke Dynamic DNS Updater is an open source dynamic dns hostname updater client for [DNS-O-Matic](http://www.dnsomatic.com).  It runs on Windows and is written in C# and is built on the .NET Framework 4.

DNS-O-Matic is a service that enables you to install a single updater client on your computer, and update hostnames at one or more of the [dozens](http://www.dnsomatic.com/wiki/supportedservices) of dynamic dns providers that DNS-O-Matic supports.

A Windows Installer is available on GitHub ([Github Downloads](https://github.com/dmarchelya/BespokeDynamicDnsUpdater/downloads)).  This is the best place to download the installer without having to compile source code.

**How Do I Run Bespoke Dynamic DNS Updater?**

Bespoke Dynamic DNS Updater can be run in two different configurations:

1. As a console application.  DNS-O-Matic credentials and hostnames are supplied in the application configuration file, and the console application can be run manually, or via a scheduled task.

2. As a Windows service.  The service can be installed from the command line (a batch script is provided), or via a Windows Setup.exe application.  When installed via the command line, DNS-O-Matic credentials and hostnames are supplied in the application configuration file.  When installed via the Setup.exe, the installation wizard prompts for DNS-O-Matic credentials and hostnames.

**How Often Does the Bespoke Dynamic DNS Updater Update My IP Addresses?**

When run as a Windows Service, the updater checks if the IP Address has changed every 5 minutes.  If the IP Address has changed since the last check, the client will update the IP Address with the DNS-O-Matic service, otherwise no update is sent to DNS-O-Matic.

----------


**Why Use the DNS-O-Matic Service?**

1. You can pick an updater client that you trust, or with features that you need, and it will still work even when you swith dynamic dns providers.
2. You can update hostnames at different dynamic dns providers, without having to install multiple clients on the same computer.
3. As a user of the DNS-O-Matic service, you can configure DNS-O-Matic to send you email notifications when your IP Addresses change, or when there are errors updating your hostnames. This is something that many updater clients or dynamic dns services do not provide.

**Why another DNS-O-Matic Client?**

1. There is no official DNS-O-Matic client.
2. Other existing clients are primarily closed source.
3. There was no other open source client written in for the .NET Framework.

**Why an Open Source DNS-O-Matic Client?**

1. Developers have the ability to extend or improve functionality as needed.
2. Individuals and IT departments need to be able to trust the code that is running on their machines.  Without an official client the best alternative is an open source solution.

----------
**License**

Copyright 2012 Bespoke Industries
http://www.bespokeindustries.com/

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.