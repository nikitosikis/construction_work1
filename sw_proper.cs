using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
public static class SWProper {
 static ModelDoc2 m; static DrawingDoc d;
 static void L(double x,double y,double u,double v){if(Math.Abs(x-u)+Math.Abs(y-v)<.00001)return;var s=m.SketchManager.CreateLine(x/1000,y/1000,0,u/1000,v/1000,0);if(s!=null)s.Color=0;}
 static void N(string text,double x,double y,double angle=0,double size=3.5){m.ClearSelection2(true);var n=(Note)m.InsertNote(text);var a=(Annotation)n.GetAnnotation();var f=(TextFormat)a.GetTextFormat(0);f.CharHeight=size/1000;a.SetTextFormat(0,false,f);n.Angle=angle; a.SetPosition(x/1000,y/1000,0);m.ClearSelection2(true);}
 static void C(double x,double y,double r){var s=m.SketchManager.CreateCircleByRadius(x/1000,y/1000,0,r/1000);if(s!=null)s.Color=0;}
 static void Arrow(double x,double y,double dx,double dy){double q=Math.Sqrt(dx*dx+dy*dy);dx=dx/q*2.6;dy=dy/q*2.6;L(x,y,x+dx-dy*.22,y+dy+dx*.22);L(x,y,x+dx+dy*.22,y+dy-dx*.22);}
 static void H(double x1,double x2,double y1,double y2,double yy,string t){double s1=Math.Sign(yy-y1),s2=Math.Sign(yy-y2);L(x1,y1+s1*.8,x1,yy+s1*2);L(x2,y2+s2*.8,x2,yy+s2*2);L(x1,yy,x2,yy);Arrow(x1,yy,1,0);Arrow(x2,yy,-1,0);N(t,(x1+x2)/2-t.Length*.85,yy+4.7);}
 static void V(double y1,double y2,double x1,double x2,double xx,string t){if(Math.Abs(xx-x1)>.1)L(x1,y1,xx+Math.Sign(xx-x1)*2,y1);if(Math.Abs(xx-x2)>.1)L(x2,y2,xx+Math.Sign(xx-x2)*2,y2);L(xx,y1,xx,y2);Arrow(xx,y1,0,1);Arrow(xx,y2,0,-1);N(t,xx-4,(y1+y2)/2-t.Length*.9+(t.StartsWith("⌀")?8:0),Math.PI/2);}
 static void Lead(double px,double py,double ex,double ey,double width,string t){L(px,py,ex,ey);Arrow(px,py,ex-px,ey-py);L(ex,ey,ex+width,ey);N(t,ex+.7,ey+4.8);}
 static void Arc(double cx,double cy,double r,double a,double b){int count=Math.Max(8,(int)(Math.Abs(b-a)*1.5));for(int i=0;i<count;i++){double t=(a+(b-a)*i/count)*Math.PI/180,u=(a+(b-a)*(i+1)/count)*Math.PI/180;L(cx+r*Math.Cos(t),cy+r*Math.Sin(t),cx+r*Math.Cos(u),cy+r*Math.Sin(u));}}
 static void Angle(double cx,double cy,double r,double a,double b,string text,double tx,double ty){Arc(cx,cy,r,a,b);double p=a*Math.PI/180,q=b*Math.PI/180;L(cx,cy,cx+(r+3)*Math.Cos(p),cy+(r+3)*Math.Sin(p));L(cx,cy,cx+(r+3)*Math.Cos(q),cy+(r+3)*Math.Sin(q));Arrow(cx+r*Math.Cos(p),cy+r*Math.Sin(p),-Math.Sin(p),Math.Cos(p));Arrow(cx+r*Math.Cos(q),cy+r*Math.Sin(q),Math.Sin(q),-Math.Cos(q));N(text,tx,ty);}
 static void Poly(double[,] p){for(int i=0;i<p.GetLength(0)-1;i++)L(p[i,0],p[i,1],p[i+1,0],p[i+1,1]);}
 static void Hatch(double[,] p,double spacing=3){double lo=1e9,hi=-1e9;for(int i=0;i<p.GetLength(0);i++){lo=Math.Min(lo,p[i,1]-p[i,0]);hi=Math.Max(hi,p[i,1]-p[i,0]);}for(double k=Math.Ceiling(lo/spacing)*spacing;k<hi;k+=spacing){var xs=new List<double>();for(int i=0;i<p.GetLength(0)-1;i++){double a=p[i,1]-p[i,0]-k,b=p[i+1,1]-p[i+1,0]-k;if((a<=0&&b>0)||(b<=0&&a>0)){double t=-a/(b-a);xs.Add(p[i,0]+t*(p[i+1,0]-p[i,0]));}}xs.Sort();for(int j=0;j+1<xs.Count;j+=2)L(xs[j],xs[j]+k,xs[j+1],xs[j+1]+k);}}
 static void Center(double x1,double y1,double x2,double y2){double dx=x2-x1,dy=y2-y1,len=Math.Sqrt(dx*dx+dy*dy);dx/=len;dy/=len;for(double a=0;a<len;a+=13){L(x1+dx*a,y1+dy*a,x1+dx*Math.Min(a+8,len),y1+dy*Math.Min(a+8,len));if(a+10<len)L(x1+dx*(a+10),y1+dy*(a+10),x1+dx*Math.Min(a+10.7,len),y1+dy*Math.Min(a+10.7,len));}}
 static void Rough(double x,double y,string t){L(x-2,y+3,x,y);L(x,y,x+4,y+7);L(x+4,y+7,x+17,y+7);N(t,x+5,y+11.5,0,2.5);}
 static void Section(double x,double y){
  double[,] top={{0,32.5},{37,32.5},{64.745,25.068},{65.263,25},{69.75,25},{70.75,24},{70.75,19.5},{74.5,19.5},{74.5,15},{76.2,15},{76.2,12.3},{61,12.3},{61,19},{60,20},{25,20},{25,27.1},{24.6,27.5},{0,27.5},{0,32.5}};
  double[,] topRight={{81.5,25},{88,25},{91,22},{91,12.3},{79.8,12.3},{79.8,15},{81.5,15},{81.5,25}};
  double[,] bot={{11.5843,-32.5},{37,-32.5},{64.745,-25.068},{65.263,-25},{69.75,-25},{70.75,-24},{70.75,-19.5},{79.25,-19.5},{79.25,-24},{80.25,-25},{88,-25},{91,-22},{91,-11},{90,-10},{61,-10},{61,-19},{60,-20},{25,-20},{25,-27.1},{24.6,-27.5},{10.522,-27.5},{11.5843,-32.5}};
  foreach(var p in new double[][,]{top,topRight,bot}){for(int i=0;i<p.GetLength(0);i++){p[i,0]+=x;p[i,1]+=y;}Poly(p);Hatch(p,3);}
  Center(x-7,y,x+99,y);
  N("А-А",x+39,y+69,0,5);
  H(x,x+25,y+32.5,y+27.5,y+47,"25");H(x+25,x+61,y+20,y+19,y+47,"36");H(x+61,x+91,y+10,y+22,y+47,"30");
  H(x,x+91,y+32.5,y+22,y+59,"91");
  V(y-27.5,y+27.5,x+12,x+12,x+12,"⌀55");V(y-20,y+20,x+43,x+43,x+43,"⌀40");Center(x+78,y+11,x+78,y+30);
  Lead(x+24.8,y-27.4,x+26,y-48,18,"R0,4");Lead(x+60.7,y-19.7,x+59,y-39,15,"R1");
  Rough(x+4,y+27.5,"Ra 3,2");L(x+34,y+20,x+34,y+28);Rough(x+34,y+28,"Ra 3,2");L(x+71,y-10,x+104,y-6);Rough(x+104,y-6,"Ra 3,2");
  // Extension of the slanted left end, measured against a plane normal to the axis.
  double vx=x+10.522,vy=y-27.5;Angle(vx,vy,19,-90,-78,"12°",vx-9,vy-22);
  // Detail reference around the right-hand outer profile.
  Lead(x+79.8,y+14,x+112,y+16,48,"⌀7, глуб. 4,5");N("⌀3,6 сквозь",x+113,y+26);
  Lead(x+90.5,y-10.5,x+106,y-28,25,"1×45°");
 }
 static void Neck(){
  double x=286,y=187;Func<double,double> X=a=>x+(a-62)*2;Func<double,double> Y=a=>y+(a-10)*2;
  double[,] p={{62,25.803},{64.745,25.068},{65.263,25},{69.75,25},{70.75,24},{70.75,19.5},{79.25,19.5},{79.25,24},{80.25,25},{88,25},{91,22},{91,11},{90,10},{62,10},{62,25.803}};for(int i=0;i<p.GetLength(0);i++){p[i,0]=X(p[i,0]);p[i,1]=Y(p[i,1]);}Poly(p);Hatch(p,3);
  N("Б (2:1)",303,273,0,5);
  H(X(70.75),X(79.25),Y(19.5),Y(19.5),240,"8,5");
  H(X(79.25),X(91),Y(24),Y(22),254,"11,75");
  Lead(X(70.25),Y(24.5),263,249,26,"1×45°");Lead(X(79.75),Y(24.5),348,239,29,"1×45°");
  Lead(X(89.5),Y(23.5),364,226,25,"3×45°");
  Lead(X(65.0),Y(25.017),263,227,18,"R2");
  H(X(65.263),X(69.75),Y(25),Y(25),174,"4,49*");
  H(X(80.25),X(88),Y(25),Y(25),174,"7,75*");
 }
 static void KeyDetail(){
  double x=367,y=111;
  N("В (4:1)",351,147,0,5);
  // Enlarged fragment of the left side of the axial bore: straight slot bottom and two flanks.
  Arc(x+40,y,40,148.33,165.522);L(x+1.2702,y+10,x-8.2,y+10);Arc(x-8.2,y+9,1,90,180);L(x-9.2,y+9,x-9.2,y-9);Arc(x-8.2,y-9,1,180,270);L(x-8.2,y-10,x+1.2702,y-10);Arc(x+40,y,40,194.478,211.67);
  
  V(y-10,y+10,x-9.2,x-9.2,342,"5");
  Lead(x-9.2,y+10,383,134,19,"R0,25");
 }
 public static void Run(string root){
  var app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");int err=0,warn=0;string existing=Path.Combine(root,"КУДМ.724282.016_Корпус_фиксатора_итог.SLDDRW");var prior=(ModelDoc2)app.GetOpenDocumentByName(existing);if(prior!=null){if(prior.GetSaveFlag())throw new Exception("Current output has unsaved edits");app.CloseDoc(prior.GetTitle());}string part=Path.Combine(root,"16_Corpus_surface_repair.SLDPRT");var pm=app.OpenDoc6(part,1,0,"",ref err,ref warn);if(pm==null)throw new Exception("No part");var cp=pm.Extension.get_CustomPropertyManager("");cp.Add3("Description",30,"Корпус фиксатора",2);cp.Add3("Material",30,"Сталь 10 ГОСТ 1050-2013",2);cp.Add3("Обозначение",30,"КУДМ.724282.016",2);pm.Save3(1,ref err,ref warn);
  m=(ModelDoc2)app.NewDocument(@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\templates\gost-part drw.drwdot",0,.420,.297);d=(DrawingDoc)m;var sh=(Sheet)d.GetCurrentSheet();d.SetupSheet5(sh.GetName(),12,12,1,1,true,@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\lang\english\sheetformat\a3 - gost_sh1_land.slddrt",.420,.297,"Default",false);
  var front=d.CreateDrawViewFromModelView3(part,"*Спереди",.134677,.101987,0);var end=d.CreateDrawViewFromModelView3(part,"*Справа",.277692,.114987,0);sh=(Sheet)d.GetCurrentSheet();sh.SetScale(1,1,true,true);front.UseSheetScale=1;end.UseSheetScale=1;m.EditRebuild3();double[] fm=(double[])front.ModelToViewTransform.ArrayData,em=(double[])end.ModelToViewTransform.ArrayData;double x=fm[9]*1000,y=fm[10]*1000;Console.WriteLine("FRONT "+x+","+y+" END "+em[9]*1000+","+em[10]*1000);d.ActivateSheet(sh.GetName());m.SketchManager.AddToDB=true;
  Section(x,213);Neck();KeyDetail();
  // Exterior dimensions and directly attached hole callouts.
  H(x,x+37,y+23.9217,y+32.5,151,"37");
  H(x,x+17,y-23.9217,y-28,50,"17");H(x+78,x+91,y-25,y-22,50,"13");
  V(y-32.5,y+32.5,x+10,x+10,69,"⌀65");V(y-19.5,y+19.5,x+75,x+75,197,"⌀39");V(y-25,y+25,x+85,x+85,211,"⌀50");
  Lead(x+17,y+28,41,142,44,"⌀7, глуб. 3");N("4 отв. M4-6H",42,152);
  C(x+78,y+23,15);Lead(x+67,y+34,142,143,8,"Б");
  // 15-degree cone angle shown by an arc outside the outline.
  Angle(x+37,y+32.5,23,165,180,"15°",x+8,y+45);
  Center(x-5,y,x+100,y);foreach(double xx in new double[]{x-5,x+96}){L(xx,y-4,xx,y+5);Arrow(xx,y-4,0,1);}N("А",x-13,y+8);N("А",x+94,y+8);
  // End-view axes, keyway reference and bore-entry chamfer.
  double ex=270,ey=y+13;Center(ex-38,ey,ex+38,ey);Center(ex,ey-38,ex,ey+38);C(ex-10.8,ey,6);Lead(ex-15,ey+5,242,137,8,"В");
  H(ex-12.3,ex+10,ey,ey,65,"22,3");Lead(ex+7.071,ey-7.071,305,78,18,"⌀20");Lead(358,111,333,159,40,"Паз на длине 30");
  // Radial axes of the four threaded holes are shown as center lines on the end view.
  foreach(double ang in new double[]{60,120,240,300}){double a=ang*Math.PI/180;Center(ex+21*Math.Cos(a),ey+21*Math.Sin(a),ex+39*Math.Cos(a),ey+39*Math.Sin(a));}
  Lead(ex+34*Math.Cos(Math.PI/3),ey+34*Math.Sin(Math.PI/3),305,168,31,"Оси 4 отв.");Angle(ex,ey,43,60,90,"30°",ex+15,ey+47);Angle(ex,ey,43,90,120,"30°",ex-23,ey+47);
  N("* Размеры для справок.",43,35);N("Неуказанные предельные отклонения: H14, h14, ±IT14/2.",43,25);N("Удалить заусенцы.",43,15);
  Rough(365,270,"Ra 6,3");N("(     )",386,281,0,5);L(391,275,393,272);L(393,272,397,279);
  N("Корпус фиксатора",302,39);N("Сталь 10 ГОСТ 1050-2013",301,18);
  m.SketchManager.AddToDB=false;m.EditRebuild3();m.ViewZoomtofit2();string name="КУДМ.724282.016_Корпус_фиксатора_итог";string path=Path.Combine(root,name+".SLDDRW");bool saved=m.Extension.SaveAs(path,0,1,null,ref err,ref warn);Console.WriteLine("DRAWING "+saved+" ERROR "+err);if(!saved)throw new Exception("Drawing save failed");bool pdf=m.Extension.SaveAs(Path.Combine(root,name+".pdf"),0,1,null,ref err,ref warn);Console.WriteLine("PDF "+pdf+" ERROR "+err);if(!pdf)throw new Exception("PDF save failed");
 }
}
