using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static PdfPlus.Shape;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PdfPlus.Components
{
    public class GH_Pdf_Page_AddContents : GH_Component
    {
        List<Page> pages = new List<Page>();

        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_AddGeometry class.
        /// </summary>
        public GH_Pdf_Page_AddContents()
          : base("Set Contents", "Content",
              "Add text, image, or geometric based shapes to a PDF Page.",
              Constants.ShortName, Constants.WritePage)
        {
        }

        Transform movematrix;
        Page page;

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
            //movematrix = Transform.Translation(Vector3d.Zero);

            if (RunCount == 1)
            {
                pages.Clear();
            }


            Page page = null;
            if (!DA.GetData(0, ref page))return;
            page = new Page(page);

            List<IGH_Goo> geometry = new List<IGH_Goo>();
            if (!DA.GetDataList(1, geometry)) return;

            foreach(IGH_Goo goos in geometry)
            {
                page.AddShape(goos);

                ////transformation for preview
                //Rhino.Geometry.Plane plane = Rhino.Geometry.Plane.WorldZX;
                //plane.OriginY = page.BaseObject.Height.Point / 2.0;

                //Rhino.Geometry.Plane frame = Rhino.Geometry.Plane.WorldXY;
                //frame.Transform(Transform.Mirror(plane));
                //movematrix = Transform.PlaneToPlane(page.Frame, frame);

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
                    
                    item.DrawInViewport(args.Display, page);
                }
        }
            base.DrawViewportMeshes(args);
        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7e649fc6-716b-4079-90eb-9fd203298a38"); }
        }
    }
}