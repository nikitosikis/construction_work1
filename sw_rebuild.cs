using System;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
public static class SWRebuild {
 static ISldWorks app; static ModelDoc2 m; static Sketch sk; static SketchSegment baseAxis;
 static double[] P(double x,double y,double z){var u=(MathUtility)app.GetMathUtility();return (double[])((MathPoint)((MathPoint)u.CreatePoint(new double[]{x/1000,y/1000,z/1000})).MultiplyTransform(sk.ModelToSketchTransform)).ArrayData;}
 static SketchSegment L(double x,double y,double z,double a,double b,double c){double[] p=P(x,y,z),q=P(a,b,c);return m.SketchManager.CreateLine(p[0],p[1],0,q[0],q[1],0);}
 static void Arc(double cx,double cy,double cz,double x,double y,double z,double a,double b,double c,short dir){double[] p=P(cx,cy,cz),q=P(x,y,z),r=P(a,b,c);m.SketchManager.CreateArc(p[0],p[1],0,q[0],q[1],0,r[0],r[1],0,dir);}
 static Feature Plane(int index){Feature f=(Feature)m.FirstFeature();int j=0;while(f!=null){if(f.GetTypeName2()=="RefPlane"&&j++==index)return f;f=(Feature)f.GetNextFeature();}throw new Exception("No plane");}
 static void Begin(int plane){m.ClearSelection2(true);Plane(plane).Select2(false,0);m.SketchManager.InsertSketch(true);sk=(Sketch)m.GetActiveSketch2();m.SketchManager.AddToDB=true;Console.WriteLine("SKETCH PLANE "+plane+" TRANSFORM "+string.Join(",",(double[])sk.ModelToSketchTransform.ArrayData));}
 static Feature End(){m.SketchManager.AddToDB=false;var f=(Feature)sk;m.SketchManager.InsertSketch(true);m.ClearSelection2(true);f.Select2(false,0);return f;}
 static void Cut(string name,bool dir,double depth,int start=0,double offset=0,bool both=false){End();var f=m.FeatureManager.FeatureCut3(!both,false,dir,both?1:0,both?1:0,depth/1000,depth/1000,false,false,false,false,0,0,false,false,false,false,false,true,true,false,false,false,start,offset/1000,false);if(f==null)throw new Exception("Cut failed "+name);f.Name=name;m.EditRebuild3();}
 static void Circle(double x,double y,double z,double r){var p=P(x,y,z);m.SketchManager.CreateCircleByRadius(p[0],p[1],0,r/1000);}
 static void RevolveCut(SketchSegment axis,string name){End();var sd=((SelectionMgr)m.SelectionManager).CreateSelectData();sd.Mark=4;Console.WriteLine("HOLE AXIS SELECT "+axis.Select4(true,sd));var f=m.FeatureManager.FeatureRevolveCut2(2*Math.PI,false,0,0,0,true,true,false,false,false);if(f==null)throw new Exception("Hole failed "+name);f.Name=name;m.EditRebuild3();}
 static void Radials(double angle){m.ClearSelection2(true);Plane(0).Select2(false,0);var sd=((SelectionMgr)m.SelectionManager).CreateSelectData();sd.Mark=1;baseAxis.Select4(true,sd);var rp=(RefPlane)m.FeatureManager.InsertRefPlane(16,angle*Math.PI/180,4,0,0,0);if(rp==null)throw new Exception("Radial plane");var pf=(Feature)rp;pf.Name="Плоскость отверстий "+angle;
  for(int sign=-1;sign<=1;sign+=2){m.ClearSelection2(true);pf.Select2(false,0);m.SketchManager.InsertSketch(true);sk=(Sketch)m.GetActiveSketch2();m.SketchManager.AddToDB=true;var mu=(MathUtility)app.GetMathUtility();var inv=sk.ModelToSketchTransform.Inverse();var v=(double[])((MathPoint)((MathPoint)mu.CreatePoint(new double[]{0,1,0})).MultiplyTransform(inv)).ArrayData;double uy=v[1]*sign,uz=v[2]*sign;Console.WriteLine("RADIAL AXIS "+uy+","+uz);double[,] a={{17,26},{18.7,26},{18.7,29.5},{20.5,29.5},{20.5,40},{17,40},{17,26}};for(int i=0;i<6;i++)L(a[i,0],a[i,1]*uy,a[i,1]*uz,a[i+1,0],a[i+1,1]*uy,a[i+1,1]*uz);var ax=L(17,20*uy,20*uz,17,45*uy,45*uz);ax.ConstructionGeometry=true;RevolveCut(ax,"Радиальное отверстие M4 "+angle+" "+sign);}
 }
 static void Report(){var p=(PartDoc)m;var bs=(object[])p.GetBodies2(-1,false);if(bs==null)throw new Exception("No bodies");foreach(Body2 b in bs){Console.WriteLine("BODY "+b.GetType()+" FACES "+b.GetFaceCount()+" GAPS "+b.Diagnose().GetGapsCount());Console.WriteLine("BOX "+string.Join(",",(double[])b.GetBodyBox()));}var mp=m.Extension.CreateMassProperty();Console.WriteLine("VOLUME "+mp.Volume);}
 public static void Run(string root,string mode){try{app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");}catch{app=(ISldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));}app.Visible=true;Console.WriteLine("SW "+app.RevisionNumber());int e=0,w=0;
  if(mode=="finish"){m=(ModelDoc2)app.ActiveDoc;Report();Feature f=(Feature)m.FirstFeature();while(f!=null){if(f.GetTypeName2()=="RefPlane"){m.ClearSelection2(true);f.Select2(false,0);m.BlankRefGeom();}f=(Feature)f.GetNextFeature();}m.ClearSelection2(true);m.ViewZoomtofit2();Console.WriteLine("SAVE FINAL "+m.Extension.SaveAs(Path.Combine(root,"16_Корпус_для_чертежа_v3.SLDPRT"),0,1,null,ref e,ref w)+" ERR "+e);return;}
  if(mode=="inspect"){var old=app.OpenDoc6(Path.Combine(root,"Чертеж5.SLDDRW"),3,1,"",ref e,ref w);Console.WriteLine("USER DRAWING "+(old==null?"failed "+e:old.GetTitle()));if(old!=null){var d=(DrawingDoc)old;var v=(View)d.GetFirstView();while(v!=null){Console.WriteLine("VIEW "+v.GetName2()+" TYPE "+v.Type+" MODEL "+v.GetReferencedModelName());v=(View)v.GetNextView();}}return;}
  m=(ModelDoc2)app.NewDocument(@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\templates\gost-part.prtdot",0,0,0);if(m==null)throw new Exception("Template missing");
  Begin(0);
  double[,] p={{0,32.5},{37,32.5},{64.745695,25.068113}};for(int i=0;i<2;i++)L(p[i,0],p[i,1],0,p[i+1,0],p[i+1,1],0);
  Arc(65.2632176,27,0,64.745695,25.068113,0,65.2632176,25,0,1);
  double[,] q={{65.2632176,25},{69.75,25},{70.75,24},{70.75,19.5},{79.25,19.5},{79.25,24},{80.25,25},{88,25},{91,22},{91,11},{90,10},{61,10},{61,19}};for(int i=0;i<q.GetLength(0)-1;i++)L(q[i,0],q[i,1],0,q[i+1,0],q[i+1,1],0);
  Arc(60,19,0,61,19,0,60,20,0,1);L(60,20,0,25,20,0);L(25,20,0,25,27.1,0);Arc(24.6,27.1,0,25,27.1,0,24.6,27.5,0,1);L(24.6,27.5,0,0,27.5,0);L(0,27.5,0,0,32.5,0);
  var axis=L(0,0,0,100,0,0);baseAxis=axis;axis.ConstructionGeometry=true;var sf=End();axis.Select4(true, ((SelectionMgr)m.SelectionManager).CreateSelectData());m.ClearSelection2(true);sf.Select2(false,0);var sd=((SelectionMgr)m.SelectionManager).CreateSelectData();sd.Mark=4;axis.Select4(true,sd);
  var rev=m.FeatureManager.FeatureRevolve2(true,true,false,false,false,false,0,0,2*Math.PI,0,false,false,0,0,0,0,0,true,true,true);if(rev==null)throw new Exception("Revolve failed");rev.Name="Точеный профиль";Report();
  Begin(1);double t=Math.Tan(12*Math.PI/180);double[,] a={{-25,-50},{(22+50)*t,-50},{(22-50)*t,50},{-25,50},{-25,-50}};for(int i=0;i<4;i++)L(a[i,0],0,a[i,1],a[i+1,0],0,a[i+1,1]);Cut("Наклон торца 12 градусов",false,100,0,0,true);Report();
  Begin(2);L(0,-2.5,0,0,2.5,0);L(0,2.5,0,0,2.5,12.05);Arc(0,2.25,12.05,0,2.5,12.05,0,2.25,12.3,1);L(0,2.25,12.3,0,-2.25,12.3);Arc(0,-2.25,12.05,0,-2.25,12.3,0,-2.5,12.05,1);L(0,-2.5,12.05,0,-2.5,0);Cut("Паз 5 R0.25",true,31,3,61);Report();
  Radials(30);Radials(150);Report();
  Begin(1);double[,] h={{78,8},{79.8,8},{79.8,15},{81.5,15},{81.5,35},{78,35},{78,8}};for(int i=0;i<6;i++)L(h[i,0],0,h[i,1],h[i+1,0],0,h[i+1,1]);var hax=L(78,0,0,78,0,40);hax.ConstructionGeometry=true;RevolveCut(hax,"Радиальное отверстие 3.6 цековка 7");Report();
  m.ClearSelection2(true);m.ShowNamedView2("*Изометрия",7);m.ViewZoomtofit2();m.EditRebuild3();var cp=m.Extension.get_CustomPropertyManager("");cp.Add3("Description",30,"Корпус фиксатора",2);cp.Add3("Material",30,"Сталь 10 ГОСТ 1050-2013",2);cp.Add3("Обозначение",30,"КУДМ.724282.016",2);string path=Path.Combine(root,"16_Корпус_перестроенный.SLDPRT");Console.WriteLine("SAVE "+m.Extension.SaveAs(path,0,1,null,ref e,ref w)+" ERR "+e);
 }
}
