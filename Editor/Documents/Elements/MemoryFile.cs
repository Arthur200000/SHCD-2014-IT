namespace Qisi.Editor.Documents.Elements
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal class MemoryFile
    {
        internal MemoryFile(string filepath)
        {
            FileStream stream = File.OpenRead(filepath);
            int count = 0;
            count = (int) stream.Length;
            this.FileByte = new byte[count];
            stream.Read(this.FileByte, 0, count);
            stream.Close();
            this.FileName = Path.GetFileName(filepath);
        }

        internal MemoryFile(string filename, byte[] filebytes)
        {
            this.FileName = filename;
            this.FileByte = filebytes;
        }

        internal void ToFile(string dirPath)
        {
            if ((this.FileByte != null) && (this.FileName != null))
            {
                try
                {
                    File.WriteAllBytes(Path.Combine(dirPath, this.FileName), this.FileByte);
                }
                catch
                {
                }
            }
        }

        internal byte[] FileByte { get; set; }

        internal string FileName { get; set; }
    }
}

