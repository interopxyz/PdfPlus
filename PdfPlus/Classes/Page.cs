using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;
using Pf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;

using Md = MigraDoc.DocumentObjectModel;
using Mr = MigraDoc.Rendering;

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

        protected List<Shape> shapes = new List<Shape>();
        protected List<Block> blocks = new List<Block>();

        #endregion

        #region constructors

        public Page()
        {
            this.baseObject = new Pf.PdfPage();
        }

        public Page(Pf.PdfPage page)
        {
            this.baseObject = (Pf.PdfPage)page.Clone();
        }

        public Page(Pf.PdfPage page, Page reference)
        {
            this.baseObject = (Pf.PdfPage)page.Clone();

            foreach (Shape shape in reference.shapes) this.shapes.Add(new Shape(shape));
            this.Unit = reference.Unit;
            this.Frame = new Rg.Plane(reference.Frame);
            this.graph = reference.graph;
        }

        public Page(Page page)
        {
            if (page != null)
            {
                foreach (Shape shape in page.shapes) this.shapes.Add(new Shape(shape));
                foreach (Block block in page.blocks) this.blocks.Add(new Block(block));

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
            //this.SetDefaultPages();
        }

        public Page(SizesA size, PageOrientation orientation = PageOrientation.Default)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Millimeter;
            this.Orientation = orientation;
            //this.SetDefaultPages();
        }

        public Page(SizesB size, PageOrientation orientation = PageOrientation.Default)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Millimeter;
            this.Orientation = orientation;
            //this.SetDefaultPages();
        }

        public Page(SizesImperial size, PageOrientation orientation = PageOrientation.Default)
        {
            this.baseObject.Size = size.ToSharp();
            this.Unit = Units.Inch;
            this.Orientation = orientation;
            //this.SetDefaultPages();
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

        public List<Block> Blocks
        {
            get
            {
                List<Block> output = new List<Block>();
                foreach (Block block in blocks)
                {
                    Block blk = new Block(block);
                    output.Add(blk);
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
            get { return this.baseObject.Orientation.ToPlus(); }
            set { this.baseObject.Orientation = value.ToPdf(); }
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
            document = this.RenderBlocks(document);
            if (shapes.Count > 0) { 
            document.AddPage(this.baseObject);
            this.Render(this.baseObject);
            }
            return document;
        }

        public Md.Document SetPage(Md.Document document)
        {
            switch (this.baseObject.Size)
            {
                default:
                    document.LastSection.PageSetup.PageWidth = this.baseObject.Width.ToMigraDoc();
                    document.LastSection.PageSetup.PageHeight = this.baseObject.Height.ToMigraDoc();
                    break;
                case PdfSharp.PageSize.A0:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.A0;
                    break;
                case PdfSharp.PageSize.A1:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.A1;
                    break;
                case PdfSharp.PageSize.A2:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.A2;
                    break;
                case PdfSharp.PageSize.A3:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.A3;
                    break;
                case PdfSharp.PageSize.A4:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.A4;
                    break;
                case PdfSharp.PageSize.A5:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.A5;
                    break;
                case PdfSharp.PageSize.Letter:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.Letter;
                    break;
                case PdfSharp.PageSize.Ledger:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.Ledger;
                    break;
                case PdfSharp.PageSize.Legal:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.Legal;
                    break;
                case PdfSharp.PageSize.Tabloid:
                    document.LastSection.PageSetup.PageFormat = Md.PageFormat.P11x17;
                    break;
            }

            document.LastSection.PageSetup.Orientation = this.baseObject.Orientation.ToMigraDoc();

            return document;
        }

        public Pf.PdfDocument RenderBlocks(Pf.PdfDocument document)
        {

            var clone = (Pf.PdfPage)this.baseObject.Clone();

            Md.Document doc = new Md.Document();
            Md.Section section = doc.Sections.AddSection();
            doc = this.SetPage(doc);

            Md.Style style = doc.Styles["Heading1"];
            style.Font.Name = "Times New Roman";
            style.Font.Size = 16;
            style.Font.Color = Md.Color.FromArgb(255,255,0,0);
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.KeepWithNext = true;

            foreach (Block block in blocks) block.Render(doc);

            Mr.PdfDocumentRenderer pdfDocumentRenderer = new Mr.PdfDocumentRenderer();
            pdfDocumentRenderer.Document = doc;
            pdfDocumentRenderer.PdfDocument = document;

            pdfDocumentRenderer.RenderDocument();

            return pdfDocumentRenderer.PdfDocument;
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

        public bool AddBlock(IGH_Goo goo)
        {
            Block block = null;
            bool isValid = goo.TryGetBlock(ref block);
            this.blocks.Add(block);

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
