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

        public virtual bool IsBold
        {
            get {
                bool isBold = false;
                if (this.Style == FontStyle.Bold)isBold = true;
                if (this.Style == FontStyle.BoldItalic) isBold = true;

                return isBold;
                } 
            }
        

    public virtual bool IsItalic
    {
        get
        {
            bool isItalic = false;
            if (this.Style == FontStyle.Italic) isItalic = true;
            if (this.Style == FontStyle.BoldItalic) isItalic = true;

            return isItalic;
        }
        }

        public virtual bool IsStrikeout
        {
            get
            {
                bool isStrikeout = false;
                if (this.Style == FontStyle.Strikeout) isStrikeout = true;

                return isStrikeout;
            }
        }

        public virtual bool IsUnderlined
        {
            get
            {
                bool isUnderlined = false;
                if (this.Style == FontStyle.Underline) isUnderlined = true;

                return isUnderlined;
            }
        }


        #endregion

        #region methods



        #endregion

        #region overrides



        #endregion
    }
}
