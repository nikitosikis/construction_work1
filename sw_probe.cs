using System;using System.Runtime.InteropServices;using SolidWorks.Interop.sldworks;
public static class SWProbe{public static void Run(){
var a=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");
var m=(ModelDoc2)a.ActiveDoc;Console.WriteLine("ACTIVE "+m.GetTitle());
var d=(DrawingDoc)m;var v=(View)d.GetFirstView();
while(v!=null){Console.WriteLine("VIEW "+v.GetName2()+" scale "+v.ScaleDecimal+" pos "+string.Join(",",(double[])v.Position));if(v.GetNextView()!=null||v.ReferencedDocument!=null){try{Console.WriteLine("OUTLINE "+string.Join(",",(double[])v.GetOutline()));Console.WriteLine("MATRIX "+string.Join(",",(double[])v.ModelToViewTransform.ArrayData));}catch{}}
v=(View)v.GetNextView();}
int e=0,w=0;var p=(ModelDoc2)a.OpenDoc6(@"C:\Users\jopajack\Desktop\кудм\16_Corpus.SLDPRT",1,1,"",ref e,ref w);var bodies=(object[])((PartDoc)p).GetBodies2(-1,false);foreach(Body2 b in bodies){Console.WriteLine("BODY "+b.GetType()+" "+b.Name+" "+string.Join(",",(double[])b.GetBodyBox()));}
}}
