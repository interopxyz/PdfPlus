using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Contents
{
    public class GH_Pdf_Shape_Link : GH_Pdf__Base
    {

        string[] descriptions = new string[] { "The hyperlink to link to", "The document name to link to", "The page index (integer only) to link to" };
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Shape_Link class.
        /// </summary>
        public GH_Pdf_Shape_Link()
          : base("Link Region", "Link",
              "Create a link to a document, hyperlink, or page within a rectangular boundary",
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
            pManager.AddIntegerParameter("Target", "T", "The target of the link", GH_ParamAccess.item, 0);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Link", "L", descriptions[0], GH_ParamAccess.item,"");
            pManager[1].Optional = true;
            pManager.AddRectangleParameter("Boundary", "B", "The rectangular boundary of the Shape", GH_ParamAccess.item);

            Param_Integer paramA = (Param_Integer)pManager[0];
            foreach (Shape.LinkTypes value in Enum.GetValues(typeof(Shape.LinkTypes)))
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
            int type = 0;
            if (!DA.GetData(0, ref type)) return;

            string link = "";
            if (!DA.GetData(1, ref link)) return;

            Rectangle3d boundary = new Rectangle3d();
            if (!DA.GetData(2, ref boundary)) return;

            Shape shape = new Shape(link, boundary, (Shape.LinkTypes)type);

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
                return Properties.Resources.PDF_Link_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("413ae7fd-0b46-4c15-b17c-dffedbe7f6c9"); }
        }
    }
}