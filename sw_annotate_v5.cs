using System;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
public static class SWAnnotate {
 static ISldWorks app;static ModelDoc2 m;static DrawingDoc d;
 static bool Pick(double x,double y,bool add=false,string type="EDGE"){return m.Extension.SelectByID2("",type,x/1000,y/1000,0,add,0,null,0);}
 static void Dim(double x1,double y1,double x2,double y2,double x,double y,bool vertical=false,string prefix=""){m.ClearSelection2(true);Console.WriteLine("SELECT "+Pick(x1,y1)+" "+Pick(x2,y2,true));var v=(DisplayDimension)(vertical?m.AddVerticalDimension2(x/1000,y/1000,0):m.AddHorizontalDimension2(x/1000,y/1000,0));if(v==null){Console.WriteLine("DIM FAIL");return;}if(prefix!="")v.SetText(1,prefix);v.SetPrecision2(0,-1,-1,-1);Console.WriteLine("DIM "+((Dimension)v.GetDimension2(0)).SystemValue);m.ClearSelection2(true);}
 static void Note(string t,double x,double y){m.ClearSelection2(true);var n=(Note)m.InsertNote(t);var a=(Annotation)n.GetAnnotation();var f=(TextFormat)a.GetTextFormat(0);f.CharHeight=.0035;a.SetTextFormat(0,false,f);a.SetPosition(x/1000,y/1000,0);m.ClearSelection2(true);}
 public static void Run(string root,string mode){app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");m=(ModelDoc2)app.ActiveDoc;d=(DrawingDoc)m;int er=0,wa=0;View v=(View)d.GetFirstView();var sheet=v;v=(View)v.GetNextView();while(v!=null){if(v.Type==2){var s=(DrSection)v.GetSection();s.SetDisplayOnlySurfaceCut(true);}v=(View)v.GetNextView();}d.ActivateSheet(((Sheet)d.GetCurrentSheet()).GetName());
  m.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayOrigins,false);Dim(85,245,110,238,97.5,260);Dim(110,238,146,231,128,260);Dim(146,231,176,232,161,260);Dim(85,245,176,232,130,272);Dim(100,132.5,100,67.5,65,100,true,"<MOD-DIAM>");
  Annotation a=(Annotation)sheet.GetFirstAnnotation3();while(a!=null){if(a.GetType()==6){var n=(Note)a.GetSpecificAnnotation();Console.WriteLine("NOTE "+n.GetText()+" LINK "+n.PropertyLinkedText+" POS "+string.Join(";",(double[])a.GetPosition()));}a=(Annotation)a.GetNext3();}
  Note("Корпус фиксатора",302,39);Note("Сталь 10 ГОСТ 1050-2013",300,18);m.ClearSelection2(true);m.EditRebuild3();m.ViewZoomtofit2();m.Save3(1,ref er,ref wa);Console.WriteLine("PDF "+m.Extension.SaveAs(Path.Combine(root,"КУДМ.724282.016_Корпус_фиксатора_по_образцу_v5.pdf"),0,1,null,ref er,ref wa));m.Save3(1,ref er,ref wa);
 }
}
