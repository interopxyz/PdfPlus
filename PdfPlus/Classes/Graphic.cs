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
    /// <summary>
    /// The graphic class is a container of all the graphics settings
    /// </summary>
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

        /// <summary>
        /// The graphic class is a container of all the graphics settings
        /// </summary>
        public Graphic()
        {

        }

        /// <summary>
        /// Copy a graphic settings
        /// </summary>
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
        }

        public Graphic(Sd.Color color)
        {
            this.Color = color;

            this.stroke = Sd.Color.Transparent;
            this.weight = 0.0;
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
}
