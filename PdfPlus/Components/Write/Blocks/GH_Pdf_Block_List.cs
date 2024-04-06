using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Block_List : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_List class.
        /// </summary>
        public GH_Pdf_Block_List()
          : base("List Block", "Lst Blk",
              "Create an unordered (bulleted) list or numbered list Block",
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
            pManager.AddGenericParameter(Constants.Fragment.Name, Constants.Fragment.NickName, Constants.Fragment.Input, GH_ParamAccess.list);
            pManager.AddIntegerParameter("Bullet Type", "B", "The list bullet type", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;


            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (Block.ListTypes value in Enum.GetValues(typeof(Block.ListTypes)))
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
            List<IGH_Goo> goos = new List<IGH_Goo>();
            if (!DA.GetDataList(0, goos)) return;

            List<Fragment> outputs = new List<Fragment>();

            foreach (IGH_Goo goo in goos)
            {
                Fragment fragment = null;
                if (goo.TryGetFragment(ref fragment)) outputs.Add(fragment);
            }

            int type = 0;
            DA.GetData(1, ref type);

            Block block = Block.CreateList(outputs,(Block.ListTypes)type);

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
                return Properties.Resources.Pdf_Block_List;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0e512243-8cec-406b-bb6b-4a6f16e2456d"); }
        }
    }
}