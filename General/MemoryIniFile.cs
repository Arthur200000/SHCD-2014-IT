namespace Qisi.General
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class MemoryIniFile : IDisposable
    {
        private List<IniSection> List = new List<IniSection>();

        private static StreamReader Decrypt(string data)
        {
            byte[] buffer;
            byte[] bytes = Encoding.ASCII.GetBytes("KEYS1009");
            byte[] rgbIV = Encoding.ASCII.GetBytes("KEYS1009");
            try
            {
                buffer = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream(buffer);
            return new StreamReader(new CryptoStream(stream, provider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Read));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        private static string Encrypt(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes("KEYS1009");
            byte[] rgbIV = Encoding.ASCII.GetBytes("KEYS1009");
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream();
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(bytes, rgbIV), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Convert.ToBase64String(stream.ToArray());
        }

        ~MemoryIniFile()
        {
            this.Dispose(false);
        }

        public IniSection FindSection(string SectionName)
        {
            foreach (IniSection section in this.List)
            {
                if (section.SectionName.ToLower() == SectionName.ToLower())
                {
                    return section;
                }
            }
            return null;
        }

        public void LoadFromByte(byte[] buffer, Encoding encoding)
        {
            MemoryStream stream = new MemoryStream(buffer);
            StreamReader sR = new StreamReader(stream, encoding);
            this.LoadFromStream(sR);
            stream.Close();
            stream.Dispose();
        }

        public void LoadFromEncodedFile(string FileName)
        {
            string data = File.ReadAllText(FileName);
            this.LoadFromStream(Decrypt(data));
        }

        public void LoadFromFile(string FileName)
        {
            FileStream stream = new FileStream(Path.GetFullPath(FileName), FileMode.Open);
            StreamReader sR = new StreamReader(stream, Encoding.Default);
            this.LoadFromStream(sR);
            stream.Close();
            stream.Dispose();
        }

        public void LoadFromFile(string FileName, Encoding encoding)
        {
            FileStream stream = new FileStream(Path.GetFullPath(FileName), FileMode.Open);
            StreamReader sR = new StreamReader(stream, encoding);
            this.LoadFromStream(sR);
            stream.Close();
            stream.Dispose();
        }

        public void LoadFromStream(StreamReader SR)
        {
            this.List.Clear();
            string sectionName = null;
            IniSection item = null;
            while (true)
            {
                sectionName = SR.ReadLine();
                if (sectionName == null)
                {
                    SR.Dispose();
                    return;
                }
                sectionName = sectionName.Trim();
                if (sectionName != "")
                {
                    if (((sectionName != "") && (sectionName[0] == '[')) && (sectionName[sectionName.Length - 1] == ']'))
                    {
                        sectionName = sectionName.Remove(0, 1);
                        sectionName = sectionName.Remove(sectionName.Length - 1, 1);
                        item = this.FindSection(sectionName);
                        if (item == null)
                        {
                            item = new IniSection(sectionName);
                            this.List.Add(item);
                        }
                    }
                    else
                    {
                        if (item == null)
                        {
                            item = this.FindSection("UnDefSection");
                            if (item == null)
                            {
                                item = new IniSection("UnDefSection");
                                this.List.Add(item);
                            }
                        }
                        int index = sectionName.IndexOf('=');
                        if (index != 0)
                        {
                            string key = sectionName.Substring(0, index);
                            string str3 = sectionName.Substring(index + 1, (sectionName.Length - index) - 1);
                            item.AddKeyValue(key, str3);
                        }
                        else
                        {
                            item.AddKeyValue(sectionName, "");
                        }
                    }
                }
            }
        }

        public void LoadFromString(string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            this.LoadFromByte(bytes, encoding);
        }

        public bool ReadValue(string SectionName, string key, bool defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }

        public DateTime ReadValue(string SectionName, string key, DateTime defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }

        public int ReadValue(string SectionName, string key, int defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }

        public float ReadValue(string SectionName, string key, float defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }

        public string ReadValue(string SectionName, string key, string defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }

        public void SaveToEncryptedFile(string FileName)
        {
            string contents = Encrypt(this.SaveToString());
            File.WriteAllText(FileName, contents, Encoding.Default);
        }

        public void SaveToFile(string FileName)
        {
            string path = "";
            foreach (IniSection section in this.List)
            {
                path = path + section.SaveToString();
            }
            File.WriteAllText(path, FileName, Encoding.Default);
        }

        public string SaveToString()
        {
            string str = "";
            foreach (IniSection section in this.List)
            {
                str = str + section.SaveToString();
            }
            return str;
        }

        private bool SectionExists(string SectionName)
        {
            foreach (IniSection section in this.List)
            {
                if (section.SectionName.ToLower() == SectionName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public IniSection WriteValue(string SectionName, string key, bool value)
        {
            IniSection item = this.FindSection(SectionName);
            if (item == null)
            {
                item = new IniSection(SectionName);
                this.List.Add(item);
            }
            item.WriteValue(key, value);
            return item;
        }

        public IniSection WriteValue(string SectionName, string key, DateTime value)
        {
            IniSection item = this.FindSection(SectionName);
            if (item == null)
            {
                item = new IniSection(SectionName);
                this.List.Add(item);
            }
            item.WriteValue(key, value);
            return item;
        }

        public IniSection WriteValue(string SectionName, string key, int value)
        {
            IniSection item = this.FindSection(SectionName);
            if (item == null)
            {
                item = new IniSection(SectionName);
                this.List.Add(item);
            }
            item.WriteValue(key, value);
            return item;
        }

        public IniSection WriteValue(string SectionName, string key, float value)
        {
            IniSection item = this.FindSection(SectionName);
            if (item == null)
            {
                item = new IniSection(SectionName);
                this.List.Add(item);
            }
            item.WriteValue(key, value);
            return item;
        }

        public IniSection WriteValue(string SectionName, string key, string value)
        {
            IniSection item = this.FindSection(SectionName);
            if (item == null)
            {
                item = new IniSection(SectionName);
                this.List.Add(item);
            }
            item.WriteValue(key, value);
            return item;
        }
    }
}

