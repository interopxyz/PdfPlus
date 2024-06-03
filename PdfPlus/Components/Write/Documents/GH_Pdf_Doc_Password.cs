using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel.Types;

namespace PdfPlus.Components.Write.Documents
{
    public class GH_Pdf_Doc_Password : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_Password class.
        /// </summary>
        public GH_Pdf_Doc_Password()
          : base("Document Password", "Pass",
              "Lock the PDF Document with a Password",
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
            pManager.AddTextParameter("Password", "P", "Password to Lock the ", GH_ParamAccess.item);
            pManager[1].Optional = true;
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
            if (!DA.GetData(0, ref goo)) return;
            Document document = new Document();
            if (!goo.TryGetDocument(ref document)) return;

            string password = "";
            if (DA.GetData(1, ref password)) document.Password = password;

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
                return Properties.Resources.Pdf_Document_Password;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("df83b610-7553-4857-8f86-defb4800a0ea"); }
        }
    }
}