using Grasshopper.Kernel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Blk_Text : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_Text class.
        /// </summary>
        public GH_Pdf_Blk_Text()
          : base("Text Block", "Txt",
              "Create a text block",
              Constants.ShortName, Constants.MigraDoc)
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
            pManager.AddTextParameter("Text Content", "T", "The text content to display", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Block.Name, Constants.Block.NickName, Constants.Block.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string text = string.Empty;
            DA.GetData(0, ref text);

            Block block = Block.CreateText(text);

            DA.SetData(0, block);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4e9ff875-26bb-4365-a839-1fbccccae6bd"); }
        }
    }
}