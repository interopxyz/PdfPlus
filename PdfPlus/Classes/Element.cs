using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfPlus
{
    public class Element
    {

        #region members

        public enum ElementTypes { Empty, Shape, Block, Data}
        protected ElementTypes elementType = ElementTypes.Empty;
        protected Graphic graphic = new Graphic();
        protected Font font = new Font();

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
        }

        #endregion


        #region properties

        public virtual Graphic Graphic
        {
            get { return new Graphic(graphic); }
            set { this.graphic = new Graphic(value); }
        }

        public virtual Font Font
        {
            get { return new Font(font); }
            set { this.font = new Font(value); }
        }

        #endregion
    }
}
