using System;using System.IO;using System.Runtime.InteropServices;using SolidWorks.Interop.sldworks;
public static class SWLastDetails{
 static ModelDoc2 m;static DrawingDoc d;
 static void L(double x,double y,double a,double b){d.ActivateView("");m.SketchManager.AddToDB=true;var s=m.SketchManager.CreateLine(x/1000,y/1000,0,a/1000,b/1000,0);s.Color=0;s.Width=0;m.SketchManager.AddToDB=false;}
 static void Arrow(double x,double y,double dx,double dy){double q=Math.Sqrt(dx*dx+dy*dy);dx=dx/q*2.6;dy=dy/q*2.6;L(x,y,x+dx-dy*.22,y+dy+dx*.22);L(x,y,x+dx+dy*.22,y+dy-dx*.22);}
 static void N(string t,double x,double y,double size=3.5,double angle=0){m.ClearSelection2(true);d.ActivateView("");var n=(Note)m.InsertNote(t);n.Angle=angle;var a=(Annotation)n.GetAnnotation();a.Color=0;var f=(TextFormat)a.GetTextFormat(0);f.CharHeight=size/1000;a.SetTextFormat(0,false,f);a.SetPosition(x/1000,y/1000,0);m.ClearSelection2(true);}
 public static void Run(string root,string mode){var app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");m=(ModelDoc2)app.ActiveDoc;d=(DrawingDoc)m;
  L(95.5215498,187.5,85,237);L(85,215,85,241);L(85,237,73,237);L(85,215,73,215);L(75,215,75,237);Arrow(75,215,0,1);Arrow(75,237,0,-1);N("22",71,226,3.5,Math.PI/2);
  L(182,134,194,134);Arrow(182,134,1,0);N("Д",187,143,5);N("Д",359,162,5);
  m.ClearSelection2(true);m.EditRebuild3();m.ViewZoomtofit2();int e=0,w=0;m.Save3(1,ref e,ref w);Console.WriteLine("PDF "+m.Extension.SaveAs(Path.Combine(root,"КУДМ.724282.016_Корпус_фиксатора_по_образцу.pdf"),0,1,null,ref e,ref w));m.Save3(1,ref e,ref w);
 }
}
