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
            if (page != null)
            {
                foreach (Shape shape in page.shapes)
                {
                    this.shapes.Add(new Shape(shape));
                }

                this.Unit = page.Unit;
                this.Frame = new Rg.Plane(page.Frame);
                this.graph = page.graph;
                this.baseObject = (Pf.PdfPage)page.baseObject.Clone();
            }
        }

        public Page(Units unit, double width, double height)
        {
            this.Unit = unit;
            this.Width = width;
            this.Height = height;
            this.SetDefaultPages();
        }

        public Page(SizesA size)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Millimeter;
            this.SetDefaultPages();
        }

        public Page(SizesB size)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Millimeter;
            this.SetDefaultPages();
        }

        public Page(SizesImperial size)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Inch;
            this.SetDefaultPages();
        }

        public List<Shape> Shapes
        {
            get
            {
                List<Shape> output = new List<Shape>();
                foreach (Shape shape in shapes)
                {
                    Shape shp = new Shape(shape);
                    shp.AlignContent(this,false);
                    output.Add(shp);
                }
                return output;
            }
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

        public virtual PageOrientation Orientation
        {
            set { 
                if(value!= PageOrientation.Default) this.baseObject.Orientation = value.ToPdf(); 
            }
        }

        public virtual Rg.Rectangle3d ArtBox
        {
            get { return this.baseObject.ArtBox.ToRhino(this.Frame); }
            set { this.baseObject.ArtBox = value.ToPdfRect(this.Frame); }
        }

        public virtual Rg.Rectangle3d BleedBox
        {
            get { return this.baseObject.BleedBox.ToRhino(this.Frame); }
            set { this.baseObject.BleedBox = value.ToPdfRect(this.Frame); }
        }

        public virtual Rg.Rectangle3d CropBox
        {
            get { return this.baseObject.CropBox.ToRhino(this.Frame); }
            set { this.baseObject.CropBox = value.ToPdfRect(this.Frame); }

        }

        public virtual Rg.Rectangle3d TrimBox
        {
            get { return this.baseObject.TrimBox.ToRhino(this.Frame); }
            set { this.baseObject.TrimBox = value.ToPdfRect(this.Frame); }

        }

        #endregion

        #region methods

        private void SetDefaultPages()
        {
            this.baseObject.MediaBox = this.Boundary.Inflate(0).ToPdfRect(this.Frame);//+1/2"
            this.baseObject.BleedBox = this.Boundary.Inflate(-10).ToPdfRect(this.Frame);//+1/4"XX
            this.baseObject.TrimBox = this.Boundary.Inflate(-5).ToPdfRect(this.Frame);//+0"XX
            this.baseObject.CropBox = this.Boundary.Inflate(0).ToPdfRect(this.Frame);//XX
            this.baseObject.ArtBox = this.Boundary.Inflate(-20).ToPdfRect(this.Frame);//-1/4"XX
        }

        public Pf.PdfDocument AddToDocument(Pf.PdfDocument document)
        {
            var clone = (Pf.PdfPage)this.baseObject.Clone();
            document.AddPage(clone);
            this.Render(clone);
            return document;
        }

        protected void Render(Pf.PdfPage page)
        {
            graph = Pd.XGraphics.FromPdfPage(page);
            foreach (Shape shp in shapes)
            {
                Shape shape = new Shape(shp);
                shape.AlignContent(this);
                shape.Render(graph, page, this.Frame);
            }
            graph.Dispose();
        }

        public bool AddShape(IGH_Goo goo)
        {
            Shape shape = null;
            bool isValid = goo.TryGetShape(ref shape);
            shape.AlignContent(this,false);
            this.shapes.Add(shape);

            return isValid;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return "Page " + Math.Round(this.Width, 2) + Unit.Abbreviation() + "," + Math.Round(this.Height, 2) + Unit.Abbreviation();
        }

        #endregion
    }
}
