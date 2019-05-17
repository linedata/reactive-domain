using System.Net;
using ReactiveDomain.Foundation;

namespace ReactiveDomain.Persistence.Samples { 
    public class SessionSettings
    {
        public readonly string DefaultUsername = "admin";
        public readonly string DefaultPassword = "changeit";
        public readonly string DefaultServer = "127.0.0.1";
        public readonly string DefaultTcpPort = "1113";
        public readonly string DefaultHttpPort = "2113";
        public readonly string DefaultDnsName = "eventstore";
        public readonly int WindowWidth = 150;
        public readonly int WindowHeight = 40;

        //10.117.9.246, 10.117.10.123, 10.117.9.159
        public readonly string DefaultIpOne = "10.117.10.2";
        public readonly string DefaultIpTwo = "10.117.10.123";
        public readonly string DefaultIpThree = "10.117.10.18";

        // Strings for EventStoreConnection.Create
        public string GossipSeedsConnection;
        public string TcpConnection;
        public string DnsConnection;

        public TestReadModel ReadModel;
        public IRepository Repository;
        public string Username;
        public string Password;
        public string Server;
        public string TcpPort;
        public string HttpPort;
        public string DnsName;
        public string IpOne;
        public string IpTwo;
        public string IpThree;
        public IPEndPoint[] ClusterIpEndPoints;
        public IPEndPoint IpEpOne;
        public IPEndPoint IpEpTwo;
        public IPEndPoint IpEpThree;
        public string TlsConnectionDisplay;
        public bool UseTlsConnection;
        public string VerboseLoggingDisplay;
        public bool UseVerboseLogging;
        public string TlsHostName;
        public string ValidateCertificateDisplay;
        public bool ValidateTlsCertificate;
    }
}
