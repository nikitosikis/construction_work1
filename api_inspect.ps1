param([string[]]$Names)
Add-Type -Path "./.sdk/Common/Kompas6Constants.dll"
Add-Type -Path "./.sdk/Common/Kompas6Constants3D.dll"
Add-Type -Path "./.sdk/Common/KompasAPI7.dll"
foreach($name in $Names){
 $t=('KompasAPI7.'+$name) -as [type]
 if(!$t){$t=('Kompas6Constants.'+$name) -as [type]}
 "TYPE $name"
 if($t.IsEnum){[Enum]::GetValues($t) | ForEach-Object {"$_ = $([int]$_)"};continue}
 $t.GetProperties() | ForEach-Object { "$($_.PropertyType.Name) $($_.Name)" }
 $t.GetMethods() | Where-Object {$_.Name -notmatch '^(get_|set_)'} | ForEach-Object {$_.ToString()}
}

