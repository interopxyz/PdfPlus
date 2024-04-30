using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Shape_ImagePt : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_AddImagePt class.
        /// </summary>
        public GH_Pdf_Shape_ImagePt()
          : base("Image Shape", "Img Shp",
              "Create an Image Shape at a point location",
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
            pManager.AddGenericParameter("Image", "I", "The System.Drawing.Bitmap or Image Filepath to display", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Frame", "F", "The placement plane for the image. The Origin of the plane and XL Plane rotation angle will apply to the image", GH_ParamAccess.item); pManager.AddNumberParameter("Scale", "S", "A unitized scale factor (0-1) for the image", GH_ParamAccess.item, 1.0);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Output, GH_ParamAccess.item);
            pManager.AddRectangleParameter("Boundary", "B", "The image boundary in points", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_Goo goo = null;
            Sd.Bitmap bitmap = null;
            string path = "";

            if (!DA.GetData(0, ref goo)) return;
            if (!goo.TryGetBitmap(ref bitmap,ref path)) return;

            Plane p = Plane.WorldXY;
            if (!DA.GetData(1, ref p)) return;

            double scale = 1.0;
            DA.GetData(2, ref scale);

            double w = bitmap.Width * (72.0 / 96.0) * scale;
            double h = bitmap.Height * (72.0 / 96.0) * scale;

            Rectangle3d rect = new Rectangle3d(Plane.WorldXY, p.Origin, new Point3d(p.OriginX + w, p.OriginY + h, 0));

            Shape shape = Shape.CreateImage(bitmap, rect, path);
            shape.Angle = Vector3d.VectorAngle(Vector3d.XAxis, p.XAxis, Plane.WorldXY) / Math.PI * 180.0;

            Rectangle3d bnd = new Rectangle3d(Plane.WorldXY, p.Origin, new Point3d(p.OriginX + w, p.OriginY + h, 0));
            bnd.Transform(Transform.Rotation(shape.Angle / 180.0 * Math.PI, p.Origin));

            //prev_shapes.Add(shape);
            DA.SetData(0, shape);
            DA.SetData(1, bnd);
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
                return Properties.Resources.Pdf_Shape_Image_Point;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d640f163-bd2c-449b-855f-014dbdef46ad"); }
        }
    }
}