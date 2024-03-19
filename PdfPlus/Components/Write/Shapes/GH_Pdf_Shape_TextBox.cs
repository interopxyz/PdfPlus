using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Shape_TextBox : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_AddText class.
        /// </summary>
        public GH_Pdf_Shape_TextBox()
          : base("Text Box", "Txt Box",
              "Create a Text Shape within a rectangular boundary",
              Constants.ShortName, Constants.Shapes)
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
            pManager.AddRectangleParameter("Boundary", "B", "The rectangular boundary of the Shape", GH_ParamAccess.item);
            pManager.AddTextParameter("Content", "T", "The content of the text", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Alignment", "A", "The paragraph alignment", GH_ParamAccess.item,0);
            pManager[2].Optional = true;

            Param_Integer paramA = (Param_Integer)pManager[2];
            foreach (Alignment value in Enum.GetValues(typeof(Alignment)))
            {
                paramA.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Output,GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rectangle3d boundary = new Rectangle3d();
            if (!DA.GetData(0, ref boundary)) return;

            string content = string.Empty;
            if (!DA.GetData(1, ref content)) return;

            int alignment = 0;
            if (!DA.GetData(2, ref alignment)) return;


            Shape shape = Shape.CreateText(content, boundary,(Alignment)alignment, new Font());

            prev_shapes.Add(shape);
            DA.SetData(0, shape);
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
                return Properties.Resources.Pdf_Content_Text_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7ecff30f-8500-4d86-96a1-e4effdcc81c2"); }
        }
    }
}