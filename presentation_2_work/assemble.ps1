param([string]$Reference,[string]$Body,[string]$Work)
$ErrorActionPreference='Stop'
$shell=Join-Path $Work 'template_copy.pptx'
Copy-Item -LiteralPath $Reference -Destination $shell -Force
$data=Get-Content -LiteralPath (Join-Path $Work 'notes.json') -Raw -Encoding UTF8 | ConvertFrom-Json
$app=$null
$deck=$null
try {
  $app=New-Object -ComObject PowerPoint.Application
  $deck=$app.Presentations.Open($shell,0,0,0)
  if ($deck.Slides.Count -ne 13) { throw 'Unexpected reference deck slide count' }
  for ($i=12; $i -ge 2; $i--) { $deck.Slides.Item($i).Delete() }
  $n=$deck.Slides.InsertFromFile($Body,1)
  if ($n -ne 11 -or $deck.Slides.Count -ne 13) { throw "Inserted $n content slides" }
  $cover=$deck.Slides.Item(1)
  $title=$cover.Shapes.Item('Title 4').TextFrame.TextRange
  $title.Text="Расчёт подшипникового узла`rс газовой смазкой`rдля высокоскоростного шпинделя"
  $cover.Shapes.Item('TextBox 1').TextFrame.TextRange.Text='Аэростатическая схема и расчётный пример'
  $bodyBg=Join-Path $Work 'itmo_body.jpg'
  for($i=2;$i -le 12;$i++){
    $s=$deck.Slides.Item($i)
    $s.FollowMasterBackground=0
    $s.Background.Fill.UserPicture($bodyBg)
  }
  $fig=$deck.Slides.Item(3).Shapes.AddPicture((Join-Path $Work 'aeroC.png'),0,-1,(457*.75),(132*.75),(456*.75),(294*.75))
  $fig.Name='spindle-sourced-diagram'
  $fig.AlternativeText='Продольный разрез аэростатического шпинделя. Chen и др., 2018, рис. 1.'
  $fig=$deck.Slides.Item(6).Shapes.AddPicture((Join-Path $Work 'journal_section.png'),0,-1,(583*.75),(111*.75),(280*.75),(330*.75))
  $fig.Name='radial-bearing-sourced-section'
  $fig.AlternativeText='Поперечное сечение радиального газового подшипника. Lubricants, 2023, рис. 1в.'
  foreach($i in @(1,13)) {
    $s=$deck.Slides.Item($i)
    $s.NotesPage.Shapes.Placeholders.Item(2).TextFrame.TextRange.Text=$data."$i"
  }
  $deck.SaveAs((Join-Path $Work 'candidate.pptx'),24)
  Write-Output ('Saved assembled {0}-slide candidate' -f $deck.Slides.Count)
} finally {
  if($null -ne $deck){$deck.Close()}
  if($null -ne $app){[System.Runtime.InteropServices.Marshal]::ReleaseComObject($app) | Out-Null}
}
