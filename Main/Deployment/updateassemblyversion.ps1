param([string] $Major, [string] $Minor )
Write-Output "Here is the Major Parameter $($Major)";
Write-Output "Here is the Minor Parameter $($Minor)";
$DayOfYear = (Get-Date).DayofYear
$Year = (Get-Date -Format yy)
$Hour = (Get-Date).Hour
$Minute = (Get-Date).Minute
$Second = (Get-Date).Second
$SecondsInHour = [int]$Hour * 3600;
$SecondsInMinute = [int]$Minute * 60;
$HalfSecondsInDay = [int](($SecondsInHour + $SecondsInMinute + [int]$Second) / 2)
$Version = "$($Major).$($Minor).$($DayOfYear)$($Year).$($HalfSecondsInDay)";
Write-Output "Applying version $($Version)";
Get-ChildItem -Include assemblyinfo.cs, assemblyinfo.vb -Recurse | 
    ForEach-Object {
        $_.IsReadOnly = $false
		 (Get-Content -Path $_) -replace '(?<=Assembly(?:File)?Version\(")[^"]*(?="\))',$Version |
            Set-Content -Path $_
    }
