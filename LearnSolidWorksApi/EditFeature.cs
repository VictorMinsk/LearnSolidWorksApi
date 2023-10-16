using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;

namespace LearnSolidWorksApi;

public class EditFeature
{
    private readonly SldWorks _swApp;
    public EditFeature(SldWorks swApp)
    {
        _swApp = swApp;
    }

    /// <summary>
    /// 新建零件文档
    /// </summary>
    public ModelDoc2 NewDocument()
    {
        //先确保证设置中默认模板已经设置
        var defaultTemplate =
            _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
        //使用默认模板新建零件
        return (_swApp.NewDocument(defaultTemplate, 0, 0, 0) as ModelDoc2)!;
    }

    /// <summary>
    /// 拉伸特征
    /// </summary>
    public void FeatureExtrusion()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);


        swSketchMgr.CreateCenterRectangle(0, 0, 0, 1, 1, 0);
        swSketchMgr.InsertSketch(true);
        swModel.ViewZoomtofit2();

        //由于退出草图编辑时草图默认处于选中状态
        //为了让我们更明白，拉伸之前我们选择了草图，故意清除选择并再次选择草图，然后再创建拉伸特征
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("", "SKETCH", 0, 1, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

        var swFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
             (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
             2, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
             false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
             true, false, true, //多实体合并结果
             (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距
        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
            //特征类型：Extrusion，特征名称：Boss-Extrude1
        }
    }

    /// <summary>
    /// 旋转特征
    /// </summary>
    public void FeatureRevolve()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);

        //中心线
        swSketchMgr.CreateCenterLine(0, 0, 0, 0, 2, 0);
        //旋转草图
        swSketchMgr.CreateCircleByRadius(-2, 1, 0, 0.5);
        swSketchMgr.InsertSketch(true);
        swModel.ViewZoomtofit2();

        swModel.ClearSelection2(true);
        //根据坐标选择中心线，标记为4
        swModelDocExt.SelectByID2("", "EXTSKETCHSEGMENT", 0, 1, 0, false, 4, null, 0);
        //根据坐标选择草图圆，加选，标记为0
        swModelDocExt.SelectByID2("", "EXTSKETCHSEGMENT", -1.5, 1, 0, true, 0, null, 0);

        var swFeat = swFeatureMgr.FeatureRevolve2(
            true, true, false, false, //与旋转结果有关的参数
            false, false, //与旋转方向相关的参数
            (int)swEndConditions_e.swEndCondBlind, 0, //旋转结束条件
            360 * Math.PI / 180, 0, //旋转角度相关的参数
            false, false, 0, 0, //与偏移相关的参数(到离指定面指定的距离)            
            (int)swThinWallType_e.swThinWallOneDirection, 0, 0, //与薄壁类型相关的参数
            true, false, true//与多实体相关的参数
            );
        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }
    }

    /// <summary>
    /// 扫描特征
    /// </summary>
    public void Sweep()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModelDocExt.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);

        swSketchMgr.CreateEllipse(0, 0, 0, 0.5, 0, 0, 0, 1, 0);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);

        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateArc(-2, 0, 0, 0, 0, 0, -2, 2, 0, 1);
        swSketchMgr.InsertSketch(true);

        swModel.ViewZoomtofit2();

        swModel.ClearSelection2(true);

        //选择扫描草图轮廓，标记为1
        swModelDocExt.SelectByID2("", "SKETCH", 0.5, 0, 0, false, 1, null, 0);
        //选择扫描草图路径，标记为4
        swModelDocExt.SelectByID2("", "SKETCH", -2, 2, 0, true, 4, null, 0);

        var swSweep = (SweepFeatureData)swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweep);
        var swFeat = swFeatureMgr.CreateFeature(swSweep);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }
    }

    /// <summary>
    /// 放样特征
    /// </summary>
    public void Loft()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        //绘制第一个轮廓
        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.07, 0, 0, 0, 0.03, 0);
        swSketchMgr.InsertSketch(true);

        //插入新的参考基准面，绘制第二个轮廓
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        //todo:待解释InsertRefPlane
        swFeatureMgr.InsertRefPlane(8, 0.07, 0, 0, 0, 0);
        swModel.ClearSelection2(true);

        swModelDocExt.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.05, 0, 0, 0, 0.01, 0);
        swSketchMgr.InsertSketch(true);


        //插入新的参考基准面，绘制第三个轮廓
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        //todo:待解释InsertRefPlane
        swFeatureMgr.InsertRefPlane(8, 0.14, 0, 0, 0, 0);
        swModel.ClearSelection2(true);

        swModelDocExt.SelectByID2("Plane2", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateEllipse(0, 0, 0, 0.06, 0, 0, 0, 0.02, 0);
        swSketchMgr.InsertSketch(true);


        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();


        //创建引导曲线
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("Point1@Sketch1", "EXTSKETCHPOINT", 0.07, 0, 0, false, 1, null, 0);
        swModelDocExt.SelectByID2("Point1@Sketch2", "EXTSKETCHPOINT", 0.05, 0, 0, true, 1, null, 0);
        swModelDocExt.SelectByID2("Point1@Sketch3", "EXTSKETCHPOINT", 0.06, 0, 0, true, 1, null, 0);
        swModel.Insert3DSplineCurve(false);
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("Point3@Sketch1", "EXTSKETCHPOINT", -0.07, 0, 0, false, 1, null, 0);
        swModelDocExt.SelectByID2("Point3@Sketch2", "EXTSKETCHPOINT", -0.05, 0, 0, true, 1, null, 0);
        swModelDocExt.SelectByID2("Point3@Sketch3", "EXTSKETCHPOINT", -0.06, 0, 0, true, 1, null, 0);
        swModel.Insert3DSplineCurve(false);

        //选择轮廓，标记为1，选择顺序与放样方向一致
        swModelDocExt.SelectByID2("Sketch1", "SKETCH", 0.07, 0, 0, false, 1, null, 0);
        swModelDocExt.SelectByID2("Sketch2", "SKETCH", 0.05, 0, 0.07, true, 1, null, 0);
        swModelDocExt.SelectByID2("Sketch3", "SKETCH", 0.05, 0, 0.07, true, 1, null, 0);

        //选择引导线，标记为2
        swModelDocExt.SelectByID2("Curve1", "REFERENCECURVES", 0, 0, 0, true, 2, null, 0);
        swModelDocExt.SelectByID2("Curve2", "REFERENCECURVES", 0, 0, 0, true, 2, null, 0);

        //创建放样特征
        swFeatureMgr.InsertProtrusionBlend(false, true, false, 1, 0, 0, 1, 1, true, true, false, 0, 0, 0, true, true, true);
    }

    /// <summary>
    /// 拉伸切除特征
    /// </summary>
    public void FeatureCut()
    {

        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("", "FACE", 0, 0, 2, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        //创建键槽草图
        swSketchMgr.CreateSketchSlot((int)swSketchSlotCreationType_e.swSketchSlotCreationType_line,
            (int)swSketchSlotLengthType_e.swSketchSlotLengthType_FullLength,
            0.5, -0.25, 0, 0, 0.25, 0, 0, 0.5, 0.25, 0, 1, true);
        //绘制草图后我们直接创建拉伸切除
        var swFeat = swFeatureMgr.FeatureCut4(
             true, false, false, //拉伸切除方向
             (int)swEndConditions_e.swEndCondBlind, 0, //结束条件相关
             1, 0, //拉伸深度
             false, false, false, true, 0, 0, //拔模相关
             false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
             false, //钣金正交切除（非钣金使用false）
             false, true, //多实体，特征范围
             false, true, false, //装配体特征范围
             (int)swStartConditions_e.swStartSketchPlane, 0, false, //结束条件
             false//钣金零件，优化几何图形
         );
        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
            //特征类型：ICE，特征名称：Cut-Extrude1
        }
        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 旋转切除
    /// </summary>
    public void RevolveCut()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("", "FACE", 0, 0, 2, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCenterLine(0, 0, 0, 0, 1, 0);
        swSketchMgr.CreateCircleByRadius(0.5, 1, 0, 0.1);
        var swFeat = swFeatureMgr.FeatureRevolve2(
            true, true, false, true, //与旋转结果有关的参数
            false, false, //与旋转方向相关的参数
            (int)swEndConditions_e.swEndCondBlind, 0, //旋转结束条件
            360 * Math.PI / 180, 0, //旋转角度相关的参数
            false, false, 0, 0, //与偏移相关的参数(到离指定面指定的距离)            
            (int)swThinWallType_e.swThinWallOneDirection, 0, 0, //与薄壁类型相关的参数
            true, false, true//与多实体相关的参数
        );
        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
            //特征类型：RevCut，特征名称：Cut-Revolve1
        }
        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 扫描切除
    /// </summary>
    public void SweepCut()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        swModel.ClearSelection2(true);
        swModelDocExt.SelectByID2("", "FACE", 0, 0, 2, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(0, 1, 0, 0.2);
        swSketchMgr.InsertSketch(true);

        swModel.ClearSelection2(true);
        //swModelDocExt.SelectByID2("", "FACE", 0, 1, 1, false, 0, null, 0);
        swModelDocExt.SelectByRay(0, 2, 1, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);

        swSketchMgr.InsertSketch(true);
        double[] points = new double[12];
        points[0] = 0;
        points[1] = 0;
        points[3] = 0.2;
        points[4] = -0.5;
        points[6] = -0.2;
        points[7] = -1.5;
        points[9] = 0;
        points[10] = -2;
        swSketchMgr.CreateSpline3(points, null, null, true, out _);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);


        //选择扫描草图轮廓，标记为1
        swModelDocExt.SelectByID2("", "SKETCH", 0.2, 1, 2, false, 1, null, 0);
        //选择扫描草图路径，标记为4
        swModelDocExt.SelectByID2("", "SKETCH", 0, 1, 0, true, 4, null, 0);

        var swSweep = (SweepFeatureData)swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweepCut);
        var swFeat = swFeatureMgr.CreateFeature(swSweep);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 放样切除
    /// </summary>
    public void LoftCut()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        swModel.ClearSelection2(true);
        //正面
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCenterRectangle(0, 0, 0, 0.7, 0.7, 0);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);

        //背面
        swModelDocExt.SelectByRay(0, 0, -2, 0, 0, 1, 0.001, (int)swSelectType_e.swSelFACES, false, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(0, 0, 0, 0.5);
        swSketchMgr.InsertSketch(true);
        swModel.ClearSelection2(true);

        //选择轮廓，标记为1，选择顺序与放样方向一致
        swModelDocExt.SelectByID2("", "SKETCH", 0.5, 0, 0, false, 1, null, 0);
        swModelDocExt.SelectByID2("", "SKETCH", 0.7, 0.7, 2, true, 1, null, 0);

        var swFeat = swFeatureMgr.InsertCutBlend(false, true, true, 0.01, 0, 0, false, 0, 0, 0, true, true);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    /// <summary>
    /// 异型孔向导
    /// </summary>
    public void HoleWizard()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        swSketchMgr.InsertSketch(true);

        swSketchMgr.CreateCenterRectangle(0, 0, 0, 10d/1000d, 5d/1000d, 0);

        swFeatureMgr.FeatureExtrusion2(true, false, false, 0, 0, 50d/1000d, 0, false, false, false, false, 0, 0, false, false, false, false, true, true, true, 0, 0, false);

        swModelDocExt.SelectByRay(0, 10d/1000d, 30d/1000d, 0,
            -1, 0, 0.001, 2, false, 0, 0);

        var swHoleFeature = swFeatureMgr.HoleWizard5((int)swWzdGeneralHoleTypes_e.swWzdCounterBore,//0,孔或槽的类型，这里是柱形沉头孔
            (int)swWzdHoleStandards_e.swStandardISO,//8，使用的标准
            (int)swWzdHoleStandardFastenerTypes_e.swStandardISOSocketHeadCap,//139，紧固件类型
            "M6",//孔规格大小
            (int)swEndConditions_e.swEndCondThroughAll, //终止条件，完全贯穿
            0.0066,//孔直径
            7.86674288964986E-03,//孔深度
            -1, //槽长度，这里不适用，赋值-1
            0.011,//沉头直径
            0.0064,//沉头深度
            0, //头部间隙
            1, //螺钉套合，1正常
            0, //底部钻孔角度，贯穿时不适用
            0.01105, //近端锥孔直径
            1.5707963267949, //近端锥孔角度90度
            0,//螺钉下锥孔直径
            0, //螺钉下锥孔角度
            0,//远端锥孔直径
            0,//远端锥孔角度
            0, //偏移量，这里不适用
            "",//螺纹线等级，不适用
            false,//方向，不反转
            true,//特征影响选定实体
            true, //自动选择影响实体
            true,//装配体特征影响零部件
            true,//自动选择零部件
                 false);//装配体特征传播特征到零部件

        var swSketchFeature = (Feature)swHoleFeature.GetFirstSubFeature();
        swSketchFeature.Select2(false, 0);
        swModel.EditSketch();
        var swSelectionManager = (SelectionMgr)swModel.SelectionManager;
        var swSketch = (Sketch)swSketchFeature.GetSpecificFeature2();

        var swSketchPointArray = swSketch.GetSketchPoints2() as object[];
        for (int i = 0; i < swSketchPointArray.Length; i++)
        {
            swSelectionManager.AddSelectionListObject(swSketchPointArray[i], null);
            swModel.EditDelete();
        }
        //实际使用中，可以通过参数控制位置坐标
        swSketchMgr.CreatePoint(0, -10d / 1000d, 0);
        swSketchMgr.CreatePoint(0, -25d / 1000d, 0);
        swSketchMgr.CreatePoint(0, -40d / 1000d, 0);

        swSketchMgr.InsertSketch(true);



        if (swHoleFeature != null)
        {
            Console.WriteLine($"特征类型：{swHoleFeature.GetTypeName2()}，特征名称：{swHoleFeature.Name}");
            //特征类型：HoleWzd，特征名称：M6 六角凹头螺钉的柱形沉头孔1
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();

    }

    public void FaceFillet()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;


        //使用简单的圆角类型
        var swFilletData = (SimpleFilletFeatureData2
            )swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillet);
        //初始化圆角特征数据对象，使用面圆角
        swFilletData.Initialize((int)swSimpleFilletType_e.swFaceFillet);

        swModel.ClearSelection2(true);
        //正面，标记为2
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, (int)swSelectType_e.swSelFACES, false, 2, 0);
        //顶面，标记为4
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 4, 0);

        swFilletData.AsymmetricFillet = true;
        swFilletData.DefaultRadius = 0.1d;
        swFilletData.DefaultDistance = 0.2d;
        //指定特征圆角轮廓类型
        swFilletData.ConicTypeForCrossSectionProfile = (int)swFeatureFilletProfileType_e.swFeatureFilletCircular;


        var swFeat = swFeatureMgr.CreateFeature(swFilletData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

    }

    public void ConstRadiusFillet()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        var swSelMgr = (SelectionMgr)swModel.SelectionManager;

        //使用简单的圆角类型
        var swFilletData = (SimpleFilletFeatureData2
            )swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillet);
        //初始化圆角特征数据对象，使用固定大小圆角
        swFilletData.Initialize((int)swSimpleFilletType_e.swConstRadiusFillet);

        swModel.ClearSelection2(true);

        //Call IModelDocExtension::SelectByID2 with Mark = 1 to select the edges, faces, features, or loops to fillet. 
        //选择边，标记为1
        swModelDocExt.SelectByRay(0, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, false, 1, 0);
        //其实可以不用对edges属性赋值
        //var swEdge = swSelMgr.GetSelectedObject6(1, 1);
        //var edges = new [] { swEdge };
        //swFilletData.Edges=edges;

        //选择面
        //swModelDocExt.SelectByRay(0, 3, 1, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 1, 0);


        //选择特征
        //var extrudeFeat = (Feature)swModel.FeatureByPositionReverse(0);//选择特征树中的最后一个特征
        //extrudeFeat.Select2(false, 1);


        swFilletData.AsymmetricFillet = false;
        swFilletData.DefaultRadius = 0.1d;
        //指定特征圆角轮廓类型
        swFilletData.ConicTypeForCrossSectionProfile = (int)swFeatureFilletProfileType_e.swFeatureFilletCircular;


        var swFeat = swFeatureMgr.CreateFeature(swFilletData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }
    }

    public void FullRoundFillet()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        var swSelMgr = (SelectionMgr)swModel.SelectionManager;

        //使用简单的圆角类型
        var swFilletData = (SimpleFilletFeatureData2
            )swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmFillet);
        //初始化圆角特征数据对象，使用完整圆角
        swFilletData.Initialize((int)swSimpleFilletType_e.swFullRoundFillet);

        swModel.ClearSelection2(true);
        //左侧面，标记为2
        swModelDocExt.SelectByRay(-3, 0, 1, 1, 0, 0, 0.001, (int)swSelectType_e.swSelFACES, false, 2, 0);
        //正面，标记为512
        swModelDocExt.SelectByRay(0, 0, 3, 0, 0, -1, 0.001, (int)swSelectType_e.swSelFACES, true, 512, 0);
        //右侧面，标记为4
        swModelDocExt.SelectByRay(3, 0, 1, -1, 0, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 4, 0);

        //设置传播到相切面
        swFilletData.PropagateToTangentFaces = true;


        var swFeat = swFeatureMgr.CreateFeature(swFilletData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

    }

    public void Chamfer()
    {
        FeatureExtrusion();

        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        var swSelMgr = (SelectionMgr)swModel.SelectionManager;
        swModel.ClearSelection2(true);

        swModelDocExt.SelectByRay(0, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, false, 0, 0);

        //AngleDistance
        var swFeat = swFeatureMgr.InsertFeatureChamfer(
            (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,
            (int)swChamferType_e.swChamferAngleDistance,
            0.1, Math.PI/4d, 0,
            0, 0, 0);

        //DistanceDistance
        //var swFeat = swFeatureMgr.InsertFeatureChamfer(
        //    (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,
        //    (int)swChamferType_e.swChamferDistanceDistance,
        //    0.1, 0, 0.2,
        //    0, 0, 0);

        //EqualDistance,无效选项,实在没办法使用swChamferDistanceDistance
        //var swFeat = swFeatureMgr.InsertFeatureChamfer(
        //          (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,
        //          (int)swChamferType_e.swChamferEqualDistance,
        //          0, 0, 0.2,
        //          0.2, 0, 0);
        //var swFeat = swFeatureMgr.InsertFeatureChamfer(
        //    (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,
        //    (int)swChamferType_e.swChamferDistanceDistance,
        //    0.2, 0, 0.2,
        //    0, 0, 0);



        //Vertex
        //swModelDocExt.SelectByRay(1, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelVERTICES, false, 0, 0);
        //var swFeat = swFeatureMgr.InsertFeatureChamfer(
        //          (int)swFeatureChamferOption_e.swFeatureChamferTangentPropagation,
        //          (int)swChamferType_e.swChamferVertex,
        //          0, 0, 0,
        //          0.1, 0.2, 0.3);


        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }
    }

    public void Mirror()
    {
        FeatureExtrusion();
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        var swSelMgr = (SelectionMgr)swModel.SelectionManager;
        swModel.ClearSelection2(true);
        //选择顶面，再做一个拉伸特征
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(-0.5d, -1.5d, 0, 0.4d);
        var extrudeFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
            (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
            0.5, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
            false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
            true, false, true, //多实体合并结果
            (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距

        swModel.ClearSelection2(true);
        //要镜像的特征，标记为1
        extrudeFeat.Select2(false, 1);
        //镜像面，标记为2
        swModelDocExt.SelectByID2("Right Plane", "PLANE", 0, 0, 0, true, 2, null, 0);
        var swFeat = swFeatureMgr.InsertMirrorFeature2(
            false,//取值true是镜像实体，取值false是镜像特征或面
            false, //取值true表示只镜像几何体特征，取值false表示求解所有特征，仅适用于镜像特征
            false, //合并所有镜像实体，仅适用于镜像实体
            false,//knit表面，仅适用于镜像面
            (int)swFeatureScope_e.swFeatureScope_AllBodies);//受镜像特征影响的实体

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    public void LinearPattern()
    {
        FeatureExtrusion();
        var swModel = (ModelDoc2)_swApp.ActiveDoc;
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;
        var swSelMgr = (SelectionMgr)swModel.SelectionManager;
        swModel.ClearSelection2(true);
        //选择顶面，再做一个拉伸特征
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(-0.5d, -1.5d, 0, 0.4d);
        var extrudeFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
            (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
            0.5, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
            false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
            true, false, true, //多实体合并结果
            (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距

        swModel.ClearSelection2(true);
        //Select feature to pattern
        extrudeFeat.Select2(false, 4);
        //Select edges for Direction 1 and Direction 2
        swModelDocExt.SelectByRay(0, 3, 2, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, true, 1, 0);
        swModelDocExt.SelectByRay(-1, 3, 1, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, true, 2, 0);

        //Create linear pattern
        var swLinearPatternFeatureData = (LinearPatternFeatureData
            )swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmLPattern);

        swLinearPatternFeatureData.D1EndCondition = 0;//阵列的方式，有间距与实例数和到参考
        swLinearPatternFeatureData.D1ReverseDirection = true;//反向
        swLinearPatternFeatureData.D1Spacing = 1d;//间距，
        swLinearPatternFeatureData.D1TotalInstances = 2;//实例数

        swLinearPatternFeatureData.D2EndCondition = 0;
        swLinearPatternFeatureData.D2PatternSeedOnly = false;//方向2，只阵列源
        swLinearPatternFeatureData.D2ReverseDirection = false;
        swLinearPatternFeatureData.D2Spacing = 1d;
        swLinearPatternFeatureData.D2TotalInstances = 2;

        swLinearPatternFeatureData.GeometryPattern = false;//几何体阵列
        swLinearPatternFeatureData.VarySketch = false;
        var swFeat = swFeatureMgr.CreateFeature(swLinearPatternFeatureData);


        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();

    }

    public void CircularPattern()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModelDocExt.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);

        swSketchMgr.CreateCircleByRadius(0, 0, 0, 1);
        var baseFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
            (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
            1, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
            false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
            true, false, true, //多实体合并结果
            (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距


        swModel.ClearSelection2(true);
        //选择顶面，再做一个拉伸特征
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(-0.7d, 0, 0, 0.2d);
        var extrudeFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
            (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
            0.3, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
            false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
            true, false, true, //多实体合并结果
            (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距

        //被阵列的特征
        extrudeFeat.Select2(false, 4);
        //方向
        swModelDocExt.SelectByRay(0, 3, 1, 0, -1, 0, 0.001, (int)swSelectType_e.swSelEDGES, true, 1, 0);

        var swCircularPatternData = (CircularPatternFeatureData)swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmCirPattern);
        swCircularPatternData.EqualSpacing = true;//等间距
        swCircularPatternData.ReverseDirection = false;//反向
        swCircularPatternData.Spacing = Math.PI*2;//角度
        swCircularPatternData.TotalInstances = 3;//实例数
        swCircularPatternData.Direction2 = false;//方向2
        swCircularPatternData.GeometryPattern = false;//几何体阵列        
        swCircularPatternData.VarySketch = false;

        var swFeat = swFeatureMgr.CreateFeature(swCircularPatternData);


        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

    public void SketchDrivenPattern()
    {
        var swModel = NewDocument();
        var swModelDocExt = swModel.Extension;
        var swSketchMgr = swModel.SketchManager;
        var swFeatureMgr = swModel.FeatureManager;

        swModelDocExt.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        swSketchMgr.InsertSketch(true);

        swSketchMgr.CreateCircleByRadius(0, 0, 0, 1);
        var baseFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
            (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
            100d/1000d, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
            false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
            true, false, true, //多实体合并结果
            (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距
        
        swModel.ClearSelection2(true);
        //选择顶面，再做一个拉伸特征
        swModelDocExt.SelectByRay(0, 1, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreateCircleByRadius(-0.7d, 0, 0, 0.2d);
        var extrudeFeat = swFeatureMgr.FeatureExtrusion3(
            true, false, false,//与拉伸方向有关
            (int)swEndConditions_e.swEndCondBlind, (int)swEndConditions_e.swEndCondBlind, //结束条件相关
            100d/1000d, 0, //拉伸深度
            false, false, false, true, 0, 0, //拔模相关
            false, false, false, false, //结束条件为到离指定面指定的距离，反向等距和转化曲面
            true, false, true, //多实体合并结果
            (int)swStartConditions_e.swStartSketchPlane, 0, false); //拉伸起始条件，等距

        swModel.ClearSelection2(true);
        //选择顶面，绘制一个包含多个点的草图（随意创建，只要在面内上即可），驱动草图
        swModelDocExt.SelectByRay(0, 3, 0, 0, -1, 0, 0.001, (int)swSelectType_e.swSelFACES, true, 0, 0);
        swSketchMgr.InsertSketch(true);
        swSketchMgr.CreatePoint(0, 0, 0);
        swSketchMgr.CreatePoint(-0.2, -0.5, 0);
        swSketchMgr.CreatePoint(0.3, 0.7, 0);
        swSketchMgr.CreatePoint(0.7, -0.2, 0);
        swSketchMgr.InsertSketch(true);


        swModel.ClearSelection2(true);
        //Select feature to pattern，选择特征标记为4
        extrudeFeat.Select2(false, 4);
        //选择草图标记为64
        swModelDocExt.SelectByID2("", "SKETCH", 0, 100d/1000d, 0, true, 64, null, 0);

        var swSketchPatternData = (SketchPatternFeatureData)swFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmSketchPattern);
        swSketchPatternData.GeometryPattern = false;//几何体阵列
        swSketchPatternData.UseCentroid = true;//参考点，重心

        var swFeat = swFeatureMgr.CreateFeature(swSketchPatternData);

        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }

        //轴测图
        swModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        swModel.ViewZoomtofit2();
    }

}
