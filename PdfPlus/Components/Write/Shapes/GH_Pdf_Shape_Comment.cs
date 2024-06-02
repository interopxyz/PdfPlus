using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace PdfPlus.Components.Write.Shapes
{
    public class GH_Pdf_Shape_Comment : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Pdf_Shape_Comment class.
        /// </summary>
        public GH_Pdf_Shape_Comment()
          : base("Comment Shape", "Comment Shp",
              "Create a Comment Shape at a location",
              Constants.ShortName, Constants.Shapes)
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Content", "C", "The content of the comment", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "P", "The location point of the comment", GH_ParamAccess.item);
            pManager.AddTextParameter("Title", "T", "The title of the comment", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Subject", "S", "The subject of the comment", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Icon", "I", "The comment icon", GH_ParamAccess.item, 0);
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Constants.Shape.Name, Constants.Shape.NickName, Constants.Shape.Output, GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string content = "";
            if (!DA.GetData(0, ref content)) return;

            Point3d point = new Point3d();
            if (!DA.GetData(1, ref point)) return;

            string title = "Unnamed";
            DA.GetData(2, ref title);

            string subject = "Unspecified";
            DA.GetData(3, ref subject);

            int icon = 6;
            DA.GetData(4, ref icon);

            Shape shape = Shape.CreateComment(title, subject, content, point, (Shape.CommentIcons)icon);

            DA.SetData(0, shape);

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
                return Properties.Resources.Pdf_Shape_Comment;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6f487c10-d602-47a0-b4f9-33e3c9dcb82f"); }
        }
    }
}