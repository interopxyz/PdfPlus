using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Block_Break : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_PageBreak class.
        /// </summary>
        public GH_Pdf_Block_Break()
          : base("Break Block", "Brk Blk",
              "Create a page break or line break Block",
              Constants.ShortName, Constants.Blocks)
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
            pManager.AddIntegerParameter("Type", "T", "The break type", GH_ParamAccess.item, 0);
            pManager[0].Optional = true;
            pManager.AddIntegerParameter("Count", "C", "The number of repetitions of the break", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;


            Param_Integer paramA = (Param_Integer)pManager[0];
            paramA.AddNamedValue("Line", 0);
            paramA.AddNamedValue("Page", 1);

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
            int type = 0;
            DA.GetData(0, ref type);

            int count = 1;
            DA.GetData(1, ref count);
            if (count < 1) count = 1;

            Block block = Block.CreatePageBreak(count);
            switch (type)
            {
                case 0:
                    block = Block.CreateLineBreak(count);
                    break;
            }

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
                return Properties.Resources.Pdf_Block_Break;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c8f9c9fa-6c24-4934-88af-20b0aeb788f9"); }
        }
    }
}