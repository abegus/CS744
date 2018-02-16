using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _744Project.Models;

namespace _744Project.Controllers
{
    public class RelayToRelayConnectionsController : Controller
    {
        private MasterModel db = new MasterModel();

        // GET: RelayToRelayConnections
        public ActionResult Index()
        {
            return View(db.RelayToRelayConnections.ToList());
        }

        // GET: RelayToRelayConnections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelayToRelayConnection relayToRelayConnection = db.RelayToRelayConnections.Find(id);
            if (relayToRelayConnection == null)
            {
                return HttpNotFound();
            }
            return View(relayToRelayConnection);
        }

        // GET: RelayToRelayConnections/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RelayToRelayConnections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "relayConnectionID,relayID,relayID2,relayWeight")] RelayToRelayConnection relayToRelayConnection)
        {
            if (ModelState.IsValid)
            {
                db.RelayToRelayConnections.Add(relayToRelayConnection);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(relayToRelayConnection);
        }

        // GET: RelayToRelayConnections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelayToRelayConnection relayToRelayConnection = db.RelayToRelayConnections.Find(id);
            if (relayToRelayConnection == null)
            {
                return HttpNotFound();
            }
            return View(relayToRelayConnection);
        }

        // POST: RelayToRelayConnections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "relayConnectionID,relayID,relayID2,relayWeight")] RelayToRelayConnection relayToRelayConnection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relayToRelayConnection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(relayToRelayConnection);
        }

        // GET: RelayToRelayConnections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelayToRelayConnection relayToRelayConnection = db.RelayToRelayConnections.Find(id);
            if (relayToRelayConnection == null)
            {
                return HttpNotFound();
            }
            return View(relayToRelayConnection);
        }

        // POST: RelayToRelayConnections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RelayToRelayConnection relayToRelayConnection = db.RelayToRelayConnections.Find(id);
            db.RelayToRelayConnections.Remove(relayToRelayConnection);
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
