using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

namespace PdfPlus
{
    public class Element
    {

        #region members

        public enum ElementTypes { Empty, Shape, Block, Data}
        protected ElementTypes elementType = ElementTypes.Empty;

        //Formatting
        protected Graphic graphic = new Graphic();
        protected Font font = new Font();

        //Text
        protected string text = string.Empty;

        //Chart
        public enum ChartTypes { Bar, BarStacked, Column, ColumnStacked, Line, Area, Pie };
        protected ChartTypes chartType = ChartTypes.ColumnStacked;
        protected List<DataSet> data = new List<DataSet>();
        protected string xAxis = string.Empty;
        protected string yAxis = string.Empty;

        //Images
        protected string imageName = string.Empty;
        protected Sd.Bitmap imageObject = null;

        #endregion

        #region constructors

        public Element()
        {

        }

        public Element(Element element)
        {
            this.elementType = element.elementType;

            this.graphic = new Graphic(element.graphic);
            this.font = new Font(element.font);

            //Text
            this.text = element.text;

            //Chart

            this.chartType = element.chartType;
            SetData(element.data);
            this.xAxis = element.xAxis;
            this.yAxis = element.yAxis;

            //Image
            this.imageName = element.imageName;
            if (element.imageObject != null) this.imageObject = new Sd.Bitmap(element.imageObject);
        }

        #endregion

        #region properties

        public virtual ElementTypes ElementType { get { return elementType; } }

        public virtual Graphic Graphic
        {
            get { return new Graphic(graphic); }
            set { this.graphic = new Graphic(value); }
        }

        public virtual Font Font
        {
            get { return new Font(font); }
            set { this.font = value; }
        }

        #endregion

        #region methods

        public void SetData(List<DataSet> data)
        {
            foreach (DataSet d in data)
            {
                this.data.Add(new DataSet(d));
            }
        }

        #endregion

    }
}
