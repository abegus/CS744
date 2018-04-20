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

        public Dictionary<String, NetworkRegion> regions;   //stores a list of NetworkRegions
        public NetworkEntity processingCenter; // the single pc

        //public List<Transaction> transactions; // grabs all encrypted transactions
        //public JsonData jsonData;
        
            
        //By SALEH: This is to grab the queue limits:
        public Dictionary<String, RelayQueues> relay;        
        //END SALEH's code



        public NetworkViewModel()
        {
            regions = new Dictionary<string, NetworkRegion>(); // [id => NetworkRegion], ... ,

            connections = new List<IpConnection>();
            networkEntities = new Dictionary<string, NetworkEntity>();
            transactions = new List<EncryptedTransaction>();

            //get the transactions and regions which will populate their respective Lists
            getTransactions();
            getRegions();

            //var storeToRelays = from store in db.Stores select store;
            var storesToRelays = db.StoresToRelays.ToList();
            convertStoresToConnections(storesToRelays);
            //connections = 
            //convertStoresToIpConnections(storeToRelays);

            var relayToRelays = from rr in db.RelayToRelayConnections select rr;
            //connections.AddRange(
            convertRelayConnections(relayToRelays);

            var relayToPC = from rpc in db.RelayToProcessCenterConnections select rpc;
            //connections.AddRange(
            convertRelayProcessingConnections(relayToPC);

            //By SALEH: This is to grab the queue limits from relays:
            relay = new Dictionary<string, RelayQueues>();
            getQueues();
            //END SALEH's code
    }
        //By SALEH: This is to grab the queue limits from relays:
        private void getQueues()
        {            
            var rels = db.Relays.ToList();            
            foreach (var rel in rels)
            {
                   relay.Add(rel.relayIP + "", new RelayQueues(rel));
                //string tempIp = rel.relayIP;
                //int tempQueue = rel.relayQueue;
            }
        }
        //END SALEH's code

        private void getRegions()
        {
            var regs = db.Regions.ToList();
            foreach(var reg in regs)
            {
                regions.Add(reg.regionID +"" ,new NetworkRegion(reg));
            }
        }

        private void getTransactions()
        {
            List<Transaction> tranList = (from trans in db.Transactions where trans.encryptedFlag == true select trans).ToList();  //trans.encryptedFlag
            foreach (var trans in tranList)
            {
                transactions.Add(new EncryptedTransaction(trans));
            }
        }

        private void convertStoresToConnections(IEnumerable<StoresToRelays> stores)
        {
            foreach(var sr in stores)
            {
                var isActive = sr.isActive;
                connections.Add(new IpConnection(sr.Store.storeIP, sr.Relay.relayIP, sr.weight, isActive, 0, 1));

                var location1 = getEntityLocation(sr.Store.storeIP);

                //only add the relay it if it doesnt already exist
                if (!regions[sr.Relay.regionID + ""].networkEntities.ContainsKey(sr.Relay.relayIP))// !networkEntities.ContainsKey(store.Relay.relayIP)
                {
                    var location2 = getEntityLocation(sr.Relay.relayIP);

                    NetworkEntity relay = new NetworkEntity(sr.Relay.relayIP, 1, sr.Relay.relayID, location2.Item1, location2.Item2, sr.Relay.isActive, sr.Relay.isGateway, sr.Relay.regionID + "");

                    //OLD ADD, REMOVE
                    networkEntities.Add(sr.Relay.relayIP, relay);
                    //NEW ADD
                    regions[sr.Relay.regionID + ""].networkEntities.Add(sr.Relay.relayIP, relay);
                }

                //only add the Store it if it doesnt already exist
                if (!regions[sr.Relay.regionID + ""].networkEntities.ContainsKey(sr.Store.storeIP))// !networkEntities.ContainsKey(store.Relay.relayIP)
                {
                    var location2 = getEntityLocation(sr.Store.storeIP);

                    NetworkEntity store = new NetworkEntity(sr.Store.storeIP, 0, sr.Store.storeID, location2.Item1, location2.Item2, true, false, sr.Relay.regionID + "");

                    //OLD ADD, REMOVE
                    networkEntities.Add(sr.Store.storeIP, store);
                    //NEW ADD
                    regions[sr.Relay.regionID + ""].networkEntities.Add(sr.Store.storeIP, store);
                }
            }
        }

        /* this  funCTION IS BROKEN. NEED TO REMODEL TO HANDLE MANY TO MANY RELATIONSHIP */
        private List<IpConnection> convertStoresToIpConnections(IEnumerable<Store> stores)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert stores to connections...
            foreach (var store in stores)
            {
                //grab the true/false from isActive
                var isActive = (from con in db.StoresToRelays where con.storeID == store.storeID where store.relayID == con.relayID select con).First().isActive;
                //add a new connection from store to relay
                connections.Add(new IpConnection(store.storeIP, store.Relay.relayIP, store.storeWeight, isActive, 0, 1));//don't have relay to store connections inactivatable in database yet.

                //get locations
                var location1 = getEntityLocation(store.storeIP);

                //add JUST THE STORE to the network entities
                NetworkEntity temp = new NetworkEntity(store.storeIP, 0, store.storeID, location1.Item1, location1.Item2, true, false, store.regionID + "");
                
                //OLD ADD
                networkEntities.Add(store.storeIP, temp); 
                //NEW ADD
                regions[store.regionID + ""].networkEntities.Add(store.storeIP, temp);

                //only add the relay it if it doesnt already exist
                if (!regions[store.Relay.regionID+""].networkEntities.ContainsKey(store.Relay.relayIP))// !networkEntities.ContainsKey(store.Relay.relayIP)
                {
                    var location2 = getEntityLocation(store.Relay.relayIP);

                    NetworkEntity relay = new NetworkEntity(store.Relay.relayIP, 1, store.Relay.relayID, location2.Item1, location2.Item2, store.Relay.isActive, store.Relay.isGateway, store.Relay.regionID+"");
                    
                    //OLD ADD, REMOVE
                    networkEntities.Add(store.Relay.relayIP, relay);
                    //NEW ADD
                    regions[store.Relay.regionID + ""].networkEntities.Add(store.Relay.relayIP, relay);
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
                connections.Add(new IpConnection(relayCon.Relay.relayIP, relayCon.Relay2.relayIP, relayCon.relayWeight, relayCon.isActive, 1 ,1 ));

                //get locations
                var location1 = getEntityLocation(relayCon.Relay.relayIP);
                var location2 = getEntityLocation(relayCon.Relay2.relayIP);

                NetworkEntity temp1 = new NetworkEntity(relayCon.Relay.relayIP, 1, relayCon.Relay.relayID, location1.Item1, location1.Item2, relayCon.Relay.isActive, relayCon.Relay.isGateway, relayCon.Relay.regionID +"");
                NetworkEntity temp2 = new NetworkEntity(relayCon.Relay2.relayIP, 1, relayCon.Relay2.relayID, location2.Item1, location2.Item2, relayCon.Relay2.isActive, relayCon.Relay2.isGateway, relayCon.Relay2.regionID +"");
                if (!regions[relayCon.Relay.regionID + ""].networkEntities.ContainsKey(relayCon.Relay.relayIP))
                {//!networkEntities.ContainsKey(relayCon.Relay.relayIP)) 
                    //OLD ADD
                    networkEntities.Add(relayCon.Relay.relayIP, temp1);
                    //NEW ADD
                    regions[relayCon.Relay.regionID + ""].networkEntities.Add(relayCon.Relay.relayIP, temp1);
                }
                if (!regions[relayCon.Relay2.regionID + ""].networkEntities.ContainsKey(relayCon.Relay2.relayIP))
                {
                    //!networkEntities.ContainsKey(relayCon.Relay2.relayIP))
                    //OLD ADD
                    networkEntities.Add(relayCon.Relay2.relayIP, temp2);
                    //NEW ADD
                    regions[relayCon.Relay2.regionID + ""].networkEntities.Add(relayCon.Relay2.relayIP, temp2);
                }
            }

            return newConnections;
        }

        private List<IpConnection> convertRelayProcessingConnections(IEnumerable<RelayToProcessCenterConnection> rpc)
        {
            List<IpConnection> newConnections = new List<IpConnection>();

            //convert relay-PC to connections...
            foreach(var con in rpc)
            {
                connections.Add(new IpConnection(con.Relay.relayIP, con.ProcessCenter.processCenterIP, con.relayToProcessCenterConnectionWeight, con.isActive, 1, 2)); // not in database yet

                //Get locations
                var location1 = getEntityLocation(con.Relay.relayIP);
                var location2 = getEntityLocation(con.ProcessCenter.processCenterIP);

                NetworkEntity temp1 = new NetworkEntity(con.Relay.relayIP, 1, con.Relay.relayID,location1.Item1, location1.Item2, con.Relay.isActive, con.Relay.isGateway, con.Relay.regionID + "");
                NetworkEntity temp2 = new NetworkEntity(con.ProcessCenter.processCenterIP, 2, con.ProcessCenter.processCenterID, location2.Item1, location2.Item2, con.Relay.isActive, con.Relay.isGateway, "PC cant have a region*");
                if (!regions[con.Relay.regionID + ""].networkEntities.ContainsKey(con.Relay.relayIP))
                {//(!networkEntities.ContainsKey(con.Relay.relayIP))
                    //OLD ADD
                    networkEntities.Add(con.Relay.relayIP, temp1);
                    //NEW ADD
                    regions[con.Relay.regionID + ""].networkEntities.Add(con.Relay.relayIP, temp1);
                }
                if (processingCenter == null)
                {//(!networkEntities.ContainsKey(con.ProcessCenter.processCenterIP))
                    //OLD ADD
                    networkEntities.Add(con.ProcessCenter.processCenterIP, temp2);
                    //NEW ADD
                    processingCenter = temp2;
                }
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
    //By SALEH: 
    public class RelayQueues
    {
        //public string relayID { get; set; }
        public int relayQueueLimit { get; set; }
        //The below are not needed, but left in case someone requests them to be added:
        //public string relayName { get; set; }
        public string relayIP { get; set; }
        //public int regionId { get; set; }
        //public Boolean isActive { get; set; }
        //public Boolean isGateway { get; set; }
        public Dictionary<String, NetworkEntity> networkEntities { get; set; }

        public RelayQueues(Relay relay)
        {
            //this.relayID = relay.relayID;
            this.relayQueueLimit = relay.relayQueue;
            //The below are not needed, but left in case someone requests them to be added:
            //this.relayName = relay.relayName;
            this.relayIP = relay.relayIP;
            //this.regionId = relay.regionID;
            //this.isActive = relay.isActive;
            //this.isGateway = relay.isGateway;
            //this.networkEntities = new Dictionary<string, NetworkEntity>();
        }
    }
    //END SALEH's code
    public class EncryptedTransaction
    {
        public int Id { get; set; }
        //public int? cardId { get; set; }
        public string storeIp { get; set; }
        public bool? transactionStatus { get; set; }
        public string transactionAmount { get; set; }
        public bool? encryptedFlag { get; set; }
        //Start by Saleh
        public long cardNumber { get; set; }
        //public string storeIP { get; set; }
        //End by Saleh
        public EncryptedTransaction(Transaction trans)
        {
            this.Id = trans.transactionID;
            //this.cardId = trans.cardID;
            this.transactionStatus = trans.transactionStatus;
            //this.storeIp = trans.StoreTransactions.FirstOrDefault().Store.storeIP;
            this.encryptedFlag = trans.encryptedFlag;
            this.transactionAmount = trans.transactionAmount;
            //Start by Saleh
            this.cardNumber = trans.cardNumber;
            this.storeIp = trans.storeIP;
            //End by Saleh
    }

}

    public class IpConnection //private?
    {
        //two ips of the connection
        public string ip1 { get; set; }
        public string ip2 { get; set; }
        public int weight { get; set; }
        public bool isActive { get; set; }
        public int type1 { get; set; }
        public int type2 { get; set; }

        public IpConnection (string ip1, string ip2, int? weight, bool isActive, int type1, int type2)
        {
            this.isActive = isActive;
            this.ip1 = ip1;
            this.ip2 = ip2;
            this.weight = weight??0;
            this.type1 = type1;
            this.type2 = type2;
        }
    }

    public class NetworkEntity
    {
        public string ip { get; set; }
        public int type { get; set; }//0 store,   1 relay,    2 PC
        public string databaseId { get; set; }
        public bool isActive { get; set; }
        public bool isGateway { get; set; }
        public string regionId { get; set; }        

        //for view locations
        public decimal x { get; set; }
        public decimal y { get; set; }

        public NetworkEntity(string ip, int type, string databaseId, decimal x, decimal y, 
            bool isActive, bool isGateway, string regionId)
        {
            this.ip = ip;
            this.type = type;
            this.databaseId = databaseId;
            this.x = x;
            this.y = y;
            this.isActive = isActive;
            this.isGateway = isGateway;
            this.regionId = regionId;           
        }
        //maybe a list<Transactions>..... numTransactions....
    }

    public class NetworkRegion
    {
        public NetworkRegion(Regions reg)
        {
            this.gatewayIp = reg.gatewayIP;
            this.name = reg.regionName;
            this.id = reg.regionID +"";
            this.networkEntities = new Dictionary<string, NetworkEntity>();
        }

        public string gatewayIp { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public Dictionary<String, NetworkEntity> networkEntities { get; set; }
    }
}