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
    public class TransactionsController : Controller
    {
        private MasterModel db = new MasterModel();
        Encryption encryption = new Encryption();

        public ActionResult Encrypt(string id)
        {
            encryption.encryptAndStoreTransaction(id);
            return RedirectToAction("index", "Transactions");
        }

        public ActionResult Decrypt(string id)
        {
            encryption.decryptAndStoreTransaction(id);
            return RedirectToAction("index", "Transactions");
        }

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.CreditCard);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber");
            ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeIp");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "transactionID,transactionTime,transactionAmount,transactionType,transactionMerchant,transactionStatus,encryptedFlag,cardID,storeID")] Transaction transaction) // and the storeId
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                //to get he storeId, you have to send an additional string through the from on Transaction/Create. 
                StoreTransaction st = new StoreTransaction();
                //st.storeID = storeId;
                st.storeID = transaction.storeID;
                st.transactionID = transaction.transactionID;

                db.StoreTransactions.Add(st);
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber", transaction.cardID);
            ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeName", transaction.storeID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber", transaction.cardID);
            ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeName", transaction.storeID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "transactionID,transactionTime,transactionAmount,transactionType,transactionMerchant,transactionStatus,encryptedFlag,cardID,storeID")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber", transaction.cardID);
            ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeName", transaction.storeID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
