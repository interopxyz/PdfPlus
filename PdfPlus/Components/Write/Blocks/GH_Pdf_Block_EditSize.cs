using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_EditSize : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Block_EditSize class.
        /// </summary>
        public GH_Pdf_Block_EditSize()
          : base("Resize Block", "Size Blk",
              "Resize Blocks that support fixed boundaries",
              Constants.ShortName, Constants.Blocks)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Block.Name, Constants.Block.NickName, Constants.Block.Input, GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Optional image width override", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Height", "H", "Optional image height override", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Justification", "J", "Optional image justification on page", GH_ParamAccess.item);
            pManager[3].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[3];
            foreach (Justification value in Enum.GetValues(typeof(Justification)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
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
            IGH_Goo goo = null;
            Block block = null;

            if (!DA.GetData(0, ref goo)) return;
            if (!goo.TryGetBlock(ref block)) return;

            double width = 0;
            if (DA.GetData(1, ref width)) block.Width = width;

            double height = 0;
            if (DA.GetData(2, ref height)) block.Height = height;

            int justification = 0;
            if (DA.GetData(3, ref justification)) block.Justification = (Justification)justification;

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
                return Properties.Resources.Pdf_Block_Size;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c03a1734-83b7-4ec6-b437-5c37853319ea"); }
        }
    }
}