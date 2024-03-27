using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Block_ChartPie : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_ChartPie class.
        /// </summary>
        public GH_Pdf_Block_ChartPie()
          : base("Pie Chart Block", "Pie Blk",
              "Create a pie chart block",
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
            pManager.AddGenericParameter("DataSet", "Ds", "A single Chart Data Set to visualize", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Legend Location", "L", "Optional Legend location", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;

            Param_Integer paramC = (Param_Integer)pManager[1];
            foreach (Alignment value in Enum.GetValues(typeof(Alignment)))
            {
                paramC.AddNamedValue(value.ToString(), (int)value);
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
            DataSet data =null;
            if (!DA.GetData(0, ref data)) return;

            Block block = Block.CreateChart(data, Element.ChartTypes.Pie);

            int alignment = 0;
            if (DA.GetData(1, ref alignment)) block.Alignment = (Alignment)alignment;

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
                return Properties.Resources.Pdf_Block_Chart_Pie;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f959474e-0a17-44d8-9ae3-dd921e5a94db"); }
        }
    }
}