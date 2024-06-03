using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;

namespace PdfPlus.Components.Write.Documents
{
    public class GH_Pdf_Doc_Options : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_Options class.
        /// </summary>
        public GH_Pdf_Doc_Options()
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
            pManager.AddIntegerParameter("Layout", "L", "Specifies the page layout to be used by a viewer when the document is opened.", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Page", "P", "Specifies how the document should be displayed by a viewer when opened.", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Color Mode", "C", "Specifies what color model is used in a PDF document.", GH_ParamAccess.item, 0);
            pManager[3].Optional = true;
            //pManager.AddIntegerParameter("Compression Mode", "M", "Specifies the compression mode of the document.", GH_ParamAccess.item, 0);
            //pManager[4].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (Document.LayoutModes value in Enum.GetValues(typeof(Document.LayoutModes)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

            Param_Integer paramB = (Param_Integer)pManager[2];
            foreach (Document.PageModes value in Enum.GetValues(typeof(Document.PageModes)))
            {
                paramB.AddNamedValue(value.ToString(), (int)value);
            }

            Param_Integer paramC = (Param_Integer)pManager[3];
            foreach (Document.ColorModes value in Enum.GetValues(typeof(Document.ColorModes)))
            {
                paramC.AddNamedValue(value.ToString(), (int)value);
            }

            //Param_Integer paramD = (Param_Integer)pManager[4];
            //foreach (Document.CompressionModes value in Enum.GetValues(typeof(Document.CompressionModes)))
            //{
            //    paramD.AddNamedValue(value.ToString(), (int)value);
            //}

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Output, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Layout", "L", "Specifies the page layout to be used by a viewer when the document is opened.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Page", "P", "Specifies how the document should be displayed by a viewer when opened.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Color Mode", "C", "Specifies what color model is used in a PDF document.", GH_ParamAccess.item);
            //pManager.AddIntegerParameter("Compression Mode", "M", "Specifies the compression mode of the document.", GH_ParamAccess.item);
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

            int layout = 0;
            if (DA.GetData(1, ref layout)) document.LayoutMode = (Document.LayoutModes)layout;

            int page = 0;
            if (DA.GetData(2, ref page)) document.PageMode = (Document.PageModes)page;

            int color = 0;
            if (DA.GetData(3, ref color)) document.ColorMode = (Document.ColorModes)color;

            //int compression = 0;
            //if (DA.GetData(4, ref compression)) document.CompressionMode = (Document.CompressionModes)compression;

            DA.SetData(0, document);
            DA.SetData(1, document.LayoutMode);
            DA.SetData(2, document.PageMode);
            DA.SetData(3, document.ColorMode);
            //DA.SetData(4, document.CompressionMode);
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
                return Properties.Resources.Pdf_Document_Settings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ae62da1a-7db5-4fc4-9434-442c2369462d"); }
        }
    }
}