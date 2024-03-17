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

        public enum BlockTypes { None, PageBreak, Text, List, Table, Image };
        private BlockTypes blockType = BlockTypes.None;

        public enum PathTypes { }

        //Text
        private string paragraph = string.Empty;

        //List
        public enum ListTypes { Dot,Circle,Square,Number,NumberAlt,Letter}
        private ListTypes listType = ListTypes.Dot;
        private List<string> items = new List<string>();

        //Table
        private List<List<string>> contents = new List<List<string>>();

        //Image
        string imageFileName = string.Empty;

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

            //image
            this.imageFileName = block.imageFileName;

        }

        #region break

        public static Block CreatePageBreak()
        {
            Block block = new Block();
            block.blockType = BlockTypes.PageBreak;

            return block;
        }

        #endregion

        #region text

        public static Block CreateText(string content)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Text;
            block.paragraph = content;

            return block;
        }

        #endregion

        #region list

        public static Block CreateList(List<string> items, ListTypes type = ListTypes.Dot)
        {
            Block block = new Block();
            block.blockType = BlockTypes.List;
            block.listType = type;
            foreach (string item in items) block.items.Add(item);

            return block;
        }

        #endregion

        #region table

        public static Block CreateTable(List<List<string>> items)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Table;

            int i = 0;
            foreach (List<string> set in items)
            {
                block.contents.Add(new List<string>());
                foreach(string item in set) block.contents[i].Add(item);
                i++;
            }

            return block;
        }

        #endregion

        #endregion

        #region properties



        #endregion

        #region methods

        public Md.Document Render(Md.Document document)
        {
            switch (this.blockType)
            {
                case BlockTypes.PageBreak:
                    document.AddSection();
                    break;
                case BlockTypes.Text:
                    document.LastSection.AddParagraph(this.paragraph);
                    break;
                case BlockTypes.List:
                    Md.Style style = document.AddStyle("List-"+this.listType.ToString(), "Normal");
                    style.ParagraphFormat.LeftIndent = "0.5cm";

                    Md.ListType listType = (Md.ListType)this.listType;

                    for (int idx = 0; idx < items.Count; ++idx)
                    {
                        Md.ListInfo listinfo = new Md.ListInfo();
                        listinfo.ContinuePreviousList = idx > 0;
                        listinfo.ListType = listType;
                        Md.Paragraph listItem = document.LastSection.AddParagraph(items[idx]);
                        listItem.Style = "List";
                        listItem.Format.ListInfo = listinfo;
                    }
                    break;
                case BlockTypes.Table:
                    Md.Tables.Table table = document.LastSection.AddTable();
                    int rowCount = 0;

                    //Columns
                    for (int i = 0; i < contents.Count; i++)
                    {
                        rowCount = Math.Max(rowCount, contents[i].Count);
                        table.AddColumn();
                    }

                    //Rows
                    for (int i = 0; i < rowCount; i++)
                    {
                        table.AddRow();
                    }

                    //Cells
                    for (int i = 0; i < contents.Count; i++)
                    {
                        for (int j = 0; j < contents[i].Count; j++)
                        {
                            table.Rows[j].Cells[i].AddParagraph(contents[i][j]);
                        }
                    }

                    break;
                case BlockTypes.Image:
                    document.LastSection.AddImage(imageFileName);
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
