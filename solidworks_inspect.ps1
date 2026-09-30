param([string[]]$Names)
Add-Type -Path 'C:/Program Files/SOLIDWORKS Corp/SOLIDWORKS (2)/api/redist/SolidWorks.Interop.sldworks.dll'
Add-Type -Path 'C:/Program Files/SOLIDWORKS Corp/SOLIDWORKS (2)/api/redist/SolidWorks.Interop.swconst.dll'
foreach($n in $Names){
 $t=('SolidWorks.Interop.sldworks.'+$n) -as [type]
 if(!$t){$t=('SolidWorks.Interop.swconst.'+$n) -as [type]}
 "TYPE $n"
 if($t.IsEnum){[Enum]::GetValues($t)|ForEach-Object {"$_ $([int]$_)"};continue}
 $t.GetMethods() | Where-Object {$_.Name -match 'LoadFile4|OpenDoc6|NewDocument|SaveAs|SetupSheet|CreateDrawViewFromModelView|CreateSectionViewAt5|CreateUnfoldedViewAt3|SetScale|InsertNote|AddDimension|GetVisibleEntities|SelectEntity|SetPosition|CreateLine|InsertSketch'} | ForEach-Object {$_.ToString()}
}
