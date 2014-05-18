namespace Qisi.General
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    internal class User
    {
        public UserSeesion commandSession { get; set; }

        public string currentDir { get; set; }

        public TcpListener dataListener { get; set; }

        public UserSeesion dataSession { get; set; }

        public bool isBinary { get; set; }

        public bool isPassive { get; set; }

        public int loginOK { get; set; }

        public IPEndPoint remoteEndPoint { get; set; }

        public string userName { get; set; }

        public string workDir { get; set; }
    }
}

