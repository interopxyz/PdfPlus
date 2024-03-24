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
        protected int columnWidth = -1;
        protected int rowHeight = -1;

        //Geometry
        protected Drawing drawing = null;

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
            this.columnWidth = block.columnWidth;
            this.rowHeight = block.rowHeight;

            //drawing
            if (block.drawing != null) this.drawing = new Drawing(block.drawing);

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

            block.font = Fonts.Table;

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

        #region drawing

        public static Block CreateDrawing(List<Shape> shapes)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Drawing;
            block.drawing = new Drawing(shapes);

            return block;
        }

        public static Block CreateDrawing(Shape  shape)
        {
            Block block = Block.CreateDrawing(new List<Shape> { shape });

            return block;
        }

        #endregion

        #endregion

        #region properties

        public virtual BlockTypes BlockType
        {
            get { return this.blockType; }
        }

        public virtual Drawing Drawing
        {
            get { return drawing; }
        }
        
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

        public virtual int ColumnWidth
        {
            get { return this.columnWidth; }
            set { this.columnWidth = value; }
        }

        public virtual int RowHeight
        {
            get { return this.RowHeight; }
            set { this.rowHeight = value; }
        }

        #endregion

        #region methods

        public Md.Document Render(Md.Document document)
        {
            
            Md.Paragraph txt;
            switch (this.blockType)
            {
                case BlockTypes.LineBreak:
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
                    #region table
                    Md.Tables.Table table = document.LastSection.AddTable();
                    int colCount = tableContents.Count;
                    int rowCount = 0;
                    double columnWidth = this.columnWidth;
                    if(columnWidth<0) columnWidth = (document.Sections[0].PageSetup.PageWidth - document.Sections[0].PageSetup.LeftMargin - document.Sections[0].PageSetup.RightMargin)/colCount;

                    table.TopPadding = 6;
                    table.BottomPadding = 6;
                    table.Format.Alignment = Md.ParagraphAlignment.Justify;
                    Md.Shading shading = new Md.Shading();
                    shading.Color = this.graphic.Color.ToMigraDoc();
                    table.Format.Shading = shading;

                    //Columns
                    for (int i = 0; i < tableContents.Count; i++)
                    {
                        rowCount = Math.Max(rowCount, tableContents[i].Count);
                        Md.Tables.Column column = table.AddColumn();
                        column.Width = columnWidth;
                        column.Borders.Left.Visible = false;
                    }

                    //Rows
                    for (int i = 0; i < rowCount; i++)
                    {
                        Md.Tables.Row row = table.AddRow();
                        row.Borders.Color = this.graphic.Stroke.ToMigraDoc();
                        row.Borders.Visible = false;
                    }

                    //Borders
                    if (this.verticalBorderStyle == BorderStyles.All)
                    {
                        table.Columns[0].Borders.Left.Visible = true;
                        table.Columns[0].Borders.Left.Width = this.graphic.Weight;
                        table.Columns[colCount - 1].Borders.Right.Visible = true;
                        table.Columns[colCount - 1].Borders.Right.Width = this.graphic.Weight;
                        table.Columns[colCount - 1].Borders.Left.Visible = true;
                        table.Columns[colCount - 1].Borders.Left.Width = this.graphic.Weight;
                    }

                    if (this.verticalBorderStyle != BorderStyles.None)
                    {
                        table.Columns[0].Borders.Right.Visible = true;
                        table.Columns[0].Borders.Right.Width = this.graphic.Weight;
                        for (int i = 1; i < colCount - 1; i++)
                        {
                            table.Columns[i].Borders.Left.Visible = true;
                            table.Columns[i].Borders.Left.Width = this.graphic.Weight;
                            table.Columns[i].Borders.Right.Visible = true;
                            table.Columns[i].Borders.Right.Width = this.graphic.Weight;
                        }
                    }

                    if (this.horizontalBorderStyle == BorderStyles.All)
                    {
                        table.Rows[0].Borders.Top.Visible = true;
                        table.Rows[0].Borders.Top.Width = this.graphic.Weight;
                        table.Rows[rowCount - 1].Borders.Top.Visible = true;
                        table.Rows[rowCount - 1].Borders.Top.Width = this.graphic.Weight;
                        table.Rows[rowCount - 1].Borders.Bottom.Visible = true;
                        table.Rows[rowCount - 1].Borders.Bottom.Width = this.graphic.Weight;
                    }

                    if (this.horizontalBorderStyle != BorderStyles.None)
                    {
                        table.Rows[0].Borders.Bottom.Visible = true;
                        table.Rows[0].Borders.Bottom.Width = this.graphic.Weight;
                        for (int i = 1; i < rowCount - 1; i++)
                        {
                            table.Rows[i].Borders.Top.Visible = true;
                            table.Rows[i].Borders.Top.Width = this.graphic.Weight;
                            table.Rows[i].Borders.Bottom.Visible = true;
                            table.Rows[i].Borders.Bottom.Width = this.graphic.Weight;
                        }
                    }

                    //Cells
                    List<List<Md.Paragraph>> contents = new List<List<Md.Paragraph>>();
                    for (int i = 0; i < colCount; i++)
                    {
                        contents.Add(new List<Md.Paragraph>());
                        for (int j = 0; j < rowCount; j++)
                        {
                            Md.Tables.Cell cell = table.Rows[j].Cells[i];
                            cell.Shading.Color = this.FillColor.ToMigraDoc();

                            txt = cell.AddParagraph(tableContents[i][j]);
                            txt.Format = this.font.ToMigraDocParagraphFormat(document.Styles["Normal"].ParagraphFormat.Clone());

                            contents[i].Add(txt);
                        }
                    }

                    if (this.columnWidth == 0)
                    {
                        Pf.PdfDocument d = new Pf.PdfDocument();
                        Pf.PdfPage p = d.AddPage();
                        Pd.XGraphics gfx = Pd.XGraphics.FromPdfPage(p);

                        for (int i = 0; i < colCount; i++)
                        {
                            double txtWidth = 0;
                            for (int j = 0; j < rowCount; j++)
                            {
                                txtWidth = Math.Max(Math.Ceiling(txtWidth), Math.Ceiling(gfx.MeasureString(tableContents[i][j], this.font.ToPdf()).Width));
                            }
                            table.Columns[i].Width = txtWidth+8;
                        }
                    }

                    //Alternating Colors
                    if (hasAlternatingColor)
                    {
                        int alpha = 255;

                        for (int j = 0; j < colCount; j++)
                        {
                            Md.Tables.Cell cell = table.Rows[0].Cells[j];
                            cell.Shading.Color = this.AlternateColor.ToMigraDoc(alpha);
                            cell.Format.Shading.Color = Md.Colors.Transparent;
                        }
                        if (this.HasXAxis) alpha = 125;

                        for (int i = 2; i < rowCount; i += 2)
                        {
                            for (int j = 0; j < colCount; j++)
                            {
                                Md.Tables.Cell cell = table.Rows[i].Cells[j];
                                cell.Shading.Color = this.AlternateColor.ToMigraDoc(alpha);
                                cell.Format.Shading.Color = Md.Colors.Transparent;
                            }
                        }

                    }

                    //Headers
                    if (HasXAxis)
                    {
                        for (int i = 0; i < colCount; i++)
                        {
                            contents[i][0].Format.Font.Bold = true;
                            contents[i][0].Format.Font.Size = contents[i][0].Format.Font.Size * 1.25;
                        }
                        table.Rows[0].Borders.Bottom.Width = table.Rows[0].Borders.Bottom.Width*2;
                    }

                    if (HasYAxis)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            contents[0][i].Format.Font.Bold = true;
                            contents[0][i].Format.Font.Size = contents[0][i].Format.Font.Size * 1.25;
                        }
                        table.Columns[0].Borders.Right.Width = table.Columns[0].Borders.Right.Width*2;
                    }
                    #endregion
                    break;
                case BlockTypes.Chart:
                    #region chart
                    Md.Shapes.Charts.ChartType ct = this.chartType.ToMigraDoc();

                    Md.Shapes.Charts.Chart chart = document.LastSection.AddChart(ct);

                    chart.TopArea.TopPadding = 6;

                    //Chart Area
                    chart.Left = this.Justification.ToMigraDocShapePosition();

                    if (this.Width <= 0)
                    {
                        chart.Width = (document.Sections[0].PageSetup.PageWidth - document.Sections[0].PageSetup.LeftMargin - document.Sections[0].PageSetup.RightMargin);
                        if (this.chartType == ChartTypes.Pie) chart.Width = this.Width / 3.0;
                    }
                    else
                    {
                        chart.Width = this.Width;
                    }

                    if (this.Height <= 0)
                    {
                        chart.Height = chart.Width/2.0;
                        if (this.chartType == ChartTypes.Pie) chart.Height = chart.Width;
                    }
                    else
                    {
                        chart.Height = this.Height;
                    }

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
                        //chart.YAxis.TickLabels.Font.Size = 0;
                        //chart.YAxis.TickLabels.Font.Color = Sd.Color.Transparent.ToMigraDoc();
                    }

                    if (this.HasYAxis)
                    {
                        chart.YAxis.Title.VerticalAlignment = Md.Tables.VerticalAlignment.Center;
                        chart.YAxis.Title.Alignment = Md.Shapes.Charts.HorizontalAlignment.Center;
                        chart.YAxis.Title.Orientation = 90;

                        chart.YAxis.HasMajorGridlines = true;
                    }
                    else
                    {
                        chart.YAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.None;
                        chart.YAxis.HasMajorGridlines = false;
                        //chart.YAxis.TickLabels.Font.Size = 0;
                        //chart.YAxis.TickLabels.Font.Color = Sd.Color.Transparent.ToMigraDoc();
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
                    #endregion
                    break;
                case BlockTypes.Drawing:
                    #region drawing
                    Md.Shapes.TextFrame frame = document.LastSection.AddTextFrame();

                    Rg.BoundingBox box = this.drawing.BoundingBox;

                    frame.Width = box.Diagonal.X;
                    frame.Height = box.Diagonal.Y;
                    if ((this.width>0) & (this.height > 0))
                    {
                        frame.Width = this.width;
                        frame.Height = this.height;
                    }
                    else if (this.width > 0)
                    {
                        frame.Width = this.width;
                        frame.Height = box.Diagonal.Y * (this.Width / box.Diagonal.X);
                    }
                    else if(this.height > 0)
                    {
                        frame.Width = box.Diagonal.X * (this.Height / box.Diagonal.Y);
                        frame.Height = this.height;
                    }
                    frame.Left = this.Justification.ToMigraDocShapePosition();
                    frame.Tag = "Drawing";
                    #endregion
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
