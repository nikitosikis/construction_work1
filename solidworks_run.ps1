$ErrorActionPreference='Stop'
$taskSwApi='C:/Program Files/SOLIDWORKS Corp/SOLIDWORKS (2)/api/redist'
$taskSwRefs=@("$taskSwApi/SolidWorks.Interop.sldworks.dll","$taskSwApi/SolidWorks.Interop.swconst.dll")
foreach($r in $taskSwRefs){Add-Type -Path $r}
Add-Type -Path './solidworks_build.cs' -ReferencedAssemblies $taskSwRefs
if($env:SW_COMPILE_ONLY -ne '1'){[SolidworksBuild]::Run((Get-Location).Path)}
