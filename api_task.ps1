param([string]$Source='sw_rebuild.cs',[string]$Class='SWRebuild',[string]$Mode='build')
$ErrorActionPreference='Stop'
$taskApi='C:/Program Files/SOLIDWORKS Corp/SOLIDWORKS (2)/api/redist'
$taskRefs=@("$taskApi/SolidWorks.Interop.sldworks.dll","$taskApi/SolidWorks.Interop.swconst.dll")
foreach($taskRef in $taskRefs){Add-Type -Path $taskRef}
Add-Type -Path $Source -ReferencedAssemblies $taskRefs
$taskType=$Class -as [type]
try{$taskType.GetMethod('Run').Invoke($null,@((Get-Location).Path,$Mode))}catch{if($_.Exception.InnerException){Write-Output $_.Exception.InnerException.ToString()};throw}
