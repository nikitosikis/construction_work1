using System;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

public static class SWFill {
 public static void Run(string root) {
  var app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");
  int err=0,warn=0;
  var path=Path.Combine(root,"16_Corpus_surface_repair.SLDPRT");
  var m=app.OpenDoc6(path,1,0,"",ref err,ref warn);
  if(m==null) throw new Exception("Open failed: "+err);
  Console.WriteLine("Opened "+m.GetTitle());
  var p=(PartDoc)m;
  var bodies=(object[])p.GetBodies2(-1,false);
  Console.WriteLine("Bodies "+bodies.Length);
  var body=(Body2)bodies[0];
  var dg=body.Diagnose();
  Console.WriteLine("Gaps "+dg.GetGapsCount());
  for(int k=0;k<dg.GetGapsCount();k++) {
   m.ClearSelection2(true);
   var sel=(SelectionMgr)m.SelectionManager;
   var data=sel.CreateSelectData();
   data.Mark=1;
   foreach(CoEdge ce in (object[])dg.GetCoEdgesAtGap(k)) {
    var edge=(Edge)ce.GetEdge();
    Console.WriteLine("Select "+((Entity)edge).Select4(true,data));
   }
   Console.WriteLine("Selected "+sel.GetSelectedObjectCount2(-1));
   var fill=m.FeatureManager.InsertFillSurface2(3,
       (int)swFeatureFillSurfaceOptions_e.swOptimizeSurface,
       data,(int)swContactType_e.swContact,null,null);
   Console.WriteLine("Fill "+(fill==null?"NULL":fill.Name));
  }
  m.EditRebuild3();
  foreach(Body2 b in (object[])p.GetBodies2(-1,false))
   Console.WriteLine("After body type "+b.GetType()+" faces "+((object[])b.GetFaces()).Length+" gaps "+b.Diagnose().GetGapsCount());
  var output=Path.Combine(root,"16_Corpus_filled.SLDPRT");
  Console.WriteLine("Save "+m.Extension.SaveAs(output,0,1,null,ref err,ref warn)+" err "+err);
 }
}
