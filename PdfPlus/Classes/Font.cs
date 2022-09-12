using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;
using pdf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;
using Rg = Rhino.Geometry;

namespace PdfPlus
{
    public class Font
    {
        #region members

        public string Family = "Arial";
        public double Size = 12.0;
        public Sd.Color Color = Sd.Color.Black;
        public FontStyle Style = FontStyle.Regular;

        #endregion

        #region constructors

        public Font()
        {

        }

        public Font(Font font)
        {
            this.Family = font.Family;
            this.Size = font.Size;
            this.Color = font.Color;

            this.Style = font.Style;
        }

        public Font(string family, double size, Sd.Color color)
        {
            this.Family = family;
            this.Size = size;
            this.Color = color;
        }

        #endregion

        #region properties



        #endregion

        #region methods



        #endregion

        #region overrides



        #endregion
    }
}
