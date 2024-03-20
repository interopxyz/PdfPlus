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
    public class Shape: Element
    {

        #region members

        public enum ShapeType { None, Line, Polyline, Bezier, Circle, Ellipse, Arc, Brep, Mesh, TextBox, ImageFrame, TextObj, ImageObj, ChartObj, LinkObj };
        protected ShapeType shapeType = ShapeType.None;

        public enum LinkTypes { Hyperlink,Filepath,Page};
        protected LinkTypes linkType = LinkTypes.Hyperlink;

        #endregion

        #region constructors

        protected Shape():base()
        {
            this.elementType = ElementTypes.Shape;
        }

        public Shape(Shape shape):base(shape)
        {
            this.shapeType = shape.shapeType;
            this.linkType = shape.linkType;
        }

        #region text

        public static Shape CreateText(string content, Rg.Rectangle3d boundary, Alignment alignment, Font font)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.TextBox;

            shape.text = content;
            shape.alignment = alignment;
            shape.boundary = new Rg.Rectangle3d(boundary.Plane, boundary.Corner(0), boundary.Corner(2));
            shape.font = new Font(font);

            return shape;
        }

        public static Shape CreateText(string content, Rg.Point3d location, Font font)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.TextObj;

            shape.text = content;
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

            shape.imageObject = new Sd.Bitmap(bitmap);
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
            double factor = 72.0/96.0;
            plane.Origin = location - new Rg.Vector3d(0, bitmap.Height * factor * scale, 0);
            shape.boundary = new Rg.Rectangle3d(plane, location, new Rg.Point3d(location.X+bitmap.Width * factor * scale, location.Y+bitmap.Height * factor * scale, 0));

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

            shape.polyline = polyline.Duplicate();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Line line, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Line;

            shape.line = new Rg.Line(line.From, line.To);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Arc arc, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = arc.ToNurbsCurve();
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Circle circle, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Circle;

            shape.circle = new Rg.Circle(circle.Plane, circle.Radius);
            shape.boundary = new Rg.Rectangle3d(circle.Plane, new Rg.Interval(-circle.Radius, circle.Radius), new Rg.Interval(-circle.Radius, circle.Radius));
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Ellipse ellipse, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = ellipse.ToNurbsCurve();
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.BezierCurve bezier, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = bezier.ToNurbsCurve();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Curve curve, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = new Rg.NurbsCurve(curve.ToNurbsCurve());
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.NurbsCurve curve, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Bezier;

            shape.curve = new Rg.NurbsCurve(curve);
            shape.curve.MakePiecewiseBezier(true);
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Brep brep, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Brep;

            shape.brep = brep.DuplicateBrep();
            shape.graphic = new Graphic(graphic);

            return shape;
        }

        public static Shape CreateGeometry(Rg.Mesh mesh, Graphic graphic)
        {
            Shape shape = new Shape();
            shape.shapeType = ShapeType.Mesh;

            shape.mesh = mesh.DuplicateMesh();
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
                    Stream stream = imageObject.ToStream();
                    Pd.XImage xImageA = Pd.XImage.FromStream(stream);

                    graph.DrawImage(xImageA, this.boundary.ToPdf());
                    stream.Dispose();
                    break;

                case ShapeType.TextObj:
                    Pd.XStringFormat format = new Pd.XStringFormat();
                    format.Alignment = font.Justification.ToPdfLine();
                    format.LineAlignment = Pd.XLineAlignment.BaseLine;
                    graph.DrawString(this.text, font.ToPdf(), font.Color.ToPdfBrush(), location.ToPdf(), format);
                    break;

                case ShapeType.TextBox:
                    Pd.XFont pdfFont = font.ToPdf();

                    Pl.XTextFormatter textFormatter = new Pl.XTextFormatter(graph);
                    textFormatter.Alignment = this.Font.Justification.ToPdf();
                    textFormatter.LayoutRectangle = this.boundary.ToPdf();
                    textFormatter.Text = this.text;
                    textFormatter.Font = pdfFont;

                    textFormatter.DrawString(this.text, pdfFont, font.Color.ToPdfBrush(), this.boundary.ToPdf(), Pd.XStringFormats.TopLeft);
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
                            if(isInt) page.AddDocumentLink(boundary.ToPdfRect(coordinateframe), index + 1);

                            break;
                    }
                    break;
                case ShapeType.ChartObj:
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
                    chart.Font.Size = this.FontSize;
                    chart.Font.Bold = this.font.IsBold;
                    chart.Font.Italic = this.font.IsItalic;
                    if (this.font.IsUnderlined) chart.Font.Underline = Pc.Underline.Single;

                    if (this.HasXAxis)
                    {
                        chart.XAxis.MajorTickMark = Pc.TickMarkType.Outside;
                        chart.XAxis.Title.Caption = this.xAxis;
                        chart.XAxis.Title.Alignment = Pc.HorizontalAlignment.Center;
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
                        chart.YAxis.Title.Orientation = 90;
                        chart.YAxis.Title.Alignment = Pc.HorizontalAlignment.Center;
                        chart.YAxis.Title.VerticalAlignment = Pc.VerticalAlignment.Center;
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
