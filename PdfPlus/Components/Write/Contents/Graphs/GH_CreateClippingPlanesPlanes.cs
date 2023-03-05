using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using PdfPlus.Classes;
using Grasshopper;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
            pManager.AddGeometryParameter("GeometryBase", "Geo", "Geometry that will be used to generate the planes", GH_ParamAccess.tree);
            pManager.AddNumberParameter("InflateFactor", "F", "A double to inflate the the bounding box of your geometry", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Flip", "FP", "Flip the planes", GH_ParamAccess.item);
            // bool nZ, bool pZ, bool pX, bool nX, bool nY, bool pY
            pManager.AddBooleanParameter("NZ", "NZ", "Negative Z (Bot plane)", GH_ParamAccess.item);
            pManager.AddBooleanParameter("PZ", "PZ", "Positive Z (Top plane)", GH_ParamAccess.item);
            pManager.AddBooleanParameter("PX", "PX", "Positive X", GH_ParamAccess.item);
            pManager.AddBooleanParameter("NX", "NX", "Negative X", GH_ParamAccess.item);
            pManager.AddBooleanParameter("PY", "PY", "Positive Y", GH_ParamAccess.item);
            pManager.AddBooleanParameter("NY", "PY", "Positive Y", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Planes that will be used to generate the clipping Planes", GH_ParamAccess.tree);
        }

        List<Plane> planeList = new List<Plane>();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            planeList.Clear();
            GH_Structure<IGH_GeometricGoo> geos = new GH_Structure<IGH_GeometricGoo> ();


            DA.GetDataTree(0, out geos);

            double InfalteFactor = 0;
            DA.GetData(1, ref InfalteFactor);

            bool flip = false;
            DA.GetData(2, ref flip);

            bool NZ = false;
            DA.GetData(3, ref NZ);

            bool PZ = false;
            DA.GetData(4, ref PZ);

           bool PX = false;
            DA.GetData(5, ref PX);

            bool NX = false;
            DA.GetData(6, ref NX);

            bool PY = false;
            DA.GetData(7, ref PY);

            bool NY = false;
            DA.GetData(8, ref NY);

            GH_Structure<GH_Plane> TreePlanes = new GH_Structure<GH_Plane>();

            //int i = 0;
            //foreach (var branch in geos.Branches)
            //{
            //    GH_Path path = new GH_Path(i);
            //    foreach (var item in branch)
            //    {

            //        GeometryBase geo = null;
            //        item.CastTo<GeometryBase>(out geo);
            //        var bb = geo.GetBoundingBox(true);
            //        var planes = ScreenCaptureHelper.GetBBPlanes(bb, InfalteFactor, flip, NZ, PZ, PX, NX, NY, PY);
            //        planeList.AddRange(planes);
            //        List<GH_Plane> ghplanes = new List<GH_Plane>();
            //        foreach(Plane plane in planes) 
            //        {
            //            GH_Plane gH_Plane = new GH_Plane(plane);
            //            ghplanes.Add(gH_Plane);
            //        }
            //        TreePlanes.AppendRange(ghplanes, path);
            //    }
            //}

            for(int i = 0; i < geos.Branches.Count; i++) 
            {
                GH_Path path = new GH_Path(i);
                var branch = geos.Branches[i];
                for (int ii = 0; ii < branch.Count; ii++) 
                {
                    GeometryBase geo =  GH_Convert.ToGeometryBase(branch[ii]);
                    var bb = geo.GetBoundingBox(true);
                    var planes = ScreenCaptureHelper.GetBBPlanes(bb, InfalteFactor, flip, NZ, PZ, PX, NX, NY, PY);
                    planeList.AddRange(planes);
                    List<GH_Plane> ghplanes = new List<GH_Plane>();
                    foreach (Plane plane in planes)
                    {
                        GH_Plane gH_Plane = new GH_Plane(plane);
                        ghplanes.Add(gH_Plane);
                    }
                    TreePlanes.AppendRange(ghplanes, path);
                }
            }

            //var bb = geo.GetBoundingBox(true);
            //planeList = ScreenCaptureHelper.GetBBPlanes(bb, InfalteFactor, flip, NZ, PZ, PX, NX, NY, PY);


            DA.SetDataTree(0, TreePlanes);

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


        //Draw all wires and points in this method.
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            ScreenCaptureHelper.DrawClippingPlanesRectangles(args.Display, planeList);
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