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
        public List<EncryptedTransaction> transactions;
        //public List<Transaction> transactions; // grabs all encrypted transactions
        //public JsonData jsonData;

        public NetworkViewModel()
        {
            connections = new List<IpConnection>();
            networkEntities = new Dictionary<string, NetworkEntity>();
            transactions = new List<EncryptedTransaction>();
            getTransactions();


            var storeToRelays = from store in db.Stores select store;
            //connections = 
            convertStoresToIpConnections(storeToRelays);

            var relayToRelays = from rr in db.RelayToRelayConnections select rr;
            //connections.AddRange(
            convertRelayConnections(relayToRelays);

            var relayToPC = from rpc in db.RelayToProcessCenterConnections select rpc;
            //connections.AddRange(
            convertRelayProcessingConnections(relayToPC);


        }

        private void getTransactions()
        {
            List<Transaction> tranList = (from trans in db.Transactions where trans.encryptedFlag == true select trans).ToList();  //trans.encryptedFlag
            foreach(var trans in tranList)
            {
                transactions.Add(new EncryptedTransaction(trans));
            }
        }

        private List<IpConnection> convertStoresToIpConnections(IEnumerable<Store> stores)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert stores to connections...
            foreach(var store in stores)
            {
                //add a new connection from store to relay
                connections.Add(new IpConnection(store.storeIP, store.Relay.relayIP, store.storeWeight, false));//don't have relay to store connections inactivatable in database yet.

                //get locations
                var location1 = getEntityLocation(store.storeIP);

                //add JUST THE STORE to the network entities
                NetworkEntity temp = new NetworkEntity(store.storeIP, 0, store.storeID, location1.Item1, location1.Item2, true, false);
                networkEntities.Add(store.storeIP, temp);

                //only add it if it doesnt already exist
                if (!networkEntities.ContainsKey(store.Relay.relayIP))
                {
                    var location2 = getEntityLocation(store.Relay.relayIP);

                    NetworkEntity relay = new NetworkEntity(store.Relay.relayIP, 1, store.Relay.relayID, location2.Item1, location2.Item2, store.Relay.isActive, store.Relay.isGateway);
                    networkEntities.Add(store.Relay.relayIP, relay);
                }
               
            }

            return newConnections;
        }

        private List<IpConnection> convertRelayConnections(IEnumerable<RelayToRelayConnection> rr)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-relay to connections...
            foreach(var relayCon in rr)
            {
                connections.Add(new IpConnection(relayCon.Relay.relayIP, relayCon.Relay2.relayIP, relayCon.relayWeight, relayCon.isActive));

                //get locations
                var location1 = getEntityLocation(relayCon.Relay.relayIP);
                var location2 = getEntityLocation(relayCon.Relay2.relayIP);

                NetworkEntity temp1 = new NetworkEntity(relayCon.Relay.relayIP, 1, relayCon.Relay.relayID, location1.Item1, location1.Item2, relayCon.Relay.isActive, relayCon.Relay.isGateway);
                NetworkEntity temp2 = new NetworkEntity(relayCon.Relay2.relayIP, 1, relayCon.Relay2.relayID, location2.Item1, location2.Item2, relayCon.Relay2.isActive, relayCon.Relay2.isGateway);
                if (!networkEntities.ContainsKey(relayCon.Relay.relayIP)) 
                    networkEntities.Add(relayCon.Relay.relayIP, temp1 );
                if (!networkEntities.ContainsKey(relayCon.Relay2.relayIP))
                    networkEntities.Add(relayCon.Relay2.relayIP, temp2 );
            }

            return newConnections;
        }

        private List<IpConnection> convertRelayProcessingConnections(IEnumerable<RelayToProcessCenterConnection> rpc)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-PC to connections...
            foreach(var con in rpc)
            {
                connections.Add(new IpConnection(con.Relay.relayIP, con.ProcessCenter.processCenterIP, con.relayToProcessCenterConnectionWeight, con.isActive)); // not in database yet

                //Get locations
                var location1 = getEntityLocation(con.Relay.relayIP);
                var location2 = getEntityLocation(con.ProcessCenter.processCenterIP);

                NetworkEntity temp1 = new NetworkEntity(con.Relay.relayIP, 1, con.Relay.relayID,location1.Item1, location1.Item2, con.Relay.isActive, con.Relay.isGateway);
                NetworkEntity temp2 = new NetworkEntity(con.ProcessCenter.processCenterIP, 2, con.ProcessCenter.processCenterID, location2.Item1, location2.Item2, con.Relay.isActive, con.Relay.isGateway);
                if (!networkEntities.ContainsKey(con.Relay.relayIP))
                    networkEntities.Add(con.Relay.relayIP, temp1);
                if (!networkEntities.ContainsKey(con.ProcessCenter.processCenterIP))
                    networkEntities.Add(con.ProcessCenter.processCenterIP, temp2);
            }

            return newConnections;
        }

        private Tuple<decimal,decimal> getEntityLocation(string Ip)
        {
            NodePosition nc = db.NodePositions.Find(Ip);
            if(nc == null)
            {
                return new Tuple<decimal,decimal>(0,0); ;
            }
            return new Tuple<decimal, decimal>(nc.x, nc.y);
        }


    }

    public class EncryptedTransaction
    {
        public int Id { get; set; }
        public int? cardId { get; set; }
        public string storeIp { get; set; }
        public bool? transactionStatus { get; set; }
        public string transactionAmount { get; set; }
        public bool? encryptedFlag { get; set; }

        public EncryptedTransaction(Transaction trans)
        {
            this.Id = trans.transactionID;
            this.cardId = trans.cardID;
            this.transactionStatus = trans.transactionStatus;
            this.storeIp = trans.StoreTransactions.FirstOrDefault().Store.storeIP;
            this.encryptedFlag = trans.encryptedFlag;
            this.transactionAmount = trans.transactionAmount;
        }

    }

    public class IpConnection //private?
    {
        //two ips of the connection
        public string ip1 { get; set; }
        public string ip2 { get; set; }
        public int weight { get; set; }
        public bool isActive { get; set; }

        public IpConnection (string ip1, string ip2, int? weight, bool isActive)
        {
            this.isActive = isActive;
            this.ip1 = ip1;
            this.ip2 = ip2;
            this.weight = weight??0;
        }
    }

    public class NetworkEntity
    {
        public string ip { get; set; }
        public int type { get; set; }//0 store,   1 relay,    2 PC
        public string databaseId { get; set; }
        public bool isActive { get; set; }
        public bool isGateway { get; set; }

        //for view locations
        public decimal x { get; set; }
        public decimal y { get; set; }

        public NetworkEntity(string ip, int type, string databaseId, decimal x, decimal y, bool isActive, bool isGateway)
        {
            this.ip = ip;
            this.type = type;
            this.databaseId = databaseId;
            this.x = x;
            this.y = y;
            this.isActive = isActive;
            this.isGateway = isGateway;
        }
        //maybe a list<Transactions>..... numTransactions....
    }
}