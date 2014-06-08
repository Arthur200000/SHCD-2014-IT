namespace Qisi.General
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
	/// <summary>
	/// FTP client.
	/// </summary>
    public class FTPClient
    {
        private string currentDir = "/";
        private List<string> DirList = null;
        private List<string> FileList = null;
        private const int ftpport = 21;
        private string ftpUristring = null;
        private NetworkCredential networkCredential;
		/// <summary>
		/// Occurs when log.
		/// </summary>
        public event TextEventHandler Log;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.FTPClient"/> class.
		/// </summary>
		/// <param name="ip">Ip to target, as string.</param>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
        public FTPClient(string ip, string username, string password)
        {
            this.ftpUristring = "ftp://" + ip;
            this.networkCredential = new NetworkCredential(username, password);
            this.DirList = new List<string>();
            this.FileList = new List<string>();
        }

        private void AddInfo(string str)
        {
            try
            {
                this.Log(this, new MessageEventArgs(str));
            }
            catch
            {
            }
        }
		/// <summary>
		/// Changes the dir, FTP cd.
		/// </summary>
		/// <param name="newDir">New dir.</param>
        public void changeDir(string newDir)
        {
            if (newDir == "")
            {
                if (this.currentDir == "/")
                {
                    this.AddInfo("当前目录已经是顶层目录");
                }
                else
                {
                    int length = this.currentDir.LastIndexOf("/");
                    if (length == 0)
                    {
                        this.currentDir = "/";
                    }
                    else
                    {
                        this.currentDir = this.currentDir.Substring(0, length);
                    }
                    this.ShowFtpFileAndDirectory();
                }
            }
            else if (this.DirList.Contains(newDir.PadRight (34, ' ')))
            {
                if (this.currentDir == "/")
                {
                    this.currentDir = "/" + newDir;
                }
                else
                {
                    this.currentDir = this.currentDir + "/" + newDir;
                }
                string[] strArray = this.currentDir.Split(new char[] { ' ' });
                this.currentDir = strArray[0];
                this.ShowFtpFileAndDirectory();
            }
        }

        private FtpWebRequest CreateFtpWebRequest(string uri, string requestMethod)
        {
            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(uri);
            request.Credentials = this.networkCredential;
            request.KeepAlive = true;
            request.UseBinary = true;
            request.Method = requestMethod;
            request.Proxy = null;
            return request;
        }

        private bool Delete(string filename)
        {
            if (!this.FileList.Contains(filename.PadRight (34, ' ')))
            {
                this.AddInfo("要删除的文件不存在");
                return false;
            }
            try
            {
                string uriString = this.GetUriString(filename);
                FtpWebRequest request = this.CreateFtpWebRequest(uriString, "DELE");
                if (this.GetFtpResponse(request) == null)
                {
                    return false;
                }
                this.ShowFtpFileAndDirectory();
                return true;
            }
            catch (WebException exception)
            {
                this.AddInfo(exception.Message + " 删除失败");
                return false;
            }
        }
		/// <summary>
		/// Download the specified fileName from filePath, FTP get.
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="filePath">File path.</param>
        public bool download(string fileName, string filePath)
        {
            if (!this.FileList.Contains(fileName.PadRight (34, ' ')))
            {
                this.AddInfo("要下载的文件不存在");
                return false;
            }
            try
            {
                string uriString = this.GetUriString(fileName);
                FtpWebRequest request = this.CreateFtpWebRequest(uriString, "RETR");
                FtpWebResponse ftpResponse = this.GetFtpResponse(request);
                if (ftpResponse == null)
                {
                    this.AddInfo("服务器未响应...");
                    return false;
                }
                Stream responseStream = ftpResponse.GetResponseStream();
                FileStream stream2 = System.IO.File.Create(filePath);
                int count = 8196;
                byte[] buffer = new byte[count];
                int num2 = 1;
                this.AddInfo("打开下载通道，文件下载中...");
                while (num2 != 0)
                {
                    num2 = responseStream.Read(buffer, 0, count);
                    stream2.Write(buffer, 0, num2);
                }
                responseStream.Close();
                stream2.Close();
                this.AddInfo(string.Concat(new object[] { "下载完毕，服务器返回：", ftpResponse.StatusCode, " ", ftpResponse.StatusDescription }));
                return true;
            }
            catch (WebException exception)
            {
                this.AddInfo("发生错误，返回状态为：" + exception.Status);
                return false;
            }
        }

        private FtpWebResponse GetFtpResponse(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse) request.GetResponse();
                this.AddInfo("验证完毕，服务器回应信息：[" + response.WelcomeMessage + "]");
                this.AddInfo("正在连接：[ " + response.BannerMessage + "]");
                return response;
            }
            catch (WebException exception)
            {
                this.AddInfo("发送错误。返回信息为：" + exception.Status);
                return null;
            }
        }

        private string GetUriString(string filename)
        {
            if (this.currentDir.EndsWith("/"))
            {
                return (this.ftpUristring + this.currentDir + filename);
            }
            return (this.ftpUristring + this.currentDir + "/" + filename);
        }
		/// <summary>
		/// Login.
		/// </summary>
        public bool login()
        {
            if (this.ShowFtpFileAndDirectory())
            {
                this.AddInfo("登录成功");
                return true;
            }
            this.AddInfo("登录失败");
            return false;
        }
		/// <summary>
		/// Logout.
		/// </summary>
        public void logout()
        {
            this.CreateFtpWebRequest(this.ftpUristring, "QUIT");
        }
		/// <summary>
		/// mkdir.
		/// </summary>
		/// <param name="newDir">New dir.</param>
        public void makeDir(string newDir)
        {
            this.ShowFtpFileAndDirectory();
            if (newDir == "")
            {
                if (this.currentDir == "/")
                {
                    this.AddInfo("当前目录已经是顶层目录");
                }
                else
                {
                    int length = this.currentDir.LastIndexOf("/");
                    if (length == 0)
                    {
                        this.currentDir = "/";
                    }
                    else
                    {
                        this.currentDir = this.currentDir.Substring(0, length);
                    }
                    this.ShowFtpFileAndDirectory();
                }
            }
            else if (this.DirList.Contains(newDir.PadRight (34, ' ')))
            {
                if (this.currentDir == "/")
                {
                    this.currentDir = "/" + newDir;
                }
                else
                {
                    this.currentDir = this.currentDir + "/" + newDir;
                }
                string[] strArray = this.currentDir.Split(new char[] { ' ' });
                this.currentDir = strArray[0];
            }
            else
            {
                string uriString = this.GetUriString(newDir);
                FtpWebRequest request = this.CreateFtpWebRequest(uriString, "MKD");
                Stream responseStream = this.GetFtpResponse(request).GetResponseStream();
                this.ShowFtpFileAndDirectory();
                this.changeDir(newDir);
            }
        }

        private bool ShowFtpFileAndDirectory()
        {
            int num2;
            int num3;
            string str3;
            string[] strArray3;
            int length;
            string uri = string.Empty;
            if (this.currentDir == "/")
            {
                uri = this.ftpUristring;
            }
            else
            {
                uri = this.ftpUristring + this.currentDir;
            }
            uri = uri.Split(new char[] { ' ' })[0];
            FtpWebRequest request = this.CreateFtpWebRequest(uri, "LIST");
            FtpWebResponse ftpResponse = this.GetFtpResponse(request);
            if (ftpResponse == null)
            {
                return false;
            }
            this.AddInfo(string.Concat(new object[] { "连接成功，服务器返回的是：", ftpResponse.StatusCode, " ", ftpResponse.StatusDescription }));
            Stream responseStream = ftpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            this.AddInfo("获取响应流....");
            string str2 = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            ftpResponse.Close();
            this.AddInfo("传输完成");
            this.DirList.Clear();
            this.FileList.Clear();
            string[] strArray2 = str2.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            for (num2 = 0; num2 < strArray2.Length; num2++)
            {
                if (strArray2[num2].EndsWith("."))
                {
                    num = strArray2[num2].Length - 2;
                    break;
                }
            }
            num2 = 0;
            while (num2 < strArray2.Length)
            {
                str2 = strArray2[num2];
                num3 = str2.LastIndexOf('\t');
                if (num3 == -1)
                {
                    if (num < str2.Length)
                    {
                        num3 = num;
                    }
                    else
                    {
                        goto Label_0295;
                    }
                }
                str3 = str2.Substring(num3 + 1);
                if (((str3 != ".") && !(str3 == "..")) && ((str2[0] == 'd') || str2.ToLower().Contains("<dir>")))
                {
                    strArray3 = str3.Split(new char[] { ' ' });
                    length = strArray3.Length;
                    str3 = strArray3 [length - 1].PadRight (34, ' ');
                    this.DirList.Add(str3);
                    this.AddInfo("[目录]" + str3);
                }
            Label_0295:
                num2++;
            }
            for (num2 = 0; num2 < strArray2.Length; num2++)
            {
                str2 = strArray2[num2];
                num3 = str2.LastIndexOf('\t');
                if (num3 == -1)
                {
                    if (num < str2.Length)
                    {
                        num3 = num;
                    }
                    else
                    {
                        continue;
                    }
                }
                str3 = str2.Substring(num3 + 1);
                if (((str3 != ".") && !(str3 == "..")) && !((str2[0] == 'd') || str2.ToLower().Contains("<dir>")))
                {
                    strArray3 = str3.Split(new char[] { ' ' });
                    length = strArray3.Length;
                    str3 = strArray3 [length - 1].PadRight (34, ' ');
                    this.FileList.Add(str3);
                    this.AddInfo(str3);
                }
            }
            return true;
        }
		/// <summary>
		/// PUT the specified filepath.
		/// </summary>
		/// <param name="filepath">Filepath.</param>
        public bool Upload(string filepath)
        {
            if (!System.IO.File.Exists(filepath))
            {
                return false;
            }
            FileInfo info = new FileInfo(filepath);
            try
            {
                string uriString = this.GetUriString(info.Name);
                FtpWebRequest request = this.CreateFtpWebRequest(uriString, "STOR");
                request.ContentLength = info.Length;
                int count = 8196;
                byte[] buffer = new byte[count];
                FileStream stream = info.OpenRead();
                Stream requestStream = request.GetRequestStream();
                this.AddInfo("打开上传流，文件上传中...");
                for (int i = stream.Read(buffer, 0, count); i != 0; i = stream.Read(buffer, 0, count))
                {
                    requestStream.Write(buffer, 0, i);
                }
                requestStream.Close();
                stream.Close();
                FtpWebResponse ftpResponse = this.GetFtpResponse(request);
                if (ftpResponse == null)
                {
                    this.AddInfo("服务器未响应...");
                    return false;
                }
                this.AddInfo(string.Concat(new object[] { "上传完毕，服务器返回：", ftpResponse.StatusCode, " ", ftpResponse.StatusDescription }));
                this.ShowFtpFileAndDirectory();
                return true;
            }
            catch (WebException exception)
            {
                this.AddInfo("上传发生错误，返回信息为：" + exception.Status);
                return false;
            }
        }

        public delegate void TextEventHandler(object sender, MessageEventArgs e);
    }
}

