using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Page_Deconstruct : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_Deconstruct class.
        /// </summary>
        public GH_Pdf_Page_Deconstruct()
          : base("Deconstruct Page", "DePage",
              "Deconstruct a PDF Page",
              Constants.ShortName, Constants.Pages)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Page.Name, Constants.Page.NickName, Constants.Page.Input, GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary", "B", "The boundary of the page in Points", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Page page = null;
            if (!DA.GetData(0, ref page)) return;
            page = new Page(page);

            DA.SetData(0, page.Boundary);
            
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
                return Properties.Resources.Pdf_Page_Explode_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("902c28b8-3ec8-47c1-8152-aa2faaa7d152"); }
        }
    }
}