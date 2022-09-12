using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components.Contents
{
    public class GH_Pdf_Shape_AddImage : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_AddImage class.
        /// </summary>
        public GH_Pdf_Shape_AddImage()
          : base("Image Frame", "Img Frame",
              "Create an Image Shape within a rectangular boundary",
              Constants.ShortName, Constants.WritePage)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Image", "I", "The (bitmap) image to display", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Boundary", "B", "The boundary of the image", GH_ParamAccess.item);
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
            Sd.Bitmap bitmap = null;
            
            if (!DA.GetData(0, ref goo)) return;
            if (!goo.TryGetBitmap(ref bitmap)) return;

            Rectangle3d boundary = new Rectangle3d();
            if (!DA.GetData(1, ref boundary)) return;

            Shape text = new Shape(bitmap, boundary);

            DA.SetData(0, text);
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
                return Properties.Resources.Pdf_Content_Image_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e191f171-6313-4342-959c-db8ab28cdd0c"); }
        }
    }
}