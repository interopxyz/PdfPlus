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
        public enum Presets { Normal, Title, Subtitle, Heading1, Heading2, Heading3, Heading4, Heading5, Heading6, Quote, Footnote, Caption };

        protected bool isModified = false;

        protected string name = "None";

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

            this.name = font.name;

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

        public Font(string name, string family, double size, Sd.Color color, FontStyle style = FontStyle.Regular)
        {
            this.isModified = true;

            this.name = name;

            this.family = family;
            this.hasFamily = true;
            this.size = size;
            this.hasSize = true;
            this.color = color;
            this.hasColor = true;
            this.style = style;
            this.hasStyle = true;
        }

        public Font(string family, double size, Sd.Color color, FontStyle style = FontStyle.Regular)
        {
            this.isModified = true;

            this.family = family;
            this.hasFamily = true;
            this.size = size;
            this.hasSize = true;
            this.color = color;
            this.hasColor = true;
            this.style = style;
            this.hasStyle = true;
        }

        #endregion

        #region properties

        public virtual bool IsModified { get { return this.isModified; } }
        public virtual bool HasFamily { get { return this.hasFamily; } }
        public virtual bool HasSize { get { return this.hasSize; } }
        public virtual bool HasColor { get { return this.hasColor; } }
        public virtual bool HasStyle { get { return this.hasStyle; } }
        public virtual bool HasJustification { get { return this.hasJustification; } }

        public virtual string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

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

    public static class Fonts
    {
        public static Font Normal { get { return new Font("Normal", "Arial", 11, Sd.Color.Black); } }
        public static Font Title { get { return new Font("Title", "Arial", 26, Sd.Color.Black); } }
        public static Font Subtitle { get { return new Font("Subtitle", "Arial", 15, Sd.Color.DarkGray); } }
        public static Font Heading1 { get { return new Font("Heading1", "Arial", 20, Sd.Color.Black); } }
        public static Font Heading2 { get { return new Font("Heading2", "Arial", 18, Sd.Color.Black); } }
        public static Font Heading3 { get { return new Font("Heading3", "Arial", 14, Sd.Color.Gray); } }
        public static Font Heading4 { get { return new Font("Heading4", "Arial", 12, Sd.Color.Gray); } }
        public static Font Heading5 { get { return new Font("Heading5", "Arial", 11, Sd.Color.Gray); } }
        public static Font Heading6 { get { return new Font("Heading6", "Arial", 11, Sd.Color.Gray, FontStyle.Italic); } }
        public static Font Quote { get { return new Font("Quote", "Arial", 11, Sd.Color.DarkGray); } }
        public static Font Footnote { get { return new Font("Footnote", "Arial", 8, Sd.Color.Black, FontStyle.Italic); } }
        public static Font Caption { get { return new Font("Caption", "Arial", 8, Sd.Color.Black); } }
        public static Font List { get { return new Font("List", "Arial", 11, Sd.Color.Black); } }

    }
}
