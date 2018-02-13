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
        public Dictionary<String, NetworkEntity> networkEntities; //Key: IP, Value: NetworkEntiy. a list of data objects from the Network.  (store, relay, PC)

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
            foreach(var store in stores)
            {
                //add a new connection from store to relay
                newConnections.Add(new IpConnection(store.storeIP, store.Relay.relayIP, store.storeWeight));
                //add JUST THE STORE to the network entities
                networkEntities.Add(store.storeIP, new NetworkEntity(store.storeIP, 0, store.storeID));
            }

            return newConnections;
        }

        private List<IpConnection> convertRelayConnections(IEnumerable<RelayToRelayConnection> rr)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-relay to connections...
            foreach(var relayCon in rr)
            {
                newConnections.Add(new IpConnection(relayCon.Relay.relayIP, relayCon.Relay2.relayIP, relayCon.relayWeight));

                //do I just add the first one, or both?
                networkEntities.Add(relayCon.Relay.relayIP, new NetworkEntity(relayCon.Relay.relayIP, 1, relayCon.Relay.relayID));
                networkEntities.Add(relayCon.Relay2.relayIP, new NetworkEntity(relayCon.Relay2.relayIP, 1, relayCon.Relay2.relayID));
            }

            return newConnections;
        }

        private List<IpConnection> convertRelayProcessingConnections(IEnumerable<RelayToProcessCenterConnection> rpc)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-PC to connections...
            foreach(var con in rpc)
            {
                newConnections.Add(new IpConnection(con.Relay.relayIP, con.ProcessCenter.processCenterIP, con.relayToProcessCenterConnectionWeight));

                //do I just add the first one, or both?
                networkEntities.Add(con.Relay.relayIP, new NetworkEntity(con.Relay.relayIP, 1, con.Relay.relayID));
                networkEntities.Add(con.ProcessCenter.processCenterIP, new NetworkEntity(con.ProcessCenter.processCenterIP, 2, con.ProcessCenter.processCenterID));
            }

            return newConnections;
        }


    }

    public class IpConnection //private?
    {
        //two ips of the connection
        public string ip1 { get; set; }
        public string ip2 { get; set; }
        public int weight { get; set; }

        public IpConnection (string ip1, string ip2, int? weight)
        {
            ip1 = this.ip1;
            ip2 = this.ip2;
            weight = this.weight;
        }
    }

    public class NetworkEntity
    {
        public string ip { get; set; }
        public int type { get; set; }//0 store,   1 relay,    2 PC
        public string databaseId { get; set; }

        public NetworkEntity(string ip, int type, string databaseId)
        {
            ip = this.ip;
            type = this.type;
            databaseId = this.databaseId;
        }
        //maybe a list<Transactions>..... numTransactions....
    }
}