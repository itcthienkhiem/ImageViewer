using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    [MenuAction("QueryPatient", "global-menus/测试/测试", "test")]
  
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    class QeuryPatientTool : Tool<IDesktopToolContext>
    {
        public void test()
        {
            ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, new QueryPatientComponent(), "查询提取病人影像");
        }
    }


    [ExtensionPoint]
    public sealed class QueryPatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(QueryPatientComponentViewExtensionPoint))]
    public class QueryPatientComponent : ApplicationComponent
    {

        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Does nothing unless the task has completed; closes the progress dialog.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }
        // your component code here

    }
}
