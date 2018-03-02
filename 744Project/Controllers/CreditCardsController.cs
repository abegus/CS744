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
    public class CreditCardsController : Controller
    {
        private MasterModel db = new MasterModel();

        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);

        // GET: CreditCards
        public ActionResult Index()
        {
            var creditCards = db.CreditCards.Include(c => c.Account);
            return View(creditCards.ToList());
        }

        // GET: CreditCards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            return View(creditCard);
        }

        // GET: CreditCards/Create
        public ActionResult Create()
        {
            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber");
            return View();
        }

        // POST: CreditCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cardID,cardNumber,cardExpirationDate,cardSecurityCode,cardMaxAllowed,accountID,firstName,lastName")] CreditCard creditCard)
        {
            if (ModelState.IsValid)
            {
                db.CreditCards.Add(creditCard);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber", creditCard.accountID);
            return View(creditCard);
        }

        // GET: CreditCards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber", creditCard.accountID);
            return View(creditCard);
        }

        // POST: CreditCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cardID,cardNumber,cardExpirationDate,cardSecurityCode,cardMaxAllowed,accountID,firstName,lastName")] CreditCard creditCard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(creditCard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber", creditCard.accountID);
            return View(creditCard);
        }

        // GET: CreditCards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreditCard creditCard = db.CreditCards.Find(id);
            if (creditCard == null)
            {
                return HttpNotFound();
            }
            return View(creditCard);
        }


        public void checkAttachedProcessTransactions(int id)
        {
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from ProcessCenterTransactions where transactionID = '" + id + "'  ";
            int totalTransactionsForCard = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalTransactionsForCard > 0)//if it's true, then there are transactions for that card and they need to be deleted as well.
            {
                //count the related cards for the selected account:
                cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY ProcessCenterTransactionID ASC) ,* FROM ProcessCenterTransactions where transactionID = '" + id + "') as t";
                int totalTransactions = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= totalTransactions; i++)
                {
                    //select the ProcessCenterTransactionID for the selected transaction:
                    cmd.CommandText = "select ProcessCenterTransactionID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY ProcessCenterTransactionID ASC) ,* FROM ProcessCenterTransactions where transactionID = '" + id + "') as t";
                    int ProcessCenterTransactionID = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = "delete from ProcessCenterTransactionID where transactionID = '" + id + "' ";
                    cmd.ExecuteScalar();
                }
            }
        }

        public void checkAttachedTransactions(int id)
        {
            //Open a new connection to the database:
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from transactions where cardID = '" + id + "'  ";
            int totalTransactionsForCard = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalTransactionsForCard > 0)//if it's true, then there are transactions for that card and they need to be deleted as well.
            {
                //count the related cards for the selected account:
                cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY transactionID ASC) ,* FROM transactions where cardID = '" + id + "') as t";
                int totalTransactions = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= totalTransactions; i++)
                {
                    //select the transactionID for the selected credit card:
                    cmd.CommandText = "select transactionID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY transactionID ASC) ,* FROM Transactions where cardID = '" + id + "') as t";
                    int transactionID = Convert.ToInt32(cmd.ExecuteScalar());
                    //check if there is a transaction in the process center:
                    checkAttachedProcessTransactions(transactionID);
                    cmd.CommandText = "delete from transactions where cardID = '" + id + "' ";
                    cmd.ExecuteScalar();
                }
            }
            //Close the connection to the database:
            connect.Close();
        }
        public Boolean lastCardForAccount(int cardId)
        {
            Boolean theLastCard = false;
            //Open a new connection to the database:
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //First, get the Account ID for the selected card:
            cmd.CommandText = "select accountID from creditcards where cardID = '" + cardId + "' ";
            string accountID = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select count(*) from creditcards where accountID = '" + accountID + "'  ";
            int totalCardsForTheaccount = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalCardsForTheaccount > 1)
                theLastCard = false;
            else
                theLastCard = true;
            //Close the connection to the database:
            connect.Close();
            return theLastCard;
        }

        // POST: CreditCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CreditCard creditCard = db.CreditCards.Find(id);
            //check if the credit card is the only card for its account:
            Boolean theLastCard = lastCardForAccount(id);
            if (!theLastCard) //if there is another card in its account, delete it.
            {
                //Check if there transactions associated with that credit card:
                checkAttachedTransactions(id);
                db.CreditCards.Remove(creditCard);
                db.SaveChanges();
            }
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
