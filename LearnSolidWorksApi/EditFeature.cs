using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi;

public class EditFeature
{
    public ModelDoc2 SwModel { get; set; }
    public ModelDocExtension SwModelDocExt { get; set; }
    public SketchManager SwSketchMgr { get; set; }
    public FeatureManager SwFeatureMgr { get; set; }
    private readonly SldWorks _swApp;
    public EditFeature(SldWorks swApp)
    {
        _swApp = swApp;
    }
    /// <summary>
    /// 获取当前激活的文档
    /// </summary>
    public void ActiveDoc()
    {
        SwModel=(ModelDoc2)_swApp.ActiveDoc;
    }
    public void SetProperties()
    {        
        SwModelDocExt = SwModel.Extension;
        SwSketchMgr = SwModel.SketchManager;
        SwFeatureMgr = SwModel.FeatureManager;
    }

    /// <summary>
    /// 新建零件文档
    /// </summary>
    public void NewDocument()
    {
        //先确保证设置中默认模板已经设置
        var defaultTemplate =
            _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
        //使用默认模板新建零件
        SwModel = (_swApp.NewDocument(defaultTemplate, 0, 0, 0) as ModelDoc2)!;   
    }

    /// <summary>
    /// 在前视基准面上插入草图
    /// </summary>
    public void SketchOnFrontPlane()
    {
        SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        SwSketchMgr.InsertSketch(true);
    }
    /// <summary>
    /// 在上视基准面上插入草图
    /// </summary>
    public void SketchOnTopPlane()
    {
        SwModelDocExt.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        SwSketchMgr.InsertSketch(true);
    }
    /// <summary>
    /// 在右视基准面上插入草图
    /// </summary>
    public void SketchOnRightPlane()
    {
        SwModelDocExt.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
        SwSketchMgr.InsertSketch(true);
    }

    /// <summary>
    /// 调整视图并退出草图编辑
    /// </summary>
    public void ViewZoomAndExitSketch()
    {
        SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
        SwModel.ViewZoomtofit2();
        SwSketchMgr.InsertSketch(true);
    }

    /// <summary>
    /// 拉伸特征
    /// </summary>
    public void FeatureExtrusion()
    {
        NewDocument();
        SetProperties();
        SketchOnFrontPlane();
        SwSketchMgr.CreateCenterRectangle(0, 0, 0, 1, 1, 0);
        ViewZoomAndExitSketch();

        //由于退出草图编辑时草图默认处于选中状态
        //为了让我们更明白，拉伸之前我们选择了草图，故意清除选择并再次选择草图，然后再创建拉伸特征
        SwModel.ClearSelection2(true);
        SwModelDocExt.SelectByID2("", "SKETCH", 0, 1, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

        var swFeat = SwFeatureMgr.FeatureExtrusion3(
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
        }
    }

    /// <summary>
    /// 旋转特征
    /// </summary>
    public void FeatureRevolve()
    {
        NewDocument();
        SketchOnFrontPlane();
        //中心线
        SwSketchMgr.CreateCenterLine(0, 0, 0, 0, 2, 0);
        //旋转草图
        SwSketchMgr.CreateCircleByRadius(-2, 1, 0, 0.5);
        ViewZoomAndExitSketch();

        SwModel.ClearSelection2(true);
        //根据坐标选择中心线，标记为4
        SwModelDocExt.SelectByID2("", "EXTSKETCHSEGMENT", 0, 1, 0, false, 4, null, 0);
        //根据坐标选择草图圆，加选，标记为0
        SwModelDocExt.SelectByID2("", "EXTSKETCHSEGMENT", -1.5, 1, 0, true, 0, null, 0);

        var swFeat = SwFeatureMgr.FeatureRevolve2(true, true, false, false, false, false, (int)swEndConditions_e.swEndCondBlind,
            (int)swEndConditions_e.swEndCondBlind, 360 * Math.PI / 180, 0, false, false, 0, 0,
            (int)swThinWallType_e.swThinWallOneDirection, 0, 0, true, false, true);
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
        NewDocument();
        SketchOnTopPlane();
        SwSketchMgr.CreateEllipse(0, 0, 0, 0.5, 0, 0, 0, 1, 0);
        SwSketchMgr.InsertSketch(true);
        SwModel.ClearSelection2(true);

        SketchOnFrontPlane();
        SwSketchMgr.CreateArc(-2,0,0,0,0,0,-2,2,0,1);
        ViewZoomAndExitSketch();
        SwModel.ClearSelection2(true);

        //选择扫描轮廓，标记为1
        SwModelDocExt.SelectByID2("", "SKETCH", 0.5, 0, 0, false, 1, null, 0);
        //选择扫描路径，标记为4
        SwModelDocExt.SelectByID2("", "SKETCH", -2, 2, 0, true, 4, null, 0);
        
        var swSweep = (SweepFeatureData)SwFeatureMgr.CreateDefinition((int)swFeatureNameID_e.swFmSweep);
        var swFeat = SwFeatureMgr.CreateFeature(swSweep);
        
        if (swFeat != null)
        {
            Console.WriteLine($"特征类型：{swFeat.GetTypeName2()}，特征名称：{swFeat.Name}");
        }
    }

    /// <summary>
    /// 放样特征
    /// </summary>
    public void InsertProtrusionBlend()
    {
        NewDocument();
        //绘制第一个轮廓
        SketchOnFrontPlane();
        SwSketchMgr.CreateEllipse(0, 0, 0, 0.07, 0, 0, 0, 0.03, 0);
        SwSketchMgr.InsertSketch(true);

        //插入新的参考基准面，绘制第二个轮廓
        SwModel.ClearSelection2(true);
        SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        //todo:待解释InsertRefPlane
        SwFeatureMgr.InsertRefPlane(8, 0.07, 0, 0, 0, 0);
        SwModel.ClearSelection2(true);
        
        SwModelDocExt.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);
        SwSketchMgr.InsertSketch(true);
        SwSketchMgr.CreateEllipse(0, 0, 0, 0.05, 0, 0, 0, 0.01, 0);
        SwSketchMgr.InsertSketch(true);


        //插入新的参考基准面，绘制第三个轮廓
        SwModel.ClearSelection2(true);
        SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        //todo:待解释InsertRefPlane
        SwFeatureMgr.InsertRefPlane(8, 0.14, 0, 0, 0, 0);
        SwModel.ClearSelection2(true);

        SwModelDocExt.SelectByID2("Plane2", "PLANE", 0, 0, 0, false, 0, null, 0);
        SwSketchMgr.InsertSketch(true);
        SwSketchMgr.CreateEllipse(0, 0, 0, 0.06, 0, 0, 0, 0.02, 0);
        SwSketchMgr.InsertSketch(true);


        //轴测图
        SwModel.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
        SwModel.ViewZoomtofit2();


        //创建引导曲线
        SwModel.ClearSelection2(true);
        SwModelDocExt.SelectByID2("Point1@Sketch1", "EXTSKETCHPOINT", 0.07, 0, 0, false, 1, null, 0);
        SwModelDocExt.SelectByID2("Point1@Sketch2", "EXTSKETCHPOINT", 0.05, 0, 0, true, 1, null, 0);
        SwModelDocExt.SelectByID2("Point1@Sketch3", "EXTSKETCHPOINT", 0.06, 0, 0, true, 1, null, 0);
        SwModel.Insert3DSplineCurve(false);
        SwModel.ClearSelection2(true);
        SwModelDocExt.SelectByID2("Point3@Sketch1", "EXTSKETCHPOINT", -0.07, 0, 0, false, 1, null, 0);
        SwModelDocExt.SelectByID2("Point3@Sketch2", "EXTSKETCHPOINT", -0.05, 0, 0, true, 1, null, 0);
        SwModelDocExt.SelectByID2("Point3@Sketch3", "EXTSKETCHPOINT", -0.06, 0, 0, true, 1, null, 0);
        SwModel.Insert3DSplineCurve(false);

        //选择轮廓，标记为1，选择顺序与放样方向一致
        SwModelDocExt.SelectByID2("Sketch1", "SKETCH", 0.07, 0, 0, false, 1, null, 0);
        SwModelDocExt.SelectByID2("Sketch2", "SKETCH", 0.05, 0, 0.07, true, 1, null, 0);
        SwModelDocExt.SelectByID2("Sketch3", "SKETCH", 0.05, 0, 0.07, true, 1, null, 0);

        //选择引导线，标记为2
        SwModelDocExt.SelectByID2("Curve1", "REFERENCECURVES", 0, 0, 0, true, 2, null, 0);
        SwModelDocExt.SelectByID2("Curve2", "REFERENCECURVES", 0, 0, 0, true, 2, null, 0);
        
        //创建放样特征
        SwFeatureMgr.InsertProtrusionBlend(false, true, false, 1, 0, 0, 1, 1, true, true, false, 0, 0, 0, true, true, true);
    }

}
