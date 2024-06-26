﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sd = System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;

using Pf = PdfSharp.Pdf;

using Md = MigraDoc.DocumentObjectModel;

namespace PdfPlus
{
    public class Document : IGH_Goo
    {

        #region members

        public enum ColorModes { None, RGB, CMYK};
        public enum PageModes { None, Fullscreen, Outline, Thumbs }
        public enum LayoutModes { None, Single, TwoColumnLeft, TwoColumnRight,TwoPageLeft, TwoPageRight }
        public enum CompressionModes { None, Compressed, UnCompressed}

        protected List<Page> pages = new List<Page>();

        public ColorModes ColorMode = ColorModes.None;
        public PageModes PageMode = PageModes.None;
        public LayoutModes LayoutMode = LayoutModes.None;
        public CompressionModes CompressionMode = CompressionModes.None;

        public string Title = "";
        public string Subject = "";
        public string Author = "";
        protected string creator = "Pdf+ plugin for Grasshopper 3d (https://github.com/interopxyz/PdfPlus)";
        protected List<string> keywords = new List<string>();

        private string password = "";
        private bool hasPassword = false;

        private string ownerPassword = Guid.NewGuid().ToString();
        private bool permitExtract = true;
        private bool permitModify = true;
        private bool permitPrint = true;


        #endregion

        #region constructors

        public Document()
        {

        }

        public Document(Document document)
        {
            CopyFrom(document);
        }

        public Document(Page page)
        {
            this.AddPages(page);
        }

        public Document(List<Page> pages)
        {
            this.AddPages(pages);
        }

        #endregion

        #region properties

        public virtual string Password
        {
            set
            {
                this.password = value;
                this.hasPassword = true;
            }
        }

        public virtual string OwnerPassword
        {
            set
            {
                this.ownerPassword = value;
            }
        }

        public virtual bool PermitModify
        {
            set { this.permitModify = value; }
            get { return this.permitModify; }
        }

        public virtual bool PermitExtract
        {
            set { this.permitExtract = value; }
            get { return this.permitExtract; }
        }

        public virtual bool PermitPrint
        {
            set { this.permitPrint = value; }
            get { return this.permitPrint; }
        }

        public virtual string Creator
        {
            get { return this.creator; }
            set { this.creator = value; }
        }

        public virtual List<string> Keywords
        {
            get { 
                List<string> output = new List<string>();
                foreach (string keyword in this.keywords) output.Add(keyword);
                return output;
            }
            set { foreach (string keyword in value) this.keywords.Add(keyword); }
        }

        public virtual List<Page> Pages
        {
            get
            {
                List<Page> output = new List<Page>();
                foreach (Page page in pages) output.Add(new Page(page));
                return output;
            }
        }

        public virtual List<Shape> Shapes(bool blocks = false)
        {
                List<Shape> outputs = new List<Shape>();
                
                foreach (Page page in this.pages)
                {
                if (blocks) { 
                    foreach(Page subPage in page.RenderBlocksToPages())
                    {
                        foreach (Shape shape in subPage.Shapes) if(!shape.IsPreview) outputs.Add(new Shape(shape));
                    }
                }
                foreach (Shape shape in page.Shapes) if (!shape.IsPreview) outputs.Add(new Shape(shape));
                }
                return outputs;
        }

        public virtual List<Block> Blocks
        {
            get
            {
                List<Block> outputs = new List<Block>();
                foreach (Page page in this.pages)
                {
                    foreach (Block block in page.Blocks)
                    {
                        outputs.Add(new Block(block));
                    }
                }
                return outputs;
            }
        }

        #endregion

        #region methods

        public static Md.Document DefaultDocument()
        {
            Md.Document doc = new Md.Document();
            doc = Document.SetDefaultStyles(doc);
            return doc;
        }

        public static Md.Document SetDefaultStyles(Md.Document doc)
        {
            Font font = Fonts.Normal;
            Md.Style style = doc.Styles[font.Name];

            #region Normal

            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();

            #endregion

            #region Title

            font = Fonts.Title;
            style = doc.Styles.AddStyle(font.Name, "Normal");
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceAfter = 3;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Subtitle

            font = Fonts.Subtitle;
            style = doc.Styles.AddStyle(font.Name, "Normal");
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceAfter = 16;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading1

            font = Fonts.Heading1;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 20;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading2

            font = Fonts.Heading2;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 18;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading3

            font = Fonts.Heading3;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 16;
            style.ParagraphFormat.SpaceAfter = 4;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading4

            font = Fonts.Heading4;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 14;
            style.ParagraphFormat.SpaceAfter = 4;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading5

            font = Fonts.Heading5;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 12;
            style.ParagraphFormat.SpaceAfter = 4;
            style.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading6

            font = Fonts.Heading6;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 12;
            style.ParagraphFormat.SpaceAfter = 4;
            style.ParagraphFormat.KeepWithNext = true;
            style.ParagraphFormat.Font.Italic = true;

            #endregion

            #region Quote

            font = Fonts.Quote;
            style = doc.Styles.AddStyle(font.Name, "Normal");
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 12;
            style.ParagraphFormat.SpaceAfter = 12;
            style.ParagraphFormat.LeftIndent = 18;
            style.ParagraphFormat.Borders.Left.Width = new Md.Unit(1, Md.UnitType.Point);
            style.ParagraphFormat.Borders.Left.Color = Sd.Color.LightGray.ToMigraDoc();
            style.ParagraphFormat.Borders.DistanceFromLeft = new Md.Unit(3, Md.UnitType.Point);

            #endregion

            #region Footnote

            font = Fonts.Footnote;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.LeftIndent = 12;
            style.ParagraphFormat.RightIndent = 12;
            style.ParagraphFormat.Font.Italic = true;

            #endregion

            #region Caption

            font = Fonts.Caption;
            style = doc.Styles.AddStyle(font.Name, "Normal");
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();
            style.ParagraphFormat.SpaceAfter = 12;
            style.ParagraphFormat.LeftIndent = 12;
            style.ParagraphFormat.RightIndent = 12;

            #endregion

            #region List

            font = Fonts.List;
            style = doc.Styles[font.Name];
            style.Font.Name = font.Family;
            style.Font.Size = font.Size;
            style.Font.Color = font.Color.ToMigraDoc();

            #endregion

            #region Hyperlink

            var links = doc.Styles["Hyperlink"];
            links.Font.Color = Sd.Color.CornflowerBlue.ToMigraDoc();

            #endregion

            #region Normal (Markdown)

            var body = doc.Styles["Normal"];

            body.Font.Size = Md.Unit.FromPoint(10);
            body.Font.Color = Sd.Color.Black.ToMigraDoc();

            body.ParagraphFormat.LineSpacingRule = Md.LineSpacingRule.Multiple;
            body.ParagraphFormat.LineSpacing = 1.00;
            body.ParagraphFormat.SpaceAfter = 0;

            #endregion

            #region Unordered List (Markdown)

            var unorderedlist = doc.AddStyle("UnorderedList", "Normal");
            var listInfo = new Md.ListInfo();
            listInfo.ListType = Md.ListType.BulletList1;
            unorderedlist.ParagraphFormat.ListInfo = listInfo;
            unorderedlist.ParagraphFormat.LeftIndent = "1cm";
            unorderedlist.ParagraphFormat.FirstLineIndent = "-0.5cm";
            unorderedlist.ParagraphFormat.SpaceAfter = 0;

            #endregion 

            #region Ordered List (Markdown)

            var orderedlist = doc.AddStyle("OrderedList", "UnorderedList");
            orderedlist.ParagraphFormat.ListInfo.ListType = Md.ListType.NumberList1;

            // for list spacing (since MigraDoc doesn't provide a list object that we can target)
            var listStart = doc.AddStyle("ListStart", "Normal");
            listStart.ParagraphFormat.SpaceAfter = 0;
            listStart.ParagraphFormat.LineSpacing = 0.5;
            var listEnd = doc.AddStyle("ListEnd", "ListStart");
            listEnd.ParagraphFormat.LineSpacing = 1;

            #endregion

            #region Horizontal Rule

            var hr = doc.AddStyle("HorizontalRule", "Normal");
            var hrBorder = new Md.Border();
            hrBorder.Width = "1pt";
            hrBorder.Color = Sd.Color.DarkGray.ToMigraDoc();
            hr.ParagraphFormat.Borders.Bottom = hrBorder;
            hr.ParagraphFormat.LineSpacing = 0;
            hr.ParagraphFormat.SpaceBefore = 15;

            #endregion

            return doc;
        }

        public void Save(string filepath)
        {
            if (PdfPlusEnvironment.FileIoBlocked)
                return;
            Pf.PdfDocument doc = Bake();
            doc.Save(filepath);
        }

        public void AddPages(List<Page> pages)
        {
            foreach (Page page in pages)
            {
                this.pages.Add(new Page(page));
            }
        }

        public void AddPages(Page page)
        {
            this.pages.Add(new Page(page));
        }

        public List<Page> RenderBlocksToPages()
        {
            List<Page> pgs = new List<Page>();
            foreach (Page page in this.Pages) pgs.AddRange(page.RenderBlocksToPages());
            return pgs;
        }

        public void CopyFrom(Document document)
        {
            this.pages = new List<Page>();
            AddPages(document.pages);

            this.Title = document.Title;
            this.Subject = document.Subject;
            this.Author = document.Author;
            this.Creator = document.Creator;
            this.Keywords = document.Keywords;

            this.password = document.password;
            this.hasPassword = document.hasPassword;

            this.ownerPassword = document.ownerPassword;
            this.permitExtract = document.permitExtract;
            this.permitModify = document.permitModify;
            this.permitPrint = document.permitPrint;

            this.ColorMode = document.ColorMode;
            this.PageMode = document.PageMode;
            this.LayoutMode = document.LayoutMode;
            this.CompressionMode = document.CompressionMode;



        }

        public void SetDocumentProperties(Pf.PdfDocument document)
        {
            document.Info.Title = this.Title;
            document.Info.Subject = this.Subject;

            document.Info.Author = this.Author;
            document.Info.Creator = this.Creator;

            document.Info.Keywords = String.Join(", ", this.keywords);

            if (this.hasPassword) document.SecuritySettings.UserPassword = this.password;

            if (!this.permitExtract | !this.permitModify | !this.permitPrint)
            {
            document.SecuritySettings.OwnerPassword = this.ownerPassword;
            document.SecuritySettings.DocumentSecurityLevel = Pf.Security.PdfDocumentSecurityLevel.Encrypted40Bit;

            document.SecuritySettings.PermitExtractContent = this.permitExtract;
            document.SecuritySettings.PermitAccessibilityExtractContent = this.permitExtract;
            document.SecuritySettings.PermitModifyDocument = this.permitModify;
            document.SecuritySettings.PermitPrint = this.permitPrint;
            }

            document.PageLayout = this.LayoutMode.ToPdf();
            document.PageMode = this.PageMode.ToPdf();
            document.Options.ColorMode = this.ColorMode.ToPdf();
            
            //document.CustomValues.CompressionMode = this.CompressionMode.ToPdf();
        }

        protected Pf.PdfDocument Bake()
        {
            Pf.PdfDocument doc = new Pf.PdfDocument();
            
            Dictionary<string,List<Page>> clusters = new Dictionary<string,  List<Page>>();
            foreach (Page pg in this.pages)
            {
                if (!clusters.ContainsKey(pg.id)) clusters.Add(pg.id, new List<Page>());
                clusters[pg.id].Add(pg);
            }

            List<Page> queue = new List<Page>();
            foreach(string key in clusters.Keys)
            {
                queue.AddRange(clusters[key].OrderBy(o => o.Index).ToList());
            }

            foreach (Page pg in queue)
            {
                doc = pg.AddToDocument(doc);
            }

            //Metadata
            this.SetDocumentProperties(doc);

            return doc;
        }

        public MemoryStream GetStream()
        {
            if (this.pages.Count == 0)
                return null;
            var pdf = Bake();
            var stream = new MemoryStream();
            pdf.Save(stream, false);
            return stream;
        }

        public byte[] GetByteArray()
        {
            var stream = GetStream();
            if (stream == null)
                return null;
            byte[] fileContents = stream.ToArray();
            stream.Dispose();
            return fileContents;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            string c = this.pages.Count.ToString();
            bool status = false;
            foreach (Page p in this.pages) if (p.Blocks.Count > 0) status = true;
            if (status) c += "+";
            return "Document | " + c + " Pages";
        }

        #endregion

        #region IGH_Goo

        public bool IsValid { get { return this.IsValidWhyNot == string.Empty; } }

        public string IsValidWhyNot
        {
            get
            {
                if (this.pages.Count == 0)
                {
                    return "No pages";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string TypeName { get { return "PdfPlus"; } }

        public string TypeDescription { get { return "Grasshopper type for PDF+ document"; } }

        public IGH_Goo Duplicate()
        {
            return new Document(this);
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {

            if (source is Document)
            {
                CopyFrom((Document)source);
                return true;
            }
            else if (source is IGH_Goo)
            {
                if (((IGH_Goo)source).CastTo(out Document document))
                {
                    CopyFrom((Document)source);
                    return true;
                }
            }

            return false;
        }

        public bool CastTo<T>(out T target)
        {
            target = default(T);

            if (typeof(T).IsAssignableFrom(typeof(Pf.PdfDocument)))
            {
                target = (T)((object)Bake());
                return true;
            }
            else if (typeof(T).IsAssignableFrom(typeof(byte[])))
            {
                var obj = (object)GetByteArray();
                if (obj == null)
                    return false;
                target = (T)obj;
                return true;
            }
            else if (typeof(T).IsAssignableFrom(typeof(MemoryStream)))
            {
                var obj = (object)GetStream();
                if (obj == null)
                    return false;
                target = (T)obj;
                return true;
            }

            return false;
        }

        public object ScriptVariable()
        {
            return Bake();
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }

        public bool Read(GH_IReader reader)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}