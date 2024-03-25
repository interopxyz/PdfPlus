using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Format_DataSetFast : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Format_DataSetFast class.
        /// </summary>
        public GH_Pdf_Format_DataSetFast()
          : base("Quick Data", "Data",
              "Compile text or numbers into a Data Set",
              Constants.ShortName, Constants.Formats)
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
            pManager.AddTextParameter("Data Tree", "N", "A datatree of text or number values. (Only use numbers for charts)", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DataSet", "Ds", "A PDF Plus Data Object", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            GH_Structure<GH_String> ghData = new GH_Structure<GH_String>();

            if (!DA.GetDataTree(0, out ghData)) return;

            Dictionary<int, List<List<string>>> dataSet = new Dictionary<int, List<List<string>>>();
            List<DataSet> dataSets = new List<DataSet>();

            dataSet.Add(0, new List<List<string>>());
            int i = 0;
            foreach (GH_Path path in ghData.Paths)
            {
                if (path.Length > 1)
                {
                    if (!dataSet.ContainsKey(path[0])) dataSet.Add(path[0], new List<List<string>>());
                    dataSet[path[0]].Add(ghData.Branches[i].ToStringList());
                }
                else
                {
                    dataSet[0].Add(ghData.Branches[i].ToStringList());
                }
                i++;
            }

            int rc = this.RunCount - 1;
            if (rc > (dataSet.Keys.Count - 1)) rc = dataSet.Keys.Count - 1;
            foreach (List<string> data in dataSet[rc])
            {
                dataSets.Add(new DataSet(data));
            }

            DA.SetDataList(0, dataSets);

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
                return Properties.Resources.Pdf_Format_QuickData;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d2fd6808-d0d9-49a1-8765-617f8c15b067"); }
        }
    }
}