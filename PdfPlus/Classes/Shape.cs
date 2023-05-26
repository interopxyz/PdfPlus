using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;
using Pc = PdfSharp.Charting;
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
        public enum ShapeType { None, Line, Polyline, Bezier, Circle, Ellipse, Arc, Brep, Mesh, TextBox, ImageFrame, TextObj, ImageObj, ChartObj, LinkObj };
        protected ShapeType shapeType = ShapeType.None;

        public enum LinkTypes { Hyperlink,Filepath,Page};
        protected LinkTypes linkType = LinkTypes.Hyperlink;

        public enum ChartTypes { Bar, BarStacked, Column, ColumnStacked, Line, Area, Pie };
        protected ChartTypes chartType = ChartTypes.ColumnStacked;
        protected List<DataSet> data = new List<DataSet>();

        string xAxis = string.Empty;
        string yAxis = string.Empty;
        Justification justification = Justification.None;

        protected Graphic graphic = new Graphic();
        protected Font font = new Font();

        protected Sd.Bitmap image = new Sd.Bitmap(10, 10);
        protected string imagePath = "";

        protected string content = string.Empty;
        protected Alignment alignment = Alignment.Left;

        protected Rg.Point3d location = new Rg.Point3d();
        protected Rg.Polyline polyline = new Rg.Polyline();
        protected Rg.Line line = new Rg.Line();
        protected Rg.Arc arc = new Rg.Arc();
        protected Rg.Circle circle = new Rg.Circle();
        protected Rg.Rectangle3d boundary = new Rg.Rectangle3d();
        protected Rg.NurbsCurve curve = new Rg.NurbsCurve(3, 2);

        protected Rg.Brep brep = new Rg.Brep();
        protected Rg.Mesh mesh = new Rg.Mesh();

        #endregion

        #region constructors

        public Shape(Shape shape)
        {
            this.graphic = new Graphic(shape.graphic);
            this.font = new Font(shape.font);

            this.shapeType = shape.shapeType;
            this.linkType = shape.linkType;

            this.chartType = shape.chartType;
            SetData(shape.data);
            this.xAxis = shape.xAxis;
            this.yAxis = shape.yAxis;
            this.justification = shape.justification;

            this.content = shape.content;
            this.alignment = shape.alignment;

            this.image = new Sd.Bitmap(shape.image);
            this.imagePath = shape.imagePath;

            this.location = new Rg.Point3d(shape.location);
            this.polyline = shape.polyline.Duplicate();
            this.boundary = new Rg.Rectangle3d(shape.boundary.Plane, shape.boundary.Corner(0), shape.boundary.Corner(2));
            this.line = new Rg.Line(shape.line.From, shape.line.To);
            this.curve = new Rg.NurbsCurve(shape.curve);
            this.circle = new Rg.Circle(shape.circle.Plane, shape.circle.Radius);

            this.brep = shape.brep.DuplicateBrep();
            this.mesh = shape.mesh.DuplicateMesh();
        }

        #region text
        public Shape(string content, Rg.Rectangle3d boundary, Alignment alignment, Font font)
        {
            shapeType = ShapeType.TextBox;

            this.content = content;
            this.alignment = alignment;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));
            this.font = new Font(font);
        }

        public Shape(string content, Rg.Point3d location, Font font)
        {
            shapeType = ShapeType.TextObj;

            this.content = content;
            this.location = new Rg.Point3d(location);
            this.font = new Font(font);
        }

        public Shape(string link, Rg.Rectangle3d boundary, LinkTypes type)
        {
            this.shapeType = ShapeType.LinkObj;
            this.linkType = type;

            this.content = link;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
        }

        #endregion

        #region image

        public Shape(Sd.Bitmap bitmap, Rg.Rectangle3d boundary, string path = "")
        {
            shapeType = ShapeType.ImageFrame;

            this.image = new Sd.Bitmap(bitmap);
            this.imagePath = path;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));
        }

        public Shape(Sd.Bitmap bitmap, Rg.Point3d location, double scale = 1.0, string path = "")
        {
            shapeType = ShapeType.ImageObj;

            this.image = new Sd.Bitmap(bitmap);
            this.imagePath = path;
            this.location = new Rg.Point3d(location);
            Rg.Plane plane = Rg.Plane.WorldXY;
            double factor = 72.0/96.0;
            plane.Origin = location - new Rg.Vector3d(0, bitmap.Height * factor * scale, 0);
            this.boundary = new Rg.Rectangle3d(plane, location, new Rg.Point3d(location.X+bitmap.Width * factor * scale, location.Y+bitmap.Height * factor * scale, 0));
        }

        #endregion

        #region chart

        public Shape(List<DataSet> data, ChartTypes chartType, Rg.Rectangle3d boundary)
        {
            shapeType = ShapeType.ChartObj;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
            this.chartType = chartType;
            SetData(data);
        }
        public Shape(DataSet data, ChartTypes chartType, Rg.Rectangle3d boundary)
        {
            shapeType = ShapeType.ChartObj;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
            this.chartType = chartType;
            SetData(new List<DataSet> { data });
        }

        public Shape(List<DataSet> data, ChartTypes chartType, Rg.Rectangle3d boundary, string title)
        {
            shapeType = ShapeType.ChartObj;
            this.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.X, boundary.Y);
            this.chartType = chartType;
            SetData(data);
            this.content = title; ;
            ;
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
            shapeType = ShapeType.Circle;
            this.circle = new Rg.Circle(circle.Plane, circle.Radius);
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

        public virtual ChartTypes ChartType
        {
            get { return chartType; }
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

        public virtual bool IsBold
        {
            get { return this.font.IsBold; }
        }

        public virtual bool IsItalic
        {
            get { return this.font.IsItalic; }
        }

        public virtual bool IsUnderlined
        {
            get { return this.font.IsUnderlined; }
        }

        public virtual bool IsStrikeout
        {
            get { return this.font.IsStrikeout; }
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

        public virtual Graphic Graphic
        {
            get { return new Graphic(graphic); }
            set { this.graphic = new Graphic(value); }
        }

        public virtual Font Font
        {
            get { return new Font(font); }
            set { this.font = new Font(value); }
        }

        public virtual string XAxis
        {
            get { return this.xAxis; }
            set { this.xAxis = value; }
        }

        public virtual string YAxis
        {
            get { return this.yAxis; }
            set { this.yAxis = value; }
        }

        public virtual bool HasXAxis
        {
            get { return (this.xAxis != string.Empty); }
        }

        public virtual bool HasYAxis
        {
            get { return (this.yAxis != string.Empty); }
        }

        public virtual Justification Justification
        {
            get { return this.justification; }
            set { this.justification = value; }
        }

        public virtual string TextContent
        {
            get { return this.content; }
        }

        public virtual Rg.Point3d Location
        {
            get { return this.location; }
        }

        public virtual Rg.Rectangle3d Boundary
        {
            get { return this.boundary; }
        }

        public virtual Rg.Line Line
        {
            get { return this.line; }
        }

        public virtual Rg.Arc Arc
        {
            get { return this.arc; }
        }

        public virtual Rg.Ellipse Ellipse
        {
            get { return new Rg.Ellipse(this.boundary.Plane,this.boundary.Width,this.boundary.Height); }
        }

        public virtual Rg.Polyline Polyline
        {
            get { return this.polyline; }
        }
        public virtual Rg.Circle Circle
        {
            get { return this.circle; }
        }
        public virtual Rg.Brep Brep
        {
            get { return this.brep; }
        }

        public virtual Rg.Mesh Mesh
        {
            get { return this.mesh; }
        }

        public virtual Rg.NurbsCurve Bezier
        {
            get {
                Rg.NurbsCurve nurbs = this.curve.ToNurbsCurve();
                nurbs.MakePiecewiseBezier(true);
                return nurbs; 
            }
        }

        public virtual Sd.Bitmap Image
        {
            get { return new Sd.Bitmap(this.image); }
        }

        public virtual string ImagePath
        {
            get { return imagePath; }
        }

        #endregion

        #region methods

        public List<Rg.Curve> RenderChart(out List<Sd.Color> colors)
        {
            List<Rg.Curve> output = new List<Rg.Curve>();
            List<Sd.Color> clrs = new List<Sd.Color>();

            if (this.Type == ShapeType.ChartObj)
            {
                Rg.Plane plane = Rg.Plane.WorldXY;
                double radius = Math.Min(this.boundary.Width, this.boundary.Height) * 0.4;
                Rg.Point3d center = this.boundary.Center;
                Rg.Point3d corner = new Rg.Point3d(center.X-radius,center.Y-radius,center.Z);
                int count = data.Count;
                Rg.Interval bounds = data.Bounds();
                int s = 0;
                double step = radius * 2.0 / count;
                double space = (step / count);
                int total = 0;
                double w = 0;
                foreach (DataSet ds in data)total = Math.Max(total, ds.Values.Count);

                switch (this.chartType)
                {
                    case ChartTypes.Pie:
                        plane.Origin = center;

                        Rg.Circle circle = new Rg.Circle(plane, radius);
                        output.Add(circle.ToNurbsCurve());
                        clrs.Add(Sd.Color.Black);

                        List<double> vals = this.data[0].Values.ReMapStack();
                        
                        foreach (double t in vals)
                        {
                            output.Add(new Rg.Line(center, circle.PointAt(t)).ToNurbsCurve());
                            clrs.Add(data[0].Colors[s]);
                            s++;
                        }

                        break;
                    case ChartTypes.Area:
                        foreach(DataSet ds in data)
                        {
                            List<double> areaVals = ds.Values.ReMap(bounds);
                            Rg.Polyline plot = new Rg.Polyline();
                            int lineCount = areaVals.Count-1;
                            double i = 0;
                            plot.Add(corner);
                            foreach (double t in areaVals)
                            {
                                plot.Add(corner + new Rg.Vector3d((double)(i/ lineCount) *radius*2, t * radius*2, 0));
                                i++;
                            }
                            plot.Add(corner + new Rg.Vector3d(radius*2, 0, 0));
                            plot.Add(corner);

                            output.Add(plot.ToNurbsCurve());
                            clrs.Add(ds.Graphic.Stroke);
                        }

                        break;
                    case ChartTypes.Line:
                        foreach(DataSet ds in data)
                        {
                            List<double> lineVals = ds.Values.ReMap(bounds);
                            Rg.Polyline plot = new Rg.Polyline();
                            int lineCount = lineVals.Count-1;
                            double i = 0;
                            foreach (double t in lineVals)
                            {
                                plot.Add(corner + new Rg.Vector3d((double)(i/ lineCount) *radius*2, t * radius*2, 0));
                                i++;
                            }

                            output.Add(plot.ToNurbsCurve());
                            clrs.Add(ds.Graphic.Stroke);
                        }

                        break;
                    case ChartTypes.Column:
                        step = radius*2 / total;
                        space = (step / count);
                        foreach (DataSet ds in data)
                        {
                            List<double> lineVals = ds.Values.ReMap(bounds);
                            int colCount = lineVals.Count;
                            double i = 0;
                            foreach (double t in lineVals)
                            {
                                double x = s* space+ i* step;
                                Rg.Point3d ptX = corner + new Rg.Vector3d(x, 0, 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(x, t * (radius * 2 - 1) + 1, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(space * 0.9, 0, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                clrs.Add(ds.Colors[(int)i]);
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
                                Rg.Point3d ptX = corner + new Rg.Vector3d(0,y, 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(t * (radius * 2 - 1) + 1,y, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(0, space*0.9, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                clrs.Add(ds.Colors[(int)i]);
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
                            for(int i=0;i< barVals.Count-1;i++)
                            {
                                double y = (double)s / total*radius*2.0;
                                Rg.Point3d ptX = corner + new Rg.Vector3d(barVals[i] * (radius * 2 - 1) + Convert.ToInt32(i > 0),y, 0);
                                Rg.Point3d ptY = corner + new Rg.Vector3d(barVals[i + 1] * (radius * 2 - 1) + 1,y, 0);
                                Rg.Vector3d vcX = new Rg.Vector3d(0, w * 0.9, 0);
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX, ptY, ptY + vcX, ptX + vcX, ptX }).ToNurbsCurve());
                                clrs.Add(this.data[i].Colors[(int)s]);
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
                                output.Add(new Rg.Polyline(new List<Rg.Point3d> { ptX,ptY,ptY+vcX,ptX+vcX,ptX} ).ToNurbsCurve());
                                clrs.Add(this.data[i].Colors[(int)s]);
                            }
                            s++;
                        }
                        break;
                }
            }
            colors = clrs;
                return output;
        }

        public void SetData(List<DataSet> data)
        {
            foreach (DataSet d in data)
            {
                this.data.Add(new DataSet(d));
            }
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

        public void Render(Pd.XGraphics graph, Pf.PdfPage page, Rg.Plane coordinateframe)
        {
            switch (this.shapeType)
            {
                case ShapeType.Line:
                    graph.DrawLine(graphic.ToPdf(), line.From.ToPdf(), line.To.ToPdf());
                    break;

                case ShapeType.Polyline:
                    Pd.XGraphicsPath plinePath = new Pd.XGraphicsPath();

                    plinePath.StartFigure();
                    plinePath.AddLines(polyline.ToPdf().ToArray());
                        if (polyline.IsClosed) plinePath.CloseFigure();
                    graph.DrawPath(graphic.ToPdf(), graphic.Color.ToPdfBrush(), plinePath);
                    break;

                case ShapeType.Circle:
                    graph.DrawEllipse(graphic.ToPdf(), graphic.Color.ToPdfBrush(), boundary.ToPdf());
                    break;

                case ShapeType.Ellipse:
                    graph.DrawEllipse(graphic.ToPdf(), graphic.Color.ToPdfBrush(), boundary.ToPdf());
                    break;

                case ShapeType.Bezier:
                    graph.DrawBeziers(graphic.ToPdf(), curve.ToBezierPolyline().ToPdf().ToArray());
                    break;

                case ShapeType.Brep:
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
                    graph.DrawPath(graphic.ToPdf(), graphic.Color.ToPdfBrush(), polyPath);
                    break;

                case ShapeType.ImageFrame:
                case ShapeType.ImageObj:
                    Stream stream = image.ToStream();
                    Pd.XImage xImageA = Pd.XImage.FromStream(stream);

                    graph.DrawImage(xImageA, this.boundary.ToPdf());
                    stream.Dispose();
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
                case ShapeType.LinkObj:
                    switch (this.linkType)
                    {
                        case LinkTypes.Hyperlink:
                            page.AddWebLink(boundary.ToPdfRect(coordinateframe), this.content);
                            break;
                        case LinkTypes.Filepath:
                            page.AddFileLink(boundary.ToPdfRect(coordinateframe), this.content);
                            break;
                        case LinkTypes.Page:
                            int index = 0;
                            bool isInt = int.TryParse(this.content, out index);
                            if(isInt) page.AddDocumentLink(boundary.ToPdfRect(coordinateframe), index + 1);

                            break;
                    }
                    break;
                case ShapeType.ChartObj:
                    Pc.Chart chart = CombinationChart();

                    if (this.justification != Justification.None)
                    {
                        chart.Legend.Docking = this.justification.ToPdf();
                        chart.Legend.Font.Color = this.font.Color.ToPdf();
                        chart.Legend.Font.Name = this.font.Family;
                        chart.Legend.Font.Size = this.font.Size;
                        chart.Legend.Font.Bold = this.font.IsBold;
                        chart.Legend.Font.Italic = this.font.IsItalic;
                        if (this.font.IsUnderlined) chart.Legend.Font.Underline = Pc.Underline.Single;
                    }

                    chart.Font.Color = this.FontColor.ToPdf();
                    chart.Font.Name = this.FontFamily;
                    chart.Font.Size = this.FontSize;
                    chart.Font.Bold = this.font.IsBold;
                    chart.Font.Italic = this.font.IsItalic;
                    if (this.font.IsUnderlined) chart.Font.Underline = Pc.Underline.Single;

                    if (this.HasXAxis)
                    {
                        chart.XAxis.MajorTickMark = Pc.TickMarkType.Outside;
                        chart.XAxis.Title.Caption = this.xAxis;
                        chart.XAxis.HasMajorGridlines = true;

                        chart.XAxis.LineFormat.Color = this.graphic.Stroke.ToPdf();
                        chart.XAxis.LineFormat.Width = this.graphic.Weight;

                        chart.XAxis.MajorGridlines.LineFormat.Color = this.graphic.Stroke.ToPdf();
                        chart.XAxis.MajorGridlines.LineFormat.Width = this.graphic.Weight;

                        chart.XAxis.TickLabels.Font.Color = this.FontColor.ToPdf();
                        chart.XAxis.TickLabels.Font.Name = this.FontFamily;
                        chart.XAxis.TickLabels.Font.Size = this.FontSize;
                        chart.XAxis.TickLabels.Font.Bold = this.font.IsBold;
                        chart.XAxis.TickLabels.Font.Italic = this.font.IsItalic;
                        if (this.font.IsUnderlined) chart.XAxis.TickLabels.Font.Underline = Pc.Underline.Single;

                        chart.XAxis.Title.Font.Color = this.FontColor.ToPdf();
                        chart.XAxis.Title.Font.Name = this.FontFamily;
                        chart.XAxis.Title.Font.Size = this.FontSize;
                        chart.XAxis.Title.Font.Bold = this.font.IsBold;
                        chart.XAxis.Title.Font.Italic = this.font.IsItalic;
                        if (this.font.IsUnderlined) chart.XAxis.Title.Font.Underline = Pc.Underline.Single;
                    }
                    else
                    {
                        chart.XAxis.HasMajorGridlines = false;
                        chart.XAxis.MajorTickMark = Pc.TickMarkType.None;
                    }

                    if (this.HasYAxis)
                    {
                        chart.YAxis.MajorTickMark = Pc.TickMarkType.Outside;
                        chart.YAxis.Title.Caption = this.yAxis;
                        chart.YAxis.HasMajorGridlines = true;

                        chart.YAxis.LineFormat.Color = this.graphic.Stroke.ToPdf();
                        chart.YAxis.LineFormat.Width = this.graphic.Weight;

                        chart.YAxis.MajorGridlines.LineFormat.Color = this.graphic.Stroke.ToPdf();
                        chart.YAxis.MajorGridlines.LineFormat.Width = this.graphic.Weight;

                        chart.YAxis.TickLabels.Format = "#.####";
                        chart.YAxis.TickLabels.Font.Color = this.FontColor.ToPdf();
                        chart.YAxis.TickLabels.Font.Name = this.FontFamily;
                        chart.YAxis.TickLabels.Font.Size = this.FontSize;
                        chart.YAxis.TickLabels.Font.Bold = this.font.IsBold;
                        chart.YAxis.TickLabels.Font.Italic = this.font.IsItalic;
                        if (this.font.IsUnderlined) chart.YAxis.TickLabels.Font.Underline = Pc.Underline.Single;

                        chart.YAxis.Title.Font.Color = this.FontColor.ToPdf();
                        chart.YAxis.Title.Font.Name = this.FontFamily;
                        chart.YAxis.Title.Font.Size = this.FontSize;
                        chart.YAxis.Title.Font.Bold = this.font.IsBold;
                        chart.YAxis.Title.Font.Italic = this.font.IsItalic;
                        if (this.font.IsUnderlined) chart.YAxis.Title.Font.Underline = Pc.Underline.Single;
                    }


                    Pc.ChartFrame frame = new Pc.ChartFrame();
                    frame.Location = new Pd.XPoint(boundary.Corner(0).X, boundary.Corner(3).Y);
                    frame.Size = new Pd.XSize(boundary.Width, boundary.Height);

                    frame.Background = graphic.Color.ToPdfBrush();

                    frame.Add(chart);
                    frame.Draw(graph);

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
                        series.HasDataLabel = d.LabelData;

                        series.DataLabel.Font.Color = d.Font.Color.ToPdf();
                        series.DataLabel.Font.Name = d.Font.Family;
                        series.DataLabel.Font.Size = d.Font.Size;
                        series.DataLabel.Font.Bold = d.Font.IsBold;
                        series.DataLabel.Font.Italic = d.Font.IsItalic;
                        if (d.Font.IsUnderlined) series.DataLabel.Font.Underline = Pc.Underline.Single;

                        if ((int)this.chartType < 4)
                        {
                            if (d.Graphic.HasColor)
                            {
                                if (d.Colors.Count > 0)
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

                    pieseries.Add(dt.Values.ToArray());
                    pieseries.HasDataLabel = true;
                    pieseries.DataLabel.Font.Color = dt.Font.Color.ToPdf();
                    pieseries.DataLabel.Font.Name = dt.Font.Family;
                    pieseries.DataLabel.Font.Size = dt.Font.Size;
                    pieseries.DataLabel.Font.Bold = dt.Font.IsBold;
                    pieseries.DataLabel.Font.Italic = dt.Font.IsItalic;
                    if (dt.Font.IsUnderlined) pieseries.DataLabel.Font.Underline = Pc.Underline.Single;

                    if (dt.Graphic.HasColor)
                    {
                        if (dt.Colors.Count > 0)
                        {
                            for (int p = 0; p < pieseries.Elements.Count; p++)
                            {
                                pieseries.Elements[p].FillFormat.Color = dt.Colors[p % dt.Colors.Count].ToPdf();
                            }
                        }
                        else
                        {
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
