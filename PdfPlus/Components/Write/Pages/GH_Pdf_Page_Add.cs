using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Page_Add : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_AddPage class.
        /// </summary>
        public GH_Pdf_Page_Add()
          : base("Create Page", "Page",
              "Create a new PDF Page from a width and height",
              Constants.ShortName, Constants.Pages)
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
            pManager.AddIntegerParameter(Constants.Units.Name, Constants.Units.NickName, Constants.Units.Input, GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Width", "W", "The document width in the chosen units", GH_ParamAccess.item,210);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Height", "H", "The document height in the chosen units", GH_ParamAccess.item, 297);
            pManager[3].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (Units value in Enum.GetValues(typeof(Units)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
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
            int unit = 0;
            DA.GetData(1, ref unit);

            double width = 210;
            DA.GetData(2, ref width);

            double height = 297;
            DA.GetData(3, ref height);

            Page page = new Page((Units)unit, width, height);

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
                return Properties.Resources.Pdf_Page_Add_Custom;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b2b9de5c-523b-4bd7-a1b2-41012b3e1d53"); }
        }
    }
}