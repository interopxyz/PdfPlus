using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel.Types;

namespace PdfPlus.Components.Write.Documents
{
    public class GH_Pdf_Doc_MetaData : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_MetaData class.
        /// </summary>
        public GH_Pdf_Doc_MetaData()
          : base("Document MetaData", "Meta",
              "Edit the PDF Document MetaData",
              Constants.ShortName, Constants.Documents)
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
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Input, GH_ParamAccess.item);
            pManager.AddTextParameter("Title", "T", "Concise title of the document "+Environment.NewLine+ "Notes:"+Environment.NewLine+"The title is not the filename and does not have to match."+Environment.NewLine+"Many search engines use the title to describe the document in their search results list.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Subject", "S", "Brief summary of the document", GH_ParamAccess.item);
            pManager[2].Optional = true;
            //pManager.AddTextParameter("Author", "A", "Optional name of the document's author(s)", GH_ParamAccess.item);
            //pManager[3].Optional = true;
            //pManager.AddTextParameter("Keywords", "K", "A list of keywords to assit in the document's discoverability in searches", GH_ParamAccess.list);
            //pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Output, GH_ParamAccess.item);
            pManager.AddTextParameter("Title", "T", "Concise title of the document", GH_ParamAccess.item);
            pManager.AddTextParameter("Subject", "S", "Brief summary of the document", GH_ParamAccess.item);
            //pManager.AddTextParameter("Author", "A", "Optional name of the document's author(s)", GH_ParamAccess.item);
            //pManager.AddTextParameter("Keywords", "K", "A list of keywords to assit in the document's discoverability in searches", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //TRY GET DOCUMENT
            IGH_Goo goo = null;
            if (!DA.GetData(0, ref goo)) return;
            Document document = new Document();
            if (!goo.TryGetDocument(ref document)) return;

            string title = "";
            if (DA.GetData(1, ref title)) document.Title = title;

            string subject = "";
            if (DA.GetData(2, ref subject)) document.Subject= subject;

            //string author = "";
            //if (DA.GetData(3, ref author)) document.Author = author;

            //List<string> keywords = new List<string>();
            //if (DA.GetDataList(4, keywords)) document.Keywords = keywords;

            DA.SetData(0, document);
            DA.SetData(1, document.Title);
            DA.SetData(2, document.Subject);
            //DA.SetData(3, document.Author);
            //DA.SetDataList(4, document.Keywords);
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
                return Properties.Resources.Pdf_Document_Meta;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c3e354ad-50dc-42d0-ae7e-47201a854eec"); }
        }
    }
}