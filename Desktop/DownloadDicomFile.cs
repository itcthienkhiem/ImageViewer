﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using Global.Data;
using System.Collections; 
using Global.FtpSocketClient;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{

    /// <summary>
    /// Class DownloadDicomFile  as download file class
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class represents a DICOM Part 10 format file.  The class inherits off an AbstractMessage class.  The class contains
    /// </para>
    /// </remarks>
    public class DownloadDicomFile
    {
        public List<string> m_files = null;
        public string PatientName = "";
        public delegate void ShowMedicalViewDelegate(List<string> list);
        private int m_count = 0;
        //public bool GetPatientRisInfo(string sRisID, string PatientID, string Modality, string StudyDate, string RunMode, out RisTags ristag)
        //{
        //    bool bSucc = false;
        //    ristag = new RisTags();
        //    SqlDataAdapter sqlQueryExamrecord;
        //    SqlDataReader sqlExamrecordReader;
        //    string sel = "";
        //    if (sRisID.Length < 12)
        //    {
        //        sel = string.Format(@" select name,sex,age,ageunit,machinetype,machinename,visceras,examdate,examtime " +
        //                                    " from examrecord " +
        //                                    " where olddocid='{0}' and machinetype='{1}' " +
        //                                    " and modidate>='{2} 00:00:00' and modidate<='{3} 23:59:59' and modulename='{4}' ",
        //                                    PatientID, Modality, StudyDate, StudyDate, RunMode);
        //    }
        //    else
        //    {
        //        sel = string.Format(@" select name,sex,age,ageunit,machinetype,machinename,visceras,examdate,examtime " +
        //                                    " from examrecord " +
        //                                    " where id='{0}' and modulename='{1}' ", sRisID, RunMode);
        //    }
        //    try
        //    {
        //        sqlQueryExamrecord = new SqlDataAdapter(sel, GlobalData.MainConn.ChangeType());
        //        sqlQueryExamrecord.SelectCommand.CommandType = CommandType.Text;
        //        sqlExamrecordReader = sqlQueryExamrecord.SelectCommand.ExecuteReader();
        //        while (sqlExamrecordReader.Read())
        //        {
        //            ristag.RisPatientID = PatientID;
        //            ristag.RisPatientName = (string)sqlExamrecordReader["name"];
        //            ristag.RisPatientSex = (string)sqlExamrecordReader["sex"];
        //            ristag.RisPatientAge = string.Format("{0}{1}", sqlExamrecordReader["age"], sqlExamrecordReader["ageunit"]);
        //            ristag.RisMachineType = (string)sqlExamrecordReader["machinetype"];
        //            ristag.RisVisceras = (string)sqlExamrecordReader["visceras"];
        //            ristag.RisExamDate = ((DateTime)sqlExamrecordReader["examdate"]).ToShortDateString();
        //            ristag.RisExamTime = string.Format("{0}", sqlExamrecordReader["examtime"]);
        //            bSucc = true;
        //        }
        //        sqlExamrecordReader.Close();
        //        sel = string.Format(@" select string1 from u_param where paramcode='系统参数' and string2='{0}' and string4='{1}' ", "HospitalName", RunMode);
        //        sqlQueryExamrecord = new SqlDataAdapter(sel, GlobalData.MainConn.ChangeType());
        //        sqlQueryExamrecord.SelectCommand.CommandType = CommandType.Text;
        //        sqlExamrecordReader = sqlQueryExamrecord.SelectCommand.ExecuteReader();
        //        while (sqlExamrecordReader.Read())
        //        {
        //            ristag.RisHospital = (string)sqlExamrecordReader["string1"];
        //        }
        //        sqlExamrecordReader.Close();
        //        sqlQueryExamrecord.Dispose();
        //        ristag.bSucc = bSucc;
        //        return bSucc;
        //    }
        //    catch (Exception ex)
        //    {
                
        //        return false;
        //    }
        //}

        public void GetDownloadFileName(string sPatientID, string sModality, string sStudyDate, string accessionNum, ref List<ArrayList> imageList, ref string strPatientName)
        {
            SqlDataAdapter sqlQueryImages;
            SqlDataReader sqlImagesReader;
            ArrayList arrSeriesString = new ArrayList();
            SqlDataAdapter sqlCommand;
            SqlDataReader SeriesReader;
            string sqlstr;

            strPatientName = ""; 
            if (accessionNum != "")
            {

                sqlstr = string.Format(" select name " +
                                                  " from examrecord " +
                                                  " where id='{0}' and modulename='RIS' " , accessionNum);
                sqlCommand = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
                sqlCommand.SelectCommand.CommandType = CommandType.Text;
                SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
                while (SeriesReader.Read())
                {
                    strPatientName =(string) SeriesReader["name"];
                }
                sqlCommand.Dispose();
                SeriesReader.Close();

                sqlstr = string.Format(" select b.SeriesInstanceUID,b.SeriesNumber,a.AccessionNumber " +
                                                   " from studies a,series b " +
                                                   " where a.StudyInstanceUID=b.StudyInstanceUID " +
                                                   " and a.AccessionNumber='{0}' order by SeriesNumber ", accessionNum);
                sqlCommand = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
                sqlCommand.SelectCommand.CommandType = CommandType.Text;
                SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
                while (SeriesReader.Read())
                {
                    arrSeriesString.Add((string)SeriesReader["SeriesInstanceUID"]);
                }
                sqlCommand.Dispose();
                SeriesReader.Close();
                if (arrSeriesString.Count == 0) //根据ACCESSION获取设备类型，影像号，检查日期
                {
                    sqlstr = string.Format(" select machinetype,CONVERT(varchar, submittime, 120 ) AS EXAMDATE, olddocid  " +
                                                 " from  examrecord " +
                                                 " where modulename='RIS' " +
                                                 " and id='{0}'", accessionNum);
                    sqlCommand = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
                    sqlCommand.SelectCommand.CommandType = CommandType.Text;
                    SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
                    while (SeriesReader.Read())
                    {
                        sPatientID = (string)SeriesReader["olddocid"];
                        sModality = (string)SeriesReader["machinetype"];
                        sStudyDate = (string)SeriesReader["examdate"].ToString().Substring(0, 10);
                        break;
                    }
                    sqlCommand.Dispose();
                    SeriesReader.Close();
                }
                else
                {
                    for (int i = 0; i < arrSeriesString.Count; i++)
                    {
                        ArrayList l_list = new ArrayList();
                        string seriesuid = (string)arrSeriesString[i];
                        string sel = string.Format("select ReferencedFile,SOPInstanceUID,SeriesInstanceUID from images " +
                                                    " where SeriesInstanceUID='{0}'" +
                                                    " order by imagenumber ",
                                                   seriesuid);
                        //Loger.Info(sel, "SQL");
                        sqlQueryImages = new SqlDataAdapter(sel, GlobalData.MainConn.ChangeType());
                        sqlQueryImages.SelectCommand.CommandType = CommandType.Text;
                        sqlImagesReader = sqlQueryImages.SelectCommand.ExecuteReader();
                        while (sqlImagesReader.Read())
                            l_list.Add((string)sqlImagesReader["ReferencedFile"]);
                        imageList.Add(l_list);
                        sqlImagesReader.Close();
                        sqlQueryImages.Dispose();
                    }
                    return;
                }
            }
        
           
            sqlstr = string.Format(" select machinetype,CONVERT(varchar, submittime, 120 ) AS EXAMDATE, olddocid  " +
                                            " from  examrecord " +
                                            " where modulename='RIS' " +
                                            " and id='{0}'", accessionNum);
            sqlCommand = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
            sqlCommand.SelectCommand.CommandType = CommandType.Text;
            SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
            while (SeriesReader.Read())
            {
                sPatientID = (string)SeriesReader["olddocid"];
                sModality = (string)SeriesReader["machinetype"];
                sStudyDate = (string)SeriesReader["examdate"].ToString().Substring(0, 10);
                break;
            }

            sqlCommand.Dispose();
            SeriesReader.Close();
            
            //按照设备类型获取图像
            sqlstr = string.Format(" select b.SeriesInstanceUID,b.SeriesNumber,a.AccessionNumber " +
                                            " from studies a,series b " +
                                            " where a.StudyInstanceUID=b.StudyInstanceUID " +
                                            " and a.studydate>='{0} 00:00:00' and a.studydate<='{1} 23:59:59' " +
                                            " and a.patientid='{2}' and b.Modality='{3}' " +
                                            " order by SeriesNumber ",
                                            sStudyDate, sStudyDate, sPatientID, sModality);
            //Loger.Info(sqlstr, "SQL");
            sqlCommand = new SqlDataAdapter(sqlstr, GlobalData.MainConn.ChangeType());
            sqlCommand.SelectCommand.CommandType = CommandType.Text;
            SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
            arrSeriesString.Clear();
            while (SeriesReader.Read())
            {
                arrSeriesString.Add((string)SeriesReader["SeriesInstanceUID"]);
            }
            SeriesReader.Close();
            sqlCommand.Dispose();

            for (int i = 0; i < arrSeriesString.Count; i++)
            {
                ArrayList l_list = new ArrayList();
                string seriesuid = (string)arrSeriesString[i];
                string sel = string.Format("select ReferencedFile,SOPInstanceUID,SeriesInstanceUID from images " +
                                           " where patientid='{0}' " +
                                           " and modality='{1}' " +
                                           " and SeriesInstanceUID='{2}'" +
                                           " order by imagenumber ",
                                           sPatientID, sModality, seriesuid);

                sqlQueryImages = new SqlDataAdapter(sel, GlobalData.MainConn.ChangeType());
                sqlQueryImages.SelectCommand.CommandType = CommandType.Text;
                sqlImagesReader = sqlQueryImages.SelectCommand.ExecuteReader();
                while (sqlImagesReader.Read())
                    l_list.Add((string)sqlImagesReader["ReferencedFile"]);
                imageList.Add(l_list);
                sqlImagesReader.Close();
                sqlQueryImages.Dispose();
            }
            return;
        }


        public void GetDownloadFileNameOracle(string sPatientID, string sModality, string sStudyDate, string accessionNum, ref List<ArrayList> imageList, ref string strPatientName)
        {
            OracleDataAdapter  sqlQueryImages;
            OracleDataReader sqlImagesReader;
            ArrayList arrSeriesString = new ArrayList();
            OracleDataAdapter sqlCommand;
            OracleDataReader SeriesReader;
            string sqlstr;
            strPatientName = "";
            if (accessionNum != "")
            {

                sqlstr = string.Format(" select name " +
                                                " from examrecord " +
                                                " where id='{0}' and modulename='RIS' ", accessionNum);
                sqlCommand = new OracleDataAdapter(sqlstr, GlobalData.MainConn.ChangeTypeOracle());
                sqlCommand.SelectCommand.CommandType = CommandType.Text;
                SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
                while (SeriesReader.Read())
                {
                    strPatientName = (string)SeriesReader["name"];
                }
                sqlCommand.Dispose();
                SeriesReader.Close();


                sqlstr = string.Format(" select b.SeriesInstanceUID,b.SeriesNumber,a.AccessionNumber " +
                                                   " from studies a,series b " +
                                                   " where a.StudyInstanceUID=b.StudyInstanceUID " +
                                                   " and a.AccessionNumber='{0}' order by SeriesNumber ", accessionNum);
                sqlCommand = new OracleDataAdapter(sqlstr, GlobalData.MainConn.ChangeTypeOracle());
                sqlCommand.SelectCommand.CommandType = CommandType.Text;
                SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
                while (SeriesReader.Read())
                {
                    arrSeriesString.Add((string)SeriesReader["SeriesInstanceUID"]);
                }
                sqlCommand.Dispose();
                SeriesReader.Close();
                if (arrSeriesString.Count == 0) //根据ACCESSION获取设备类型，影像号，检查日期
                {
                    ArrayList l_list = new ArrayList();
                    sqlstr = string.Format("select ReferencedFile from images where AccessionNumber='{0}'", accessionNum);

                    //sqlstr = string.Format(" select machinetype, to_char(submittime, 'YYYY-MM-DD') as EXAMDATE, olddocid  " +
                    //                             " from  examrecord " +
                    //                             " where modulename='RIS' " +
                    //                             " and id='{0}'", accessionNum);
                    sqlCommand = new OracleDataAdapter(sqlstr, GlobalData.MainConn.ChangeTypeOracle());
                    sqlCommand.SelectCommand.CommandType = CommandType.Text;
                    SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
                    while (SeriesReader.Read())
                    {
                        l_list.Add((string)SeriesReader["ReferencedFile"]);
                    }
                    sqlCommand.Dispose();
                    SeriesReader.Close();

                    imageList.Add(l_list);
                    return;
                }
                else
                {
                    for (int i = 0; i < arrSeriesString.Count; i++)
                    {
                        ArrayList l_list = new ArrayList();
                        string seriesuid = (string)arrSeriesString[i];
                        string sel = string.Format("select ReferencedFile,SOPInstanceUID,SeriesInstanceUID from images " +
                                                    " where SeriesInstanceUID='{0}'" +
                                                    " order by imagenumber ",
                                                   seriesuid);
                        //Loger.Info(sel, "SQL");
                        sqlQueryImages = new OracleDataAdapter(sel, GlobalData.MainConn.ChangeTypeOracle());
                        sqlQueryImages.SelectCommand.CommandType = CommandType.Text;
                        sqlImagesReader = sqlQueryImages.SelectCommand.ExecuteReader();
                        while (sqlImagesReader.Read())
                            l_list.Add((string)sqlImagesReader["ReferencedFile"]);
                        imageList.Add(l_list);
                        sqlImagesReader.Close();
                        sqlQueryImages.Dispose();
                    }
                    return;
                }
            }

            //按照设备类型获取图像
            sqlstr = string.Format(" select b.SeriesInstanceUID,b.SeriesNumber,a.AccessionNumber " +
                                            " from studies a,series b " +
                                            " where a.StudyInstanceUID=b.StudyInstanceUID " +
                                            " and a.studydate>=to_date('{0} 00:00:00', 'yyyy-mm-dd,hh24:mi:ss')  and a.studydate<=  to_date('{1} 23:59:59', 'yyyy-mm-dd,hh24:mi:ss') " +
                                            " and a.patientid='{2}' and b.Modality='{3}' " +
                                            " order by SeriesNumber ",
                                            sStudyDate, sStudyDate, sPatientID, sModality);
            //Loger.Info(sqlstr, "SQL");
            sqlCommand = new OracleDataAdapter(sqlstr, GlobalData.MainConn.ChangeTypeOracle());
            sqlCommand.SelectCommand.CommandType = CommandType.Text;
            SeriesReader = sqlCommand.SelectCommand.ExecuteReader();
            arrSeriesString.Clear();
            while (SeriesReader.Read())
            {
                arrSeriesString.Add((string)SeriesReader["SeriesInstanceUID"]);
            }
            SeriesReader.Close();
            sqlCommand.Dispose();

            for (int i = 0; i < arrSeriesString.Count; i++)
            {
                ArrayList l_list = new ArrayList();
                string seriesuid = (string)arrSeriesString[i];
                string sel = string.Format("select ReferencedFile,SOPInstanceUID,SeriesInstanceUID from images " +
                                           " where patientid='{0}' " +
                                           " and modality='{1}' " +
                                           " and SeriesInstanceUID='{2}'" +
                                           " order by imagenumber ",
                                           sPatientID, sModality, seriesuid);

                sqlQueryImages = new OracleDataAdapter(sel, GlobalData.MainConn.ChangeTypeOracle());
                sqlQueryImages.SelectCommand.CommandType = CommandType.Text;
                sqlImagesReader = sqlQueryImages.SelectCommand.ExecuteReader();
                while (sqlImagesReader.Read())
                    l_list.Add((string)sqlImagesReader["ReferencedFile"]);
                imageList.Add(l_list);
                sqlImagesReader.Close();
                sqlQueryImages.Dispose();
            }
            return;
        }


        public  bool DownloadDicomImagesFromInfo(string sPatientID, string sModality, string sStudyDate)
        {
            string lRemotefile;
            string lLocalfile;
            List<ArrayList> list = new List<ArrayList>();
            List<string> listFiles = new List<string>();

            if (m_files == null)
                m_files = new List<string>();
            string strPatientName = "";

            foreach (string accessionNum in GlobalData.RunParams.listAccessionNumber)
            {
                if (Conn.isOracle())
                    GetDownloadFileNameOracle(sPatientID, sModality, sStudyDate, accessionNum, ref list, ref strPatientName);
                else
                    GetDownloadFileName(sPatientID, sModality, sStudyDate, accessionNum, ref list, ref strPatientName);
                Platform.Log(LogLevel.Error, "size of file is " + list.Count + " the patient name is " + strPatientName);
                if (list.Count == 0)
                {
                    Platform.Log(LogLevel.Error, "从数据库查出图像数为 0 ！！ " );
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList l_list = list[i];

                    foreach (Object str in l_list)
                    {
                        lRemotefile = (string)str;
                        if (GlobalData.RunParams.ImageSaveLocal == "1")
                        {
                            lLocalfile = GlobalData.DcmLocalPath + lRemotefile;
                        }
                        else
                        {
                            string strTemp = "";
                            int rlex = lRemotefile.LastIndexOf("\\");
                            if (rlex > 0)
                                strTemp = lRemotefile.Substring(rlex + 1);
                            lLocalfile = GlobalData.DcmFilesPath + strTemp;
                        }
                        bool bExistsDicomFile = System.IO.File.Exists(lLocalfile) ? true : GetDicomFileFromServer(lRemotefile, lLocalfile);
                        //bool bExistsDicomFile =  GetDicomFileFromServer(lRemotefile, lLocalfile);
                        if (bExistsDicomFile)
                        {
                            //m_files.Add(lLocalfile);
                            listFiles.Add(lLocalfile);
                        }
                    }
                    m_count++;
                    //if (m_count >4)
                    //{
                    //    //listFilesOld = listFiles;
                    //    continue;
                    //}
                    ProcessDownLoadFile(listFiles, strPatientName);
                }
            }
            //if (m_count > 4) ProcessDownLoadFile(listFiles, strPatientName);
            return true;
        }

        /// <summary>
        /// process the download file 
        /// </summary>
        /// <param name="list"> That contains The name of the file.</param>
       
        public virtual void  ProcessDownLoadFile(List<string> list, string PatientName)
        {
            foreach (string str in list)
                m_files.Add(str);
            list.Clear();
            this.PatientName = PatientName;
        }

        public bool GetDicomFileFromServer(string remotefile, string localfile)
        {
            try
            {
                ClassFtpSocketClient sk = new ClassFtpSocketClient();
                bool bSucc = sk.RecvDataFromFtpServer(GlobalData.RunParams.RemoteIP, GlobalData.RunParams.RemotePort, remotefile);
                if (bSucc)
                {
                    byte[] l_bytes = sk.GetBytes();
                    System.IO.FileStream l_filestream = System.IO.File.Create(localfile);
                    l_filestream.Write(l_bytes, 0, l_bytes.Length);
                    l_filestream.Close();
                }
                return bSucc;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void ConnectDataBase()
        {
            Conn.setServer(GlobalData.DBParams.DBServer);
            Conn.setUid(GlobalData.DBParams.DBUser);
            Conn.setPwd(GlobalData.DBParams.DBPwd);
            Conn.setDatabase(GlobalData.DBParams.DBName);
            GlobalData.MainConn = new Conn();
            try
            {
                GlobalData.MainConn.Open();
                GlobalData.bDBConnected = true;
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, "connect db was failed.." + ex.ToString());
            }
        }

        public void DownloadImages()
        {
            //GlobalData.RunParams = GlobalData.AnalyzeMainNextCall();
            m_count = 0;
            try
            {
                if (GlobalData.MainConn.isConnectedBroken())
                {
                    GlobalData.MainConn.Close();
                    ConnectDataBase();
                }
                DownloadDicomImagesFromInfo(GlobalData.RunParams.PatientID, GlobalData.RunParams.Modality, GlobalData.RunParams.StudyDate);
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex.ToString());
            }
        }
    }
}
