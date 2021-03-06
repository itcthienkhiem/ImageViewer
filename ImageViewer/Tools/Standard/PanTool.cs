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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuPan", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuPan", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarPan", "Select", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.P)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[ClearCanvas.ImageViewer.BaseTools.MouseImageViewerTool.MouseButtonIconSet("activate", "Icons.PanToolSmall.png", "Icons.PanToolMedium.png", "Icons.PanToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Pan")]

	[KeyboardAction("panleft", "imageviewer-keyboard/ToolsStandardPan/PanLeft", "PanLeft", KeyStroke = XKeys.Control | XKeys.Left)]
	[KeyboardAction("panright", "imageviewer-keyboard/ToolsStandardPan/PanRight", "PanRight", KeyStroke = XKeys.Control | XKeys.Right)]
	[KeyboardAction("panup", "imageviewer-keyboard/ToolsStandardPan/PanUp", "PanUp", KeyStroke = XKeys.Control | XKeys.Up)]
	[KeyboardAction("pandown", "imageviewer-keyboard/ToolsStandardPan/PanDown", "PanDown", KeyStroke = XKeys.Control | XKeys.Down)]

	[MouseToolButton(XMouseButtons.Left, true)]
	[DefaultMouseToolButton(XMouseButtons.Left, ModifierFlags.Control)]
    
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PanTool : MouseImageViewerTool
	{
		private readonly SpatialTransformImageOperation _operation;
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;
		private ToolModalityBehaviorHelper _toolBehavior;

		public PanTool()
			: base(SR.TooltipPan)
		{
			this.CursorToken = new CursorToken("Icons.PanToolSmall.png", this.GetType().Assembly);
			_operation = new SpatialTransformImageOperation(Apply);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private ISpatialTransform GetSelectedImageTransform()
		{
			return _operation.GetOriginator(this.SelectedPresentationImage) as ISpatialTransform;
		}

		private bool CanPan()
		{
			return GetSelectedImageTransform() != null;
		}

		private void CaptureBeginState()
		{
			if (!CanPan())
				return;

			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			IMemorable originator = GetSelectedImageTransform();
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanPan() || _memorableCommand == null)
				return;

			_memorableCommand.EndState = GetSelectedImageTransform().CreateMemento();
			UndoableCommand applicatorCommand = _toolBehavior.Behavior.SelectedImagePanTool ? null : _applicator.ApplyToLinkedImages();
			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(this.SelectedPresentationImage);

			if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				historyCommand.Enqueue(_memorableCommand);
			if (applicatorCommand != null)
				historyCommand.Enqueue(applicatorCommand);

			if (historyCommand.Count > 0)
			{
				historyCommand.Name = SR.CommandPan;
				this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
			_memorableCommand = null;
            if (GetCheckedSync() == true)
            {
                var historyCommand2 = new DrawableUndoableOperationCommand<IPresentationImage>(this._operation, GetAllImages());
                historyCommand2.Execute();

                if (historyCommand2.Count > 0)
                {
                    historyCommand2.Name = SR.CommandMatchScale;
                    base.ImageViewer.CommandHistory.AddCommand(historyCommand2);
                }
            }
		}

		private void PanLeft()
		{
			IncrementPanWithUndo(-15, 0);
		}

		private void PanRight()
		{
			IncrementPanWithUndo(15, 0);
		}

		private void PanUp()
		{
			IncrementPanWithUndo(0, -15);
		}

		private void PanDown()
		{
			IncrementPanWithUndo(0, 15);
		}

		private void IncrementPanWithUndo(int xIncrement, int yIncrement)
		{
			if (!CanPan())
				return;

			this.CaptureBeginState();
			this.IncrementPan(xIncrement, yIncrement);
			this.CaptureEndState();
		}

		private void IncrementPan(int xIncrement, int yIncrement)
		{
			if (!CanPan())
				return;

			ISpatialTransform transform = _operation.GetOriginator(this.SelectedPresentationImage);

			// Because the pan increment is in destination coordinates, we have to convert
			// them to source coordinates, since the transform translation is in source coordinates.
			// This will allow the pan to work properly irrespective of the zoom, flip and rotation.
			
			SizeF sourceIncrement = transform.ConvertToSource(new SizeF(xIncrement, yIncrement));

			transform.TranslationX += sourceIncrement.Width;
			transform.TranslationY += sourceIncrement.Height;

			this.SelectedPresentationImage.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			this.IncrementPan(base.DeltaX, base.DeltaY);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}

		public void Apply(IPresentationImage image)
		{
            IImageSpatialTransform transform = (IImageSpatialTransform)_operation.GetOriginator(image);
			transform.TranslationX = this.SelectedSpatialTransformProvider.SpatialTransform.TranslationX;
			transform.TranslationY = this.SelectedSpatialTransformProvider.SpatialTransform.TranslationY;
            if (GetCheckedSync() == false)
                return;
            //
            IVoiLutLinear selectedLut = (IVoiLutLinear)this.SelectedVoiLutProvider.VoiLutManager.VoiLut;

            IVoiLutProvider provider = ((IVoiLutProvider)image);
            if (!(provider.VoiLutManager.VoiLut is IBasicVoiLutLinear))
            {
                BasicVoiLutLinear installLut = new BasicVoiLutLinear(selectedLut.WindowWidth, selectedLut.WindowCenter);
                provider.VoiLutManager.InstallVoiLut(installLut);
            }

            IBasicVoiLutLinear lut = (IBasicVoiLutLinear)provider.VoiLutManager.VoiLut;
            lut.WindowWidth = selectedLut.WindowWidth;
            lut.WindowCenter = selectedLut.WindowCenter;
            //
            IImageSpatialTransform referenceTransform = (IImageSpatialTransform)this.SelectedSpatialTransformProvider.SpatialTransform;
            transform.Scale = referenceTransform.Scale;
            transform.ScaleToFit = referenceTransform.ScaleToFit;
		}
	}
}
