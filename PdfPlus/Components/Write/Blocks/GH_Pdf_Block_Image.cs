using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_Image : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_Image class.
        /// </summary>
        public GH_Pdf_Block_Image()
          : base("Image Block", "Img Blk",
              "Create an image block",
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
            pManager.AddGenericParameter("Image", "I", "The System.Drawing.Bitmap or Image Filepath to display", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Optional image width override", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Height", "H", "Optional image height override", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Justification", "J", "Optional image justification on page", GH_ParamAccess.item, 0);
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
            Sd.Bitmap bitmap = null;
            string path = "";

            if (!DA.GetData(0, ref goo)) return;
            if (!goo.TryGetBitmap(ref bitmap, ref path)) return;

            Block block = null;
            if (path != "")
            {
                block = Block.CreateImage(path);
            }
            else
            {
                block = Block.CreateImage(bitmap);
            }

            double width = 0;
            if (DA.GetData(1, ref width)) block.Width = width;

            double height = 0;
            if (DA.GetData(2, ref width)) block.Height = height;

            Font font = block.Font;
            int justification = 0;
            if (DA.GetData(3, ref justification)) font.Justification = (Justification)justification;
            block.Font = font;
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
                return Properties.Resources.Pdf_Block_Image;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1bccdc38-d2d7-4076-b827-da6cce74b558"); }
        }
    }
}