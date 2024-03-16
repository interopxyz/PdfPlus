using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

using Pc = PdfSharp.Charting;
using Pf = PdfSharp.Pdf;
using Pd = PdfSharp.Drawing;
using Pl = PdfSharp.Drawing.Layout;

using Md = MigraDoc.DocumentObjectModel;

using Rg = Rhino.Geometry;
using System.IO;

namespace PdfPlus
{
    public class Block
    {

        #region members

        public enum BlockTypes { None, Text, List, Table };
        private BlockTypes blockType = BlockTypes.None;

        public enum PathTypes { }

        //Text
        private string paragraph = string.Empty;

        //List
        public enum ListTypes { Dot,Square}
        private ListTypes listType = ListTypes.Dot;
        private List<string> items = new List<string>();

        //Table
        private List<List<string>> contents = new List<List<string>>();

        //

        #endregion

        #region constructors

        public Block()
        {

        }

        public Block(Block block)
        {
            this.blockType = block.blockType;

            //text
            this.paragraph = block.paragraph;

            //list
            this.listType = block.listType;
            foreach (string item in block.items) this.items.Add(item);

            //table
            int i = 0;
            foreach (List<string> entry in block.contents)
            {
                this.contents.Add(new List<string>());
                foreach (string item in entry) this.contents[i].Add(item);
                i++;
            }

        }

        #region text

        public static Block CreateText(string content)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Text;
            block.paragraph = content;

            return block;
        }

        #endregion

        #endregion

        #region properties



        #endregion

        #region members

        public Md.Document Render(Md.Document document)
        {
            document.Sections.AddSection();

            switch (this.blockType)
            {
                case BlockTypes.Text:
                    document.LastSection.AddParagraph(this.paragraph);
                    break;
            }

            return document;
        }

        #endregion


        #region overrides

        public override string ToString()
        {
            return "Block | "+this.blockType.ToString();
        }


        #endregion
    }
}
