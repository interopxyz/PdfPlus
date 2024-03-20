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
    public static class GhExtensions
    {

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
