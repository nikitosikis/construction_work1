using System;using System.Runtime.InteropServices;using SolidWorks.Interop.sldworks;
public static class SWHeal{public static void Run(){
var a=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");int e=0,w=0;
var m=(ModelDoc2)a.OpenDoc6(@"C:\Users\jopajack\Desktop\кудм\16_Corpus.SLDPRT",1,1,"",ref e,ref w);a.ActivateDoc3(m.GetTitle(),false,0,ref e);
var p=(PartDoc)m;Console.WriteLine("DIAGNOSTICS "+p.ImportDiagnosis(true,true,true,0));m.EditRebuild3();
foreach(Body2 b in (object[])p.GetBodies2(-1,false))Console.WriteLine("BODY TYPE "+b.GetType()+" FACES "+b.GetFaceCount());
m.Save3(1,ref e,ref w);Console.WriteLine("SAVED "+e);
}}
