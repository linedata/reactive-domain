using System;
using System.Net;
using EventStore.ClientAPI;
using ReactiveDomain;
using ReactiveDomain.EventStore;

namespace ReactiveDomain.Persistence.Samples {
    public class TestCommandController {
        private readonly SessionSettings _userSelections;
        private readonly string _streamName;

        public TestCommandController(SessionSettings userSelections, string streamName = null) {
            _userSelections = userSelections;
            _streamName = streamName;
        }

        public void Start() {

            RunProcessingLoop();
        }

        private void RunProcessingLoop() {

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            var command = new[] { "" };
            do {
                TestCommandView.ReDraw(_userSelections, _streamName);
                command = Console.ReadLine()?.Split(' '); // allow port and IP input in the future.

                ReactiveDomain.UserCredentials userCredentials;
                StreamStoreConnectionSettingsBuilder sscSettings;
                EventStoreConnectionManager eventStoreConnectionManager;
                IStreamStoreConnection streamStoreConnection;
                // direct ES setting used by discover2 and gossip2
                ConnectionSettings settings;

                switch (command[0].ToLower()) {
			        case "single-rd":
			            userCredentials = new ReactiveDomain.UserCredentials(_userSelections.Username, _userSelections.Password);
			            sscSettings = StreamStoreConnectionSettings.Create()
			                .SetUserCredentials(userCredentials)
			                .SetSingleServerIpEndPoint(new IPEndPoint(IPAddress.Parse(_userSelections.Server), int.Parse(_userSelections.TcpPort)))
			                .SetTlsConnection(_userSelections.UseTlsConnection, _userSelections.TlsHostName, _userSelections.ValidateTlsCertificate)
			                .SetVerboseLogging(_userSelections.UseVerboseLogging);
			            eventStoreConnectionManager = new EventStoreConnectionManager(sscSettings);
			            var esConnSingleRd = eventStoreConnectionManager.Connection;
			            esConnSingleRd.Connected += (_, __) => Console.WriteLine("Connected");

			            TestConnections.TestReactiveDomainConnection(esConnSingleRd, _streamName);

			            Console.WriteLine($"Connection to single host instance using Reactive Domain");
			            break;
			        case "dns-rd":
			            userCredentials = new ReactiveDomain.UserCredentials(_userSelections.Username, _userSelections.Password);
			            sscSettings = StreamStoreConnectionSettings.Create()
			                .SetUserCredentials(userCredentials)
			                .SetClusterDns(_userSelections.DnsName)
			                .SetClusterGossipPort(int.Parse(_userSelections.HttpPort))
			                .SetTlsConnection(_userSelections.UseTlsConnection, _userSelections.TlsHostName, _userSelections.ValidateTlsCertificate)
			                .SetVerboseLogging(_userSelections.UseVerboseLogging);
			            eventStoreConnectionManager = new EventStoreConnectionManager(sscSettings);
			            streamStoreConnection = eventStoreConnectionManager.Connection;
			            streamStoreConnection.Connected += (_, __) => Console.WriteLine("Connected");

			            TestConnections.TestReactiveDomainConnection(streamStoreConnection, _streamName);

			            Console.WriteLine($"Connection to cluster using DNS and Reactive Domain");
			            break;
			        case "gossip-rd":
			            userCredentials = new ReactiveDomain.UserCredentials(_userSelections.Username, _userSelections.Password);
			            sscSettings = StreamStoreConnectionSettings.Create()
			                .SetUserCredentials(userCredentials)
			                .SetGossipSeedEndPoints(_userSelections.ClusterIpEndPoints)
			                .SetTlsConnection(_userSelections.UseTlsConnection, _userSelections.TlsHostName, _userSelections.ValidateTlsCertificate)
			                .SetVerboseLogging(_userSelections.UseVerboseLogging);
			            eventStoreConnectionManager = new EventStoreConnectionManager(sscSettings);
			            streamStoreConnection = eventStoreConnectionManager.Connection;
			            streamStoreConnection.Connected += (_, __) => Console.WriteLine("Connected");

			            TestConnections.TestReactiveDomainConnection(streamStoreConnection, _streamName);

			            Console.WriteLine($"Connection to cluster using Gossip Ports and Reactive Domain");
			            break;
				    case "single":
				        // Uses string $"ConnectTo=tcp://{Username}:{Password}@{cmd[1]}:{TcpPort}";
                        IEventStoreConnection esConnS = EventStoreConnection.Create(_userSelections.TcpConnection);
				        TestConnections.TestEventStoreConnection(esConnS, _streamName);
						Console.WriteLine($"Connection to single host instance with only EventStore");
					    break;
					case "discover":
						IEventStoreConnection esConnD = EventStoreConnection.Create(_userSelections.DnsConnection);
					    TestConnections.TestEventStoreConnection(esConnD, _streamName);
						Console.WriteLine($"Connection to cluster using DNS: ConnectTo=discover");
						break;
			        case "gossip":
			            IEventStoreConnection esConnG = EventStoreConnection.Create(_userSelections.GossipSeedsConnection);
			            TestConnections.TestEventStoreConnection(esConnG, _streamName);
			            Console.WriteLine($"Connection to cluster with gossip seeds: GossipSeeds=IP:Port");
			            break;
			        case "gossip2":
			            settings = ConnectionSettings.Create()
			                .SetDefaultUserCredentials(new EventStore.ClientAPI.SystemData.UserCredentials(_userSelections.Username, _userSelections.Password))
			                .KeepReconnecting()
			                .KeepRetrying()
			                .UseConsoleLogger()
			                //.UseSslConnection(HostName, true)
			                .Build();

			            var esConnMc = EventStoreConnection.Create(settings,
						    ClusterSettings.Create().DiscoverClusterViaGossipSeeds()
						        .SetGossipSeedEndPoints(_userSelections.ClusterIpEndPoints));
			            TestConnections.TestEventStoreConnection(esConnMc, _streamName);
					    Console.WriteLine($"Connection to three node cluster with gossip seed array");
					    break;
					case "discover2":
					    settings = ConnectionSettings.Create()
					        .SetDefaultUserCredentials(new EventStore.ClientAPI.SystemData.UserCredentials(_userSelections.Username, _userSelections.Password))
					        .KeepReconnecting()
					        .KeepRetrying()
					        .UseConsoleLogger()
					        //.UseSslConnection(HostName, true)
					        .Build();

                        var esConnDns = EventStoreConnection.Create(settings,
						    ClusterSettings.Create().DiscoverClusterViaDns().SetClusterDns(_userSelections.DnsName).SetClusterGossipPort(int.Parse(_userSelections.HttpPort)));
					    TestConnections.TestEventStoreConnection(esConnDns, _streamName);
					    Console.WriteLine($"Connection to cluster using DiscoveryViaDNS");
					    break;
			        case "clear":
			            Console.Clear();
			            break;
				}
            } while (command[0].ToLower() != "exit");
            Console.ResetColor();
	    }
    }
}
