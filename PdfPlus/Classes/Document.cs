using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sd = System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;

using pdf = PdfSharp.Pdf;

using Md = MigraDoc.DocumentObjectModel;

namespace PdfPlus
{
    public class Document : IGH_Goo
    {

        #region members

        public PageLayouts PageLayout = PageLayouts.Single;

        protected List<Page> pages = new List<Page>();

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
        public virtual List<Page> Pages
        {
            get
            {
                List<Page> output = new List<Page>();
                foreach (Page page in pages) output.Add(new Page(page));
                return output;
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

            #region Normal

            Md.Style normal = doc.Styles["Heading1"];
            normal.Font.Name = "Arial";
            normal.Font.Size = 11;
            normal.Font.Color = Sd.Color.Black.ToMigraDoc();

            #endregion

            #region Title

            Md.Style title = doc.Styles.AddStyle("Title", "Normal");
            title.Font.Name = "Arial";
            title.Font.Size = 26;
            title.Font.Color = Sd.Color.Black.ToMigraDoc();
            title.ParagraphFormat.SpaceAfter = 3;
            title.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Subtitle

            Md.Style subtitle = doc.Styles.AddStyle("Subtitle", "Normal");
            subtitle.Font.Name = "Arial";
            subtitle.Font.Size = 15;
            subtitle.Font.Color = Sd.Color.DarkGray.ToMigraDoc();
            subtitle.ParagraphFormat.SpaceAfter = 16;
            subtitle.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading1

            Md.Style heading1 = doc.Styles["Heading1"];
            heading1.Font.Name = "Arial";
            heading1.Font.Size = 20;
            heading1.Font.Color = Sd.Color.Black.ToMigraDoc();
            heading1.ParagraphFormat.SpaceBefore = 20;
            heading1.ParagraphFormat.SpaceAfter = 6;
            heading1.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading2

            Md.Style heading2 = doc.Styles["Heading2"];
            heading2.Font.Name = "Arial";
            heading2.Font.Size = 16;
            heading2.Font.Color = Sd.Color.Black.ToMigraDoc();
            heading2.ParagraphFormat.SpaceBefore = 18;
            heading2.ParagraphFormat.SpaceAfter = 6;
            heading2.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading3

            Md.Style heading3 = doc.Styles["Heading3"];
            heading3.Font.Name = "Arial";
            heading3.Font.Size = 14;
            heading3.Font.Color = Sd.Color.Gray.ToMigraDoc();
            heading3.ParagraphFormat.SpaceBefore = 16;
            heading3.ParagraphFormat.SpaceAfter = 4;
            heading3.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading4

            Md.Style heading4 = doc.Styles["Heading4"];
            heading4.Font.Name = "Arial";
            heading4.Font.Size = 12;
            heading4.Font.Color = Sd.Color.Gray.ToMigraDoc();
            heading4.ParagraphFormat.SpaceBefore = 14;
            heading4.ParagraphFormat.SpaceAfter = 4;
            heading4.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading5

            Md.Style heading5 = doc.Styles["Heading5"];
            heading5.Font.Name = "Arial";
            heading5.Font.Size = 11;
            heading5.Font.Color = Sd.Color.Gray.ToMigraDoc();
            heading5.ParagraphFormat.SpaceBefore = 12;
            heading5.ParagraphFormat.SpaceAfter = 4;
            heading5.ParagraphFormat.KeepWithNext = true;

            #endregion

            #region Heading6

            Md.Style heading6 = doc.Styles["Heading6"];
            heading6.Font.Name = "Arial";
            heading6.Font.Size = 11;
            heading6.Font.Color = Sd.Color.Gray.ToMigraDoc();
            heading6.ParagraphFormat.SpaceBefore = 12;
            heading6.ParagraphFormat.SpaceAfter = 4;
            heading6.ParagraphFormat.KeepWithNext = true;
            heading6.ParagraphFormat.Font.Italic = true;

            #endregion

            #region Quote

            Md.Style quote = doc.Styles.AddStyle("Quote", "Normal");
            quote.Font.Name = "Arial";
            quote.Font.Size = 11;
            quote.Font.Color = Sd.Color.DarkGray.ToMigraDoc();
            quote.ParagraphFormat.SpaceBefore = 12;
            quote.ParagraphFormat.SpaceAfter = 12;
            quote.ParagraphFormat.LeftIndent = 18;
            quote.ParagraphFormat.Borders.Left.Width = new Md.Unit(1, Md.UnitType.Point);
            quote.ParagraphFormat.Borders.Left.Color = Sd.Color.LightGray.ToMigraDoc();
            quote.ParagraphFormat.Borders.DistanceFromLeft = new Md.Unit(3, Md.UnitType.Point);
            #endregion

            #region Footnote

            Md.Style footnote = doc.Styles["Footnote"];
            footnote.Font.Name = "Arial";
            footnote.Font.Size = 8;
            footnote.Font.Color = Sd.Color.Black.ToMigraDoc();
            footnote.ParagraphFormat.SpaceBefore = 6;
            footnote.ParagraphFormat.SpaceAfter = 6;
            footnote.ParagraphFormat.LeftIndent = 12;
            footnote.ParagraphFormat.RightIndent = 12;
            footnote.ParagraphFormat.Font.Italic = true;

            #endregion

            #region List

            Md.Style list = doc.Styles["List"];

            #endregion

            return doc;
        }

        public void Save(string filepath)
        {
            if (PdfPlusEnvironment.FileIoBlocked)
                return;
            pdf.PdfDocument doc = Bake();
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

        public void CopyFrom(Document document)
        {
            this.pages = new List<Page>();
            AddPages(document.pages);
            this.PageLayout = document.PageLayout;
        }

        protected pdf.PdfDocument Bake()
        {
            pdf.PdfDocument doc = new pdf.PdfDocument();
            doc.PageLayout = this.PageLayout.ToPdf();

            foreach (Page page in this.pages)
            {
                doc = page.AddToDocument(doc);
            }

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
            return "Document " + this.pages.Count + "pgs";
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
            // Note: GH_Param<T>.Cast_Object(object data), which is used when adding data to a floating parameter, or
            // when collecting data from sources, tries to use
            //   target = InstantiateT();
            //   target.CastFrom(data) ...
            // before trying to use
            //   data.CastTo<T>(out target)

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

            if (typeof(T).IsAssignableFrom(typeof(pdf.PdfDocument)))
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