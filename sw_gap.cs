using System;using System.Runtime.InteropServices;using SolidWorks.Interop.sldworks;
public static class SWGap {public static void Run(){
var a=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");var m=(ModelDoc2)a.ActiveDoc;var p=(PartDoc)m;Console.WriteLine(m.GetTitle());
foreach(Body2 b in (object[])p.GetBodies2(-1,false)){var dg=b.Diagnose();for(int k=0;k<dg.GetGapsCount();k++){Console.WriteLine("GAP "+k+" EDGES "+dg.GetCoEdgesCountAtGap(k));foreach(CoEdge c in (object[])dg.GetCoEdgesAtGap(k)){var ed=(Edge)c.GetEdge();var v1=(Vertex)ed.GetStartVertex();var v2=(Vertex)ed.GetEndVertex();Console.WriteLine("EDGE "+string.Join(",",(double[])v1.GetPoint())+" -> "+string.Join(",",(double[])v2.GetPoint()));var cp=ed.GetCurveParams3();var curve=(Curve)ed.GetCurve();for(int j=0;j<=4;j++)Console.WriteLine("PT "+string.Join(",",(double[])curve.Evaluate2(cp.UMinValue+(cp.UMaxValue-cp.UMinValue)*j/4,0)));}}}
}}


