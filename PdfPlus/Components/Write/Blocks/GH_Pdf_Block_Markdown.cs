using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_Markdown : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Block_Markdown class.
        /// </summary>
        public GH_Pdf_Block_Markdown()
          : base("Markdown Block", "Mkd Blk",
              "Create a Markdown Block" + Environment.NewLine +
                "# Heading 1" + Environment.NewLine +
"## Heading 2" + Environment.NewLine +
"### Heading 3" + Environment.NewLine +
"#### Heading 4" + Environment.NewLine +
"##### Heading 5" + Environment.NewLine +
"###### Heading 6" + Environment.NewLine +
"*** 'Horizontal Rule'" + Environment.NewLine +
"--- 'Horizontal Rule'" + Environment.NewLine +
"___ 'Horizontal Rule'" + Environment.NewLine +
"**Bold**" + Environment.NewLine +
"*Italic*" + Environment.NewLine +
"***Bold & Italic***" + Environment.NewLine +
"___Bold & Italic___" + Environment.NewLine +
"__*Bold & Italic*__" + Environment.NewLine +
"**_Bold & Italic_ **" + Environment.NewLine +
"1.Numbered List Item" + Environment.NewLine +
"- Unordered List Item" + Environment.NewLine +
"* Unordered List Item" + Environment.NewLine +
"+ Unordered List Item" + Environment.NewLine +
"[Hyperlink](https://duckduckgo.com)" + Environment.NewLine +
"\\ \\` \\* \\_ \\{ } \\[] \\<> \\() \\# \\+ \\- \\. \\! \\| ",
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
            pManager.AddTextParameter("Markdown", "M", "Markdown Text", GH_ParamAccess.item);
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

            Block block = Block.CreateMarkdown(text);

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
                return Properties.Resources.Pdf_Block_MarkDown;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("af7121bf-14ab-4dce-9f29-24882f729539"); }
        }
    }
}