using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Format_Graphic : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_EditGraphics class.
        /// </summary>
        public GH_Pdf_Format_Graphic()
          : base("Set Graphics", "Set Graphics",
              "Edit shape or block graphical attributes",
              Constants.ShortName, Constants.Formats)
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
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Input, GH_ParamAccess.item);
            pManager.AddColourParameter("Fill Color", "F", "Optional fill color for the shape", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddColourParameter("Stroke Color", "S", "Optional stroke color for the shape", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Stroke Weight", "W", "Optional stroke weight for the shape", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("Pattern", "P", "A comma seperated pattern. ex(1.5,2,1.0)", GH_ParamAccess.item);
            pManager[4].Optional = true;
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
            IGH_Goo goo = null;
            if (!DA.GetData(0, ref goo)) return;

            Graphic graphic = new Graphic();

            Shape shape = null;
            bool isShape = goo.TryGetShape(ref shape);
            if(isShape)graphic = shape.Graphic;

            DataSet data = null;
            bool isData = goo.CastTo<DataSet>(out data);
            if (isData) data = new DataSet(data);
            if (isData) graphic = data.Graphic;

            Sd.Color fill = Sd.Color.Black;
            if (DA.GetData(1, ref fill)) graphic.Color = fill;

            Sd.Color stroke = Sd.Color.Black;
            if (DA.GetData(2, ref stroke)) graphic.Stroke = stroke;

            double weight = 1.0;
            if (DA.GetData(3, ref weight)) graphic.Weight = weight;

            string pattern = "1.0,2.0";
            if (DA.GetData(4, ref pattern)) graphic.SetPattern(pattern);

            if (isShape)
            {
                shape.Graphic = graphic;
                prev_shapes.Add(shape);
                DA.SetData(0, shape);
            }

            if (isData)
            {
                data.Graphic = graphic;
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
                return Properties.Resources.Pdf_Format_Graphics;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("54c11c7c-45a2-4900-b614-78c22243cd7f"); }
        }
    }
}