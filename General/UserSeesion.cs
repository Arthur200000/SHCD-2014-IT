namespace Qisi.General
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;

    internal class UserSeesion
    {
        public readonly BinaryReader binaryReader;
        public readonly BinaryWriter binaryWriter;
        private NetworkStream networkStream;
        public readonly StreamReader streamReader;
        public readonly StreamWriter streamWriter;
        public readonly TcpClient tcpClient;

        public UserSeesion(TcpClient client)
        {
            this.tcpClient = client;
            this.networkStream = client.GetStream();
            this.streamReader = new StreamReader(this.networkStream, Encoding.Default);
            this.streamWriter = new StreamWriter(this.networkStream, Encoding.Default);
            this.streamWriter.AutoFlush = true;
            this.binaryReader = new BinaryReader(this.networkStream, Encoding.Default);
            this.binaryWriter = new BinaryWriter(this.networkStream, Encoding.Default);
        }

        public void Close()
        {
            this.networkStream.Close();
            this.networkStream.Dispose();
            this.streamReader.Close();
            this.streamReader.Dispose();
            this.streamWriter.Close();
            this.streamWriter.Dispose();
            this.binaryReader.Close();
            this.binaryWriter.Close();
            this.tcpClient.Client.Close();
            this.tcpClient.Close();
        }
    }
}

