using Grasshopper.Kernel;
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
          : base("Graph Data", "Data",
              "Compile data points into a Data Set",
              Constants.ShortName, Constants.Formats)
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
            pManager.AddNumberParameter("Values", "V", "A list of numeric values", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "C", "Optional list of colors corresponding to the values", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Label", "L", "If true, the datapoints will be labeled", GH_ParamAccess.item,true);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Title", "T", "An optional name for the series", GH_ParamAccess.item);
            pManager[3].Optional = true;
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
            List<double> values = new List<double>();
            if (!DA.GetDataList(0, values)) return;

            DataSet dataSet = new DataSet(values);

            List<Sd.Color> colors = new List<Sd.Color>();
            if (DA.GetDataList(1, colors)) dataSet.Colors = colors;

            bool label = true;
            if (DA.GetData(2, ref label)) dataSet.LabelData = label;

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
                return Properties.Resources.Pdf_Format_Data;
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