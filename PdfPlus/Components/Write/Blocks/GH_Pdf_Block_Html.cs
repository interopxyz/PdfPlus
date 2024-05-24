using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_Html : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Block_Html class.
        /// </summary>
        public GH_Pdf_Block_Html()
          : base("HTML Block", "HTML Blk",
              "Create a HTML Block" + Environment.NewLine +
                "<h1>Heading 1</h1>" + Environment.NewLine +
                "<h2>Heading 2</h2>" + Environment.NewLine +
                "<h3>Heading 3</h3>" + Environment.NewLine +
                "<h4>Heading 4</h4>" + Environment.NewLine +
                "<h5>Heading 5</h5>" + Environment.NewLine +
                "<h6>Heading 6</h6>" + Environment.NewLine +
                "<hr /> 'Horizontal Rule'" + Environment.NewLine +
                "<strong>Bold</strong>" + Environment.NewLine +
                "<i>Italic</i>" + Environment.NewLine +
                "<u>Underline</u>" + Environment.NewLine +
                "<strong><i>Bold & Italic</i></strong>" + Environment.NewLine +
                "<ol><li>Ordered List Item 1</li></ol>" + Environment.NewLine +
                "<ul><li>Unordered List Item A</li></ul>" + Environment.NewLine +
                "<a href='http://www.duckduckgo.com'>Hyperlink</a>",
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
            pManager.AddTextParameter("HTML", "H", "HTML Text", GH_ParamAccess.item);
            pManager[0].Optional = true;
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
            string text = string.Empty;
            if (!DA.GetData(0, ref text)) return;

            Block block = Block.CreateHtml(text);

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
                return Properties.Resources.Pdf_Block_Html;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8e59f222-ccc5-46c0-a0f1-e0c9026bf65b"); }
        }
    }
}