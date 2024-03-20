using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components
{
    public class GH_Pdf_Format_FontPresets : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Format_FontPresets class.
        /// </summary>
        public GH_Pdf_Format_FontPresets()
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
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, Constants.Element.Input, GH_ParamAccess.item);
            pManager.AddIntegerParameter("Font", "F", "Preset fonts", GH_ParamAccess.item,0);
            pManager[1].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (Font.Presets value in Enum.GetValues(typeof(Font.Presets)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, Constants.Element.Output, GH_ParamAccess.item);
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
            if (isElement) font = elem.Font;

            int type = 0;
            if (DA.GetData(1, ref type))

                switch ((Font.Presets)type)
                {
                    default:
                        font = Fonts.Normal;
                        break;
                    case Font.Presets.Title:
                        font = Fonts.Title;
                        break;
                    case Font.Presets.Subtitle:
                        font = Fonts.Subtitle;
                        break;
                    case Font.Presets.Heading1:
                        font = Fonts.Heading1;
                        break;
                    case Font.Presets.Heading2:
                        font = Fonts.Heading2;
                        break;
                    case Font.Presets.Heading3:
                        font = Fonts.Heading3;
                        break;
                    case Font.Presets.Heading4:
                        font = Fonts.Heading4;
                        break;
                    case Font.Presets.Heading5:
                        font = Fonts.Heading5;
                        break;
                    case Font.Presets.Heading6:
                        font = Fonts.Heading6;
                        break;
                    case Font.Presets.Quote:
                        font = Fonts.Quote;
                        break;
                    case Font.Presets.Caption:
                        font = Fonts.Caption;
                        break;
                    case Font.Presets.Footnote:
                        font = Fonts.Footnote;
                        break;
                }


            Shape shape = null;
            Block block = null;
            DataSet dataSet = null;
            if (goo.TryGetShape(ref shape))
            {
                shape.Font = font;
                prev_shapes.Add(shape);
                DA.SetData(0, shape);
            }
            else if (goo.TryGetBlock(ref block))
            {
                block.Font = font;
                DA.SetData(0, block);
            }
            else if (goo.TryGetDataSet(ref dataSet))
            {
                dataSet.Font = font;
                DA.SetData(0, dataSet);
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
                return Properties.Resources.Pdf_Format_FontPresets;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("61b33e27-7ee2-40ba-a740-c02877df288a"); }
        }
    }
}