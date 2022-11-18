using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Shape_EditFont : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_EditFont class.
        /// </summary>
        public GH_Pdf_Shape_EditFont()
          : base("Shape Font", "Shp Font",
              "Edit basic Font properties",
              Constants.ShortName, Constants.WritePage)
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
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Input, GH_ParamAccess.item);
            pManager.AddTextParameter("Family Name", "F", "Optional font family name", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Size", "S", "Optional font size", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddColourParameter("Color", "C", "Optional text color", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Style", "T", "The font style type", GH_ParamAccess.item, 0);
            pManager[4].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[4];
            foreach (FontStyle value in Enum.GetValues(typeof(FontStyle)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Font font = new Font();

            IGH_Goo goo = null;
            if (!DA.GetData(0, ref goo)) return;
            Shape shape = null;
            bool isShape = goo.TryGetShape(ref shape);
            if (isShape) font = shape.Font;

            DataSet data = null;
            bool isData = goo.CastTo<DataSet>(out data);
            if (isData) data = new DataSet(data);
            if (isData) font = data.Font;

            string family = "Arial";
            if (DA.GetData(1, ref family)) font.Family = family;

            double size = 10.0;
            if (DA.GetData(2, ref size)) font.Size = size;

            Sd.Color color = Sd.Color.Black;
            if (DA.GetData(3, ref color)) font.Color = color;

            int style = 0;
            if (DA.GetData(4, ref style)) font.Style = (FontStyle)style;

            if (isShape)
            {
                shape.Font = font;
                DA.SetData(0, shape);
            }

            if (isData)
            {
                data.Font = font;
                DA.SetData(0, data);
            }
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
                return Properties.Resources.Pdf_Content_Font_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("12d4cdee-bdf3-4e1d-9bf0-efc493a8e190"); }
        }
    }
}