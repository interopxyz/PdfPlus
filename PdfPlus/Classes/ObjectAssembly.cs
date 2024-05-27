using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sd = System.Drawing;

using Rg = Rhino.Geometry;

namespace PdfPlus
{
    public class ObjectAssembly : Element
    {
        #region members

        //Text
        protected List<Fragment> fragments = new List<Fragment>();

        //Chart
        public enum ChartTypes { Column, ColumnStacked, Bar, BarStacked, Line, Area, Pie };
        protected ChartTypes chartType = ChartTypes.ColumnStacked;
        protected List<DataSet> data = new List<DataSet>();
        protected string xAxis = string.Empty;
        protected string yAxis = string.Empty;

        //Images
        protected string imageName = string.Empty;
        protected Sd.Bitmap imageObject = null;

        //Table
        protected bool hasAlternatingColor = false;
        protected Sd.Color alternateColor = Sd.Color.Transparent;

        //Geometry
        protected Rg.BoundingBox boundingBox = Rg.BoundingBox.Unset;
        protected Rg.Polyline polyline = null;
        protected Rg.Line line = Rg.Line.Unset;
        protected Rg.Circle circle = Rg.Circle.Unset;
        protected Rg.NurbsCurve curve = null;

        protected Rg.Brep brep = null;
        protected Rg.Mesh mesh = null;

        #endregion

        #region constructors

        public ObjectAssembly():base()
        {

        }

        public ObjectAssembly(Element element):base(element)
        {

        }

        public ObjectAssembly(ObjectAssembly assembly):base(assembly)
        {
            //Text
            foreach (Fragment fragment in assembly.fragments) this.fragments.Add(new Fragment(fragment));

            //DataSet
            SetData(assembly.data);

            //Table
            this.hasAlternatingColor = assembly.hasAlternatingColor;
            this.alternateColor = assembly.alternateColor;

            //Chart
            this.chartType = assembly.chartType;
            this.xAxis = assembly.xAxis;
            this.yAxis = assembly.yAxis;

            //Image
            this.imageName = assembly.imageName;
            if (assembly.imageObject != null) this.imageObject = new Sd.Bitmap(assembly.imageObject);

            //Geoemtry
            if(assembly.boundingBox.IsValid)this.boundingBox = new Rg.BoundingBox(assembly.boundingBox.GetCorners());
            if (assembly.polyline!=null)this.polyline = assembly.polyline.Duplicate();
            if (assembly.line.IsValid) this.line = new Rg.Line(assembly.line.From, assembly.line.To);
            if (assembly.curve != null) this.curve = new Rg.NurbsCurve(assembly.curve);
            if (assembly.circle.IsValid) this.circle = new Rg.Circle(assembly.circle.Plane, assembly.circle.Radius);
            if (assembly.brep != null) this.brep = assembly.brep.DuplicateBrep();
            if (assembly.mesh != null) this.mesh = assembly.mesh.DuplicateMesh();

        }

        #endregion

        #region properties

        //Text
        public override string Text { get => this.FragmentText(); set => this.fragments = new List<Fragment> { new Fragment(value) }; }

        public override Font Font
        {
            get { return new Font(this.font); }
            set {
                for (int i = 0; i < this.fragments.Count; i++) this.fragments[i].Font = value;
                this.font = new Font(value); 
            }
        }
        public override Graphic Graphic
        {
            get { return this.graphic; }
            set {
                for (int i = 0; i < this.fragments.Count; i++) this.fragments[i].Graphic = value;
                this.graphic = new Graphic(value); 
            }
        }

        public virtual List<Fragment> Fragments
        {
            get
            {
                List<Fragment> output = new List<Fragment>();
                foreach (Fragment fragment in fragments) output.Add(new Fragment(fragment));
                return fragments;
            }
        }

        public virtual Sd.Color AlternateColor
        {
            get { return this.alternateColor; }
            set
            {
                this.hasAlternatingColor = true;
                this.alternateColor = value;
            }
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

        public virtual bool IsChartHorizontal
        {
            get { return ((int)this.chartType < 2); }
        }

        public virtual Rg.BoundingBox BoundingBox
        {
            get {
                if(this.boundingBox.IsValid) return new Rg.BoundingBox(this.boundingBox.GetCorners());
                return Rg.BoundingBox.Unset;
            }
        }

        public virtual Rg.Line Line
        {
            get {
                if(this.line.IsValid)return new Rg.Line(this.line.From, this.line.To);
                return Rg.Line.Unset;
            }
        }

        public virtual Rg.Ellipse Ellipse
        {
            get { 
                if(this.boundary.IsValid)return new Rg.Ellipse(this.boundary.Plane, this.boundary.Width, this.boundary.Height);
                return new Rg.Ellipse();
            }
        }

        public virtual Rg.Polyline Polyline
        {
            get { 
                if(this.polyline !=null) return new Rg.Polyline(this.polyline);
                return null;
            }
        }

        public virtual Rg.Circle Circle
        {
            get { 
                if(this.circle.IsValid) return new Rg.Circle(this.circle.Plane, this.circle.Radius);
                return Rg.Circle.Unset;
            }
        }

        public virtual Rg.Brep Brep
        {
            get { 
                if(this.brep!=null) return this.brep.DuplicateBrep();
                return null;
            }
        }

        public virtual Rg.Mesh Mesh
        {
            get { 
                if(this.mesh!=null)return this.mesh.DuplicateMesh();
                return null;
            }
        }

        public virtual Rg.NurbsCurve Bezier
        {
            get
            {
                if (this.curve != null)
                {
                    Rg.NurbsCurve nurbs = this.curve.ToNurbsCurve();
                    nurbs.MakePiecewiseBezier(true);
                    return nurbs.DuplicateCurve().ToNurbsCurve();
                }
                return null;
            }
        }

        public virtual Sd.Bitmap Image
        {
            get { if(this.imageObject!=null) return new Sd.Bitmap(this.imageObject);
                return null;
            }
        }

        public virtual string ImagePath
        {
            get { return imageName; }
        }

        public virtual List<DataSet> Data
        {
            get { return this.data; }
        }

        #endregion

        #region methods

        private string FragmentText()
        {
            string output = "";
            foreach (Fragment fragment in this.fragments) output += fragment.FullText;
            return output;
        }

        public void SetData(List<DataSet> data)
        {
            foreach (DataSet d in data)
            {
                this.data.Add(new DataSet(d));
            }
        }

        public void SetData(DataSet data)
        {
            this.data.Add(new DataSet(data));
        }

        public void AddFragment(Fragment fragment)
        {
            this.fragments.Add(new Fragment(fragment));
        }

        public void AddFragment(List<Fragment> fragments)
        {
            foreach (Fragment fragment in fragments) this.fragments.Add(new Fragment(fragment));
        }

        public List<string> BreakLines(string txt, double Width)
        {
            //Credit: THE FOLLOWING FUNCTION IS MODIFIED FROM ONE WRITTEN BY DAVID RUTTEN https://github.com/DavidRutten

            if (string.IsNullOrWhiteSpace(txt)) return null;

            if (string.IsNullOrWhiteSpace(this.FontFamily))
                throw new ArgumentException("Font name not specified.");

            if (this.FontSize <= 1.0)
                throw new ArgumentException("Size needs to be at least 1.0, because the text measuring uses integer arithmetic.");

            if (Width < this.FontSize * 2)
                throw new ArgumentException("Container width must be at least 10 times the font size.");



            Sd.Font font = new Sd.Font(this.FontFamily, (float)this.FontSize, Sd.FontStyle.Regular, Sd.GraphicsUnit.World);

            string[] phrases = txt .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> words = new List<string>();
            foreach(string phrase in phrases)
            {
                if (phrase.Contains(Environment.NewLine))
                {
                    string[] subtext = phrase.Trim().Split( new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    //words.Add(Environment.NewLine);
                    foreach (string t in subtext)
                    {
                        if (t == "")
                        {
                            words.Add(Environment.NewLine);
                        }
                        else
                        {
                            words.Add(t);
                        }
                    }
                }
                else
                {
                    words.Add(phrase.Trim());
                }
            }

            Queue<string> queue = new Queue<string>(words);

            List<string> lines = new List<string>();
            while (queue.Count > 0)
            {
                string txtLine = ExtractLine(queue, font, Width);
                    txtLine += " ";
                    lines.Add(txtLine);
            }

            font.Dispose();
            return lines;
        }

        private string ExtractLine(Queue<string> words, Sd.Font font, double maxWidth)
        {
            //Credit: THE FOLLOWING FUNCTION IS MODIFIED FROM ONE WRITTEN BY DAVID RUTTEN https://github.com/DavidRutten

            // Try the first word. If it is already longer than the maximum width, return immediately.
            string line = words.Dequeue();
            double width = Grasshopper.Kernel.GH_FontServer.StringWidth(line, font);
            if (words.Count < 1) return line;
            if (width >= maxWidth)
                return line;
            if (line == Environment.NewLine) return line;

            // Now add subsequent words one at a time.
            while (true)
            {
                // No words left.
                if (words.Count == 0)
                    return line;

                if (words.Peek() == Environment.NewLine) return line;

                string longerLine = line + " " + words.Peek();
                
                width = Grasshopper.Kernel.GH_FontServer.StringWidth(longerLine, font);
                if (width >= maxWidth)
                    return line;

                // We can fit the longer line.
                // Remeber the new line and properly dequeue the appended word.
                line = longerLine;
                words.Dequeue();
            }
        }

        #endregion

    }
}
