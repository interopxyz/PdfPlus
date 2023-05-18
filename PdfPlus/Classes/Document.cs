using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pdf = PdfSharp.Pdf;

namespace PdfPlus
{
    public class Document
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

        public Document(List<Page> pages)
        {
            this.AddPages(pages);
        }

        public Document(Page page)
        {
            this.AddPages(page);
        }

        #endregion

        #region methods

        public void Save(string filepath)
        {
            var doc = Bake();
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
            var pdf = new pdf.PdfDocument();
            pdf.PageLayout = this.PageLayout.ToPdf();

            foreach (Page page in this.pages)
            {
                pdf = page.AddToDocument(pdf);
            }

            return pdf;
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
    }
}
