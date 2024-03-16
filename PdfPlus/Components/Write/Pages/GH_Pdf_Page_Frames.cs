using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PdfPlus.Components
{
    public class GH_Pdf_Page_Frames : GH_Component
    {
        protected List<Page> prev_pages = new List<Page>();

        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Page_Frames class.
        /// </summary>
        public GH_Pdf_Page_Frames()
          : base("Page Boundaries", "Boundaries",
              "Get or modify the boundaries of a page",
              Constants.ShortName, Constants.PdfSharp)
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

            pManager.AddRectangleParameter("Art Box", "A", "The art box of the page", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.HideParameter(1);

            pManager.AddRectangleParameter("Bleed Box", "B", "The bleed box of the page", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.HideParameter(2);

            pManager.AddRectangleParameter("Crop Box", "C", "The crop box of the page",GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.HideParameter(3);

            pManager.AddRectangleParameter("Trim Box", "T", "The trim box of the page", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.HideParameter(4);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Page.Name, Constants.Page.NickName, Constants.Page.Output, GH_ParamAccess.item);

            pManager.AddRectangleParameter("Art Box", "A", "The art box of the page"+Environment.NewLine+ "Yellow preview display color", GH_ParamAccess.item);
            pManager.HideParameter(1);

            pManager.AddRectangleParameter("Bleed Box", "B", "The bleed box of the page" + Environment.NewLine + "Cyan preview display color", GH_ParamAccess.item);
            pManager.HideParameter(2);

            pManager.AddRectangleParameter("Crop Box", "C", "The crop box of the page" + Environment.NewLine + "Black preview display color", GH_ParamAccess.item);
            pManager.HideParameter(3);

            pManager.AddRectangleParameter("Trim Box", "T", "The trim box of the page" + Environment.NewLine + "Magenta preview display color", GH_ParamAccess.item);
            pManager.HideParameter(4);
        }

        protected override void BeforeSolveInstance()
        {
            prev_pages = new List<Page>();
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Page page = null;
            if (!DA.GetData(0, ref page)) return;
            page = new Page(page);

            Rectangle3d artbox = new Rectangle3d();
            if (DA.GetData(1, ref artbox)) page.ArtBox = artbox;

            Rectangle3d bleedbox = new Rectangle3d();
            if (DA.GetData(2, ref bleedbox)) page.BleedBox = bleedbox;

            Rectangle3d cropbox = new Rectangle3d();
            if (DA.GetData(3, ref cropbox)) page.CropBox = cropbox;

            Rectangle3d trimbox = new Rectangle3d();
            if (DA.GetData(4, ref trimbox)) page.TrimBox = trimbox;

            prev_pages.Add(new Page(page));
            DA.SetData(0, page);
            DA.SetData(1, page.ArtBox);
            DA.SetData(2, page.BleedBox);
            DA.SetData(3, page.CropBox);
            DA.SetData(4, page.TrimBox);

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
                return Properties.Resources.PDF_Border_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6ca6613e-f5a3-4f72-8737-fe6d8d116189"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (Hidden) return;
            if (Locked) return;

            Plane plane = Plane.WorldXY;
            int messageSize = 4;
            string messageFont = "Arial";

            Color mediaBox = args.WireColour;
            Color bleedBox = Color.DarkCyan;
            Color trimBox = Color.MediumVioletRed;
            Color cropBox = Color.Black;
            Color artBox = Color.Gold;

            if (Attributes.Selected)
            {
                mediaBox = args.WireColour_Selected;
                bleedBox = args.WireColour_Selected;
                trimBox = args.WireColour_Selected;
                cropBox = args.WireColour_Selected;
                artBox = args.WireColour_Selected;
            }

            foreach (Page page in prev_pages)
            {
                plane = page.Frame;
                plane.Origin = page.Boundary.Corner(3);
                args.Display.Draw3dText("Media", mediaBox, plane, messageSize, messageFont);
                args.Display.DrawPatternedPolyline(page.Boundary.ToNurbsCurve().Points.ControlPolygon(), mediaBox, 12, 1, false);

                plane.Origin = page.CropBox.Corner(3) + new Vector3d(16, 0, 0);
                args.Display.Draw3dText("Crop", cropBox, plane, messageSize, messageFont);
                args.Display.DrawPatternedPolyline(page.CropBox.ToNurbsCurve().Points.ControlPolygon(), cropBox, 12, 1, false);

                plane.Origin = page.TrimBox.Corner(3) + new Vector3d(29, 0, 0);
                args.Display.Draw3dText("Trim", trimBox, plane, messageSize, messageFont);
                args.Display.DrawPatternedPolyline(page.TrimBox.ToNurbsCurve().Points.ControlPolygon(), trimBox, 12, 1, false);

                plane.Origin = page.BleedBox.Corner(3)+new Vector3d(42,0,0);
                args.Display.Draw3dText("Bleed", bleedBox, plane, messageSize, messageFont);
                args.Display.DrawPatternedPolyline(page.BleedBox.ToNurbsCurve().Points.ControlPolygon(), bleedBox, 12, 1, false);

                plane.Origin = page.ArtBox.Corner(3) + new Vector3d(58, 0, 0);
                args.Display.Draw3dText("Art", artBox, plane, messageSize, messageFont);
                args.Display.DrawPatternedPolyline(page.ArtBox.ToNurbsCurve().Points.ControlPolygon(), artBox, 12, 1, false);
            }

            // Set Display Override
            base.DrawViewportWires(args);
        }
    }
}