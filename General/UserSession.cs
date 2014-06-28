namespace Qisi.General
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
	/// <summary>
	/// User session.
	/// </summary>
    internal class UserSession
    {
        public readonly BinaryReader binaryReader;
        public readonly BinaryWriter binaryWriter;
        private NetworkStream networkStream;
        public readonly StreamReader streamReader;
        public readonly StreamWriter streamWriter;
        public readonly TcpClient tcpClient;

		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.UserSession"/> class.
		/// </summary>
		/// <param name="client">Client.</param>
        public UserSession(TcpClient client)
        {
            this.tcpClient = client;
            this.networkStream = client.GetStream();
            this.streamReader = new StreamReader(this.networkStream, Encoding.Default);
            this.streamWriter = new StreamWriter(this.networkStream, Encoding.Default);
            this.streamWriter.AutoFlush = true;
            this.binaryReader = new BinaryReader(this.networkStream, Encoding.Default);
            this.binaryWriter = new BinaryWriter(this.networkStream, Encoding.Default);
        }
		/// <summary>
		/// Close this instance.
		/// </summary>
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

