using System;
using System.IO;
using System.Runtime.InteropServices;
using KompasAPI7;
using Kompas6Constants;
public class DrawingJob {
 public static void Run(string root) {
  IApplication app=(IApplication)Activator.CreateInstance(Type.GetTypeFromProgID("Kompas.Application.7"));
  app.Visible=true;
  var doc=(IKompasDocument3D)app.Documents.Open(Path.Combine(root,"part16_model.m3d"),false,true);
  Console.WriteLine("MODEL "+doc.TopPart.Name+" MATERIAL "+doc.TopPart.Material+" MASS "+doc.TopPart.Mass);
  double x0=0,y0=0,z0=0,x1=0,y1=0,z1=0;
  doc.TopPart.GetGabarit(true,false,out x0,out y0,out z0,out x1,out y1,out z1);
  Console.WriteLine("BOUNDS {0} {1} {2} / {3} {4} {5}",x0,y0,z0,x1,y1,z1);
  var d=(IKompasDocument2D)app.Documents.Add(DocumentTypeEnum.ksDocumentDrawing,true);
  var sh=d.LayoutSheets[0]; sh.Format.Format=ksDocumentFormatEnum.ksFormatA3; sh.Format.VerticalOrientation=false; sh.Update();
  var views=d.ViewsAndLayersManager.Views;
  views.AddStandartViews(doc.PathName,"Спереди",new int[]{1,3,5,7},90,210,1,35,30);
  var iso=(IAssociationView)views.get_View(4);iso.ProjectionLink=false;iso.X=330;iso.Y=165;iso.Scale=0.5;iso.Update();
  for(int i=0;i<views.Count;i++){
   IView v=views.get_View(i);
   Console.WriteLine("VIEW {0} {1} at {2},{3} scale {4}",i,v.Name,v.X,v.Y,v.Scale);
   var c=(IDrawingContainer)v;
   Console.WriteLine("LINES {0} CIRCLES {1} ARCS {2}",c.LineSegments.Count,c.Circles.Count,c.Arcs.Count);
  }
  var front=(IAssociationView)views.get_View(1);
  front.Current=true;var cut=((ISymbols2DContainer)front).CutLines.Add();cut.X1=-10;cut.Y1=0;cut.X2=101;cut.Y2=0;cut.Points=new double[]{-10,0,101,0};cut.ArrowPos=true;cut.Text.Str="А";Console.WriteLine("CUT "+cut.Update());
  var sec=(IAssociationView)views.get_View(2);
  sec.Current=true;
  sec.HiddenLines=true;sec.HiddenLinesVisible=true;sec.HiddenLinesStyle=4;sec.Update();  Console.WriteLine("REBUILD "+sec.Rebuild()+" objects="+sec.ObjectCount+" projection="+sec.ProjectionName);
  Console.WriteLine("FULL REBUILD "+((IKompasDocument2D1)d).CompleteRebuildDocument()); using(var w=new StreamWriter(Path.Combine(root,"geometry.txt"))){
   for(int i=1;i<views.Count;i++){
    var v=views.get_View(i);var c=(IDrawingContainer)v;w.WriteLine("VIEW "+i+" "+v.Name+" "+v.X+" "+v.Y);
    for(int j=0;j<c.LineSegments.Count;j++){var l=c.LineSegments.get_LineSegment(j);w.WriteLine("L {0:F4} {1:F4} {2:F4} {3:F4} S{4}",l.X1,l.Y1,l.X2,l.Y2,l.Style);}
    for(int j=0;j<c.Circles.Count;j++){var a=c.Circles.get_Circle(j);w.WriteLine("C {0:F4} {1:F4} R{2:F4}",a.Xc,a.Yc,a.Radius);}
    for(int j=0;j<c.Arcs.Count;j++){var a=c.Arcs.get_Arc(j);w.WriteLine("A {0:F4} {1:F4} R{2:F4} {3:F4} {4:F4}",a.Xc,a.Yc,a.Radius,a.Angle1,a.Angle2);}
   }
  }
  d.SaveAs(Path.Combine(root,"layout.cdw"));
  var conv=app.get_Converter(@"C:\Program Files\ASCON\KOMPAS-3D v25 Study\Bin\Pdf2d.dll");
  Console.WriteLine("PDF "+conv.Convert(d.PathName,Path.Combine(root,"layout.pdf"),1,false));
  d.Close(DocumentCloseOptions.kdDoNotSaveChanges);
  doc.Close(DocumentCloseOptions.kdDoNotSaveChanges);
  app.Quit();
 }
}



















