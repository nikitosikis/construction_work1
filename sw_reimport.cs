using System;using System.IO;using System.Runtime.InteropServices;using SolidWorks.Interop.sldworks;using SolidWorks.Interop.swconst;
public static class SWReimport{public static void Run(){
var a=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");int e=0,w=0;
bool prev=a.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swMultiCAD_Enable3DInterconnect);
int prevBrep=a.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportUseBrep);
int prevKnit=a.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportNeutral_KnitOption);
try{
a.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swMultiCAD_Enable3DInterconnect,false);
a.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportUseBrep,1);
a.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportNeutral_KnitOption,0);
string input=@"C:\Users\jopajack\Desktop\кудм\16_surface_import.step";File.Copy(@"C:\Users\jopajack\Desktop\кудм\16. Корпус фиксатора.stp",input,true);
var imp=a.GetImportFileData(input);var m=(ModelDoc2)a.LoadFile4(input,"r",imp,ref e);if(m==null)throw new Exception("Import "+e);
var p=(PartDoc)m;Console.WriteLine("DIAG "+p.ImportDiagnosis(true,false,true,0));
foreach(Body2 b in (object[])p.GetBodies2(-1,false)){Console.WriteLine("BODY "+b.GetType()+" FACES "+b.GetFaceCount());var dg=b.Diagnose();Console.WriteLine("GAPS "+dg.GetGapsCount());}
m.Extension.SaveAs(@"C:\Users\jopajack\Desktop\кудм\16_Corpus_surface_repair.SLDPRT",0,1,null,ref e,ref w);Console.WriteLine("SAVE "+e);
}finally{a.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swMultiCAD_Enable3DInterconnect,prev);a.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportUseBrep,prevBrep);a.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportNeutral_KnitOption,prevKnit);}
}}
