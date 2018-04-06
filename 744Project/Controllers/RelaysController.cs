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
    public class RelaysController : Controller
    {
        private MasterModel db = new MasterModel();
        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);

        // GET: Relays
        public ActionResult Index()
        {
            var relays = db.Relays.Include(r => r.Region);
            return View(relays.ToList());
        }

        // GET: Relays/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relay relay = db.Relays.Find(id);
            if (relay == null)
            {
                return HttpNotFound();
            }
            return View(relay);
        }

        // GET: Relays/Create
        public ActionResult Create()
        {
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName");
            return View();
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
        // POST: Relays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "relayID,relayName,relayIP,regionID,isGateway,relayQueue,isActive")] Relay relay, int? weight)
        {
            
            Boolean thereIsAnError = false;
            string relayId = (Convert.ToInt32(getLastRelayId()) + 1).ToString();
            relay.relayID = relayId;
            relay.isActive = true;
            relay.isGateway = false;
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //check for relay.relayIP input
            string relay_duplicate_error = "";
            string relay_input_error = "";
            Boolean relay_correctIP = true;
            //check if relay IP field is empty:
            if (string.IsNullOrEmpty(relay.relayIP))//check for empty input
            {
                ModelState.AddModelError("relayIP", "The relay IP field is required");
                thereIsAnError = true;
            }                
            else
            {
                //check relay IP if it the correct format: 192.168.0.XXX; XXX: 0 -> 255                
                relay_correctIP = checkForCorrectIP(relay.relayIP, out relay_input_error);
                if (!relay_correctIP)
                {
                    ModelState.AddModelError("relayIP", relay_input_error);
                    thereIsAnError = true;
                }
            }
            if (relay_correctIP && !thereIsAnError)//this is to avoid input error with special characters that would crash the application
            {
                //check relay IP if it matches another IP
                Boolean relay_duplicate = checkDuplicateIP(relay.relayIP, out relay_duplicate_error);
                if (relay_duplicate)
                {
                    ModelState.AddModelError("relayIP", relay_duplicate_error);
                    thereIsAnError = true;
                }
            }
            //check if relay.relayName == NULL
            if (string.IsNullOrEmpty(relay.relayName))//check for empty input
            {
                ModelState.AddModelError("relayName", "The Relay Name field is required");
                thereIsAnError = true;
            }
            //check if relay.relayQueue == NULL
            if (string.IsNullOrWhiteSpace(relay.relayQueue.ToString()))
            {
                ModelState.AddModelError("relayQueue", "The Relay Queue Limit field is required");
                thereIsAnError = true;
            }
            //check if relay.relayQueue <1 || > 500
            else if (relay.relayQueue < 1 || relay.relayQueue > 500)
            {
                ModelState.AddModelError("relayQueue", "The Relay Queue Limit must be from 1 to 500");
                thereIsAnError = true;
            }
            //check if weight == NULL
            if (string.IsNullOrWhiteSpace(weight.ToString()))
            {
                ModelState.AddModelError("weight", "The Weight field is required");
                thereIsAnError = true;
            }
            //check if weight < 1 || > 500
            else if (weight < 1 || weight > 500)
            {
                ModelState.AddModelError("weight", "The Weight must be from 1 to 500");
                thereIsAnError = true;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (thereIsAnError)
            {
                ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", relay.regionID);
                return View(relay);
            }
            db.Relays.Add(relay);
            db.SaveChanges();
            saveRelayToRelay(relay.relayID, relay.regionID, weight);
            return RedirectToAction("Index");            
        }
        public void saveRelayToRelay(string relayId, int regionId, int? weight)
        {
            string relay_relayID = relayId;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //find gateway for the selected region:
            cmd.CommandText = "select gatewayIP from regions where regionId = '"+regionId+"' ";
            string gatewayIp = cmd.ExecuteScalar().ToString();
            //find the relayID for the gateway:
            cmd.CommandText = "select relayID from relays where relayIP like '"+gatewayIp+"' ";
            string relay2_relayID = cmd.ExecuteScalar().ToString();
            //insert into table RelayToRelays:
            cmd.CommandText = "insert into RelayToRelayConnections(relayWeight, relay_relayID, relay2_relayID, isActive) " +
                "values ('" + weight + "', '" + relay_relayID + "', '" + relay2_relayID + "', '" + true + "') ";
            cmd.ExecuteScalar();
            connect.Close();
        }

        // GET: Relays/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relay relay = db.Relays.Find(id);
            if (relay == null)
            {
                return HttpNotFound();
            }
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", relay.regionID);
            return View(relay);
        }

        // POST: Relays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "relayID,relayName,relayIP,regionID,isGateway,relayQueue,isActive")] Relay relay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relay).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.regionID = new SelectList(db.Regions, "regionID", "regionName", relay.regionID);
            return View(relay);
        }

        // GET: Relays/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relay relay = db.Relays.Find(id);
            if (relay == null)
            {
                return HttpNotFound();
            }
            return View(relay);
        }

        // POST: Relays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Relay relay = db.Relays.Find(id);
            db.Relays.Remove(relay);
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
