using System;

namespace ReactiveDomain.Persistence.Samples {
    public class Program {

        public static void Main(string[] args) {
            Console.WriteLine("Reactive Domain Test Application Starting...");
	        Console.WriteLine("\tUsing Reactive Domain connections to any single host, DNS cluster, and Gossip Seeds cluster");
            Console.WriteLine("\twith companion EventStore connections for comparison.");
            Console.WriteLine("\tOptional argument: stream-name used by the EventStore tests.");

            string streamName = args.Length.Equals(0) ? $"EventStoreTest-{DateTime.Now:yyyy-MM-dd}" : args[0];

            var app = new Application();
            app.Run(streamName);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }

    public class Application {

        public void Run(string streamName)
        {
            var selectedSettings = new SessionSettings();
            var sessionSettingsController = new SessionSettingsController(selectedSettings, streamName);
            var testCommandsController = new TestCommandController(selectedSettings, streamName);


            Console.WindowWidth = selectedSettings.WindowWidth;
            Console.WindowHeight = selectedSettings.WindowHeight;

            InitializeInput(selectedSettings);
            
            try
            {
                sessionSettingsController.Start();
                testCommandsController.Start();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error on input: {e.Message}");
                Console.WriteLine("Press any key to exit.");
                Console.ResetColor();
                Console.ReadLine();
                Environment.Exit(11);
            }
        }

        void InitializeInput(SessionSettings settings)
        {
            settings.Username = settings.DefaultUsername;
            settings.Password = settings.DefaultPassword;
            settings.Server = settings.DefaultServer;
            settings.TcpPort = settings.DefaultTcpPort;
            settings.HttpPort = settings.DefaultHttpPort;
            settings.DnsName = settings.DefaultDnsName;
            settings.IpOne = settings.DefaultIpOne;
            settings.IpTwo = settings.DefaultIpTwo;
            settings.IpThree = settings.DefaultIpThree;
            settings.TlsConnectionDisplay = "false";
            settings.VerboseLoggingDisplay = "false";
            settings.TlsHostName = string.Empty;
            settings.ValidateCertificateDisplay = "false";
        }
    }
}
