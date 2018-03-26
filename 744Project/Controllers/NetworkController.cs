using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _744Project.Models;
using _744Project.ViewModels;
using System.Web.Script.Serialization;

namespace _744Project.Controllers
{
    public class NetworkController : Controller
    {
        private MasterModel db = new MasterModel();

        // GET: Network
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                //return RedirectToAction("Index", "Home");
            }

            NetworkViewModel vm = new NetworkViewModel();

            //var serializer = new JavaScriptSerializer();
            //var temp = serializer.Serialize(vm);

            return View(vm);
        }

        /* This method takes a list of node positions from the graph and stores their locations in the database.
         * With this information stored, the locations can then be loaded when the page is loaded, */
        [HttpPost]
        public ActionResult NodePositions(IEnumerable<NodeLocation> nodePosition)
        {
            var temp = nodePosition;

            foreach (var node in nodePosition)
            {
                //find the pc...
                if (node.id.Equals("192.168.0.1"))
                {
                    var x = "here";
                }

                NodePosition nodeConfiguration = db.NodePositions.Find(node.id);
                //if it doesn't exist, create a new instance
                if (nodeConfiguration == null)
                {
                    nodeConfiguration = new NodePosition()
                    {
                        Ip = node.id,
                        x = node.pos.x,
                        y = node.pos.y
                    };
                    if (node.category == 1)
                    {
                        var relay = db.Relays.Find(node.id); // doesn't work, getting IP instead of ID
                        relay.isActive = node.isActive;
                    }
                    db.NodePositions.Add(nodeConfiguration);
                }
                //otherwise update current positions
                else
                {
                    nodeConfiguration.x = node.pos.x;
                    nodeConfiguration.y = node.pos.y;
                    if (node.category == 1)
                    {
                        var relay = db.Relays.Find(node.id); // doesn't work, getting IP instead of ID
                        relay.isActive = node.isActive;
                    }
                    if (node.category == 2)
                    {
                        var x = "is a pc";
                    }
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // GET: Network/GetConnection
        // This sends a source and a target IP for a connection. Returns a view with the connection information.
        public PartialViewResult GetConnection(string source, string target, string type1, string type2)
        {
            ConnectionViewModel vm = new ConnectionViewModel();

            //check if source is a relay, store, or PC, then add it  to the vm
            vm.sourceIp = source;
            vm.sourceType = Int32.Parse(type1);
            vm.targetIp = target;
            vm.targetType = Int32.Parse(type2);

            //relay source, pc target
            if (type1.Equals("1") && type2.Equals("2"))
            {
                var relay = (from re in db.Relays where re.relayIP.Equals(source) select re).First();
                var pc = (from p in db.ProcessCenters where p.processCenterIP.Equals(target) select p).First();
                vm.isActive = relay.RelayToProcessCenterConnections.Where(p => p.processCenterID == pc.processCenterID).First().isActive;
                //var con = db.RelayToProcessCenterConnections.Find(relay.relayID, pc.processCenterID);
                // vm.isActive = con.isActive;
            }
            //relay source, relay target;
            else if (type1.Equals("1") && type2.Equals("1"))
            {
                var rrs = from rr in db.RelayToRelayConnections select rr;
                foreach (var v in rrs)
                {
                    if (v.Relay.relayIP == source && v.Relay2.relayIP == target)
                    {
                        vm.isActive = v.isActive;
                    }
                }
                var relay1 = (from re in db.Relays where re.relayIP.Equals(source) select re).First();
                var relay2 = (from re in db.Relays where re.relayIP.Equals(target) select re).First();
                //var rrs = (from rr in db.RelayToRelayConnections where rr.relayID.Equals(relay1.relayID) && rr.relayID2.Equals(relay2.relayID) select rr).First();
                // vm.isActive = rrs.isActive;
                //vm.isActive = relay1.RelayToRelayConnections.Where(r => r.Relay2.relayID == relay2.relayID).First().isActive;
                // var con = db.RelayToRelayConnections.Find(relay1.relayID, relay2.relayID);
                //vm.isActive = con.isActive;
            }
            //store source, relay target
            else if (type1.Equals("0") && type2.Equals("1"))
            {
                var store = (from re in db.Stores where re.storeIP.Equals(source) select re).First();
                var relay2 = (from re in db.Relays where re.relayIP.Equals(target) select re).First();
                vm.isActive = relay2.StoresToRelays.Where(s => s.storeID == store.storeID).First().isActive;
                //vm.isActive = relay.RelayToProcessCenterConnections.Where(p => p.processCenterID == pc.processCenterID).First().isActive;
                //return null;
            }
            else
            {
                return null;
            }

            return PartialView(vm);
        }

        // GET: Network/GetNodeInformation
        // This sends a database ID and a type (0: Store, 1: Relay, 2: PC). Return a view with the results
        public PartialViewResult GetNodeInformation(string id, string type)
        {
            int numType = Int32.Parse(type);
            NetworkEntityViewModel vm;

            if (numType == 0)//if its a store
            {
                vm = new NetworkEntityViewModel(db.Stores.Find(id));
            }
            else if (numType == 1)//if its a relay
            {
                vm = new NetworkEntityViewModel(db.Relays.Find(id));
            }
            else//else its a PC
            {
                vm = new NetworkEntityViewModel(db.ProcessCenters.Find(id));
            }
            return PartialView(vm);
        }

        [HttpPost]
        public ActionResult SaveConInformation(ConnectionViewModel vm)
        {
            if (vm.sourceType == 1 && vm.targetType == 2)
            {
                var relay = (from re in db.Relays where re.relayIP.Equals(vm.sourceIp) select re).First();
                var pc = (from p in db.ProcessCenters where p.processCenterIP.Equals(vm.targetIp) select p).First();
                // vm.isActive = relay.RelayToProcessCenterConnections.Where(p => p.processCenterID == pc.processCenterID).First().isActive;
                var con = relay.RelayToProcessCenterConnections.Where(p => p.processCenterID == pc.processCenterID).First();
                con.isActive = !con.isActive;
            }
            //relay source, relay target;
            else if (vm.sourceType == 1 && vm.targetType == 1)
            {
                var rrs = from rr in db.RelayToRelayConnections select rr;
                foreach (var v in rrs)
                {
                    if (v.Relay.relayIP == vm.sourceIp && v.Relay2.relayIP == vm.targetIp)
                    {
                        v.isActive = !v.isActive;
                       // vm.isActive = v.isActive;
                    }
                }
                var relay1 = (from re in db.Relays where re.relayIP.Equals(vm.sourceIp) select re).First();
                var relay2 = (from re in db.Relays where re.relayIP.Equals(vm.targetIp) select re).First();
            }
            //store source, relay target
            else if (vm.sourceType == 0 && vm.targetType == 1)
            {
                var store = (from re in db.Stores where re.storeIP.Equals(vm.sourceIp) select re).First();
                var relay2 = (from re in db.Relays where re.relayIP.Equals(vm.targetIp) select re).First();
                // vm.isActive = relay2.StoresToRelays.Where(s => s.storeID == store.storeID).First().isActive;
                var con = relay2.StoresToRelays.Where(s => s.storeID == store.storeID).First();
                con.isActive = !con.isActive;
            }
            else
            {
                return null;
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SaveNoteInformation(NetworkEntityViewModel vm)
        {
            if (vm.type == 0)//if its a store
            {
                var store = db.Stores.Find(vm.id);
               // store.isActive()
            }
            else if (vm.type == 1)//if its a relay
            {
                var relay = db.Relays.Find(vm.id);
                relay.isActive = !relay.isActive;
            }
            else//else its a PC
            {
                var pc = db.ProcessCenters.Find(vm.id);
                //pc.isActive = !pc.isActive;
            }
            db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction("Index", "Home");
            //return ;
        }

        // GET: Network/Details/5
        public ActionResult Details(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

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
        

        // GET: Network/Create
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Network/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "relayID,relayName")] Relay relay)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                db.Relays.Add(relay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(relay);
        }

        // GET: Network/Edit/5
        public ActionResult Edit(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

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

        // POST: Network/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "relayID,relayName")] Relay relay)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                db.Entry(relay).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(relay);
        }

        // GET: Network/Delete/5
        public ActionResult Delete(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //test

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

        // POST: Network/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

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
