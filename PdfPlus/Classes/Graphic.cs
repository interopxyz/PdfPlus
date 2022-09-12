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

        public Sd.Color Color = Sd.Color.Transparent;
        public double Weight = 1.0;
        public Sd.Color Stroke = Sd.Color.Black;
        public List<double> Pattern = new List<double>();

        #endregion

        #region constructor

        public Graphic()
        {

        }

        public Graphic(Graphic graphic)
        {
            this.Color = graphic.Color;
            this.Weight = graphic.Weight;
            this.Stroke = graphic.Stroke;

            this.Pattern = graphic.Pattern;
        }

        public Graphic(Sd.Color stroke, double weight)
        {
            this.Stroke = stroke;
            this.Weight = weight;

            this.Color = Sd.Color.Transparent;
        }

        public Graphic(Sd.Color color)
        {
            this.Color = color;

            this.Stroke = Sd.Color.Transparent;
            this.Weight = 0.0;
        }

        #endregion

        #region properties

        public virtual bool HasPattern
        {
            get { return (this.Pattern.Count > 0); }
        }

        #endregion
    }
}
