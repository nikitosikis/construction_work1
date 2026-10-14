using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
public static class SWDrawingV3 {
 static ISldWorks app;static ModelDoc2 m;static DrawingDoc d;static MathUtility mu;static View front,section,trans,end,detail,key;
 static bool Pick(double x,double y,bool add=false){if(m.Extension.SelectByID2("","EDGE",x/1000,y/1000,0,add,0,null,0))return true;return false;}
 static void Format(Annotation a,double size=3.5){a.Color=0;var f=(TextFormat)a.GetTextFormat(0);f.CharHeight=size/1000;a.SetTextFormat(0,false,f);}
 static void Dim(View view,double x1,double y1,double x2,double y2,double x,double y,bool vertical=false,string prefix="",string suffix="",int precision=0){m.ClearSelection2(true);d.ActivateView(view.GetName2());bool a=Pick(x1,y1),b=Pick(x2,y2,true);if(!a||!b){m.ClearSelection2(true);string t=((vertical?Math.Abs(y2-y1):Math.Abs(x2-x1))/view.ScaleDecimal).ToString("F"+precision);if(prefix!="")t="⌀"+t;t+=suffix;Console.WriteLine("MANUAL DIM "+t+" AT "+x+","+y);if(vertical){L(x1,y1,x,y1);L(x2,y2,x,y2);L(x,y1,x,y2);Arrow(x,Math.Min(y1,y2),0,1);Arrow(x,Math.Max(y1,y2),0,-1);N(t,x-4,(y1+y2)/2-t.Length*.9+6,3.5,Math.PI/2);}else H(x1,x2,y1,y2,y,t);return;}var v=(DisplayDimension)(vertical?m.AddVerticalDimension2(x/1000,y/1000,0):m.AddHorizontalDimension2(x/1000,y/1000,0));if(v==null)throw new Exception("Dimension failed");if(prefix!="")v.SetText(1,prefix);if(suffix!="")v.SetText(2,suffix);v.SetPrecision2(precision,-1,-1,-1);Format((Annotation)v.GetAnnotation());Console.WriteLine("DIM VALUE "+((Dimension)v.GetDimension2(0)).SystemValue+" AT "+x+","+y);m.ClearSelection2(true);}
 static void N(string t,double x,double y,double size=3.5,double angle=0){m.ClearSelection2(true);d.ActivateView("");var n=(Note)m.InsertNote(t);var a=(Annotation)n.GetAnnotation();Format(a,size);n.Angle=angle;a.SetPosition(x/1000,y/1000,0);m.ClearSelection2(true);}
 static void Leader(View view,double px,double py,double tx,double ty,string t){m.ClearSelection2(true);d.ActivateView(view.GetName2());if(!Pick(px,py)){Console.WriteLine("MANUAL LEADER "+t);L(px,py,tx,ty-5);Arrow(px,py,tx-px,ty-5-py);L(tx,ty-5,tx+t.Split(new char[]{(char)10})[0].Length*2,ty-5);N(t,tx,ty);return;}var n=(Note)m.InsertNote(t);var a=(Annotation)n.GetAnnotation();Format(a);a.SetLeader3(3,0,false,false,false,false);a.SetPosition(tx/1000,ty/1000,0);a.SetLeaderAttachmentPointAtIndex(0,px/1000,py/1000,0);m.ClearSelection2(true);}
 static void L(double x,double y,double u,double v){d.ActivateView("");m.SketchManager.AddToDB=true;var s=m.SketchManager.CreateLine(x/1000,y/1000,0,u/1000,v/1000,0);if(s!=null){s.Color=0;s.Width=0;}m.SketchManager.AddToDB=false;}
 static void Arrow(double x,double y,double dx,double dy){double q=Math.Sqrt(dx*dx+dy*dy);dx=dx/q*2.6;dy=dy/q*2.6;L(x,y,x+dx-dy*.22,y+dy+dx*.22);L(x,y,x+dx+dy*.22,y+dy-dx*.22);}
 static void H(double x1,double x2,double y1,double y2,double yy,string t){double s1=Math.Sign(yy-y1),s2=Math.Sign(yy-y2);L(x1,y1+s1*.8,x1,yy+s1*2);L(x2,y2+s2*.8,x2,yy+s2*2);L(x1,yy,x2,yy);Arrow(x1,yy,1,0);Arrow(x2,yy,-1,0);N(t,(x1+x2)/2-t.Length*.85,yy+4.7);}
 static void V(double x1,double y1,double x2,double y2,double x,string t){L(x1,y1,x+Math.Sign(x-x1)*2,y1);L(x2,y2,x+Math.Sign(x-x2)*2,y2);double lo=Math.Min(y1,y2),hi=Math.Max(y1,y2);L(x,lo,x,hi);Arrow(x,lo,0,1);Arrow(x,hi,0,-1);N(t,x-4,(lo+hi)/2-t.Length*.9+6,3.5,Math.PI/2);}
 static void Arc(double cx,double cy,double r,double a,double b){int count=Math.Max(8,(int)(Math.Abs(b-a)*1.5));for(int i=0;i<count;i++){double t=(a+(b-a)*i/count)*Math.PI/180,u=(a+(b-a)*(i+1)/count)*Math.PI/180;L(cx+r*Math.Cos(t),cy+r*Math.Sin(t),cx+r*Math.Cos(u),cy+r*Math.Sin(u));}}
 static void Angle(double cx,double cy,double r,double a,double b,string t,double tx,double ty){Arc(cx,cy,r,a,b);double p=a*Math.PI/180,q=b*Math.PI/180;L(cx,cy,cx+(r+3)*Math.Cos(p),cy+(r+3)*Math.Sin(p));L(cx,cy,cx+(r+3)*Math.Cos(q),cy+(r+3)*Math.Sin(q));Arrow(cx+r*Math.Cos(p),cy+r*Math.Sin(p),-Math.Sin(p),Math.Cos(p));Arrow(cx+r*Math.Cos(q),cy+r*Math.Sin(q),Math.Sin(q),-Math.Cos(q));N(t,tx,ty);}
 static void Center(double x1,double y1,double x2,double y2){double dx=x2-x1,dy=y2-y1,len=Math.Sqrt(dx*dx+dy*dy);dx/=len;dy/=len;for(double a=0;a<len;a+=13){L(x1+dx*a,y1+dy*a,x1+dx*Math.Min(a+8,len),y1+dy*Math.Min(a+8,len));if(a+10<len)L(x1+dx*(a+10),y1+dy*(a+10),x1+dx*Math.Min(a+10.7,len),y1+dy*Math.Min(a+10.7,len));}}
 static void Rough(double x,double y,string t){L(x-2,y+3,x,y);L(x,y,x+4,y+7);L(x+4,y+7,x+17,y+7);N(t,x+5,y+11.5,2.5);}
 static void Radial(View view,double px,double py,double tx,double ty,int precision){d.ActivateView(view.GetName2());m.ClearSelection2(true);if(!Pick(px,py))throw new Exception("Radius selection");var dd=(DisplayDimension)m.AddRadialDimension2(tx/1000,ty/1000,0);if(dd==null)throw new Exception("Radius dimension");dd.SetPrecision2(precision,-1,-1,-1);Format((Annotation)dd.GetAnnotation());Console.WriteLine("RADIUS "+((Dimension)dd.GetDimension2(0)).SystemValue);m.ClearSelection2(true);}
 static void Labels(View v,string label,double x,double y){var an=(Annotation)v.GetFirstAnnotation3();while(an!=null){if(an.GetType()==6){var note=(Note)an.GetSpecificAnnotation();if(note.GetText().Contains(label)){Format(an,5);an.SetPosition(x/1000,y/1000,0);}}an=(Annotation)an.GetNext3();}}
 static void Annotate(){
  m.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayOrigins,false);((DrSection)section.GetSection()).SetDisplayOnlySurfaceCut(true);((DrSection)trans.GetSection()).SetDisplayOnlySurfaceCut(true);m.EditRebuild3();
  var cp=m.Extension.get_CustomPropertyManager("");cp.Add3("Наименование",30,"Корпус фиксатора",2);cp.Add3("Обозначение",30,"КУДМ.724282.016",2);cp.Add3("Код_документа",30,"",2);cp.Add3("Материал",30,"Сталь 10 ГОСТ 1050-2013",2);cp.Add3("Масса",30,"0,940",2);cp.Add3("Лит",30,"У",2);cp.Add3("Типоразмер",30,"",2);cp.Add3("Заготовка",30,"",2);
  Labels(section,"А-А",122,285);Labels(trans,"Б-Б",250,158);Labels(detail,"В",302,285);Labels(key,"Г",365,204);
  Dim(section,85,245,110,238,97.5,260);Dim(section,110,238,146,231,128,260);Dim(section,146,231,176,232,161,260);Dim(section,85,245,176,232,130,272,false,"","*");
  Dim(front,85,110,122,115,103.5,153);V(100,132.5,100,67.5,65,"⌀65");V(171,125,171,75,198,"⌀50");V(160,119.5,160,80.5,186,"⌀39");
  Dim(section,95,242.5,105,187.5,99,225,true,"<MOD-DIAM>");Dim(section,130,235,130,195,138,225,true,"<MOD-DIAM>");
  Dim(detail,305.5,241,322.5,241,314,260,false,"","",1);Dim(detail,322.5,241,346,239,334,272,false,"","",2);
  Dim(end,352.7,120,375,119,363.5,73,false,"","",1);
  double[] kt=(double[])key.ModelToViewTransform.ArrayData;double kx=kt[9]*1000-12.05*4,ky=kt[10]*1000;
  Dim(key,kx+4,ky+10,kx+4,ky-10,345,ky,true,"","",0);Radial(key,kx-.7071,ky+9.7071,389,194,2);
  Radial(section,109.882843,187.617157,120,169,1);Radial(section,145.707107,195.292893,148,174,0);Radial(section,150.002165,189.98289,180,169,0);
  Leader(front,105.5,128.1458,42,154,"4 отв. M4-6H\nЦековка ⌀7, глуб. 3");Leader(section,164.8,229,193,242,"⌀3,6 сквозь\nЦековка ⌀7, глуб. 4,5");
  Leader(detail,304.5,244,269,251,"1×45°");Leader(detail,323.5,244,358,257,"1×45°");Leader(detail,343,242,361,237,"3×45°");Leader(section,175.5,204.5,200,185,"1×45°");
  Leader(end,372.071,111.929,386,80,"⌀20");
  Center(77,215,93,215);Center(106,215,125,215);Center(142,215,184,215);Center(77,100,184,100);Center(222,105,298,105);Center(260,67,260,145);Center(328,119,402,119);Center(365,82,365,156);
  Center(102,124,102,133);Center(102,67,102,76);Center(163,94,163,106);
  H(85,102,76.0783,71.855,45,"17");H(163,176,75,78,45,"13");
  Angle(122,67.5,23,0,15,"15°",149,63);Angle(95.522,187.5,18,-90,-78,"12°",87,165);
  foreach(double ang in new double[]{60,120,240,300}){double a=ang*Math.PI/180;Center(260+22*Math.Cos(a),105+22*Math.Sin(a),260+39*Math.Cos(a),105+39*Math.Sin(a));double ux=Math.Cos(a),uy=Math.Sin(a),vx=-uy,vy=ux;foreach(int sign in new int[]{-1,1})L(260+27.5*ux+2*sign*vx,105+27.5*uy+2*sign*vy,260+29.5*ux+2*sign*vx,105+29.5*uy+2*sign*vy);}
  Angle(260,105,43,60,90,"30°",275,151);Angle(260,105,43,90,120,"30°",236,151);
  L(94,242.5,48,238);Rough(48,238,"Ra 3,2");Rough(116,195,"Ra 3,2");L(375,119,395,156);Rough(395,156,"Ra 3,2");Rough(366,273,"Ra 6,3");N("(     )",386,284,5);L(391,278,393,275);L(393,275,397,282);
  N("1. * Размер для справок.",43,31);N("2. Неуказанные предельные отклонения размеров: H14, h14, ±IT14/2.",43,22);N("3. Удалить заусенцы.",43,13);
  d.ActivateView("");m.ClearSelection2(true);m.EditRebuild3();
 }
 static double[] Project(View v,double[] p){return (double[])((MathPoint)((MathPoint)mu.CreatePoint(p)).MultiplyTransform(v.ModelToViewTransform)).ArrayData;}
 static void Origin(View v,double x,double y){m.EditRebuild3();double[] t=(double[])v.ModelToViewTransform.ArrayData;double[] p=(double[])v.Position;p[0]+=x/1000-t[9];p[1]+=y/1000-t[10];v.Position=p;m.EditRebuild3();}
 static void Report(View v){Console.WriteLine("VIEW "+v.GetName2()+" MATRIX "+string.Join(";",(double[])v.ModelToViewTransform.ArrayData)+" OUTLINE "+string.Join(";",(double[])v.GetOutline()));}
 static void Vertices(View v){var a=(object[])v.GetVisibleEntities2(null,2);if(a==null)return;foreach(Vertex b in a){double[] p=(double[])b.GetPoint(),q=Project(v,p);Console.WriteLine("VERTEX "+v.GetName2()+" "+string.Join(";",p)+" -> "+q[0]*1000+";"+q[1]*1000);}}
 static View Sec(View v,double x1,double y1,double x2,double y2,double px,double py,string label,int option){m.ClearSelection2(true);d.ActivateView(v.GetName2());double[] vp=(double[])v.Position;var s=m.SketchManager.CreateLine(x1/1000-vp[0],y1/1000-vp[1],0,x2/1000-vp[0],y2/1000-vp[1],0);m.ClearSelection2(true);s.Select4(false,null);var a=d.CreateSectionViewAt5(px/1000,py/1000,0,label,option,null,0);if(a==null)throw new Exception("Section "+label+" failed");a.UseSheetScale=1;m.ClearSelection2(true);return a;}
 static void Save(string root){m.EditRebuild3();m.ViewZoomtofit2();int e=0,w=0;string name="КУДМ.724282.016_Корпус_фиксатора_по_образцу_v6";bool ok=m.Extension.SaveAs(Path.Combine(root,name+".SLDDRW"),0,1,null,ref e,ref w);Console.WriteLine("DRAWING "+ok+" ERR "+e);if(!ok)throw new Exception("Save drawing");ok=m.Extension.SaveAs(Path.Combine(root,name+".pdf"),0,1,null,ref e,ref w);Console.WriteLine("PDF "+ok+" ERR "+e);m.Save3(1,ref e,ref w);}
 public static void Run(string root,string mode){app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");mu=(MathUtility)app.GetMathUtility();int er=0,wa=0;
  if(mode=="inspect"){m=(ModelDoc2)app.ActiveDoc;d=(DrawingDoc)m;View v=(View)d.GetFirstView();v=(View)v.GetNextView();while(v!=null){Report(v);Vertices(v);v=(View)v.GetNextView();}return;}
  var prev=(ModelDoc2)app.GetOpenDocumentByName(Path.Combine(root,"КУДМ.724282.016_Корпус_фиксатора_по_образцу_v6.SLDDRW"));if(prev!=null){if(prev.GetSaveFlag())throw new Exception("New drawing has unsaved user edits");app.CloseDoc(prev.GetTitle());}string part=Path.Combine(root,"16_Корпус_для_чертежа_v3.SLDPRT");var pm=app.OpenDoc6(part,1,1,"",ref er,ref wa);m=(ModelDoc2)app.NewDocument(@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\templates\gost-part drw.drwdot",0,.420,.297);d=(DrawingDoc)m;var sh=(Sheet)d.GetCurrentSheet();d.SetupSheet5(sh.GetName(),12,12,1,1,true,@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\lang\english\sheetformat\a3 - gost_sh1_land.slddrt",.420,.297,"Default",false);sh=(Sheet)d.GetCurrentSheet();sh.SetScale(1,1,true,true);
  front=d.CreateDrawViewFromModelView3(part,"*Спереди",.130,.100,0);front.UseSheetScale=1;Origin(front,85,100);Report(front);
  section=Sec(front,78,100,184,100,130,217,"А",4);Origin(section,85,215);Report(section);
  trans=Sec(front,102,58,102,142,263,102,"Б",5);Origin(trans,260,105);Report(trans);
  end=d.CreateDrawViewFromModelView3(part,"*Справа",.367,.119,0);end.UseSheetScale=1;Origin(end,365,119);Report(end);
  d.ActivateView(section.GetName2());double[] sp=(double[])section.Position;var circle=m.SketchManager.CreateCircleByRadius(.163-sp[0],.233-sp[1],0,.016);m.ClearSelection2(true);circle.Select4(false,null);detail=(View)d.CreateDetailViewAt4(.312,.235,0,0,2,1,"В",1,false,false,false,0);if(detail==null)throw new Exception("Detail failed");Origin(detail,164,195);Report(detail);
  d.ActivateView(end.GetName2());double[] ep=(double[])end.Position;circle=m.SketchManager.CreateCircleByRadius(.3535-ep[0],.119-ep[1],0,.004);m.ClearSelection2(true);circle.Select4(false,null);key=(View)d.CreateDetailViewAt4(.372,.180,0,0,4,1,"Г",1,false,false,false,0);if(key==null)throw new Exception("Key detail failed");Report(key);
  m.ViewZoomtofit2();m.GraphicsRedraw2();Annotate();d.ActivateSheet(sh.GetName());m.ClearSelection2(true);Save(root);
 }
}







