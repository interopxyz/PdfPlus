using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace PdfPlus
{
    public class PdfPlusInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "PdfPlus";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.PdfPlus_24;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "A plugin for the creation of PDFs";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("77aa2e81-9cd7-4aee-8158-4b2d321aba6e");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "David Mans";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "interopxyz@gmail.com";
            }
        }

        public override string AssemblyVersion
        {
            get
            {
                return "1.3.2.0";
            }
        }
    }

    public class PdfPlusCategoryIcon : GH_AssemblyPriority
    {
        public object Properties { get; private set; }

        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.ComponentServer.AddCategoryIcon(Constants.ShortName, PdfPlus.Properties.Resources.PdfPlus_16);
            Instances.ComponentServer.AddCategorySymbolName(Constants.ShortName, 'P');
            return GH_LoadingInstruction.Proceed;
        }
    }
}