$ErrorActionPreference='Stop'
$refs=@('Kompas6Constants','Kompas6Constants3D','KompasAPI7','Kompas6API5','Kompas6API2D5COM','Kompas6API3D5COM') | ForEach-Object { (Resolve-Path ".sdk/Common/$_.dll").Path }
foreach($r in $refs){Add-Type -Path $r}
Add-Type -Path './drawing.cs' -ReferencedAssemblies $refs
[DrawingJob]::Run((Get-Location).Path)
