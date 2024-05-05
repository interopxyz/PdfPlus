using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Sd = System.Drawing;

using Rg = Rhino.Geometry;

using Pc = PdfSharp.Charting;
using Pf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;
using Pl = PdfSharp.Drawing.Layout;

namespace PdfPlus
{
    public class Shape : ObjectAssembly
    {

        #region members

        public enum ShapeType { None, Line, Polyline, Bezier, Circle, Ellipse, Arc, Brep, Mesh, TextBox, ImageFrame, TextObj, ImageObj, ChartObj, LinkObj, PreviewText, PreviewBoundary };
        protected ShapeType shapeType = ShapeType.None;

        public enum LinkTypes { Hyperlink, Filepath, Page };
        protected LinkTypes linkType = LinkTypes.Hyperlink;

        protected double scale = 1.0;

        public bool Renderable = true;

        #endregion

        #region constructors

        protected Shape() : base()
        {
            this.elementType = ElementTypes.Shape;
        }

        public Shape(Shape shape) : base(shape)
        {
            this.shapeType = shape.shapeType;
            this.linkType = shape.linkType;

            this.scale = shape.scale;
            this.Renderable = shape.Renderable;
        }

        public Shape(Block block): base(block)
        {
            this.elementType = ElementTypes.Shape;
            switch (block.BlockType)
            {
                case Block.BlockTypes.Chart:
                    this.shapeType = ShapeType.ChartObj;
                    break;
                case Block.BlockTypes.Image:
                    this.shapeType = ShapeType.ImageFrame;
                    break;
                case Block.BlockTypes.Text:
                    this.shapeType = ShapeType.TextBox;
                    break;
            }
        }

        #region preview

        public static Shape CreatePreview(Rg.Rectangle3d boundary)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.PreviewBoundary;

            shape.polyline = boundary.ToNurbsCurve().Points.ControlPolygon();

            return shape;
        }

        public static Shape CreatePreview(string text, Rg.Point3d location, double angle)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.PreviewText;

            shape.location = new Rg.Point3d(location);
            shape.fragments.Add(new Fragment(text));
            shape.font = Fonts.Preview;
            shape.angle = angle;

            shape.Renderable = false;
            return shape;
        }

        #endregion

        #region text

        public static Shape CreateText(string content, Rg.Rectangle3d boundary, Alignment alignment, Font font)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.TextBox;

            shape.fragments.Add(new Fragment(content,font));
            shape.alignment = alignment;
            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));
            shape.font = new Font(font);

            return shape;
        }

        public static Shape CreateText(Fragment content, Rg.Rectangle3d boundary, Alignment alignment, Font font)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.TextBox;

            shape.fragments.Add(new Fragment(content));
            shape.alignment = alignment;
            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));
            shape.font = new Font(font);

            return shape;
        }

        public static Shape CreateText(string content, Rg.Point3d location, Font font)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.TextObj;

            shape.fragments.Add(new Fragment(content,font));
            shape.location = new Rg.Point3d(location);
            shape.font = new Font(font);

            return shape;
        }

        public static Shape CreateText(Fragment content, Rg.Point3d location, Font font)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.TextObj;

            shape.fragments.Add(new Fragment(content));
            shape.location = new Rg.Point3d(location);
            shape.font = new Font(font);

            return shape;
        }

        public static Shape CreateLink(string link, Rg.Rectangle3d boundary, LinkTypes type)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.LinkObj;
            shape.linkType = type;

            shape.text = link;
            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);

            return shape;
        }

        #endregion

        #region image

        public static Shape CreateImage(Sd.Bitmap bitmap, Rg.Rectangle3d boundary, string path = "")
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.ImageFrame;

            if(bitmap!=null)shape.imageObject = new Sd.Bitmap(bitmap);
            shape.imageName = path;
            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));

            return shape;
        }

        public static Shape CreateImage(Sd.Bitmap bitmap, Rg.Point3d location, double scale = 1.0, string path = "")
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.ImageObj;

            shape.imageObject = new Sd.Bitmap(bitmap);
            shape.imageName = path;
            shape.location = new Rg.Point3d(location);
            Rg.Plane plane = Rg.Plane.WorldXY;
            double factor = 72.0 / 96.0;
            plane.Origin = location - new Rg.Vector3d(0, bitmap.Height * factor * scale, 0);
            shape.boundary = new Rg.Rectangle3d(plane, location, new Rg.Point3d(location.X + bitmap.Width * factor * scale, location.Y + bitmap.Height * factor * scale, 0));

            return shape;
        }

        #endregion

        #region chart

        public static Shape CreateChart(List<DataSet> data, ChartTypes chartType, Rg.Rectangle3d boundary)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.ChartObj;

            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
            shape.chartType = chartType;
            shape.SetData(data);

            return shape;
        }

        public static Shape CreateChart(DataSet data, ChartTypes chartType, Rg.Rectangle3d boundary)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.ChartObj;

            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
            shape.chartType = chartType;
            shape.SetData(new List<DataSet> { data });

            return shape;
        }

        public static Shape CreateChart(List<DataSet> data, ChartTypes chartType, Rg.Rectangle3d boundary, string title)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.ChartObj;

            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
            shape.chartType = chartType;
            shape.SetData(data);

            return shape;
        }

        #endregion

        #region geometry

        public static Shape CreateGeometry(Rg.Polyline polyline, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Polyline;

            shape.boundingBox = polyline.BoundingBox;
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.polyline = polyline.Duplicate();
            shape.curve = polyline.ToNurbsCurve();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Rectangle3d rectangle, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Polyline;

            shape.boundingBox = rectangle.BoundingBox;
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.polyline = rectangle.ToPolyline(); 
            shape.curve = rectangle.ToNurbsCurve();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Line line, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Line;

            shape.boundingBox = line.BoundingBox;
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.line = new Rg.Line(line.From, line.To);
            shape.curve = line.ToNurbsCurve();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Arc arc, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.boundingBox = arc.BoundingBox();
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.curve = arc.ToNurbsCurve();
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Circle circle, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Circle;

            shape.boundingBox = circle.BoundingBox;
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.circle = new Rg.Circle(circle.Plane, circle.Radius);
            shape.curve = circle.ToNurbsCurve();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Ellipse ellipse, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = ellipse.ToNurbsCurve();
            shape.boundingBox = shape.curve.GetBoundingBox(true);
            shape.boundary = shape.boundingBox.ToRectangle3d();

            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.BezierCurve bezier, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = bezier.ToNurbsCurve();

            shape.boundingBox = shape.curve.GetBoundingBox(true);
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Curve curve, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = new Rg.NurbsCurve(curve.ToNurbsCurve());

            shape.boundingBox = shape.curve.GetBoundingBox(true);
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.NurbsCurve curve, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = new Rg.NurbsCurve(curve);

            shape.boundingBox = shape.curve.GetBoundingBox(true);
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Brep brep, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Brep;

            shape.brep = brep.DuplicateBrep();

            shape.boundingBox = shape.brep.GetBoundingBox(true);
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Mesh mesh, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Mesh;

            shape.mesh = mesh.DuplicateMesh();

            shape.boundingBox = shape.mesh.GetBoundingBox(true);
            shape.boundary = shape.boundingBox.ToRectangle3d();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        #endregion

        #endregion

        #region properties

        public virtual ShapeType Type
        {
            get { return this.shapeType; }
        }

        public virtual double Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        public virtual bool IsPreview
        {
            get
            {
                switch (this.shapeType)
                {
                    case ShapeType.PreviewText:
                    case ShapeType.PreviewBoundary:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool Is3d
        {
            get
            {
                switch (this.shapeType)
                {
                    case ShapeType.Mesh:
                    case ShapeType.Brep:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool Is2d
        {
            get
            {
                switch (this.shapeType)
                {
                    case ShapeType.Arc:
                    case ShapeType.Bezier:
                    case ShapeType.Circle:
                    case ShapeType.Ellipse:
                    case ShapeType.Line:
                    case ShapeType.Polyline:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual bool IsText
        {
            get
            {
                switch (this.shapeType)
                {
                    case ShapeType.TextBox:
                    case ShapeType.TextObj:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual Rg.GeometryBase Geometry
        {
            get
            {
                switch (this.shapeType)
                {
                    case ShapeType.Arc:
                    case ShapeType.Bezier:
                    case ShapeType.Circle:
                    case ShapeType.Ellipse:
                    case ShapeType.Line:
                    case ShapeType.Polyline:
                        return this.Bezier;
                    case ShapeType.Mesh:
                        return this.Mesh;
                    case ShapeType.Brep:
                        return this.Brep;
                }
                return null;
            }
            }

        #endregion

        #region methods

        public List<Rg.Curve> RenderPreviewChart(out List<Sd.Color> colors)
        {
            List<Rg.Curve> output = new List<Rg.Curve>();
            List<Sd.Color> clrs = new List<Sd.Color>();

            if (this.Type == ShapeType.ChartObj)
            {
                Rg.Plane plane = Rg.Plane.WorldXY;
                double width = this.boundary.Width * 0.8;
                double height = this.boundary.Height * 0.8;
                double radius = Math.Min(width, height) /2.0;
                Rg.Point3d center = this.boundary.Center;
                Rg.Point3d corner = new Rg.Point3d(center.X - radius, center.Y - radius, center.Z);
                int count = data.Count;
                Rg.Interval bounds = data.Bounds();
                int s = 0;
                double step = radius * 2.0 / count;
                double space = (step / count);
                int total = 0;
                double w = 0;
                foreach (DataSet ds in data) total = Math.Max(total, ds.Values.Count);

                switch (this.chartType)
                {
                    case ChartTypes.Pie:
                        plane.Origin = center;
                        plane.Rotate(Math.PI / 2, Rg.Vector3d.ZAxis);
                        Rg.Circle circle = new Rg.Circle(plane, radius);
                        Rg.NurbsCurve circ = circle.ToNurbsCurve();
                        circ.Reverse();
                        circ.Domain = new Rg.Interval(0, 1);
                        output.Add(circ);
                        clrs.Add(Sd.Color.Black);

                        List<double> vals = this.data[0].Values.ReMapStack();

                        foreach (double t in vals)
                        {
                            output.Add(new Rg.Line(center, circ.PointAt(t)).ToNurbsCurve());
                            if (data[0].HasColors)
                            {
                            clrs.Add(data[0].Colors[s]);
                            }
                            else
                            {
                                if (data[0].Graphic.Color.A == 0)
                                {
                                    clrs.Add(Sd.Color.Black);
                                }
                                else
                                {
                                    clrs.Add(data[0].Graphic.Color);
                                }
                            }
                            s++;
                        }

                        break;
                    case ChartTypes.Area:
                        foreach (DataSet ds in data)
                        {
                            List<double> areaVals = ds.Values.ReMap(bounds);
                            Rg.Polyline plot = new Rg.Polyline();
                            int lineCount = areaVals.Count - 1;
                            double i = 0;
                            plot.Add(corner);
                            foreach (double t in areaVals)
                            {
                                plot.Add(corner + new Rg.Vector3d((double)(i / lineCount) * radius * 2, t * radius * 2, 0));
                                i++;
                            }
                            plot.Add(corner + new Rg.Vector3d(radius * 2, 0, 0));
                            plot.Add(corner);

                            output.Add(plot.ToNurbsCurve());
                            clrs.Add(ds.Graphic.Stroke);
                        }

                        break;
                    case ChartTypes.Line:
                        foreach (DataSet ds in data)
                        {
                            List<double> lineVals = ds.Values.ReMap(bounds);
                            Rg.Polyline plot = new Rg.Polyline();
                            int lineCount = lineVals.Count - 1;
                            double i = 0;
                            foreach (double t in lineVals)
                            {
                                plot.Add(corner + new Rg.Vector3d((double)(i / lineCount) * radius * 2, t * radius * 2, 0));
                                i++;
                            }

                            output.Add(plot.ToNurbsCurve());
                            clrs.Add(ds.Graphic.Stroke);
                        }

                        break;
                    case ChartTypes.Column:
                        step = radius * 2 / total;
                        space = (step / count);
                        foreach (DataSet ds in data)
                        {
                            List<double> lineVals = ds.Values.ReMap(bounds);
                            int colCount = lineVals.Count;
                            double i = 0;
                            foreach (double t in lineVals)
                            {
                                double x = s * space + i * step;
                                Rg.Point3d ptX = corner + new Rg.Vector3d(x, 0, 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(x, t * (radius * 2 - 1) + 1, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(space * 0.9, 0, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                if (ds.HasColors)
                                {
                                    clrs.Add(ds.Colors[s]);
                                }
                                else
                                {
                                    if (ds.Graphic.Color.A == 0)
                                    {
                                        clrs.Add(Sd.Color.Black);
                                    }
                                    else
                                    {
                                        clrs.Add(ds.Graphic.Color);
                                    }
                                }
                                i++;
                            }
                            s++;
                        }

                        break;
                    case ChartTypes.Bar:
                        step = radius * 2 / total;
                        space = (step / count);
                        foreach (DataSet ds in data)
                        {
                            List<double> lineVals = ds.Values.ReMap(bounds);
                            double i = 0;
                            foreach (double t in lineVals)
                            {
                                double y = s * space + i * step;
                                Rg.Point3d ptX = corner + new Rg.Vector3d(0, y, 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(t * (radius * 2 - 1) + 1, y, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(0, space * 0.9, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                if (ds.HasColors)
                                {
                                    clrs.Add(ds.Colors[s]);
                                }
                                else
                                {
                                    if (ds.Graphic.Color.A == 0)
                                    {
                                        clrs.Add(Sd.Color.Black);
                                    }
                                    else
                                    {
                                        clrs.Add(ds.Graphic.Color);
                                    }
                                }
                                i++;
                            }
                            s++;
                        }
                        break;
                    case ChartTypes.BarStacked:
                        List<List<double>> barValues = data.ReMapSet();

                        total = barValues.Count;
                        w = 1.0 / total * radius * 2.0;
                        foreach (List<double> barVals in barValues)
                        {
                            for (int i = 0; i < barVals.Count - 1; i++)
                            {
                                double y = (double)s / total * radius * 2.0;
                                Rg.Point3d ptX = corner + new Rg.Vector3d(barVals[i] * (radius * 2 - 1) + Convert.ToInt32(i > 0), y, 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(barVals[i + 1] * (radius * 2 - 1) + 1, y, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(0, w * 0.9, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                if (this.data[i].HasColors)
                                {
                                    clrs.Add(this.data[i].Colors[(int)s]);
                                }
                                else
                                {
                                    if (this.data[i].Graphic.Color.A == 0)
                                    {
                                        clrs.Add(Sd.Color.Black);
                                    }
                                    else
                                    {
                                        clrs.Add(data[i].Graphic.Color);
                                    }
                                }
                            }
                            s++;
                        }
                        break;
                    case ChartTypes.ColumnStacked:
                        List<List<double>> colValues = data.ReMapSet();

                        total = colValues.Count;
                        w = 1.0 / total * radius * 2.0;
                        foreach (List<double> barVals in colValues)
                        {
                            for (int i = 0; i < barVals.Count - 1; i++)
                            {
                                double x = (double)s / total * radius * 2.0;
                                Rg.Point3d ptX = corner + new Rg.Vector3d(x, barVals[i] * (radius * 2 - 1) + Convert.ToInt32(i > 0), 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(x, barVals[i + 1] * (radius * 2 - 1) + 1, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(w * 0.9, 0, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                if (this.data[i].HasColors) { 
                                clrs.Add(this.data[i].Colors[(int)s]);
                                }
                                else
                                {
                                    if (this.data[i].Graphic.Color.A == 0)
                                    {
                                        clrs.Add(Sd.Color.Black);
                                    }
                                    else
                                    {
                                        clrs.Add(data[i].Graphic.Color);
                                    }
                                }
                            }
                            s++;
                        }
                        break;
                }
            }
            colors = clrs;
            return output;
        }

        public void AlignContent(Page page, bool translate = true)
        {

            Rg.Plane plane = Rg.Plane.WorldZX;
            plane.OriginY = page.BaseObject.Height.Point / 2.0;


            Rg.Plane frame = Rg.Plane.WorldXY;
            frame.OriginY = page.BaseObject.Height.Point;
            Rg.Vector3d yaxis = frame.YAxis;
            yaxis.Reverse();
            frame.YAxis = yaxis;
            if (!translate) frame = new Rg.Plane(page.Frame);

            switch (this.shapeType)
            {
                case ShapeType.ImageObj:
                case ShapeType.TextObj:
                    this.location.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;
                case ShapeType.ChartObj:
                case ShapeType.TextBox:
                case ShapeType.ImageFrame:
                case ShapeType.Ellipse:
                    this.boundary.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    break;
                case ShapeType.Circle:
                    this.boundary.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
                    this.circle.Transform(Rg.Transform.PlaneToPlane(page.Frame, frame));
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

        #region rendering

        public Shape Transform(Rg.Transform transform)
        {
            Shape shape = new Shape(this);

            shape.boundary.Transform(transform);
            shape.boundingBox.Transform(transform);
            shape.location.Transform(transform);

            switch (this.shapeType)
            {
                case ShapeType.Line:
                    shape.line.Transform(transform);
                    shape.curve.Transform(transform);
                    break;
                case ShapeType.Polyline:
                    shape.polyline.Transform(transform);
                    shape.curve.Transform(transform);
                    break;
                case ShapeType.Circle:
                    shape.circle.Transform(transform);
                    shape.curve.Transform(transform);
                    break;
                case ShapeType.Bezier:
                    shape.curve.Transform(transform);
                    break;
                case ShapeType.Brep:
                    shape.brep.Transform(transform);
                    break;
                case ShapeType.Mesh:
                    shape.mesh.Transform(transform);
                    break;
            }
            return shape;
        }

        public Pd.XGraphics RenderGeometry(Pd.XGraphics graph)
        {
            switch (this.shapeType)
            {
                case ShapeType.Line:
                    graph = this.RenderLine(graph);
                    break;
                case ShapeType.Polyline:
                    graph = this.RenderPolyline(graph);
                    break;
                case ShapeType.Circle:
                    graph = this.RenderEllipse(graph);
                    break;
                case ShapeType.Ellipse:
                    graph = this.RenderEllipse(graph);
                    break;
                case ShapeType.Bezier:
                    graph = this.RenderBezier(graph);
                    break;
                case ShapeType.Brep:
                    graph = this.RenderBrep(graph);
                    break;
                case ShapeType.Mesh:
                    graph = this.RenderMesh(graph);
                    break;
                case ShapeType.TextObj:
                    graph = this.RenderTextPoint(graph);
                    break;
                case ShapeType.TextBox:
                    graph = this.RenderTextBoundary(graph);
                    break;
                case ShapeType.ChartObj:
                    graph = this.RenderChartObject(graph);
                    break;
            }
            return graph;
        }

        protected Pd.XGraphics RenderLine(Pd.XGraphics graph)
        {
            graph.DrawLine(graphic.ToPdf(), line.From.ToPdf(), line.To.ToPdf());
            return graph;
        }

        protected Pd.XGraphics RenderPolyline(Pd.XGraphics graph)
        {
            Pd.XGraphicsPath plinePath = new Pd.XGraphicsPath();

            plinePath.StartFigure();
            plinePath.AddLines(polyline.ToPdf().ToArray());
            if (polyline.IsClosed) plinePath.CloseFigure();
            graph.DrawPath(graphic.ToPdf(), graphic.Color.ToPdfBrush(), plinePath);
            return graph;
        }

        protected Pd.XGraphics RenderEllipse(Pd.XGraphics graph)
        {
            graph.DrawEllipse(graphic.ToPdf(), graphic.Color.ToPdfBrush(), boundary.ToPdf());
            return graph;
        }

        protected Pd.XGraphics RenderBezier(Pd.XGraphics graph)
        {
            graph.DrawBeziers(graphic.ToPdf(), curve.ToBezierPolyline().ToPdf().ToArray());
            return graph;
        }

        protected Pd.XGraphics RenderBrep(Pd.XGraphics graph)
        {
            Pd.XGraphicsPath crvPath = new Pd.XGraphicsPath();

            List<Rg.Curve> curves = Rg.NurbsCurve.JoinCurves(brep.DuplicateNakedEdgeCurves(true, true)).ToList();
            foreach (Rg.Curve curve in curves)
            {
                Rg.NurbsCurve nurbs = curve.ToNurbsCurve();
                crvPath.StartFigure();
                crvPath.AddBeziers(nurbs.ToBezierPolyline().ToPdf().ToArray());
                if (nurbs.IsClosed) crvPath.CloseFigure();
            }
            graph.DrawPath(graphic.ToPdf(), graphic.Color.ToPdfBrush(), crvPath);
            return graph;
        }

        protected Pd.XGraphics RenderMesh(Pd.XGraphics graph)
        {
            Pd.XGraphicsPath polyPath = new Pd.XGraphicsPath();

            List<Rg.Polyline> polylines = this.mesh.GetNakedEdges().ToList();
            foreach (Rg.Polyline pline in polylines)
            {
                polyPath.StartFigure();
                polyPath.AddLines(pline.ToPdf().ToArray());
                if (pline.IsClosed) polyPath.CloseFigure();
            }
            graph.DrawPath(graphic.ToPdf(), graphic.Color.ToPdfBrush(), polyPath);
            return graph;
        }

        protected Pd.XGraphics RenderTextPoint(Pd.XGraphics graph)
        {
            Pd.XStringFormat format = new Pd.XStringFormat();
            format.Alignment = font.Justification.ToPdfLine();
            format.LineAlignment = Pd.XLineAlignment.BaseLine;
            graph.RotateAtTransform(-this.Angle, location.ToPdf());
            graph.DrawString(this.Text, font.ToPdf(this.scale), font.Color.ToPdfBrush(), location.ToPdf(), format);
            graph.RotateAtTransform(this.Angle, location.ToPdf());
            return graph;
        }

        protected Pd.XGraphics RenderTextBoundary(Pd.XGraphics graph)
        {
            Pd.XFont pdfFont = font.ToPdf(this.scale);

            Pl.XTextFormatter textFormatter = new Pl.XTextFormatter(graph);
            textFormatter.Alignment = this.Font.Justification.ToPdf();

            Rg.Point2d center = new Rg.Point2d(this.boundary.Center.X, this.boundary.Center.Y);
            Rg.Point2d bottomLeft = center - 0.5 * new Rg.Vector2d(this.boundary.Width, this.boundary.Height);
            Pd.XRect layoutRect = new Pd.XRect(new Pd.XPoint(bottomLeft.X, bottomLeft.Y), new Pd.XSize(this.boundary.Width, this.boundary.Height));

            textFormatter.LayoutRectangle = layoutRect;

            double angleRad = Rg.Vector3d.VectorAngle(Rg.Vector3d.XAxis, this.boundary.Plane.XAxis, Rg.Plane.WorldXY);
            double angleDeg = Rhino.RhinoMath.ToDegrees(angleRad);
            Pd.XGraphicsState state = graph.Save();
            graph.RotateAtTransform(angleDeg, new Pd.XPoint(center.X, center.Y));
            textFormatter.DrawString(this.Text, pdfFont, font.Color.ToPdfBrush(), layoutRect, this.ToPdfAlignment());
            graph.Restore(state);

            return graph;
        }

        protected Pd.XGraphics RenderChartObject(Pd.XGraphics graph)
        {
            Pc.Chart chart = CombinationChart();

            if (this.alignment != Alignment.None)
            {
                chart.Legend.Docking = this.alignment.ToPdf();

                chart.Legend.Font.Color = this.font.Color.ToPdf();
                chart.Legend.Font.Name = this.font.Family;
                chart.Legend.Font.Size = this.font.Size;
                chart.Legend.Font.Bold = this.font.IsBold;
                chart.Legend.Font.Italic = this.font.IsItalic;
                if (this.font.IsUnderlined) chart.Legend.Font.Underline = Pc.Underline.Single;
            }

            chart.Font.Color = this.FontColor.ToPdf();
            chart.Font.Name = this.FontFamily;
            chart.Font.Size = this.FontSize * this.scale;
            chart.Font.Bold = this.font.IsBold;
            chart.Font.Italic = this.font.IsItalic;
            if (this.font.IsUnderlined) chart.Font.Underline = Pc.Underline.Single;

            //X Axis
            chart.XAxis.MajorTickMark = Pc.TickMarkType.None;
            chart.XAxis.TickLabels.Font.Color = this.FontColor.ToPdf();
            chart.XAxis.TickLabels.Font.Name = this.FontFamily;
            chart.XAxis.TickLabels.Font.Size = this.FontSize * this.scale;
            chart.XAxis.TickLabels.Font.Bold = this.font.IsBold;
            chart.XAxis.TickLabels.Font.Italic = this.font.IsItalic;
            if (this.font.IsUnderlined) chart.XAxis.TickLabels.Font.Underline = Pc.Underline.Single;
            if (this.HasXAxis)
            {
                chart.XAxis.Title.Alignment = Pc.HorizontalAlignment.Center;

                chart.XAxis.Title.Caption = this.xAxis;
                chart.XAxis.Title.Font.Color = this.FontColor.ToPdf();
                chart.XAxis.Title.Font.Name = this.FontFamily;
                chart.XAxis.Title.Font.Size = this.FontSize * this.scale;
                chart.XAxis.Title.Font.Bold = this.font.IsBold;
                chart.XAxis.Title.Font.Italic = this.font.IsItalic;
                if (this.font.IsUnderlined) chart.XAxis.Title.Font.Underline = Pc.Underline.Single;
                if (!this.IsChartHorizontal) chart.XAxis.Title.Orientation = 90;

                if (this.graphic.HasStroke)
                {
                    chart.XAxis.LineFormat.Color = this.graphic.Stroke.ToPdf();
                    chart.XAxis.LineFormat.Width = this.graphic.Weight;
                }
            }

            chart.XAxis.HasMajorGridlines = (this.horizontalBorderStyle != BorderStyles.None);
            if ((int)this.chartType < 2) chart.XAxis.HasMajorGridlines = (this.verticalBorderStyle != BorderStyles.None);
            if (chart.XAxis.HasMajorGridlines)
            {
                if (this.graphic.HasStroke)
                {
                    chart.XAxis.MajorGridlines.LineFormat.Color = this.graphic.Stroke.ToPdf();
                    chart.XAxis.MajorGridlines.LineFormat.Width = this.graphic.Weight;
                }
            }

            //Y Axis
            chart.YAxis.MajorTickMark = Pc.TickMarkType.None;
            chart.YAxis.TickLabels.Format = "#.####";
            chart.YAxis.TickLabels.Font.Color = this.FontColor.ToPdf();
            chart.YAxis.TickLabels.Font.Name = this.FontFamily;
            chart.YAxis.TickLabels.Font.Size = this.FontSize * this.scale;
            chart.YAxis.TickLabels.Font.Bold = this.font.IsBold;
            chart.YAxis.TickLabels.Font.Italic = this.font.IsItalic;
            if (this.font.IsUnderlined) chart.YAxis.TickLabels.Font.Underline = Pc.Underline.Single;
            if (this.HasYAxis)
            {
                chart.YAxis.Title.Alignment = Pc.HorizontalAlignment.Center;
                chart.YAxis.Title.VerticalAlignment = Pc.VerticalAlignment.Center;

                chart.YAxis.Title.Caption = this.yAxis;
                chart.YAxis.Title.Font.Color = this.FontColor.ToPdf();
                chart.YAxis.Title.Font.Name = this.FontFamily;
                chart.YAxis.Title.Font.Size = this.FontSize * this.scale;
                chart.YAxis.Title.Font.Bold = this.font.IsBold;
                chart.YAxis.Title.Font.Italic = this.font.IsItalic;
                if (this.font.IsUnderlined) chart.YAxis.Title.Font.Underline = Pc.Underline.Single;
                if (this.IsChartHorizontal) chart.YAxis.Title.Orientation = 90;

                if (this.graphic.HasStroke)
                {
                    chart.YAxis.LineFormat.Color = this.graphic.Stroke.ToPdf();
                    chart.YAxis.LineFormat.Width = this.graphic.Weight;
                }
            }

            chart.YAxis.HasMajorGridlines = (this.verticalBorderStyle != BorderStyles.None);
            if ((int)this.chartType < 2) chart.YAxis.HasMajorGridlines = (this.horizontalBorderStyle != BorderStyles.None);
            if (chart.YAxis.HasMajorGridlines)
            {
                if (this.graphic.HasStroke)
                {
                    chart.YAxis.MajorGridlines.LineFormat.Color = this.graphic.Stroke.ToPdf();
                    chart.YAxis.MajorGridlines.LineFormat.Width = this.graphic.Weight;
                }
            }

            Pc.ChartFrame frame = new Pc.ChartFrame();
            frame.Location = new Pd.XPoint(boundary.Corner(0).X, boundary.Corner(3).Y);
            frame.Size = new Pd.XSize(boundary.Width, boundary.Height);

            frame.Background = graphic.Color.ToPdfBrush();

            frame.Add(chart);
            frame.Draw(graph);

            return graph;
        }

        #endregion

        public void Render(Pd.XGraphics graph, Pf.PdfPage page, Rg.Plane coordinateframe)
        {
            switch (this.shapeType)
            {
                case ShapeType.Line:
                    graph = this.RenderLine(graph);
                    break;
                case ShapeType.Polyline:
                    graph = this.RenderPolyline(graph);
                    break;
                case ShapeType.Circle:
                    graph = this.RenderEllipse(graph);
                    break;
                case ShapeType.Ellipse:
                    graph = this.RenderEllipse(graph);
                    break;
                case ShapeType.Bezier:
                    graph = this.RenderBezier(graph);
                    break;
                case ShapeType.Brep:
                    graph = this.RenderBrep(graph);
                    break;
                case ShapeType.Mesh:
                    graph = this.RenderMesh(graph);
                    break;
                case ShapeType.TextObj:
                    graph = this.RenderTextPoint(graph);
                    break;
                case ShapeType.TextBox:
                    graph = this.RenderTextBoundary(graph);
                    break;
                case ShapeType.LinkObj:
                    switch (this.linkType)
                    {
                        case LinkTypes.Hyperlink:
                            page.AddWebLink(boundary.ToPdfRect(coordinateframe), this.text);
                            break;
                        case LinkTypes.Filepath:
                            page.AddFileLink(boundary.ToPdfRect(coordinateframe), this.text);
                            break;
                        case LinkTypes.Page:
                            int index = 0;
                            bool isInt = int.TryParse(this.text, out index);
                            if (isInt) page.AddDocumentLink(boundary.ToPdfRect(coordinateframe), index + 1);

                            break;
                    }
                    break;
                case ShapeType.ImageFrame:
                case ShapeType.ImageObj:
                    Stream stream = imageObject.ToStream();
                    Pd.XImage xImageA = Pd.XImage.FromStream(stream);

                    graph.RotateAtTransform(-this.Angle, this.boundary.Corner(0).ToPdf());
                    graph.DrawImage(xImageA, this.boundary.ToPdf());
                    graph.RotateAtTransform(this.Angle, this.boundary.Corner(0).ToPdf());
                    stream.Dispose();
                    break;
                case ShapeType.ChartObj:
                    graph = this.RenderChartObject(graph);
                    break;
            }

        }

        private Pc.Chart CombinationChart()
        {
            Pc.ChartType cType = this.chartType.ToPdf();
            Pc.Chart chart = new Pc.Chart(cType);

            switch (this.chartType)
            {
                default:
                    foreach (DataSet d in this.data)
                    {
                        if (d.IsNumeric)
                        {
                            Pc.Series series = chart.SeriesCollection.AddSeries();
                            series.ChartType = cType;
                            series.Name = d.Title;

                            if (d.Graphic.HasStroke)
                            {
                                series.LineFormat.Visible = true;
                                series.LineFormat.Color = d.Graphic.Stroke.ToPdf();
                                series.LineFormat.Width = d.Graphic.Weight;
                            }

                            if (d.Graphic.HasColor)
                            {
                                series.FillFormat.Visible = true;
                                series.FillFormat.Color = d.Graphic.Color.ToPdf();
                            }

                            series.MarkerStyle = Pc.MarkerStyle.Circle;
                            series.MarkerSize = d.Graphic.Weight * 2;

                            series.Add(d.Values.ToArray());

                            if (d.LabelData)
                            {
                                series.HasDataLabel = true;

                                series.DataLabel.Position = d.LabelAlignment.ToPdf();

                                series.DataLabel.Font.Color = d.Font.Color.ToPdf();
                                series.DataLabel.Font.Name = d.Font.Family;
                                series.DataLabel.Font.Size = d.Font.Size;
                                series.DataLabel.Font.Bold = d.Font.IsBold;
                                series.DataLabel.Font.Italic = d.Font.IsItalic;
                                if (d.Font.IsUnderlined) series.DataLabel.Font.Underline = Pc.Underline.Single;
                            }
                            else
                            {
                                series.HasDataLabel = false;
                            }

                            if ((int)this.chartType < 4)
                            {
                                if (d.HasColors)
                                {
                                    for (int p = 0; p < series.Elements.Count; p++)
                                    {
                                        series.Elements[p].FillFormat.Color = d.Colors[p].ToPdf();
                                    }
                                }

                            }

                        }
                    }
                    break;
                case ChartTypes.Pie:
                    Pc.Series pieseries = chart.SeriesCollection.AddSeries();
                    pieseries.ChartType = cType;

                    DataSet dt = data[0];
                    pieseries.Add(dt.Values.ToArray());
                    pieseries.HasDataLabel = dt.LabelData;
                    pieseries.LineFormat.Visible = true;
                    pieseries.LineFormat.Color = dt.Graphic.Stroke.ToPdf();
                    pieseries.LineFormat.Width = dt.Graphic.Weight;

                    pieseries.FillFormat.Visible = true;
                    pieseries.FillFormat.Color = dt.Graphic.Color.ToPdf();

                    pieseries.MarkerStyle = Pc.MarkerStyle.Circle;
                    pieseries.MarkerSize = dt.Graphic.Weight * 2;

                    pieseries.HasDataLabel = dt.LabelData;
                    if (pieseries.HasDataLabel) { 
                    pieseries.DataLabel.Font.Color = dt.Font.Color.ToPdf();
                    pieseries.DataLabel.Font.Name = dt.Font.Family;
                    pieseries.DataLabel.Font.Size = dt.Font.Size;
                    pieseries.DataLabel.Font.Bold = dt.Font.IsBold;
                    pieseries.DataLabel.Font.Italic = dt.Font.IsItalic;
                        pieseries.DataLabel.Position = dt.LabelAlignment.ToPdf();
                        if (dt.Font.IsUnderlined) pieseries.DataLabel.Font.Underline = Pc.Underline.Single;
                    }

                    if (dt.HasColors)
                    {
                        if (dt.Colors.Count > 0)
                        {
                            for (int p = 0; p < pieseries.Elements.Count; p++)
                            {
                                pieseries.Elements[p].FillFormat.Color = dt.Colors[p % dt.Colors.Count].ToPdf();
                            }
                        }
                    }
                    else
                    {
                        if (dt.Graphic.HasColor) { 
                        for (int p = 0; p < pieseries.Elements.Count; p++)
                        {
                            pieseries.Elements[p].FillFormat.Color = dt.Graphic.Color.ToPdf();
                        }
                        }
                    }
                    break;
            }


            //chart.Legend.LineFormat.Visible = true;

            return chart;
        }

        #endregion

    }
}
