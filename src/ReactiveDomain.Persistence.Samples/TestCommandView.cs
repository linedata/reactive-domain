using System;

namespace ReactiveDomain.Persistence.Samples {
    public static class TestCommandView {
        public static void ReDraw(SessionSettings userSelections, string streamName) {

            Console.WriteLine("\n\nOptions...");
            Console.WriteLine($"\tsingle-rd   - connect to ES on {userSelections.Server} using Reactive Domain ");
            Console.WriteLine($"\tdns-rd      - connect to ES cluster with {userSelections.DnsName} and {userSelections.HttpPort} using Reactive Domain ");
            Console.WriteLine($"\tgossip-rd   - connect to ES cluster with {userSelections.ClusterIpEndPoints.Length} Gossip IPs and {userSelections.HttpPort} using Reactive Domain ");
            Console.WriteLine("\t---------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"\tEventStream Tests Stream Name: {streamName}");
            Console.WriteLine($"\tsingle      - connect to ES on with TCP string: {userSelections.TcpConnection}, no Reactive Domain");
            Console.WriteLine($"\tdiscover    - connect to ES cluster with discover string: {userSelections.DnsConnection}, no Reactive Domain");
            Console.WriteLine($"\tgossip      - connect to ES cluster with GossipSeeds string: {userSelections.GossipSeedsConnection}, no Reactive Domain" +
                              $"");
            Console.WriteLine("\t---------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"\tdiscover2   - connect to ES cluster with {userSelections.DnsName} and {userSelections.HttpPort} using DiscoverClusterViaDns, no Reactive Domain");
            Console.WriteLine($"\tgossip2     - connect to ES cluster with {userSelections.ClusterIpEndPoints.Length} Gossip IPs and {userSelections.HttpPort} using DiscoverClusterViaGossipSeeds, no Reactive Domain");
            Console.WriteLine("\t---------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("\tclear       - clear the screen and repaint these options");
            Console.WriteLine("\texit        - exit the application");

            Console.Write("> ");
        }
    }
}
