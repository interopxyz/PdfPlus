using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using Sd = System.Drawing;

namespace PdfPlus.Components
{
    public class GH_PDF_Shape_Shape_Placeholder : GH_Component
    {
        List<Page> pages = new List<Page>();

        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_AddGeometry class.
        /// </summary>
        public GH_PDF_Shape_Shape_Placeholder()
          : base("Add Framed Contents", "Framed Content",
              "Add text, image, or geometric based shapes to a PDF Page, it will be moved and scaled to fit the frame",
              Constants.ShortName, Constants.WritePage)
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

            pManager.AddGenericParameter(Constants.Page.Name, Constants.Page.NickName, Constants.Page.Input, GH_ParamAccess.item);
            pManager.AddGenericParameter("Content", "C", "Shapes, Text, or Geometry to add to the document", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddRectangleParameter("Frame", "Frame", "Frame", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Page.Name, Constants.Page.NickName, Constants.Page.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            if (RunCount == 1)
            {
                pages.Clear();
            }


            Page page = null;
            if (!DA.GetData(0, ref page)) return;
            page = new Page(page);

            List<IGH_Goo> geometry = new List<IGH_Goo>();
            if (!DA.GetDataList(1, geometry)) return;

            Rectangle rect = default;
            bool hadRect = DA.GetData(2, ref rect);


            int shapeCount = page.shapes.Count;

            foreach (IGH_Goo goos in geometry)
            {
                page.AddShape(goos);

            }

            if (hadRect)
            {
                // Get boundingbox of all shapes:

                BoundingBox b = default;
                for (int i = shapeCount; i < page.shapes.Count; i++)
                {
                    if(i == shapeCount)
                    {
                        b = page.shapes[i].boundary.BoundingBox;
                        continue;
                    }

                    b.Union(page.shapes[i].boundary.BoundingBox);

                    
                }

                Point3d center = b.Center;

                Point3d frameCenter = new Point3d(rect.X + rect.Width * 0.5, rect.Y + rect.Height * 0.5, 0.0);

                Vector3d movementVector = frameCenter - center;

                int[] scales = new[] { 1, 5, 10, 20, 50, 100, 200, 500, 1000 };

                // TODO INSERT PTS TO MM



                for (int i = shapeCount; i < page.shapes.Count; i++)
                {

                    //page.shapes[i].Crop(rect);
                }
            }



            pages.Add(page);

            DA.SetData(0, page);
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
                return Properties.Resources.Pdf_Page_AddContent4_01;
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            foreach (var page in pages)
            {
                foreach (var item in page.shapes)
                {
                    item.DrawInViewport(args.Display);
                }
            }
            base.DrawViewportMeshes(args);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7e6f9fc6-71cb-4079-902b-9fd203292a38"); }
        }
    }
}