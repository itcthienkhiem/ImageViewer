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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.View.WinForms;

using ClearCanvas.Common.Utilities;

using Global.Data;


namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarStack", "Select", "SortMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.S)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.StackToolSmall.png", "Icons.StackToolMedium.png", "Icons.StackToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Standard")]
	//
	[MouseWheelHandler(ModifierFlags.None)]
	[MouseToolButton(XMouseButtons.Left, false)]
	//
	[KeyboardAction("stackup", "imageviewer-keyboard/ToolsStandardStack/StackUp", "StackUp", KeyStroke = XKeys.PageUp)]
	[KeyboardAction("stackdown", "imageviewer-keyboard/ToolsStandardStack/StackDown", "StackDown", KeyStroke = XKeys.PageDown)]
	[KeyboardAction("jumptobeginning", "imageviewer-keyboard/ToolsStandardStack/JumpToBeginning", "JumpToBeginning", KeyStroke = XKeys.Home)]
	[KeyboardAction("jumptoend", "imageviewer-keyboard/ToolsStandardStack/JumpToEnd", "JumpToEnd", KeyStroke = XKeys.End)]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class StackTool : MouseImageViewerTool, IStack
	{
		private MemorableUndoableCommand _memorableCommand;
		private int _initialPresentationImageIndex;
		private IImageBox _currentImageBox;
		private SimpleActionModel _sortMenuModel;

		public StackTool()
			: base(SR.TooltipStack)
		{
			CursorToken = new CursorToken("Icons.StackToolSmall.png", GetType().Assembly);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode SortMenuModel
		{
			get
			{
				if (_sortMenuModel == null)
				{
					_sortMenuModel = new SimpleActionModel(new ApplicationThemeResourceResolver(GetType().Assembly));
					foreach (var item in ImageComparerList.Items)
					{
						var itemVar = item;
						var action = _sortMenuModel.AddAction(GetActionKey(itemVar), itemVar.Description, null, itemVar.Description, () => Sort(itemVar));
						action.Checked = GetSortMenuItemCheckState(itemVar);
					}
				}

				return _sortMenuModel;
			}
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			UpdateCheckState();
            thumbileChanged(e.SelectedPresentationImage.ParentDisplaySet);
		}

		private static string GetActionKey(ImageComparerList.Item item)
		{
			return item.IsReverse ? string.Format("{0} (Reverse)", item.Name) : item.Name;
		}

		private void UpdateCheckState()
		{
			if (_sortMenuModel == null)
				return;

			foreach (var item in ImageComparerList.Items)
			{
				var itemVar = item;
				var actionKey = GetActionKey(itemVar);
				var action = _sortMenuModel[actionKey] as ClickAction;
				if (action == null)
					continue;

				action.Checked = GetSortMenuItemCheckState(itemVar);
			}
		}

		private bool GetSortMenuItemCheckState(ImageComparerList.Item item)
		{
			return SelectedPresentationImage != null && SelectedPresentationImage.ParentDisplaySet != null &&
			       item.Comparer.Equals(SelectedPresentationImage.ParentDisplaySet.PresentationImages.SortComparer);
		}

		private void Sort(ImageComparerList.Item item)
		{
			IImageBox imageBox = ImageViewer.SelectedImageBox;
			IDisplaySet displaySet;
			if (imageBox == null || (displaySet = ImageViewer.SelectedImageBox.DisplaySet) == null)
				return;

			if (displaySet.PresentationImages.Count == 0)
				return;

			//try to keep the top-left image the same.
			IPresentationImage topLeftImage = imageBox.TopLeftPresentationImage;

			var command = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};

			displaySet.PresentationImages.Sort(item.Comparer);
			imageBox.TopLeftPresentationImage = topLeftImage;
			imageBox.Draw();

			command.EndState = imageBox.CreateMemento();
			if (!command.BeginState.Equals(command.EndState))
			{
				var historyCommand = new DrawableUndoableCommand(imageBox) {Name = SR.CommandSortImages};
				historyCommand.Enqueue(command);
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}

			UpdateCheckState();
		}

		private void CaptureBeginState(IImageBox imageBox)
		{
			_memorableCommand = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};
			// Capture state before stack
			_currentImageBox = imageBox;

			_initialPresentationImageIndex = imageBox.SelectedTile.PresentationImageIndex;
		}

		private bool CaptureEndState()
		{
			if (_memorableCommand == null || _currentImageBox == null)
			{
				_currentImageBox = null;
				return false;
			}

			bool commandAdded = false;

			// If nothing's changed then just return
			if (_initialPresentationImageIndex != _currentImageBox.SelectedTile.PresentationImageIndex)
			{
				// Capture state after stack
				_memorableCommand.EndState = _currentImageBox.CreateMemento();
				if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				{
					var historyCommand = new DrawableUndoableCommand(_currentImageBox) {Name = SR.CommandStack};
					historyCommand.Enqueue(_memorableCommand);
					Context.Viewer.CommandHistory.AddCommand(historyCommand);
					commandAdded = true;
				}
			}

			_memorableCommand = null;
			_currentImageBox = null;

			return commandAdded;
		}

		private void JumpToBeginning()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = 0;
			if (CaptureEndState())
				imageBox.Draw();
		}

		private void JumpToEnd()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;

			if (imageBox.DisplaySet == null)
				return;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count - 1;
			if (CaptureEndState())
				imageBox.Draw();
		}

		private void StackUp()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(-imageBox.Tiles.Count, imageBox);
			CaptureEndState();
            GlobalData.direct = 1;
			//No draw - AdvanceImage has already done it.
		}

		private void StackDown()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(+imageBox.Tiles.Count, imageBox);
			CaptureEndState();
            GlobalData.direct = -1;
			//No draw - AdvanceImage has already done it.
		}

		//private static void AdvanceImage(int increment, IImageBox selectedImageBox)
        private  void AdvanceImage(int increment, IImageBox selectedImageBox)
		{

            if (increment > 0)
                GlobalData.direct = 1;
            else
                GlobalData.direct = -1;

			int prevTopLeftPresentationImageIndex = selectedImageBox.TopLeftPresentationImageIndex;
			selectedImageBox.TopLeftPresentationImageIndex += increment;


            if (selectedImageBox.TopLeftPresentationImageIndex != prevTopLeftPresentationImageIndex)
                selectedImageBox.Draw();
            
            else
            {
                ImageViewerComponent view = this.ImageViewer as ImageViewerComponent;
                ActionModelNode node = view.ToolbarModel;

                ActionModelNode tempNode = null;
                IAction [] action = null;
                foreach (ActionModelNode tempnode in node.ChildNodes)
                {
                    if (tempnode.PathSegment.ResourceKey == "ToolbarSynchronizeStacking")
                    {
                        tempNode = tempnode;
                        break;
                    }
                }
                if (tempNode != null)
                {
                    action = tempNode.GetActionsInOrder();
                }
                if ((action != null) && (action.Count() > 0))
                {
                    ButtonAction ac = action[0] as ButtonAction;
                    if (ac.Checked == true)
                       return;
                }
                if (increment > 0)
                {
                    AdvanceDisplaySet(1);
                  
                }
                else
                {
                    AdvanceDisplaySet(-1);
                    
                }
            }
		}

        private IDisplaySet GetSourceDisplaySet()
        {
            IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
            if (imageBox == null)
                return null;

            IDisplaySet currentDisplaySet = imageBox.DisplaySet;

            if (currentDisplaySet == null)
                return null;

            if (currentDisplaySet.ParentImageSet == null)
            {
                // if the display set doesn't have a parent image set, fall back to using the logical workspace and the UID fo the display set
                // this situation usually arises from dynamically generated alternate views of a display set which is part of an image set
                return currentDisplaySet.ImageViewer != null ? currentDisplaySet.ImageViewer.LogicalWorkspace.ImageSets.SelectMany(ims => ims.DisplaySets).FirstOrDefault(ds => ds.Uid == currentDisplaySet.Uid) : null;
            }

            return CollectionUtils.SelectFirst(currentDisplaySet.ParentImageSet.DisplaySets, displaySet => displaySet.Uid == currentDisplaySet.Uid);
        }

        public void AdvanceDisplaySet(int direction)
        {
            if (!Enabled)
                return;

            IDisplaySet sourceDisplaySet = GetSourceDisplaySet();
            if (sourceDisplaySet == null)
                return;

            IImageBox imageBox = base.Context.Viewer.SelectedImageBox;
            IImageSet parentImageSet = sourceDisplaySet.ParentImageSet;

            int sourceDisplaySetIndex = parentImageSet.DisplaySets.IndexOf(sourceDisplaySet);
            sourceDisplaySetIndex += direction;

            if (sourceDisplaySetIndex < 0)
            {
                if (base.Context.Viewer.LogicalWorkspace.ImageSets.Count >= 2)
                {
                    int tempNum = 0;
                    foreach (IImageSet set in base.Context.Viewer.LogicalWorkspace.ImageSets)
                    {
                        if (set.Equals(parentImageSet))
                        {
                            tempNum++;
                            break;
                        }
                        tempNum++;
                    }
                    if (tempNum >= base.Context.Viewer.LogicalWorkspace.ImageSets.Count)
                    {
                        tempNum = 0;
                    }
                    parentImageSet = base.Context.Viewer.LogicalWorkspace.ImageSets[tempNum];
                    sourceDisplaySetIndex = 0;
                }
                else
                    sourceDisplaySetIndex = parentImageSet.DisplaySets.Count - 1;
            }
            else if (sourceDisplaySetIndex >= parentImageSet.DisplaySets.Count)
            {
                if (base.Context.Viewer.LogicalWorkspace.ImageSets.Count >= 2)
                {
                    int tempNum = 0;
                    foreach (IImageSet set in base.Context.Viewer.LogicalWorkspace.ImageSets)
                    {
                        if (set.Equals(parentImageSet))
                        {
                            tempNum++;
                            break;
                        }
                        tempNum++;
                    }

                    if (tempNum >= base.Context.Viewer.LogicalWorkspace.ImageSets.Count)
                    {
                        tempNum = 0;
                    }
                    parentImageSet = base.Context.Viewer.LogicalWorkspace.ImageSets[tempNum];
                    sourceDisplaySetIndex = 0;
                } else
                     sourceDisplaySetIndex = 0;
            }
            //MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
            //memorableCommand.BeginState = imageBox.CreateMemento();
            
            imageBox.DisplaySet = parentImageSet.DisplaySets[sourceDisplaySetIndex].CreateFreshCopy();
            if (direction < 0)
            {
                imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count - 1;
            }

            thumbileChanged(imageBox.DisplaySet);

            imageBox.Draw();

            //memorableCommand.EndState = imageBox.CreateMemento();

            //DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
            //historyCommand.Enqueue(memorableCommand);
            //base.Context.Viewer.CommandHistory.AddCommand(historyCommand);
        }

        public void thumbileChanged(IDisplaySet displayset)
        {

            DesktopWindow desktopWindow = (DesktopWindow)base.Context.DesktopWindow;
            DesktopWindowView windowview = (ClearCanvas.Desktop.View.WinForms.DesktopWindowView)desktopWindow.DesktopWindowView;
            Crownwood.DotNetMagic.Docking.Content content = windowview.DesktopForm.DockingManager.Contents["缩略图"];
            if (content == null)
                return;
            ClearCanvas.ImageViewer.Thumbnails.View.WinForms.ThumbnailComponentControl ctrol = (ClearCanvas.ImageViewer.Thumbnails.View.WinForms.ThumbnailComponentControl)content.Control;
            ctrol.getGallaryView().isDisplaySetChoose(displayset);
        }

		public override bool Start(IMouseInformation mouseInformation)
		{
            try
            {
                if (this.SelectedPresentationImage == null)
                    return false;

                base.Start(mouseInformation);

                if (mouseInformation.Tile == null)
                    return false;

                CaptureBeginState(mouseInformation.Tile.ParentImageBox);
            }
            catch 
            {
                Platform.Log(LogLevel.Error, "Error in StackTool.start()");
            }
			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Track(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			if (DeltaY == 0)
				return true;

			int increment;

			if (DeltaY > 0)
				increment = 1;
			else
				increment = -1;

			AdvanceImage(increment, mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureEndState();
		}

		public override void StartWheel()
		{
			if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			if (imageBox == null)
				return;

			CaptureBeginState(imageBox);
		}

		protected override void WheelBack()
		{
			if (this.SelectedPresentationImage == null)
				return;

			AdvanceImage(1, Context.Viewer.SelectedTile.ParentImageBox);
		}

		protected override void WheelForward()
		{
			if (this.SelectedPresentationImage == null)
				return;

			AdvanceImage(-1, Context.Viewer.SelectedTile.ParentImageBox);
		}

		public override void StopWheel()
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureEndState();
		}

		#region IStack Members

		void IStack.StackBy(int delta)
		{
			if (Context.Viewer.SelectedTile == null)
				throw new InvalidOperationException("No tile selected.");

			if (this.SelectedPresentationImage == null)
				throw new InvalidOperationException("No image selected.");

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(delta, imageBox);
			//No draw - AdvanceImage has already done it.
			CaptureEndState();
		}

		void IStack.StackTo(int instanceNumber, int? frameNumber)
		{
			if (Context.Viewer.SelectedTile == null)
				throw new InvalidOperationException("No tile selected.");

			if (this.SelectedPresentationImage == null)
				throw new InvalidOperationException("No image selected.");

			var displaySet = Context.Viewer.SelectedPresentationImage.ParentDisplaySet;

			//First will throw if no such image exists.
			var image = (IPresentationImage) displaySet.PresentationImages.OfType<IImageSopProvider>().First(
				i => i.ImageSop.InstanceNumber == instanceNumber
				     && (!frameNumber.HasValue || (i.ImageSop.NumberOfFrames > 1 && frameNumber.Value == i.Frame.FrameNumber)));

			IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImage = image;
			if (!CaptureEndState())
				return; /// TODO (CR Dec 2011): Should we still select the top-left??

			imageBox.Draw();
			imageBox.Tiles[0].Select();
		}

		#endregion
	}
}