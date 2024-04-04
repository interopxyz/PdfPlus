using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Format_Font : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_EditFont class.
        /// </summary>
        public GH_Pdf_Format_Font()
          : base("Set Font", "Set Font",
              "Modify Shape, Block, or DataSet fonts",
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
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, Constants.Element.Input, GH_ParamAccess.item);
            pManager.AddTextParameter("Family Name", "F", "Optional Font family name", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Size", "S", "Optional Text size", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddColourParameter("Color", "C", "Optional Text color", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Styling", "T", "Options Text styling type", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item);
            pManager[5].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[4];
            foreach (FontStyle value in Enum.GetValues(typeof(FontStyle)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

            Param_Integer paramB = (Param_Integer)pManager[5];
            foreach (Justification value in Enum.GetValues(typeof(Justification)))
            {
                paramB.AddNamedValue(value.ToString(), (int)value);
            }

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, Constants.Element.Output, GH_ParamAccess.item);
            pManager.AddTextParameter("Family Name", "F", "Font family name", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "Text size", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "Text color", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Styling", "T", "Text styling type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item);
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
            Element elem = null;
            bool isElement = goo.TryGetElement(ref elem);
            if (isElement) font = new Font(elem.Font);

            string family = "Arial";
            if (DA.GetData(1, ref family)) font.Family = family;
            DA.SetData(1, font.Family);

            double size = 10.0;
            if (DA.GetData(2, ref size)) font.Size = size;
            DA.SetData(2, font.Size);

            Sd.Color color = Sd.Color.Black;
            if (DA.GetData(3, ref color)) font.Color = color;
            DA.SetData(3, font.Color);

            int style = 0;
            if (DA.GetData(4, ref style)) font.Style = (FontStyle)style;
            DA.SetData(4, font.Style);

            int justification = 0;
            if (DA.GetData(5, ref justification)) font.Justification = (Justification)justification;
            DA.SetData(5, font.Justification);

            switch (elem.ElementType)
            {
                case Element.ElementTypes.Block:
                    goo.CastTo<Block>(out Block block);
                    block.Font = font;
                    DA.SetData(0, block);
                    break;
                case Element.ElementTypes.Shape:
                    goo.CastTo<Shape>(out Shape shape);
                    shape.Font = font;
                    prev_shapes.Add(shape);
                    DA.SetData(0, shape);
                    break;
                case Element.ElementTypes.Data:
                    goo.CastTo<DataSet>(out DataSet dataSet);
                    dataSet.Font = font;
                    DA.SetData(0, dataSet);
                    break;
                case Element.ElementTypes.Fragment:
                    Fragment fragment = null;
                    goo.TryGetFragment(ref fragment);
                    fragment.SetFonts(font);
                    DA.SetData(0, fragment);
                    break;
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
                return Properties.Resources.Pdf_Format_Font;
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