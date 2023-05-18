using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PdfPlus.Components
{
    public abstract class GH_Pdf__Base : GH_Component
    {
        protected List<Shape> prev_shapes = new List<Shape>();

        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Base_ShapePreview class.
        /// </summary>
        public GH_Pdf__Base()
          : base("Base", "Base",
              "Create a Text Shape at a point location",
              Constants.ShortName, Constants.WritePage)
        {
        }

        public GH_Pdf__Base(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        protected override void BeforeSolveInstance()
        {
            prev_shapes = new List<Shape>();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("92db2d26-2a13-4992-a450-f20704b41a73"); }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            double mTol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            foreach (Shape shape in prev_shapes)
            {
                Plane plane = Plane.WorldXY;

                switch (shape.Type)
                {
                    case Shape.ShapeType.TextObj:
                        plane.Origin = shape.Location;
                        Rhino.Display.Text3d text = new Rhino.Display.Text3d(shape.TextContent, plane, shape.FontSize * 0.663);
                        text.FontFace = shape.FontFamily;
                        text.Bold = shape.IsBold;
                        text.Italic = shape.IsItalic;

                        args.Display.Draw3dText(text, shape.FontColor);
                        args.Display.DrawPoint(shape.Location, Rhino.Display.PointStyle.Circle, 2, shape.FontColor);

                        Point3d[] corners = text.BoundingBox.GetCorners();
                        if (shape.IsUnderlined) args.Display.DrawLine(new Line(corners[0], corners[1]), shape.FontColor);

                        if (shape.IsStrikeout) args.Display.DrawLine(new Line((corners[0]+ corners[3])/2.0, (corners[1] + corners[2]) / 2.0), shape.FontColor);
                        break;
                    case Shape.ShapeType.TextBox:

                        Point3d[] c = shape.Boundary.ToNurbsCurve().Points.ControlPolygon().ToArray();
                        plane.Origin = c[3] - new Vector3d(0, shape.FontSize * 0.663,0);

                        Rhino.Display.Text3d txt = new Rhino.Display.Text3d(shape.TextContent, plane, shape.FontSize * 0.663);
                        txt.FontFace = shape.FontFamily;
                        txt.Bold = shape.IsBold;
                        txt.Italic = shape.IsItalic;
                        
                        args.Display.Draw3dText(txt, shape.FontColor);

                        args.Display.DrawDottedPolyline(shape.Boundary.ToNurbsCurve().Points.ControlPolygon(), shape.FontColor,false);

                        break;

                    case Shape.ShapeType.ChartObj:

                        args.Display.DrawPatternedPolyline(shape.Boundary.ToNurbsCurve().Points.ControlPolygon(), shape.FillColor, 1000,1, false);

                        break;
                    case Shape.ShapeType.ImageObj:
                    case Shape.ShapeType.ImageFrame:
                        
                        if (shape.ImagePath != "")
                        {
                            Material material = new Material();
                            material.SetBitmapTexture(shape.ImagePath);

                            material.DiffuseColor = Color.White;
                            material.AmbientColor = Color.Black;
                            material.EmissionColor = Color.Black;
                            material.Reflectivity = 0.0;
                            material.ReflectionGlossiness = 0.0;
                            args.Display.DrawMeshShaded(RectToMesh(shape.Boundary), new Rhino.Display.DisplayMaterial(material));
                        }
                        else
                        {
                            args.Display.DrawMeshFalseColors(MeshColorByBitmap(shape.Boundary, shape.Image, 2));
                        }
                        break;
                }
            }

            // Set Display Override
            base.DrawViewportWires(args);
        }
        
        private Mesh MeshColorByBitmap(Rectangle3d rectangle, Bitmap bitmap, int count)
        {
            int xStep = bitmap.Width / count;
            int yStep = bitmap.Height / count;

            Mesh mesh = RectToDenseMesh(rectangle, xStep-1, yStep-1);
            List<Color> colors = new List<Color>();

            for (int y = 0; y < bitmap.Height; y += yStep){
                for (int x = 0; x < bitmap.Width; x += xStep){
                    colors.Add(bitmap.GetPixel(x, y));
                }
            }

            mesh.VertexColors.Clear();
            mesh.VertexColors.AppendColors(colors.ToArray());

            return mesh;
        }

        private Line[] RectToLines(Rectangle3d rectangle)
        {
            Line[] output = new Line[3];
            Point3d[] p = rectangle.ToNurbsCurve().Points.ControlPolygon().ToArray();
            output[0] = new Line(p[0], p[1]);
            output[1] = new Line(p[1], p[2]);
            output[2] = new Line(p[2], p[3]);
            output[3] = new Line(p[3], p[0]);

            return output;
        }

        private Surface RectToSurface(Rectangle3d rectangle)
        {
            Point3d[] p = rectangle.ToNurbsCurve().Points.ControlPolygon().ToArray();
            Surface srf = NurbsSurface.CreateFromCorners(p[0],p[1],p[2],p[3]);

            return srf;
        }

        private Mesh RectToDenseMesh(Rectangle3d rectangle, int x, int y)
        {
            Mesh mesh = Mesh.CreateFromPlane(rectangle.Plane, rectangle.X, rectangle.Y, x, y);

            return mesh;
        }

        private Mesh RectToMesh(Rectangle3d rectangle)
        {
            Mesh mesh = Mesh.CreateFromPlane(rectangle.Plane, rectangle.X, rectangle.Y, 1, 1);
            return mesh;
        }
    }
}