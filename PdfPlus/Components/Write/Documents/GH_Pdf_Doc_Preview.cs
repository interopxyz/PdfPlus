using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Documents
{
    public class GH_Pdf_Doc_Preview : GH_Pdf__Base
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Doc_Preview class.
        /// </summary>
        public GH_Pdf_Doc_Preview()
          : base("Preview PDF", "Prev PDF",
              "Preview PDF Shapes, Pages, or Documents.",
              Constants.ShortName, Constants.Documents)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Element.Name, Constants.Element.NickName, "A PDF+ Shape, Block, DataSet, Text Fragment Element, or Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Document doc = new Document();
            IGH_Goo goo = null;
            if (DA.GetData(0, ref goo))
                {
                if(goo.TryGetDocument(ref doc))
                {
                    this.PrevDocumentShapes(doc);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Pdf_Document_Preview;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d9baa330-a6ee-4e7a-9ddc-a4491d2be7af"); }
        }
    }
}