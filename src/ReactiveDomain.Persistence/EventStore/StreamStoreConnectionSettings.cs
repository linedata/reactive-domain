﻿using System.Collections.Generic;
using System.Net;
using EventStore.ClientAPI;
using ReactiveDomain.Util;
using ReactiveDomain.Logging;

namespace ReactiveDomain.EventStore
{
    /// <summary>
    /// A <see cref="StreamStoreConnectionSettings"/> object is an immutable representation of the settings for an
    /// <see cref="IStreamStoreConnection"/>.
    /// </summary>
    /// <remarks>
    /// The preferred method to build a <see cref="StreamStoreConnectionSettings"/> object is using
    /// a <see cref="StreamStoreConnectionSettingsBuilder"/>, either via the <see cref="Create"/> method, or via
    /// the constructor of <see cref="StreamStoreConnectionSettingsBuilder"/>.
    /// This class supports connections to a single node or a cluster of nodes. There is no preference of the connection
    /// type. If conflicting values are provided, a <see cref="StreamStoreConnectionException"/> is thrown.
    /// </remarks>
    public sealed class StreamStoreConnectionSettings
    {
        /// <summary>
        /// Creates a new set of <see cref="ConnectionSettings"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ConnectionSettingsBuilder"/> you can use to build up a <see cref="ConnectionSettings"/> via
        /// the Fluent design pattern.
        /// </returns>.
        public static StreamStoreConnectionSettingsBuilder Create()
        {
            return new StreamStoreConnectionSettingsBuilder();
        }

        /// <summary>
        /// The <see cref="ReactiveDomain.UserCredentials"/> to use for operations.
        /// </summary>
        public readonly UserCredentials UserCredentials;

        /// <summary>
        /// The DNS name to use for discovering endpoints.
        /// </summary>
        public readonly string ClusterDns;

        /// <summary>
        /// Endpoints for seeding gossip if not using DNS.
        /// </summary>
        public readonly GossipSeed[] GossipSeeds;

        /// <summary>
        /// Network IP Port used by the connection. Can be the single server TCP port, or the external HTTP port used for cluster gossip communication.
        /// </summary>
        public readonly int NetworkIpPort;

        /// <summary>
        /// The IPAddress of a single data store instance.
        /// </summary>
        public readonly IPAddress SingleServerIpAddress;

        /// <summary>
        /// Apply the verbose internal logging from <see cref="EventStoreConnection"/> internal logic. Default: False.
        /// </summary>
        public readonly bool VerboseLogging;

        /// <summary>
        /// The <see cref="Logging.ILogger"/> that this connection will use.
        /// </summary>
        public readonly Logging.ILogger Log;

        /// <summary>
        /// Convenience connection type variables
        /// </summary>
        private ConnectionType _connectionType = 0;

        private enum ConnectionType {
            SingleNode,
            DnsCluster,
            GossipSeedsCluster
        }

        public bool IsSingleConnection => _connectionType == ConnectionType.SingleNode;
        public bool IsDnsClusterConnection => _connectionType == ConnectionType.DnsCluster;
        public bool IsGossipSeedClusterConnection => _connectionType == ConnectionType.GossipSeedsCluster;

        internal StreamStoreConnectionSettings(
            UserCredentials userCredentials,
            IPAddress singleServerIpAddress,
            string clusterDns,
            IReadOnlyList<IPAddress> ipAddresses,
            int networkIpPort,
            Logging.ILogger log,
            bool verboseLogging = false)
        {
            Ensure.NotNull(log, nameof(log));
            Ensure.NotNull(userCredentials, nameof(userCredentials));
            Ensure.Between(1024, 65535, networkIpPort, nameof(networkIpPort));

            if (singleServerIpAddress != null && !string.IsNullOrWhiteSpace(clusterDns) ||
                singleServerIpAddress != null && ipAddresses != null && ipAddresses.Count > 0 ||
                !string.IsNullOrWhiteSpace(clusterDns) && ipAddresses != null && ipAddresses.Count > 0) {
                    throw new StreamStoreConnectionException("Conflicting server or cluster input passed.");
            }

            if (singleServerIpAddress != null) {
                _connectionType = ConnectionType.SingleNode;
            } else if (!string.IsNullOrWhiteSpace(clusterDns)) {
                _connectionType = ConnectionType.DnsCluster;
            } else if (ipAddresses != null && ipAddresses.Count > 0) {
                _connectionType = ConnectionType.GossipSeedsCluster;

                GossipSeeds = new GossipSeed[ipAddresses.Count];
                for (var i = 0; i < ipAddresses.Count; i++)
                {
                    var ipendpoint = new IPEndPoint(ipAddresses[i], networkIpPort);
                    GossipSeeds[i] = new GossipSeed(ipendpoint, ipAddresses[i].ToString());
                }
            }

            UserCredentials = userCredentials;
            SingleServerIpAddress = singleServerIpAddress;
            ClusterDns = clusterDns;
            NetworkIpPort = networkIpPort;
            Log = log;
            VerboseLogging = verboseLogging;
        }
    }
}