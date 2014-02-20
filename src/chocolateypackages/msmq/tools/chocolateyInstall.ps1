$packageName = 'NServiceBus.MSMQ'

function InstallDismFeatures($features) {

    if (DismRebootRequired) {
        throw "A reboot is required prior to installing this package"
    }

    $featureNames = [string]::Join(" ", @($features | % { "/FeatureName:$_"}))
    $cmd = "dism.exe /Online  /Enable-Feature /NoRestart /Quiet $featureNames"
    Write-Host ("Executing: {0}" -f $cmd)
    Invoke-Expression $cmd
    CheckDismForUndesirables
    
    if (DismRebootRequired) {
        Write-Host "A reboot is required to complete the MSMQ installation"
    }
    else {
        StartMSMQ    
    }
} 

function CheckDismForUndesirables() {
    $undesirables = @("MSMQ-Triggers", "MSMQ-ADIntegration", "MSMQ-HTTP", "MSMQ-Multicast", "MSMQ-DCOMProxy")
    $msmqFeatures = dism.exe /Online /Get-Features /Format:Table | Select-String "^MSMQ" -List 
    $removeThese = @()
    
    foreach ($msmqFeature in $msmqFeatures) {
        
        $key = $msmqFeature.ToString().Split("|")[0].Trim()    
        $value = $msmqFeature.ToString().Split("|")[1].Trim()
        if ($undesirables -contains $key) {
            if (($value -eq "Enabled") -or ($value -eq "Enable Pending")) {
                $removeThese += $key                 
            }
        }
    }
    
    if ($removeThese.Count -gt 0 ) {
         $featureNames = [string]::Join(" ", @($removeThese | % { "/FeatureName:$_"}))
         
         Write-Warning "Undesirable MSMQ feature(s) detected. Please remove using this command: `r`n`t dism.exe /Online /Disable-Feature $featureNames `r`nNote: This command is case sensitive"  
    } 
}

function StartMSMQ () {

    $msmqService = Get-Service -Name "MSMQ" -ErrorAction SilentlyContinue
    if (!$msmqService)  {
        throw "MSMQ service not found"
    }   

    switch ($msmqService.Status) {
       "Stopped" { $msmqService.Start()  }
       "StopPending" { $msmqService.Start() }
     }
}


function DismRebootRequired() {
    $info = dism.exe /Online /Get-Features /Format:Table | Select-String "Disable Pending", "Enable Pending" -List 
    return ($info.Count -gt 0)
}

try {
    $osVersion = [Environment]::OSVersion.Version
    $ver = "{0}.{1}" -f $osVersion.Major, $osVersion.Minor

    switch ($ver) 
    {
        { @("6.3", "6.2") -contains $_ }  {
             # Win 8.x and Win 2012
             InstallDismFeatures(@("MSMQ-Server"))
        }
        
        "6.1" {  
              # Windows 7 and Windows 2008 R2
             InstallDismFeatures(@("MSMQ-Server", "MSMQ-Container"))
         }
        "6.0" { 
            #TBD -  Windows Server 2008 and Vista
            $osInfo = Get-WmiObject Win32_OperatingSystem
            if ($osInfo.ProductType -eq 1) {
                throw "Unsupported Operating System"
            }
            else {
                throw "Unsupported Operating System"
            }
        }
        default {
            # XP and Win2003 
            throw "Unsupported Operating System"
        }
    }
    Write-ChocolateySuccess "$packageName"
}
catch {
     Write-ChocolateyFailure "$packageName" "$($_.Exception.Message)"
}