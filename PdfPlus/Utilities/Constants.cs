using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfPlus
{
    public class Constants
    {

        #region naming

        public static string LongName
        {
            get { return ShortName + " v" + Major + "." + Minor; }
        }

        public static string ShortName
        {
            get { return "PDF"; }
        }

        private static string Minor
        {
            get { return typeof(Constants).Assembly.GetName().Version.Minor.ToString(); }
        }
        private static string Major
        {
            get { return typeof(Constants).Assembly.GetName().Version.Major.ToString(); }
        }

        public static string SubDoc
        {
            get { return "PDF"; }
        }

        public static string SubPage
        {
            get { return "PDF"; }
        }

        #endregion

        #region 

        public static string WritePage
        {
            get { return "PDF"; }
        }

        public static string ReadPage
        {
            get { return "Read PDF"; }
        }

        public static Descriptor Shape
        {
            get { return new Descriptor("Shape", "Sh", "PDF Shape", "PDF Shape", "PDF Shapes"); }
        }

        public static Descriptor Text
        {
            get { return new Descriptor("Text", "Tx", "PDF Text", "PDF Text", "PDF Text"); }
        }

        public static Descriptor Units
        {
            get { return new Descriptor("Units", "U", "PDF Units", "PDF Units", "PDF Units"); }
        }

        public static Descriptor Page
        {
            get { return new Descriptor("Page", "Pg", "A PDF Page", "A PDF Page", "PDF Pages"); }
        }

        public static Descriptor Document
        {
            get { return new Descriptor("Document", "Dc", "A PDF Document", "A PDF Document", "PDF Documents"); }
        }

        #endregion
    }

    public class Descriptor
    {
        private string name = string.Empty;
        private string nickname = string.Empty;
        private string input = string.Empty;
        private string output = string.Empty;
        private string outputs = string.Empty;

        public Descriptor(string name, string nickname, string input, string output, string outputs)
        {
            this.name = name;
            this.nickname = nickname;
            this.input = input;
            this.output = output;
            this.outputs = outputs;
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual string NickName
        {
            get { return nickname; }
        }

        public virtual string Input
        {
            get { return input; }
        }

        public virtual string Output
        {
            get { return output; }
        }

        public virtual string Outputs
        {
            get { return outputs; }
        }
    }
}
