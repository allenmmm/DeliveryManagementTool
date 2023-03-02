param(	[string] $targetFile = "unspecified",
	[string] $xmlNode = "unspecified",
	[string] $xmlName = "unspecified",
	[string] $xmlValue = "unspecified")
write-output "Script Running"
$xml = New-Object XML
$xml.Load($targetFile)
$element = $xml.SelectNodes("$xmlNode[@name='$xmlName']")
$element.SetAttribute("value", $xmlValue)
$xml.Save($targetFile)