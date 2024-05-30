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
    public class Graphic
    {
        #region members

        protected Sd.Color color = Sd.Color.Transparent;
        protected bool hasColor = false;

        protected Sd.Color stroke = Sd.Color.Black;
        protected double weight = 1.0;
        protected List<double> pattern = new List<double>();
        protected bool hasStroke = false;

        #endregion

        #region constructor

        public Graphic()
        {

        }

        public Graphic(Graphic graphic)
        {
            this.color = graphic.color;
            this.hasColor = graphic.hasColor;

            this.weight = graphic.weight;
            this.stroke = graphic.stroke;
            this.pattern = graphic.pattern;
            this.hasStroke = graphic.hasStroke;
        }

        public Graphic(Sd.Color stroke, double weight)
        {
            this.Stroke = stroke;
            this.Weight = weight;
            this.hasColor = false;
        }

        public Graphic(Sd.Color stroke, Sd.Color fillColor)
        {
            this.Stroke = stroke;
            this.Color = fillColor;
        }

        public Graphic(Sd.Color stroke, double weight, List<double> pattern)
        {
            this.Stroke = stroke;
            this.Weight = weight;
            this.Pattern = pattern;
            this.hasColor = false;
        }

        public Graphic(Sd.Color stroke, double weight, List<double> pattern, Sd.Color fillColor)
        {
            this.Stroke = stroke;
            this.Weight = weight;
            this.Pattern = pattern;
            this.Color = fillColor;
        }

        public Graphic(Sd.Color color)
        {
            this.Color = color;

            this.Stroke = Sd.Color.Transparent;
            this.Weight = 0.0;
            this.hasStroke = false;
        }

        public void SetPattern(string pattern)
        {
            List<double> numbers = new List<double>();
            string[] values = pattern.Split(',');

            foreach (string val in values)
            {
                if (double.TryParse(val, out double d)) numbers.Add(d);
            }
            this.pattern = numbers;
        }

        #endregion

        #region properties

        public virtual bool HasPattern
        {
            get { return (this.Pattern.Count > 0); }
        }

        public virtual Sd.Color Color
        {
            get { return color; }
            set
            {
                this.color = value;
                this.hasColor = true;
            }
        }

        public virtual bool HasColor
        {
            get { return hasColor; }
        }

        public virtual Sd.Color Stroke
        {
            get { return stroke; }
            set
            {
                this.stroke = value;
                this.hasStroke = true;
            }
        }

        public virtual double Weight
        {
            get { return weight; }
            set
            {
                this.weight = value;
                this.hasStroke = true;
            }
        }

        public virtual List<double> Pattern
        {
            get { return pattern; }
            set
            {
                this.pattern = value;
                this.hasStroke = true;
            }
        }

        public virtual bool HasStroke
        {
            get { return hasStroke; }
        }

        #endregion
    }

    public static class Graphics
    {
                public static Graphic Outline { get { return new Graphic(Sd.Color.Black,1); } }
        public static Graphic Dotted { get { return new Graphic(Sd.Color.Black, 1, new List<double> { 1, 1 }); } }
        public static Graphic Dashed { get { return new Graphic(Sd.Color.Black, 1, new List<double> { 5, 5 }); } }
        public static Graphic Solid { get { return new Graphic(Sd.Color.Black); } }
    }
}
