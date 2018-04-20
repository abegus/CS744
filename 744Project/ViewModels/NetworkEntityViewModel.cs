using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _744Project.Models;
using System.Collections;

namespace _744Project.ViewModels
{
    /* Used when a GET is called by the popup in the Network View. It returns a generic NetworkEntity 
     * which can be ither a Store, a Relay, or a Processing Center. */
    public class NetworkEntityViewModel
    {
        public bool isActive { get; set; }
        public string ip { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public int type { get; set; }
        public int? region { get; set; }
        public bool? isGateway { get; set; }
        public int? queueLimit { get; set; }
        public List<string> queue { get; set; } //should be empty, but it can be used by the JS?

        public NetworkEntityViewModel() { }

        public NetworkEntityViewModel(Store store)
        {
            type = 0;
            isActive = true; //CHANGE ONCE STORES HAVE ACTIVE / INACTIVE status
            ip = store.storeIP;
            id = store.storeID;
            region = store.regionID;
            isGateway = null; // a store cannot be a gateway
            queue = new List<string>();
            name = store.storeName;

        }
        public NetworkEntityViewModel(Relay relay)
        {
            type = 1;
            isActive = relay.isActive;
            ip = relay.relayIP;
            id = relay.relayID;
            region = relay.regionID;
            isGateway = relay.isGateway;
            queue = new List<string>();
            queueLimit = relay.relayQueue;
            name = null;
        }
        public NetworkEntityViewModel(ProcessCenter pc)
        {
            type = 1;
            isActive = true; //CHANGE ONCE PC HAVE ACTIVE / INACTIVE status
            ip = pc.processCenterIP;
            id = pc.processCenterID;
            region = null; //cant have a region, they are external
            isGateway = null; //cant be a gateway.
            queue = new List<string>();
            type = 2;
            name = null;
        }
    }
}