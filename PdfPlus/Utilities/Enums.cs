using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfPlus
{
    public enum Position { TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight }
    public enum Justification { None, Left, Center, Right, Justify };
    public enum Alignment { None, Top, Middle, Bottom };
    public enum Location { None, Top, Bottom, Middle, Left, Right, Center };
    public enum FontStyle { Regular, Bold, BoldItalic, Italic, Strikeout, Underline };
    public enum Units { Millimeter, Centimeter, Inch, Point };
    public enum SizesA { A0, A1, A2, A3, A4, A5 };
    public enum SizesB { B0, B1, B2, B3, B4, B5 };
    public enum SizesImperial { Letter, Legal, Ledger, Statement, Tabloid };
    public enum PageLayouts { Single, SingleScroll, Double, DoubleScroll, DoubleCover, DoubleCoverScroll };
    public enum PageOrientation { Default, Landscape, Portrait };
}
