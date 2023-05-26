using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;

using pdf = PdfSharp.Pdf;

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

        public void Save(string filepath)
        {
            if (PdfPlusEnvironment.FileIoBlocked)
                return;
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