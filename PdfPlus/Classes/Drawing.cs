using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ps = PdfSharp;
using Pd = PdfSharp.Drawing;
using Rg = Rhino.Geometry;

namespace PdfPlus
{
    public class Drawing
    {
        #region members

        protected List<Shape> shapes = new List<Shape>();

        #endregion

        #region constructors

        public Drawing(List<Shape> shapes)
        {
            foreach (Shape shape in shapes)
            {
                this.shapes.Add(new Shape(shape));
            }
        }

        public Drawing(Drawing drawing)
        {
            foreach(Shape shape in drawing.shapes)
            {
                this.shapes.Add(new Shape(shape));
            }
        }

        #endregion

        #region properties



        #endregion

        #region methods

        public virtual Rg.BoundingBox BoundingBox
        {
            get
            {
                Rg.BoundingBox box = Rg.BoundingBox.Unset;
                foreach (Shape shape in this.shapes) box.Union(shape.Boundary.BoundingBox);
                return box;
                    }
        }

        public Pd.XGraphics Render(Pd.XGraphics graph, Rg.Rectangle3d rectangle)
        {

            Rg.BoundingBox box = this.BoundingBox;

            double factor = 0;

            //if (box.Diagonal.X > 0) factor = Math.Min(factor, rectangle.Width / box.Diagonal.X);
            if ((box.Diagonal.Y > 0)&(box.Diagonal.X > 0)) factor = Math.Min(rectangle.Width / box.Diagonal.X, rectangle.Height / box.Diagonal.Y );

            if (factor == 0) factor = 1;
            Rg.Plane plane = Rg.Plane.WorldZX;
            plane.Origin = box.Center;
            Rg.Transform mirror = Rg.Transform.Mirror(plane);
            Rg.Transform scale = Rg.Transform.Scale(plane.Origin, factor);

            Rg.Point3d a = box.Center;
            Rg.Point3d b = rectangle.Center;
            Rg.Transform translate = Rg.Transform.Translation(new Rg.Vector3d(b - a));

            foreach (Shape shape in this.shapes)
            {
                shape.Scale = factor;
                shape.Transform(translate*scale*mirror).RenderGeometry(graph);
            }

            return graph;
        }

        #endregion

    }
}
