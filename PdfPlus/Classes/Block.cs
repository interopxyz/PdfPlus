﻿using System;
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

        public enum BlockTypes { None, LineBreak, PageBreak, Text, List, Table, Image };
        private BlockTypes blockType = BlockTypes.None;

        public enum PathTypes { }

        //Text
        public enum FormatTypes { Normal, Heading1, Heading2, Heading3, Heading4, Heading5, Heading6, Heading7, Heading8, Heading9, List, Footnote, Header, Footer, Hyperlink, Custom };
        private FormatTypes formatType = FormatTypes.Normal;
        private string formatName = "Normal";
        private string paragraph = string.Empty;

        //Break
        private int breakCount = 1;

        //List
        public enum ListTypes { Dot, Circle, Square, Number, NumberAlt, Letter }
        private ListTypes listType = ListTypes.Dot;
        private List<string> listItems = new List<string>();

        //Table
        private List<List<string>> tableContents = new List<List<string>>();

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
            this.formatType = block.formatType;
            this.formatName = block.formatName;

            //break
            this.breakCount = block.breakCount;

            //list
            this.listType = block.listType;
            foreach (string item in block.listItems) this.listItems.Add(item);

            //table
            int i = 0;
            foreach (List<string> entry in block.tableContents)
            {
                this.tableContents.Add(new List<string>());
                foreach (string item in entry) this.tableContents[i].Add(item);
                i++;
            }

            //image
            this.imageFileName = block.imageFileName;

        }

        #region break

        public static Block CreatePageBreak(int count = 1)
        {
            Block block = new Block();
            block.blockType = BlockTypes.PageBreak;
            block.breakCount = count;

            return block;
        }

        public static Block CreateLineBreak(int count = 1)
        {
            Block block = new Block();
            block.blockType = BlockTypes.LineBreak;
            block.breakCount = count;

            return block;
        }

        #endregion

        #region text

        public static Block CreateText(string content, FormatTypes format = FormatTypes.Normal)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Text;
            block.paragraph = content;
            block.formatType = format;
            block.formatName = format.ToString();

            return block;
        }

        #endregion

        #region list

        public static Block CreateList(List<string> items, ListTypes type = ListTypes.Dot)
        {
            Block block = new Block();
            block.blockType = BlockTypes.List;
            block.listType = type;
            foreach (string item in items) block.listItems.Add(item);

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
                block.tableContents.Add(new List<string>());
                foreach (string item in set) block.tableContents[i].Add(item);
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
            Md.Paragraph txt;
            switch (this.blockType)
            {
                case BlockTypes.LineBreak:
                    txt = document.LastSection.AddParagraph();
                    for(int i =0;i<this.breakCount;i++) txt.AddLineBreak();
                    break;
                case BlockTypes.PageBreak:
                    for (int i = 0; i < this.breakCount; i++) document.LastSection.AddPageBreak();
                    break;
                case BlockTypes.Text:
                    txt = document.LastSection.AddParagraph(this.paragraph);
                    txt.Format = (Md.ParagraphFormat)document.Styles[this.formatName].ParagraphFormat.Clone();
                    break;
                case BlockTypes.List:
                    Md.Style style = document.AddStyle("List-" + this.listType.ToString(), "Normal");
                    style.ParagraphFormat.LeftIndent = "0.5cm";

                    Md.ListType listType = (Md.ListType)this.listType;

                    for (int idx = 0; idx < listItems.Count; ++idx)
                    {
                        Md.ListInfo listinfo = new Md.ListInfo();
                        listinfo.ContinuePreviousList = idx > 0;
                        listinfo.ListType = listType;
                        Md.Paragraph listItem = document.LastSection.AddParagraph(listItems[idx]);
                        listItem.Style = "List";
                        listItem.Format.ListInfo = listinfo;
                    }
                    break;
                case BlockTypes.Table:
                    Md.Tables.Table table = document.LastSection.AddTable();
                    int rowCount = 0;

                    //Columns
                    for (int i = 0; i < tableContents.Count; i++)
                    {
                        rowCount = Math.Max(rowCount, tableContents[i].Count);
                        table.AddColumn();
                    }

                    //Rows
                    for (int i = 0; i < rowCount; i++)
                    {
                        table.AddRow();
                    }

                    //Cells
                    for (int i = 0; i < tableContents.Count; i++)
                    {
                        for (int j = 0; j < tableContents[i].Count; j++)
                        {
                            table.Rows[j].Cells[i].AddParagraph(tableContents[i][j]);
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
            return "Block | " + this.blockType.ToString();
        }


        #endregion

    }
}
