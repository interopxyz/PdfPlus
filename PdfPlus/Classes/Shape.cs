using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;
using Pf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;
using Pl = PdfSharp.Drawing.Layout;
using Rg = Rhino.Geometry;
using System.IO;

namespace PdfPlus
{
    public class Shape
    {
        #region members
        public enum ShapeType { None,Line,Polyline,Bezier,Ellipse,Arc, Brep, Mesh, TextBox, ImageFrame, TextObj, ImageObj};
        protected ShapeType shapeType = ShapeType.None;

        protected Graphic graphic = new Graphic();
        protected Font font = new Font();

        protected Sd.Bitmap image = new Sd.Bitmap(10,10);

        protected string content = string.Empty;
        protected Alignment alignment = Alignment.Left;

        protected Rg.Point3d location = new Rg.Point3d();
        protected Rg.Polyline polyline = new Rg.Polyline();
        protected Rg.Line line = new Rg.Line();
        protected Rg.Arc arc = new Rg.Arc();
        protected Rg.Rectangle3d boundary = new Rg.Rectangle3d();
        protected Rg.NurbsCurve curve = new Rg.NurbsCurve(3,2);

        protected Rg.Brep brep = new Rg.Brep();
        protected Rg.Mesh mesh = new Rg.Mesh();

        #endregion

        #region constructors

        public Shape(Shape shape)
        {
            this.graphic = new Graphic(shape.graphic);
            this.font = new Font(shape.font);

            this.shapeType = shape.shapeType;

            this.content = shape.content;
            this.alignment = shape.alignment;

            this.image = new Sd.Bitmap(shape.image);

            this.location = new Rg.Point3d(shape.location);
            this.polyline = shape.polyline.Duplicate();
            this.boundary = new Rg.Rectangle3d(shape.boundary.Plane, shape.boundary.Corner(0), shape.boundary.Corner(2));
            this.line = new Rg.Line(shape.line.From,shape.line.To);
            this.curve = new Rg.NurbsCurve(shape.curve);

            this.brep = shape.brep.DuplicateBrep();
            this.mesh = shape.mesh.DuplicateMesh();
        }

        #region text
        public Shape(string content, Rg.Rectangle3d boundary, Alignment alignment, Font font)
        {
            shapeType = ShapeType.TextBox;

            this.content = content;
            this.alignment = alignment;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0),boundary.Corner(2));
            this.font = new Font(font);
        }

        public Shape(string content, Rg.Point3d location, Font font)
        {
            shapeType = ShapeType.TextObj;

            this.content = content;
            this.location = new Rg.Point3d(location);
            this.font = new Font(font);
        }

        #endregion

        #region image

        public Shape(Sd.Bitmap bitmap, Rg.Rectangle3d boundary)
        {
            shapeType = ShapeType.ImageFrame;

            this.image = new Sd.Bitmap(bitmap);
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));
        }

        public Shape(Sd.Bitmap bitmap, Rg.Point3d location)
        {
            shapeType = ShapeType.ImageObj;

            this.image = new Sd.Bitmap(bitmap);
            this.location = new Rg.Point3d(location);
        }

        #endregion

        #region geometry

        public Shape(Rg.Polyline polyline, Graphic graphic)
        {
            shapeType = ShapeType.Polyline;

            this.polyline = polyline.Duplicate();

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Line line, Graphic graphic)
        {
            shapeType = ShapeType.Line;

            this.line = new Rg.Line(line.From, line.To);

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Arc arc, Graphic graphic)
        {
            shapeType = ShapeType.Bezier;

            this.curve = arc.ToNurbsCurve();
            this.curve.MakePiecewiseBezier(true);

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Circle circle, Graphic graphic)
        {
            shapeType = ShapeType.Ellipse;

            this.boundary = new Rg.Rectangle3d(circle.Plane, new Rg.Interval(-circle.Radius, circle.Radius), new Rg.Interval(-circle.Radius, circle.Radius));

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Ellipse ellipse, Graphic graphic)
        {
            shapeType = ShapeType.Bezier;

            this.curve = ellipse.ToNurbsCurve();
            this.curve.MakePiecewiseBezier(true);

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.BezierCurve bezier, Graphic graphic)
        {
            shapeType = ShapeType.Bezier;

            this.curve = bezier.ToNurbsCurve();

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Curve curve, Graphic graphic)
        {
            shapeType = ShapeType.Bezier;

            this.curve = new Rg.NurbsCurve(curve.ToNurbsCurve());
            this.curve.MakePiecewiseBezier(true);

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.NurbsCurve curve, Graphic graphic)
        {
            shapeType = ShapeType.Bezier;

            this.curve = new Rg.NurbsCurve(curve);
            this.curve.MakePiecewiseBezier(true);

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Brep brep, Graphic graphic)
        {
            shapeType = ShapeType.Brep;

            this.brep = brep.DuplicateBrep();

            this.graphic = new Graphic(graphic);
        }

        public Shape(Rg.Mesh mesh, Graphic graphic)
        {
            shapeType = ShapeType.Mesh;

            this.mesh = mesh.DuplicateMesh();

            this.graphic = new Graphic(graphic);
        }

        #endregion

        #endregion

        #region properties

        public virtual ShapeType Type
        {
            get { return this.shapeType; }
        }

        public virtual Sd.Color FontColor
        {
            get { return this.font.Color; }
            set { this.font.Color = value; }
        }

        public virtual string FontFamily
        {
            get { return this.font.Family; }
            set { this.font.Family = value; }
        }

        public virtual double FontSize
        {
            get { return this.font.Size; }
            set { this.font.Size = value; }
        }

        public virtual Sd.Color FillColor
        {
            get { return this.graphic.Color; }
            set { this.graphic.Color = value; }
        }

        public virtual Sd.Color StrokeColor
        {
            get { return this.graphic.Stroke; }
            set { this.graphic.Stroke = value; }
        }

        public virtual double StrokeWeight
        {
            get { return this.graphic.Weight; }
            set { this.graphic.Weight = value; }
        }

        public virtual FontStyle Style
        {
            get { return this.font.Style; }
            set { this.font.Style = value; }
        }

        #endregion

        #region methods

        public void SetPattern(string pattern)
        {
            List<double> numbers = new List<double>();
            string[] values = pattern.Split(',');

            foreach(string val in values)
            {
                if (double.TryParse(val, out double d)) numbers.Add(d);
            }
            this.graphic.Pattern = numbers;
        }

        public void AlignContent(Page page)
        {

            Rg.Plane plane = Rg.Plane.WorldZX;
            plane.OriginY = page.BaseObject.Height.Point / 2.0;

            Rg.Plane frame = Rg.Plane.WorldXY;
            frame.Transform(Rg.Transform.Mirror(plane));
            switch (this.shapeType)
            {
                case ShapeType.ImageObj:
                case ShapeType.TextObj:
                    this.location.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;

                case ShapeType.TextBox:
                case ShapeType.ImageFrame:
                case ShapeType.Ellipse:
                    this.boundary.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;

                case ShapeType.Arc:
                case ShapeType.Bezier:
                    this.curve.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;

                case ShapeType.Line:
                    this.line.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;

                case ShapeType.Polyline:
                    this.polyline.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;

                case ShapeType.Brep:
                    this.brep.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;

                case ShapeType.Mesh:
                    this.mesh.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;
            }
        }

        public void Render(Pd.XGraphics graph)
        {
            switch (this.shapeType)
            {
                case ShapeType.Line:
                    graph.DrawLine(graphic.ToPdf(), line.From.ToPdf(), line.To.ToPdf());
                    break;

                case ShapeType.Polyline:
                    graph.DrawLines(graphic.ToPdf(), polyline.ToPdf().ToArray());
                    break;

                case ShapeType.Ellipse:
                    graph.DrawEllipse(graphic.ToPdf(), boundary.ToPdf());
                    break;

                case ShapeType.Bezier:
                    graph.DrawBeziers(graphic.ToPdf(), curve.ToBezierPolyline().ToPdf().ToArray());
                    break;

                case ShapeType.Brep:
                    Pd.XGraphicsPath crvPath = new Pd.XGraphicsPath();

                    List<Rg.Curve> curves = Rg.NurbsCurve.JoinCurves(brep.DuplicateNakedEdgeCurves(true, true)).ToList();
                    foreach (Rg.NurbsCurve nurbs in curves)
                    {
                        crvPath.StartFigure();
                        crvPath.AddBeziers(nurbs.ToBezierPolyline().ToPdf().ToArray());
                        if(nurbs.IsClosed)crvPath.CloseFigure();
                    }
                    graph.DrawPath(graphic.ToPdf(), graphic.Color.ToPdfBrush(), crvPath);
                    break;

                case ShapeType.Mesh:
                    Pd.XGraphicsPath polyPath = new Pd.XGraphicsPath();

                    List<Rg.Polyline> polylines = this.mesh.GetNakedEdges().ToList();
                    foreach (Rg.Polyline pline in polylines)
                    {
                        polyPath.StartFigure();
                        polyPath.AddLines(pline.ToPdf().ToArray());
                        if (pline.IsClosed) polyPath.CloseFigure();
                    }
                    graph.DrawPath(graphic.ToPdf(),graphic.Color.ToPdfBrush(), polyPath);
                    break;

                case ShapeType.ImageFrame:
                    Stream streamA = image.ToStream();
                    Pd.XImage xImageA = Pd.XImage.FromStream(streamA);
                    
                    graph.DrawImage(xImageA,this.boundary.ToPdf());
                    streamA.Dispose();
                    break;

                case ShapeType.ImageObj:
                    Stream streamB = image.ToStream();
                    Pd.XImage xImageB = Pd.XImage.FromStream(streamB);
                    graph.DrawImage(xImageB, location.ToPdf());
                    streamB.Dispose();
                    break;

                case ShapeType.TextObj:
                    graph.DrawString(this.content, font.ToPdf(), font.Color.ToPdfBrush(), location.ToPdf());
                    break;

                case ShapeType.TextBox:
                    Pd.XFont pdfFont = font.ToPdf();

                    Pl.XTextFormatter textFormatter = new Pl.XTextFormatter(graph);
                    textFormatter.Alignment = this.alignment.ToPdf();
                    textFormatter.LayoutRectangle = this.boundary.ToPdf();
                    textFormatter.Text = this.content;
                    textFormatter.Font = pdfFont;

                    textFormatter.DrawString(this.content, pdfFont, font.Color.ToPdfBrush(), this.boundary.ToPdf(), Pd.XStringFormats.TopLeft);
                    break;
            }

        }

        #endregion

    }
}
