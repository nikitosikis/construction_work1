using System;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

public static class SolidworksBuild {
 public static void Run(string root) {
  var app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");
  app.Visible=true;
  int err=0,warn=0;
  var input=Path.Combine(root,"16. Корпус фиксатора.stp");
  var partPath=Path.Combine(root,"16_Corpus.SLDPRT");
  ModelDoc2 part;
  if(File.Exists(partPath)) part=app.OpenDoc6(partPath,1,1,"",ref err,ref warn);
  else {
   var import=app.GetImportFileData(input);
   part=app.LoadFile4(input,"r",import,ref err);
   if(part==null) throw new Exception("STEP import failed: "+err);
   if(!part.Extension.SaveAs(partPath,0,1,null,ref err,ref warn)) throw new Exception("Part save failed: "+err);
  }
  Console.WriteLine("PART "+part.GetTitle());
  foreach(var n in (object[])part.GetModelViewNames()) Console.WriteLine("MODELVIEW "+n);
  var props=part.Extension.get_CustomPropertyManager("");
  props.Add3("Description",30,"Корпус фиксатора",2);
  props.Add3("Material",30,"Сталь 10 ГОСТ 1050-2013",2);
  props.Add3("Обозначение",30,"16",2);
  var model=(ModelDoc2)app.NewDocument(@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\templates\gost-part drw.drwdot",0,0.420,0.297);
  if(model==null) throw new Exception("Cannot create GOST drawing");
  var drawing=(DrawingDoc)model;
  var sheet=(Sheet)drawing.GetCurrentSheet();
  bool setup=drawing.SetupSheet5(sheet.GetName(),12,12,1,1,true,@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\lang\english\sheetformat\a3 - gost_sh1_land.slddrt",0.420,0.297,"Default",false);
  Console.WriteLine("SHEET "+setup);
  var front=drawing.CreateDrawViewFromModelView3(partPath,"*Спереди",0.120,0.210,0);
  if(front==null) throw new Exception("Cannot create front view");
  front.UseSheetScale=1;
  var end=drawing.CreateDrawViewFromModelView3(partPath,"*Справа",0.265,0.210,0);
  var iso=drawing.CreateDrawViewFromModelView3(partPath,"*Изометрия",0.345,0.140,0);
  iso.UseSheetScale=0;iso.ScaleDecimal=0.5;
  model.ClearSelection2(true);
  drawing.ActivateView(front.GetName2());
  var line=model.SketchManager.CreateLine(-0.060,0,0,0.060,0,0);
  model.ClearSelection2(true);
  line.Select4(false,null);
  var section=drawing.CreateSectionViewAt5(0.120,0.105,0,"А",0,null,0);
  Console.WriteLine("SECTION "+(section!=null));
  model.ClearSelection2(true);
  model.EditRebuild3();
  model.ViewZoomtofit2();
  if(!model.Extension.SaveAs(Path.Combine(root,"16_Corpus_draft.SLDDRW"),0,1,null,ref err,ref warn)) throw new Exception("Drawing save failed "+err);
  Console.WriteLine("SAVED DRAWING");
  model.Extension.SaveAs(Path.Combine(root,"16_Corpus_draft.pdf"),0,1,null,ref err,ref warn);
 }
}
