rem this batch file should be copied to the directory where the exe resides and run from that location
set servicedir=%CD%
rem cd %servicedir%
cd C:\Windows\Microsoft.NET\Framework\v4.0.30319
installutil %servicedir%\DnsOMaticClient.Net.WindowsService.exe
net start DnsOMaticClientWindowsService
pause