using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

using Rg = Rhino.Geometry;

namespace PdfPlus
{
    public class Element
    {

        #region members

        public enum ElementTypes { Empty, Shape, Block, Data}
        protected ElementTypes elementType = ElementTypes.Empty;

        //Formatting
        protected Graphic graphic = new Graphic();
        protected Font font = new Font();

        protected Alignment alignment = Alignment.None;
        protected Justification justification = Justification.Left;

        //Text
        protected string text = string.Empty;

        //Chart
        public enum ChartTypes { Bar, BarStacked, Column, ColumnStacked, Line, Area, Pie };
        protected ChartTypes chartType = ChartTypes.ColumnStacked;
        protected List<DataSet> data = new List<DataSet>();
        protected string xAxis = string.Empty;
        protected string yAxis = string.Empty;

        //Images
        protected string imageName = string.Empty;
        protected Sd.Bitmap imageObject = null;

        //Geometry
        protected Rg.Point3d location = new Rg.Point3d();
        protected Rg.Polyline polyline = new Rg.Polyline();
        protected Rg.Line line = new Rg.Line();
        protected Rg.Arc arc = new Rg.Arc();
        protected Rg.Circle circle = new Rg.Circle();
        protected Rg.Rectangle3d boundary = new Rg.Rectangle3d();
        protected Rg.NurbsCurve curve = new Rg.NurbsCurve(3, 2);

        protected Rg.Brep brep = new Rg.Brep();
        protected Rg.Mesh mesh = new Rg.Mesh();

        #endregion

        #region constructors

        public Element()
        {

        }

        public Element(Element element)
        {
            this.elementType = element.elementType;

            //Formatting
            this.alignment = element.alignment;
            this.justification = element.justification;

            this.graphic = new Graphic(element.graphic);
            this.font = new Font(element.font);

            //Text
            this.text = element.text;

            //Chart
            this.chartType = element.chartType;
            SetData(element.data);
            this.xAxis = element.xAxis;
            this.yAxis = element.yAxis;

            //Image
            this.imageName = element.imageName;
            if (element.imageObject != null) this.imageObject = new Sd.Bitmap(element.imageObject);

            //Geoemtry
            this.location = new Rg.Point3d(element.location);
            this.polyline = element.polyline.Duplicate();
            this.boundary = new Rg.Rectangle3d(element.boundary.Plane, element.boundary.Corner(0), element.boundary.Corner(2));
            this.line = new Rg.Line(element.line.From, element.line.To);
            this.curve = new Rg.NurbsCurve(element.curve);
            this.circle = new Rg.Circle(element.circle.Plane, element.circle.Radius);
            this.brep = element.brep.DuplicateBrep();
            this.mesh = element.mesh.DuplicateMesh();

        }

        #endregion

        #region properties

        //Text
        public virtual string Text
        {
            get { return this.text; }
        }

        public virtual ElementTypes ElementType { get { return elementType; } }

        public virtual Alignment Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }

        public virtual Justification Justification
        {
            get { return this.justification; }
            set { this.justification = value; }
        }

        //Graphics
        public virtual Graphic Graphic
        {
            get { return new Graphic(graphic); }
            set { this.graphic = new Graphic(value); }
        }

        //Fonts
        public virtual Font Font
        {
            get { return new Font(font); }
            set { this.font = value; }
        }

        public virtual Sd.Color FontColor
        {
            get { return this.font.Color; }
            set { this.font.Color = value; }
        }

        public virtual string FontFamily
        {
            get { return this.font.Family; }
            set { this.font.Family = value; }
        }

        public virtual double FontSize
        {
            get { return this.font.Size; }
            set { this.font.Size = value; }
        }

        public virtual bool IsBold
        {
            get { return this.font.IsBold; }
        }

        public virtual bool IsItalic
        {
            get { return this.font.IsItalic; }
        }

        public virtual bool IsUnderlined
        {
            get { return this.font.IsUnderlined; }
        }

        public virtual bool IsStrikeout
        {
            get { return this.font.IsStrikeout; }
        }

        public virtual Sd.Color FillColor
        {
            get { return this.graphic.Color; }
            set { this.graphic.Color = value; }
        }

        public virtual Sd.Color StrokeColor
        {
            get { return this.graphic.Stroke; }
            set { this.graphic.Stroke = value; }
        }

        public virtual double StrokeWeight
        {
            get { return this.graphic.Weight; }
            set { this.graphic.Weight = value; }
        }

        public virtual FontStyle Style
        {
            get { return this.font.Style; }
            set { this.font.Style = value; }
        }

        //Chart
        public virtual ChartTypes ChartType
        {
            get { return chartType; }
        }

        public virtual string XAxis
        {
            get { return this.xAxis; }
            set { this.xAxis = value; }
        }

        public virtual string YAxis
        {
            get { return this.yAxis; }
            set { this.yAxis = value; }
        }

        public virtual bool HasXAxis
        {
            get { return (this.xAxis != string.Empty); }
        }

        public virtual bool HasYAxis
        {
            get { return (this.yAxis != string.Empty); }
        }

        //Geometry
        public virtual Rg.Point3d Location
        {
            get { return this.location; }
        }

        public virtual Rg.Rectangle3d Boundary
        {
            get { return this.boundary; }
        }

        public virtual Rg.Line Line
        {
            get { return this.line; }
        }

        public virtual Rg.Arc Arc
        {
            get { return this.arc; }
        }

        public virtual Rg.Ellipse Ellipse
        {
            get { return new Rg.Ellipse(this.boundary.Plane, this.boundary.Width, this.boundary.Height); }
        }

        public virtual Rg.Polyline Polyline
        {
            get { return this.polyline; }
        }
        public virtual Rg.Circle Circle
        {
            get { return this.circle; }
        }
        public virtual Rg.Brep Brep
        {
            get { return this.brep; }
        }

        public virtual Rg.Mesh Mesh
        {
            get { return this.mesh; }
        }

        public virtual Rg.NurbsCurve Bezier
        {
            get
            {
                Rg.NurbsCurve nurbs = this.curve.ToNurbsCurve();
                nurbs.MakePiecewiseBezier(true);
                return nurbs;
            }
        }

        public virtual Sd.Bitmap Image
        {
            get { return new Sd.Bitmap(this.imageObject); }
        }

        public virtual string ImagePath
        {
            get { return imageName; }
        }

        #endregion

        #region methods

        public void SetData(List<DataSet> data)
        {
            foreach (DataSet d in data)
            {
                this.data.Add(new DataSet(d));
            }
        }

        #endregion

    }
}
