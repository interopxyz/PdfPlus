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

        public static List<string> ToStringList(this List<GH_String> input)
        {
            List<string> output = new List<string>();
            foreach (GH_String txt in input) output.Add(txt.Value);

            return output;
        }

    }
}
