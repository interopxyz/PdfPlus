using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components.Write.Formatting
{
    public class GH_Pdf_Format_Paragraph : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Format_Paragraph class.
        /// </summary>
        public GH_Pdf_Format_Paragraph()
          : base("Set Paragraph Format", "Set Paragraph",
              "Get and Set the Paragraph properties of PDF+ Text Blocks.  ",
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
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, "A PDF+ Shape, Block, DataSet, Text Fragment Element, or Text (String)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Line Spacing", "S", "Optional line spacing value", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Spacing Before", "B", "Optional spacing before the paragraph", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Spacing After", "A", "Optional spacing after the paragraph", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("Indentation Left", "L", "Optional indentation to the left of the paragraph", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Indentation Right", "R", "Optional indentation to the right of the paragraph", GH_ParamAccess.item);
            pManager[6].Optional = true;
            pManager.AddIntegerParameter("Horizontal Border", "H", "Optional paragraph horizontal border setting", GH_ParamAccess.item);
            pManager[7].Optional = true;
            pManager.AddIntegerParameter("Vertical Border", "V", "Optional paragraph vertical border setting", GH_ParamAccess.item);
            pManager[8].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[1];
            foreach (Justification value in Enum.GetValues(typeof(Justification)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }

            Param_Integer paramB = (Param_Integer)pManager[7];
            paramB.AddNamedValue("Both", 0);
            paramB.AddNamedValue("Left", 1);
            paramB.AddNamedValue("Right", 2);
            paramB.AddNamedValue("None", 3);

            Param_Integer paramC = (Param_Integer)pManager[8];
            paramC.AddNamedValue("Both", 0);
            paramC.AddNamedValue("Top", 1);
            paramC.AddNamedValue("Bottom", 2);
            paramC.AddNamedValue("None", 3);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, "A PDF+ Shape, Block, DataSet, or Text Fragment Element", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item);
            pManager.AddNumberParameter("Line Spacing", "S", "Optional line spacing value", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spacing Before", "B", "Optional spacing before the paragraph", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spacing After", "A", "Optional spacing after the paragraph", GH_ParamAccess.item);
            pManager.AddNumberParameter("Indentation Left", "L", "Optional indentation to the left of the paragraph", GH_ParamAccess.item);
            pManager.AddNumberParameter("Indentation Right", "R", "Optional indentation to the right of the paragraph", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Horizontal Borders", "H", "Optional line spacing value", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Vertical Borders", "V", "Optional line spacing value", GH_ParamAccess.item);
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

            int justification = 0;
            if (DA.GetData(1, ref justification)) font.Justification = (Justification)justification;
            DA.SetData(1, font.Justification);

            double linespacing= 1.0;
            if (DA.GetData(2, ref linespacing)) font.LineSpacing = linespacing;
            DA.SetData(2, font.LineSpacing);

            double spacingbefore = 0.0;
            if (DA.GetData(3, ref spacingbefore)) font.SpaceBefore = spacingbefore;
            DA.SetData(3, font.SpaceBefore);

            double spacingafter = 0.0;
            if (DA.GetData(4, ref spacingafter)) font.SpaceAfter = spacingafter;
            DA.SetData(4, font.SpaceAfter);

            double indentleft = 0.0;
            if (DA.GetData(5, ref indentleft)) font.IndentLeft = indentleft;
            DA.SetData(5, font.IndentLeft);

            double indentright = 0.0;
            if (DA.GetData(6, ref indentright)) font.IndentRight = indentright;
            DA.SetData(6, font.IndentRight);

            int borderhorizontal = 0;
            bool hasBorderH = DA.GetData(7, ref borderhorizontal) ;

            int bordervertical = 0;
            bool hasBorderV = DA.GetData(8, ref bordervertical);

            switch (elem.ElementType)
            {
                case Element.ElementTypes.Block:
                    Block block = null;
                    goo.TryGetBlock(ref block);
                    block.Font = font;

                    if (hasBorderH) block.HorizontalBorderStyle = GetStyle(borderhorizontal);
                    DA.SetData(7, (int)block.HorizontalBorderStyle);
                    if (hasBorderV) block.VerticalBorderStyle = GetStyle(bordervertical);
                    DA.SetData(8, (int)block.VerticalBorderStyle);

                    DA.SetData(0, block);
                    break;
                default:
                    Fragment fragment = null;
                    if(!goo.TryGetFragment(ref fragment)) return;
                    fragment.Font = font;

                    if (hasBorderH) fragment.HorizontalBorderStyle = GetStyle(borderhorizontal);
                    DA.SetData(7, (int)fragment.HorizontalBorderStyle);
                    if (hasBorderV) fragment.VerticalBorderStyle = GetStyle(bordervertical);
                    DA.SetData(8, (int)fragment.VerticalBorderStyle);

                    DA.SetData(0, fragment);
                    break;
            }
        }

        protected Element.BorderStyles GetStyle(int index)
        {
            switch (index)
            {
                default:
                    return Element.BorderStyles.None;
                case 0:
                    return Element.BorderStyles.All;
                case 1:
                    return Element.BorderStyles.Start;
                case 2:
                    return Element.BorderStyles.End;
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
                return Properties.Resources.Pdf_Format_Paragraph;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1774216d-cb85-49a8-9888-467f58e84e1e"); }
        }
    }
}