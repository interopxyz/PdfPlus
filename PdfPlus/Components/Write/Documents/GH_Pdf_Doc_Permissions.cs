using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel.Types;

namespace PdfPlus.Components.Write.Documents
{
    public class GH_Pdf_Doc_Permissions : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_Permissions class.
        /// </summary>
        public GH_Pdf_Doc_Permissions()
          : base("Document Permissions", "Permissions",
              "Set the PDF Document Access Permissions",
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
            pManager.AddBooleanParameter("Modify", "M", "Allow users to Modify the PDF", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Extract", "E", "Allow users to Extract from the PDF", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Print", "P", "Allow users to Print the Document", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Optional Password", "O", "Optional Password to let users override permissions."+Environment.NewLine+"If no value is provided a random GUID will be assigned." + Environment.NewLine +"Note: This password does not limit access to the PDF.", GH_ParamAccess.item);
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Output, GH_ParamAccess.item);
            pManager.AddBooleanParameter("Modify", "M", "Allow users to Modify the PDF", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Extract", "E", "Allow users to Extract from the PDF", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Print", "P", "Allow users to Print the Document", GH_ParamAccess.item);
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

            bool modify = false;
            if (DA.GetData(1, ref modify)) document.PermitModify = modify;

            bool extract = false;
            if (DA.GetData(2, ref extract)) document.PermitExtract = extract;

            bool print = false;
            if (DA.GetData(3, ref print)) document.PermitPrint= print;

            string password = Guid.NewGuid().ToString();
            if (DA.GetData(4, ref password)) document.PermitPrint = print;

            DA.SetData(0, document);
            DA.SetData(1, document.PermitModify);
            DA.SetData(2, document.PermitExtract);
            DA.SetData(3, document.PermitPrint);
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
                return Properties.Resources.Pdf_Document_Permissions;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0aaa0787-69f0-41a5-95a3-faf1fe694286"); }
        }
    }
}