using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
public static class SWDrawingV3 {
 static ISldWorks app;static ModelDoc2 m;static DrawingDoc d;static MathUtility mu;static View front,section,trans,end,detail;
 static double[] Project(View v,double[] p){return (double[])((MathPoint)((MathPoint)mu.CreatePoint(p)).MultiplyTransform(v.ModelToViewTransform)).ArrayData;}
 static void Origin(View v,double x,double y){m.EditRebuild3();double[] t=(double[])v.ModelToViewTransform.ArrayData;double[] p=(double[])v.Position;p[0]+=x/1000-t[9];p[1]+=y/1000-t[10];v.Position=p;m.EditRebuild3();}
 static void Report(View v){Console.WriteLine("VIEW "+v.GetName2()+" MATRIX "+string.Join(";",(double[])v.ModelToViewTransform.ArrayData)+" OUTLINE "+string.Join(";",(double[])v.GetOutline()));}
 static void Vertices(View v){var a=(object[])v.GetVisibleEntities2(null,2);if(a==null)return;foreach(Vertex b in a){double[] p=(double[])b.GetPoint(),q=Project(v,p);Console.WriteLine("VERTEX "+v.GetName2()+" "+string.Join(";",p)+" -> "+q[0]*1000+";"+q[1]*1000);}}
 static View Sec(View v,double x1,double y1,double x2,double y2,double px,double py,string label,int option){m.ClearSelection2(true);d.ActivateView(v.GetName2());double[] vp=(double[])v.Position;var s=m.SketchManager.CreateLine(x1/1000-vp[0],y1/1000-vp[1],0,x2/1000-vp[0],y2/1000-vp[1],0);m.ClearSelection2(true);s.Select4(false,null);var a=d.CreateSectionViewAt5(px/1000,py/1000,0,label,option,null,0);if(a==null)throw new Exception("Section "+label+" failed");a.UseSheetScale=1;m.ClearSelection2(true);return a;}
 static void Save(string root){m.EditRebuild3();m.ViewZoomtofit2();int e=0,w=0;string name="КУДМ.724282.016_Корпус_фиксатора_по_образцу_v5";bool ok=m.Extension.SaveAs(Path.Combine(root,name+".SLDDRW"),0,1,null,ref e,ref w);Console.WriteLine("DRAWING "+ok+" ERR "+e);if(!ok)throw new Exception("Save drawing");ok=m.Extension.SaveAs(Path.Combine(root,name+".pdf"),0,1,null,ref e,ref w);Console.WriteLine("PDF "+ok+" ERR "+e);m.Save3(1,ref e,ref w);}
 public static void Run(string root,string mode){app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");mu=(MathUtility)app.GetMathUtility();int er=0,wa=0;
  if(mode=="inspect"){m=(ModelDoc2)app.ActiveDoc;d=(DrawingDoc)m;View v=(View)d.GetFirstView();v=(View)v.GetNextView();while(v!=null){Report(v);Vertices(v);v=(View)v.GetNextView();}return;}
  var prev=(ModelDoc2)app.GetOpenDocumentByName(Path.Combine(root,"КУДМ.724282.016_Корпус_фиксатора_по_образцу_v5.SLDDRW"));if(prev!=null){if(prev.GetSaveFlag())throw new Exception("New drawing has unsaved user edits");app.CloseDoc(prev.GetTitle());}string part=Path.Combine(root,"16_Корпус_для_чертежа_v3.SLDPRT");var pm=app.OpenDoc6(part,1,1,"",ref er,ref wa);m=(ModelDoc2)app.NewDocument(@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\templates\gost-part drw.drwdot",0,.420,.297);d=(DrawingDoc)m;var sh=(Sheet)d.GetCurrentSheet();d.SetupSheet5(sh.GetName(),12,12,1,1,true,@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\lang\english\sheetformat\a3 - gost_sh1_land.slddrt",.420,.297,"Default",false);sh=(Sheet)d.GetCurrentSheet();sh.SetScale(1,1,true,true);
  front=d.CreateDrawViewFromModelView3(part,"*Спереди",.130,.100,0);front.UseSheetScale=1;Origin(front,85,100);Report(front);
  section=Sec(front,78,100,184,100,130,217,"А",4);Origin(section,85,215);Report(section);
  trans=Sec(front,102,58,102,142,263,102,"Б",5);Origin(trans,260,105);Report(trans);
  end=d.CreateDrawViewFromModelView3(part,"*Справа",.367,.105,0);end.UseSheetScale=1;Origin(end,365,105);Report(end);
  d.ActivateView(section.GetName2());double[] sp=(double[])section.Position;var circle=m.SketchManager.CreateCircleByRadius(.162-sp[0],.235-sp[1],0,.021);m.ClearSelection2(true);circle.Select4(false,null);detail=(View)d.CreateDetailViewAt4(.308,.232,0,0,2,1,"В",1,false,false,false,0);Console.WriteLine("DETAIL "+(detail!=null));if(detail!=null)Report(detail);d.ActivateSheet(sh.GetName());m.ClearSelection2(true);Save(root);
 }
}



