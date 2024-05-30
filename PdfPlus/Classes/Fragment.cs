using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfPlus
{
    public class Fragment : Element
    {

        #region members

        List<Element> segments = new List<Element>();

        #endregion

        #region constructors

        public Fragment() : base()
        {
            this.elementType = ElementTypes.Fragment;
        }

        public Fragment(Fragment fragment) : base(fragment)
        {
            foreach (Element element in fragment.segments) this.segments.Add(new Element(element));
        }

        public Fragment(List<Element> elements) : base()
        {
            foreach (Element element in elements) this.segments.Add(new Element(element));
        }

        public Fragment(List<Fragment> fragments) : base()
        {
            foreach (Fragment fragment in fragments)
            {
                foreach (Element element in fragment.segments) this.segments.Add(new Element(element));
            }
        }

        public Fragment(string content) : base()
        {
            this.elementType = ElementTypes.Fragment;
            Element element = new Element();
            element.Text = content;
            this.segments.Add(element);
        }

        public Fragment(List<string> content) : base()
        {
            this.elementType = ElementTypes.Fragment;
            foreach (string text in content)
            {
                Element element = new Element();
                element.Text = text;
                this.segments.Add(element);
            }
        }

        public Fragment(string content, Font font) : base()
        {
            this.elementType = ElementTypes.Fragment;
            Element element = new Element();
            element.Text = content;
            element.Font = font;
            this.segments.Add(element);
        }

        public Fragment(List<string> content, Font font) : base()
        {
            this.elementType = ElementTypes.Fragment;
            foreach (string text in content)
            {
                Element element = new Element();
                element.Text = text;
                element.Font = font;
                this.segments.Add(element);
            }
        }

        public Fragment(List<string> content, List<Font> fonts) : base()
        {
            this.elementType = ElementTypes.Fragment;
            int i = 0;
            int c = content.Count;
            foreach (string text in content)
            {
                Element element = new Element();
                element.Text = text;
                element.Font = fonts[i % c];
                this.segments.Add(element);
                i++;
            }
        }

        #endregion

        #region properties

        public virtual List<Element> Segments
        {
            get
            {
                List<Element> output = new List<Element>();
                foreach (Element element in this.segments)
                {
                    output.Add(new Element(element));
                }
                return this.segments; }
        }

        public virtual string FullText
        {
            get { return String.Join("", this.Texts.ToArray()); }
        }

        public virtual List<string> Texts
        {
            get
            {
                List<string> output = new List<string>();
                foreach (Element element in this.segments)
                {
                    output.Add(element.Text);
                }
                return output;
            }
        }

        public override Font Font
        { 
            get { return new Font(this.font); }
            set
            {
                for (int i = 0; i < this.segments.Count; i++) segments[i].Font = new Font(value);
                this.font = new Font(value); 
            }
    }

        public override Graphic Graphic
        {
            get { return new Graphic(this.graphic); }
            set
            {
                for (int i = 0; i < this.segments.Count; i++) segments[i].Graphic = new Graphic(value);
                this.graphic = new Graphic(value);
            }
        }

        public override BorderStyles HorizontalBorderStyle
        {
            get { return this.horizontalBorderStyle; }
            set
            {
                for (int i = 0; i < this.segments.Count; i++) segments[i].HorizontalBorderStyle = value;
                this.horizontalBorderStyle = value; 
            }
        }

        public override BorderStyles VerticalBorderStyle
        {
            get { return this.verticalBorderStyle; }
            set
            {
                for (int i = 0; i < this.segments.Count; i++) segments[i].VerticalBorderStyle = value;
                this.verticalBorderStyle = value; 
            }
        }

        public virtual List<Font> Fonts
        {
            get
            {
                List<Font> output = new List<Font>();
                foreach (Element element in this.segments) output.Add(new Font(element.Font));
                return output;
            }
        }

        #endregion

        #region methods

        public void AddFragments(Fragment fragment)
        {
            foreach(Element element in fragment.segments) this.segments.Add(new Element(element));
        }

        public void AddFragments(List<Fragment> fragments)
        {
            foreach(Fragment fragment in fragments)this.AddFragments(fragment);
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            if (this.FullText.Length > 25)
            {
                return "Fragment | (" + this.FullText.Substring(0, 24)+"...)";
            }
            else
            {
                return "Fragment | (" + this.FullText + ")";
            }
        }

        #endregion

    }
}
