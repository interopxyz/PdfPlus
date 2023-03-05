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
using System.Drawing;

namespace PdfPlus
{
    public class Page
    {
        #region members

        protected Pf.PdfPage baseObject = new Pf.PdfPage();
        protected Pd.XGraphics graph = null;

        public Units Unit = Units.Millimeter;
        public Rg.Plane Frame = Rg.Plane.WorldXY;

        public List<Shape> shapes = new List<Shape>();

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

        public virtual PageOrientation Orientation
        {
            set { 
                if(value!= PageOrientation.Default) this.baseObject.Orientation = value.ToPdf(); 
            }
        }

        public virtual Rg.Rectangle3d ArtBox
        {
            get { return this.baseObject.ArtBox.ToRhino(); }
            set { this.baseObject.ArtBox = value.ToPdfRect(this.Frame); }
        }

        public virtual Rg.Rectangle3d BleedBox
        {
            get { return this.baseObject.BleedBox.ToRhino(); }
            set { this.baseObject.BleedBox = value.ToPdfRect(this.Frame); }
        }

        public virtual Rg.Rectangle3d CropBox
        {
            get { return this.baseObject.CropBox.ToRhino(); }
            set { this.baseObject.CropBox = value.ToPdfRect(this.Frame); }

        }

        public virtual Rg.Rectangle3d TrimBox
        {
            get { return this.baseObject.TrimBox.ToRhino(); }
            set { this.baseObject.TrimBox = value.ToPdfRect(this.Frame); }

        }

        #endregion

        #region methods

        public void AddHyperLink(Rg.Rectangle3d boundary, string hyperlink)
        {
            //this.baseObject.AddDocumentLink(boundary.ToPdfRect(), 1);
            this.baseObject.AddWebLink(boundary.ToPdfRect(this.Frame), hyperlink);
        }

        public void AddFileLink(Rg.Rectangle3d boundary, string filename)
        {
            this.baseObject.AddFileLink(boundary.ToPdfRect(this.Frame), filename);
        }

        public void AddPageLink(Rg.Rectangle3d boundary, int page)
        {
            this.baseObject.AddDocumentLink(boundary.ToPdfRect(this.Frame), page);
        }

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
                shape.Render(graph,this);
            }
            graph.Dispose();
        }

        public bool AddShape(IGH_Goo goo)
        {
            Shape shape = null;
            bool isValid = true;
            if(goo is GH_Rectangle rect)
            {
                shape = new Shape(rect.Value, new Graphic());
                isValid = rect.IsValid;
            }
            else
            {
                isValid = goo.TryGetShape(ref shape);
            }
            
            shape.AlignContent(this);
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
