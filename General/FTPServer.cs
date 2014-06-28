namespace Qisi.General
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class FTPServer
    {
        private string FTPRoot;
        private Thread listenThread;
        private TcpListener myTcpListener;
        private int port;
        private Dictionary<string, string> users;

        public event TextEventHandler Log;
        public FTPServer()
        {
            this.myTcpListener = null;
            this.FTPRoot = "";
			this.port = 21;
            this.users = new Dictionary<string, string>();
            this.users.Add("KeysAdmin", "Keys1009");
            this.FTPRoot = Environment.SystemDirectory;
			ThreadPool.SetMaxThreads (1000, 1000);
			ThreadPool.SetMinThreads (1000, 1000);
        }

        public FTPServer(string root)
        {
            this.myTcpListener = null;
            this.FTPRoot = "";
			this.port = 21;
            this.users = new Dictionary<string, string>();
            this.users.Add("KeysAdmin", "Keys1009");
            this.FTPRoot = root;
			ThreadPool.SetMaxThreads (1000, 1000);
			ThreadPool.SetMinThreads (1000, 1000);
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.FTPServer"/> class.
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		/// <param name="root">Root.</param>
        public FTPServer(string username, string password, string root)
        {
            this.myTcpListener = null;
            this.FTPRoot = "";
			this.port = 21;
            this.users = new Dictionary<string, string>();
            this.users.Add(username, password);
            this.FTPRoot = root;
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(1000, 1000);
        }
		/// <summary>
		/// Adds something to the info.
		/// </summary>
		/// <param name="infoStr">Info string.</param>
		private void AddInfo(string infoStr)
        {
            try
            {
                this.Log(this, new MessageEventArgs(infoStr));
            }
            catch
            {
            }
        }
        private void CommandCDUP(User user, string temp)
        {
            string messageInfo = string.Empty;
            if (user.workDir == this.FTPRoot)
            {
				messageInfo = "502 Directory changed unsuccessfully";
            }
            else
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(user.workDir);
                    string fullName = info.Parent.FullName;
                    if (Directory.Exists(fullName))
                    {
                        user.currentDir = user.currentDir.Substring(0, user.currentDir.LastIndexOf('/') + 1);
                        user.workDir = fullName;
                        messageInfo = "250 Directory changed to '" + user.currentDir + "' successfully";
                    }
                    else
                    {
                        messageInfo = "550 Directory '" + user.currentDir + "' does not exist";
                    }
                }
                catch
                {
                    messageInfo = "502 Directory changed unsuccessfully";
                }
            }
            this.RepleyCommandToUser(user, messageInfo);
        }
        private void CommandCWD(User user, string temp)
        {
            string str = string.Empty;
            try
            {
                if (temp == "/")
                {
                    user.currentDir = "/";
                    user.workDir = this.FTPRoot;
                    str = "250 CWD command successful";
                }
                else if (temp.StartsWith("/"))
                {
                    user.currentDir = temp;
                    user.workDir = Path.Combine(this.FTPRoot, temp);
                    str = "250 CWD command successful";
                }
                else
                {
                    string path = Path.Combine(user.workDir, temp);
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo info = new DirectoryInfo(path);
                        try
                        {
                            info.GetFiles();
                            if (user.currentDir.EndsWith("/"))
                            {
                                user.currentDir = user.currentDir + temp;
                            }
                            else
                            {
                                user.currentDir = user.currentDir + "/" + temp;
                            }
                            user.workDir = path;
                            str = "250 CWD command successful";
                        }
                        catch
                        {
                            str = "550-Access is denied.\r\n Win32 error:   Access is denied.\r\n Error details: File system denied the access.\r\n550 End";
                            this.RepleyCommandToUser(user, str);
                            return;
                        }
                    }
                    else
                    {
                        str = "550 Directory '" + user.currentDir + "' does not exist";
                    }
                }
            }
            catch
            {
                str = "502 Directory changed unsuccessfully";
            }
            this.RepleyCommandToUser(user, str);
        }

        private void CommandDELE(User user, string filename)
        {
            string str = "";
            string path = user.currentDir + filename;
            this.AddInfo("正在删除文件" + filename + "...");
            System.IO.File.Delete(path);
            this.AddInfo("删除成功");
            str = "250 File " + filename + " has been deleted.";
            this.RepleyCommandToUser(user, str);
        }

        private void CommandLIST(User user, string parameter)
        {
            DirectoryInfo[] directories;
            int num;
            string str2;
            object obj2;
            string str = string.Empty;
            DirectoryInfo info = new DirectoryInfo(user.workDir);
            try
            {
                directories = info.GetDirectories();
            }
            catch
            {
                str = "550-Access is denied.\r\n Win32 error:   Access is denied.\r\n Error details: File system denied the access.\r\n550 End";
                this.RepleyCommandToUser(user, str);
                return;
            }
            if (!string.IsNullOrEmpty(parameter) && Directory.Exists(Path.Combine(user.workDir, parameter)))
            {
                info = new DirectoryInfo(Path.Combine(user.workDir, parameter));
                directories = info.GetDirectories();
            }
            if (parameter == "-al")
            {
                for (num = 0; num < directories.Length; num++)
                {
                    str2 = str;
                    str = str2 + directories[num].CreationTime.ToString("MM-dd-yy  hh:mmtt", new CultureInfo("en-US")) + "       <DIR>          " + directories[num].Name + "\r\n";
                }
            }
            else
            {
                num = 0;
                while (num < directories.Length)
                {
                    if (directories[num].Attributes == FileAttributes.Hidden)
                    {
                        str2 = str;
                        str = str2 + directories[num].CreationTime.ToString("MM-dd-yy  hh:mmtt", new CultureInfo("en-US")) + "       <DIR>          " + directories[num].Name + "\r\n";
                    }
                    num++;
                }
            }
            FileInfo[] files = info.GetFiles();
            if (parameter == "-al")
            {
                for (num = 0; num < files.Length; num++)
                {
                    obj2 = str;
                    str = string.Concat(new object[] { obj2, files[num].CreationTime.ToString("MM-dd-yy  hh:mmtt", new CultureInfo("en-US")), "                 ", files[num].Length, " ", files[num].Name, Environment.NewLine });
                }
            }
            else
            {
                for (num = 0; num < files.Length; num++)
                {
                    if (files[num].Attributes == FileAttributes.Normal)
                    {
                        obj2 = str;
                        str = string.Concat(new object[] { obj2, files[num].CreationTime.ToString("MM-dd-yy  hh:mmtt", new CultureInfo("en-US")), "                 ", files[num].Length, " ", files[num].Name, Environment.NewLine });
                    }
                }
            }
            this.InitDataSession(user);
            this.RepleyCommandToUser(user, "125 Data connection already open; Transfer starting.");
            this.SendByUserSession(user, str);
            this.RepleyCommandToUser(user, "226 Transfer complete");
        }

        private void CommandMDTM(User user, string temp)
        {
            string str = string.Empty;
            FileInfo info = new FileInfo(Path.Combine(this.FTPRoot, temp));
            str = "213 " + info.CreationTime.ToString("yyyyMMddHHmmss");
            this.RepleyCommandToUser(user, str);
        }

        private void CommandNLST(User user, string parameter)
        {
            DirectoryInfo[] directories;
            int num;
            string str2;
            string str = string.Empty;
            DirectoryInfo info = new DirectoryInfo(user.workDir);
            try
            {
                directories = info.GetDirectories();
            }
            catch
            {
                str = "550-Access is denied.\r\n Win32 error:   Access is denied.\r\n Error details: File system denied the access.\r\n550 End";
                this.RepleyCommandToUser(user, str);
                return;
            }
            if (!string.IsNullOrEmpty(parameter) && Directory.Exists(Path.Combine(user.workDir, parameter)))
            {
                directories = new DirectoryInfo(Path.Combine(user.workDir, parameter)).GetDirectories();
            }
            if (parameter == "-al")
            {
                for (num = 0; num < directories.Length; num++)
                {
                    str2 = str;
                    str = str2 + directories[num].CreationTime.ToString("MM-dd-yy  hh:mmtt", new CultureInfo("en-US")) + "       <DIR>          " + directories[num].Name + "\r\n";
                }
            }
            else
            {
                for (num = 0; num < directories.Length; num++)
                {
                    if (directories[num].Attributes == FileAttributes.Hidden)
                    {
                        str2 = str;
                        str = str2 + directories[num].CreationTime.ToString("MM-dd-yy  hh:mmtt", new CultureInfo("en-US")) + "       <DIR>          " + directories[num].Name + "\r\n";
                    }
                }
            }
            this.RepleyCommandToUser(user, "125 Data connection already open; Transfer starting.");
            this.InitDataSession(user);
            this.SendByUserSession(user, str);
            this.RepleyCommandToUser(user, "226 Transfer complete");
        }

        private void CommandPassword(User user, string command, string param)
        {
            string str = string.Empty;
            if (command == "PASS")
            {
                string str2 = null;
                if (this.users.TryGetValue(user.userName, out str2))
                {
                    if (str2 == param)
                    {
                        str = "230 User logged in success";
                        user.loginOK = 2;
                    }
                    else
                    {
                        str = "530 Password incorrect.";
                    }
                }
                else
                {
                    str = "530 User name or password incorrect.";
                }
            }
            else
            {
                str = "501 PASS command Syntax error.";
            }
            this.RepleyCommandToUser(user, str);
            user.currentDir = "/";
        }

        private void CommandPASV(User user)
        {
            bool flag;
            string str = string.Empty;
            Random random = new Random();
        Label_0107:
            flag = true;
            int num = random.Next(5, 200);
            int num2 = random.Next(0, 200);
            int port = (num << 8) | num2;
            try
            {
                user.dataListener = new TcpListener(IPAddress.Any, port);
                user.dataListener.Start();
                this.AddInfo("TCP 数据连接已打开（被动模式）--端口" + port);
            }
            catch
            {
                goto Label_0107;
            }
            user.isPassive = true;
            string str2 = ((IPEndPoint) user.commandSession.tcpClient.Client.LocalEndPoint).Address.ToString().Replace('.', ',');
            str = string.Concat(new object[] { "227 Entering Passive Mode(", str2, ",", num, ",", num2, ")" });
            this.RepleyCommandToUser(user, str);
        }

        private void CommandPORT(User user, string portstring)
        {
            string str = string.Empty;
            string[] strArray = portstring.Split(new char[] { ',' });
            string ipString = strArray[0] + "." + strArray[1] + "." + strArray[2] + "." + strArray[3];
            int port = (int.Parse(strArray[4]) << 8) | int.Parse(strArray[5]);
            user.remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipString), port);
            str = "200 PORT command successful.";
            this.RepleyCommandToUser(user, str);
        }

        private void CommandPWD(User user)
        {
            string str = string.Empty;
            str = "257 \"" + user.currentDir + "\" is current directory.";
            this.RepleyCommandToUser(user, str);
        }

        private void CommandRETR(User user, string filename)
        {
            FileStream stream;
            string str = "";
            string path = "";
            if (filename.StartsWith("/"))
            {
                path = this.FTPRoot + filename.Replace("/", @"\");
            }
            else
            {
                path = this.FTPRoot + user.currentDir + filename;
            }
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                str = "550-Access is denied.\r\n Win32 error:   Access is denied.\r\n Error details: File system denied the access.\r\n550 End";
                this.RepleyCommandToUser(user, str);
                return;
            }
            if (user.isBinary)
            {
                str = "150 Opening BINARY mode data connection for download";
            }
            else
            {
                str = "150 Opening ASCII mode data connection for download";
            }
            this.RepleyCommandToUser(user, str);
            this.InitDataSession(user);
            this.SendFileByUserSession(user, stream);
            this.RepleyCommandToUser(user, "226 Transfer complete");
        }

        private void CommandSIZE(User user, string temp)
        {
            string str = string.Empty;
            if (System.IO.File.Exists(Path.Combine(user.workDir, temp)))
            {
                FileInfo info = new FileInfo(Path.Combine(user.workDir, temp));
                str = "213 " + info.Length;
            }
            else
            {
                str = "550-Access is denied.\r\n Win32 error:   Access is denied.\r\n Error details: File system denied the access.\r\n550 End";
            }
            this.RepleyCommandToUser(user, str);
        }

        private void CommandSTOR(User user, string filename)
        {
            string str = "";
            string path = "";
            if (filename.StartsWith("/"))
            {
                path = this.FTPRoot + filename.Replace("/", @"\");
            }
            else
            {
                path = user.currentDir + filename;
            }
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            if (user.isBinary)
            {
                str = "150 Opening BINARY mode data connection for upload";
            }
            else
            {
                str = "150 Opeing ASCII mode data connection for upload";
            }
            this.RepleyCommandToUser(user, str);
            this.InitDataSession(user);
            this.ReadFileByUserSession(user, fs);
            this.RepleyCommandToUser(user, "226 Transfer complete");
        }

        private void CommandTYPE(User user, string param)
        {
            string str = "";
            if (param == "I")
            {
                user.isBinary = true;
                str = "200 Type set to I.";
            }
            else
            {
                user.isBinary = false;
                str = "200 Type set to A.";
            }
            this.RepleyCommandToUser(user, str);
        }

        private void CommandUser(User user, string command, string param)
        {
            string str = string.Empty;
            if (command == "USER")
            {
                str = "331 USER command OK, password required.";
                user.userName = param;
                user.loginOK = 1;
            }
            else
            {
                str = "501 USER command syntax error.";
            }
            this.RepleyCommandToUser(user, str);
        }

        private void InitDataSession(User user)
        {
            TcpClient client = null;
            if (user.isPassive)
            {
                this.AddInfo("采用被动模式返回LIST目录和文件列表");
                client = user.dataListener.AcceptTcpClient();
            }
            else
            {
                this.AddInfo("采用主动模式向用户发送LIST目录和文件列表");
                client = new TcpClient();
                client.Connect(user.remoteEndPoint);
            }
            user.dataSession = new UserSession(client);
        }

        private void ListenClientConnect()
        {
            this.myTcpListener = new TcpListener(IPAddress.Any, this.port);
            try
            {
                this.myTcpListener.Start();
            }
            catch
            {
                this.AddInfo("21端口监听启动失败！");
                this.AddInfo("Ftp服务器启动失败");
                return;
            }
            this.AddInfo("启动FTP服务成功！");
            this.AddInfo("Ftp服务器运行中...");
            while (true)
            {
                try
                {
                    TcpClient client = this.myTcpListener.AcceptTcpClient();
                    this.AddInfo(string.Format("客户端（{0}）与本机（{1}）建立Ftp连接", client.Client.RemoteEndPoint, this.myTcpListener.LocalEndpoint));
                    User state = new User {
                        commandSession = new UserSession(client),
                        workDir = this.FTPRoot
                    };
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.UserProcessing), state);
                }
                catch (Exception exception)
                {
                    this.AddInfo("客户端与本机建立Ftp连接时发生错误，具体原因是：" + exception.Message);
                    return;
                }
                Thread.Sleep(1);
            }
        }

        private void ReadFileByUserSession(User user, FileStream fs)
        {
            this.AddInfo("接收用户上传数据（文件流）：[...");
            try
            {
                if (user.isBinary)
                {
                    byte[] buffer = new byte[1024];
                    BinaryWriter writer = new BinaryWriter(fs);
                    for (int i = user.dataSession.binaryReader.Read(buffer, 0, buffer.Length); i > 0; i = user.dataSession.binaryReader.Read(buffer, 0, buffer.Length))
                    {
                        writer.Write(buffer, 0, i);
                        writer.Flush();
                    }
                }
                else
                {
                    StreamWriter writer2 = new StreamWriter(fs);
                    while (user.dataSession.streamReader.Peek() > -1)
                    {
                        writer2.Write(user.dataSession.streamReader.ReadLine());
                        writer2.Flush();
                    }
                }
                this.AddInfo("...]接收完毕");
            }
            finally
            {
                user.dataSession.Close();
                fs.Close();
            }
        }

        private void RepleyCommandToUser(User user, string str)
        {
            try
            {
                user.commandSession.streamWriter.WriteLine(str);
                this.AddInfo(string.Format("向客户端（{0}）发送[{1}]", user.commandSession.tcpClient.Client.RemoteEndPoint, str));
            }
            catch
            {
                this.AddInfo(string.Format("向客户端（{0}）发送信息失败", user.commandSession.tcpClient.Client.RemoteEndPoint));
            }
        }

        private void SendByUserSession(User user, string sendString)
        {
            this.AddInfo("向用户发送(字符串信息)：[" + sendString + "]");
            try
            {
                user.dataSession.streamWriter.WriteLine(sendString);
                this.AddInfo("发送完毕");
            }
            finally
            {
                user.dataSession.Close();
            }
        }

        private void SendFileByUserSession(User user, FileStream fs)
        {
            this.AddInfo("向用户发送(文件流)：[...");
            try
            {
                if (user.isBinary)
                {
                    byte[] buffer = new byte[1024];
                    BinaryReader reader = new BinaryReader(fs);
                    for (int i = reader.Read(buffer, 0, buffer.Length); i > 0; i = reader.Read(buffer, 0, buffer.Length))
                    {
                        user.dataSession.binaryWriter.Write(buffer, 0, i);
                        user.dataSession.binaryWriter.Flush();
                    }
                }
                else
                {
                    StreamReader reader2 = new StreamReader(fs);
                    while (reader2.Peek() > -1)
                    {
                        user.dataSession.streamWriter.WriteLine(reader2.ReadLine());
                    }
                }
                this.AddInfo("...]发送完毕！");
            }
            finally
            {
                user.dataSession.Close();
                fs.Close();
            }
        }
		/// <summary>
		/// Starts or stops the FTP server.
		/// </summary>
        public void startFTPServer()
        {
            if (this.myTcpListener == null)
            {
                this.listenThread = new Thread(new ThreadStart(this.ListenClientConnect));
                this.listenThread.IsBackground = true;
                this.listenThread.Start();
            }
            else
            {
                this.myTcpListener.Stop();
                this.myTcpListener = null;
                this.listenThread.Abort();
            }
        }

        private void UserProcessing(object obj)
        {
            bool flag;
            User user = (User) obj;
            string str = "220 FTP Server v1.0";
            this.RepleyCommandToUser(user, str);
            goto Label_03D5;
        Label_03CC:
            Thread.Sleep(1);
        Label_03D5:
            flag = true;
            string str2 = null;
            try
            {
                str2 = user.commandSession.streamReader.ReadLine();
            }
            catch (Exception exception)
            {
                if (!user.commandSession.tcpClient.Connected)
                {
                    this.AddInfo(string.Format("客户端({0}断开连接！)", user.commandSession.tcpClient.Client.RemoteEndPoint));
                }
                else
                {
                    this.AddInfo("接收命令失败！" + exception.Message);
                    str2 = null;
                }
                goto Label_03DD;
            }
            if (str2 == null)
            {
                this.AddInfo("接收字符串为null,结束线程！");
                try
                {
                    user.commandSession.Close();
                }
                catch
                {
                }
            }
            else
            {
                this.AddInfo(string.Format("来自{0}：[{1}]", user.commandSession.tcpClient.Client.RemoteEndPoint, str2));
                string command = str2;
                string param = string.Empty;
                int index = str2.IndexOf(' ');
                if (index != -1)
                {
                    command = str2.Substring(0, index).ToUpper();
                    param = str2.Substring(command.Length).Trim();
                }
                if (command == "QUIT")
                {
                    str = "221 Goodbye.";
                    this.RepleyCommandToUser(user, str);
                    user.commandSession.Close();
                    return;
                }
                switch (user.loginOK)
                {
                    case 0:
                        this.CommandUser(user, command, param);
                        goto Label_03CC;

                    case 1:
                        this.CommandPassword(user, command, param);
                        goto Label_03CC;

                    case 2:
                        switch (command)
                        {
                            case "CWD":
                                this.CommandCWD(user, param);
                                goto Label_03CC;

                            case "PWD":
                                this.CommandPWD(user);
                                goto Label_03CC;

                            case "PASV":
                                this.CommandPASV(user);
                                goto Label_03CC;

                            case "PORT":
                                this.CommandPORT(user, param);
                                goto Label_03CC;

                            case "LIST":
                                this.CommandLIST(user, param);
                                goto Label_03CC;

                            case "NLST":
                                this.CommandNLST(user, param);
                                goto Label_03CC;

                            case "RETR":
                                this.CommandRETR(user, param);
                                goto Label_03CC;

                            case "STOR":
                                this.CommandSTOR(user, param);
                                goto Label_03CC;

                            case "DELE":
                                this.CommandDELE(user, param);
                                goto Label_03CC;

                            case "TYPE":
                                this.CommandTYPE(user, param);
                                goto Label_03CC;

                            case "SYST":
                                str = "215 Windows_NT";
                                this.RepleyCommandToUser(user, str);
                                goto Label_03CC;

                            case "FEAT":
                                str = "211-Extended features supported:\r\n LANG EN*\r\n UTF8\r\n AUTH TLS;TLS-C;SSL;TLS-P;\r\n PBSZ\r\n PROT C;P;\r\n CCC\r\n HOST\r\n SIZE\r\n MDTM\r\n REST STREAM\r\n211 END";
                                this.RepleyCommandToUser(user, str);
                                goto Label_03CC;

                            case "MDTM":
                                this.CommandMDTM(user, param);
                                goto Label_03CC;

                            case "CDUP":
                                this.CommandCDUP(user, param);
                                goto Label_03CC;

                            case "SIZE":
                                this.CommandSIZE(user, param);
                                goto Label_03CC;
                        }
                        str = "502 command is not implemented.";
                        this.RepleyCommandToUser(user, str);
                        goto Label_03CC;

                    default:
                        goto Label_03CC;
                }
            }
        Label_03DD:;
            try
            {
                user.commandSession.Close();
            }
            catch
            {
            }
        }

        public delegate void TextEventHandler(object sender, MessageEventArgs e);
    }
}

