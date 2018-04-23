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
            //var transactions = db.Transactions.Include(t => t.CreditCard);
            var transactions = db.Transactions;
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

        [HttpPost]
        public ActionResult ProcessTransaction(TransactionViewModel trans)
        {
            // var transaction = db.Transactions.Find(trans.transId);
            Encryption cl = new Encryption();
            cl.decryptAndStoreTransaction(trans.transId + "");

            //now grab transaction from DB and check that it is valid.
            var transaction = db.Transactions.Find(trans.transId);
            var valid = true;
            //cant check if the card attached to the account matches one in the database, because it must in order
            //to be a valid transaction....

            //check if card exists in creditcards
            var creditCard = (from cd in db.CreditCards where cd.cardNumber == transaction.cardNumber select cd).FirstOrDefault();
            
            //if there is no such credit card with that number, the transaction is invalid.
            if(creditCard == null)
            {
                valid = false;
            }

            //check if the card is expired           
            else
            {
                if (creditCard.cardExpirationDate < transaction.transactionTime)
                {
                    valid = false;
                }

                //check if the merchant's name and Ip match in the database
                var store = (from st in db.Stores where st.storeIP == transaction.storeIP select st).FirstOrDefault();
                //no such IP
                if(store == null)
                {
                    valid = false;
                }
                //no matching store name IF the transactionMerchant is not "SELF"
                else if(transaction.transactionMerchant != "SELF" && transaction.transactionMerchant != "Self")
                {
                    if(transaction.transactionMerchant != store.storeName)
                        valid = false;
                }



                //check if the account has enough balance for the amount mentioned
                //first check debit, 
                //var account = transaction.CreditCard.Account;

                if(!valid)
                {
                    var account = creditCard.Account;
                    //no more need for checking Debit...
                    if (transaction.transactionType.Equals("Debit"))
                    {

                        account.accountBalance -= System.Convert.ToDecimal(transaction.transactionAmount);
                    }
                    if (transaction.transactionType.Equals("Credit"))
                    {
                        if (account.accountBalance + System.Convert.ToDecimal(transaction.transactionAmount) > account.accountMax)
                        {
                            valid = false;
                        }
                        else
                        {
                            account.accountBalance += System.Convert.ToDecimal(transaction.transactionAmount);
                        }
                    }
                    db.SaveChanges();
                }
                
            }
            
            

            if (valid)
            {
                return Json(new { success = true, responseText = "Succeeded transaction" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, responseText = "Failed transaction" }, JsonRequestBehavior.AllowGet);
            }
            //eturn RedirectToAction("Index", "Home");
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            //ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber");
            //ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeIp");
            return View();
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

        static string g_transactionType = "", g_transactionMerchant = "";
        static Boolean g_isSelf;
        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "transactionID,transactionTime,transactionAmount,transactionType, transactionMerchant,transactionStatus,encryptedFlag,cardNumber,storeIP, isSelf")] Transaction transaction) // and the storeId
        {

            //if (!string.IsNullOrWhiteSpace(g_transactionMerchant))
            //transaction.transactionMerchant = g_transactionMerchant;

            if (transaction.transactionType == "Debit")
            {
                g_isSelf = false;
                g_transactionType = "Debit";
                if (transaction.isSelf == true)
                {
                    g_isSelf = true;
                    g_transactionMerchant = "Self";
                }
                else
                {
                    g_isSelf = false;
                    g_transactionMerchant = transaction.transactionMerchant;
                }
            }
            else
            {
                g_transactionType = "Credit";
                g_isSelf = false;
                g_transactionMerchant = transaction.transactionMerchant;
                //transaction.transactionMerchant = (from store in db.Stores where store.storeID == transaction.storeID select store.storeName).FirstOrDefault();                
            }
            transaction.transactionMerchant = g_transactionMerchant;
            transaction.isSelf = g_isSelf;
            transaction.transactionType = g_transactionType;            
            
            if (ModelState.IsValid)
            {
                
                //to get he storeId, you have to send an additional string through the from on Transaction/Create. 
                //StoreTransaction st = new StoreTransaction();
                ////st.storeID = storeId;
                //st.storeID = transaction.storeID;
                //st.transactionID = transaction.transactionID;                
                string errorMessage = "";
                Boolean correctIP = checkForCorrectIP(transaction.storeIP, out errorMessage);
                if (!correctIP)
                {
                    ModelState.AddModelError("storeIP", errorMessage);
                    return View(transaction);
                }


                //db.StoreTransactions.Add(st);
                db.Transactions.Add(transaction);
                db.SaveChanges();
                encryption.encryptAndStoreTransaction(transaction.transactionID.ToString());
                g_transactionType = ""; g_transactionMerchant = ""; g_isSelf = false;
                return RedirectToAction("Index");
            }


            //ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber", transaction.cardID);
            //ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeIp", transaction.storeID);                        
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
            //ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber", transaction.cardID);
            //ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeIp", transaction.storeID);
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
                if (transaction.transactionType == "Debit")
                {
                    transaction.transactionMerchant = "Self";
                    
                }
                else
                {
                    //transaction.transactionMerchant = (from store in db.Stores where store.storeID == transaction.storeID select store.storeName).FirstOrDefault();
                    
                }
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.cardID = new SelectList(db.CreditCards, "cardID", "cardNumber", transaction.cardID);
            //ViewBag.storeID = new SelectList(db.Stores, "storeID", "storeIp", transaction.storeID);
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
            //Call a class CreditCardsController:
            CreditCardsController credit = new CreditCardsController();
            //check if there is a transaction in the ProcessCenterTransactions:
            credit.checkAttachedTransactionsForTransaction(id, "ProcessCenterTransactions", "processCenterTransactionID");
            //check if there is a transaction in StoreTransactions:
            credit.checkAttachedTransactionsForTransaction(id, "StoreTransactions", "storeTransactionID");
            //check if there is a transaction in RelayTransactions:
            credit.checkAttachedTransactionsForTransaction(id, "RelayTransactions", "relayTransactionID");
            //End
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
