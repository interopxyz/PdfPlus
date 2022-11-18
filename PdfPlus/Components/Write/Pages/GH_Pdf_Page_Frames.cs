using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Page_Frames : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_Frames class.
        /// </summary>
        public GH_Pdf_Page_Frames()
          : base("Page Boundaries", "Boundaries",
              "Get or modify the boundaries of a page",
              Constants.ShortName, Constants.WritePage)
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
            pManager.AddRectangleParameter("Art Box", "A", "The art box of the page", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddRectangleParameter("Bleed Box", "B", "The bleed box of the page", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddRectangleParameter("Crop Box", "C", "The crop box of the page",GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddRectangleParameter("Trim Box", "T", "The trim box of the page", GH_ParamAccess.item);
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Page.Name, Constants.Page.NickName, Constants.Page.Output, GH_ParamAccess.item);
            pManager.AddRectangleParameter("Art Box", "A", "The art box of the page", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Bleed Box", "B", "The bleed box of the page", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Crop Box", "C", "The crop box of the page", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Trim Box", "T", "The trim box of the page", GH_ParamAccess.item);
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

            Rectangle3d artbox = new Rectangle3d();
            if (DA.GetData(1, ref artbox)) page.ArtBox = artbox;

            Rectangle3d bleedbox = new Rectangle3d();
            if (DA.GetData(2, ref bleedbox)) page.BleedBox = bleedbox;

            Rectangle3d cropbox = new Rectangle3d();
            if (DA.GetData(3, ref cropbox)) page.CropBox = cropbox;

            Rectangle3d trimbox = new Rectangle3d();
            if (DA.GetData(4, ref trimbox)) page.TrimBox = trimbox;

            DA.SetData(0, page);
            DA.SetData(1, page.ArtBox);
            DA.SetData(2, page.BleedBox);
            DA.SetData(3, page.CropBox);
            DA.SetData(4, page.TrimBox);

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
                return Properties.Resources.PDF_Border_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6ca6613e-f5a3-4f72-8737-fe6d8d116189"); }
        }
    }
}