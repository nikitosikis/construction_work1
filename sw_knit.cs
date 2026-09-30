using System;using System.Runtime.InteropServices;using SolidWorks.Interop.sldworks;
public static class SWKnit{public static void Run(){
var a=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");var m=(ModelDoc2)a.ActiveDoc;var p=(PartDoc)m;int e=0,w=0;
var b=(Body2)((object[])p.GetBodies2(-1,false))[0];m.ClearSelection2(true);
var sd=(SelectData)((SelectionMgr)m.SelectionManager).CreateSelectData();sd.Mark=1;Console.WriteLine("SELECT "+b.Select2(false,sd));
var f=m.FeatureManager.InsertSewRefSurface(false,true,false,0.00001,0.00001);Console.WriteLine("KNIT "+(f!=null));m.EditRebuild3();
foreach(Body2 bb in (object[])p.GetBodies2(-1,false)) Console.WriteLine("BODY "+bb.GetType()+" FACES "+bb.GetFaceCount());
if(f!=null){m.Extension.SaveAs(@"C:\Users\jopajack\Desktop\кудм\16_Corpus_solid.SLDPRT",0,1,null,ref e,ref w);Console.WriteLine("SAVE "+e);}
}}

