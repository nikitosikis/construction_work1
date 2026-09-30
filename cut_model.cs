using System;
using System.IO;
using KompasAPI7;
using Kompas6Constants;
using Kompas6Constants3D;
public class CutModel {
 public static void Run(string root){
  File.Copy(Path.Combine(root,"part16_model.m3d"),Path.Combine(root,"section_model.m3d"),true);
  var app=(IApplication)Activator.CreateInstance(Type.GetTypeFromProgID("Kompas.Application.7"));
  var d=(IKompasDocument3D)app.Documents.Open(Path.Combine(root,"section_model.m3d"),false,false);
  var p=d.TopPart;
  var cut=((IModelContainer)p).Cuts.Add();
  cut.CutObject=p.get_DefaultObject(ksObj3dTypeEnum.o3d_planeXOZ);
  cut.Direction=false;cut.BuildingType=ksCutBuildingTypeEnum.ksCutByPlane;cut.ChooseType=ksChooseType.ksChBodies;cut.ChoosePartsType=ksChoosePartsType.ksChAllParts;
  Console.WriteLine("BODY "+p.GetBodyById(1).IsSolid+" plane="+p.get_DefaultObject(ksObj3dTypeEnum.o3d_planeXOZ).Name);cut.ChooseBodies=new object[]{p.GetBodyById(1)}; Console.WriteLine("INIT "+cut.InitCutObjects(p.get_DefaultObject(ksObj3dTypeEnum.o3d_planeXOZ),false)); Console.WriteLine("CUT MODEL "+cut.Update());
  Console.WriteLine("ERROR "+cut.Owner.ObjectError);d.Save();d.Close(DocumentCloseOptions.kdDoNotSaveChanges);app.Quit();
 }
}





