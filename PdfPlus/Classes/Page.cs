using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;
using Pf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;
using Rg = Rhino.Geometry;
using Grasshopper.Kernel.Types;

namespace PdfPlus
{
    public class Page
    {
        #region members

        protected Pf.PdfPage baseObject = new Pf.PdfPage();
        protected Pd.XGraphics graph = null;
        public Units Unit = Units.Millimeter;
        public Rg.Plane Frame = Rg.Plane.WorldXY;

        protected List<Shape> shapes = new List<Shape>();

        #endregion

        #region constructors

        public Page()
        {
            this.baseObject = new Pf.PdfPage();
        }

        public Page(Page page)
        {
            foreach(Shape shape in page.shapes)
            {
                this.shapes.Add(new Shape(shape));
            }
            this.Unit = page.Unit;

            this.Frame = new Rg.Plane(page.Frame);

            this.graph = page.graph;
            this.baseObject = (Pf.PdfPage)page.baseObject.Clone();
        }

        public Page(Units unit, double width, double height)
        {
            this.Unit = unit;
            this.Width = width;
            this.Height = height;
            
        }

        public Page(SizesA size)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Millimeter;
        }

        public Page(SizesB size)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Millimeter;
        }

        public Page(SizesImperial size)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Inch;
        }

        #endregion

        #region properties

        public virtual double Width
        {
            get { return this.baseObject.Width.GetValue(Unit); }
            set { this.baseObject.Width = Unit.SetValue(value); }
        }

        public virtual double Height
        {
            get { return this.baseObject.Height.GetValue(Unit); }
            set { this.baseObject.Height = Unit.SetValue(value); }
        }

        public virtual Pf.PdfPage BaseObject
        {
            get { return (Pf.PdfPage)baseObject.Clone(); }
        }

        public virtual Rg.Rectangle3d Boundary
        {
            get { return new Rg.Rectangle3d(this.Frame, this.baseObject.Width.Point, this.baseObject.Height.Point); }
        }

        #endregion

        #region methods

        public Pf.PdfDocument AddToDocument(Pf.PdfDocument document)
        {
            this.baseObject = document.AddPage(this.BaseObject);
            this.Render();
            return document;
        }

        public void Render()
        {
            graph = Pd.XGraphics.FromPdfPage(this.baseObject);
            foreach (Shape shape in shapes)
            {
                shape.Render(graph);
            }
            graph.Dispose();
        }

        public bool AddShape(IGH_Goo goo)
        {
            Shape shape = null;
            bool isValid = goo.TryGetShape(ref shape);
            shape.AlignContent(this);
            this.shapes.Add(shape);

            return isValid;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return "Page " + Math.Round(this.Width,2)+ Unit.Abbreviation() + "," + Math.Round(this.Height,2) + Unit.Abbreviation();
        }

        #endregion
    }
}
