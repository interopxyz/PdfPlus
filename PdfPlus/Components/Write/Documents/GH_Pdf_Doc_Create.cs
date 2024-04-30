using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Documents
{
    public class GH_Pdf_Doc_Create : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_AddPages class.
        /// </summary>
        public GH_Pdf_Doc_Create()
          : base("Create Document", "Create Doc",
              "Add a list of Pages to an existing PDF Document or create a new Document.",
              Constants.ShortName, Constants.Documents)
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
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Input, GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddGenericParameter("Pages", "Pg", "Pages to add to the document", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Page Layout", "L", "The page layout", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;

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

            //TRY GET DOCUMENT
            IGH_Goo goo = null;
            Document document = new Document();
            
            if(DA.GetData(0, ref goo))goo.TryGetDocument(ref document);

            //TRY GET PAGES
            List<IGH_Goo> goos = new List<IGH_Goo>();
            if (!DA.GetDataList(1, goos)) return;
            List<Page> pages = new List<Page>();
            foreach (IGH_Goo g in goos)
            {
                Page page = new Page();
                if (g.TryGetPage(ref page)) pages.Add(page);
            }

            int layout = 0;
            if (DA.GetData(2, ref layout)) document.PageLayout = (PageLayouts)layout;

            document.AddPages(pages);

            //PrevDocumentShapes(document);
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
                return Properties.Resources.Pdf_Document_Add;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c851d2f0-6304-46ea-89d7-b649b7d27594"); }
        }
    }
}