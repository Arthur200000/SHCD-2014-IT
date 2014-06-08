namespace Qisi.General
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
	/// <summary>
	/// Memory ini file.
	/// </summary>
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
		/// <summary>
		/// Releases all resource used by the <see cref="Qisi.General.MemoryIniFile"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Qisi.General.MemoryIniFile"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Qisi.General.MemoryIniFile"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="Qisi.General.MemoryIniFile"/> so
		/// the garbage collector can reclaim the memory that the <see cref="Qisi.General.MemoryIniFile"/> was occupying.</remarks>
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
		/// <summary>
		/// Encrypt the specified data, using DES Encryptor "KEYS1009".
		/// </summary>
		/// <param name="data">Data.</param>
        private static string Encrypt(string data)
        {
			byte[] keys1009 = Encoding.ASCII.GetBytes("KEYS1009");
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream();
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(keys1009, keys1009), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Convert.ToBase64String(stream.ToArray());
        }
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Qisi.General.MemoryIniFile"/> is reclaimed by garbage collection.
		/// </summary>
        ~MemoryIniFile()
        {
            this.Dispose(false);
        }
		/// <summary>
		/// Finds the section.
		/// </summary>
		/// <returns>The section.</returns>
		/// <param name="SectionName">Section name.</param>
		IniSection FindSection(string SectionName)
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
		/// <summary>
		/// Loads from buffer, using Encoding.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <param name="encoding">Encoding.</param>
        public void LoadFromByte(byte[] buffer, Encoding encoding)
        {
            MemoryStream stream = new MemoryStream(buffer);
            StreamReader sR = new StreamReader(stream, encoding);
            this.LoadFromStream(sR);
            stream.Close();
            stream.Dispose();
        }
		/// <summary>
		/// Loads from encoded file.
		/// </summary>
		/// <param name="FileName">File name.</param>
        public void LoadFromEncodedFile(string FileName)
        {
            string data = File.ReadAllText(FileName);
            this.LoadFromStream(Decrypt(data));
        }
		/// <summary>
		/// Loads from file.
		/// </summary>
		/// <param name="FileName">File name.</param>
        public void LoadFromFile(string FileName)
        {
            FileStream stream = new FileStream(Path.GetFullPath(FileName), FileMode.Open);
            StreamReader sR = new StreamReader(stream, Encoding.Default);
            this.LoadFromStream(sR);
            stream.Close();
            stream.Dispose();
        }
		/// <summary>
		/// Loads from file.
		/// </summary>
		/// <param name="FileName">File name.</param>
		/// <param name="encoding">Encoding.</param>
        public void LoadFromFile(string FileName, Encoding encoding)
        {
            FileStream stream = new FileStream(Path.GetFullPath(FileName), FileMode.Open);
            StreamReader sR = new StreamReader(stream, encoding);
            this.LoadFromStream(sR);
            stream.Close();
            stream.Dispose();
        }
		/// <summary>
		/// Loads from stream.
		/// </summary>
		/// <param name="SR">StreamReader.</param>
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
		/// <summary>
		/// Loads from string.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="encoding">Encoding.</param>
        public void LoadFromString(string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            this.LoadFromByte(bytes, encoding);
        }
		/// <summary>
		/// Reads the value.
		/// </summary>
		/// <returns><c>true</c>, if value was  read, <c>defaultv</c> otherwise.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="defaultv">If set to <c>true</c> defaultv.</param>
        public bool ReadValue(string SectionName, string key, bool defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }
		/// <summary>
		/// Reads the value into DateTime.
		/// </summary>
		/// <returns>The DateTime value.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="defaultv">Defaultv.</param>
        public DateTime ReadValue(string SectionName, string key, DateTime defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }
		/// <summary>
		/// Reads the value into int.
		/// </summary>
		/// <returns>The int value.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="defaultv">Defaultv.</param>
        public int ReadValue(string SectionName, string key, int defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }
		/// <summary>
		/// Reads the value into float.
		/// </summary>
		/// <returns>The float value.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="defaultv">Defaultv.</param>
        public float ReadValue(string SectionName, string key, float defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }
		/// <summary>
		/// Reads the value as string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="defaultv">Defaultv.</param>
        public string ReadValue(string SectionName, string key, string defaultv)
        {
            IniSection section = this.FindSection(SectionName);
            if (section != null)
            {
                return section.ReadValue(key, defaultv);
            }
            return defaultv;
        }
		/// <summary>
		/// Saves to encrypted file.
		/// </summary>
		/// <param name="FileName">File name.</param>
        public void SaveToEncryptedFile(string FileName)
        {
            string contents = Encrypt(this.SaveToString());
            File.WriteAllText(FileName, contents, Encoding.Default);
        }
		/// <summary>
		/// Saves to file.
		/// </summary>
		/// <param name="FileName">File name.</param>
        public void SaveToFile(string FileName)
        {
            string path = "";
            foreach (IniSection section in this.List)
            {
                path = path + section.SaveToString();
            }
            File.WriteAllText(path, FileName, Encoding.Default);
        }
		/// <summary>
		/// Saves to string.
		/// </summary>
		/// <returns>The to string.</returns>
        public string SaveToString()
        {
            string str = "";
            foreach (IniSection section in this.List)
            {
                str = str + section.SaveToString();
            }
            return str;
        }
		/// <summary>
		/// Returns if <c>SectionName</c> exists.
		/// </summary>
		/// <returns><c>true</c>, if <c>SectionName</c> was sectioned, <c>false</c> otherwise.</returns>
		/// <param name="SectionName">Section name.</param>
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
		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">If set to <c>true</c> value.</param>
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
		/// <summary>
		/// Writes the DateTi,e value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="SectionName">Section name.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
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

