
using System;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Collections.Generic;

namespace Global.Data
{
    public struct CallParams
    {
        public string RemoteAET;
        public string RemoteIP;
        public int RemotePort;
        public string PatientID;
        public string PatientName;
        public string Modality;
        public string StudyDate;
        public string AccessionNumber;
        public string ShowStyle;
        public string AppTitle;
        public string CallerHwnd;
        public string RunMode;
        public string ImageSaveLocal;
        public string [] listAccessionNumber;
       
    }

    public struct RisTags
    {
        public string RisPatientID;
        public string RisPatientName;
        public string RisMachineType;
        public string RisPatientAge;
        public string RisPatientSex;
        public string RisBirthday;
        public string RisExamDate;
        public string RisExamTime;
        public string RisVisceras;
        public string RisHospital;
        public string RisManufacturer;
        public bool bInvertImage;
        public bool bSucc;
    }

    public struct DBConnection
    {
        public string DBServer;
        public string DBName;
        public string DBUser;
        public string DBPwd;
        public string DBType;
    }

    public struct FormPosition
    {
        public int WinLeft;
        public int WinTop;
        public int WinWidth;
        public int WinHeight;
    }

    public struct WorkstationAETitle
    {
        public string ServerAE;
        public string ServerIP;
        public string ServerPort;
        public string ClientAE;
        public string ClientPort;
    }

    //��ʾ��Ϣ
    public class TagInformation
    {
        public NameValueCollection TagLeftContainer = new NameValueCollection();
        public NameValueCollection TagRightContainer = new NameValueCollection();
        public NameValueCollection TagLeftBottomContainer = new NameValueCollection();
        public NameValueCollection TagRightBottomContainer = new NameValueCollection();
        public bool bInvertImage = false;
        public TagInformation()
        {

        }
    }

    public static class GlobalData
    {
        public static WorkstationAETitle AEtitle; //����IHEC��˭��

        public static string PacsIniFile;//�������������
        public static string EfilmIniFile;//������������        
        public static CallParams RunParams;//����
        public static DBConnection DBParams;//���ݿ�����
        public static FormPosition PosParams;//����λ��
        public static bool ReCallOwner = false;//���¿��´���

        public static int SeriesRows = 1;
        public static int SeriesColumns = 2;
        public static int ImageRows = 2;
        public static int ImageColumns = 2;
        public static string DcmFilesPath = null;
        public static string DcmPrintPath = null;//��ӡ�ļ�·��
        public static string DcmLocalPath = null;
        public static string Sensitivity;//����λ��Ӧ��
        public static string[] GatewayModality;//ģ��ת�����豸�б�
        public static bool bDBConnected = false;
        public static bool bReShowDicom = false;
        public const int c_TitlebarIndex = 7;
        public static bool bDisplayRulers = false;
        public static bool bShowTags = true;
        public static bool bShowRisTags = true;
        public static bool bSeriesSync = false;   //
        public static bool bShowSerisFlag = true; //������������ʾ����
        public static bool bDoubleMonitor = false; //˫��
        public static int ScreenNumber = 0; //��Ļ
        public static int MonitorNum1 = 0; //��Ļ
        public static int MonitorNum2 = 0; //��Ļ

        public static Conn MainConn = null;
        public static ArrayList arrCurrentImages = new ArrayList();
        public static ArrayList arrRisTags = new ArrayList();

        public static SortedDictionary<string, TagInformation> myTagDic = new SortedDictionary<string, TagInformation>();

        public static NameValueCollection TagCnEnMap = new NameValueCollection();
        public static NameValueCollection TagModalityMap = new NameValueCollection();
        

        public static Keys[] keys =
         {
             
            //Keys.F1,
            //Keys.F2,
            //Keys.F3,
            //Keys.F4,
            //Keys.F5,
            //Keys.F6,            
            Keys.F7,
            Keys.F8,
            Keys.F9,
            Keys.F10,
            Keys.F11,
            Keys.F12,
            Keys.None,
            Keys.PageUp,
            Keys.PageDown,
            Keys.End,
            Keys.Home
         };



       

        public static void arrCurrentImagesClear()
        {
            
            arrCurrentImages.Clear();
            arrRisTags.Clear();
        }

 
        public static CallParams AnalyzeMain(string[] args)
        {
            CallParams _params = new CallParams();
            _params.AccessionNumber = "";
            foreach (string s in args)
            {
                string[] ss = s.Split('=');
                if (ss[0].ToUpper() == "REMOTEAET") _params.RemoteAET = ss[1];
                if (ss[0].ToUpper() == "REMOTEHOST") _params.RemoteIP = ss[1];
                if (ss[0].ToUpper() == "REMOTEPORT") _params.RemotePort = int.Parse(ss[1]);
                //if (ss[0].ToUpper() == "OLDDOCID") _params.PatientID = ss[1];
                if (ss[0].ToUpper() == "MODALITY") _params.Modality = ss[1];
                if (ss[0].ToUpper() == "DATETIME") _params.StudyDate = ss[1];
                if (ss[0].ToUpper() == "ACCESSIONNUMBER")
                {
                    string[] stmp = ss[1].Split(',');
                    if (stmp.Length == 1)
                        _params.AccessionNumber = ss[1];
                    _params.listAccessionNumber = stmp;
                }
                if (ss[0].ToUpper() == "SHOWSTYLE") _params.ShowStyle = ss[1];
                if (ss[0].ToUpper() == "APPTITLE") _params.AppTitle = ss[1];
                if (ss[0].ToUpper() == "RUNMODE") _params.RunMode = ss[1];
                if (ss[0].ToUpper() == "CALLERHWND") _params.CallerHwnd = ss[1];
                if (ss[0].ToUpper() == "IMAGESAVELOCAL") _params.ImageSaveLocal = ss[1];    
                //added by luojiang
                if (ss[0].ToUpper() == "PATIENTID")
                {
                    _params.PatientID = ss[1];
                }
                if (ss[0].ToUpper() == "OLDDOCID")
                {
                    if (ss[1].Length != 0)
                        _params.PatientID = ss[1];
                }
            }
            ReCallOwner = _params.ShowStyle == "2";
            return _params;
        }

        public static CallParams AnalyzeMainNextCall()
        {
            IniFiles inifile = new IniFiles(GlobalData.PacsIniFile);
            string CallStr = inifile.ReadString("PARAM", "DICOMPARAM", "");
            string[] argsCall = CallStr.Split(' ');
            return AnalyzeMain(argsCall);
        }

        public static void ReadIniFile()
        {
            try
            {
                IniFiles inifile = new IniFiles(GlobalData.PacsIniFile);
                //��ȡ���ݿ����Ӳ���
                DBParams.DBServer = inifile.ReadString("DataBase", "DBServer", ".");
                DBParams.DBName = inifile.ReadString("DataBase", "DBName", "RIS451");
                DBParams.DBUser = inifile.ReadString("DataBase", "DBUser", "sa");
                DBParams.DBPwd = inifile.ReadString("DataBase", "DBPwd", "masterkey");
                DBParams.DBType = inifile.ReadString("DataBase", "DBTYPE", "");

                //����Ӱ��·��                
                DcmLocalPath = inifile.ReadString("System", "AVIPath", @"D:\Images");                
                if (ReCallOwner)
                {
                    PosParams.WinLeft = inifile.ReadInteger("DicomParam2", "WINDOWLEFT", 0);
                    PosParams.WinTop = inifile.ReadInteger("DicomParam2", "WINDOWTOP", 0);
                    PosParams.WinWidth = inifile.ReadInteger("DicomParam2", "WINDOWWIDTH", 1024);
                    PosParams.WinHeight = inifile.ReadInteger("DicomParam2", "WINDOWHEIGHT", 768);
                }
                else
                {
                    PosParams.WinLeft = inifile.ReadInteger("DicomParam", "WINDOWLEFT", 0);
                    PosParams.WinTop = inifile.ReadInteger("DicomParam", "WINDOWTOP", 0);
                    PosParams.WinWidth = inifile.ReadInteger("DicomParam", "WINDOWWIDTH", 1024);
                    PosParams.WinHeight = inifile.ReadInteger("DicomParam", "WINDOWHEIGHT", 768);
                }

                IniFiles efilmini = new IniFiles(GlobalData.EfilmIniFile);
                if (GlobalData.RunParams.Modality != null)
                {
                    SeriesRows = efilmini.ReadInteger(GlobalData.RunParams.Modality, "SeriesRows", 1);
                    SeriesColumns = efilmini.ReadInteger(GlobalData.RunParams.Modality, "SeriesColumns", 2);
                    bShowSerisFlag = efilmini.ReadString(GlobalData.RunParams.Modality, "SeriesFlag", "Yes") == "Yes";
                }
                else
                {
                    SeriesRows = 1;
                    SeriesColumns = 2;
                } 
                ImageRows = efilmini.ReadInteger("System", "ImageRows", 2);
                ImageColumns = efilmini.ReadInteger("System", "ImageColumns", 2);
                
                DcmFilesPath = Application.StartupPath + @"\DicomFiles\";
                DcmPrintPath = Application.StartupPath + @"\PrintFiles\";
                efilmini.WriteString("System", "DicomFilesPath", DcmFilesPath);
                efilmini.WriteString("System", "PrintFilesPath", DcmFilesPath);

                GatewayModality = efilmini.ReadString("System", "DicomGateway", "�ڰ׳�,�ʳ�").Split(',');
                Sensitivity = efilmini.ReadString("System", "Sensitivity", "");
                bShowRisTags = efilmini.ReadString("System", "ShowRisTags", "Yes") == "Yes";
                

                //��ȡ�����ļ����Ű���Ϣ
                const string strModalityTemp = "DX,CR,CT,MR,RF,MG,DR,DSA,US,OT";
                string [] strModality = strModalityTemp.Split(',');
                try
                {
                    foreach (string strTemp in strModality)
                    {
                        bool l_bflag = false;
                        int l_flag = efilmini.ReadInteger(strTemp, "InvertImage", 0);
                        if (l_flag == 1)
                            l_bflag = true;
                        string strAliasModality = efilmini.ReadString(strTemp,strTemp, "");
                        if (strAliasModality != "")
                        {
                            TagModalityMap.Add(strTemp, strAliasModality);
                        }
                        int count = efilmini.ReadInteger(strTemp, "COUNT", 0);
                        if (count == 0)
                            continue;
                        TagInformation taginfo = new TagInformation();
                        taginfo.bInvertImage = l_bflag;

                        string strTemper = null;
                        for (int i = 1; i < count + 1; i++)
                        {
                            strTemper = efilmini.ReadString(strTemp, i.ToString(), "");
                            if (strTemper != "")
                            {
                                string strOrient = strTemper.Split(',')[0];
                                if (strOrient == "LeftTop")
                                    taginfo.TagLeftContainer.Add(strTemper.Split(',')[1], strTemper.Split(',')[2]);
                                else if (strOrient == "RightTop")
                                    taginfo.TagRightContainer.Add(strTemper.Split(',')[1], strTemper.Split(',')[2]);
                                else if (strOrient == "LeftBottom")
                                    taginfo.TagLeftBottomContainer.Add(strTemper.Split(',')[1], strTemper.Split(',')[2]);
                                else if (strOrient == "RightBottom")
                                    taginfo.TagRightBottomContainer.Add(strTemper.Split(',')[1], strTemper.Split(',')[2]);
                            }
                        }
                        if (count > 0)
                            myTagDic.Add(strTemp, taginfo);
                    }
                    int TagCnEnSize = efilmini.ReadInteger("TagName", "COUNT", 0);
                    for (int i = 1; i < TagCnEnSize + 1; i++)
                    {
                        string strTemp = efilmini.ReadString("TagName", i.ToString(), "");
                        if (strTemp != "")
                        {
                            TagCnEnMap.Add(strTemp.Split(',')[0].ToString(), strTemp.Split(',')[1].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("�Ű���Ϣ���ô���:" + ex.ToString());
                    Loger.Info("�Ű���Ϣ���ô���" + ex.ToString(), "aa");
                }

                Directory.CreateDirectory(DcmPrintPath);
                Directory.CreateDirectory(DcmFilesPath);

                ScreenNumber = efilmini.ReadInteger("System", "ScreenNo", 0);
                MonitorNum1 = efilmini.ReadInteger("System", "Monitor1", 0);
                MonitorNum2 = efilmini.ReadInteger("System", "Monitor2", 0);
                AEtitle.ServerAE = efilmini.ReadString("AETITLE", "ServerAE", "DICOMSERVER");
                AEtitle.ServerIP = efilmini.ReadString("AETITLE", "ServerIP", "127.0.0.1");
                AEtitle.ServerPort = efilmini.ReadString("AETITLE", "ServerPORT", "5678");
                AEtitle.ClientAE = efilmini.ReadString("AETITLE", "ClientAE", "WORKSTATION");
                AEtitle.ClientPort = efilmini.ReadString("AETITLE", "ClientPort", "109");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void WriteIniFile()
        {
            if ((PosParams.WinLeft > -4096) && (PosParams.WinTop > -4096))
            {
                IniFiles inifile = new IniFiles(GlobalData.PacsIniFile);
                if (ReCallOwner)
                {
                    inifile.WriteInteger("DicomParam2", "WINDOWLEFT", PosParams.WinLeft);
                    inifile.WriteInteger("DicomParam2", "WINDOWTOP", PosParams.WinTop);
                    inifile.WriteInteger("DicomParam2", "WINDOWWIDTH", PosParams.WinWidth);
                    inifile.WriteInteger("DicomParam2", "WINDOWHEIGHT", PosParams.WinHeight);
                }
                else
                {
                    inifile.WriteInteger("DicomParam", "WINDOWLEFT", PosParams.WinLeft);
                    inifile.WriteInteger("DicomParam", "WINDOWTOP", PosParams.WinTop);
                    inifile.WriteInteger("DicomParam", "WINDOWWIDTH", PosParams.WinWidth);
                    inifile.WriteInteger("DicomParam", "WINDOWHEIGHT", PosParams.WinHeight);
                }
            }
        }

        public static bool StrInArray(string str, string[] strarry)
        {
            if (str == null) return false;
            if (strarry == null || strarry.Length == 0) return false;
            for (int i = 0; i < strarry.Length; i++)
            {
                if (strarry[i] == null) continue;
                if (str == strarry[i]) return true;
            }
            return false;
        }
    }

    public class Loger
    {
        private static volatile Loger _instance = null;
        private static object _syncRoot = new Object();

        private string LogFile = Application.StartupPath + @"\EFilm.log";
        private StreamWriter LogWrite = null;

        public static Loger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Loger();
                    }
                }
                return _instance;
            }
        }

        private Loger()
        {
            try
            {
                LogWrite = new StreamWriter(LogFile, true);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void _add(string type, string title, string info)
        {
            if (null != LogWrite && LogWrite.BaseStream.CanWrite)
            {
                LogWrite.WriteLine("[{0} {1}] {2}: {3} - {4}", DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString(), type, title, info);
                LogWrite.Flush();
            }
        }

        public static void Error(string info, string title)
        {
            Instance._add("����", title, info);
        }

        public static void Alert(string info, string title)
        {
            Instance._add("����", title, info);
        }

        public static void Info(string info, string title)
        {
            Instance._add("��ʾ", title, info);
        }

        ~Loger()
        {
            if (null != LogWrite && LogWrite.BaseStream.CanWrite)
            {
                LogWrite.Close();
                LogWrite.Dispose();
            }
        }
    }

    public class IniFiles
    {
        public string FileName;
        //������дINI�ļ���API����
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        public IniFiles(string AFileName)
        {
            // �ж��ļ��Ƿ����
            FileInfo fileInfo = new FileInfo(AFileName);
            if ((!fileInfo.Exists))
            {
                //�ļ������ڣ������ļ�
                System.IO.StreamWriter sw = new System.IO.StreamWriter(AFileName, false, System.Text.Encoding.Default);
                try
                {
                    sw.Write("#���õ���");
                    sw.Close();
                }
                catch
                {
                    throw (new ApplicationException("Ini�ļ�������"));
                }
            }
            FileName = fileInfo.FullName;
        }
        //дINI�ļ�
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Value, FileName))
            {

                throw (new ApplicationException("дIni�ļ�����"));
            }
        }
        //��ȡINI�ļ�ָ��
        public string ReadString(string Section, string Ident, string Default)
        {
            Byte[] Buffer = new Byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
            
            string s = Encoding.GetEncoding(0).GetString(Buffer,0, bufLen);
            //Encoding.GetEncoding("GB2312")
            
            return s.Trim();
        }

        //������
        public int ReadInteger(string Section, string Ident, int Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(intStr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        //д����
        public void WriteInteger(string Section, string Ident, int Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }

        //������
        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        //дBool
        public void WriteBool(string Section, string Ident, bool Value)
        {
            WriteString(Section, Ident, Convert.ToString(Value));
        }

        //��Ini�ļ��У���ָ����Section�����е�����Ident��ӵ��б���
        public void ReadSection(string Section, StringCollection Idents)
        {
            Byte[] Buffer = new Byte[16384];
            //Idents.Clear();
            int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
             FileName);
            //��Section���н���
            GetStringsFromBuffer(Buffer, bufLen, Idents);
        }
        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
        //��Ini�ļ��У���ȡ���е�Sections������
        public void ReadSections(StringCollection SectionList)
        {
            byte[] Buffer = new byte[65535];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, Buffer,
             Buffer.GetUpperBound(0), FileName);
            GetStringsFromBuffer(Buffer, bufLen, SectionList);
        }
        //��ȡָ����Section������Value���б���
        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection KeyList = new StringCollection();
            ReadSection(Section, KeyList);
            Values.Clear();
            foreach (string key in KeyList)
            {
                Values.Add(key, ReadString(Section, key, ""));
            }
        }
        /**/
        ////��ȡָ����Section������Value���б��У�
        //public void ReadSectionValues(string Section, NameValueCollection Values,char splitString)
        //{�� string sectionValue;
        //����string[] sectionValueSplit;
        //����StringCollection KeyList = new StringCollection();
        //����ReadSection(Section, KeyList);
        //����Values.Clear();
        //����foreach (string key in KeyList)
        //����{
        //��������sectionValue=ReadString(Section, key, "");
        //��������sectionValueSplit=sectionValue.Split(splitString);
        //��������Values.Add(key, sectionValueSplit[0].ToString(),sectionValueSplit[1].ToString());

        //����}
        //}
        //���ĳ��Section
        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, FileName))
            {
                throw (new ApplicationException("�޷����Ini�ļ��е�Section"));
            }
        }
        //ɾ��ĳ��Section�µļ�
        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, FileName);
        }   

        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, FileName);
        }

        //���ĳ��Section�µ�ĳ����ֵ�Ƿ����
        public bool ValueExists(string Section, string Ident)
        {
            //
            StringCollection Idents = new StringCollection();
            ReadSection(Section, Idents);
            return Idents.IndexOf(Ident) > -1;
        }

        //ȷ����Դ���ͷ�
        ~IniFiles()
        {
            UpdateFile();
        }
    }
    /// ���ݿ������� 
    public class Conn
    {
        private SqlConnection myConn;
        private OracleConnection oracleConn;
        private string ServerStr;
        static string server;
        static string uid;
        static string pwd;
        static string database;

        static bool b_IsOracle;
        public Conn()
        {
            if (GlobalData.DBParams.DBType == "oracle" || GlobalData.DBParams.DBType == "ORACLE")
            {
                //ServerStr = "server=" + server + ";uid=" + uid + ";password=" + pwd + ";database=" + database;
                string oracleString = "Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST=" + server + ")(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=" + database + ")));User Id=" + uid + ";Password=" + pwd;
                oracleConn = new OracleConnection(ServerStr);
                b_IsOracle = true;
            }
            else
            {
                b_IsOracle = false;
                ServerStr = "server=" + server + ";uid=" + uid + ";password=" + pwd + ";database=" + database;
                myConn = new SqlConnection(ServerStr);
            }
        }
        public void Open()
        {
            if (b_IsOracle)
                oracleConn.Open();
            else
                myConn.Open();
        }
        
        public int tOpen()
        {
            try
            {
                myConn.Open();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public void Close()
        {
            if (b_IsOracle)
                oracleConn.Close();
            else
                myConn.Close();
        }
        /// ��������ת��ΪSqlConnection�� 
        /// SqlConnection 
        public SqlConnection ChangeType()
        {
            return myConn;

        }

        public OracleConnection ChangeTypeOracle()
        {
            return oracleConn;
        }

        public static void setServer(string Server)
        {
            server = Server;
        }
        public static void setUid(string Uid)
        {
            uid = Uid;
        }
        public static void setPwd(string Pwd)
        {
            pwd = Pwd;
        }
        public static void setDatabase(string Database)
        {
            database = Database;
        }

        public static bool isOracle()
        {
            return b_IsOracle;
        }
    }
}
