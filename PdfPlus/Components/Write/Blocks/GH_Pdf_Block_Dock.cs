using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_Dock : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Block_Dock class.
        /// </summary>
        public GH_Pdf_Block_Dock()
          : base("Dock Block", "Dock Blk",
              "Place a list of Blocks aligned horizontally on the page."+Environment.NewLine+"Compatible with Text, List, Chart, Drawing, and Image Blocks",
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
            pManager.AddGenericParameter(Constants.Block.Name, Constants.Block.NickName, Constants.Block.Input, GH_ParamAccess.list);
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
            List<IGH_Goo> goos = new List<IGH_Goo>();
            if (!DA.GetDataList(0, goos)) return;

            List<Block> blocks = new List<Block>();
            foreach (IGH_Goo goo in goos)
            {
                Block block = null;
                if (goo.TryGetBlock(ref block)) blocks.Add(block);
            }

            if (blocks.Count > 0)
            {
                Block output = Block.CreateDock(blocks);
                DA.SetData(0, output);
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
                return Properties.Resources.Pdf_Block_Horizontal;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1e1aa4a1-326a-4c1f-b524-64930f49a4af"); }
        }
    }
}