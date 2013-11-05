PlatformInstaller
=================


# Features

* For v1 we assume that the installer runs on a devbox

Use case : On a new machine, next next next

* Pull down all relevant NServiceBus packages from NuGet and unpack them (how valuable is this?? can we drop it?)
* Do a install of the NServiceBus.Installer to prepare the machine (for now)
* Pull down the Samples from github (eg: https://github.com/Particular/NServiceBus.Azure.Samples/tree/4.2.0)
* Pull down (from Chocolatey) and install ServiceControl + the endpointplugin(s) from nuget
* Pull down (from Chocolatey) and install ServiceInsight from nuget
* Pull down (from Chocolatey) and install ServiceMatrix from nuget
* Pull down (from Chocolatey) and install ServicePulse from nuget
* Show the release notes from the different pieces: https://github.com/Particular/NServiceBus/releases/tag/4.1.0

Actions for the current Installer

* Remove binaries
* Remove samples
* Only keep the powershell dll and the custom actions


Discuss with Udi:

* Move to a model where we show what commands/actions to perform to install to teach the users what to do

