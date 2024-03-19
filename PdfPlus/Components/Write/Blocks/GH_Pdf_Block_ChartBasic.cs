﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_ChartBasic : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Blk_ChartLine class.
        /// </summary>
        public GH_Pdf_Block_ChartBasic()
          : base("Basic Chart Block", "Cht Blk",
              "Create a basic chart block",
              Constants.ShortName, Constants.Blocks)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("DataSet", "Ds", "Chart Data to visualize", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Type", "T", "The chart format to be displayed", GH_ParamAccess.item, 4);
            pManager[1].Optional = true;


            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("Bar", 0);
            paramA.AddNamedValue("BarStacked", 1);
            paramA.AddNamedValue("Column", 2);
            paramA.AddNamedValue("ColumnStacked", 3);
            paramA.AddNamedValue("Line", 4);
            paramA.AddNamedValue("Area", 5);
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
            List<DataSet> data = new List<DataSet>();
            if (!DA.GetDataList(0, data)) return;

            int type = 4;
            DA.GetData(1, ref type);

            Block block = Block.CreateChart(data, (Element.ChartTypes)type);

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
                return Properties.Resources.Pdf_Block_Column;
            }
        }
        

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("59db45ad-3851-4d74-a6fa-b2007b55cf50"); }
        }
    }
}