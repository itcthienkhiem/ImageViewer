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

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
    [MenuAction("activate", "imageviewer-contextmenu/MenuHeartRatio", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
    [MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuHeartRatio", "Select", Flags = ClickActionFlags.CheckAction)]
    [MenuAction("activate", RulerTool.MeasurementToolbarDropdownSite + "/MenuHeartRatio", "Select", Flags = ClickActionFlags.CheckAction)]
    [ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarHeartRatio", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
    [MouseButtonIconSet("activate", "Icons.EllipticalRoiToolSmall.png", "Icons.EllipticalRoiToolMedium.png", "Icons.EllipticalRoiToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Elliptical")]

    [MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(MeasurementToolbarToolExtensionPoint))]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public partial class HeartRatiolRoiTool : MeasurementTool
    {
        public HeartRatiolRoiTool() : base(SR.TooltipPolygonalRoi) { }

        protected override string CreationCommandName
        {
            get { return SR.CommandCreateHeartRatio; }
        }

        protected override string RoiNameFormat
        {
            get { return SR.FormatPolygonName; }
        }

        //protected override IGraphic CreateGraphic()
        //{
        //    return new PolygonControlGraphic(true, new MoveControlGraphic(new PolylineGraphic(true)));
        //}

        //protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
        //{
        //    return new InteractivePolygonGraphicBuilder((IPointsGraphic)graphic);
        //}

        //protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
        //{
        //    return new PolygonalRoiCalloutLocationStrategy();
        //}

        protected override IGraphic CreateGraphic()
        {
            return new VerticesControlGraphic(new MoveControlGraphic(new PolylineGraphic()));
        }

        protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
        {
            return new InteractiveHeartRatioLineGraphicBuilder(2, (IPointsGraphic)graphic);
        }

        protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
        {
            return new RulerCalloutLocationStrategy();
        }
    }

}