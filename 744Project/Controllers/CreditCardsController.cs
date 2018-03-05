﻿using System;
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
                //Get a random card number from the table NewCardNumbers and store it in cardNumber of table CreditCards:
                getRandomCardNumber(creditCard.cardID);
                return RedirectToAction("Index");
            }

            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber", creditCard.accountID);
            return View(creditCard);
        }
        public void getRandomCardNumber(int cardID)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Count the number of rows in the table we are getting the data from:
            cmd.CommandText = "select count(*) from NewCardNumbers";
            int totalRandomNumbers = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalRandomNumbers > 0) //For now this is not needed, but it's a better practice to ensure that we have results from NewCardNumbers.
            {
                Random random = new Random();
                //get a random number from the total number of rows:
                int randomRow = random.Next(1, totalRandomNumbers);
                //select the cardNumber from the random number:
                cmd.CommandText = "select cardNumber from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY cardID ASC) ,* FROM NewCardNumbers) as t where rowNum = '"+randomRow+"' ";
                //Store it in a string. (Its actual value is long, but it doesn't matter since string will accept almost anything and when storing back to the other table there won't be an issue)
                string cardNumber = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "update CreditCards set cardNumber = '"+cardNumber+"' where cardID = '"+cardID+"' ";
                cmd.ExecuteScalar();
            }
            connect.Close();
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
            //To get the cardID then use it when calling the POST method:
            if (id != null)
            {
                cardID = Convert.ToInt32(id);
                cardNumber = creditCard.cardNumber;
            }

            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber", creditCard.accountID);
            return View(creditCard);
        }
        //To store the value of cardID and cardNumber:
        static int cardID;
        static long cardNumber;

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
                //Change the accountNumber again to match the accountID:
                changeCardNumberToPreviousCardNumber(cardID, cardNumber);
                return RedirectToAction("Index");
            }
            ViewBag.accountID = new SelectList(db.Accounts, "accountID", "accountNumber", creditCard.accountID);            
            return View(creditCard);
        }
        public void changeCardNumberToPreviousCardNumber(int oldCardId, long newCardNumber)// was int newCardNumber
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "update CreditCards set cardNumber = '" + newCardNumber + "' where cardID = '" + oldCardId + "' ";
            cmd.ExecuteScalar();
            connect.Close();
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
        public void checkAttachedTransactionsForTransaction(int id, string tableName, string tableId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from "+tableName+" where transactionID = '" + id + "'  ";
            int totalTransactionsForCard = Convert.ToInt32(cmd.ExecuteScalar());
            if (totalTransactionsForCard > 0)//if it's true, then there are transactions for that card and they need to be deleted as well.
            {
                //count the related cards for the selected account:
                cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY "+tableId+" ASC) ,* FROM " +tableName+" where transactionID = '" + id + "') as t";
                int totalTransactions = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= totalTransactions; i++)
                {
                    //select the ProcessCenterTransactionID for the selected transaction:
                    cmd.CommandText = "select "+tableId+" from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY "+tableId+" ASC) ,* FROM "+tableName+" where transactionID = '" + id + "') as t where rowNum = '"+i+"' ";
                    int attachedTransactionID = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = "delete from "+tableName+" where transactionID = '" + id + "' ";
                    cmd.ExecuteScalar();
                }
            }
            connect.Close();
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
                connect.Close();
                for (int i = 1; i <= totalTransactions; i++)
                {
                    //select the transactionID for the selected credit card:
                    cmd.CommandText = "select transactionID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY transactionID ASC) ,* FROM Transactions where cardID = '" + id + "') as t where rowNum = '"+i+"' ";
                    int transactionID = Convert.ToInt32(cmd.ExecuteScalar());
                    //check if there is a transaction in the ProcessCenterTransactions:                    
                    checkAttachedTransactionsForTransaction(transactionID, "ProcessCenterTransactions", "processCenterTransactionID");
                    //check if there is a transaction in StoreTransactions:
                    checkAttachedTransactionsForTransaction(transactionID, "StoreTransactions", "storeTransactionID");
                    //check if there is a transaction in RelayTransactions:
                    checkAttachedTransactionsForTransaction(transactionID, "RelayTransactions", "relayTransactionID");                    
                }
                connect.Open();
                //Now, delete the transaction from the Transactions table:
                cmd.CommandText = "delete from transactions where cardID = '" + id + "' ";
                cmd.ExecuteScalar();
                //Close the connection to the database:
                connect.Close();
            }
            
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
            else if (totalCardsForTheaccount == 1)
                theLastCard = true;
            else
                theLastCard = false;
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
                //Check if there are transactions associated with that credit card:
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
