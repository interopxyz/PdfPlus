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
using MigraDoc.Extensions.Markdown;
using MigraDoc.Extensions.Html;

using Rg = Rhino.Geometry;
using System.IO;

namespace PdfPlus
{
    public class Block : ObjectAssembly
    {

        #region members

        public enum BlockTypes { None, LineBreak, PageBreak, Text, Markdown, Html, List, Table, Chart, Image, Drawing, HorizontalDock, VerticalDock };
        protected BlockTypes blockType = BlockTypes.None;
        protected List<Block> blocks = new List<Block>();

        //Formatting
        protected double width = 0;
        protected double height = 0;

        //Text
        protected Font.Presets formatType = Font.Presets.Normal;
        protected string formatName = "Normal";
        protected string contents = string.Empty;

        //Break
        protected int breakCount = 1;

        //List
        public enum ListTypes { Dot, Circle, Square, Number, NumberAlt, Letter }
        protected ListTypes listType = ListTypes.Dot;
        protected List<string> listItems = new List<string>();

        //Table
        protected int columnWidth = -1;
        protected int rowHeight = -1;

        //Geometry
        protected Drawing drawing = null;

        #endregion

        #region constructors

        public Block() : base()
        {
            this.elementType = ElementTypes.Block;
        }

        public Block(Block block) : base(block)
        {

            this.blockType = block.blockType;

            //text
            this.formatType = block.formatType;
            this.formatName = block.formatName;
            this.contents = block.contents;

            //break
            this.breakCount = block.breakCount;

            //list
            this.listType = block.listType;
            foreach (string item in block.listItems) this.listItems.Add(item);

            //table
            this.columnWidth = block.columnWidth;
            this.rowHeight = block.rowHeight;

            //drawing
            if (block.drawing != null) this.drawing = new Drawing(block.drawing);

            //format
            this.width = block.width;
            this.height = block.height;
            this.alignment = block.alignment;
            this.justification = block.justification;

            List<Block> newBlocks = new List<Block>();
            foreach (Block blk in block.blocks) newBlocks.Add(new Block(blk));
            this.blocks = newBlocks;
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

        public static Block CreateText(string content, Font.Presets format = Font.Presets.None)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Text;
            block.formatType = format;
            block.formatName = format.ToString();
            block.fragments.Add(new Fragment(content, Fonts.GetPreset(format)));
            block.font = Fonts.GetPreset(format);

            return block;
        }

        public static Block CreateText(Fragment fragment, Font.Presets format = Font.Presets.None)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Text;
            block.fragments.Add(new Fragment(fragment));
            block.formatType = format;
            block.formatName = format.ToString();
            block.font = Fonts.GetPreset(format);

            return block;
        }

        public static Block CreateMarkdown(string markdown)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Markdown;
            block.contents = markdown;

            return block;
        }

        public static Block CreateHtml(string html)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Html;
            block.contents = html;

            return block;
        }

        #endregion

        #region list

        public static Block CreateList(List<string> items, ListTypes type = ListTypes.Dot)
        {
            Block block = new Block();
            block.blockType = BlockTypes.List;
            block.listType = type;
            foreach (string item in items) block.fragments.Add(new Fragment(item));

            return block;
        }

        public static Block CreateList(List<Fragment> items, ListTypes type = ListTypes.Dot)
        {
            Block block = new Block();
            block.blockType = BlockTypes.List;
            block.listType = type;
            foreach (Fragment fragment in items) block.fragments.Add(new Fragment(fragment));

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
                block.data.Add(new DataSet(set));
                i++;
            }

            block.font = Fonts.Table;

            return block;
        }

        public static Block CreateTable(List<DataSet> datasets)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Table;

            int i = 0;
            foreach (DataSet d in datasets)
            {
                block.data.Add(new DataSet(d));
                i++;
            }

            block.font = Fonts.Table;

            return block;
        }

        #endregion

        #region chart

        public static Block CreateChart(DataSet data, ChartTypes chartType = ChartTypes.Pie)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Chart;
            block.chartType = chartType;

            block.SetData(data);
            block.justification = Justification.Center;

            return block;
        }

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
            block.width = -1;
            block.height = -1;
            block.justification = Justification.Center;

            return block;
        }

        public static Block CreateImage(Sd.Bitmap bitmap)
        {
            Block block = new Block();
            block.blockType = BlockTypes.Image;
            block.imageObject = new Sd.Bitmap(bitmap);
            block.graphic.Color = Sd.Color.LightGray;
            block.width = -1;
            block.height = -1;
            block.justification = Justification.Center;
            block.imageName = bitmap.ToBase64String("base64:");

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

        public static Block CreateDrawing(Shape shape)
        {
            Block block = Block.CreateDrawing(new List<Shape> { shape });

            return block;
        }

        #endregion

        #region dock

        public static Block CreateDock(List<Block> blocks)
        {
            Block block = new Block();
            block.blockType = BlockTypes.HorizontalDock;
            foreach (Block blk in blocks) block.blocks.Add(new Block(blk));

            return block;
        }

        public static Block CreateGroup(List<Block> blocks)
        {
            Block block = new Block();
            block.blockType = BlockTypes.VerticalDock;
            foreach (Block blk in blocks) block.blocks.Add(new Block(blk));

            return block;
        }

        #endregion

        #endregion

        #region properties

        public virtual ListTypes ListType
        {
            get { return this.listType; }
        }

        public virtual BlockTypes BlockType
        {
            get { return this.blockType; }
        }

        public virtual List<Block> Blocks
        {
            get { return this.blocks; }
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

        public void RenderText(Md.Paragraph paragraph, Md.Document document)
        {
            paragraph.Tag = "Text~" + this.id;
            if (this.formatType == Font.Presets.None)
            {
                paragraph.RenderFragments(this.fragments);
            }
            else
            {
                paragraph.AddText(this.Text);
                paragraph.Format = this.font.ToMigraDocParagraphFormat(document.Styles[this.formatName].ParagraphFormat.Clone());
            }
        }

        public void RenderMarkdown(Md.Section section, Md.Document document)
        {
            int count = section.Elements.Count;
            section.AddMarkdown(this.contents);
            for (int i = count; i < section.Elements.Count; i++)
            {
                string style = ((Md.Paragraph)section.Elements[i]).Style;
                section.Elements[i].Tag = "Markdown~" + this.id;
            }
        }

        public void RenderHtml(Md.Section section, Md.Document document)
        {
            int count = section.Elements.Count;
            section.AddHtml(this.contents);
            for (int i = count; i < section.Elements.Count; i++) section.Elements[i].Tag = "Html~" + this.id;
        }

        public void RenderList(Md.Document document)
        {
            Md.ListType listType = (Md.ListType)this.listType;
            for (int i = 0; i < this.fragments.Count; i++)
            {
                Md.ListInfo listinfo = new Md.ListInfo();
                listinfo.ContinuePreviousList = i > 0;
                listinfo.ListType = listType;
                Md.Paragraph listItem = document.LastSection.AddParagraph();
                listItem.Tag = "List~" + this.id;
                listItem.Format.KeepTogether = true;
                if(this.formatType== Font.Presets.None)
                {
                    listItem.RenderFragments(this.fragments[i]);
                }
                else
                {
                    listItem.AddText(this.fragments[i].FullText);
                    listItem.Format = this.font.ToMigraDocParagraphFormat(document.Styles["List"].ParagraphFormat.Clone());
                }
                listItem.Format.ListInfo = listinfo;
            }
        }

        public void RenderList(Md.Tables.Cell cell, Md.Document document)
        {
            Md.ListType listType = (Md.ListType)this.listType;
            for (int i = 0; i < this.fragments.Count; i++)
            {
                Md.ListInfo listinfo = new Md.ListInfo();
                listinfo.ContinuePreviousList = i > 0;
                listinfo.ListType = listType;
                listinfo.Tag = "List~" + this.id;
                Md.Paragraph listItem = cell.AddParagraph();
                if (this.formatType == Font.Presets.None)
                {
                    listItem.RenderFragments(this.fragments[i]);
                }
                else
                {
                    listItem.AddText(this.fragments[i].FullText);
                    listItem.Format = this.font.ToMigraDocParagraphFormat(document.Styles["List"].ParagraphFormat.Clone());
                }
                listItem.Format.ListInfo = listinfo;
            }
        }

        public void RenderTable(Md.Tables.Table table, Md.Document document, double containerWidth)
        {
            table.Tag = "Table~" + this.id;

            int colCount = data.Count;
            int rowCount = 0;
            double columnWidth = this.columnWidth;
            if (columnWidth < 0) columnWidth = containerWidth/colCount;

            table.TopPadding = 6;
            table.BottomPadding = 6;
            table.Format.Alignment = Md.ParagraphAlignment.Justify;
            Md.Shading shading = new Md.Shading();
            shading.Color = this.graphic.Color.ToMigraDoc();
            table.Format.Shading = shading;

            //Columns
            for (int i = 0; i < colCount; i++)
            {
                rowCount = Math.Max(rowCount, data[i].Contents.Count);
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
                DataSet d = new DataSet(data[i]);
                if (!d.HasColors) d.SetColors(this.FillColor);
                contents.Add(new List<Md.Paragraph>());
                for (int j = 0; j < rowCount; j++)
                {
                    Md.Tables.Cell cell = table.Rows[j].Cells[i];
                    cell.Shading.Color = d.Colors[j].ToMigraDoc();

                    Md.Paragraph txt = cell.AddParagraph(d.Contents[j]);
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
                        txtWidth = Math.Max(Math.Ceiling(txtWidth), Math.Ceiling(gfx.MeasureString(data[i].Contents[j], this.font.ToPdf()).Width));
                    }
                    table.Columns[i].Width = txtWidth + 8;
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
                table.Rows[0].Borders.Bottom.Width = table.Rows[0].Borders.Bottom.Width * 2;
            }

            if (HasYAxis)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    contents[0][i].Format.Font.Bold = true;
                    contents[0][i].Format.Font.Size = contents[0][i].Format.Font.Size * 1.25;
                }
                table.Columns[0].Borders.Right.Width = table.Columns[0].Borders.Right.Width * 2;
            }
        }

        public void RenderChart(Md.Shapes.Charts.Chart chart, Md.Document document, double width)
        {
            chart.Tag = "Chart~" + this.id;
            chart.Type = this.chartType.ToMigraDoc();

            chart.TopArea.TopPadding = 15;
            chart.BottomArea.BottomPadding = 15;
            chart.LeftArea.LeftPadding = 15;
            chart.RightArea.RightPadding = 15;

            //Chart Area
            chart.Left = this.Justification.ToMigraDocShapePosition();

            if (this.Width <= 0)
            {
                chart.Width = width;
            }
            else
            {
                chart.Width = this.Width;
            }

            if (this.Height <= 0)
            {
                chart.Height = chart.Width / 2.0;
                if (this.chartType == ChartTypes.Pie) chart.Height = chart.Width;
            }
            else
            {
                chart.Height = this.Height;
            }

            chart.Height = Math.Max(chart.Height, 75.00);

            if (this.graphic.HasColor) chart.FillFormat.Color = this.graphic.Color.ToMigraDoc();

            //Series
            if (this.chartType != ChartTypes.Pie)
            {
                foreach (DataSet d in this.data)
                {
                    if (d.IsNumeric)
                    {
                        Md.Shapes.Charts.Series series = chart.SeriesCollection.AddSeries();

                        series.Name = d.Title;
                        series.MarkerStyle = Md.Shapes.Charts.MarkerStyle.None;
                        series.Add(d.Values.ToArray());
                        if (d.Graphic.HasColor) series.FillFormat.Color = d.Graphic.Color.ToMigraDoc();
                        if (d.Graphic.HasStroke)
                        {
                            series.LineFormat.Width = d.Graphic.Weight;
                            series.LineFormat.Color = d.Graphic.Stroke.ToMigraDoc();
                        }

                        if (d.HasColors)
                        {
                            List<Md.Shapes.Charts.Point> elements = series.Elements.Cast<Md.Shapes.Charts.Point>().ToList();
                            for (int i = 0; i < elements.Count; i++)
                            {
                                elements[i].FillFormat.Color = d.Colors[i].ToMigraDoc();
                            }
                        }

                        if (d.LabelData)
                        {
                            series.HasDataLabel = true;

                            series.DataLabel.Position = d.LabelAlignment.ToMigraDoc();

                            series.DataLabel.Font = d.Font.ToMigraDoc();
                            if (d.Font.IsUnderlined) series.DataLabel.Font.Underline = Md.Underline.Single;
                        }
                        else
                        {
                            series.HasDataLabel = false;
                        }
                    }

                    //Axis
                    chart.XAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.None;
                    chart.XAxis.TickLabels.Font = this.font.ToMigraDoc();
                    if (this.HasXAxis)
                    {
                        chart.XAxis.Title.Alignment = Md.Shapes.Charts.HorizontalAlignment.Center;

                        chart.XAxis.Title.Caption = this.XAxis;
                        chart.XAxis.Title.Font = this.font.ToMigraDoc();
                        if (!this.IsChartHorizontal) chart.XAxis.Title.Orientation = 90;

                        if (this.graphic.HasStroke)
                        {
                            chart.XAxis.LineFormat.Color = this.graphic.Stroke.ToMigraDoc();
                            chart.XAxis.LineFormat.Width = this.graphic.Weight;
                        }
                    }

                    chart.XAxis.HasMajorGridlines = (this.horizontalBorderStyle != BorderStyles.None);
                    if ((int)this.chartType < 2) chart.XAxis.HasMajorGridlines = (this.verticalBorderStyle != BorderStyles.None);
                    if (chart.XAxis.HasMajorGridlines)
                    {
                        if (this.graphic.HasStroke)
                        {
                            chart.XAxis.MajorGridlines.LineFormat.Color = this.graphic.Stroke.ToMigraDoc();
                            chart.XAxis.MajorGridlines.LineFormat.Width = this.graphic.Weight;
                        }
                    }

                    //Axis Y
                    chart.YAxis.MajorTickMark = Md.Shapes.Charts.TickMarkType.None;
                    chart.YAxis.TickLabels.Font = this.font.ToMigraDoc();
                    chart.YAxis.TickLabels.Format = "#.####";
                    if (this.HasYAxis)
                    {
                        chart.YAxis.Title.VerticalAlignment = Md.Tables.VerticalAlignment.Center;
                        chart.YAxis.Title.Alignment = Md.Shapes.Charts.HorizontalAlignment.Center;

                        chart.YAxis.Title.Caption = this.YAxis;
                        chart.YAxis.Title.Font = this.font.ToMigraDoc();
                        if (this.IsChartHorizontal) chart.YAxis.Title.Orientation = 90;

                        if (this.graphic.HasStroke)
                        {
                            chart.YAxis.LineFormat.Color = this.graphic.Stroke.ToMigraDoc();
                            chart.YAxis.LineFormat.Width = this.graphic.Weight;
                        }
                    }

                    chart.YAxis.HasMajorGridlines = (this.verticalBorderStyle != BorderStyles.None);
                    if ((int)this.chartType < 2) chart.YAxis.HasMajorGridlines = (this.horizontalBorderStyle != BorderStyles.None);
                    if (chart.YAxis.HasMajorGridlines)
                    {
                        if (this.graphic.HasStroke)
                        {
                            chart.YAxis.MajorGridlines.LineFormat.Color = this.graphic.Stroke.ToMigraDoc();
                            chart.YAxis.MajorGridlines.LineFormat.Width = this.graphic.Weight;
                        }
                    }
                }
            }
            else
            {
                DataSet d = this.data[0];
                if (d.IsNumeric)
                {
                    Md.Shapes.Charts.Series series = chart.SeriesCollection.AddSeries();

                    series.Name = d.Title;
                    series.MarkerStyle = Md.Shapes.Charts.MarkerStyle.None;
                    series.Add(d.Values.ToArray());
                    if (d.Graphic.HasColor) series.FillFormat.Color = d.Graphic.Color.ToMigraDoc();
                    if (d.Graphic.HasStroke)
                    {
                        series.LineFormat.Width = d.Graphic.Weight;
                        series.LineFormat.Color = d.Graphic.Stroke.ToMigraDoc();
                    }

                    if ((int)this.chartType < 4)
                    {
                        if (d.HasColors)
                        {
                            List<Md.Shapes.Charts.Point> elements = series.Elements.Cast<Md.Shapes.Charts.Point>().ToList();
                            for (int i = 0; i < elements.Count; i++)
                            {
                                elements[i].FillFormat.Color = d.Colors[i].ToMigraDoc();
                            }
                        }

                    }

                    if (d.LabelData)
                    {
                        series.HasDataLabel = true;

                        series.DataLabel.Position = d.LabelAlignment.ToMigraDoc();

                        series.DataLabel.Font = d.Font.ToMigraDoc();
                        if (d.Font.IsUnderlined) series.DataLabel.Font.Underline = Md.Underline.Single;
                    }
                    else
                    {
                        series.HasDataLabel = false;
                    }
                }

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
                legend.Format.Font = this.font.ToMigraDoc();
            }

        }

        public void RenderImage(Md.Shapes.Image image, Md.Document document, double width = -1)
        {
            image.Tag = "Image~" + this.id;

            if ((this.width > 0) & (this.height > 0))
            {
                image.LockAspectRatio = false;
                image.Width = this.width;
                image.Height = this.height;
            }
            else if ((this.width < 0) & (this.height < 0))
            {
                image.LockAspectRatio = true;
                image.Width = width;
            }
            else
            {
                if (this.width > 0)
                {
                    image.LockAspectRatio = true;
                    image.Width = this.width;
                }
                else if (this.height > 0)
                {
                    image.LockAspectRatio = true;
                    image.Height = this.height;
                }
            }

            image.Left = this.Justification.ToMigraDocShapePosition();
        }

        public void RenderDrawing(Md.Shapes.TextFrame frame, Md.Document document, double width)
        {
            frame.Tag = "Drawing~" + this.id;

            Rg.BoundingBox box = this.drawing.BoundingBox;

            frame.Width = box.Diagonal.X;
            frame.Height = box.Diagonal.Y;
            if((this.width < 0)&(this.height <=0))
            {
                frame.Width = width;
                frame.Height = (box.Diagonal.Y / box.Diagonal.X) * width;
            }
            else if ((this.width > 0) & (this.height > 0))
            {
                frame.Width = this.width;
                frame.Height = this.height;
            }
            else if (this.width > 0)
            {
                frame.Width = this.width;
                frame.Height = box.Diagonal.Y * (this.Width / box.Diagonal.X);
            }
            else if (this.height > 0)
            {
                frame.Width = box.Diagonal.X * (this.Height / box.Diagonal.Y);
                frame.Height = this.height;
            }
            frame.Left = this.Justification.ToMigraDocShapePosition();
        }

        public void RenderDockHorizontal(Md.Tables.Table dock, Md.Document document, double width)
        {
            if (this.blocks.Count > 0)
            {
                dock.Tag = "DockH~" + this.id;

                dock.Format.Alignment = Md.ParagraphAlignment.Justify;

                int columns = this.blocks.Count;
                double columnWidth = width / columns;
                //Columns
                for (int i = 0; i < columns; i++)
                {
                    Md.Tables.Column column = dock.AddColumn();
                    column.Width = columnWidth;
                    column.Borders.Left.Visible = false;
                }

                Md.Tables.Row row = dock.AddRow();

                for (int i = 0; i < columns; i++)
                {
                    Block blk = this.blocks[i];
                    Md.Tables.Cell cell = dock.Rows[0].Cells[i];
                    RenderToCell(cell, document, blk, columnWidth);
                }
            }
        }

        public void RenderDockVertical(Md.Tables.Table dock, Md.Document document, double width)
        {
            if (this.blocks.Count > 0)
            {
                dock.Tag = "DockV~" + this.id;
                dock.Format.Alignment = Md.ParagraphAlignment.Justify;

                Md.Tables.Column column = dock.AddColumn();
                Md.Tables.Row row = dock.AddRow();
                Md.Tables.Cell cell = dock.Rows[0].Cells[0];
                column.Width = width;
                for (int i = 0; i < this.blocks.Count; i++)
                {
                    Block blk = this.blocks[i];
                    RenderToCell(cell, document, blk, width);
                }
            }
        }

        protected void RenderToCell(Md.Tables.Cell cell, Md.Document document, Block block, double width)
        {
            switch (block.blockType)
            {
                case BlockTypes.Text:
                    block.RenderText(cell.AddParagraph(), document);
                    break;
                case BlockTypes.Markdown:
                    block.RenderMarkdown(cell.Section, document);
                    break;
                case BlockTypes.Html:
                    block.RenderHtml(cell.Section, document);
                    break;
                case BlockTypes.List:
                    block.RenderList(cell, document);
                    break;
                case BlockTypes.Chart:
                    block.RenderChart(cell.AddChart(), document, width);
                    break;
                case BlockTypes.Drawing:
                    block.RenderDrawing(cell.AddTextFrame(), document, width);
                    break;
                case BlockTypes.Image:
                    block.RenderImage(cell.AddImage(block.imageName), document, width);
                    break;
                case BlockTypes.Table:
                    cell.Elements.Add(new Md.Tables.Table());
                    Md.Tables.Table table = (Md.Tables.Table)cell.Elements[cell.Elements.Count - 1];
                    block.RenderTable(table, document, width);
                    break;
                case BlockTypes.VerticalDock:
                    cell.Elements.Add(new Md.Tables.Table());
                    Md.Tables.Table group = (Md.Tables.Table)cell.Elements[cell.Elements.Count - 1];
                    block.RenderDockVertical(group, document, width);
                    break;
                case BlockTypes.HorizontalDock:
                    cell.Elements.Add(new Md.Tables.Table());
                    Md.Tables.Table dock = (Md.Tables.Table)cell.Elements[cell.Elements.Count - 1];
                    block.RenderDockHorizontal(dock, document, width);
                    break;
            }
        }

        public Md.Document RenderToDocument(Md.Document document)
        {
            double width = 0;
            if (document.Sections[0].PageSetup.Orientation == Md.Orientation.Landscape)
            {
                width = (document.Sections[0].PageSetup.PageHeight - document.Sections[0].PageSetup.LeftMargin - document.Sections[0].PageSetup.RightMargin);
            }
            else
            {
                width = (document.Sections[0].PageSetup.PageWidth - document.Sections[0].PageSetup.LeftMargin - document.Sections[0].PageSetup.RightMargin);
            }

            switch (this.blockType)
            {
                case BlockTypes.LineBreak:
                    #region linebreak
                    Md.Paragraph brk = document.LastSection.AddParagraph();
                    brk.Tag = "Linebreak~" + this.id;
                    for (int i = 0; i < this.breakCount; i++) brk.AddLineBreak();
                    #endregion
                    break;
                case BlockTypes.PageBreak:
                    #region pagebreak
                    for (int i = 0; i < this.breakCount; i++)
                    {
                        document.LastSection.AddPageBreak();
                        document.LastSection.Elements[document.LastSection.Elements.Count-1].Tag = "Linebreak~" + this.id;
                    }
                    #endregion
                    break;
                case BlockTypes.Text:
                    #region text
                    this.RenderText(document.LastSection.AddParagraph(),document);
                    #endregion
                    break;
                case BlockTypes.Markdown:
                    #region markdown
                    this.RenderMarkdown(document.LastSection, document);
                    #endregion
                    break;
                case BlockTypes.Html:
                    #region html
                    this.RenderHtml(document.LastSection, document);
                    #endregion
                    break;
                case BlockTypes.List:
                    #region list
                    this.RenderList(document);
                    #endregion
                    break;
                case BlockTypes.Table:
                    #region table
                        this.RenderTable(document.LastSection.AddTable(), document, width);
                    #endregion
                    break;
                case BlockTypes.Chart:
                    #region chart            
                    this.RenderChart(document.LastSection.AddChart(),document,width);
                    #endregion
                    break;
                case BlockTypes.Drawing:
                    #region drawing
                    this.RenderDrawing(document.LastSection.AddTextFrame(),document,width);
                    #endregion
                    break;
                case BlockTypes.Image:
                    #region image
                    this.RenderImage(document.LastSection.AddImage(this.imageName), document, width);
                    #endregion
                    break;
                case BlockTypes.HorizontalDock:
                    #region dock
                    this.RenderDockHorizontal(document.LastSection.AddTable(),document, width);
                    #endregion
                    break;
                case BlockTypes.VerticalDock:
                    #region group
                    this.RenderDockVertical(document.LastSection.AddTable(), document, width);
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
