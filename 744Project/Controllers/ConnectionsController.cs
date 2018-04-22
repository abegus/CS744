using _744Project.Models;
using _744Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _744Project.Controllers
{
    public class ConnectionsController : Controller
    {
        private MasterModel db = new MasterModel();
        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);

        // GET: Connections
        public ActionResult Index()
        {
            //There is no need to display, so this function wont be used.
            return View(db.Regions.ToList());
        }

        public void getAllIps(int ip)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from ProcessCenters";
            int totalPCS = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Relays";
            int totalRelays = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Stores";
            int totalStores = Convert.ToInt32(cmd.ExecuteScalar());            
            List<string> ip1 = new List<string>();
            List<string> ip2 = new List<string>();
            int ip1Counter = 0;
            int ip2Counter = 0;
            for (int i = 1; i <= totalPCS; i++)
            {
                cmd.CommandText = "select processCenterId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY processCenterId ASC), *FROM processCenters) as t where rowNum = '" + i+"'";
                string PCId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select processCenterIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY processCenterId ASC), *FROM processCenters) as t where rowNum = '" + i + "'";
                string PCIp = cmd.ExecuteScalar().ToString();
                if (ip == 1)
                {
                    ip1Counter++;
                    ip1.Add(PCIp);
                }
                else if (ip == 2)
                {
                    ip2Counter++;
                    ip2.Add(PCIp);
                }
                    
            }
            for (int i = 1; i <= totalRelays; i++)
            {
                cmd.CommandText = "select relayId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays) as t where rowNum = '" + i + "'";
                string relayId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select relayIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays) as t where rowNum = '" + i + "'";
                string relayIp = cmd.ExecuteScalar().ToString();
                if (ip == 1)
                {
                    ip1Counter++;
                    ip1.Add(relayIp);
                }
                else if (ip == 2)
                {
                    ip2Counter++;
                    ip2.Add(relayIp);
                }
            }
            for (int i = 1; i <= totalStores; i++)
            {
                cmd.CommandText = "select storeId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY storeID ASC), *FROM Stores) as t where rowNum = '" + i + "'";
                string storeId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select storeIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY storeID ASC), *FROM Stores) as t where rowNum = '" + i + "'";
                string storeIp = cmd.ExecuteScalar().ToString();
                if (ip == 1)
                {
                    ip1Counter++;
                    ip1.Add(storeIp);
                }
                else if (ip == 2)
                {
                    ip2Counter++;
                    ip2.Add(storeIp);
                }
            }            
            if (ip == 1)
            {                
                ViewBag.ip1 = ip1;
            }
            else if (ip == 2)
                ViewBag.ip2 = ip2;

            connect.Close();
        }


        // GET: Stores/Create
        public ActionResult Create()
        {
            //get all IPs for ip1
            getAllIps(1);
            //get all IPs for ip2
            getAllIps(2);            
            return View();            
        }
        public int getIpType(string ip)
        {
            int type = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from ProcessCenters where processCenterIP like '"+ip+"' ";
            int totalPCs = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Relays where RelayIP like '" + ip + "' ";
            int totalRelays = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Stores where StoreIP like '" + ip + "' ";
            int totalStores = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalPCs > 0)
                type = 1;
            else if (totalRelays > 0)
                type = 2;
            else
                type = 3;
            connect.Close();
            return type;
        }

        public int getRegionId(string ip, int type)
        {
            int regionId = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //type: 1=PC, 2=Relay, 3=Store
            if (type == 1)
            {
                //it's a PC; therefore, it has no region and zero would work for our project.
            }
            else if(type == 2)
            {
                cmd.CommandText = "select regionID from Relays where relayIP like '" + ip + "' ";
                regionId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                cmd.CommandText = "select regionID from Stores where storeIP like '"+ip+"' ";
                regionId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            connect.Close();
            return regionId;
        }
        public Boolean IsGateway(string ip, int type)
        {
            Boolean isGateway = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            if(type == 2)//2 = Relay
            {
                cmd.CommandText = "select isGateway from Relays where relayIP like '"+ip+"' ";
                int intIsGateway = Convert.ToInt32(cmd.ExecuteScalar());
                if (intIsGateway == 1)//true
                    isGateway = true;                
            }            
            connect.Close();
            return isGateway;
        }


        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string ip1, string ip2, int? weight)
        {
            Boolean thereIsError = false;
            //Check if ip1 == ip2
            if (ip1.Equals(ip2))
            {
                //ModelState.AddModelError("ip1", "The first IP matchs the second IP.");
                ModelState.AddModelError("ip2", "The first IP matchs the second IP.");
                thereIsError = true;
            }
            //Check if ip1 is store, relay, or process center
            int ip1Type = getIpType(ip1); //Type: 1=PC, 2=Relay, 3=Store
            //Check if ip2 is store, relay, or process center
            int ip2Type = getIpType(ip2); //Type: 1=PC, 2=Relay, 3=Store
            //get region ID for IP 1:
            int ip1RegionId = getRegionId(ip1, ip1Type);
            //get region ID for IP 2:
            int ip2RegionId = getRegionId(ip2, ip2Type);
            //if any IP is a relay, check if relay is gateway:
            Boolean ip1IsGateway = IsGateway(ip1, ip1Type);
            Boolean ip2IsGateway = IsGateway(ip2, ip2Type);
            
            //if ip1 is store && ip2 is store, the connection is prohibited
            if (ip1Type == 3 && ip2Type == 3)
            {
                //ModelState.AddModelError("ip1", "The first IP is store and cannot be connected to another store.");
                ModelState.AddModelError("ip2", "The first IP and second IP are Stores. Stores cannot be connected.");
                thereIsError = true;
            }

            //if (ip1 is store && ip2 is PC) || (ip1 is PC && ip2 is store) , the connection is prohibited
            if ((ip1Type == 3 && ip2Type == 1) || (ip1Type == 1 && ip2Type == 3))
            {                
                ModelState.AddModelError("ip2", "Stores cannot be connected directly to Process Centers.");
                thereIsError = true;
            }
            //if (ip1 is store && ip2 is relay) || (ip1 is relay && ip2 is store), and they are in different regions, the connection is prohibited
            if (((ip1Type == 3 && ip2Type == 2) || (ip1Type == 2 && ip2Type == 3)) && (ip1RegionId != ip2RegionId))
            {
                ModelState.AddModelError("ip2", "Store and Relay cannot be connected if they are in different regions.");
                thereIsError = true;
            }
            //if ip1 is relay && ip2 is relay, and they are in different regions, and they are not gateways, the connection is prohibited
            if((ip1Type == 2 && ip2Type == 2) && (ip1RegionId != ip2RegionId) && (!ip1IsGateway || !ip2IsGateway)  )
            {
                ModelState.AddModelError("ip2", "Relays cannot be connected if they are not Gateways and in different regions.");
                thereIsError = true;
            }
            //check if weight == NULL
            if (string.IsNullOrWhiteSpace(weight.ToString()) || weight == null)
            {
                ModelState.AddModelError("weight", "The Weight field is required");
                thereIsError = true;
            }
            //check if store.storeWeight < 1 || > 500
            else if (weight < 1 || weight > 500)
            {
                ModelState.AddModelError("weight", "The Weight must be from 1 to 500");
                thereIsError = true;
            }            
            
            if(thereIsError)
            {                    
                //get all IPs for ip1
                getAllIps(1);
                //get all IPs for ip2
                getAllIps(2);
                return View();
            }
            //get all IPs for ip1
            getAllIps(1);
            //get all IPs for ip2
            getAllIps(2);
            addConnection(ip1, ip2, ip1Type, ip2Type, weight);
            //Translate the Type from int to string:
            string strType1 = getTypeToString(ip1Type);
            string strType2 = getTypeToString(ip2Type);

            ViewBag.SuccessMessage = strType1 + ": (" + ip1 + ") has been successfully connected to " + strType2 + ": (" + ip2+")";
            return View();

        }
        public string getTypeToString(int type)
        {
            string strType = "";
            if (type == 1)
                strType = "Process Center";
            else if (type == 2)
                strType = "Relay";
            else
                strType = "Store";
            return strType;
        }
        public string getId(string ip, int type)
        {
            string id = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            if (type == 1)
            {
                cmd.CommandText = "select processCenterID from processCenters where processCenterIP like '"+ip+"' ";
                id = cmd.ExecuteScalar().ToString();
            }
            else if(type == 2)
            {
                cmd.CommandText = "select relayID from relays where relayip like '" + ip + "' ";
                id = cmd.ExecuteScalar().ToString();
            }
            else if(type == 3)
            {
                cmd.CommandText = "select storeId from stores where storeIP like '" + ip + "' ";
                id = cmd.ExecuteScalar().ToString();
            }
            connect.Close();
            return id;
        }
        public Boolean relayPCAreConnected(string id1, string id2, out string tableId)
        {
            Boolean connected = false;
            tableId = "";
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from RelayToProcessCenterConnections where relayID = '" + id1 + "' and processCenterID = '" + id2 + "' ";
            int count1 = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from RelayToProcessCenterConnections where relayID = '" + id2 + "' and processCenterID = '" + id1 + "' ";
            int count2 = Convert.ToInt32(cmd.ExecuteScalar());
            // > 0 means they exist
            if (count1 > 0 && count2 == 0)
            {
                cmd.CommandText = "select relayToProcessCenterConnectionID from RelayToProcessCenterConnections where relayID = '" + id1 + "' and processCenterID = '" + id2 + "' ";
                tableId = cmd.ExecuteScalar().ToString();
                connected = true;
            }
            else if (count1 == 0 && count2 > 0)
            {
                cmd.CommandText = "select relayToProcessCenterConnectionID from RelayToProcessCenterConnections where relayID = '" + id2 + "' and processCenterID = '" + id1 + "' ";
                tableId = cmd.ExecuteScalar().ToString();
                connected = true;
            }
            else if (count1 > 0 && count2 > 0)
            {
                //get the number of rows in the RelayToProcessCenterConnections table:
                cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY relayToProcessCenterConnectionID ASC) ,* FROM RelayToProcessCenterConnections) as t";
                int totalRows = Convert.ToInt32(cmd.ExecuteScalar());
                //Select the last relayToProcessCenterConnectionID:
                cmd.CommandText = "select relayToProcessCenterConnectionID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY relayToProcessCenterConnectionID ASC) ,* FROM RelayToProcessCenterConnections) as t where rowNum = '" + totalRows + "' ";
                tableId = cmd.ExecuteScalar().ToString();
                connected = true;
            }
            return connected;
        }
        public Boolean areTheyConnected(string id1, string id2, out string relayConnectionId)
        {
            Boolean connected = false;
            relayConnectionId = "";
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from relayToRelayConnections where relay_relayID = '"+id1+"' and relay2_relayID = '"+id2+"' ";
            int count1 = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from relayToRelayConnections where relay_relayID = '" + id2 + "' and relay2_relayID = '" + id1 + "' ";
            int count2 = Convert.ToInt32(cmd.ExecuteScalar());
            // > 0 means they exist
            if (count1 > 0 && count2 == 0)
            {
                cmd.CommandText = "select relayConnectionID from relayToRelayConnections where relay_relayID = '" + id1 + "' and relay2_relayID = '" + id2 + "' ";
                relayConnectionId = cmd.ExecuteScalar().ToString();
                connected = true;
            }
            else if (count1 == 0 && count2 > 0)
            {
                cmd.CommandText = "select relayConnectionID from relayToRelayConnections where relay_relayID = '" + id2 + "' and relay2_relayID = '" + id1 + "' ";
                relayConnectionId = cmd.ExecuteScalar().ToString();
                connected = true;
            }
            return connected;
        }
        public void addConnection(string ip1, string ip2, int ip1Type, int ip2Type, int? weight)
        {
            string id1 = getId(ip1, ip1Type);
            string id2 = getId(ip2, ip2Type);
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //The only allowed connections are Gateway to Gateway, Gateway to PC, and Store to Relay
            //if  both are gateways:
            if (ip1Type == 2 && ip2Type == 2)
            {
                //check if the relays are already connected:
                Boolean theyAreAlreadyConnected = areTheyConnected(id1, id2, out string relayConnectionId);
                if (theyAreAlreadyConnected)
                {
                    ModelState.AddModelError("ip2", "The Relays are already connected, but their connection wight has been successfully changed");
                    cmd.CommandText = "update RelayToRelayConnections set relayWeight = '"+weight+"' where relayConnectionID = '"+relayConnectionId+"' ";
                    cmd.ExecuteScalar();
                }
                else
                {
                    cmd.CommandText = "insert into relayToRelayConnections (relayWeight, relay_relayID, relay2_relayID, isActive) " +
                        "values('" + weight + "', '" + id1 + "', '" + id2 + "', '" + true + "')";
                    cmd.ExecuteScalar();
                }
            }
            //if IP1 = gateway and IP2 = PC
            else if (ip1Type == 2 && ip2Type == 1) 
            {
                //check if the relay to PC are already connected:
                Boolean relayToPCConnected = relayPCAreConnected(id1, id2, out string relayToProcessCenterConnectionId);
                if (relayToPCConnected)
                {
                    ModelState.AddModelError("ip2", "The Relay and Process Center are already connected, but their connection wight has been successfully changed");
                    cmd.CommandText = "update RelayToProcessCenterConnections set relayToProcessCenterConnectionWeight = '" + weight + "' where relayToProcessCenterConnectionID = '" + relayToProcessCenterConnectionId + "' ";
                    cmd.ExecuteScalar();
                }
                else
                {
                    cmd.CommandText = "insert into RelayToProcessCenterConnections (relayID, processCenterID, isActive, relayToProcessCenterConnectionWeight) " +
                        "values('" + id1 + "', '" + id2 + "', '" + true + "', '" + weight + "')";
                    cmd.ExecuteScalar();
                }
            }
            //if IP1 = PC and IP2 = gateway
            else if (ip1Type == 1 && ip2Type == 2)
            {
                //check if the relay to PC are already connected:
                Boolean relayToPCConnected = relayPCAreConnected(id1, id2, out string relayToProcessCenterConnectionId);
                if (relayToPCConnected)
                {
                    ModelState.AddModelError("ip2", "The Relay and Process Center are already connected, but their connection wight has been successfully changed");
                    cmd.CommandText = "update RelayToProcessCenterConnections set relayToProcessCenterConnectionWeight = '" + weight + "' where relayToProcessCenterConnectionID = '" + relayToProcessCenterConnectionId + "' ";
                    cmd.ExecuteScalar();
                }
                else
                {
                    cmd.CommandText = "insert into RelayToProcessCenterConnections (relayID, processCenterID, isActive, relayToProcessCenterConnectionWeight) " +
                    "values('" + id2 + "', '" + id1 + "', '" + true + "', '" + weight + "')";
                    cmd.ExecuteScalar();
                }
            }
            //if IP1 = store and IP2 = relay
            else if (ip1Type == 3 && ip2Type == 2)
            {
                cmd.CommandText = "select count(*) from StoresToRelays where relayID like '"+id2+"' and storeID like '"+id1+"' ";
                int totalConnections = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalConnections > 0)
                {
                    cmd.CommandText = "update StoresToRelays set weight = '"+weight+"' where relayID = '"+id2+"' and storeID = '"+id1+"' ";
                    cmd.ExecuteScalar();
                }
                else
                {
                    cmd.CommandText = "insert into StoresToRelays (relayID, storeID, isActive, weight) " +
                    "values('" + id2 + "', '" + id1 + "', '" + true + "', '" + weight + "')";
                    cmd.ExecuteScalar();
                }                
            }
            //if IP1 = relay and IP2 = store
            else if (ip1Type == 2 && ip2Type == 3)
            {
                cmd.CommandText = "select count(*) from StoresToRelays where relayID like '" + id1 + "' and storeID like '" + id2 + "' ";
                int totalConnections = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalConnections > 0)
                {
                    cmd.CommandText = "update StoresToRelays set weight = '" + weight + "' where relayID = '" + id1 + "' and storeID = '" + id2 + "' ";
                    cmd.ExecuteScalar();
                }
                else
                {
                    cmd.CommandText = "insert into StoresToRelays (relayID, storeID, isActive, weight) " +
                        "values('" + id1 + "', '" + id2 + "', '" + true + "', '" + weight + "')";
                    cmd.ExecuteScalar();
                }
            }
            connect.Close();
        }



    }
}