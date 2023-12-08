using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Page_AddMetric : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_AddPageTemplate class.
        /// </summary>
        public GH_Pdf_Page_AddMetric()
          : base("Add Page Metric", "Page Metric",
              "Create a new PDF Page from a standard metric page size",
              Constants.ShortName, Constants.WritePage)
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
            pManager.AddPlaneParameter("Frame", "F", "Optional origin plane of the page", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Type", "T", "A standard Metric A series paper size", GH_ParamAccess.item, 4);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Orientation", "O", "Set the portrait or landscape orientation of the page", GH_ParamAccess.item,0);
            pManager[2].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (SizesA value in Enum.GetValues(typeof(SizesA)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

            Param_Integer paramB = (Param_Integer)pManager[2];
            foreach (PageOrientation value in Enum.GetValues(typeof(PageOrientation)))
            {
                paramB.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Page.Name, Constants.Page.NickName, Constants.Page.Output, GH_ParamAccess.item);
            pManager.AddRectangleParameter("Boundary", "B", "The page boundary in points", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int type = 0;
            DA.GetData(1, ref type);

            int orient = 0;
            DA.GetData(2, ref orient);
            Page page = new Page((SizesA)type, (PageOrientation)orient);

            Plane plane = Plane.WorldXY;
            if (DA.GetData(0, ref plane)) page.Frame = new Plane(plane);


            DA.SetData(0, page);
            DA.SetData(1, page.Boundary);
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
                return Properties.Resources.Pdf_Page_Metric_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("60233390-b0a2-4715-899c-bc900b939a0c"); }
        }
    }
}