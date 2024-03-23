using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Block_Table : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_Blk_Table class.
        /// </summary>
        public GH_Pdf_Block_Table()
          : base("Table Block", "Tbl Blk",
              "Create a table block",
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
            pManager.AddTextParameter("Text Tree", "T", "A datatree of text values", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Border Style", "B", "Table border style", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager.AddColourParameter("Alternating Color", "A", "Table alternating row color", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Heading ", "H", "Table Heading formatting", GH_ParamAccess.item, 0);
            pManager[3].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            paramA.AddNamedValue("None", 0);
            paramA.AddNamedValue("All", 1);
            paramA.AddNamedValue("Horizontal", 2);
            paramA.AddNamedValue("Horizontal Interior", 3);
            paramA.AddNamedValue("Vertical", 4);
            paramA.AddNamedValue("Vertical Interior", 5);

            Param_Integer paramB = (Param_Integer)pManager[3];
            paramB.AddNamedValue("None", 0);
            paramB.AddNamedValue("Row", 1);
            paramB.AddNamedValue("Column", 2);
            paramB.AddNamedValue("Row / Column", 3);
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

            GH_Structure<GH_String> ghData = new GH_Structure<GH_String>();

            if (!DA.GetDataTree(0, out ghData)) return;

            Dictionary<int, List<List<string>>> dataSets = new Dictionary<int, List<List<string>>>();
            List<List<string>> dataSet = new List<List<string>>();

            dataSets.Add(0, new List<List<string>>());
            int i = 0;
            foreach (GH_Path path in ghData.Paths)
            {
                if (path.Length > 1)
                {
                    if (!dataSets.ContainsKey(path[0])) dataSets.Add(path[0], new List<List<string>>());
                    dataSets[path[0]].Add(ghData.Branches[i].ToStringList());
                }
                else
                {
                    dataSets[0].Add(ghData.Branches[i].ToStringList());
                }
                i++;
            }

            int rc = this.RunCount - 1;
            if (rc > (dataSets.Keys.Count - 1)) rc = dataSets.Keys.Count - 1;
            foreach (List<string> data in dataSets[rc])
            {
                dataSet.Add(data);
            }


            Block block = Block.CreateTable(dataSet);

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

            Sd.Color color = Sd.Color.Gray;
            if (DA.GetData(2, ref color)) block.AlternateColor = color;

            int headers = 0;
            DA.GetData(3, ref headers);

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
                return Properties.Resources.Pdf_Block_Table;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("621a662a-fefb-45ab-997d-efc8d4259ebc"); }
        }
    }
}