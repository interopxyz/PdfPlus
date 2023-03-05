using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PdfPlus.Components.Write.Contents.Graphs
{
    public class GH_CreateClippingPlanesPlanes : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_CreateClippingPlanesPlanes class.
        /// </summary>
        public GH_CreateClippingPlanesPlanes()
          : base("CreateClippingPlanesPlanes", "CreatePP",
              "Creates the planes that will be used to generate the clipping planes",
              Constants.ShortName, Constants.WritePage)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("GeometryBase", "Geo", "Geometry that will be used to generate the planes", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Planes that will be used to generate the clipping Planes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("84A5EF11-E9D3-4423-85FA-245A7F876BA9"); }
        }
    }
}