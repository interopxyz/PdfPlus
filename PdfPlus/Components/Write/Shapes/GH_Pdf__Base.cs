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
        protected List<Page> prev_pages = new List<Page>();

        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Base_ShapePreview class.
        /// </summary>
        public GH_Pdf__Base()
          : base("Base", "Base",
              "Create a Text Shape at a point location",
              Constants.ShortName, Constants.Shapes)
        {
        }

        public GH_Pdf__Base(string Name, string NickName, string Description, string Category, string Subcategory) : base(Name, NickName, Description, Category, Subcategory)
        {
        }

        protected override void BeforeSolveInstance()
        {
            prev_shapes = new List<Shape>();
            prev_pages = new List<Page>();
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

        protected void PrevPageShapes(Page page)
        {
            prev_shapes.AddRange(page.Shapes);
            prev_pages.Add(new Page(page));
        }

        protected void PrevDocumentShapes(Document doc)
        {
            foreach (Page page in doc.Pages)
            {
                foreach(Page subPage in page.RenderBlocksToPages())this.PrevPageShapes(subPage);
                prev_pages.Add(new Page(page));
                prev_shapes.AddRange(page.Shapes);
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if (Hidden) return;
            if (Locked) return;
            double mTol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            int messageSize = 4;
            string messageFont = "Arial";

            Rhino.Display.DisplayMaterial mat = new Rhino.Display.DisplayMaterial();
            if (Attributes.Selected)
            {
                mat = args.ShadeMaterial_Selected;
            }
            else
            {
                mat = args.ShadeMaterial;
            }

            Color activeColor = mat.Diffuse;
            double factor = (72.0 / 96.0);

            foreach (Page page in prev_pages)
            {
                args.Display.DrawPatternedPolyline(page.Boundary.ToNurbsCurve().Points.ControlPolygon(), activeColor, 12, 1, false);
            }

            foreach (Shape shape in prev_shapes)
            {
                Plane plane = Plane.WorldXY;

                Color strokeColor = shape.StrokeColor;
                Color fillColor = shape.FillColor;
                Color fontColor = shape.FontColor;
                if (Attributes.Selected)
                {
                    strokeColor = activeColor;
                    fillColor = activeColor;
                    fontColor = activeColor;
                }
                switch (shape.Type)
                {
                    case Shape.ShapeType.TextObj:
                        plane.Origin = shape.Location;
                        Rhino.Display.Text3d text = new Rhino.Display.Text3d(shape.Text, plane, shape.FontSize * factor);
                        text.FontFace = shape.FontFamily;
                        text.Bold = shape.IsBold;
                        text.Italic = shape.IsItalic;

                        args.Display.Draw3dText(text, fontColor);
                        args.Display.DrawPoint(shape.Location, Rhino.Display.PointStyle.Circle, 2, activeColor);

                        Point3d[] corners = text.BoundingBox.GetCorners();
                        if (shape.IsUnderlined) args.Display.DrawLine(new Line(corners[0], corners[1]), fontColor);

                        if (shape.IsStrikeout) args.Display.DrawLine(new Line((corners[0] + corners[3]) / 2.0, (corners[1] + corners[2]) / 2.0), fontColor);
                        break;
                    case Shape.ShapeType.TextBox:
                        Point3d[] c = shape.PreviewPolyline.ToArray();
                        plane.Origin = c[0]+new Vector3d(0,-4,0);
                        args.Display.Draw3dText("Preview Purposes Only", activeColor, plane, messageSize, messageFont);
                        plane.Origin = c[3] - new Vector3d(0, shape.FontSize * factor * 1.5, 0);
                        List<string> lines = BreakLines(shape.Text, shape.FontFamily, shape.FontSize, shape.Boundary.Width);
                        foreach (string line in lines)
                        {
                            Rhino.Display.Text3d txt = new Rhino.Display.Text3d(line, plane, shape.FontSize * 72.0 / 100.0);
                            txt.FontFace = shape.FontFamily;
                            txt.Bold = shape.IsBold;
                            txt.Italic = shape.IsItalic;

                            args.Display.Draw3dText(txt, fontColor);
                            plane.Origin = plane.Origin + new Vector3d(0, -shape.FontSize * factor * 1.8, 0);
                        }

                        args.Display.DrawDottedPolyline(shape.PreviewPolyline, activeColor, false);
                        break;

                    case Shape.ShapeType.ChartObj:

                        plane.Origin = shape.PreviewPolyline.BoundingBox.Min;
                        args.Display.DrawDottedPolyline(shape.PreviewPolyline, fillColor,false);
                        args.Display.Draw3dText("This preview does not represent actual chart appearance", activeColor, plane, messageSize, messageFont);
                        List<Curve> crvs = shape.RenderChart(out List<Color> clrs);
                        if (Attributes.Selected)
                        {
                            for (int i = 0; i < crvs.Count; i++) args.Display.DrawCurve(crvs[i], activeColor, 1);
                        }
                        else
                        {
                            for (int i = 0; i < crvs.Count; i++) args.Display.DrawCurve(crvs[i], clrs[i], 1);
                        }

                        break;
                    case Shape.ShapeType.ImageObj:
                    case Shape.ShapeType.ImageFrame:

                        if (Attributes.Selected)
                        {
                            args.Display.DrawMeshShaded(RectToMesh(shape.PreviewBoundary), mat);
                        }
                        else
                        {
                            if (shape.ImagePath != "")
                            {
                                Material material = new Material();
                                material.SetBitmapTexture(shape.ImagePath);

                                material.DiffuseColor = Color.White;
                                material.AmbientColor = Color.Black;
                                material.EmissionColor = Color.Black;
                                material.Reflectivity = 0.0;
                                material.ReflectionGlossiness = 0.0;
                                args.Display.DrawMeshShaded(RectToMesh(shape.PreviewBoundary), new Rhino.Display.DisplayMaterial(material));
                            }
                            else
                            {
                                args.Display.DrawMeshFalseColors(MeshColorByBitmap(shape.PreviewBoundary, shape.Image, 2));
                            }
                        }
                        break;
                    case Shape.ShapeType.LinkObj:
                        Curve[] linkCurves = new Curve[] { shape.PreviewBoundary.ToNurbsCurve() };
                        Hatch[] linkHatches = Hatch.Create(linkCurves, 0, 0, 1, mTol);
                        foreach (Hatch hatch in linkHatches) args.Display.DrawHatch(hatch, Color.FromArgb(30,activeColor), shape.FillColor);
                        args.Display.DrawDottedPolyline(shape.PreviewPolyline, strokeColor,false);
                        break;
                    case Shape.ShapeType.Arc:
                        args.Display.DrawCurve(shape.Bezier, strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Bezier:
                        args.Display.DrawCurve(shape.Bezier, strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Circle:
                        args.Display.DrawCurve(shape.Circle.ToNurbsCurve(), strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Ellipse:
                        args.Display.DrawCurve(shape.Ellipse.ToNurbsCurve(), strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Line:
                        args.Display.DrawLine(shape.Line, strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Polyline:
                        args.Display.DrawPolyline(shape.Polyline, strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Brep:
                        Curve[] brepCurves = shape.Brep.DuplicateNakedEdgeCurves(true, true);
                        Hatch[] brepHatches = Hatch.Create(brepCurves, 0, 0, 1, mTol);
                        foreach (Hatch hatch in brepHatches) args.Display.DrawHatch(hatch, fillColor, shape.FillColor);
                        foreach (Curve curve in brepCurves) args.Display.DrawCurve(curve, strokeColor, (int)shape.StrokeWeight);
                        break;
                    case Shape.ShapeType.Mesh:
                        List<Curve> meshCurves = new List<Curve>();
                        Polyline[] meshPlines = shape.Mesh.GetNakedEdges();
                        foreach (Polyline pline in meshPlines) meshCurves.Add(pline.ToNurbsCurve());
                        Hatch[] meshHatches = Hatch.Create(meshCurves, 0, 0, 1, mTol);
                        foreach (Hatch hatch in meshHatches) args.Display.DrawHatch(hatch, fillColor, shape.FillColor);
                        foreach (Curve curve in meshCurves) args.Display.DrawCurve(curve, strokeColor, (int)shape.StrokeWeight);
                        break;
                }
            }

            // Set Display Override
            base.DrawViewportWires(args);
        }
        //Credit: The following function was written by David Rutten
        private List<string> BreakLines(string Text, string Font, double Size, double Width)
        {
            if (string.IsNullOrWhiteSpace(Text)) return null;

            if (string.IsNullOrWhiteSpace(Font))
                throw new ArgumentException("Font name not specified.");

            if (Size <= 1.0)
                throw new ArgumentException("Size needs to be at least 1.0, because the text measuring uses integer arithmetic.");

            if (Width < Size * 10)
                throw new ArgumentException("Container width must be at least 10 times the font size.");

            System.Drawing.Font font = new System.Drawing.Font(Font, (float)Size, System.Drawing.FontStyle.Regular, GraphicsUnit.World);

            string[] words = Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Queue<string> queue = new Queue<string>(words);

            List<string> lines = new List<string>();
            while (queue.Count > 0)
            {
                string line = ExtractLine(queue, font, Width);
                lines.Add(line);
            }

            font.Dispose();
            return lines;
        }

        //Credit: The following function was written by David Rutten
        private string ExtractLine(Queue<string> words, System.Drawing.Font font, double maxWidth)
        {
            // Try the first word. If it is already longer than the maximum width, return immediately.
            string line = words.Dequeue();
            double width = Grasshopper.Kernel.GH_FontServer.StringWidth(line, font);
            if (width >= maxWidth)
                return line;

            // Now add subsequent words one at a time.
            while (true)
            {
                // No words left.
                if (words.Count == 0)
                    return line;

                string longerLine = line + " " + words.Peek();

                width = Grasshopper.Kernel.GH_FontServer.StringWidth(longerLine, font);
                if (width >= maxWidth)
                    return line;

                // We can fit the longer line.
                // Remeber the new line and properly dequeue the appended word.
                line = longerLine;
                words.Dequeue();
            }
        }

        private Mesh MeshColorByBitmap(Rectangle3d rectangle, Bitmap bitmap, int count)
        {
            int xStep = bitmap.Width / count;
            int yStep = bitmap.Height / count;

            Mesh mesh = RectToDenseMesh(rectangle, xStep - 1, yStep - 1);
            List<Color> colors = new List<Color>();

            for (int y = 0; y < bitmap.Height; y += count)
            {
                for (int x = 0; x < bitmap.Width; x += count)
                {
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
            Surface srf = NurbsSurface.CreateFromCorners(p[0], p[1], p[2], p[3]);

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