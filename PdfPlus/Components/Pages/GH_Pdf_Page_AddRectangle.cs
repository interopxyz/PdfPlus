using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Pages
{
    public class GH_Pdf_Page_AddRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_AddRectangle class.
        /// </summary>
        public GH_Pdf_Page_AddRectangle()
          : base("Add Page Boundary", "Page Rect",
              "Create a new PDF Page from a boundary rectangle in Point units",
              Constants.ShortName, Constants.SubPage)
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
            pManager.AddRectangleParameter("Boundary", "B", "The page boundary in Points", GH_ParamAccess.item);
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
            Rectangle3d boundary = new Rectangle3d();
            if(!DA.GetData(0, ref boundary))return;

            Point3d ptA = boundary.Corner(0);
            Point3d ptB = boundary.Corner(1);
            Point3d ptC = boundary.Corner(3);
            Page page = new Page(Units.Point, ptA.DistanceTo(ptB), ptA.DistanceTo(ptC));

            Plane plane = new Plane(ptA, ptB, ptC);
            page.Frame = new Plane(plane);

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
                return Properties.Resources.Pdf_Page_Boundary_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("331173ec-eb7d-4c09-851f-87131c6ec061"); }
        }
    }
}