using System;
using System.Globalization;
using System.Net;
using ReactiveDomain.Util;

namespace ReactiveDomain.Persistence.Samples {
    public class SessionSettingsController : IController {
        private SessionSettings _userSelections;
        private string _streamName;

        public SessionSettingsController(SessionSettings userSelections, string streamName = null) {
            _userSelections = userSelections;
            _streamName = streamName;
        }

        public void Start() {

            string input;
            do {
                RunProcessingLoop();
                input = SessionSettingsView.Redraw(_userSelections).ToUpperInvariant();
            } while (!input.StartsWith("Y") && 
                     !input.StartsWith("E"));
            if(input.StartsWith("E")) return;
        }

        public void RunProcessingLoop() { 
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Enter Test selections...[default]:");
            Console.Write($"\tUsername...................[{_userSelections.Username}] ");
            var input = Console.ReadLine();
            _userSelections.Username = input.IsEmptyString() ? _userSelections.Username : input;
            Console.Write($"\tPassword...................[{_userSelections.Password}] ");
            input = Console.ReadLine();
            _userSelections.Password = input.IsEmptyString() ? _userSelections.Password : input;
            Console.Write($"\tSingle Server..............[{_userSelections.Server}] ");
            input = Console.ReadLine();
            _userSelections.Server = input.IsEmptyString() ? _userSelections.Server : input;
            Console.Write($"\tTcp Port...................[{_userSelections.TcpPort}] ");
            input = Console.ReadLine();
            _userSelections.TcpPort = input.IsEmptyString() ? _userSelections.TcpPort : input;
            Console.Write($"\tHttp Port..................[{_userSelections.HttpPort}] ");
            input = Console.ReadLine();
            _userSelections.HttpPort = input.IsEmptyString() ? _userSelections.HttpPort : input;

            Console.Write($"\tDNS Name...................[{_userSelections.DnsName}] ");
            input = Console.ReadLine();
            _userSelections.DnsName = input.IsEmptyString() ? _userSelections.DnsName : input;

            Console.WriteLine("\n\t3 Cluster Nodes");
            Console.Write($"\t\tNode One...........[{_userSelections.IpOne}] ");
            input = Console.ReadLine();
            _userSelections.IpEpOne = input.IsEmptyString()
                ? new IPEndPoint(IPAddress.Parse(_userSelections.IpOne), int.Parse(_userSelections.HttpPort))
                : new IPEndPoint(IPAddress.Parse(input), int.Parse(_userSelections.HttpPort));
            Console.Write($"\t\tNode Two...........[{_userSelections.IpTwo}] ");
            input = Console.ReadLine();
            _userSelections.IpEpTwo = input.IsEmptyString()
                ? new IPEndPoint(IPAddress.Parse(_userSelections.IpTwo), int.Parse(_userSelections.HttpPort))
                : new IPEndPoint(IPAddress.Parse(input), int.Parse(_userSelections.HttpPort));
            Console.Write($"\t\tNode Three.........[{_userSelections.IpThree}] ");
            input = Console.ReadLine();
            _userSelections.IpEpThree = input.IsEmptyString()
                ? new IPEndPoint(IPAddress.Parse(_userSelections.IpThree), int.Parse(_userSelections.HttpPort))
                : new IPEndPoint(IPAddress.Parse(input), int.Parse(_userSelections.HttpPort));

            Console.Write($"\n\tUse TLS Connection.........[{_userSelections.TlsConnectionDisplay}] ");
            input = Console.ReadLine();
            _userSelections.TlsConnectionDisplay = input.IsEmptyString() ? _userSelections.TlsConnectionDisplay : input;
            _userSelections.UseTlsConnection =
                _userSelections.TlsConnectionDisplay.StartsWith("T", true, CultureInfo.InvariantCulture);
            _userSelections.TlsConnectionDisplay = _userSelections.UseTlsConnection ? "true" : "false";

            if (_userSelections.UseTlsConnection)
            {
                Console.Write($"\t\tTLS Host Name..............[{_userSelections.TlsHostName}] ");
                input = Console.ReadLine();
                _userSelections.TlsHostName = input.IsEmptyString() ? "Required field" : input;

                Console.Write($"\t\tValidate TLS Certificate...[{_userSelections.ValidateCertificateDisplay}] ");
                input = Console.ReadLine();
                _userSelections.ValidateCertificateDisplay =
                    input.IsEmptyString() ? _userSelections.ValidateCertificateDisplay : input;
                _userSelections.ValidateTlsCertificate =
                    _userSelections.ValidateCertificateDisplay.StartsWith("T", true, CultureInfo.InvariantCulture);
                _userSelections.ValidateCertificateDisplay = _userSelections.ValidateTlsCertificate ? "true" : "false";
            }

            Console.Write($"\n\tVerbose Logging............[{_userSelections.VerboseLoggingDisplay}] ");
            input = Console.ReadLine();
            _userSelections.VerboseLoggingDisplay = input.IsEmptyString() ? _userSelections.VerboseLoggingDisplay : input;
            _userSelections.UseVerboseLogging =
                _userSelections.VerboseLoggingDisplay.StartsWith("T", true, CultureInfo.InvariantCulture);
            _userSelections.VerboseLoggingDisplay = _userSelections.UseVerboseLogging ? "true" : "false";

            // define ES connection strings some tests
            _userSelections.ClusterIpEndPoints = new[] {_userSelections.IpEpOne, _userSelections.IpEpTwo, _userSelections.IpEpThree};
            _userSelections.GossipSeedsConnection =
                $"GossipSeeds={_userSelections.IpEpOne},{_userSelections.IpEpTwo},{_userSelections.IpEpThree}";
            _userSelections.TcpConnection =
                $"ConnectTo=tcp://{_userSelections.Username}:{_userSelections.Password}@{_userSelections.Server}:{_userSelections.TcpPort}";
            _userSelections.DnsConnection =
                $"ConnectTo=discover://{_userSelections.Username}:{_userSelections.Password}@{_userSelections.DnsName}:{_userSelections.HttpPort}";

            Console.ResetColor();
        }
    }
}
