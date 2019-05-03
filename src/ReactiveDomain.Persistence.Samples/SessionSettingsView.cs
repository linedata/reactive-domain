using System;

namespace ReactiveDomain.Persistence.Samples {
    public static class SessionSettingsView {

        public static string Redraw(SessionSettings selections) {
            Console.Clear();
            Console.WriteLine(
                "\n--------Selected Settings-----------------------------------------------------------------------------------------");
            Console.WriteLine($"\n\tGeneral Settings");
            Console.WriteLine($"\t\tUsername.................................{selections.Username}");
            Console.WriteLine($"\t\tPassword.................................{selections.Password}");
            Console.WriteLine($"\t\tSingle Server............................{selections.Server}");
            Console.WriteLine($"\t\tTcp Port.................................{selections.TcpPort}");
            Console.WriteLine($"\t\tHttp Port................................{selections.HttpPort}");
            Console.WriteLine($"\t\tDNS Name.................................{selections.DnsName}");

            Console.WriteLine($"\n\tEventStore Connection Strings");
            Console.WriteLine($"\t\tConnectTo=tcp............................{selections.TcpConnection}");
            Console.WriteLine($"\t\tConnectTo=discover.......................{selections.DnsConnection}");
            Console.WriteLine($"\t\tGossipSeeds..............................{selections.GossipSeedsConnection}");

            Console.WriteLine($"\n\tThree Cluster Nodes");
            Console.WriteLine($"\t\tNode One.................................{selections.IpOne}");
            Console.WriteLine($"\t\tNode Two.................................{selections.IpTwo}");
            Console.WriteLine($"\t\tNode Three...............................{selections.IpThree}");

            if (selections.UseTlsConnection) {
                Console.WriteLine($"\n\tTLS Encrypted Connection Settings");
                if (selections.TlsHostName.StartsWith("Required")) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\t\tExpected TLS Certificate Host Name...Error: {selections.TlsHostName}");
                    Console.ResetColor();
                }
                else {
                    Console.WriteLine($"\t\tExpected TLS Certificate Host Name...{selections.TlsHostName}");
                }
                Console.WriteLine($"\t\tValidate the TLS Certificate.........{selections.ValidateCertificateDisplay}");
            } else {
                Console.WriteLine($"\n\tUse a TLS Encrypted Connection is false");
            }

            Console.WriteLine($"\n\tUse Verbose Logging is {selections.VerboseLoggingDisplay}");

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------\n");

            Console.Write($"\tAccept this input?.............[y[es] / * / e[xit] ");
            var input = Console.ReadLine();
            return input;
        }
    }
}