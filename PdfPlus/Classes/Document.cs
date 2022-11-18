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
        protected pdf.PdfDocument baseObject = new pdf.PdfDocument();

        protected List<Page> pages = new List<Page>();

        #endregion

        #region constructors

        public Document()
        {
            this.baseObject = new pdf.PdfDocument();
        }

        public Document(Document document)
        {
            this.baseObject = (pdf.PdfDocument)document.baseObject.Clone();
            this.PageLayout = document.PageLayout;
            foreach(Page page in document.pages)
            {
                this.pages.Add(new Page(page));
            }
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

        #region properties

        public virtual pdf.PdfDocument BaseObject
        {
            get { return (pdf.PdfDocument)baseObject.Clone(); }
        }


        #endregion

        #region methods

        public void Save(string filepath)
        {
            this.baseObject = new pdf.PdfDocument();
            this.baseObject.PageLayout = this.PageLayout.ToPdf();
            
            foreach (Page page in this.pages)
            {
                this.baseObject = page.AddToDocument(this.baseObject);
            }
            int i = this.baseObject.Pages.Count;
            this.baseObject.Save(filepath);
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
            page.AddToDocument(this.baseObject);
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
