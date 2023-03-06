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

using System.IO;
using System.Windows.Media.Imaging;
using Grasshopper.Kernel.Types;

namespace PdfPlus
{
    public static class PdfExtensions
    {
        #region Grasshopper 

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
                switch (goo.TypeName)
                {
                    case "Curve":
                        Rg.Curve curve;
                        isValid = true;

                        if (goo.CastTo<Rg.Curve>(out curve))
                        {
                            string type = goo.ToString();

                            switch (type.Substring(0, 6))
                            {
                                default:
                                    shape = new Shape(curve.ToNurbsCurve(), new Graphic());
                                    break;
                                case "Polyli": //ne Curve
                                    Rg.Polyline pline;
                                    if (curve.TryGetPolyline(out pline)) shape = new Shape(pline, new Graphic());
                                    break;
                                case "Line-l": //ike Curve
                                    shape = new Shape(new Rg.Line(curve.PointAtStart, curve.PointAtEnd), new Graphic());
                                    break;
                                case "Ellipt": //ical Curve
                                    Rg.Ellipse ellipse;
                                    curve.TryGetEllipse(out ellipse);
                                    shape = new Shape(ellipse, new Graphic());
                                    break;
                                case "Rectan":
                                    Rg.Rectangle3d rect;
                                    if(goo.CastTo<Rg.Rectangle3d>(out rect))
                                    shape = new Shape(rect, new Graphic());
                                    break;
                            }
                            isValid = true;
                        }
                        break;
                    case "Arc":
                        Rg.Arc arc;

                        if (goo.CastTo<Rg.Arc>(out arc))
                        {
                            shape = new Shape(arc, new Graphic());
                            isValid = true;
                        }
                        break;
                    case "Circle":
                        Rg.Circle circle;

                        if (goo.CastTo<Rg.Circle>(out circle))
                        {
                            shape = new Shape(circle, new Graphic());
                            isValid = true;
                        }
                        break;
                    case "Line":
                        Rg.Line line;

                        if (goo.CastTo<Rg.Line>(out line))
                        {
                            shape = new Shape(line, new Graphic());
                            isValid = true;
                        }
                        break;
                    case "Surface":
                        Rg.Surface surface;
                        if (goo.CastTo<Rg.Surface>(out surface))
                        {
                            Rg.Brep srfBrep = Rg.Brep.CreateFromSurface(surface);
                            shape = new Shape(srfBrep, new Graphic());
                            isValid = true;
                        }
                        break;
                    case "Brep":
                        Rg.Brep brep;
                        if (goo.CastTo<Rg.Brep>(out brep))
                        {
                            shape = new Shape(brep, new Graphic());
                            isValid = true;
                        }
                        break;
                    case "Mesh":
                        Rg.Mesh mesh;
                        if (goo.CastTo<Rg.Mesh>(out mesh))
                        {
                            shape = new Shape(mesh,new Graphic());
                            isValid = true;
                        }
                        break;
                }
            }
            return isValid;
        }
        public static bool TryGetBitmap(this IGH_Goo goo, ref Sd.Bitmap bitmap)
        {

            string filePath = string.Empty;
            goo.CastTo<string>(out filePath);
            Sd.Bitmap bmp = null;

            if (goo.CastTo<Sd.Bitmap>(out bmp))
            {
                bitmap = new Sd.Bitmap(bmp);
                return true;
            }
            else if (File.Exists(filePath))
            {
                if (filePath.GetBitmapFromFile(out bmp))
                {
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

        public static Pd.XUnit SetValue(this Units unit,double value)
        {
            switch (unit)
            {
                default:
                    return Pd.XUnit.FromMillimeter(value);
                    //break;
                case Units.Centimeter:
                    return Pd.XUnit.FromCentimeter(value);
                    //break;
                case Units.Inch:
                    return Pd.XUnit.FromInch(value);
                    //break;
                case Units.Point:
                    return Pd.XUnit.FromPoint(value);
                    //break;
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

        #region charts

        public static Pc.DockingType ToPdf(this Justification input)
        {
            switch (input)
            {
                default:
                    return Pc.DockingType.Bottom;
                case Justification.Left:
                    return Pc.DockingType.Left;
                case Justification.Right:
                    return Pc.DockingType.Right;
                case Justification.Top:
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
                    //break;
                case Shape.ChartTypes.Bar:
                    return Pc.ChartType.Bar2D;
                    //break;
                case Shape.ChartTypes.BarStacked:
                    return Pc.ChartType.BarStacked2D;
                    //break;
                case Shape.ChartTypes.Column:
                    return Pc.ChartType.Column2D;
                    //break;
                case Shape.ChartTypes.Line:
                    return Pc.ChartType.Line;
                    //break;
                case Shape.ChartTypes.Pie:
                    return Pc.ChartType.Pie2D;
                    //break;
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

        public static Rg.Polyline ToBezierPolyline(this Rg.NurbsCurve input, double multiple = 1.0)
        {
            List<Rg.Point3d> points = new List<Rg.Point3d>();
            Rg.BezierCurve[] beziers = Rg.BezierCurve.CreateCubicBeziers(input, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance* multiple, Rhino.RhinoDoc.ActiveDoc.PageAngleToleranceRadians* multiple);
            foreach(Rg.BezierCurve bezier in beziers)
            {
                points.Add(bezier.GetControlVertex3d(0));
                points.Add(bezier.GetControlVertex3d(1));
                points.Add(bezier.GetControlVertex3d(2));
            }
            points.Add(beziers[beziers.Length-1].GetControlVertex3d(3));

            return new Rg.Polyline(points);
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
            Pd.XRect rect = new Pd.XRect(input.Corner(0).X-plane.OriginX, input.Corner(0).Y - plane.OriginY, input.Width, input.Height);
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

        public static Rg.Rectangle3d ToRhino(this Pd.XRect input)
        {
            Rg.Rectangle3d boundary = new Rg.Rectangle3d(Rg.Plane.WorldXY, input.BottomLeft.ToRhino(), input.TopRight.ToRhino());
            return boundary;
        }

        public static Rg.Rectangle3d ToRhino(this Pf.PdfRectangle input)
        {
            Pd.XRect rect = input.ToXRect();
            Rg.Rectangle3d boundary = rect.ToRhino();
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

        public static Pl.XParagraphAlignment ToPdf(this Alignment input)
        {
            switch (input)
            {
                default:
                    return Pl.XParagraphAlignment.Left;
                case Alignment.Center:
                    return Pl.XParagraphAlignment.Center;
                case Alignment.Right:
                    return Pl.XParagraphAlignment.Right;
                case Alignment.Justify:
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

    }
}
