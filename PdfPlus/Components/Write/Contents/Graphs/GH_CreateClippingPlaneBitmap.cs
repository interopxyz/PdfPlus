using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using PdfPlus.Classes;
using Grasshopper;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing; 

namespace PdfPlus.Components.Write.Contents.Graphs
{
    public class GH_CreateClippingPlaneBitmap : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_CreateClippingPlaneBitmap class.
        /// </summary>
        public GH_CreateClippingPlaneBitmap()
          : base("CreateClippingPlaneBitmap", "CPPB",
              "Creates a bitmap of a certain geo based on its clipping planes props",
               Constants.ShortName, Constants.WritePage)
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("GeometryBase", "Geo", "Geometry that will be used to generate the planes", GH_ParamAccess.tree);
            pManager.AddPlaneParameter("Planes", "P", "Planes that will be used to generate the clipping Planes", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Camera Direction", "CameraDir", "The direction of the camera", GH_ParamAccess.item);
            pManager.AddIntegerParameter("BitmapHeight","BitmapHeight", "BitmapHeight",GH_ParamAccess.item);
            pManager.AddIntegerParameter("BitmapWidth", "BitmapWidth", "BitmapWidth", GH_ParamAccess.item);
            pManager.AddBooleanParameter("parallelprojection", "parallelprojection", "parallelprojection", GH_ParamAccess.item);

            // public static Bitmap GetBitmap(GeometryBase geometry , List<Plane> clippingPlanePlanes ,  Vector3d CameraDirection , int bitmapHeight , int bitmapWidth,bool parallelprojection) 
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Bitmaps", "Bitmaps", "Bitmaps", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_GeometricGoo> geos = new GH_Structure<IGH_GeometricGoo>();
            DA.GetDataTree(0, out geos);

            GH_Structure<GH_Plane> planes = new GH_Structure<GH_Plane>();
            DA.GetDataTree(1, out planes);

            GH_Vector gH_Vector = new GH_Vector();
            DA.GetData(2, ref gH_Vector);
            Vector3d cameraDirection = gH_Vector.Value;

            GH_Integer bitH = new GH_Integer();
            DA.GetData(3, ref bitH);
            int bitmapHeight = bitH.Value;  


            GH_Integer bitW = new GH_Integer();
            DA.GetData(4, ref bitW);
            int bitmapWidth = bitW.Value;   


            GH_Boolean gH_Boolean = new GH_Boolean();
            DA.GetData(5, ref gH_Boolean);
            bool para = gH_Boolean.Value;

            List<Bitmap> bitmaps = new List<Bitmap>();

            for (int i = 0; i < geos.Branches.Count; i++)
            {
                GH_Path path = new GH_Path(i);
                var geobranch = geos.Branches[i];
                var planeBranch = planes.Branches[i];
                var planesList = new List<Plane>();
                foreach (var ghplane in planeBranch)
                {
                    planesList.Add(ghplane.Value);
                }
                for (int ii = 0; ii < geobranch.Count; ii++)
                {
                    GeometryBase geo = GH_Convert.ToGeometryBase(geobranch[ii]);
                    var bb = geo.GetBoundingBox(true);
                    Bitmap bitmap = ScreenCaptureHelper.GetBitmap(geo, planesList, cameraDirection, bitmapHeight, bitmapWidth, para);
                    bitmaps.Add(bitmap);
                }
            }

            DA.SetDataList(0, bitmaps); 


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
            get { return new Guid("41C672D8-845B-4D0A-8E66-402AB657C540"); }
        }
    }
}