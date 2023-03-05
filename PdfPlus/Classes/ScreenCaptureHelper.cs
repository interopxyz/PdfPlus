using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Display;
using System.Windows.Forms;
using Grasshopper.Kernel.Types.Transforms;
using System.Drawing;

namespace PdfPlus.Classes
{
    public class ScreenCaptureHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bb"></param>
        /// <param name="inflateFactor"></param>
        /// <param name="flip"></param>
        /// <param name="nZ"></param>
        /// <param name="pZ"></param>
        /// <param name="pX"></param>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        public static List<Plane> GetBBPlanes(BoundingBox bb, double inflateFactor, bool flip, bool nZ, bool pZ, bool pX, bool nX, bool nY, bool pY)
        {
            List<Plane> planes = new List<Plane>();
            bb.Inflate(inflateFactor);
            var BBpts = bb.GetCorners();

            double flipFactor = 0;
            if (flip)
            {
                flipFactor = -1;
            }
            else
            {
                flipFactor = 1;
            }

            if (nZ)
            {
                var botPts = new List<Point3d>();
                botPts.Add(BBpts[0]);
                botPts.Add(BBpts[1]);
                botPts.Add(BBpts[2]);
                botPts.Add(BBpts[3]);

                var botPt = GetPointAvg(botPts);
                var botPlane = new Plane(botPt, Vector3d.ZAxis * -1 * flipFactor);
                planes.Add(botPlane);
            }

            if (pZ)
            {
                var topPts = new List<Point3d>();
                topPts.Add(BBpts[4]);
                topPts.Add(BBpts[5]);
                topPts.Add(BBpts[6]);
                topPts.Add(BBpts[7]);

                var topPt = GetPointAvg(topPts);
                var topPlane = new Plane(topPt, Vector3d.ZAxis * flipFactor);
                planes.Add(topPlane);
            }

            if (pX)
            {

                var pxPts = new List<Point3d>();
                pxPts.Add(BBpts[1]);
                pxPts.Add(BBpts[5]);
                pxPts.Add(BBpts[6]);
                pxPts.Add(BBpts[2]);

                var pxPt = GetPointAvg(pxPts);
                var pxPlane = new Plane(pxPt, Vector3d.XAxis * flipFactor);
                planes.Add(pxPlane);
            }

            if (nX)
            {
                var nxPts = new List<Point3d>();
                nxPts.Add(BBpts[7]);
                nxPts.Add(BBpts[4]);
                nxPts.Add(BBpts[3]);
                nxPts.Add(BBpts[0]);

                var nxPt = GetPointAvg(nxPts);
                var nxPlane = new Plane(nxPt, Vector3d.XAxis * -1 * flipFactor);
                planes.Add(nxPlane);
            }

            if (nY)
            {
                var nyPts = new List<Point3d>();
                nyPts.Add(BBpts[4]);
                nyPts.Add(BBpts[5]);
                nyPts.Add(BBpts[1]);
                nyPts.Add(BBpts[0]);

                var nyPt = GetPointAvg(nyPts);
                var nyPlane = new Plane(nyPt, Vector3d.YAxis * -1 * flipFactor);
                planes.Add(nyPlane);
            }


            if (pY)
            {
                var pyPts = new List<Point3d>();
                pyPts.Add(BBpts[6]);
                pyPts.Add(BBpts[7]);
                pyPts.Add(BBpts[2]);
                pyPts.Add(BBpts[3]);

                var pyPt = GetPointAvg(pyPts);
                var pyPlane = new Plane(pyPt, Vector3d.YAxis * flipFactor);
                planes.Add(pyPlane);

            }


            return planes;
        }

        /// <summary>
        /// Gets the avg point of a list of points
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static Point3d GetPointAvg(List<Point3d> pts)
        {
            Point3d pt = new Point3d(0, 0, 0);
            foreach (var point in pts)
            {
                pt += point;
            }
            pt = pt / pts.Count;
            return pt;
        }

        /// <summary>
        /// Create Clipping Planes. Remeber to delete after usage
        /// </summary>
        /// <param name="planes"></param>
        /// <returns></returns>
        public static List<System.Guid> CreateClippingPlanes(List<Plane> planes)
        {
            List<System.Guid> ids = new List<System.Guid>();

            // var activeViewPort = this.RhinoDocument.Views.ActiveView.ActiveViewport;
            var activeViewPort = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

            // System.Guid id = this.RhinoDocument.Objects.AddClippingPlane(plane, 10, 10, activeViewPort.Id);
            foreach (var plane in planes)
            {
                ids.Add(RhinoDoc.ActiveDoc.Objects.AddClippingPlane(plane, 1, 1, activeViewPort.Id));
            }
            return ids;
        }

        /// <summary>
        /// Makes a polyline to be used to be used as the display polyline
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Polyline GetPolyLineRectangle (Plane plane)
        {
            Interval interval = new Interval(-1,1);
            var rect = new Rectangle3d(plane, interval,interval);
            return rect.ToPolyline();
     
        }

        /// <summary>
        /// Makes a line for the display arrow
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Line GetArrowLine (Plane plane)
        {
            return new Line(plane.Origin, plane.Origin + plane.Normal * 1);
        }

        /// <summary>
        /// Draws the rectangles of the clipping planes and their arrows
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="planes"></param>
        public static void DrawClippingPlanesRectangles(Rhino.Display.DisplayPipeline dp , List<Plane> planes)
        {
            foreach (var plane in planes)
            {
               dp.DrawDottedPolyline(GetPolyLineRectangle(plane),System.Drawing.Color.Cyan,true);
               dp.DrawArrow(GetArrowLine(plane),System.Drawing.Color.Orange);
            }
        }


        /// <summary>
        /// Gets the bitmap of a cdrtain geometry 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="clippingPlanePlanes"></param>
        /// <param name="CameraDirection"></param>
        /// <param name="bitmapHeight"></param>
        /// <param name="bitmapWidth"></param>
        /// <param name="parallelprojection"></param>
        /// <returns></returns>
        public static Bitmap GetBitmap(GeometryBase geometry , List<Plane> clippingPlanePlanes ,  Vector3d CameraDirection , int bitmapHeight , int bitmapWidth,bool parallelprojection) 
        {
            //Make Clipping planes and hide them
            List<System.Guid>  ids = CreateClippingPlanes(clippingPlanePlanes);
            foreach (var id in ids)
            {
              Rhino.RhinoDoc.ActiveDoc.Objects.Hide(id, true);
            }
            var bb = geometry.GetBoundingBox(true);
            var activeViewPort = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
            activeViewPort.SetCameraDirection(CameraDirection, true);
            activeViewPort.SetCameraLocation(bb.Center,true);
            activeViewPort.SetCameraTarget(bb.Center, true);

            if (parallelprojection)
            {
                activeViewPort.ChangeToParallelProjection(true);
            }

            activeViewPort.ZoomBoundingBox(bb);
            System.Drawing.Bitmap bitmap = Rhino.Display.DisplayPipeline.DrawToBitmap(activeViewPort, bitmapHeight, bitmapWidth);

            foreach (var id in ids)
            {
                Rhino.RhinoDoc.ActiveDoc.Objects.Show(id, true);
                Rhino.RhinoDoc.ActiveDoc.Objects.Delete(id, true);
            }

          //  RhinoDoc.ActiveDoc.Views.Redraw();
            return bitmap;
        }



    }
}
