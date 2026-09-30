$ErrorActionPreference='Stop'
$taskSwApi='C:/Program Files/SOLIDWORKS Corp/SOLIDWORKS (2)/api/redist'
$taskSwRefs=@("$taskSwApi/SolidWorks.Interop.sldworks.dll","$taskSwApi/SolidWorks.Interop.swconst.dll")
foreach($r in $taskSwRefs){Add-Type -Path $r}
Add-Type -Path './sw_fill.cs' -ReferencedAssemblies $taskSwRefs
[SWFill]::Run((Get-Location).Path)
