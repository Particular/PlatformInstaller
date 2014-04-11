# Passed from Octopus  

$requiredvariables = "accesskey", "secretkey", "bucketname"
$requiredvariables | % {
    if (!(Test-Path "variable:$_")) {
        throw "Variable $_ has not been set in Octopus config" 
    }
}

#content folder in nuget package contains files to upload
push-location .\content


# Folder to use inside the S3 bucket
$folder = "PlatformInstaller"

Initialize-AWSDefaults -AccessKey $accesskey -SecretKey $secretkey -Region US-East-1
Write-Host "Deleting existing files on S3 ..."
$deleted = Get-S3Object -BucketName $bucketname -KeyPrefix "$folder/" | Remove-S3Object -BucketName $bucketname  -Force
$deleted | % { Write-Host ("`t Deleted {0}" -F  $_.Key) } 

Write-Host "Finished deletion"
Write-Host "Uploading new files to S3 ..."

$files = Get-ChildItem -Path "." -Exclude "*.pdb", "*.xml",  | ? { $_.PSIsContainer -eq $false }
foreach ($file in $files) { 
    $fileName =  $file.Name
    $key =  "{0}/{1}" -f $folder, $fileName

    switch ($file.Extension.ToLower()) {
        ".application" { Write-S3Object -BucketName $bucketname -File "$fileName" -Key "$key" -ContentType 'application/x-ms-application' -PublicReadOnly }
        ".manifest" { Write-S3Object -BucketName $bucketname -File "$fileName" -Key "$key" -ContentType 'application/x-ms-manifest' -PublicReadOnly }
        default  { Write-S3Object -BucketName $bucketname -File "$fileName" -Key "$key" -ContentType "application/octet-stream" -PublicReadOnly }
    }
}

# Verify upload 
$webfiles = Get-S3Object -BucketName $bucketname -KeyPrefix "$folder/" 
foreach ($file in $files) {
    $fileName =  $file.Name
    $key =  "{0}/{1}" -f $folder, $fileName
    $webfile = $webfiles | ? Key -EQ $key 
    if (!$webfile) { 
        throw "Failed to verify upload of $file - file not found"
    }
    if ($webfile.Size -ne $file.Length) {
        throw "Failed to verify upload of $file - size does not match" 
    }
    Write-Host ("`t Uploaded {0} as {1}" -f $fileName, $key ) 
}

Write-Host "Upload complete"
