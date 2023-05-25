using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfPlus.Components
{
    public class GH_Pdf_Doc_Save : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_Save class.
        /// </summary>
        public GH_Pdf_Doc_Save()
          : base("Save Document", "Save Doc",
              "Save the PDF Document to a file",
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
            pManager.AddGenericParameter(Constants.Document.Name, Constants.Document.NickName, Constants.Document.Input, GH_ParamAccess.item);
            pManager.AddTextParameter("Directory", "D", "The directory to save the Document.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("File Name", "N", "The Document name to save.", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Save", "_S", "Toggle to true to save the Document.", GH_ParamAccess.item, false);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "P", "The full file path to the saved file.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
                Document document = null;
                if (!DA.GetData(0, ref document))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please provide a document.");
                    return;
                }
                document = new Document(document);
                PrevDocumentShapes(document);

            bool save = false;
            DA.GetData(3, ref save);
            if (save)
            {

                string path = "C:\\Users\\Public\\Documents\\";
                bool hasPath = DA.GetData(1, ref path);

                if (!hasPath)
                {
                    if (this.OnPingDocument().FilePath != null)
                    {
                        path = Path.GetDirectoryName(this.OnPingDocument().FilePath);
                    }
                }

                if (!Directory.Exists(path))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The provided directory does not exist. Please verify this is a valid file path.");
                    return;
                }

                string name = DateTime.UtcNow.ToString("yyyy-dd-M_HH-mm-ss");
                DA.GetData(2, ref name);

                name = name.EndsWith(".pdf") ? name : $"{name}.pdf";
                string filepath = Path.Combine(path, name);

                document.Save(filepath);
                DA.SetData(0, filepath);
            }
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
                return Properties.Resources.Pdf_Document_Save2_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0633f5dc-c0a4-4399-9ead-ab58467cec2e"); }
        }
    }
}