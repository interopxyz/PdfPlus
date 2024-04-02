using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

using Rg = Rhino.Geometry;
using Rd = Rhino.Display;
using Ro = Rhino.DocObjects;

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
    public static class GhExtensions
    {

        public static Ro.TextHorizontalAlignment ToRhHorizontalAlignment(this Justification input)
        {
            switch (input)
            {
                default:
                    return Ro.TextHorizontalAlignment.Left;
                case Justification.Right:
                    return Ro.TextHorizontalAlignment.Right;
                case Justification.Center:
                    return Ro.TextHorizontalAlignment.Center;
            }
        }

        public static Ro.TextVerticalAlignment ToRhVerticalAlignment(this Alignment input)
        {
            switch (input)
            {
                default:
                    return Ro.TextVerticalAlignment.Bottom;
                case Alignment.Top:
                    return Ro.TextVerticalAlignment.Top;
                case Alignment.Center:
                    return Ro.TextVerticalAlignment.Middle;
            }
        }

        public static Rg.Rectangle3d ToRectangle3d(this Rg.BoundingBox input)
        {
            return new Rg.Rectangle3d(new Rg.Plane(input.Corner(true, true, true), Rg.Vector3d.ZAxis), input.Diagonal.X, input.Diagonal.Y);
        }

        public static List<string> ToStringList(this List<GH_String> input)
        {
            List<string> output = new List<string>();
            foreach (GH_String txt in input) output.Add(txt.Value);

            return output;
        }

        public static List<double> ToNumberList(this List<GH_Number> input)
        {
            List<double> output = new List<double>();
            foreach (GH_Number num in input) output.Add(num.Value);

            return output;
        }

    }
}
