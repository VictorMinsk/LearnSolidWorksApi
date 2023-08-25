// See https://aka.ms/new-console-template for more information

using LearnSolidWorksApi;
using SolidWorks.Interop.sldworks;

SldWorks? swApp = SolidWorksSingleton.GetApplication();
//swApp.SendMsgToUser("Hello, SolidWorks");
Console.WriteLine("Hello, SolidWorks");

//编辑草图
//EditSketch editSketch=new EditSketch(swApp!);
//创建草图圆角
//editSketch.CreateFillet();
//创建草图倒角
//editSketch.CreateChamfer();
//剪裁草图实体
//editSketch.SketchTrim();
//延伸草图实体
//editSketch.SketchExtend();
//等距草图实体
//editSketch.SketchOffset();
//镜像草图实体
//editSketch.SketchMirror();
//线性草图阵列
//editSketch.CreateLinearSketchPattern();
//编辑线性草图阵列
//editSketch.EditLinearSketchPattern();
//圆周草图阵列
//editSketch.CreateCircularSketchPattern();
//编辑圆周草图阵列
//editSketch.EditCircularSketchPattern();
//移动复制草图实体
//editSketch.MoveOrCopy();
//旋转复制草图实体
//editSketch.RotateOrCopy();
//隐藏显示草图约束
//editSketch.ViewSketchRelations();
//添加草图约束关系
//editSketch.SketchAddConstraints();
//标注草图尺寸
//editSketch.AddDimension();
//转换为构造线
//editSketch.CreateConstructionGeometry();
//拆分草图实体
//editSketch.SplitSegment();


//编辑特征
EditFeature editFeature = new EditFeature(swApp!);
//拉伸凸台基体
//editFeature.FeatureExtrusion();
//旋转凸台基体
//editFeature.FeatureRevolve();
//扫描凸台基体
//editFeature.Sweep();
//放样凸台基体
//editFeature.Loft();
//拉伸切除
//editFeature.FeatureCut();
//旋转切除
//editFeature.RevolveCut();
//扫描切除
//editFeature.SweepCut();
//放样切除
//editFeature.LoftCut();
//异型孔向导
//editFeature.HoleWizard();
//圆角
//editFeature.FaceFillet();
//editFeature.ConstFillet();
//editFeature.FullRoundFillet();
//倒角
//editFeature.Chamfer();
//镜像
//editFeature.Mirror();
//线性阵列
//editFeature.LinearPattern();
//圆周阵列
//editFeature.CircularPattern();
//草图驱动阵列
editFeature.SketchPattern();
