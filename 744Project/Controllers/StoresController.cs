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

namespace _744Project.Controllers
{
    public class StoresController : Controller
    {
        private MasterModel db = new MasterModel();
        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);


        public void getAllIps()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();            
            cmd.CommandText = "select count(*) from Relays";
            int totalRelays = Convert.ToInt32(cmd.ExecuteScalar());            
            List<string> ip = new List<string>();            
            //int ip1Counter = 0;            
            for (int i = 1; i <= totalRelays; i++)
            {
                cmd.CommandText = "select relayId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays) as t where rowNum = '" + i + "'";
                string relayId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select relayIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays) as t where rowNum = '" + i + "'";
                string relayIp = cmd.ExecuteScalar().ToString();
                ip.Add(relayIp);
            }            
            ViewBag.ip = ip;
            connect.Close();
        }
        // GET: Stores
        public ActionResult Index()
        {
            var stores = db.Stores.Include(s => s.Region).Include(s => s.Relay);           
            return View(stores.ToList());            
        }

        // GET: Stores/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Stores/Create
        public ActionResult Create()
        {
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName");
            ViewBag.relayID = new SelectList(db.Relays, "relayID", "relayName");
            getAllIps();           
            return View();
        }
        public string getRelayIdFromIp(string relayIp)
        {
            string id = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select relayId from relays where relayip like '"+relayIp+"' ";
            id = cmd.ExecuteScalar().ToString();
            connect.Close();
            return id;
        }
        public Boolean checkDuplicateIP(string ip, out string errorMessage)
        {
            Boolean duplicate = false;
            errorMessage = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from ProcessCenters where processCenterIP like '" + ip + "' ";
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
            else if (string.IsNullOrWhiteSpace(ip.Substring(10)))//check if last digits of ip are empty spaces
            {
                errorMessage = "Input Error: The IP cannot end with empty spaces";
                correct = false;
            }
            else if (!ip.Substring(10).All(char.IsDigit))//check last chars of IP are digits
            {
                errorMessage = "Input Error: The IP must end with digits";
                correct = false;
            }
            else if (Convert.ToInt32(ip.Substring(10)) < 1 || Convert.ToInt32(ip.Substring(10)) > 254)//end of IP cannot be 0 nor 255 because they are reserved according to project description
            {
                errorMessage = "Input Error: The last digits of the IP must be anywhere from 1 to 254";
                correct = false;
            }
            return correct;
        }
        public string getLastStoreId()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //get the number of rows in the Relays table:
            cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY storeID ASC) ,* FROM Stores) as t";
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

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "storeID,storeIP,storeName,storeWeight,relayID,regionID")] Store store, string relayIp)
        {
            //if (ModelState.IsValid)
            //{
            Boolean thereIsAnError = false;
            string storeId = (Convert.ToInt32(getLastStoreId()) + 1).ToString();
            store.storeID = storeId;
            
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get relayID from relayIP:
            store.relayID = getRelayIdFromIp(relayIp);

            //check for store.storeIP input
            string store_duplicate_error = "";
            string store_input_error = "";
            Boolean store_correctIP = true;
            //check if store IP field is empty:
            if (string.IsNullOrEmpty(store.storeIP))//check for empty input
            {
                ModelState.AddModelError("storeIP", "The Store IP field is required");
                thereIsAnError = true;
            }
            else
            {
                //check store IP if it the correct format: 192.168.0.XXX; XXX: 0 -> 255                
                store_correctIP = checkForCorrectIP(store.storeIP, out store_input_error);
                if (!store_correctIP)
                {
                    ModelState.AddModelError("storeIP", store_input_error);
                    thereIsAnError = true;
                }
            }
            if (store_correctIP && !thereIsAnError)//this is to avoid input error with special characters that would crash the application
            {
                //check store IP if it matches another IP
                Boolean store_duplicate = checkDuplicateIP(store.storeIP, out store_duplicate_error);
                if (store_duplicate)
                {
                    ModelState.AddModelError("storeIP", store_duplicate_error);
                    thereIsAnError = true;
                }
            }
            //check if store.storeName == NULL
            if (string.IsNullOrEmpty(store.storeName))//check for empty input
            {
                ModelState.AddModelError("storeName", "The Store Name field is required");
                thereIsAnError = true;
            }
            //check if store.storeWeight == NULL
            if (string.IsNullOrWhiteSpace(store.storeWeight.ToString()))
            {
                ModelState.AddModelError("storeWeight", "The Store Weight field is required");
                thereIsAnError = true;
            }
            //check if store.storeWeight <1 || > 500
            else if (store.storeWeight < 1 || store.storeWeight > 500)
            {
                ModelState.AddModelError("storeWeight", "The Store Weight must be from 1 to 500");
                thereIsAnError = true;
            }

            store.regionID = getRelaysRegion(store.relayID);
            if(store.regionID == 0)
            {
                ModelState.AddModelError("relayID", "The selected Relay does not belong to a Region");
                thereIsAnError = true;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (thereIsAnError)
            {
                ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", store.regionID);
                ViewBag.relayID = new SelectList(db.Relays, "relayID", "relayName", store.relayID);
                getAllIps();
                return View(store);
            }


            db.Stores.Add(store);
            db.SaveChanges();            
            //add storeID & relayID to StoreToRelays table:
            saveStoresToRelays(store.relayID, storeId, true, store.storeWeight);
            //return RedirectToAction("Index");
            ViewBag.SuccessMessage = "The new store has been successfully added to the network!";
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", store.regionID);
            ViewBag.relayID = new SelectList(db.Relays, "relayID", "relayName", store.relayID);
            getAllIps();
            return View(store);            
            //}

            //ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", store.regionID);
            //ViewBag.relayID = new SelectList(db.Relays, "relayID", "relayName", store.relayID);
            //return View(store);
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


        public int getRelaysRegion(string relayId)
        {
            int regionId = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //make sure that the relayID exists in the Relays table:
            cmd.CommandText = "select count(*) from Relays where relayId like '"+relayId+"' ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if(count > 0)
            {
                cmd.CommandText = "select regionID from Relays where relayID like '"+relayId+"' ";
                regionId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            connect.Close();
            return regionId;
        }

        // GET: Stores/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", store.regionID);
            ViewBag.relayID = new SelectList(db.Relays, "relayID", "relayName", store.relayID);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "storeID,storeIP,storeName,storeWeight,relayID,regionID")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", store.regionID);
            ViewBag.relayID = new SelectList(db.Relays, "relayID", "relayName", store.relayID);
            return View(store);
        }

        // GET: Stores/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
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
