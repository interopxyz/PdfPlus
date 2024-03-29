using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Format_DataSet : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Shape_GraphData class.
        /// </summary>
        public GH_Pdf_Format_DataSet()
          : base("Data Set", "Data Set",
              "Compiles a list of text or numbers into a Data Set with optional formatting options."+Environment.NewLine +
                "For use with PDF+ Chart and Table components." + Environment.NewLine +
                " - Table: Each DataSet is a column in the Table" + Environment.NewLine +
                " - Chart: Each DataSet is a series in the Chart",
              Constants.ShortName, Constants.Formats)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Values", "V", "A list of numeric values", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "C", "Optional list of colors corresponding to the values", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Label", "L", "Optional data label position", GH_ParamAccess.item,0);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Title", "T", "An optional name for the series", GH_ParamAccess.item);
            pManager[3].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[2];
            foreach (DataSet.LabelAlignments value in Enum.GetValues(typeof(DataSet.LabelAlignments)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("DataSet", "Ds", "A PDF Plus Data Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> values = new List<string>();
            if (!DA.GetDataList(0, values)) return;



            DataSet dataSet = new DataSet(values);

            List<Sd.Color> colors = new List<Sd.Color>();
            if (DA.GetDataList(1, colors)) dataSet.Colors = colors;

            int label = 0;
            if (DA.GetData(2, ref label)) dataSet.LabelAlignment = (DataSet.LabelAlignments)label;

            string title = string.Empty;
            if (DA.GetData(3, ref title)) dataSet.Title = title;

            DA.SetData(0,dataSet);
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
                return Properties.Resources.Pdf_Format_DataSet;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("926bd11c-44fc-4a6e-8b7c-ebcc7e7e82f9"); }
        }
    }
}