using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace LearnSolidWorksApi
{
    public class EditSketch
    {
        public ModelDoc2 SwModel { get; set; }
        public ModelDocExtension SwModelDocExt { get; set; }
        public SketchManager SwSketchMgr { get; set; }
        private readonly SldWorks _swApp;
        public EditSketch(SldWorks swApp)
        {
            _swApp = swApp;
        }
        /// <summary>
        /// 获取当前激活的文档
        /// </summary>
        public void ActiveDoc()
        {
            SwModel=(ModelDoc2)_swApp.ActiveDoc;
            SwModelDocExt = SwModel.Extension;
            SwSketchMgr = SwModel.SketchManager;
        }

        public void NewDocument()
        {
            //先确保证设置中默认模板已经设置
            var defaultTemplate =
                _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            //使用默认模板新建零件
            SwModel = (_swApp.NewDocument(defaultTemplate, 0, 0, 0) as ModelDoc2)!;
            SwModelDocExt = SwModel.Extension;
            SwSketchMgr = SwModel.SketchManager;
        }

        /// <summary>
        /// 创建圆角
        /// </summary>
        public void CreateFillet()
        {
            NewDocument();
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
            SwSketchMgr.CreateCornerRectangle(0, 1, 0, 1, 0, 0);
            SwModel.ClearSelection2(true);
            SwModelDocExt.SelectByID2("Point1", "SKETCHPOINT", 0, 1, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.CreateFillet(0.1, (int)swConstrainedCornerAction_e.swConstrainedCornerDeleteGeometry);
            SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 创建草图倒角
        /// </summary>
        public void CreateChamfer()
        {
            NewDocument();
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
            SwSketchMgr.CreateCornerRectangle(0, 1, 0, 1, 0, 0);
            SwModel.ClearSelection2(true);
            //选择两条线，我们先选择上边，再选择左边
            //选择线段时，第二个参数填SKETCHSEGMENT，相应的填写线段上的点的坐标，Mark参数填1
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", -0.9, 1, 0, false, 1, null, (int)swSelectOption_e.swSelectOptionDefault);
            //后选择的边时，注意Append参数为true，表示加选。
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", -1, 0.9, 0, true, 1, null, (int)swSelectOption_e.swSelectOptionDefault);
            //创建草图倒角
            //距离角度，角度需要换算成弧度，注意后选择的边对应D1
            //SwSketchManager.CreateChamfer((int)swSketchChamferType_e.swSketchChamfer_DistanceAngle,0.1,60*Math.PI/180);
            //距离距离，注意距离和我们选择草图线段的顺序是反的，后选择的边对应D1，先选择的边上是D2
            //SwSketchManager.CreateChamfer((int)swSketchChamferType_e.swSketchChamfer_DistanceDistance,0.1,0.2);
            //等距
            SwSketchMgr.CreateChamfer((int)swSketchChamferType_e.swSketchChamfer_DistanceEqual, 0.1, 0.1);

            SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            SwSketchMgr.InsertSketch(true);
        }

        public void SketchOnFrontPlane()
        {
            SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            SwSketchMgr.InsertSketch(true);
        }

        public void ViewZoomAndExitSketch()
        {
            SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            SwModel.ViewZoomtofit2();
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 剪裁草图实体
        /// </summary>
        public void SketchTrim()
        {
            NewDocument();

            //SwModelDocExt.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);
            //SwSketchManager.InsertSketch(true);
            SketchOnFrontPlane();

            SwSketchMgr.CreateLine(-2, 0, 0, 2, 0, 0);
            SwSketchMgr.CreateLine(0, -2, 0, 0, 2, 0);
            SwModel.ClearSelection2(true);

            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 1, 0, 0, false, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 1, 0, true, 0, null,
                (int)swSelectOption_e.swSelectOptionDefault);

            var boolStatus = SwSketchMgr.SketchTrim((int)swSketchTrimChoice_e.swSketchTrimCorner, 0, 0, 0);
            Console.WriteLine(boolStatus);


            //    SwModel.ShowNamedView2("", (int)swStandardViews_e.swFrontView);
            //    SwModel.ViewZoomtofit2();
            //    SwSketchManager.InsertSketch(true);
            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 延伸草图实体
        /// </summary>
        public void SketchExtend()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateLine(-0.5, 1, 0, 0, 0, 0);
            SwSketchMgr.CreateLine(-1, -0.5, 0, 1, -0.5, 0);
            SwModel.ClearSelection2(true);


            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, (int)swSelectOption_e.swSelectOptionDefault);

            var boolStatus = SwSketchMgr.SketchExtend(0, 0, 0);
            Console.WriteLine(boolStatus);

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 等距草图实体
        /// </summary>
        public void SketchOffset()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateLine(-0.5, 0.75, 0, -0.25, -0.5, 0);
            SwSketchMgr.CreateLine(-0.75, -1.25, 0, 0.5, -1.25, 0);
            //与Line1构成链
            SwSketchMgr.CreateLine(-0.5, 0.75, 0, -0.5, 1.5, 0);
            SwModel.ClearSelection2(true);


            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);

            //单侧等距，不标注偏移距离，往左往下
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, false, false, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, false);

            //反向单侧等距，标注偏移距离，往右往上
            var boolStatus = SwSketchMgr.SketchOffset2(-0.5, false, false, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);


            //两侧等距
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);


            //链选项
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, true, (int)swSkOffsetCapEndType_e.swSkOffsetNoCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //CapEnds，选择圆弧封端
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);

            //CapEnds，选择直线封端
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetLineCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetDontMakeConstruction, true);



            //CapEnds，选择圆弧封端，将原始草图线设为构造线
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetMakeOrigConstruction, true);

            //CapEnds，选择圆弧封端，将等距草图线设为构造线
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetMakeOffsConstruction, true);


            //CapEnds，选择圆弧封端，将原始草图线和等距草图线都设为构造线
            //var boolStatus = SwSketchMgr.SketchOffset2(0.5, true, false, (int)swSkOffsetCapEndType_e.swSkOffsetArcCaps, (int)swSkOffsetMakeConstructionType_e.swSkOffsetMakeBothConstruction, true);

            Console.WriteLine(boolStatus);

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 镜像草图实体
        /// </summary>
        public void SketchMirror()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateCenterLine(0, 0, 0, 0, 1, 0);
            SwSketchMgr.CreateCircleByRadius(-0.5, 0.5, 0, 0.4);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 2, null, 0);
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, true, 1, null, 0);


            SwModel.SketchMirror();

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 创建线性草图阵列
        /// </summary>
        public void CreateLinearSketchPattern()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            //SwSketchMgr.CreateLine(-0.2, 0, 0, 0.2, 0, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);

            var boolStatus =
                SwSketchMgr.CreateLinearSketchStepAndRepeat(3, 1, 1, 0, 0, 0, "", true, false, false, true, false);

            ViewZoomAndExitSketch();

            /*
            // Select an entity in the linear sketch seed pattern and open the linear sketch pattern to edit 
            boolstatus = swModelDocExt.SelectByID2("Line3@Sketch2", "EXTSKETCHSEGMENT", -0.002651338304644, -0.007221297464333, 0, false, 0, null, 0);
            swModel.EditSketch();

            // Delete the Line3 sketch entity from each instance of the linear sketch pattern 
            boolstatus = swSketchMgr.EditLinearSketchStepAndRepeat(3, 2, 0.0254, 0.0254, 0.296705972839, 1.134464013796, "", false, false, false, true, true, "Line2_Line1_Line4_");
            */
        }

        /// <summary>
        /// 编辑线性草图阵列
        /// </summary>
        public void EditLinearSketchPattern()
        {
            CreateLinearSketchPattern();
            SwModelDocExt.SelectByID2("Arc1@Sketch1", "EXTSKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModel.EditSketch();

            //种子名阵列中的下划线是必须的
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 1, 1, 0, 0, 0, "", true, false, false, true, false, "Arc1");

            //增加沿x轴阵列数量
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 1, 1, 0, 0, 0, "", true, false, false, true, false, "Arc1_");

            //增加沿y轴阵列数量，同时给定沿y轴阵列的距离
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 0, 0, "", true, false, false, true, false, "Arc1_");
            //我们发现，实例数在x轴方向又增加了，但是没有沿着y轴阵列，为什么呢？
            //原因是，我们没有指定Angle y参数的值。

            //指定Angle y参数的值时，注意需要转换为弧度。由于指定了沿y方向阵列，那么应当显示阵列距离标注、阵列数量标注和两个阵列方向的夹角标注。
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 0, 60*Math.PI/180, "", true, true, true, true, true, "Arc1_");

            //我们发现，Angle y参数并不是阵列方向2与y轴的夹角，而是阵列方向2与x轴的夹角。
            //我们可以继续修改Angle x和Angle y的参数来观察结果。
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 30*Math.PI/180, 90*Math.PI/180, "", true, true, true, true, true, "Arc1_");
            //我们发现距离和数量并不是真的沿x和y轴，而是沿阵列方向1和阵列方向2标注的。
            //API中对参数的解释都使用沿x和y轴，虽然有利于我们理解，但是我们应该清楚实际情况并非如此。

            //删除实例
            //我们把线性阵列看成一个二维矩阵
            //当我们要删除的实例处于x轴方向第3个，y轴方向第2个，那么使用（括号3逗号2括号）(3,2)表示。

            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 30*Math.PI/180, 90*Math.PI/180, "(3,2)", true, true, true, true, true, "Arc1_");
            //在绘图界面中，我们观察到第3列，第2行的实例被删除了。

            //如果要删除多个实例，则继续在后面接上需要删除的实例，无需使用符号分隔
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 30*Math.PI/180, 90*Math.PI/180, "(3,2)(5,4)", true, true, true, true, true, "Arc1_");

            //最后我们来看多个草图实体作为种子的情况
            //我们首先应该在创建草图阵列，多绘制一个草图图元。
            //回到昨天的案例中，我们绘制一条直线，并且加选再创建草图阵列
            //在API中，我们看到多个图元的名字使用下划线分隔，但是最后一个没有以下划线结尾。
            //var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 30*Math.PI/180, 90*Math.PI/180, "(3,2)(5,4)", true, true, true, true, true, "Arc1_Line1");

            //我们发现，这样会让修改线性草图阵列时漏选直线
            //该参数的解释应该为每个草图图元名后面都比必须带一个下划线_
            var boolStatus = SwSketchMgr.EditLinearSketchStepAndRepeat(5, 4, 1, 0.5, 30*Math.PI/180, 90*Math.PI/180, "(3,2)(5,4)", true, true, true, true, true, "Arc1_Line1_");


            Console.WriteLine($"编辑线性草图阵列：{boolStatus}");
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 创建圆周草图阵列
        /// </summary>
        public void CreateCircularSketchPattern()
        {
            NewDocument();
            SketchOnFrontPlane();

            SwSketchMgr.CreateCircleByRadius(-1, 0, 0, 0.1);
            SwSketchMgr.CreateLine(-1, -0.1, 0, -1, 0.1, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);

            var boolStatus =
                SwSketchMgr.CreateCircularSketchStepAndRepeat(0.5, 0, 3, 30 * Math.PI / 180, false, "", true, true, true);
            Console.WriteLine($"创建圆周阵列：{boolStatus}");


            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 编辑圆周草图阵列
        /// </summary>
        public void EditCircularSketchPattern()
        {
            CreateCircularSketchPattern();
            SwModelDocExt.SelectByID2("Arc1@Sketch1", "EXTSKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModel.EditSketch();

            //更改圆周阵列半径
            //var boolStatus =SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 3, 30 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改相对于被阵列草图实体的角度
            //var boolStatus =SwSketchMgr.EditCircularSketchStepAndRepeat(1, 10 * Math.PI / 180, 3, 30 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改圆周阵列实例数量
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 5, 30 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改圆周阵列实例间距
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 5, 45 * Math.PI / 180, false, "", true, true, true, "Arc1_Line1_");

            //更改是否旋转阵列图案
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 5, 45 * Math.PI / 180, true, "", true, true, true, "Arc1_Line1_");
            //并没有发现什么实质性的变化

            //删除阵列实例
            //var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 5, 45 * Math.PI / 180, false, "(3)", true, true, true, "Arc1_Line1_");


            //不显示阵列半径，角度间距，阵列数量标注
            var boolStatus = SwSketchMgr.EditCircularSketchStepAndRepeat(1, 0, 5, 45 * Math.PI / 180, false, "(3)", false, false, false, "Arc1_Line1_");


            Console.WriteLine($"编辑圆周草图阵列：{boolStatus}");
            SwSketchMgr.InsertSketch(true);
        }

        /// <summary>
        /// 移动复制草图实体
        /// </summary>
        public void MoveOrCopy()
        {
            NewDocument();
            SketchOnFrontPlane();
            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);

            //移动草图实体
            //SwModelDocExt.MoveOrCopy(false,1,false,0,0,0,1,1,0);
            //SwModelDocExt.MoveOrCopy(false,1,true,0,0,0,1,1,0);
            //SwModelDocExt.MoveOrCopy(false,2,false,0,0,0,1,1,0);


            //移动复制草图实体
            SwModelDocExt.MoveOrCopy(true, 1, true, 0, 0, 0, 1, 1, 0);

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 旋转复制草图实体
        /// </summary>
        public void RotateOrCopy()
        {
            NewDocument();
            SketchOnFrontPlane();
            SwSketchMgr.CreateCornerRectangle(0, 0, 0, 0.2, 0.2, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            SwModelDocExt.SelectByID2("Line3", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            SwModelDocExt.SelectByID2("Line4", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);

            //旋转草图实体,关于z轴旋转(逆时针)，给负值是顺时针
            //旋转基点(0,0,0)
            //不保持约束
            //SwModelDocExt.RotateOrCopy(false,1,false,0,0,0,0,0,1,45*Math.PI/180);

            //旋转基点(0.2,0,0)
            //SwModelDocExt.RotateOrCopy(false,1,false,0.2,0,0,0,0,1,45*Math.PI/180);


            //旋转复制草图实体
            //SwModelDocExt.RotateOrCopy(true, 1, false, 0.2, 0, 0, 0, 0, 1, 45*Math.PI/180);

            //旋转复制2ge
            //SwModelDocExt.RotateOrCopy(true, 2, false, 0.2, 0, 0, 0, 0, 1, 45*Math.PI/180);

            //保留草图约束
            SwModelDocExt.RotateOrCopy(true, 2, true, 0.2, 0, 0, 0, 0, 1, 45*Math.PI/180);

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 显示隐藏草图约束
        /// </summary>
        public void ViewSketchRelations()
        {
            //获取当前活动的文档
            ActiveDoc();
            //true显示/false隐藏草图约束，第二个参数swDetailingNoOptionSpecified
            var boolStatus = SwModelDocExt.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swViewSketchRelations, (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified, true);
            Console.WriteLine(boolStatus);
        }

        /// <summary>
        /// 添加草图约束关系
        /// </summary>
        public void SketchAddConstraints()
        {
            NewDocument();
            SketchOnFrontPlane();
            SwSketchMgr.CreateLine(0.5, 0.5, 0, 1, 1, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            #region 固定,水平,竖直
            //SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);

            //sgFIXED，固定约束
            //SwModel.SketchAddConstraints("sgFIXED");
            //sgHORIZONTAL2D，水平约束
            //SwModel.SketchAddConstraints("sgHORIZONTAL2D");
            //sgVERTICAL2D，竖直约束
            //SwModel.SketchAddConstraints("sgVERTICAL2D"); 
            #endregion


            #region 重合,中点
            //SwModelDocExt.SelectByID2("Point1@Origin", "EXTSKETCHPOINT", 0, 0, 0, false, 0, null, 0);

            //sgCOINCIDENT，重合约束
            //SwModelDocExt.SelectByID2("Point2", "SKETCHPOINT", 0, 0, 0, true, 0, null, 0);
            //SwModel.SketchAddConstraints("sgCOINCIDENT");
            //sgATMIDDLE，中点约束
            //SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            //SwModel.SketchAddConstraints("sgATMIDDLE"); 
            #endregion


            #region 共线，平行，垂直，等长
            //SwSketchMgr.CreateLine(0, 0, 0, 0.4, 0.2, 0);
            //SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //SwModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            //sgCOLINEAR，共线约束
            //SwModel.SketchAddConstraints("sgCOLINEAR");
            //sgPARALLEL，平行约束
            //SwModel.SketchAddConstraints("sgPARALLEL");
            //sgPERPENDICULAR，垂直约束
            //SwModel.SketchAddConstraints("sgPERPENDICULAR"); 
            //sgSAMELENGTH，等长约束
            //SwModel.SketchAddConstraints("sgSAMELENGTH");
            #endregion

            #region 相切，相同曲线长度
            //SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            //SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            //sgTANGENT，相切约束
            //SwModel.SketchAddConstraints("sgTANGENT");
            //sgSAMECURVELENGTH，相同曲线长度约束
            //SwModel.SketchAddConstraints("sgSAMECURVELENGTH");
            #endregion

            #region 同心，同心共径
            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwSketchMgr.CreateCircleByRadius(1, 0, 0, 0.4);
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwModelDocExt.SelectByID2("Arc2", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            //sgCONCENTRIC，同心约束
            //SwModel.SketchAddConstraints("sgCONCENTRIC");
            //sgCORADIAL，同心共径
            SwModel.SketchAddConstraints("sgCORADIAL");
            #endregion

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 标注草图尺寸
        /// </summary>
        public void AddDimension()
        {
            NewDocument();
            SketchOnFrontPlane();

            //关闭输入尺寸
            _swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);

            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();


            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //SwModel.AddDimension2(0, 0, 0);
            //修改位置观察
            //SwModel.AddDimension2(0.2,0.2, 0);


            //修改参数观察
            SwModelDocExt.AddDimension(0, 0, 0, (int)swSmartDimensionDirection_e.swSmartDimensionDirection_Up);
            
            
            //打开输入尺寸
            _swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, true);
            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 转换为构造线
        /// </summary>
        public void CreateConstructionGeometry()
        {
            NewDocument();
            SketchOnFrontPlane();
            SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();
            
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwSketchMgr.CreateConstructionGeometry();

            SwModel.ClearSelection2(true);
            SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            SwSketchMgr.CreateConstructionGeometry();

            ViewZoomAndExitSketch();
        }

        /// <summary>
        /// 拆分草图实体
        /// </summary>
        public void SplitSegment()
        {
            NewDocument();
            SketchOnFrontPlane();
            //关闭草图捕捉
            _swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchInference, false);

            //SwSketchMgr.CreateCircleByRadius(0, 0, 0, 0.2);
            //SwModel.ClearSelection2(true);
            //SwModel.ViewZoomtofit2();

            //SwModelDocExt.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //拆分封闭的草图
            //SwSketchMgr.SplitClosedSegment(-0.2,0,0,0.2,0,0);

            SwSketchMgr.CreateLine(0, 0, 0, 0.2, 0, 0);
            SwModel.ClearSelection2(true);
            SwModel.ViewZoomtofit2();

            SwModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            //拆分非封闭的草图
            SwSketchMgr.SplitOpenSegment(0.1, 0, 0);

            //打开草图捕捉
            _swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchInference, true);
            ViewZoomAndExitSketch();
        }


    }
}
