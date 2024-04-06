using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Block_GetBlocks : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_GetBlocks class.
        /// </summary>
        public GH_Pdf_Block_GetBlocks()
          : base("Get Blocks", "Get Blk",
              "Get the Blocks from Pages or Documents.",
              Constants.ShortName, Constants.Blocks)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
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
            pManager.AddGenericParameter(Constants.Block.Name, Constants.Block.NickName, Constants.Block.Input, GH_ParamAccess.list);
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

            DA.SetDataList(0, document.Blocks);
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
                return Properties.Resources.Pdf_Block_Deconstruct;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ef1ea0e4-0378-462d-9ec2-58d579848650"); }
        }
    }
}