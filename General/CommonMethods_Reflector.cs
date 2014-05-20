namespace Qisi.General
{
    using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Cryptography;
    using System.Text;

    public class CommonMethods
    {
        public static bool ClearDirectory(string Path)
        {
            bool flag = true;
            DirectoryInfo info = new DirectoryInfo(Path);
            if (info.Exists)
            {
                bool flag2;
                DirectorySecurity accessControl = Directory.GetAccessControl(Path, AccessControlSections.All);
                InheritanceFlags none = InheritanceFlags.None;
                none = InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit;
                FileSystemAccessRule rule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, none, PropagationFlags.None, AccessControlType.Allow);
                accessControl.ModifyAccessRule(AccessControlModification.Add, rule, out flag2);
                info.SetAccessControl(accessControl);
                try
                {
                    foreach (FileInfo info2 in info.GetFiles())
                    {
                        if ((info2.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            info2.Attributes = FileAttributes.Normal;
                        }
                        try
                        {
                            info2.Delete();
                        }
                        catch
                        {
                            flag = false;
                        }
                    }
                }
                catch
                {
                    flag = false;
                }
                foreach (DirectoryInfo info3 in info.GetDirectories())
                {
                    if (!ClearDirectory(info3.FullName))
                    {
                        flag = false;
                    }
                    try
                    {
                        info3.Delete(true);
                    }
                    catch
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }

        public static void CopyFolder(string sourceFolder, string destFolder, bool overwrite = true)
        {
            if (Directory.Exists(sourceFolder))
            {
                string name;
                string str2;
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                FileInfo[] files = new DirectoryInfo(sourceFolder).GetFiles();
                foreach (FileInfo info2 in files)
                {
                    name = info2.Name;
                    str2 = Path.Combine(destFolder, name);
                    if ((info2.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        info2.Attributes = FileAttributes.Normal;
                    }
                    try
                    {
                        info2.CopyTo(str2, overwrite);
                    }
                    catch (Exception exception)
                    {
                        LastErrorMessage = exception.Message;
                    }
                }
                string[] directories = Directory.GetDirectories(sourceFolder);
                foreach (string str3 in directories)
                {
                    name = Path.GetFileName(str3);
                    str2 = Path.Combine(destFolder, name);
                    CopyFolder(str3, str2, overwrite);
                }
            }
        }

        public static void CopyFolder(string sourceFolder, string filter, string destFolder)
        {
            string[] strArray = filter.Split(new char[] { '|' });
            if (strArray.Length == 1)
            {
                string fileName;
                string str3;
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                string[] files = Directory.GetFiles(sourceFolder, filter);
                foreach (string str in files)
                {
                    fileName = Path.GetFileName(str);
                    str3 = Path.Combine(destFolder, fileName);
                    try
                    {
                        File.Copy(str, str3, true);
                    }
                    catch (Exception exception)
                    {
                        LastErrorMessage = exception.Message;
                    }
                }
                string[] directories = Directory.GetDirectories(sourceFolder);
                foreach (string str4 in directories)
                {
                    fileName = Path.GetFileName(str4);
                    str3 = Path.Combine(destFolder, fileName);
                    CopyFolder(str4, filter, str3);
                }
            }
            else
            {
                foreach (string str5 in strArray)
                {
                    CopyFolder(sourceFolder, str5, destFolder);
                }
            }
        }

        public static bool Decy(string filePath)
        {
            int num;
            FileStream stream = new FileInfo(filePath).OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            byte[] buffer2 = new byte[stream.Length - 64L];
            stream.Close();
            for (num = 0; num < (buffer.Length - 64); num++)
            {
                buffer2[num] = buffer[(buffer.Length - 33) - num];
            }
            byte[] buffer3 = new MD5CryptoServiceProvider().ComputeHash(buffer2);
            StringBuilder builder = new StringBuilder();
            for (num = 0; num < buffer3.Length; num++)
            {
                builder.Append(buffer3[num].ToString("x2"));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
            for (num = 0; num < 32; num++)
            {
                if (bytes[num] != buffer[num])
                {
                    return false;
                }
            }
            File.WriteAllBytes(filePath, buffer2);
            return true;
        }

        public static bool Decy(string filePath, out byte[] filebuffer)
        {
            int num;
            FileStream stream = new FileInfo(filePath).OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            filebuffer = new byte[stream.Length - 64L];
            stream.Close();
            for (num = 0; num < (buffer.Length - 0x40); num++)
            {
                filebuffer[num] = buffer[(buffer.Length - 0x21) - num];
            }
            byte[] buffer2 = new MD5CryptoServiceProvider().ComputeHash(filebuffer);
            StringBuilder builder = new StringBuilder();
            for (num = 0; num < buffer2.Length; num++)
            {
                builder.Append(buffer2[num].ToString("x2"));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
            for (num = 0; num < 0x20; num++)
            {
                if (bytes[num] != buffer[num])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Decy(byte[] buffer2, out byte[] filebuffer)
        {
            int num;
            filebuffer = new byte[buffer2.Length - 0x40];
            for (num = 0; num < (buffer2.Length - 0x40); num++)
            {
                filebuffer[num] = buffer2[(buffer2.Length - 0x21) - num];
            }
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(filebuffer);
            StringBuilder builder = new StringBuilder();
            for (num = 0; num < buffer.Length; num++)
            {
                builder.Append(buffer[num].ToString("x2"));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
            for (num = 0; num < 0x20; num++)
            {
                if (bytes[num] != buffer2[num])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Ency(string filePath)
        {
            byte[] buffer;
            FileStream stream = new FileInfo(filePath).OpenRead();
            if (Ency(stream, out buffer))
            {
                File.WriteAllBytes(filePath, buffer);
                stream.Close();
                stream.Dispose();
                return true;
            }
            stream.Close();
            stream.Dispose();
            return false;
        }

        public static bool Ency(FileStream stream, out byte[] file)
        {
            file = new byte[stream.Length];
            try
            {
                int num;
                MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                stream.Close();
                stream.Dispose();
                byte[] buffer2 = provider.ComputeHash(buffer);
                StringBuilder builder = new StringBuilder();
                for (num = 0; num < buffer2.Length; num++)
                {
                    builder.Append(buffer2[num].ToString("x2"));
                }
                byte[] bytes = Encoding.Default.GetBytes(builder.ToString());
                file = new byte[buffer.Length + (2 * bytes.Length)];
                for (num = 0; num < bytes.Length; num++)
                {
                    file[num] = bytes[num];
                }
                for (num = 0; num < buffer.Length; num++)
                {
                    file[num + bytes.Length] = buffer[(buffer.Length - 1) - num];
                }
                for (num = 0; num < bytes.Length; num++)
                {
                    file[(num + bytes.Length) + buffer.Length] = bytes[num];
                }
                return true;
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
                return false;
            }
        }

        public static bool Ency(FileStream stream, string filePath)
        {
            byte[] buffer;
            if (Ency(stream, out buffer))
            {
                File.WriteAllBytes(filePath, buffer);
                return true;
            }
            return false;
        }

        public static DataSet Readxls(string Path, bool HDR = true)
        {
            string str;
            DataSet dataSet = new DataSet();
            if (HDR)
            {
                str = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";";
            }
            else
            {
                str = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=No;IMEX=1\";";
            }
            OleDbConnection connection = new OleDbConnection(str);
            try
            {
                connection.Open();
                object[] restrictions = new object[4];
                restrictions[3] = "TABLE";
                DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
                foreach (DataRow row in oleDbSchemaTable.Rows)
                {
                    string srcTable = row["TABLE_NAME"].ToString().Trim();
                    if (srcTable.EndsWith("$"))
                    {
                        new OleDbDataAdapter("SELECT * FROM [" + srcTable + "]", str).Fill(dataSet, srcTable);
                    }
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
            }
            return dataSet;
        }

        public static DataSet Readxls(string Path, string[] TableNames, bool HDR = true)
        {
            string str;
            DataSet dataSet = new DataSet();
            if (HDR)
            {
                str = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";";
            }
            else
            {
                str = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=No;IMEX=1\";";
            }
            OleDbConnection connection = new OleDbConnection(str);
            try
            {
                connection.Open();
                foreach (string str2 in TableNames)
                {
                    new OleDbDataAdapter("SELECT * FROM [" + str2 + "$]", str).Fill(dataSet, str2);
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return dataSet;
        }

        public static DataTable Readxls(string Path, string TableName, bool HDR)
        {
            string str;
            DataTable dataTable = new DataTable();
            if (HDR)
            {
                str = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";";
            }
            else
            {
                str = "Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path + "; Extended Properties = \"Excel 8.0;HDR=No;IMEX=1\";";
            }
            OleDbConnection connection = new OleDbConnection(str);
            try
            {
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + TableName + "$]", str);
                adapter.Fill(dataTable);
                adapter.Dispose();
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return dataTable;
        }

        public static DataSet Readxlsx(string Path, bool HDR = true)
        {
            string str;
            DataSet dataSet = new DataSet();
            if (HDR)
            {
                str = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=Yes;IMEX=1\";";
            }
            else
            {
                str = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=No;IMEX=1\";";
            }
            OleDbConnection connection = new OleDbConnection(str);
            try
            {
                connection.Open();
                object[] restrictions = new object[4];
                restrictions[3] = "TABLE";
                DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);
                foreach (DataRow row in oleDbSchemaTable.Rows)
                {
                    string srcTable = row["TABLE_NAME"].ToString().Trim();
                    if (srcTable.EndsWith("$"))
                    {
                        new OleDbDataAdapter("SELECT * FROM [" + srcTable + "]", str).Fill(dataSet, srcTable);
                    }
                }
                return dataSet;
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return dataSet;
        }

        public static DataSet Readxlsx(string Path, string[] TableNames, bool HDR = true)
        {
            string str;
            DataSet dataSet = new DataSet();
            if (HDR)
            {
                str = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=Yes;IMEX=1\";";
            }
            else
            {
                str = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=No;IMEX=1\";";
            }
            OleDbConnection connection = new OleDbConnection(str);
            try
            {
                connection.Open();
                foreach (string str2 in TableNames)
                {
                    new OleDbDataAdapter("SELECT * FROM [" + str2 + "$]", str).Fill(dataSet, str2);
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return dataSet;
        }

        public static DataTable Readxlsx(string Path, string TableName, bool HDR = true)
        {
            string str;
            DataTable dataTable = new DataTable();
            if (HDR)
            {
                str = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=Yes;IMEX=1\";";
            }
            else
            {
                str = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + Path + "; Extended Properties = \"Excel 12.0;HDR=No;IMEX=1\";";
            }
            OleDbConnection connection = new OleDbConnection(str);
            try
            {
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [" + TableName + "$]", str);
                adapter.Fill(dataTable);
                adapter.Dispose();
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return dataTable;
        }

        public static bool Unzip(byte[] buffer2, string password)
        {
            bool flag = true;
            Stream baseInputStream = new MemoryStream(buffer2);
            ZipInputStream stream2 = new ZipInputStream(baseInputStream);
            ZipEntry entry = null;
            stream2.Password = password;
            try
            {
                byte[] buffer = null;
                while ((entry = stream2.GetNextEntry()) != null)
                {
                    bool flag3;
                    if (!(Path.GetFileName(entry.Name) != string.Empty))
                    {
                        continue;
                    }
                    int count = 2048;
                    byte[] buffer3 = new byte[2048];
                    Stream stream3 = new MemoryStream();
                    goto Label_00C9;
                Label_0067:
                    count = stream2.Read(buffer3, 0, buffer3.Length);
                    if (count > 0)
                    {
                        stream3.Write(buffer3, 0, count);
                    }
                    else
                    {
                        buffer = new byte[stream3.Length];
                        stream3.Seek(0L, SeekOrigin.Begin);
                        stream3.Read(buffer, 0, buffer.Length);
                        stream3.Close();
                        continue;
                    }
                Label_00C9:
                    flag3 = true;
                    goto Label_0067;
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
                flag = false;
            }
            finally
            {
                if (baseInputStream != null)
                {
                    baseInputStream.Close();
                    baseInputStream = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                if (stream2 != null)
                {
                    stream2.Close();
                    stream2 = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return (!flag || flag);
        }

        public static bool Unzip(string filePath, string path, string password)
        {
            bool flag = true;
            FileStream stream = new FileInfo(filePath).OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            Stream baseInputStream = new MemoryStream(buffer);
            ZipInputStream stream3 = new ZipInputStream(baseInputStream);
            ZipEntry entry = null;
            stream3.Password = password;
            FileStream stream4 = null;
            try
            {
                while ((entry = stream3.GetNextEntry()) != null)
                {
                    bool flag3;
                    string str = Path.Combine(path, entry.Name);
                    if (entry.Name.Contains(@"\"))
                    {
                        int length = entry.Name.LastIndexOf(@"\");
                        Directory.CreateDirectory(Path.Combine(path, entry.Name.Substring(0, length)));
                    }
                    if (!(str != string.Empty))
                    {
                        continue;
                    }
                    int count = 2048;
                    byte[] buffer2 = new byte[2048];
                    stream4 = File.Create(str);
                    goto Label_0113;
                Label_00E2:
                    count = stream3.Read(buffer2, 0, buffer2.Length);
                    if (count <= 0)
                    {
                        goto Label_0118;
                    }
                    stream4.Write(buffer2, 0, count);
                Label_0113:
                    flag3 = true;
                    goto Label_00E2;
                Label_0118:
                    stream4.Flush();
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
                flag = false;
            }
            finally
            {
                if (stream4 != null)
                {
                    stream4.Close();
                    stream4 = null;
                }
                if (baseInputStream != null)
                {
                    baseInputStream.Close();
                    baseInputStream = null;
                }
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                if (stream3 != null)
                {
                    stream3.Close();
                    stream3 = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return (!flag || flag);
        }

        public static bool Unzip(byte[] buffer2, string path, string password)
        {
            bool flag = true;
            Stream baseInputStream = new MemoryStream(buffer2);
            ZipInputStream stream2 = new ZipInputStream(baseInputStream);
            ZipEntry entry = null;
            stream2.Password = password;
            FileStream stream3 = null;
            try
            {
                while ((entry = stream2.GetNextEntry()) != null)
                {
                    bool flag3;
                    string str = Path.Combine(path, entry.Name);
                    if (entry.Name.Contains(@"\"))
                    {
                        int length = entry.Name.LastIndexOf(@"\");
                        Directory.CreateDirectory(Path.Combine(path, entry.Name.Substring(0, length)));
                    }
                    if (!(str != string.Empty))
                    {
                        continue;
                    }
                    int count = 2048;
                    byte[] buffer = new byte[2048];
                    stream3 = File.Create(str);
                    goto Label_00DE;
                Label_00AE:
                    count = stream2.Read(buffer, 0, buffer.Length);
                    if (count <= 0)
                    {
                        goto Label_00E3;
                    }
                    stream3.Write(buffer, 0, count);
                Label_00DE:
                    flag3 = true;
                    goto Label_00AE;
                Label_00E3:
                    stream3.Flush();
                    stream3.Close();
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
                flag = false;
            }
            finally
            {
                if (baseInputStream != null)
                {
                    baseInputStream.Close();
                    baseInputStream = null;
                }
                if (stream3 != null)
                {
                    stream3.Close();
                    stream3 = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                if (stream2 != null)
                {
                    stream2.Close();
                    stream2 = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return (!flag || flag);
        }

        public static bool Unzip(byte[] buffer2, out List<byte[]> StreamList, out List<string> FilenameList, string password)
        {
            bool flag = true;
            StreamList = new List<byte[]>();
            FilenameList = new List<string>();
            Stream baseInputStream = new MemoryStream(buffer2);
            ZipInputStream stream2 = new ZipInputStream(baseInputStream);
            ZipEntry entry = null;
            stream2.Password = password;
            byte[] buffer = null;
            try
            {
                while ((entry = stream2.GetNextEntry()) != null)
                {
                    bool flag3;
                    string fileName = Path.GetFileName(entry.Name);
                    if (!(fileName != string.Empty))
                    {
                        goto Label_00DD;
                    }
                    int count = 2048;
                    byte[] buffer3 = new byte[2048];
                    Stream stream3 = new MemoryStream();
                    goto Label_00D7;
                Label_0075:
                    count = stream2.Read(buffer3, 0, buffer3.Length);
                    if (count > 0)
                    {
                        stream3.Write(buffer3, 0, count);
                    }
                    else
                    {
                        buffer = new byte[stream3.Length];
                        stream3.Seek(0L, SeekOrigin.Begin);
                        stream3.Read(buffer, 0, buffer.Length);
                        stream3.Close();
                        goto Label_00DD;
                    }
                Label_00D7:
                    flag3 = true;
                    goto Label_0075;
                Label_00DD:
                    StreamList.Add(buffer);
                    FilenameList.Add(Path.GetFileName(fileName));
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
                flag = false;
            }
            finally
            {
                if (entry != null)
                {
                    entry = null;
                }
                if (stream2 != null)
                {
                    stream2.Close();
                    stream2 = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return (!flag || flag);
        }

        public static bool Unzip(byte[] buffer2, out List<byte[]> StreamList, out List<string> FilenameList, string ext, string password)
        {
            bool flag = true;
            StreamList = new List<byte[]>();
            FilenameList = new List<string>();
            Stream baseInputStream = new MemoryStream(buffer2);
            ZipInputStream stream2 = new ZipInputStream(baseInputStream);
            ZipEntry entry = null;
            stream2.Password = password;
            byte[] buffer = null;
            try
            {
                while ((entry = stream2.GetNextEntry()) != null)
                {
                    bool flag3;
                    string fileName = Path.GetFileName(entry.Name);
                    if (!(fileName != string.Empty))
                    {
                        goto Label_00DE;
                    }
                    int count = 2048;
                    byte[] buffer3 = new byte[2048];
                    Stream stream3 = new MemoryStream();
                    goto Label_00D8;
                Label_0076:
                    count = stream2.Read(buffer3, 0, buffer3.Length);
                    if (count > 0)
                    {
                        stream3.Write(buffer3, 0, count);
                    }
                    else
                    {
                        buffer = new byte[stream3.Length];
                        stream3.Seek(0L, SeekOrigin.Begin);
                        stream3.Read(buffer, 0, buffer.Length);
                        stream3.Close();
                        goto Label_00DE;
                    }
                Label_00D8:
                    flag3 = true;
                    goto Label_0076;
                Label_00DE:
                    if (Path.GetExtension(fileName) == ext)
                    {
                        StreamList.Add(buffer);
                        FilenameList.Add(Path.GetFileNameWithoutExtension(fileName));
                    }
                }
            }
            catch (Exception exception)
            {
                LastErrorMessage = exception.Message;
                flag = false;
            }
            finally
            {
                if (entry != null)
                {
                    entry = null;
                }
                if (stream2 != null)
                {
                    stream2.Close();
                    stream2 = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return (!flag || flag);
        }

        public static bool Zip(string FileToZip, string ZipedFile, string Password)
        {
            if (Directory.Exists(FileToZip))
            {
                return ZipFileDictory(FileToZip, ZipedFile, Password);
            }
            return (File.Exists(FileToZip) && ZipFile(FileToZip, ZipedFile, Password));
        }

        private static bool ZipFile(string FileToZip, string ZipedFile, string Password)
        {
            if (!File.Exists(FileToZip))
            {
                LastErrorMessage = "指定要压缩的文件: " + FileToZip + " 不存在!";
                return false;
            }
            FileStream baseOutputStream = null;
            ZipOutputStream stream2 = null;
            ZipEntry entry = null;
            bool flag = true;
            try
            {
                try
                {
                    baseOutputStream = File.OpenRead(FileToZip);
                }
                catch (Exception exception)
                {
                    LastErrorMessage = exception.Message;
                    return false;
                }
                byte[] buffer = new byte[baseOutputStream.Length];
                baseOutputStream.Read(buffer, 0, buffer.Length);
                baseOutputStream.Close();
                baseOutputStream.Dispose();
                baseOutputStream = File.Create(ZipedFile);
                stream2 = new ZipOutputStream(baseOutputStream) {
                    Password = Password
                };
                entry = new ZipEntry(Path.GetFileName(FileToZip));
                stream2.PutNextEntry(entry);
                stream2.SetLevel(6);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.Flush();
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (entry != null)
                {
                    entry = null;
                }
                if (stream2 != null)
                {
                    stream2.Finish();
                    stream2.Close();
                    stream2.Dispose();
                }
                if (baseOutputStream != null)
                {
                    baseOutputStream.Close();
                    baseOutputStream.Dispose();
                    baseOutputStream = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return flag;
        }

        private static bool ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName)
        {
            bool flag = true;
            ZipEntry entry = null;
            FileStream stream = null;
            try
            {
                string[] files = Directory.GetFiles(FolderToZip);
                foreach (string str in files)
                {
                    stream = File.OpenRead(str);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(str))) {
                        DateTime = DateTime.Now,
                        Size = stream.Length
                    };
                    stream.Close();
                    stream.Dispose();
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                    s.Flush();
                }
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            string[] directories = Directory.GetDirectories(FolderToZip);
            foreach (string str2 in directories)
            {
                if (!ZipFileDictory(str2, s, Path.Combine(ParentFolderName, Path.GetFileName(str2))))
                {
                    return false;
                }
            }
            return flag;
        }

        private static bool ZipFileDictory(string FolderToZip, string ZipedFile, string Password)
        {
            if (!Directory.Exists(FolderToZip))
            {
                return false;
            }
            ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFile));
            s.SetLevel(6);
            s.Password = Password;
            bool flag = ZipFileDictory(FolderToZip, s, "");
            s.Finish();
            s.Close();
            s.Dispose();
            return flag;
        }

        public static string LastErrorMessage
        {
            [CompilerGenerated]
            get
            {
                return <LastErrorMessage>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <LastErrorMessage>k__BackingField = value;
            }
        }
    }
}

