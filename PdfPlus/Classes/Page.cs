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

namespace PdfPlus
{
    public class Page
    {

        #region members

        public string id = Guid.NewGuid().ToString();
        public int Index = 0;
        public int Total = 0;

        protected Pf.PdfPage baseObject = new Pf.PdfPage();
        protected Pd.XGraphics graph = null;

        public Units Unit = Units.Millimeter;
        public Rg.Plane Frame = Rg.Plane.WorldXY;

        protected List<Shape> shapes = new List<Shape>();
        protected List<Block> blocks = new List<Block>();

        protected double left = 72;
        protected double right = 72;
        protected double top = 72;
        protected double bottom = 72;

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
            this.id = reference.id;
            this.Index = reference.Index;
            this.Total= reference.Total;
        }

        public Page(Page page, bool copyBase = true)
        {
            if (page != null)
            {
                foreach (Shape shape in page.shapes) this.shapes.Add(new Shape(shape));
                foreach (Block block in page.blocks) this.blocks.Add(new Block(block));

                this.Unit = page.Unit;
                this.Frame = new Rg.Plane(page.Frame);
                if (copyBase) this.graph = page.graph;
                this.baseObject = (Pf.PdfPage)page.baseObject.Clone();
                this.id = page.id;
                this.Index = page.Index;
                this.Total = page.Total;
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
                    shp.AlignContent(this, false);
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

        public virtual double PointWidth
        {
            get { return this.baseObject.Width.Point; }
        }

        public virtual double PointHeight
        {
            get { return this.baseObject.Height.Point; }
        }

        public virtual double PointMarginLeft
        {
            get { return this.left; }
        }

        public virtual double PointMarginRight
        {
            get { return this.right; }
        }

        public virtual double PointMarginTop
        {
            get { return this.top; }
        }

        public virtual double PointMarginBottom
        {
            get { return this.bottom; }
        }

        public virtual Pf.PdfPage BaseObject
        {
            get { return (Pf.PdfPage)baseObject.Clone(); }
        }

        public virtual Rg.Rectangle3d Boundary
        {
            get { return new Rg.Rectangle3d(this.Frame, this.baseObject.Width.Point, this.baseObject.Height.Point); }
        }

        public virtual Rg.Point3d Center
        {
            get { return this.Frame.PointAt(this.baseObject.Width / 2.0, this.baseObject.Height / 2.0); }
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

        public Md.Document SetPage(Md.Document document)
        {
            document.LastSection.PageSetup.Orientation = this.baseObject.Orientation.ToMigraDoc();
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
            if (this.baseObject.Orientation == PdfSharp.PageOrientation.Landscape)
            {
                document.LastSection.PageSetup.PageWidth = this.PointHeight;
                document.LastSection.PageSetup.PageHeight = this.PointWidth;
            }
            else
            {
                document.LastSection.PageSetup.PageWidth = this.PointWidth;
                document.LastSection.PageSetup.PageHeight = this.PointHeight;
            }
            document.LastSection.PageSetup.LeftMargin = this.PointMarginLeft;
            document.LastSection.PageSetup.RightMargin = this.PointMarginRight;
            document.LastSection.PageSetup.TopMargin = this.PointMarginTop;
            document.LastSection.PageSetup.BottomMargin = this.PointMarginBottom;

            return document;
        }

        public List<string> SubText(Block block, string text, double width, double height, ref int index)
        {
            int textLineIndex = index;
            List<string> textLines = block.BreakLines(text, width + 12);
            double lineHeight = block.FontSize * 1.5 * (72.0 / 96.0);
            int lineCount = (int)Math.Floor(height / lineHeight);
            List<string> textContents = textLines;
            if ((textLineIndex + lineCount) > textLines.Count) lineCount = textLines.Count - textLineIndex;
            textContents = textLines.GetRange(textLineIndex, lineCount);
            textLineIndex += lineCount;
            index = textLineIndex;

            return textContents;
        }

        #region Previews

        public List<Page> RenderBlocksToPages()
        {
            List<Page> pages = new List<Page>();
            if (this.blocks.Count > 0)
            {

                Md.Document doc = Document.DefaultDocument();
                doc.Sections.AddSection();
                doc = this.SetPage(doc);

                foreach (Block block in blocks) block.RenderToDocument(doc);

                Pf.PdfDocument document = new Pf.PdfDocument();
                Mr.PdfDocumentRenderer pdfDocumentRenderer = new Mr.PdfDocumentRenderer(true);
                pdfDocumentRenderer.Document = doc;
                pdfDocumentRenderer.PdfDocument = document;

                pdfDocumentRenderer.PrepareRenderPages();
                int count = pdfDocumentRenderer.PageCount;
                string lastTag = "";
                int tableRowIndex = 0;
                int textLineIndex = 0;

                double spacing = 0;
                string newId= Guid.NewGuid().ToString();
                for (int i = 0; i < count; i++)
                {
                int listIndex = 0;
                    pdfDocumentRenderer.RenderPages(i + 1, i + 1);
                    Mr.RenderInfo[] infos = pdfDocumentRenderer.DocumentRenderer.GetRenderInfoFromPage(i + 1);
                    Pd.XGraphics graph = Pd.XGraphics.FromPdfPage(pdfDocumentRenderer.PdfDocument.Pages[i]);

                    double pageHeight = pdfDocumentRenderer.PdfDocument.Pages[i].Height;

                    Page newPage = new Page(this.baseObject);
                    newPage.Frame = this.Frame;
                    newPage.Frame.OriginY += spacing;
                    newPage.id = newId;
                    newPage.Index = i;
                    newPage.Total = (count - 1);

                    spacing -= (pageHeight + 10);

                    foreach (Mr.RenderInfo info in infos)
                    {

                        Rg.Rectangle3d gpRect = info.LayoutInfo.ContentArea.ToRectangle3d(newPage.Frame);
                        Rg.Plane f = Rg.Plane.WorldXY;

                        Rg.Plane p = Rg.Plane.WorldZX;
                        p.Origin = newPage.Center;
                        gpRect.Transform(Rg.Transform.Mirror(p));
                        f.Origin = gpRect.BoundingBox.Corner(true, true, true);

                        //Origin Plane
                        Rg.Plane originPlane = new Rg.Plane(f);
                        double x = f.OriginX;
                        double y = f.OriginY;
                        double w = gpRect.Width;
                        double h = gpRect.Height;

                        //Base Frame
                        Rg.Rectangle3d boundary = new Rg.Rectangle3d(originPlane, w, h);

                        if (info.DocumentObject.Tag != null)
                        {
                            string[] tag = info.DocumentObject.Tag.ToString().Split('~');
                            Block block = this.blocks.Find(b => b.id == tag[1]);
                            block.Font.Justification = Justification.Center;

                            if (tag[1] != lastTag)
                            {
                                tableRowIndex = 0;
                                textLineIndex = 0;
                                listIndex = 0;
                                lastTag = tag[1];
                            }

                            switch (tag[0])
                            {
                                case "DockH":
                                    int j = 0;
                                    foreach(Block blk in block.Blocks)
                                    {
                                        tableRowIndex = 0;
                                        textLineIndex = 0;
                                        listIndex = 0;
                                        double dockwidth = w / block.Blocks.Count;
                                        Rg.Plane pln = new Rg.Plane(originPlane);
                                        pln.OriginX += (dockwidth) *j;
                                        Rg.Rectangle3d bnd = new Rg.Rectangle3d(pln, dockwidth, h);
                                        switch (blk.BlockType)
                                        {
                                            case Block.BlockTypes.Chart:
                                                newPage.AddShape(this.PreviewChart(blk, bnd));
                                                break;
                                            case Block.BlockTypes.Drawing:
                                                newPage.AddShapes(this.PreviewDrawing(blk, bnd));
                                                break;
                                            case Block.BlockTypes.Image:
                                                newPage.AddShape(this.PreviewImage(blk, bnd));
                                                break;
                                            case Block.BlockTypes.Text:
                                                newPage.AddShape(this.PreviewText(this.SubText(blk, blk.Text, dockwidth, h, ref textLineIndex), blk, bnd));
                                                break;
                                            case Block.BlockTypes.List:
                                                textLineIndex = 0;
                                                newPage.AddShape(this.PreviewText(this.SubText(blk, block.ListType.ToUnicode(listIndex) + blk.Fragments[listIndex].FullText, dockwidth, h, ref textLineIndex), blk, bnd));
                                                listIndex += 1;
                                                break;
                                        }
                                        j++;
                                    }
                                    break;
                                case "Table":
                                    newPage.AddShapes(this.PreviewTable(block, boundary, info, ref tableRowIndex));
                                    break;
                                case "Drawing":
                                    newPage.AddShapes(this.PreviewDrawing(block,gpRect));
                                    break;
                                case "Chart":
                                    newPage.AddShape(this.PreviewChart(block, boundary));
                                    break;
                                case "Image":
                                    newPage.AddShape(this.PreviewImage(block,boundary));
                                    break;
                                case "List":
                                    textLineIndex = 0;
                                    newPage.AddShape(this.PreviewText(this.SubText(block, block.ListType.ToUnicode(listIndex) + block.Fragments[listIndex].FullText, w, h, ref textLineIndex), block, boundary));
                                    listIndex += 1;
                                    break;
                                case "Text":
                                    newPage.AddShape(this.PreviewText(this.SubText(block,block.Text,w,h,ref textLineIndex), block, boundary));
                                    break;
                            }
                            Shape prevTxt = Shape.CreatePreview(block.BlockType.ToString(), new Rg.Point3d(x-1, y, 0), 90);
                            prevTxt.Angle = 90;
                        newPage.AddShape(prevTxt);
                        }

                        newPage.AddShape(Shape.CreatePreview(boundary));
                    }
                    newPage.AddBlocks(this.blocks);
                    pages.Add(newPage);
                }

            }

            return pages;
        }

        public Shape PreviewText(List<string> lines, Block block, Rg.Rectangle3d boundary)
        {
            Shape shape = Shape.CreateText( string.Join("",lines), boundary, block.Alignment, block.Font);
            shape.Renderable = false;
            return shape;
        }

        public List<Shape> PreviewTable(Block block, Rg.Rectangle3d boundary, Mr.RenderInfo info, ref int index)
        {
            Rg.Plane frame = new Rg.Plane(boundary.Plane);
            int tableRowIndex = index;
            double widght = boundary.Width;
            double height = boundary.Height;
            List<Shape> output = new List<Shape>();
            frame.OriginY += boundary.Height;
            Md.Tables.Table table = (Md.Tables.Table)info.DocumentObject;
            double totalHeight = 0;
            double cw = 0;
            for (int c = 1; c < table.Columns.Count; c++)
            {
                cw += table.Columns[c].Width.Point;
                output.Add(Shape.CreateGeometry(new Rg.Line(frame.Origin + new Rg.Point3d(cw, 0, 0), frame.YAxis, -boundary.Height), new Graphic(Sd.Color.Gray, 1)));

            }
            for (int r = tableRowIndex; r < table.Rows.Count; r++)
            {
                double rh = block.Font.Size + 14;
                frame.OriginY -= rh;
                totalHeight += rh;
                if (totalHeight >= (height + 1)) break;
                tableRowIndex += 1;
                frame.OriginX = boundary.Plane.OriginX;

                output.Add(Shape.CreateGeometry(new Rg.Line(frame.Origin, frame.XAxis, widght), new Graphic(Sd.Color.Gray, 1)));

                for (int c = 0; c < table.Columns.Count; c++)
                {
                    cw = table.Columns[c].Width.Point;

                    Shape tableContent = Shape.CreateText(block.Data[c].Contents[r], frame.Origin + new Rg.Point3d(cw / 2.0, rh / 2.0, 0), block.Font);
                    tableContent.Alignment = Location.Middle;
                    output.Add(tableContent);
                    frame.OriginX += cw;
                }
            }
            output.Add(Shape.CreateGeometry(boundary, new Graphic(Sd.Color.Gray, 1)));

            for (int i = 0; i < output.Count; i++) output[i].Renderable = false;

            index = tableRowIndex;
            return output;
        }

        public Shape PreviewChart(Block block, Rg.Rectangle3d boundary)
        {
            Shape shape = new Shape(block);
            shape.SetBoundary(boundary);
            shape.Renderable = false;
            return shape;
        }

        public Shape PreviewImage(Block block, Rg.Rectangle3d boundary)
        {
            Shape shape = Shape.CreateImage(block.Image, boundary, block.ImagePath);
            shape.Renderable = false;
            return shape;
        }

        public List<Shape> PreviewDrawing(Block block, Rg.Rectangle3d boundary)
        {
            List<Shape> output = block.Drawing.ResizeDrawing(boundary, false);
            for (int i = 0; i < output.Count; i++) output[i].Renderable = false;
            return output;
        }

        #endregion

        public Pf.PdfDocument AddToDocument(Pf.PdfDocument document)
        {
            bool newPage = false;
            if (this.Index == 0)
            {
                document = this.RenderBlocks(document);
                if (this.blocks.Count < 1) newPage = true;
            }
            if (shapes.Count > 0)
            {
                if(newPage)document.AddPage(this.baseObject);
                this.Render(document.Pages[document.PageCount-1-(this.Total-this.Index)]);
            }
            return document;
        }

        public Pf.PdfDocument RenderBlocks(Pf.PdfDocument document)
        {
            if (this.blocks.Count > 0)
            {
                Md.Document doc = Document.DefaultDocument();
                doc.Sections.AddSection();
                doc = this.SetPage(doc);

                Dictionary<string,Block> drawings = new Dictionary<string, Block>();
                List<Block> testBlocks = new List<Block>();
                foreach (Block block in this.blocks)
                {
                    block.RenderToDocument(doc);
                    testBlocks.Add(new Block(block));
                }

                while(testBlocks.Count>0) 
                {
                    Block block = testBlocks[0];
                    if (block.BlockType == Block.BlockTypes.Drawing) if (!drawings.ContainsKey(block.ID)) drawings.Add(block.ID,block);
                    switch (block.BlockType)
                    {
                        case Block.BlockTypes.HorizontalDock:
                        case Block.BlockTypes.VerticalDock:
                            foreach (Block blk in block.Blocks)
                            {
                                if (blk.BlockType == Block.BlockTypes.Drawing) if (!drawings.ContainsKey(blk.ID)) drawings.Add(blk.ID, blk);
                                switch (blk.BlockType)
                                {
                                    case Block.BlockTypes.HorizontalDock:
                                    case Block.BlockTypes.VerticalDock:
                                        testBlocks.Add(blk);
                                        break;
                                }
                        }
                            break;
                    }
                    testBlocks.RemoveAt(0);
                }

                Mr.PdfDocumentRenderer pdfDocumentRenderer = new Mr.PdfDocumentRenderer();
                pdfDocumentRenderer.Document = doc;
                pdfDocumentRenderer.PdfDocument = document;

                pdfDocumentRenderer.DocumentRenderer.PrepareDocument();
                pdfDocumentRenderer.PrepareRenderPages();
                int count = pdfDocumentRenderer.PageCount;
                int j = 0;
                for (int i = 0; i < count; i++)
                {
                    pdfDocumentRenderer.RenderPages(i + 1, i + 1);
                    Mr.RenderInfo[] infos = pdfDocumentRenderer.DocumentRenderer.GetRenderInfoFromPage(i + 1);
                    Pd.XGraphics graph = Pd.XGraphics.FromPdfPage(pdfDocumentRenderer.PdfDocument.Pages[i]);
                    
                    int t = 0;
                    foreach (Mr.RenderInfo info in infos)
                    {
                        Rg.Rectangle3d r = info.LayoutInfo.ContentArea.ToRectangle3d();
                        Rg.Plane p = r.Plane;
                        string[] tag = info.DocumentObject.Tag.ToString().Split('~');

                        switch (tag[0])
                        {
                            case "Drawing":
                                drawings[tag[1]].Drawing.Render(graph, r);
                                j++;
                                break;
                            case "DockH":
                                RenderDockH((Md.Tables.Table)info.DocumentObject, info, graph, drawings, p,true);
                                break;
                            case "DockV":
                                RenderDockV((Md.Tables.Table)info.DocumentObject, info, graph, drawings, p);
                                break;
                        }

                        t += 1;
                    }

                    graph.Dispose();
                }

                return pdfDocumentRenderer.PdfDocument;
            }

            return document;
        }

        public void RenderDockH(Md.Tables.Table table, Mr.RenderInfo info, Pd.XGraphics graph, Dictionary<string, Block> drawings, Rg.Plane plane, bool renderDrawing)
        {
            double x = info.LayoutInfo.ContentArea.X + 3;
            double y = info.LayoutInfo.ContentArea.Y;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Md.Tables.Cell cell = table.Rows[0].Cells[i];
                string[] tag = cell.Elements[0].Tag.ToString().Split('~');
                switch (tag[0])
                {
                    case "Drawing":
                        if (renderDrawing) { 
                        Md.Shapes.TextFrame frame = (Md.Shapes.TextFrame)cell.Elements[0];
                        plane.OriginX = x + table.Columns[i].Width / 2.0 - frame.Width / 2.0;
                        plane.OriginY = y +table.Rows[0].Height / 2.0 - frame.Height / 2.0;
                        Rg.Rectangle3d r2 = new Rg.Rectangle3d(plane, new Rg.Interval(0, frame.Width), new Rg.Interval(0, frame.Height));
                        drawings[tag[1]].Drawing.Render(graph, r2);
                        }
                        break;
                    case "DockH":
                        RenderDockH((Md.Tables.Table)cell.Elements[0], info, graph, drawings, plane, renderDrawing);
                        break;
                    case "DockV":
                        RenderDockV((Md.Tables.Table)cell.Elements[0],info, graph, drawings, plane);
                        break;
                }
                x += table.Columns[i].Width;
            }
        }

        public void RenderDockV(Md.Tables.Table table, Mr.RenderInfo info, Pd.XGraphics graph, Dictionary<string, Block> drawings, Rg.Plane plane)
        {
            //double x = info.LayoutInfo.ContentArea.X;
            //double y = info.LayoutInfo.ContentArea.Y;

            for (int i = 0; i < table.Rows.Count; i++)
            {
            Md.Tables.Cell cell = table.Rows[i].Cells[0];
                //Rg.Rectangle3d gpRect = info.LayoutInfo.ContentArea.ToRectangle3d();
                string[] tag = cell.Elements[0].Tag.ToString().Split('~');
                switch (tag[0])
                {
                    case "Drawing":
                        //Md.Shapes.TextFrame frame = (Md.Shapes.TextFrame)cell.Elements[0];
                        //plane.OriginX = x+table.Columns[0].Width / 2.0 - frame.Width / 2.0;
                        //plane.OriginY = y+table.Rows[0].Height / 2.0;
                        //Rg.Rectangle3d r2 = new Rg.Rectangle3d(plane, new Rg.Interval(0, frame.Width), new Rg.Interval(0, frame.Height));
                        //drawings[tag[1]].Drawing.Render(graph, r2);
                        break;
                    case "DockH":
                        RenderDockH((Md.Tables.Table)cell.Elements[0], info, graph, drawings, plane,false);
                        break;
                    case "DockV":
                        RenderDockV((Md.Tables.Table)cell.Elements[0], info, graph, drawings, plane);
                        break;
                }
            }
        }

        protected void Render(Pf.PdfPage page)
        {
            graph = Pd.XGraphics.FromPdfPage(page);
            foreach (Shape shp in shapes)
            {
                Shape shape = new Shape(shp);
                if (shape.Renderable) { 
                shape.AlignContent(this);
                shape.Render(graph, page, this.Frame);
                }
            }
            graph.Dispose();
        }

        public bool AddShape(IGH_Goo goo)
        {
            bool isValid = false;
            if (goo != null)
            {
                Shape shape = null;
                isValid = goo.TryGetShape(ref shape);
                shape.AlignContent(this, false);
                this.shapes.Add(shape);
            }
            return isValid;
        }

        public bool AddShape(Shape shape, bool align = true)
        {
            Shape shp = new Shape(shape);
            if(align)shp.AlignContent(this, false);
            this.shapes.Add(shp);

            return true;
        }

        public bool AddShapes(List<Shape> shapes, bool align = true)
        {
            foreach(Shape shape in shapes)
            {
            Shape shp = new Shape(shape);
            if (align) shp.AlignContent(this, false);
            this.shapes.Add(shp);
            }

            return true;
        }

        public bool AddBlock(IGH_Goo goo)
        {
            Block block = null;
            bool isValid = goo.TryGetBlock(ref block);
            block.id = Guid.NewGuid().ToString();
            this.blocks.Add(block);

            return isValid;
        }

        public bool AddBlock(Block block)
        {
            this.blocks.Add(new Block(block));

            return true;
        }

        public bool AddBlocks(List<Block> blocks)
        {
            foreach(Block block in blocks) this.blocks.Add(new Block(block));

            return true;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return "Page | (" + Math.Round(this.Width, 2) + Unit.Abbreviation() + "," + Math.Round(this.Height, 2) + Unit.Abbreviation()+")";
        }

        #endregion
    }
}
