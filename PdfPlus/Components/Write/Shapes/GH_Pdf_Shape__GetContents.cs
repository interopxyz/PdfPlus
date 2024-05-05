using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Shapes
{
    public class GH_Pdf_Shape__GetContents : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Shape_GetContents class.
        /// </summary>
        public GH_Pdf_Shape__GetContents()
          : base("Get Shape Contents", "Contents",
              "Get geometric, text, and other Shape contents",
              Constants.ShortName, Constants.Shapes)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Input, GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary", "B", "Shape Boundary", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "L", "Shape Location Point", GH_ParamAccess.item);
            pManager.AddTextParameter("Text", "T", "Text contents when applicable", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Geometry", "G", "Geometry contents when applicable", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_Goo goo = null;
            if (!DA.GetData(0, ref goo)) return;
            Shape shape = null;
            if(!goo.TryGetShape(ref shape))return;

            if(shape.Boundary.IsValid) DA.SetData(0, shape.Boundary);
            if (shape.Location.IsValid) DA.SetData(1, shape.Location);
            if(shape.Text!="") DA.SetData(2, shape.Text);
            if (shape.Geometry!=null) DA.SetData(3, shape.Geometry);

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
                return Properties.Resources.Pdf_Shape_Contents;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8632b176-4bfa-4df5-ac1e-4c6796c2bd17"); }
        }
    }
}