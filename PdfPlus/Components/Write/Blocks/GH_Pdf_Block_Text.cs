using Grasshopper.Kernel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace PdfPlus.Components
{
    public class GH_Pdf_Block_Text : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_Text class.
        /// </summary>
        public GH_Pdf_Block_Text()
          : base("Text Block", "Txt Blk",
              "Create a text Block",
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
            pManager.AddGenericParameter(Constants.Fragment.Name, Constants.Fragment.NickName, Constants.Fragment.Input, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Formatting", "F", "Optional predefined text formatting", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (Font.Presets value in Enum.GetValues(typeof(Font.Presets)))
            {
                paramA.AddNamedValue((int)value+" | "+value.ToString(), (int)value);
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
            if (!DA.GetData(0, ref goo)) return;
            Fragment fragment = null;
            if (!goo.TryGetFragment(ref fragment)) return;

            int formatting = 0;
            DA.GetData(1, ref formatting);

            Block block = Block.CreateText(fragment,(Font.Presets)formatting);

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
                return Properties.Resources.Pdf_Block_Text;
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