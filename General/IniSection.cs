namespace Qisi.General
{
    using System;
    using System.Collections.Generic;

    public class IniSection
    {
        private Dictionary<string, string> FDictionary;
        private string FSectionName;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.IniSection"/> class.
		/// </summary>
		/// <param name="SectionName">Section name.</param>
        public IniSection(string SectionName)
        {
            this.FSectionName = SectionName;
            this.FDictionary = new Dictionary<string, string>();
        }
		/// <summary>
		/// Adds the key value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
        public void AddKeyValue(string key, string value)
        {
            if (this.FDictionary.ContainsKey(key))
            {
                this.FDictionary[key] = value;
            }
            else
            {
                this.FDictionary.Add(key, value);
            }
        }

        public void Clear()
        {
            this.FDictionary.Clear();
        }

        public bool ReadValue(string key, bool defaultv)
        {
            return Convert.ToBoolean(this.ReadValue(key, Convert.ToString(defaultv)));
        }

        public DateTime ReadValue(string key, DateTime defaultv)
        {
            return Convert.ToDateTime(this.ReadValue(key, Convert.ToString(defaultv)));
        }

        public int ReadValue(string key, int defaultv)
        {
            return Convert.ToInt32(this.ReadValue(key, Convert.ToString(defaultv)));
        }

        public float ReadValue(string key, float defaultv)
        {
            return Convert.ToSingle(this.ReadValue(key, Convert.ToString(defaultv)));
        }

        public string ReadValue(string key, string defaultv)
        {
            if (this.FDictionary.ContainsKey(key))
            {
                return this.FDictionary[key];
            }
            return defaultv;
        }

        public string SaveToString()
        {
            string str = "";
            str = str + "[" + this.FSectionName + "]\r\n";
            foreach (KeyValuePair<string, string> pair in this.FDictionary)
            {
                string str3 = str;
                str = str3 + pair.Key + "=" + pair.Value + "\r\n";
            }
            return (str + "\r\n");
        }

        public void WriteValue(string key, bool value)
        {
            this.AddKeyValue(key, Convert.ToString(value));
        }

        public void WriteValue(string key, DateTime value)
        {
            this.AddKeyValue(key, Convert.ToString(value));
        }

        public void WriteValue(string key, int value)
        {
            this.AddKeyValue(key, Convert.ToString(value));
        }

        public void WriteValue(string key, float value)
        {
            this.AddKeyValue(key, Convert.ToString(value));
        }

        public void WriteValue(string key, string value)
        {
            this.AddKeyValue(key, value);
        }

        public int Count
        {
            get
            {
                return this.FDictionary.Count;
            }
        }

        public string SectionName
        {
            get
            {
                return this.FSectionName;
            }
        }
    }
}

