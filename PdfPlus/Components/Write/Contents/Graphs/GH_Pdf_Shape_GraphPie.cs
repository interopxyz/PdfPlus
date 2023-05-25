using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Shape_GraphPie : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Shape_GraphPie class.
        /// </summary>
        public GH_Pdf_Shape_GraphPie()
          : base("Add Pie Chart", "Pie",
              "Create a Chart Shape within a rectangular boundary",
              Constants.ShortName, Constants.WritePage)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("DataSet", "Ds", "A single Chart Data Set to visualize", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Boundary", "B", "The rectangular boundary of the Shape", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Legend Location", "L", "Optional Legend location", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[2];
            foreach (Justification value in Enum.GetValues(typeof(Justification)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
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
            DataSet data = null;
            if (!DA.GetData(0, ref data)) return;

            Rectangle3d boundary = new Rectangle3d(Plane.WorldXY, 300, 200);
            DA.GetData(1, ref boundary);

            Shape shape = new Shape(data, Shape.ChartTypes.Pie, boundary);

            int justification = 0;
            if (DA.GetData(2, ref justification)) shape.Justification = (Justification)justification;

            prev_shapes.Add(shape);
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
                return Properties.Resources.PDF_PieGraph_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8b9306b4-9ed2-4b81-bd7b-3b87c413e239"); }
        }
    }
}