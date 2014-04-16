#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Desktop.Tools;
namespace ClearCanvas.ImageViewer.Tools.Measurement
{
   
    [ExtensionPoint]
    public sealed class MeasurementToolbarToolExtensionPoint : ExtensionPoint< ClearCanvas.Desktop.Tools.ITool> { }

    //[MenuAction("activate", "imageviewer-contextmenu/MenuRuler", "Select", Flags = ClickActionFlags.CheckAction)]
    
    //[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuRuler", "Select", Flags = ClickActionFlags.CheckAction)]
   [DropDownButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarRuler", "Select", "MeasurementMenuModel")]

	//[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarRuler", "Select", Flags = ClickActionFlags.CheckAction)]
    //[CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
    [MouseButtonIconSet("activate", "Icons.RulerToolSmall.png", "Icons.RulerToolMedium.png", "Icons.RulerToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Linear")]

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class RulerTool : MeasurementTool
	{
		public RulerTool()
			: base(SR.TooltipRuler)
		{
		}
        
		protected override string CreationCommandName
		{
			get { return SR.CommandCreateRuler; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatRulerName; }
		}

		protected override IGraphic CreateGraphic()
		{
			return new VerticesControlGraphic(new MoveControlGraphic(new PolylineGraphic()));
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractivePolylineGraphicBuilder(2, (IPointsGraphic) graphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new RulerCalloutLocationStrategy();
		}
        //--added by luojiang
        private class ToolContextProxy : IImageViewerToolContext
        {
            private readonly IImageViewerToolContext _realContext;

            public ToolContextProxy(IImageViewerToolContext realContext)
            {
                _realContext = realContext;
            }

            #region IImageViewerToolContext Members

            public IImageViewer Viewer
            {
                get { return _realContext.Viewer; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _realContext.DesktopWindow; }
            }

            #endregion
        }


        private ClearCanvas.Desktop.Tools.ToolSet _toolSet;
        public const string MeasurementToolbarDropdownSite = "Measurement-toolbar-dropdown";
        public ActionModelNode MeasurementMenuModel
        {
            get { return ActionModelRoot.CreateModel(typeof(RulerTool).FullName, MeasurementToolbarDropdownSite, _toolSet.Actions); }
        }
        public override void Initialize()
        {
            base.Initialize();

            object[] tools;

            try
            {
                tools = new MeasurementToolbarToolExtensionPoint().CreateExtensions();
            }
            catch (NotSupportedException)
            {
                tools = new object[0];
                Platform.Log(LogLevel.Debug, "No clipboard toolbar drop-down items found.");
            }
            catch (Exception e)
            {
                tools = new object[0];
                Platform.Log(LogLevel.Debug, "Failed to create clipboard toolbar drop-down items.", e);
            }

            _toolSet = new ToolSet(tools, new ToolContextProxy(Context));
            //ע�᣿
            ImageViewerComponent viewer = Context.Viewer as ImageViewerComponent;
            viewer.RegisterImageViewerTool(_toolSet);
        }

        protected override void Dispose(bool disposing)
        {
            _toolSet.Dispose();
            base.Dispose(disposing);
        }

        
    }

    #region Oto
    partial class RulerTool : IDrawRuler
    {
        AnnotationGraphic IDrawRuler.Draw(CoordinateSystem coordinateSystem, string name, PointF point1, PointF point2)
        {
            var image = Context.Viewer.SelectedPresentationImage;
            if (!CanStart(image))
                throw new InvalidOperationException("Can't draw an elliptical ROI at this time.");

            var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
            if (coordinateSystem == CoordinateSystem.Destination)
            {
                //Use the image graphic to get the "source" coordinates because it's already in the scene.
                point1 = imageGraphic.SpatialTransform.ConvertToSource(point1);
                point2 = imageGraphic.SpatialTransform.ConvertToSource(point2);
            }

            var overlayProvider = (IOverlayGraphicsProvider) image;
            var roiGraphic = CreateRoiGraphic(false);
            roiGraphic.Name = name;
            AddRoiGraphic(image, roiGraphic, overlayProvider);

            var subject = (IPointsGraphic)roiGraphic.Subject;

            subject.Points.Add(point1);
            subject.Points.Add(point2);

            roiGraphic.Callout.Update();
            roiGraphic.State = roiGraphic.CreateSelectedState();
            //roiGraphic.Draw();
            return roiGraphic;
        }
    }
    #endregion
}