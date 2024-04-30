using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Shape_ChartBasic : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Shape_AddGraph class.
        /// </summary>
        public GH_Pdf_Shape_ChartBasic()
          : base("Basic Chart Shape", "Cht Shp",
              "Create a Bar, Column, Line, or Area Chart Shape within a rectangular boundary.",
              Constants.ShortName, Constants.Shapes)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("DataSet", "Ds", "Chart Data Set to visualize", GH_ParamAccess.list);
            pManager.AddRectangleParameter("Boundary", "B", "The rectangular boundary of the Shape", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Type", "T", "The chart format to be displayed", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddTextParameter("X Axis", "X", "Optional X Axis title for the Graph", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("Y Axis", "Y", "Optional Y Axis title for the Graph", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddIntegerParameter("Grid Lines", "G", "Optional background grid lines", GH_ParamAccess.item, 0);
            pManager[5].Optional = true;
            pManager.AddIntegerParameter("Legend Location", "L", "Optional Legend location", GH_ParamAccess.item, 0);
            pManager[6].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[2];
            paramA.AddNamedValue("Column", 0);
            paramA.AddNamedValue("ColumnStacked", 1);
            paramA.AddNamedValue("Bar", 2);
            paramA.AddNamedValue("BarStacked", 3);
            paramA.AddNamedValue("Line", 4);
            paramA.AddNamedValue("Area", 5);

            Param_Integer paramB = (Param_Integer)pManager[5];
            paramB.AddNamedValue("None", 0);
            paramB.AddNamedValue("Horizontal", 1);
            paramB.AddNamedValue("Vertial", 2);
            paramB.AddNamedValue("Horizontal / Vertical", 3);

            Param_Integer paramC = (Param_Integer)pManager[6];
            foreach (Alignment value in Enum.GetValues(typeof(Alignment)))
            {
                paramC.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<DataSet> data = new List<DataSet>();
            if (!DA.GetDataList(0, data)) return;

            Rectangle3d boundary = new Rectangle3d(Plane.WorldXY,300,200);
            DA.GetData(1, ref boundary);

            int type = 4;
            DA.GetData(2, ref type);

            Shape shape = Shape.CreateChart(data, (Shape.ChartTypes)type, boundary);

            string x = string.Empty;
            if (DA.GetData(3, ref x))shape.XAxis = x;

            string y = string.Empty;
            if (DA.GetData(4, ref y)) shape.YAxis = y;

            int grid = 0;
            DA.GetData(5, ref grid);
            if ((grid == 1) | (grid == 3)) shape.HorizontalBorderStyle = Element.BorderStyles.All;
            if (grid > 1) shape.VerticalBorderStyle = Element.BorderStyles.All;

            int alignment = 0;
            if (DA.GetData(6, ref alignment)) shape.Alignment = (Alignment)alignment;

            //prev_shapes.Add(shape);
            DA.SetData(0, shape);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Pdf_Shape_Chart_Basic;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("65bac445-61d4-472f-8855-e899fc5f5477"); }
        }
    }
}