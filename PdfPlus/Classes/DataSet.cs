using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

namespace PdfPlus
{
    public class DataSet
    {
        #region members

        protected List<double> values = new List<double>();
        protected List<Sd.Color> colors = new List<Sd.Color>();
        public bool LabelData = true;
        protected string title = string.Empty;
        protected bool hasTitle = false;
        protected bool hasGraphics = false;
        public Graphic Graphic = new Graphic();
        public Font Font = new Font();

        #endregion

        #region constructors

        public DataSet(List<double> values)
        {
            this.values = values;
            this.Graphic.Stroke = Sd.Color.Black;
            this.Graphic.Weight = 1;
        }
        public DataSet(List<double> values, List<string> labels)
        {
            this.values = values;
            this.Graphic.Stroke = Sd.Color.Black;
            this.Graphic.Weight = 1;
        }

        public DataSet(DataSet dataSet)
        {
            this.title = dataSet.title;
            this.hasTitle = dataSet.hasTitle;
            this.Graphic = new Graphic(dataSet.Graphic);
            this.Font = new Font(dataSet.Font);
            this.LabelData = dataSet.LabelData;
            foreach (double v in dataSet.values) this.values.Add(v);
            foreach (Sd.Color c in dataSet.colors) this.colors.Add(c);
        }

        #endregion

        #region properties

        public virtual List<double> Values
        {
            get { return this.values; }
        }

        public virtual List<Sd.Color> Colors
        {
            get 
            {
                List<Sd.Color> output = colors;
                output.AddRange(Enumerable.Repeat(Sd.Color.Black,this.values.Count-colors.Count));
                return output; 
            }
            set
            {
                this.colors = value;
                int c = this.colors.Count;
                for(int i = c; i < this.values.Count; i++)
                {
                    this.colors.Add(this.colors[c - 1]);
                }
            }
        }

        public virtual string Title
        {
            get { return title; }
            set
            {
                title = value;
                hasTitle = true;
            }
        }

        #endregion

        #region methods



        #endregion

        #region overrides



        #endregion

    }
}
