$ErrorActionPreference='Stop'
$taskSwApi='C:/Program Files/SOLIDWORKS Corp/SOLIDWORKS (2)/api/redist'
$taskSwRefs=@("$taskSwApi/SolidWorks.Interop.sldworks.dll","$taskSwApi/SolidWorks.Interop.swconst.dll")
foreach($r in $taskSwRefs){Add-Type -Path $r}
Add-Type -Path './sw_restart.cs' -ReferencedAssemblies $taskSwRefs
[SWRestart]::Run((Get-Location).Path)
