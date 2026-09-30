using System;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;

public static class SWFinal {
 static ModelDoc2 m;
 static DrawingDoc d;
 static void L(double x1,double y1,double x2,double y2){var s=m.SketchManager.CreateLine(x1,y1,0,x2,y2,0);if(s!=null)s.Color=0;}
 static void N(string s,double x,double y){m.ClearSelection2(true);var n=(Note)m.InsertNote(s); if(n!=null) ((Annotation)n.GetAnnotation()).SetPosition(x,y,0);m.ClearSelection2(true);}
 static void Poly(double[,] p){for(int i=0;i<p.GetLength(0)-1;i++)L(p[i,0],p[i,1],p[i+1,0],p[i+1,1]);}
 static void DimH(double x1,double x2,double yFeature,double yDim,string text){
  double ext=yDim<yFeature?-0.002:0.002;
  L(x1,yFeature,x1,yDim+ext);L(x2,yFeature,x2,yDim+ext);L(x1,yDim,x2,yDim);
  double a=.0025;L(x1,yDim,x1+a,yDim+a*.3);L(x1,yDim,x1+a,yDim-a*.3);
  L(x2,yDim,x2-a,yDim+a*.3);L(x2,yDim,x2-a,yDim-a*.3);
  N(text,(x1+x2)/2-.004,yDim+.002);
 }
 static void DimHEnds(double x1,double x2,double y1,double y2,double yDim,string label){
  L(x1,y1+.0015,x1,yDim+.002);
  L(x2,y2+.0015,x2,yDim+.002);
  L(x1,yDim,x2,yDim);
  double a=.0025;
  L(x1,yDim,x1+a,yDim+a*.3);L(x1,yDim,x1+a,yDim-a*.3);
  L(x2,yDim,x2-a,yDim+a*.3);L(x2,yDim,x2-a,yDim-a*.3);
  N(label,(x1+x2)/2-.004,yDim+.002);
 }
 static void DimChain(double x0,double x1,double x2,double y0,double y1,double y2,double dimY){
  L(x0,y0+.0015,x0,dimY+.002);L(x1,y1+.0015,x1,dimY+.002);
  L(x2,y2+.0015,x2,dimY+.002);
  foreach(double[] span in new double[][]{new double[]{x0,x1},new double[]{x1,x2}}){
   double l=span[0],r=span[1],a=.0025;
   L(l,dimY,r,dimY);
   L(l,dimY,l+a,dimY+a*.3);L(l,dimY,l+a,dimY-a*.3);
   L(r,dimY,r-a,dimY+a*.3);L(r,dimY,r-a,dimY-a*.3);
  }
  N("37",(x0+x1)/2-.004,dimY+.002);
  N("28",(x1+x2)/2-.004,dimY+.002);
 }
 static void DimV(double y1,double y2,double xFeature,double xDim,string text){
  L(xFeature,y1,xDim-.002,y1);L(xFeature,y2,xDim-.002,y2);L(xDim,y1,xDim,y2);
  L(xDim,y1,xDim-.0007,y1+.0025);L(xDim,y1,xDim+.0007,y1+.0025);
  L(xDim,y2,xDim-.0007,y2-.0025);L(xDim,y2,xDim+.0007,y2-.0025);
  N(text,xDim-.015,(y1+y2)/2);
 }
 static void Leader(double px,double py,double tx,double ty,string text){
  L(px,py,tx,ty);L(px,py,px+.0022,py+.0004);L(px,py,px+.0012,py+.0019);
  N(text,tx+.001,ty);
 }
 static void C(double x,double y,double r){var s=m.SketchManager.CreateCircleByRadius(x,y,0,r);if(s!=null)s.Color=0;}
 static void HolePattern(){
  double cx=.270,cy=.109,r=.027;
  C(cx,cy,r);C(cx,cy,r*27.5/32.5);
  L(cx-.032,cy,cx+.032,cy);L(cx,cy-.032,cx,cy+.032);
  foreach(int sy in new int[]{-1,1})foreach(int sz in new int[]{-1,1}){
   double ux=sy*Math.Cos(Math.PI/6),uy=sz*Math.Sin(Math.PI/6);
   L(cx+ux*.019,cy+uy*.019,cx+ux*.031,cy+uy*.031);
   C(cx+ux*.0245,cy+uy*.0245,.0024);
  }
  N("Схема осей 4 отв., x=17",.240,.147);
  N("±30°",.298,.111);
 }
 static void ProfileTable(){
  double x0=.312,x1=.351,x2=.376,x3=.411,top=.151,row=.0093;
  string[,] rows={
   {"x, мм","⌀, мм","Элемент"},
   {"0–37","65","Цилиндр"},
   {"37–65,26","65→50","Конус 15°"},
   {"65,26–69,75","50","Поясок"},
   {"69,75–70,75","50→48","Фаска 1×45°"},
   {"70,75–79,25","39","Канавка 8,5"},
   {"79,25–80,25","48→50","Фаска 1×45°"},
   {"80,25–88","50","Поясок"},
   {"88–91","50→44","Фаска 3×45°"}
  };
  N("Наружный профиль (x от левого торца)",x0,top+.010);
  foreach(double xx in new double[]{x0,x1,x2,x3})L(xx,top-rows.GetLength(0)*row,xx,top);
  for(int i=0;i<=rows.GetLength(0);i++)L(x0,top-i*row,x3,top-i*row);
  for(int i=0;i<rows.GetLength(0);i++){
   double yy=top-(i+.40)*row;
   N(rows[i,0],x0+.001,yy);N(rows[i,1],x1+.001,yy);N(rows[i,2],x2+.001,yy);
  }
 }
 static double Out(double x){
  if(x<37)return 32.5;
  if(x<65)return 32.5-(x-37)*(7.5/28);
  if(x<70)return 25;
  if(x<71)return 25-(x-70)*5.5;
  if(x<79)return 19.5;
  if(x<80)return 19.5+(x-79)*5.5;
  if(x<88)return 25;
  return 25-(x-88);
 }
 static double Inn(double x){if(x<25)return 27.5;if(x<61)return 20;if(x<90)return 10;return 10+(x-90);}
 static void CrossSection(double x0,double yc){
  // Sheet coordinates in metres; section is drawn at 1:1 from measured axial profile.
  double S=.001;Func<double,double> X=x=>x0+x*S;Func<double,double> Y=z=>yc+z*S;
  double[,] outer={{0,32.5},{37,32.5},{65,25},{69.75,25},{70.75,24},{70.75,19.5},{79.25,19.5},{79.25,24},{80.25,25},{88,25},{91,22}};
  double[,] inner={{0,27.5},{24.6,27.5},{25,27.1},{25,20},{60,20},{61,19},{61,10},{90,10},{91,11}};
  for(int i=0;i<outer.GetLength(0)-1;i++){
   L(X(outer[i,0]),Y(outer[i,1]),X(outer[i+1,0]),Y(outer[i+1,1]));
   L(X(i==0?11.5843:outer[i,0]),Y(-outer[i,1]),X(outer[i+1,0]),Y(-outer[i+1,1]));
  }
  for(int i=0;i<inner.GetLength(0)-1;i++){
   L(X(inner[i,0]),Y(inner[i,1]),X(inner[i+1,0]),Y(inner[i+1,1]));
   L(X(i==0?10.522:inner[i,0]),Y(-inner[i,1]),X(inner[i+1,0]),Y(-inner[i+1,1]));
  }
  L(X(0),Y(27.5),X(0),Y(32.5));L(X(10.522),Y(-27.5),X(11.5843),Y(-32.5));
  L(X(91),Y(11),X(91),Y(22));L(X(91),Y(-11),X(91),Y(-22));
  // Material hatching at 45 degrees, clipped to upper and lower wall bands.
  for(double x=2;x<90;x+=3.3){
   double r=Out(x),q=Inn(x),ym=(r+q)/2;
   double span=Math.Min(2.5,(r-q)*.65);
   L(X(x-span/2),Y(ym-span/2),X(x+span/2),Y(ym+span/2));
   if(x>13)L(X(x-span/2),Y(-ym-span/2),X(x+span/2),Y(-ym+span/2));
  }
  // Centerline.
  for(double x=-7;x<100;x+=9)L(X(x),yc,X(x+6),yc);
  N("А-А",X(9),Y(46));
 }
 public static void Run(string root){
  var app=(ISldWorks)Marshal.GetActiveObject("SldWorks.Application");
  int err=0,warn=0;
  string part=Path.Combine(root,"16_Corpus_surface_repair.SLDPRT");
  var pm=app.OpenDoc6(part,1,0,"",ref err,ref warn);
  if(pm==null)throw new Exception("Part open "+err);
  var cp=pm.Extension.get_CustomPropertyManager("");
  cp.Add3("Description",30,"Корпус фиксатора",2);
  cp.Add3("Material",30,"Сталь 10 ГОСТ 1050-2013",2);
  cp.Add3("Обозначение",30,"КУДМ.000.016",2);
  pm.Extension.SaveAs(part,0,1,null,ref err,ref warn);
  m=(ModelDoc2)app.NewDocument(@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\templates\gost-part drw.drwdot",0,.420,.297);
  if(m==null)throw new Exception("No drawing");d=(DrawingDoc)m;
  var sheet=(Sheet)d.GetCurrentSheet();
  d.SetupSheet5(sheet.GetName(),12,12,1,1,true,@"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2026\lang\english\sheetformat\a3 - gost_sh1_land.slddrt",.420,.297,"Default",false);
  var front=d.CreateDrawViewFromModelView3(part,"*Спереди",.148,.205,0);
  var end=d.CreateDrawViewFromModelView3(part,"*Справа",.300,.205,0);
  sheet=(Sheet)d.GetCurrentSheet();sheet.SetScale(1,1,true,true);
  if(front!=null)front.UseSheetScale=1;
  if(end!=null)end.UseSheetScale=1;
  m.EditRebuild3();
  // The imported part's bounding box is asymmetric. Obtain the projected
  // model origin from the view transform instead of guessing sheet positions.
  double[] fv=(double[])front.ModelToViewTransform.ArrayData;
  double mx=fv[9],my=fv[10];
  Console.WriteLine("FRONT MODEL ORIGIN "+mx+","+my+"; RIGHT EDGE "+(mx+.091));
  d.ActivateSheet(sheet.GetName());
  m.SketchManager.AddToDB=true;
  CrossSection(.102,.104);
  HolePattern();
  ProfileTable();
  // Longitudinal cutting plane through the axis of the main view.
  for(double x=mx-.003;x<mx+.094;x+=.012)L(x,my,Math.Min(x+.008,mx+.095),my);
  L(mx-.002,my-.004,mx-.002,my+.004);L(mx+.095,my-.004,mx+.095,my+.004);
  L(mx-.002,my-.004,mx-.004,my);L(mx-.002,my-.004,mx,my);
  L(mx+.095,my-.004,mx+.093,my);L(mx+.095,my-.004,mx+.097,my);
  N("А",mx-.009,my+.008);N("А",mx+.095,my+.008);
  // Main dimensions, with mm values matching the source surfaces.
  DimHEnds(mx,mx+.091,my+.0239217,my+.022,my+.073,"91");
  DimChain(mx,mx+.037,mx+.065,my+.0239217,my+.0325,my+.025,my+.057);
  DimHEnds(mx,mx+.017,my+.0239217,my+.0325,my+.044,"17");
  DimH(.102,.127,.138,.057,"25");
  DimH(.127,.163,.138,.048,"36");
  DimH(.163,.193,.138,.039,"30");
  DimV(my-.0325,my+.0325,mx,mx-.021,"⌀65");
  Leader(.113,.0765,.111,.091,"⌀55");
  Leader(.144,.084,.137,.094,"⌀40");
  Leader(.178,.094,.204,.098,"⌀20");
  Leader(.166,.129,.204,.125,"⌀50");
  Leader(.176,.1235,.204,.114,"⌀39");
  Leader(.151,.133,.154,.146,"15°");
  Leader(.113,.074,.121,.068,"12°");
  N("4 отв. M4-6H сквозь; цековка ⌀7×3",.248,.270);
  N("Оси 4 отв.: x=17; ±30° от оси Y",.248,.258);
  N("Отв. x=78: ⌀3,6 сквозь; цековка ⌀7×4,5",.248,.246);
  N("Фаски 1×45°; торец 3×45°",.248,.234);
  N("Паз 5×2,3",.345,.205);
  N("L=30",.345,.194);
  Leader(.321,.195,.343,.179,"⌀44");
  N("R2 - конус; R1 - уступ ⌀40",.206,.077);
  N("R0,4 - уступ ⌀55; R0,25 - паз",.206,.068);
  N("Неуказанные пред. откл.: H14, h14, ±IT14/2.",.095,.023);
  N("Неуказанная шероховатость Ra 6,3.",.095,.014);
  N("Отверстия ⌀20, ⌀40, ⌀55: Ra 3,2",.095,.284);
  L(.374,.278,.377,.273);L(.377,.273,.382,.284);L(.382,.284,.392,.284);
  N("Ra 6,3",.392,.280);
  N("Сталь 10 ГОСТ 1050-2013",.301,.018);
  N("Корпус фиксатора",.303,.039);
  m.SketchManager.AddToDB=false;
  m.EditRebuild3();m.ViewZoomtofit2();
  string outpath=Path.Combine(root,"КУДМ.000.016_Корпус_фиксатора_v5.SLDDRW");
  Console.WriteLine("Save drawing "+m.Extension.SaveAs(outpath,0,1,null,ref err,ref warn)+" err "+err);
  string pdf=Path.Combine(root,"КУДМ.000.016_Корпус_фиксатора_v5.pdf");
  Console.WriteLine("Save PDF "+m.Extension.SaveAs(pdf,0,1,null,ref err,ref warn)+" err "+err);
 }
}
