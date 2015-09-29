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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

using System.Data;
using System.Data.SqlClient;
using Global.Data;
using System.Collections; 
using Global.FtpSocketClient;

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionPoint]
	public sealed class HealthcareArtifactExplorerExtensionPoint : ExtensionPoint<IHealthcareArtifactExplorer> {}

	[MenuAction("show", "global-menus/MenuFile/MenuExplorer", "Show", KeyStroke = XKeys.Control | XKeys.E)]
	[IconSet("show", "Icons.ExplorerToolSmall.png", "Icons.ExplorerToolMedium.png", "Icons.ExplorerToolLarge.png")]
	[GroupHint("show", "Application.Browsing.Explorer")]
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public sealed class ExplorerTool : Tool<IDesktopToolContext>
	{
		private static ExplorerTool _mainWindowTool = null;
		private static IDesktopObject _desktopObject;

		private static bool IsWorkspaceUnclosable
		{
			get { return ExplorerLocalSettings.Default.ExplorerIsPrimary; }
		}

		private static bool IsWorkspaceClosable
		{
			get { return !ExplorerLocalSettings.Default.ExplorerIsPrimary; }
		}

		private static bool LaunchAsShelf
		{
			get
			{
				if (ExplorerLocalSettings.Default.ExplorerIsPrimary)
					return false;

				return ExplorerSettings.Default.LaunchAsShelf;
			}
		}

		public override IActionSet Actions
		{
			get
			{
				if (IsWorkspaceUnclosable || _mainWindowTool != this || GetExplorers().Count == 0)
				{
					return new ActionSet();
				}
				else
				{
					return base.Actions;
				}
			}
		}

		public override void Initialize()
		{
			if (_mainWindowTool == null)
				_mainWindowTool = this;

			base.Initialize();
		}

		public void Show()
		{
			BlockingOperation.Run(ShowInternal);
		}

		private void ShowInternal()
		{
			//ShowExplorer(Context.DesktopWindow);
		}

		public static void ShowExplorer(IDesktopWindow desktopWindow)
		{
            //try
            //{
            //    string[] files = { "e:\\26885681.dcm", "e:\\26885683.dcm" };
            //    new OpenFilesHelper(files) { WindowBehaviour = ViewerLaunchSettings.WindowBehaviour }.OpenFiles();
            //}
            //catch (Exception e)
            //{
            //    //ExceptionHandler.Report(e, SR.MessageUnableToOpenImages, Context.DesktopWindow);
            //}

            DownloadDicomFile downloadfile = new DownloadDicomFile();
            downloadfile.DownloadImages();
            string[] files = downloadfile.m_files.ToArray();
            try
            {
                new OpenFilesHelper(files) { WindowBehaviour = ViewerLaunchSettings.WindowBehaviour }.OpenFiles();
            }
            catch (Exception e)
            {
                //ExceptionHandler.Report(e, SR.MessageUnableToOpenImages, Context.DesktopWindow);
            }

            return;
          

            if (_desktopObject != null)
            {
                _desktopObject.Activate();
                return;
            }
            //return;

            List<TabPage> pages = new List<TabPage>();
            foreach (IHealthcareArtifactExplorer explorer in GetExplorers())
            {
                IApplicationComponent component = explorer.Component;
                if (component != null)
                    pages.Add(new TabPage(explorer.Name, component));
            }

            if (pages.Count == 0)
                return;

            TabComponentContainer container = new TabComponentContainer();
            foreach (TabPage page in pages)
                container.Pages.Add(page);

            if (LaunchAsShelf)
            {
                ShelfCreationArgs args = new ShelfCreationArgs();
                args.Component = container;
                args.Title = SR.TitleExplorer;
                args.Name = "Explorer";
                args.DisplayHint = ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide;

                _desktopObject = ApplicationComponent.LaunchAsShelf(desktopWindow, args);
            }
            else
            {
                WorkspaceCreationArgs args = new WorkspaceCreationArgs();
                args.Component = container;
                args.Title = SR.TitleExplorer;
                //args.Title = "打开本地文件";
                args.Name = "Explorer";
                args.UserClosable = IsWorkspaceClosable;

                _desktopObject = ApplicationComponent.LaunchAsWorkspace(desktopWindow, args);
            }

            _desktopObject.Closed += delegate { _desktopObject = null; };
		}


        //public static void ShowExplorer(IDesktopWindow desktopWindow)
        //{
        //    if (_desktopObject != null)
        //    {
        //        _desktopObject.Activate();
        //        return;
        //    }

        //    List<TabPage> pages = new List<TabPage>();
        //    foreach (IHealthcareArtifactExplorer explorer in GetExplorers())
        //    {
        //        IApplicationComponent component = explorer.Component;
        //        if (component != null)
        //            pages.Add(new TabPage(explorer.Name, component));
        //    }

        //    if (pages.Count == 0)
        //        return;

        //    TabComponentContainer container = new TabComponentContainer();
        //    foreach (TabPage page in pages)
        //        container.Pages.Add(page);

        //    if (LaunchAsShelf)
        //    {
        //        ShelfCreationArgs args = new ShelfCreationArgs();
        //        args.Component = container;
        //        args.Title = SR.TitleExplorer;
        //        args.Name = "Explorer";
        //        args.DisplayHint = ShelfDisplayHint.DockLeft | ShelfDisplayHint.DockAutoHide;

        //        _desktopObject = ApplicationComponent.LaunchAsShelf(desktopWindow, args);
        //    }
        //    else
        //    {
        //        WorkspaceCreationArgs args = new WorkspaceCreationArgs();
        //        args.Component = container;
        //        args.Title = SR.TitleExplorer;
        //        args.Name = "Explorer";
        //        args.UserClosable = IsWorkspaceClosable;

        //        _desktopObject = ApplicationComponent.LaunchAsWorkspace(desktopWindow, args);
        //    }

        //    _desktopObject.Closed += delegate { _desktopObject = null; };
        //}

		internal static List<IHealthcareArtifactExplorer> GetExplorers()
		{
			List<IHealthcareArtifactExplorer> healthcareArtifactExplorers = new List<IHealthcareArtifactExplorer>();
			try
			{
				HealthcareArtifactExplorerExtensionPoint xp = new HealthcareArtifactExplorerExtensionPoint();
				object[] extensions = xp.CreateExtensions();
				foreach (IHealthcareArtifactExplorer explorer in extensions)
				{
					if (explorer.IsAvailable)
						healthcareArtifactExplorers.Add(explorer);
				}
			}
			catch (NotSupportedException) {}

			return healthcareArtifactExplorers;
		}     
    }

    [ExtensionOf(typeof(StartupActionProviderExtensionPoint))]
    internal sealed class StartupActionProvider : IStartupActionProvider
    {
        public string Name
        {
            get { return SR.TitleExplorer; }
        }

        public string Description
        {
            get { return SR.DescriptionExplorer; }
        }

        public void Startup(IDesktopWindow mainDesktopWindow)
        {
            ExplorerTool.ShowExplorer(mainDesktopWindow);
        }
    }
}