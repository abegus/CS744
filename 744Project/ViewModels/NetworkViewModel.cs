using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _744Project.Models;
using System.Collections;


namespace _744Project.ViewModels
{

    public class NetworkViewModel
    {
        private MasterModel db = new MasterModel();

        public List<IpConnection> connections; // a list of the connections between NetworkEntities    (connections)
        public Dictionary<String, NetworkEntity> networkEntities; // a list of data objects from the Network.  (store, relay, PC)

        public NetworkViewModel()
        {
            var storeToRelays = from store in db.Stores select store;
            connections = convertStoresToIpConnections(storeToRelays);

            var relayToRelays = from rr in db.RelayToRelayConnections select rr;
            connections.AddRange(convertRelayConnections(relayToRelays));

            var relayToPC= from rpc in db.RelayToProcessCenterConnections select rpc;
            connections.AddRange(convertRelayProcessingConnections(relayToPC));
        }

        private List<IpConnection> convertStoresToIpConnections(IEnumerable<Store> stores)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert stores to connections...

            return newConnections;
        }

        private List<IpConnection> convertRelayConnections(IEnumerable<RelayToRelayConnection> rr)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-relay to connections...

            return newConnections;
        }

        private List<IpConnection> convertRelayProcessingConnections(IEnumerable<RelayToProcessCenterConnection> rpc)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-relay to connections...

            return newConnections;
        }


    }

    public class IpConnection
    {

    }

    public class NetworkEntity
    {

    }
}