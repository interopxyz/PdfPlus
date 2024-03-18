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

        protected bool isModified = false;

        protected string family = "Arial";
        protected bool hasFamily = false;
        protected double size = 12.0;
        protected bool hasSize = false;
        protected Sd.Color color = Sd.Color.Black;
        protected bool hasColor = false;
        protected FontStyle style = FontStyle.Regular;
        protected bool hasStyle = false;
        protected Justification justification = Justification.Left;
        protected bool hasJustification = false;

        #endregion

        #region constructors

        public Font()
        {

        }

        public Font(Font font)
        {
            this.isModified = font.isModified;

            this.family = font.family;
            this.hasFamily = font.hasFamily;
            this.size = font.size;
            this.hasSize = font.hasSize;
            this.color = font.color;
            this.hasColor = font.hasColor;
            this.justification = font.justification;
            this.hasJustification = font.hasJustification;

            this.style = font.style;
            this.hasStyle = font.hasStyle;
        }

        public Font(string family, double size, Sd.Color color)
        {
            this.isModified = true;

            this.family = family;
            this.hasFamily = true;
            this.size = size;
            this.hasSize = true;
            this.color = color;
            this.hasColor = true;
        }


        #endregion

        #region properties

        public virtual bool IsModified { get { return this.isModified; } }
        public virtual bool HasFamily { get { return this.hasFamily; } }
        public virtual bool HasSize { get { return this.hasSize; } }
        public virtual bool HasColor { get { return this.hasColor; } }
        public virtual bool HasStyle { get { return this.hasStyle; } }
        public virtual bool HasJustification { get { return this.hasJustification; } }

        public virtual string Family
        {
            get { return this.family; }
            set
            {
                this.isModified = true;
                this.hasFamily = true;
                this.family = value;
            }
        }

        public virtual double Size
        {
            get { return this.size; }
            set
            {
                this.isModified = true;
                this.hasSize = true;
                this.size = value;
            }
        }

        public virtual Sd.Color Color
        {
            get { return this.color; }
            set
            {
                this.isModified = true;
                this.hasColor = true;
                this.color = value;
            }
        }

        public virtual FontStyle Style
        {
            get { return this.style; }
            set
            {
                this.isModified = true;
                this.hasStyle = true;
                this.style = value;
            }
        }

        public virtual Justification Justification
        {
            get { return this.justification; }
            set
            {
                this.isModified = true;
                this.hasJustification = true;
                this.justification = value;
            }
        }

        public virtual bool IsBold
        {
            get
            {
                bool isBold = false;
                if (this.style == FontStyle.Bold) isBold = true;
                if (this.style == FontStyle.BoldItalic) isBold = true;

                return isBold;
            }
        }


        public virtual bool IsItalic
        {
            get
            {
                bool isItalic = false;
                if (this.style == FontStyle.Italic) isItalic = true;
                if (this.style == FontStyle.BoldItalic) isItalic = true;

                return isItalic;
            }
        }

        public virtual bool IsStrikeout
        {
            get
            {
                bool isStrikeout = false;
                if (this.style == FontStyle.Strikeout) isStrikeout = true;

                return isStrikeout;
            }
        }

        public virtual bool IsUnderlined
        {
            get
            {
                bool isUnderlined = false;
                if (this.style == FontStyle.Underline) isUnderlined = true;

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
