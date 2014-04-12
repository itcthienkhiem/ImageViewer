using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using Global.Data;


namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class QueryPatientControl : UserControl
    {
        private QueryPatientComponent _component;
        public QueryPatientControl( QueryPatientComponent component)
        {
            InitializeComponent();
            _component = component;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {

        }

        
        private void btnDownImage_Click(object sender, EventArgs e)
        {
            List<string> l_list = new List<string> ();
            if (listView1.Items.Count > 0)
            {
                foreach (ListViewItem li in listView1.Items)
                {
                    if (li.Selected)
                    {
                        string PatientID = string.Format("{0}", li.SubItems[0].Text);
                        string Modality = string.Format("{0}", li.SubItems[5].Text);
                        string StudyDate = string.Format("{0}", li.SubItems[4].Text);
                        string strAccessnum = li.SubItems[6].Text;
                        StudyDate = Convert.ToDateTime(StudyDate).ToShortDateString();
                        l_list.Add(strAccessnum);
                    }
                }
                GlobalData.RunParams.listAccessionNumber = l_list.ToArray();
                ClearCanvas.ImageViewer.ImageViewerComponent viewer = null;
                DesktopWindow desktopWindow = null;
                List<string> _filenames = new List<string>();

                foreach (DesktopWindow window in Application.DesktopWindows)
                {
                    foreach (Workspace space in window.Workspaces)
                    {
                        if (space.Title == "imageview")
                        {
                            desktopWindow = window;
                            viewer = space.Component as ClearCanvas.ImageViewer.ImageViewerComponent;
                        }
                    }
                }
                if (viewer != null)
                {
                    DesktopWindowView windowview = (DesktopWindowView)desktopWindow.DesktopWindowView;
                    viewer.LoadHistoryStudyFromFtp(windowview.DesktopForm);
                } 
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }

        private void btnQuery_Click_1(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string sStartDate = dtStart.Value.ToShortDateString();
            string sEndDate = dtEnd.Value.ToShortDateString();
            string sWherePateint = "";
            string sWhereModality = "";
            if (edtPatientID.Text != "")
            {
                sWherePateint = string.Format(" and a.patientid='{0}' ", edtPatientID.Text);
            }
            if (edtPatientName.Text != "")
            {
                sWherePateint += string.Format(" and a.familyname like '{0}%' ", edtPatientName.Text);
            }
            if ((cmbModality.Text != "") && (cmbModality.Text != "ALL"))
            {
                sWhereModality = string.Format(" and c.modality='{0}' ", cmbModality.Text);
            }
            string sqlstr = string.Format(" select distinct a.patientid 影像号,a.familyname 姓名,a.birthdate 出生日期," +
                                          "        a.sex 性别,b.studydate 诊断日期,b.studytime 诊断时间," +
                                          "        c.modality 设备类型,b.accessionnumber RIS流水号 " +
                                          " from patients a,studies b,series c " +
                                          " where a.patientid=b.patientid and b.studyinstanceuid=c.studyinstanceuid " +
                                          " and b.studydate>='{0} 00:00:00' and b.studydate<='{1} 23:59:59' " +
                                          " {2} {3} ", sStartDate, sEndDate, sWherePateint, sWhereModality);
           
            SqlDataAdapter sqlDaQuery = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
            DataSet sqlDsQuery = new DataSet();
            sqlDaQuery.Fill(sqlDsQuery);
            for (int i = 0; i < sqlDsQuery.Tables[0].Rows.Count; i++)
            {
                string[] result = new string[7];

                DataRow row = sqlDsQuery.Tables[0].Rows[i];
                result[0] = row["影像号"].ToString();
                result[1] = row["姓名"].ToString();
                result[2] = row["出生日期"].ToString();
                result[3] = row["性别"].ToString();
                result[4] = row["诊断时间"].ToString();
                result[5] = row["设备类型"].ToString();
                result[6] = row["RIS流水号"].ToString();
                listView1.Items.Add(new ListViewItem(result));
            }
        }
    }
}
