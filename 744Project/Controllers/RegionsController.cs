using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _744Project.Models;
using _744Project.ViewModels;

namespace _744Project.Controllers
{
    public class RegionsController : Controller
    {
        private MasterModel db = new MasterModel();
        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);

        // GET: Regions
        public ActionResult Index()
        {
            return View(db.Regions.ToList());
        }

        // GET: Regions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regions regions = db.Regions.Find(id);
            if (regions == null)
            {
                return HttpNotFound();
            }
            return View(regions);
        }

        // GET: Regions/Create
        public ActionResult Create()
        {
            //get all relay IPs 
            getAllIps();
            var vModel = new RegionsRelaysStoresViewModel();
            return View(vModel);
        }

        public Boolean checkDuplicateIP(string ip, out string errorMessage)
        {
            Boolean duplicate = false;
            errorMessage = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from ProcessCenters where processCenterIP like '"+ip+"' ";
            int processCenters = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Relays where relayIP like '" + ip + "' ";
            int relays = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Stores where storeIP like '" + ip + "' ";
            int stores = Convert.ToInt32(cmd.ExecuteScalar());
            if (processCenters > 0) //check if the IP exists in the process centers table; although we only have one 1 PC, it doesn't hurt to check :)
            {
                errorMessage = "Input Error: The IP you entered matches a Process Center's IP";
                duplicate = true;
            }
            else if (relays > 0) //check if the IP exists in relays
            {
                errorMessage = "Input Error: The IP you entered matches a Relay's IP";
                duplicate = true;
            }
            else if (stores > 0) //check if the IP exists in stores
            {
                errorMessage = "Input Error: The IP you entered matches a Store's IP";
                duplicate = true;
            }
            connect.Close();
            return duplicate;
        }


        public Boolean checkForCorrectIP(string ip, out string errorMessage)
        {
            Boolean correct = true;
            errorMessage = "";            
            if (ip.Length < 11)//minimum length should match: 192.168.0.X = 11
            {
                errorMessage = "Input Error: The IP must be more than 11 characters";                    
                correct = false;
            }
            else if (ip.Length > 13)//max length: 192.168.0.XXX = 13
            {
                errorMessage = "Input Error: The IP must be less than 13 characters";                
                correct = false;
            }
            else if (!ip.Substring(0, 10).Equals("192.168.0."))//our standard IP must start with "192.168.0."
            {
                errorMessage = "Input Error: The IP must start with '192.168.0.' ";
                correct = false;
            }
            else if(string.IsNullOrWhiteSpace(ip.Substring(10)))//check if last digits of ip are empty spaces
            {
                errorMessage = "Input Error: The IP cannot end with empty spaces";
                correct = false;
            }
            else if (!ip.Substring(10).All(char.IsDigit))//check last chars of IP are digits
            {
                errorMessage = "Input Error: The IP must end with digits";
                correct = false;
            }            
            else if (  Convert.ToInt32(ip.Substring(10)) < 1 || Convert.ToInt32(ip.Substring(10)) >254)//end of IP cannot be 0 nor 255 because they are reserved according to project description
            {
                errorMessage = "Input Error: The last digits of the IP must be anywhere from 1 to 254";
                correct = false;
            }
            return correct;
        }

        public void getAllIps()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from ProcessCenters";
            int totalPCS = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from Relays where isGateway = 1";
            int totalRelays = Convert.ToInt32(cmd.ExecuteScalar());
            List<string> ip = new List<string>();
            int ip1Counter = 0;
            for (int i = 1; i <= totalPCS; i++)
            {
                cmd.CommandText = "select processCenterId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY processCenterId ASC), *FROM processCenters) as t where rowNum = '" + i + "'";
                string PCId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select processCenterIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY processCenterId ASC), *FROM processCenters) as t where rowNum = '" + i + "'";
                string PCIp = cmd.ExecuteScalar().ToString();
                    ip1Counter++;
                    ip.Add(PCIp);                
            }
            for (int i = 1; i <= totalRelays; i++)
            {
                cmd.CommandText = "select relayId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays where isGateway = 1) as t where rowNum = '" + i + "'";
                string relayId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select relayIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays where isGateway = 1) as t where rowNum = '" + i + "'";
                string relayIp = cmd.ExecuteScalar().ToString();
                ip1Counter++;
                ip.Add(relayIp);
            }
            ViewBag.ip = ip;
            connect.Close();
        }
        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "regionID,regionName,gatewayIP")] Regions regions,
            [Bind(Include ="relayID, relayName, relayIP, regionID, isActive, relayQueue, isGateway")] Relay relay,
            [Bind(Include = "storeID, storeName, storeWeight, relayID, storeIP, regionID")] Store store,
            [Bind(Include = "relayToProcessCenterConnectionID, relayID, processCenterID, isActive, relayToProcessCenterConnectionWeight")] RelayToProcessCenterConnection relayToProcessCenterConnection,
            string ip, int? gatewayWeight)
        {
            var vModel = new RegionsRelaysStoresViewModel();
            //
            Boolean thereIsAnError = false;
            string gateway_duplicate_error = "";
            string gateway_input_error = "";
            Boolean gateway_correctIP = true;
            //check if Gateway IP field is empty:
            if (string.IsNullOrEmpty(regions.gatewayIP))//check for empty input
            {
                ModelState.AddModelError("regions.gatewayIP", "The Region Gateway IP field is required");
                thereIsAnError = true;
            }
            else
            {
                //check gateway IP if it the correct format: 192.168.0.XXX; XXX: 0 -> 255                
                gateway_correctIP = checkForCorrectIP(regions.gatewayIP, out gateway_input_error);
                if (!gateway_correctIP)
                {
                    ModelState.AddModelError("regions.gatewayIP", gateway_input_error);
                    thereIsAnError = true;
                }
            }
            if (gateway_correctIP && !thereIsAnError)//this is to avoid input error with special characters that would crash the application
            {
                //check gateway IP if it matches another IP
                Boolean gateway_duplicate = checkDuplicateIP(regions.gatewayIP, out gateway_duplicate_error);
                if (gateway_duplicate)
                {
                    ModelState.AddModelError("regions.gatewayIP", gateway_duplicate_error);
                    thereIsAnError = true;
                }
            }
            //regions.regionName = regions.regionID.ToString();
            ////check if regions.regionName == NULL:
            //if(string.IsNullOrWhiteSpace(regions.regionName))
            //{
            //    ModelState.AddModelError("regions.regionName", "The Region Name field is required");
            //    thereIsAnError = true;
            //}
            ////check if relay.relayName == NULL: (relay is the gateway)
            //if (string.IsNullOrWhiteSpace(relay.relayName))
            //{
            //    ModelState.AddModelError("relay.relayName", "The Gateway Name field is required");
            //    thereIsAnError = true;
            //}
            //check if relay.relayQueue == NULL
            if (string.IsNullOrWhiteSpace(relay.relayQueue.ToString()))
            {
                ModelState.AddModelError("relay.relayQueue", "The Gateway Queue Limit field is required");
                thereIsAnError = true;
            }            
            //check if relay.relayQueue <1 || > 500
            else if (relay.relayQueue < 1 || relay.relayQueue > 500)
            {
                ModelState.AddModelError("relay.relayQueue", "The Gateway Queue Limit must be from 1 to 500");
                thereIsAnError = true;
            }
            //check for store.storeIP input
            string store_duplicate_error = "";
            string store_input_error = "";
            Boolean store_correctIP = true;
            //check if Store IP field is empty:
            if (string.IsNullOrEmpty(store.storeIP))//check for empty input
            {
                ModelState.AddModelError("store.storeIP", "The Store IP field is required");
                thereIsAnError = true;
            }
            //check if the store IP is duplicate to the input of gateway IP:
            else if (store.storeIP.Equals(regions.gatewayIP))
            {
                ModelState.AddModelError("store.storeIP", "The Store IP must be different from the Gateway IP");
                thereIsAnError = true;
            }
            else
            {
                //check store IP if it the correct format: 192.168.0.XXX; XXX: 0 -> 255                
                store_correctIP = checkForCorrectIP(store.storeIP, out store_input_error);
                if (!store_correctIP)
                {
                    ModelState.AddModelError("store.storeIP", store_input_error);
                    thereIsAnError = true;
                }                
            }
            if (store_correctIP && !thereIsAnError)//this is to avoid input error with special characters that would crash the application
            {
                //check store IP if it matches another IP
                Boolean store_duplicate = checkDuplicateIP(store.storeIP, out store_duplicate_error);
                if (store_duplicate)
                {
                    ModelState.AddModelError("store.storeIP", store_duplicate_error);
                    thereIsAnError = true;
                }
            }
            //check if store.storeName == NULL
            if (string.IsNullOrEmpty(store.storeName))//check for empty input
            {
                ModelState.AddModelError("store.storeName", "The Store Name field is required");
                thereIsAnError = true;
            }
            //check if store.storeWeight == NULL
            if (string.IsNullOrWhiteSpace(store.storeWeight.ToString()))
            {
                ModelState.AddModelError("store.storeWeight", "The Store Weight field is required");
                thereIsAnError = true;
            }
            //check if store.storeWeight < 1 || > 500
            else if (store.storeWeight < 1 || store.storeWeight > 500)
            {
                ModelState.AddModelError("store.storeWeight", "Input Error: The Weight must be from 1 to 500");
                thereIsAnError = true;
            }
            ////check if relay to PC Weight == NULL
            //if (string.IsNullOrWhiteSpace(relayToProcessCenterConnection.relayToProcessCenterConnectionWeight.ToString()))
            //{
            //    ModelState.AddModelError("relayToProcessCenterConnection.relayToProcessCenterConnectionWeight", "The Relay to PC Weight field is required");
            //    thereIsAnError = true;
            //}
            //check if relay to PC Weight < 1 || > 500
            //else if (relayToProcessCenterConnection.relayToProcessCenterConnectionWeight < 1 || relayToProcessCenterConnection.relayToProcessCenterConnectionWeight > 500)
            //{
            //    ModelState.AddModelError("store.storeWeight", "Input Error: The Weight must be from 1 to 500");
            //    thereIsAnError = true;
            //}
            //check if gateway weight is null:
            if(gatewayWeight == null)
            {
                ModelState.AddModelError("gatewayWeight", "Input Error: The Weight must be from 1 to 500");
                thereIsAnError = true;
            }
            //check if there is an error anywhere. If there is, return the viewModel:
            if (thereIsAnError)
            {
                //get all relay IPs 
                getAllIps();
                return View(vModel);
            }
            //            
            db.Regions.Add(regions);
            db.SaveChanges();
            //            
            //get the last RegionID which was just created:
            int lastRegionId = getLastRegionId();
            //update region name and set it to region ID:
            updateRegionName(lastRegionId);
            //get the last existing RelayID in the database then add 1 to it:
            string lastRelayId = (Convert.ToInt32(getLastRelayId()) + 1).ToString();
            //add the new gateway in the relays table:
            saveRelay(lastRelayId, relay.relayName, regions.gatewayIP, lastRegionId,  relay.isActive,  relay.relayQueue, relay.isGateway);
            //get the last RelayID that was just created:
            lastRelayId = getLastRelayId();
            //add a connection from gateway to process center:
            //saveRelayToProcessCenter(lastRelayId, 1, true, relayToProcessCenterConnection.relayToProcessCenterConnectionWeight);
            connectGatewayToIP(regions.gatewayIP, ip, gatewayWeight);
            //get the last existing Store in the database then add 1 to it:
            string lastStoreId = (Convert.ToInt32(getLastStoreId()) + 1).ToString();
            //add the new store in the stores table:
            saveStore(lastStoreId, store.storeName, store.storeWeight, lastRelayId, store.storeIP, lastRegionId);
            //get the last StoreID that was just created:
            lastStoreId = getLastStoreId();
            //add storeID & relayID to StoreToRelays table:
            saveStoresToRelays(lastRelayId, lastStoreId, true, store.storeWeight);
            //
            //return RedirectToAction("Index");
            ViewBag.SuccessMessage = "The new region has been successfully added to the network!";
            //get all relay IPs 
            getAllIps();
            return View();
        }
        public void connectGatewayToIP(string oldIp, string newIp, int? gatewayWeight)
        {

            int type = getIpType(newIp);//1= PC, 2=Relay
            addConnection(oldIp, newIp, 2, type, gatewayWeight);            
        }
        public string getId(string ip, int type)
        {
            string id = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            if (type == 1)
            {
                cmd.CommandText = "select processCenterID from processCenters where processCenterIP like '" + ip + "' ";
                id = cmd.ExecuteScalar().ToString();
            }
            else if (type == 2)
            {
                cmd.CommandText = "select relayID from relays where relayip like '" + ip + "' ";
                id = cmd.ExecuteScalar().ToString();
            }
            else if (type == 3)
            {
                cmd.CommandText = "select storeId from stores where storeIP like '" + ip + "' ";
                id = cmd.ExecuteScalar().ToString();
            }
            connect.Close();
            return id;
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
                cmd.CommandText = "insert into relayToRelayConnections (relayWeight, relay_relayID, relay2_relayID, isActive) " +
                    "values('" + weight + "', '" + id1 + "', '" + id2 + "', '" + true + "')";
                cmd.ExecuteScalar();             
            }
            //if IP1 = gateway and IP2 = PC
            else if (ip1Type == 2 && ip2Type == 1)
            {
                cmd.CommandText = "insert into RelayToProcessCenterConnections (relayID, processCenterID, isActive, relayToProcessCenterConnectionWeight) " +
                    "values('" + id1 + "', '" + id2 + "', '" + true + "', '" + weight + "')";
                cmd.ExecuteScalar();
                
            }
            //if IP1 = PC and IP2 = gateway
            else if (ip1Type == 1 && ip2Type == 2)
            {    
                cmd.CommandText = "insert into RelayToProcessCenterConnections (relayID, processCenterID, isActive, relayToProcessCenterConnectionWeight) " +
                "values('" + id2 + "', '" + id1 + "', '" + true + "', '" + weight + "')";
                cmd.ExecuteScalar();
                
            }
            //if IP1 = store and IP2 = relay
            else if (ip1Type == 3 && ip2Type == 2)
            {
                cmd.CommandText = "select count(*) from StoresToRelays where relayID like '" + id2 + "' and storeID like '" + id1 + "' ";
                int totalConnections = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalConnections > 0)
                {
                    cmd.CommandText = "update StoresToRelays set weight = '" + weight + "' where relayID = '" + id2 + "' and storeID = '" + id1 + "' ";
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
                cmd.CommandText = "select count(*) from StoresToRelays where relayID like '" + id2 + "' and storeID like '" + id1 + "' ";
                int totalConnections = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalConnections > 0)
                {
                    cmd.CommandText = "update StoresToRelays set weight = '" + weight + "' where relayID = '" + id2 + "' and storeID = '" + id1 + "' ";
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
        public int getIpType(string ip)
        {
            int type = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from processCenters where processCenterIP like '"+ip+"' ";
            int totalPc = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select count(*) from relays where relayIP like '" + ip + "' ";
            int totalRelay = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalPc > 0)
                type = 1;
            else
            {
                type = 2;
            }
            connect.Close();
            return type;
        }
        public void updateRegionName(int lastRegionId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "update Regions set regionName = '"+lastRegionId+"' where regionID = '"+lastRegionId+"' ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        public void saveRelayToProcessCenter(string relayId, int processCenter, Boolean isActive, int? weight)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into RelayToProcessCenterConnections(relayID, processCenterID, isActive, relayToProcessCenterConnectionWeight) " +
                "values ('" + relayId + "', '" + processCenter + "', '" + isActive + "', '" + weight + "') ";
            cmd.ExecuteScalar();
            connect.Close();
        }

        public void saveStoresToRelays(string relayId, string storeId, Boolean isActive, int? weight)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into StoresToRelays (relayID, storeID, isActive, weight) " +
                "values ('" + relayId + "', '" + storeId + "', '" + isActive + "', '" + weight + "') ";
            cmd.ExecuteScalar();
            connect.Close();
        }

        public void saveStore(string storeId, string storeName, int? storeWeight, string relayId, string storeIP, int regionId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into Stores (storeId, storeName, storeWeight, relayId, storeIP, regionId) " +
                "values ('" + storeId + "','" + storeName + "', '" + storeWeight + "', '" + relayId + "', '" + storeIP + "', '" + regionId + "') ";
            cmd.ExecuteScalar();
            connect.Close();
        }

        public void saveRelay(string relayId, string relayName, string relayIP, int regionId, Boolean isActive, int relayQueue, Boolean isGateway)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into Relays (relayId, relayName, relayIP, regionId, isActive, relayQueue, isGateway) " +
                "values ('"+relayId+"','" + relayName + "', '" + relayIP + "', '" + regionId + "', '" + true + "', '" + relayQueue + "', '" + true + "') ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        public string getLastStoreId()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //get the number of rows in the Stores table:
            cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY storeID ASC) ,* FROM Stores) as t";
            //int totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            ////Select the last store ID:
            //cmd.CommandText = "select storesID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY storeID ASC) ,* FROM Stores) as t where rowNum = '" + totalRows + "' ";
            string storeId = cmd.ExecuteScalar().ToString();
            //check if id is the last id
            string tempId = checkIfIdIsTheLast(storeId, 3); //2 = Realy, 3 = Store.
            if (!tempId.Equals(storeId))//if they are different:
            {
                storeId = tempId;
            }
            connect.Close();
            return storeId;
        }
        public string getLastRelayId()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //get the number of rows in the Relays table:
            cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY relayID ASC) ,* FROM Relays) as t";            
            string relayId = cmd.ExecuteScalar().ToString();
            //check if id is the last id
            string tempId = checkIfIdIsTheLast(relayId, 2); //2 = Realy, 3 = Store.
            if (!tempId.Equals(relayId))//if they are different:
            {
                relayId = tempId;
            }
            connect.Close();
            return relayId;
        }
        public string checkIfIdIsTheLast(string id, int type)
        {
            string strLastId = "";
            int tempId = 0;
            int total = Convert.ToInt32(id);
            SqlCommand cmd = connect.CreateCommand();
            for (int i = 1; i <= total; i++)
            {
                if (type == 2)//if it's a Relay
                {

                    cmd.CommandText = "select relayId from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY relayID ASC) ,* FROM Relays) as t where rowNum = '" + i + "' ";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    if (newId > tempId)
                        tempId = newId;

                }
                else if (type == 3)//if it's a Store
                {
                    cmd.CommandText = "select storeId from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY storeId ASC) ,* FROM Stores) as t where rowNum = '" + i + "' ";
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    if (newId > tempId)
                        tempId = newId;
                }
            }
            strLastId = tempId.ToString();
            return strLastId;
        }
        public int getLastRegionId()
        {
            connect.Open();            
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //get the number of rows in the Regions table:
            cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY regionID ASC) ,* FROM Regions) as t";
            int totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            //Select the last region ID:
            cmd.CommandText = "select regionID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY regionID ASC) ,* FROM Regions) as t where rowNum = '" + totalRows + "' ";
            int regionId = Convert.ToInt32(cmd.ExecuteScalar());            
            connect.Close();
            return regionId;
        }
        // GET: Regions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regions regions = db.Regions.Find(id);
            if (regions == null)
            {
                return HttpNotFound();
            }
            return View(regions);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "regionID,regionName,gatewayIP")] Regions regions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(regions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(regions);
        }

        // GET: Regions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Regions regions = db.Regions.Find(id);
            if (regions == null)
            {
                return HttpNotFound();
            }
            return View(regions);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Regions regions = db.Regions.Find(id);
            db.Regions.Remove(regions);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
