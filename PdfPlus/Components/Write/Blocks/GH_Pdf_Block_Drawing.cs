using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Blocks
{
    public class GH_Pdf_Block_Drawing : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_BLock_Drawing class.
        /// </summary>
        public GH_Pdf_Block_Drawing()
          : base("Drawing Block", "Drw Blk",
              "Create a drawing Block from a list of geometry and text Shapes",
              Constants.ShortName, Constants.Blocks)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Input, GH_ParamAccess.list);
            pManager.AddIntegerParameter("Column Fitting", "F", "Table column width " + Environment.NewLine + "(-1 = Autofit Page" + Environment.NewLine + "0 = Autofit Content" + Environment.NewLine + "0 < Fixed Size", GH_ParamAccess.item, -1);
            pManager[1].Optional = true;

            Param_Integer paramC = (Param_Integer)pManager[1];
            paramC.AddNamedValue("Fit Width", -1);
            paramC.AddNamedValue("Fit Content", 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Block.Name, Constants.Block.NickName, Constants.Block.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_Goo> geometry = new List<IGH_Goo>();
            if (!DA.GetDataList(0, geometry)) return;

            List<Shape> shapes = new List<Shape>();
            foreach (IGH_Goo goo in geometry)
            {
                Shape shape = null;
                if (goo.TryGetShape(ref shape)) shapes.Add(shape); 
            }

            Block block = Block.CreateDrawing(shapes);

            int width = -1;
            if (DA.GetData(1, ref width)) block.Width = width;

            DA.SetData(0, block);
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
                return Properties.Resources.Pdf_Block_Draw;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a41fef5d-8f53-4866-a9ad-5dbf4db5cd64"); }
        }
    }
}