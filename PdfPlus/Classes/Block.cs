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
    public class Block : Element
    {

        #region members

        public enum BlockTypes { None, LineBreak, PageBreak, Text, List, Table, Chart, Image, Drawing };
        protected BlockTypes blockType = BlockTypes.None;

        //Formatting
        protected double width = 0;
        protected double height = 0;

        //Text
        protected Font.Presets formatType = Font.Presets.Normal;
        protected string formatName = "Normal";

        //Break
        protected int breakCount = 1;

        //List
        public enum ListTypes { Dot, Circle, Square, Number, NumberAlt, Letter }
        protected ListTypes listType = ListTypes.Dot;
        protected List<string> listItems = new List<string>();

        //Table
        protected List<List<string>> tableContents = new List<List<string>>();

        #endregion

        #region constructors

        public Block():base()
        {
            this.elementType = ElementTypes.Block;
        }

        public Block(Block block):base(block)
        {
            this.graphic = new Graphic(block.graphic);
            this.font = new Font(block.font);

            this.blockType = block.blockType;

            //text
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

            //format
            this.width = block.width;
            this.height = block.height;
            this.alignment = block.alignment;
            this.justification = block.justification;
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

        public static Block CreateText(string content, Font.Presets format = Font.Presets.Normal)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Text;
            block.text = content;
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

        #region chart

        public static Block CreateChart(List<DataSet> data, ChartTypes chartType = ChartTypes.Line)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Chart;
            block.chartType = chartType;

            block.SetData(data);
            block.width = 400;
            block.height = 200;
            block.justification = Justification.Center;

            return block;
        }

        #endregion

        #region image

        public static Block CreateImage(string fileName)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Image;
            block.imageName = fileName;
            block.graphic.Color = Sd.Color.LightGray;

            return block;
        }

        public static Block CreateImage(Sd.Bitmap bitmap)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Image;
            block.imageObject = new Sd.Bitmap(bitmap);
            block.graphic.Color = Sd.Color.LightGray;

            return block;
        }

        #endregion

        #endregion

        #region properties

        public virtual double Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public virtual double Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        #endregion

        #region methods

        public Md.Document Render(Md.Document document)
        {
            Md.Paragraph txt;
            switch (this.blockType)
            {
                case BlockTypes.LineBreak:
                    Md.Shapes.TextFrame frame = document.LastSection.AddTextFrame();
                    txt = document.LastSection.AddParagraph();
                    for (int i =0;i<this.breakCount;i++) txt.AddLineBreak();
                    break;
                case BlockTypes.PageBreak:
                    for (int i = 0; i < this.breakCount; i++) document.LastSection.AddPageBreak();
                    break;
                case BlockTypes.Text:
                    txt = document.LastSection.AddParagraph(this.text);
                    txt.Format = this.font.ToMigraDocParagraphFormat(document.Styles[this.formatName].ParagraphFormat.Clone());

                    break;
                case BlockTypes.List:
                    Md.ListType listType = (Md.ListType)this.listType;
                    for (int idx = 0; idx < listItems.Count; ++idx)
                    {
                        Md.ListInfo listinfo = new Md.ListInfo();
                        listinfo.ContinuePreviousList = idx > 0;
                        listinfo.ListType = listType;
                        Md.Paragraph listItem = document.LastSection.AddParagraph(listItems[idx]);
                        listItem.Format = this.font.ToMigraDocParagraphFormat(document.Styles["List"].ParagraphFormat.Clone());
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
                            txt = table.Rows[j].Cells[i].AddParagraph(tableContents[i][j]);
                            txt.Format = this.font.ToMigraDocParagraphFormat(document.Styles["Normal"].ParagraphFormat.Clone());
                        }
                    }

                    break;

                case BlockTypes.Chart:
                    Md.Shapes.Charts.ChartType ct = this.chartType.ToMigraDoc();

                    Md.Shapes.Charts.Chart chart = document.LastSection.AddChart(ct);

                    //Chart Area
                    chart.Left = this.Justification.ToMigraDocShapePosition();
                    chart.Width = this.Width;
                    chart.Height = this.Height;

                    //Series
                    foreach (DataSet d in this.data)
                    {
                        Md.Shapes.Charts.Series series = chart.SeriesCollection.AddSeries();
                        series.HasDataLabel = d.LabelData;
                        series.DataLabel.Position = Md.Shapes.Charts.DataLabelPosition.OutsideEnd;
                        series.Name = d.Title;
                        series.MarkerStyle = Md.Shapes.Charts.MarkerStyle.None;
                        series.Add(d.Values.ToArray());
                    }

                    //Axis
                    if (this.HasXAxis)
                    {
                        chart.XAxis.Title.Alignment = Md.Shapes.Charts.HorizontalAlignment.Center;
                        chart.XAxis.Title.Caption = this.XAxis;
                        chart.XAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.Inside;
                        chart.XAxis.HasMajorGridlines = true;
                    }
                    else
                    {
                        chart.XAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.None;
                        chart.XAxis.HasMajorGridlines = false;
                    }

                    if (this.HasYAxis)
                    {
                        chart.YAxis.Title.VerticalAlignment = Md.Tables.VerticalAlignment.Center;
                        chart.YAxis.Title.Alignment = Md.Shapes.Charts.HorizontalAlignment.Center;
                        chart.YAxis.Title.Orientation = 90;
                        chart.YAxis.Title.Caption = this.YAxis;
                        chart.YAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.Inside;
                        chart.YAxis.HasMajorGridlines = true;
                    }
                    else
                    {
                        chart.YAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.None;
                        chart.YAxis.HasMajorGridlines = false;
                    }

                    //Legend
                    if (this.alignment != Alignment.None)
                    {
                        Md.Shapes.Charts.Legend legend = null;

                        switch (this.alignment)
                        {
                            default:
                                legend = chart.LeftArea.AddLegend();
                                break;
                            case Alignment.Right:
                                legend = chart.RightArea.AddLegend();
                                break;
                            case Alignment.Bottom:
                                legend = chart.BottomArea.AddLegend();
                                break;
                            case Alignment.Top:
                                legend = chart.TopArea.AddLegend();
                                break;
                        }
                    }

                    break;
                case BlockTypes.Image:
                    string filename = this.imageName;
                    if (this.imageObject != null) filename = this.imageObject.ToBase64String("base64:");

                    Md.Shapes.Image img = document.LastSection.AddImage(filename);
                    if ((this.width > 0) & (this.height > 0))
                    {
                        img.LockAspectRatio = false;
                        img.Width = this.width;
                        img.Height = this.height;
                    }
                    else
                    {
                        if (this.width > 0)
                        {
                            img.LockAspectRatio = true;
                            img.Width = this.width;
                        }
                        else if (this.height>0)
                        {
                            img.LockAspectRatio = true;
                            img.Height = this.height;
                        }
                    }
                    img.Left = this.Justification.ToMigraDocShapePosition();
                    break;
                    case BlockTypes.draw
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
