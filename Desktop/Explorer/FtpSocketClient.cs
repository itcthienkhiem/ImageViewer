using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace Global.FtpSocketClient
{
    /*
    * 用于向服务器下载，上传数据类
    * Auther: Womsoft
    * 修改时间： 2010-12-13，针对协议作出部分修改
    */
    class ClassFtpSocketClient
    {
        private Socket m_client = null;
        private string m_strHead = null;
        private string m_strFileName = null;
        private string m_strDirectName = null;
        private int m_iFileLength = 0;
        private StringBuilder m_strRecvStream = new StringBuilder(4096);
        private byte[] m_ByteData = new byte[4096];


        public ClassFtpSocketClient()
        {
            m_strHead = "EEEE";
        }

        //获取文件字符串流
        public StringBuilder GetStreamData()
        {
            return m_strRecvStream;
        }

        //获取返回文件的字节流
        public byte[] GetBytes()
        {
            char[] l_TempChar = m_strRecvStream.ToString().ToCharArray();
            byte[] l_TempByte = new byte[l_TempChar.Length];
            for (int i = 0; i < l_TempByte.Length; i++)
                l_TempByte[i] = (byte)l_TempChar[i];
            return l_TempByte;

        }

        //接收文件服务器下载文件，返回true, 表示成功，返回false 表示失败
        public bool RecvDataFromFtpServer(string strip, int port, string remotefile)
        {
            m_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            m_strRecvStream.Remove(0, m_strRecvStream.Length);

            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(strip), port);

                m_client.Connect(ipep);
                string strPackageData = makeGetPackage(remotefile);
                int len = m_client.Send(Encoding.ASCII.GetBytes(strPackageData));
                StringBuilder l_strBuiler = new StringBuilder(4096);
                if (RecvDataFromServer(l_strBuiler))
                {
                    if (ProcessIntoMemory(l_strBuiler))
                    {
                        //循环接收文件
                        while (RecvDataFromServer(l_strBuiler))
                        {
                            m_strRecvStream.Append(l_strBuiler);
                            m_iFileLength -= l_strBuiler.Length;
                            if (m_iFileLength == 0)
                            {
                                return true;
                            }
                        }
                    }
                    else
                        return false;

                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                if (m_client != null)
                {
                    try
                    {
                        m_client.Shutdown(SocketShutdown.Both);
                        m_client.Close();
                    }
                    catch (Exception)
                    {

                    }
                }

            }
            return false;
        }

        //专门用于接收网络数据，超时时间设置为40秒
        public bool RecvDataFromServer(StringBuilder strRecvData)
        {
            try
            {
                m_client.ReceiveTimeout = 1000 * 40;
                int len = m_client.Receive(m_ByteData);
                if (len <= 0)
                    return false;
                //清空
                strRecvData.Remove(0, strRecvData.Length);
                //进行格式转换！
                char[] m_TempData = new char[len];
                for (int i = 0; i < len; i++)
                {
                    m_TempData[i] = (char)m_ByteData[i];
                }
                strRecvData.Append(m_TempData);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //处理接收过来的数据流，进行解包分析
        public bool ProcessIntoMemory(StringBuilder strRecvData)
        {
            int npos = 0;
            int nDataLen = strRecvData.Length;
            npos = strRecvData.ToString().IndexOf("EEEE");

            if (nDataLen > 10 && (npos == 0))
            {
                int len = int.Parse(strRecvData.ToString().Substring(4, 6));
                if (nDataLen >= len)
                {
                    string strTemp = strRecvData.ToString().Substring(0, len);
                    //如果文件存在
                    if (ProcessData(strTemp))
                    {
                        strRecvData.Remove(0, len);

                        //表示后面的文件内容
                        if (strRecvData.Length > 0)
                        {
                            m_iFileLength -= strRecvData.Length;
                            m_strRecvStream.Append(strRecvData);

                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else if (strRecvData.Length > 10)
            {
                return false;
            }

            return true;
        }


        //处理数据包内容，包括文件长度，内容，文件名称
        public bool ProcessData(string strData)
        {
            string strType = strData.Substring(10, 2);

            //删除前面的头及信息
            strData = strData.Remove(0, 12);
            while (strData.Length != 0)
            {
                string strKey = strData.Substring(0, 3);
                int keyLen = int.Parse(strData.Substring(3, 3));
                if (strKey.Equals("001"))
                {
                    m_strFileName = strData.Substring(6, keyLen);
                }
                else if (strKey.Equals("002"))
                {
                    m_iFileLength = int.Parse(strData.Substring(6, keyLen));
                }
                else if (strKey.Equals("003"))
                {
                    m_strDirectName = strData.Substring(6, keyLen);
                }
                strData = strData.Remove(0, 6 + keyLen);
            }
            //文件不存在
            if (strType.Equals("02"))
            {
                return false;
            }
            //文件存在
            else if (strType.Equals("03"))
            {
                return true;
            }
            //目录存在
            else if (strType.Equals("09"))
            {
                return true;
            }
            //其它错误
            else
            {
                return false;
            }

        }

        //产生要下载的数据包
        public string makeGetPackage(string strfilename)
        {
            string strData = null;

            strData += "01";
            strData += "001";
            strData += string.Format("{0, 0:d3}", strfilename.Length);
            strData += strfilename;
            strData += "002";
            strData += "000";
            string strPackLen = string.Format("{0, 0:d6}", (10 + strData.Length));

            return m_strHead + strPackLen + strData;
        }

        //产生要上送的数据包
        public string makePutPackage(string strfilename, long filelength)
        {

            string strData = null;

            strData += "00"; //上传文件请求
            strData += "001"; //上传文件的请求包体
            strData += string.Format("{0, 0:d3}", strfilename.Length);
            strData += strfilename;
            strData += "002";

            string strTemp = string.Format("{0}", filelength);
            strData += string.Format("{0, 0:d3}", strTemp.Length); // KEY LENGTH VALUE
            strData += strTemp;

            string strPackLen = string.Format("{0, 0:d6}", (10 + strData.Length));
            return m_strHead + strPackLen + strData;

        }

        //文件服务器上传文件，返回true, 表示成功，返回false 表示失败
        public bool PutFileIntoFtpServer(string strip, int port, string remotefilename, Stream localStream)
        {
            localStream.Position = 0;
            m_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(strip), port);

                m_client.Connect(ipep);
                string strPackageData = makePutPackage(remotefilename, localStream.Length);
                int len = m_client.Send(Encoding.ASCII.GetBytes(strPackageData));
                StringBuilder l_strBuiler = new StringBuilder(4096);
                if (RecvDataFromServer(l_strBuiler))
                {
                    if ((l_strBuiler.ToString().IndexOf("EEEE") == 0) &&
                        (l_strBuiler.ToString().Substring(10, 2).Equals("04")))
                    {
                        Stream stream = localStream;
                        int nReadNumbers = 0;

                        while ((nReadNumbers = stream.Read(m_ByteData, 0, m_ByteData.Length)) > 0)
                            m_client.Send(m_ByteData);
                    }
                    else
                        return false;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                if (m_client != null)
                {
                    try
                    {
                        m_client.Shutdown(SocketShutdown.Both);
                        m_client.Close();
                    }
                    catch (Exception)
                    {

                    }
                }

            }
            return false;
        }

    }
}
