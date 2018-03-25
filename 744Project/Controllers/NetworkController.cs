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

            foreach( var node in nodePosition)
            {
                //find the pc...
                if (node.id.Equals("192.168.0.1"))
                {
                    var x = "here";
                }

                NodePosition nodeConfiguration = db.NodePositions.Find(node.id);
                //if it doesn't exist, create a new instance
                if(nodeConfiguration == null)
                {
                    nodeConfiguration = new NodePosition()
                    {
                        Ip = node.id,
                        x = node.pos.x,
                        y = node.pos.y
                    };
                    if(node.category == 1)
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
                    if(node.category == 2)
                    {
                        var x = "is a pc";
                    }
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
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
            else if(numType == 1)//if its a relay
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
