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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Controls.WinForms.Native;

using System.Data;
using System.Data.SqlClient;
using Global.Data;
using Global.FtpSocketClient;



#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "Export")]
	[Tooltip("export", "TooltipExportToImage")]
	[IconSet("export", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png")]
	[EnabledStateObserver("export", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("export", AuthorityTokens.Clipboard.Export.Jpeg)]

    //[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "ExportTeach")]
    //[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "ExportTeach")]
    //[Tooltip("export", "TooltipExportToImage")]
    //[IconSet("export", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png")]
    //[EnabledStateObserver("export", "Enabled", "EnabledChanged")]
    //[ViewerActionPermission("export", AuthorityTokens.Clipboard.Export.Jpeg)]
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class ExportToImageTool : ClipboardTool
	{
        public const int WM_SNDRIS = USER + 1004;
        public const int USER = 0x0400;

		public ExportToImageTool()
		{
		}

		public void Export()
		{
			try
			{
				List<IClipboardItem> selectedClipboardItems = new List<IClipboardItem>(this.Context.SelectedClipboardItems);
				ImageExportComponent.Launch(this.Context.DesktopWindow, selectedClipboardItems);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageExportFailed, Context.DesktopWindow);
			}
		}

        public void ExportTeach()
        {
           
        }

        private string GetMaxIDFormImageBack()
        {
            string sImgid = GlobalData.RunParams.AccessionNumber + "001";
            string sqlstr = string.Format("select convert(varchar(10),convert(int,substring(isnull(max(imgid),0),13,3))+1) imgid from imageback where id = '{0}'",
                GlobalData.RunParams.AccessionNumber);
            SqlDataAdapter sqlDataAd = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
            sqlDataAd.SelectCommand.CommandType = CommandType.Text;
            SqlDataReader ImageReader = sqlDataAd.SelectCommand.ExecuteReader();
            while (ImageReader.Read())
            {
                sImgid = GlobalData.RunParams.AccessionNumber + ((string)ImageReader["imgid"]).PadLeft(3, '0');
            }
            ImageReader.Close();
            sqlDataAd.Dispose();
            return sImgid;
        }

        private string GetRemoteFilePath()
        {
            return GlobalData.RunParams.RunMode + @"\" +
                   GlobalData.RunParams.AccessionNumber.Substring(0, 8) + @"\" +
                   GlobalData.RunParams.AccessionNumber + @"\";
        }
	}
}