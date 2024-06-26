﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_Table : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Block_Table class.
        /// </summary>
        public GH_Pdf_Block_Table()
          : base("Table Block", "Tbl Blk",
              "Create a table Block list of DataSet objects",
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
            pManager.AddGenericParameter("DataSet", "Ds", "A list of DataSet objects", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Border Style", "B", "Table border style", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Heading ", "H", "Table Heading formatting", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Column Fitting", "F", "Table column width " + Environment.NewLine + "(-1 = Autofit Page" + Environment.NewLine + "0 = Autofit Content" + Environment.NewLine + "0 < Fixed Size", GH_ParamAccess.item, -1);
            pManager[3].Optional = true;
            pManager.AddColourParameter("Alternating Color", "A", "Table alternating row color", GH_ParamAccess.item);
            pManager[4].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("None", 0);
            paramA.AddNamedValue("All", 1);
            paramA.AddNamedValue("Horizontal", 2);
            paramA.AddNamedValue("Horizontal Interior", 3);
            paramA.AddNamedValue("Vertical", 4);
            paramA.AddNamedValue("Vertical Interior", 5);

            Param_Integer paramB = (Param_Integer)pManager[2];
            paramB.AddNamedValue("None", 0);
            paramB.AddNamedValue("Row", 1);
            paramB.AddNamedValue("Column", 2);
            paramB.AddNamedValue("Row / Column", 3);

            Param_Integer paramC = (Param_Integer)pManager[3];
            paramC.AddNamedValue("Fit Width", -1);
            paramC.AddNamedValue("Fit Content", 0);
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

            Block block = Block.CreateTable(data);

            int border = 0;
            DA.GetData(1, ref border);

            switch (border)
            {
                default:
                    block.HorizontalBorderStyle = Element.BorderStyles.None;
                    block.VerticalBorderStyle = Element.BorderStyles.None;
                    break;
                case 1:
                    block.HorizontalBorderStyle = Element.BorderStyles.All;
                    block.VerticalBorderStyle = Element.BorderStyles.All;
                    break;
                case 2:
                    block.HorizontalBorderStyle = Element.BorderStyles.All;
                    block.VerticalBorderStyle = Element.BorderStyles.None;
                    break;
                case 3:
                    block.HorizontalBorderStyle = Element.BorderStyles.Interior;
                    block.VerticalBorderStyle = Element.BorderStyles.None;
                    break;
                case 4:
                    block.HorizontalBorderStyle = Element.BorderStyles.None;
                    block.VerticalBorderStyle = Element.BorderStyles.All;
                    break;
                case 5:
                    block.HorizontalBorderStyle = Element.BorderStyles.None;
                    block.VerticalBorderStyle = Element.BorderStyles.Interior;
                    break;
            }

            int headers = 0;
            DA.GetData(2, ref headers);

            switch (headers)
            {
                default:
                    break;
                case 1:
                    block.XAxis = "Row";
                    break;
                case 2:
                    block.YAxis = "Column";
                    break;
                case 3:
                    block.XAxis = "Row";
                    block.YAxis = "Column";
                    break;

            }

            int width = -1;
            if (DA.GetData(3, ref width)) block.ColumnWidth = width;

            Sd.Color color = Sd.Color.Gray;
            if (DA.GetData(4, ref color)) block.AlternateColor = color;

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
                return Properties.Resources.Pdf_Block_Table2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4614f54a-9978-4851-b2a1-ba52a911a29a"); }
        }
    }
}