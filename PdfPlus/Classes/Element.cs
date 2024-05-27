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

        public string id = Guid.NewGuid().ToString();
        public enum ElementTypes { Empty, Shape, Block, Data, Fragment};
        protected ElementTypes elementType = ElementTypes.Empty;

        public enum BorderStyles { None, Interior, All,Start,End};
        protected BorderStyles horizontalBorderStyle = BorderStyles.None;
        protected BorderStyles verticalBorderStyle = BorderStyles.None;

        //Formatting
        protected Graphic graphic = new Graphic();
        protected Font font = new Font();

        protected Alignment alignment = Alignment.None;
        protected Justification justification = Justification.Left;

        protected double angle = 0;

        //Text
        protected string text = string.Empty;

        //Geometry
        protected Rg.Rectangle3d boundary = Rg.Rectangle3d.Unset;
        protected Rg.Point3d location = Rg.Point3d.Unset;

        #endregion

        #region constructors

        public Element()
        {
            this.id = Guid.NewGuid().ToString();
        }

        public Element(Element element)
        {
            this.id = element.id;
            this.elementType = element.elementType;

            //Formatting
            this.alignment = element.alignment;
            this.justification = element.justification;

            this.graphic = new Graphic(element.graphic);
            this.font = new Font(element.font);

            this.horizontalBorderStyle = element.horizontalBorderStyle;
            this.verticalBorderStyle = element.verticalBorderStyle;

            this.angle = element.angle;
            //Text
            this.text = element.text;

            //Geoemtry
            this.boundary = new Rg.Rectangle3d(element.boundary.Plane, element.boundary.Corner(0), element.boundary.Corner(2));
            this.location = new Rg.Point3d(element.location);

        }

        #endregion

        #region properties

        public virtual string ID
        {
            get { return this.id; }
        }

        //Text
        public virtual string Text
        {
            get { return this.text; }
            set { this.text = value; }
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

        public virtual double Angle
        {
             get{ return this.angle; }
            set { this.angle = value; }
        }

        //Graphics
        public virtual Graphic Graphic
        {
            get { return new Graphic(this.graphic); }
            set { this.graphic = new Graphic(value); }
        }

        //Fonts
        public virtual Font Font
        {
            get { return new Font(this.font); }
            set { this.font = new Font(value); }
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

        //Table
        public virtual BorderStyles HorizontalBorderStyle
        {
            get { return this.horizontalBorderStyle; }
            set { this.horizontalBorderStyle = value; }
        }

        public virtual BorderStyles VerticalBorderStyle
        {
            get { return this.verticalBorderStyle; }
            set { this.verticalBorderStyle = value; }
        }

        //Geometry
        public virtual Rg.Point3d Location
        {
            get { 
                if(this.location.IsValid)return new Rg.Point3d(this.location);
                return Rg.Point3d.Unset;
            }
        }

        public virtual Rg.Rectangle3d PreviewBoundary
        {
            get
            {
                if(this.boundary.IsValid)return new Rg.Rectangle3d(this.boundary.Plane, this.boundary.X, this.boundary.Y);
                return Rg.Rectangle3d.Unset;
            }
        }

        public virtual Rg.Polyline PreviewPolyline
        {
            get {
                if(this.boundary.IsValid)return this.boundary.ToNurbsCurve().Points.ControlPolygon();
                return null;
            }
        }

        public virtual Rg.Rectangle3d Boundary
        {
            get { 
                if(this.boundary.IsValid)return new Rg.Rectangle3d(this.boundary.Plane,this.boundary.Width,this.boundary.Height) ;
                return Rg.Rectangle3d.Unset;
            }
        }

        #endregion

        #region methods

        public void SetBoundary(Rg.Rectangle3d boundary)
        {
            this.boundary = new Rg.Rectangle3d(boundary.Plane,boundary.X, boundary.Y);
        }

        #endregion

    }
}
