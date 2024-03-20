using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

using Rg = Rhino.Geometry;

using Ps = PdfSharp;
using Pf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;
using Pl = PdfSharp.Drawing.Layout;
using Pc = PdfSharp.Charting;

using Md = MigraDoc.DocumentObjectModel;
using Mr = MigraDoc.Rendering;

using System.IO;
using System.Windows.Media.Imaging;
using Grasshopper.Kernel.Types;
using System.Reflection;

namespace PdfPlus
{
    public static class PdfExtensions
    {
        #region Grasshopper 

        public static bool TryGetGeometricShape(this IGH_Goo goo, ref Shape shape)
        {
            bool isValid = false;
            switch (goo.TypeName)
            {
                case "Curve":
                    Rg.Curve curve;
                    isValid = true;

                    if (goo.CastTo<Rg.Curve>(out curve))
                    {
                        string type = goo.ToString();

                        switch (type)
                        {
                            default:
                                shape = Shape.CreateGeometry(curve.ToNurbsCurve(), new Graphic());
                                break;
                            case "Polyline Curve":
                                Rg.Polyline pline;
                                if (curve.TryGetPolyline(out pline)) shape = Shape.CreateGeometry(pline, new Graphic());
                                break;
                            case "Line-like Curve":
                                shape = Shape.CreateGeometry(new Rg.Line(curve.PointAtStart, curve.PointAtEnd), new Graphic());
                                break;
                            case "Elliptical Curve":
                                Rg.Ellipse ellipse;
                                curve.TryGetEllipse(out ellipse);
                                shape = Shape.CreateGeometry(ellipse, new Graphic());
                                break;
                        }
                        isValid = true;
                    }
                    break;
                case "Arc":
                    Rg.Arc arc;

                    if (goo.CastTo<Rg.Arc>(out arc))
                    {
                        shape = Shape.CreateGeometry(arc, new Graphic());
                        isValid = true;
                    }
                    break;
                case "Circle":
                    Rg.Circle circle;

                    if (goo.CastTo<Rg.Circle>(out circle))
                    {
                        shape = Shape.CreateGeometry(circle, new Graphic());
                        isValid = true;
                    }
                    break;
                case "Line":
                    Rg.Line line;

                    if (goo.CastTo<Rg.Line>(out line))
                    {
                        shape = Shape.CreateGeometry(line, new Graphic());
                        isValid = true;
                    }
                    break;
                case "Rectangle":
                    Rg.Rectangle3d rect;
                    if (goo.CastTo<Rg.Rectangle3d>(out rect))
                    {
                        shape = Shape.CreateGeometry(rect, new Graphic());
                        isValid = true;
                    }
                    break;
                case "Surface":
                    Rg.Surface surface;
                    if (goo.CastTo<Rg.Surface>(out surface))
                    {
                        Rg.Brep srfBrep = Rg.Brep.CreateFromSurface(surface);
                        shape = Shape.CreateGeometry(srfBrep, new Graphic());
                        isValid = true;
                    }
                    break;
                case "Brep":
                    Rg.Brep brep;
                    if (goo.CastTo<Rg.Brep>(out brep))
                    {
                        shape = Shape.CreateGeometry(brep, new Graphic());
                        isValid = true;
                    }
                    break;
                case "Mesh":
                    Rg.Mesh mesh;
                    if (goo.CastTo<Rg.Mesh>(out mesh))
                    {
                        shape = Shape.CreateGeometry(mesh, new Graphic());
                        isValid = true;
                    }
                    break;
            }
            return isValid;
        }
        public static bool TryGetShape(this IGH_Goo goo, ref Shape shape)
        {
            Shape shp;
            bool isValid = false;

            if (goo.CastTo<Shape>(out shp))
            {
                shape = new Shape(shp);
                isValid = true;
            }
            else
            {
                isValid = goo.TryGetGeometricShape(ref shp);
                if (isValid) shape = new Shape(shp);
            }
            return isValid;
        }

        public static bool TryGetBlock(this IGH_Goo goo, ref Block block)
        {
            Block blk;
            bool isValid = false;

            if (goo.CastTo<Block>(out blk))
            {
                block = new Block(blk);
                isValid = true;
            }
            else
            {
                Shape shp = null;
                isValid = goo.TryGetGeometricShape(ref shp);
                if (isValid) block = Block.CreateDrawing(shp);
            }

            return isValid;
        }

        public static bool TryGetElement(this IGH_Goo goo, ref Element element)
        {
            Element elem;
            bool isValid = false;

            if (goo.CastTo<Element>(out elem))
            {
                element = new Element(elem);
                isValid = true;
            }
            else
            {
                Shape shp = null;
                isValid = goo.TryGetGeometricShape(ref shp);
                if (isValid) element = shp;
            }

            return isValid;
        }

        public static bool TryGetDataSet(this IGH_Goo goo, ref DataSet dataSet)
        {
            Element elem;
            bool isValid = false;

            if (goo.CastTo<DataSet>(out dataSet))
            {
                dataSet = new DataSet(dataSet);
                isValid = true;
            }

            return isValid;
        }

        public static bool TryGetBitmap(this IGH_Goo goo, ref Sd.Bitmap bitmap, ref string path)
        {

            string filePath = string.Empty;
            path = filePath;
            goo.CastTo<string>(out filePath);
            Sd.Bitmap bmp = null;

            if (goo.CastTo<Sd.Bitmap>(out bmp))
            {
                bitmap = new Sd.Bitmap(bmp);
                return true;
            }
            else if (!PdfPlusEnvironment.FileIoBlocked && File.Exists(filePath))
            {
                if (filePath.GetBitmapFromFile(out bmp))
                {
                    path = filePath;
                    bitmap = bmp;
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool GetBitmapFromFile(this string FilePath, out Sd.Bitmap bitmap)
        {
            bitmap = null;
            if (Path.HasExtension(FilePath))
            {
                string extension = Path.GetExtension(FilePath);
                switch (extension)
                {
                    default:
                        return (false);
                    case ".bmp":
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".jfif":
                    case ".gif":
                    case ".tif":
                    case ".tiff":
                        bitmap = (Sd.Bitmap)Sd.Bitmap.FromFile(FilePath);
                        return (bitmap != null);
                }

            }

            return (false);
        }

        #endregion

        #region Pdf Sharp

        public static double GetValue(this Pd.XUnit input, Units unit)
        {
            switch (unit)
            {
                default:
                    return input.Millimeter;
                case Units.Centimeter:
                    return input.Centimeter;
                case Units.Inch:
                    return input.Inch;
                case Units.Point:
                    return input.Point;
            }
        }

        public static Pd.XUnit SetValue(this Units unit, double value)
        {
            switch (unit)
            {
                default:
                    return Pd.XUnit.FromMillimeter(value);
                case Units.Centimeter:
                    return Pd.XUnit.FromCentimeter(value);
                case Units.Inch:
                    return Pd.XUnit.FromInch(value);
                case Units.Point:
                    return Pd.XUnit.FromPoint(value);
            }
        }

        public static Md.Shapes.Charts.ChartType ToMigraDoc(this Element.ChartTypes input)
        {
            switch (input)
            {
                default:
                    return Md.Shapes.Charts.ChartType.Line;
                case Element.ChartTypes.Area:
                    return Md.Shapes.Charts.ChartType.Area2D;
                case Element.ChartTypes.Bar:
                    return Md.Shapes.Charts.ChartType.Bar2D;
                case Element.ChartTypes.BarStacked:
                    return Md.Shapes.Charts.ChartType.BarStacked2D;
                case Element.ChartTypes.Column:
                    return Md.Shapes.Charts.ChartType.Column2D;
                case Element.ChartTypes.ColumnStacked:
                    return Md.Shapes.Charts.ChartType.ColumnStacked2D;
                case Element.ChartTypes.Pie:
                    return Md.Shapes.Charts.ChartType.Pie2D;
            }
        }

        public static string Abbreviation(this Units input)
        {
            switch (input)
            {
                case Units.Millimeter:
                    return "mm";
                case Units.Centimeter:
                    return "cm";
                case Units.Inch:
                    return "in";
                case Units.Point:
                    return "pt";
                default:
                    return "";
            }
        }

        #region render geometry

        #endregion

        #region charts

        public static Pc.DockingType ToPdf(this Alignment input)
        {
            switch (input)
            {
                default:
                    return Pc.DockingType.Bottom;
                case Alignment.Left:
                    return Pc.DockingType.Left;
                case Alignment.Right:
                    return Pc.DockingType.Right;
                case Alignment.Top:
                    return Pc.DockingType.Top;
            }
        }

        public static Pc.ChartType ToPdf(this Shape.ChartTypes input)
        {
            switch (input)
            {
                default:
                    return Pc.ChartType.ColumnStacked2D;
                case Shape.ChartTypes.Area:
                    return Pc.ChartType.Area2D;
                    break;
                case Shape.ChartTypes.Bar:
                    return Pc.ChartType.Bar2D;
                    break;
                case Shape.ChartTypes.BarStacked:
                    return Pc.ChartType.BarStacked2D;
                    break;
                case Shape.ChartTypes.Column:
                    return Pc.ChartType.Column2D;
                    break;
                case Shape.ChartTypes.Line:
                    return Pc.ChartType.Line;
                    break;
                case Shape.ChartTypes.Pie:
                    return Pc.ChartType.Pie2D;
                    break;
            }
        }

        #endregion

        #region graphics

        public static Pd.XBrush ToPdfBrush(this Sd.Color input)
        {
            return new Pd.XSolidBrush(input.ToPdf());
        }

        public static Pd.XColor ToPdf(this Sd.Color input)
        {
            return Pd.XColor.FromArgb(input.A, input.R, input.G, input.B);
        }

        public static Pd.XPen ToPdf(this Graphic input)
        {
            Pd.XPen pen = new Pd.XPen(input.Stroke.ToPdf(), input.Weight);
            if (input.HasPattern) pen.DashPattern = input.Pattern.ToArray();
            return pen;
        }

        #endregion

        #region page

        public static Ps.PageSize ToSharp(this SizesA size)
        {
            switch (size)
            {
                default:
                    return Ps.PageSize.A0;
                case SizesA.A1:
                    return Ps.PageSize.A1;
                case SizesA.A2:
                    return Ps.PageSize.A2;
                case SizesA.A3:
                    return Ps.PageSize.A3;
                case SizesA.A4:
                    return Ps.PageSize.A4;
                case SizesA.A5:
                    return Ps.PageSize.A5;
            }
        }

        public static Ps.PageSize ToSharp(this SizesB size)
        {
            switch (size)
            {
                default:
                    return Ps.PageSize.B0;
                case SizesB.B1:
                    return Ps.PageSize.B1;
                case SizesB.B2:
                    return Ps.PageSize.B2;
                case SizesB.B3:
                    return Ps.PageSize.B3;
                case SizesB.B4:
                    return Ps.PageSize.B4;
                case SizesB.B5:
                    return Ps.PageSize.B5;
            }
        }

        public static Ps.PageSize ToSharp(this SizesImperial size)
        {
            switch (size)
            {
                default:
                    return Ps.PageSize.Letter;
                case SizesImperial.Ledger:
                    return Ps.PageSize.Ledger;
                case SizesImperial.Legal:
                    return Ps.PageSize.Legal;
                case SizesImperial.Statement:
                    return Ps.PageSize.Statement;
                case SizesImperial.Tabloid:
                    return Ps.PageSize.Tabloid;
            }
        }

        public static PageOrientation ToPlus(this Ps.PageOrientation input)
        {
            switch (input)
            {
                default:
                    return PageOrientation.Portrait;
                    break;
                case Ps.PageOrientation.Landscape:
                    return PageOrientation.Landscape;
                    break;
            }
        }

        #endregion

        #region document

        public static Ps.Pdf.PdfPageLayout ToPdf(this PageLayouts input)
        {
            switch (input)
            {
                default:
                    return Ps.Pdf.PdfPageLayout.SinglePage;
                case PageLayouts.SingleScroll:
                    return Ps.Pdf.PdfPageLayout.OneColumn;
                case PageLayouts.Double:
                    return Ps.Pdf.PdfPageLayout.TwoPageLeft;
                case PageLayouts.DoubleScroll:
                    return Ps.Pdf.PdfPageLayout.TwoColumnLeft;
                case PageLayouts.DoubleCover:
                    return Ps.Pdf.PdfPageLayout.TwoPageRight;
                case PageLayouts.DoubleCoverScroll:
                    return Ps.Pdf.PdfPageLayout.TwoColumnRight;
            }
        }

        public static Ps.PageOrientation ToPdf(this PageOrientation input)
        {
            switch (input)
            {
                default:
                    return Ps.PageOrientation.Portrait;
                case PageOrientation.Landscape:
                    return Ps.PageOrientation.Landscape;
            }
        }

        #endregion

        #region geometry

        public static Rg.Rectangle3d Inflate(this Rg.Rectangle3d input, double offset)
        {
            Rg.Interval w = input.X;
            Rg.Interval h = input.Y;

            return new Rg.Rectangle3d(input.Plane, new Rg.Interval(w.Min - offset, w.Max + offset), new Rg.Interval(h.Min - offset, h.Max + offset));
        }

        public static Rg.Polyline ToBezierPolyline(this Rg.NurbsCurve input, double multiple = 1.0)
        {
            List<Rg.Point3d> points = new List<Rg.Point3d>();
            Rg.BezierCurve[] beziers = Rg.BezierCurve.CreateCubicBeziers(input, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance * multiple, Rhino.RhinoDoc.ActiveDoc.PageAngleToleranceRadians * multiple);
            foreach (Rg.BezierCurve bezier in beziers)
            {
                points.Add(bezier.GetControlVertex3d(0));
                points.Add(bezier.GetControlVertex3d(1));
                points.Add(bezier.GetControlVertex3d(2));
            }
            points.Add(beziers[beziers.Length - 1].GetControlVertex3d(3));

            return new Rg.Polyline(points);
        }

        public static List<double> ReMap(this List<double> input, Rg.Interval bounds)
        {
            List<double> output = new List<double>();

            foreach (double val in input) output.Add((val - bounds.Min) / (bounds.Max - bounds.Min));

            return output;
        }

        public static List<List<double>> ReMapSet(this List<DataSet> input)
        {
            List<List<double>> output = new List<List<double>>();

            int count = 0;
            foreach (DataSet ds in input)
            {
                count = Math.Max(count, ds.Values.Count);
            }

            List<double> peaks = new List<double>();
            for (int i = 0; i < count; i++)
            {
                peaks.Add(0);
                foreach (DataSet ds in input)
                {
                    if (i < ds.Values.Count)
                    {
                        peaks[i] += ds.Values[i];
                    }
                }
            }

            Rg.Interval bounds = new Rg.Interval(0, peaks.Max());

            for (int i = 0; i < count; i++)
            {
                peaks.Add(0);
                double j = 0;
                output.Add(new List<double> { 0 });
                foreach (DataSet ds in input)
                {
                    if (i < ds.Values.Count)
                    {
                        j = j + ds.Values[i] / bounds.Max;
                        output[i].Add(j);
                    }
                }
            }

            return output;
        }

        public static List<double> ReMapStack(this List<double> input)
        {
            double min = input.Min();
            double max = input.Max();
            List<double> output = new List<double>();

            double t = 0;
            foreach (double val in input)
            {
                t += (val - min) / (max - min);
                output.Add(t);
            }
            return output;
        }

        public static Rg.Interval Bounds(this List<DataSet> input)
        {
            double min = input[0].Values.Min();
            double max = input[0].Values.Max();

            foreach (DataSet data in input)
            {
                min = Math.Min(min, data.Values.Min());
                max = Math.Max(max, data.Values.Max());
            }
            return new Rg.Interval(min, max);
        }

        public static Pd.XPoint ToPdf(this Rg.Point3d input)
        {
            return new Pd.XPoint(input.X, input.Y);
        }

        public static Rg.Point3d ToRhino(this Pd.XPoint input)
        {
            return new Rg.Point3d(input.X, input.Y, 0);
        }

        public static List<Pd.XPoint> ToPdf(this Rg.Polyline input)
        {
            List<Pd.XPoint> points = new List<Pd.XPoint>();
            foreach (Rg.Point3d point in input) points.Add(point.ToPdf());
            return points;
        }

        public static Pd.XRect ToPdf(this Rg.Rectangle3d input)
        {

            Pd.XRect rect = new Pd.XRect(input.Corner(0).X, input.Corner(3).Y, input.Width, input.Height);
            return rect;
        }

        public static Pf.PdfRectangle ToPdfRect(this Rg.Rectangle3d input, Rg.Plane plane)
        {
            Pd.XRect rect = new Pd.XRect(input.Corner(0).X - plane.OriginX, input.Corner(0).Y - plane.OriginY, input.Width, input.Height);
            Pf.PdfRectangle rectangle = new Pf.PdfRectangle(rect);
            return rectangle;
        }

        public static Rg.Interval SpanX(this Pd.XRect input)
        {
            return new Rg.Interval(input.Left, input.Right);
        }

        public static Rg.Interval SpanY(this Pd.XRect input)
        {
            return new Rg.Interval(input.Bottom, input.Top);
        }

        public static Rg.Rectangle3d ToRhino(this Pd.XRect input, Rg.Plane frame)
        {
            Rg.Point3d a = input.BottomLeft.ToRhino();
            Rg.Point3d b = input.TopRight.ToRhino();
            Rg.Rectangle3d boundary = new Rg.Rectangle3d(frame, frame.PointAt(a.X, a.Y), frame.PointAt(b.X, b.Y));
            return boundary;
        }

        public static Rg.Rectangle3d ToRhino(this Pf.PdfRectangle input, Rg.Plane frame)
        {
            Pd.XRect rect = input.ToXRect();
            Rg.Rectangle3d boundary = rect.ToRhino(frame);
            return boundary;
        }

        public static Stream ToStream(this Sd.Bitmap input)
        {
            MemoryStream stream = new MemoryStream();
            input.Save(stream, Sd.Imaging.ImageFormat.Png);
            return stream;
        }


        #endregion

        #region text



        #endregion

        #region font

        public static Pd.XFont ToPdf(this Font input)
        {
            return new Pd.XFont(input.Family, input.Size, input.Style.ToPdf());
        }

        public static Pd.XStringAlignment ToPdfLine(this Justification input)
        {
            switch (input)
            {
                default:
                    return Pd.XStringAlignment.Near;
                case Justification.Center:
                    return Pd.XStringAlignment.Center;
                case Justification.Right:
                    return Pd.XStringAlignment.Far;
            }
        }

        public static Pl.XParagraphAlignment ToPdf(this Justification input)
        {
            switch (input)
            {
                default:
                    return Pl.XParagraphAlignment.Left;
                case Justification.Center:
                    return Pl.XParagraphAlignment.Center;
                case Justification.Right:
                    return Pl.XParagraphAlignment.Right;
                case Justification.Justify:
                    return Pl.XParagraphAlignment.Justify;
            }
        }

        public static Pd.XFontStyle ToPdf(this FontStyle input)
        {
            switch (input)
            {
                default:
                    return Pd.XFontStyle.Regular;
                case FontStyle.Bold:
                    return Pd.XFontStyle.Bold;
                case FontStyle.BoldItalic:
                    return Pd.XFontStyle.BoldItalic;
                case FontStyle.Italic:
                    return Pd.XFontStyle.Italic;
                case FontStyle.Strikeout:
                    return Pd.XFontStyle.Strikeout;
                case FontStyle.Underline:
                    return Pd.XFontStyle.Underline;
            }
        }

        #endregion

        #endregion

        #region MigraDoc

        #region units

        public static Md.Unit ToMigraDoc(this Pd.XUnit input)
        {
            switch (input.Type)
            {
                default:
                    return new Md.Unit(input.Point, Md.UnitType.Point);
                case Pd.XGraphicsUnit.Millimeter:
                    return new Md.Unit(input.Millimeter, Md.UnitType.Millimeter);
                case Pd.XGraphicsUnit.Centimeter:
                    return new Md.Unit(input.Centimeter, Md.UnitType.Centimeter);
                case Pd.XGraphicsUnit.Inch:
                    return new Md.Unit(input.Inch, Md.UnitType.Inch);
                case Pd.XGraphicsUnit.Presentation:
                    return new Md.Unit(input.Presentation, Md.UnitType.Pica);
            }
        }

        #endregion

        #region pages

        public static Md.Orientation ToMigraDoc(this Ps.PageOrientation input)
        {
            switch (input)
            {
                default:
                    return Md.Orientation.Landscape;
                case Ps.PageOrientation.Portrait:
                    return Md.Orientation.Portrait;
            }
        }

        #endregion

        #region font

        public static Md.ParagraphFormat ToMigraDocParagraphFormat(this Font input, Md.ParagraphFormat format)
        {
            if (input.IsModified)
            {
                if (input.HasFamily) format.Font.Name = input.Family;
                if (input.HasSize)format.Font.Size = input.Size;
                if (input.HasColor) format.Font.Color = input.Color.ToMigraDoc();
                if (input.HasJustification) format.Alignment = input.Justification.ToMigraDocParagraphAlignment();
                if (input.HasStyle)
                {
                    format.Font.Bold = input.IsBold;
                    format.Font.Italic = input.IsItalic;
                    format.Font.Underline = Md.Underline.None;
                    if (input.IsUnderlined) format.Font.Underline = Md.Underline.Single;
                }

            }

            return format;
        }

        public static Md.ParagraphAlignment ToMigraDocParagraphAlignment(this Justification input)
        {
            switch (input)
            {
                default:
                    return Md.ParagraphAlignment.Left;
                case Justification.Right:
                    return Md.ParagraphAlignment.Right;
                case Justification.Center:
                    return Md.ParagraphAlignment.Center;
                case Justification.Justify:
                    return Md.ParagraphAlignment.Justify;
            }
        }

        public static Md.Shapes.ShapePosition ToMigraDocShapePosition(this Justification input)
        {
            switch (input)
            {
                default:
                    return Md.Shapes.ShapePosition.Left;
                case Justification.Right:
                    return Md.Shapes.ShapePosition.Right;
                case Justification.Center:
                    return Md.Shapes.ShapePosition.Center;
            }
        }

        #endregion

        #region graphics

        public static Md.Color ToMigraDoc(this Sd.Color input)
        {
            return Md.Color.FromArgb(input.A, input.R, input.G, input.B);
        }

        #endregion

        #region images

        public static byte[] ToByteArray(this Sd.Bitmap input)
        {
            MemoryStream stream = new MemoryStream();

            input.Save(stream, Sd.Imaging.ImageFormat.Png);
            stream.Position = 0;
            byte[] buffer = stream.ToArray();
            stream.Close();

            return buffer;
        }

        public static string ToBase64String(this Sd.Bitmap input, string prefix = "")
        {
            MemoryStream stream = new MemoryStream();
            
            input.Save(stream, Sd.Imaging.ImageFormat.Png);
            stream.Position = 0;
            byte[] buffer = stream.ToArray();
            stream.Close();

            string output = Convert.ToBase64String(buffer);
            buffer = null;

            return prefix+output;
        }

        #endregion

        #region geometry

        public static Rg.Rectangle3d ToRectangle3d(this Mr.Area input)
        {
            return new Rg.Rectangle3d(new Rg.Plane(new Rg.Point3d(input.X, input.Y, 0), Rg.Vector3d.ZAxis), input.Width, input.Height);
        }

        #endregion

        #endregion
    }
}
