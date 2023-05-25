using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Grasshopper.Kernel.Parameters;

namespace PdfPlus.Components
{
    public class GH_Pdf_Doc_Add : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the Test class.
        /// </summary>
        public GH_Pdf_Doc_Add()
          : base("Add Document", "Doc",
              "Create a new PDF Document",
              Constants.ShortName, Constants.WritePage)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Pages", "Pg", "Pages to add to the document", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Page Layout", "L", "The page layout", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (PageLayouts value in Enum.GetValues(typeof(PageLayouts)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Page> pages = new List<Page>();
            if(!DA.GetDataList(0, pages))return;

            Document document = new Document(pages);

            int layout = 0;
            if (DA.GetData(1, ref layout)) document.PageLayout = (PageLayouts)layout;

            PrevDocumentShapes(document);
            DA.SetData(0, document);
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
                return Properties.Resources.Pdf_Document_Add_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("251fae32-d546-4777-a9a0-9f48fcb8b90d"); }
        }
    }
}