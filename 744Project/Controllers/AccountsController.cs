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
    public class AccountsController : Controller
    {
        private MasterModel db = new MasterModel();

        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);

        // GET: Accounts
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            //return View();
            //
            //var account = db.Accounts;
            //var creditCard = db.CreditCards;
            var vModel = new AccountsCreditsViewModel();
            //vModel.account = account;
            //vModel.creditCard = creditCard;            
            return View(vModel);
            //
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "accountID,accountNumber,accountBalance,accountName,accountFirstName,accountLastName,accountAddress,accountCity,accountState,accountZip,accountPhone,accountMax")] Account account,
            [Bind(Include = "cardID, cardNumber, cardExpirationDate, cardSecurityCode, cardMaxAllowed, accountID, firstName, lastName")] CreditCard creditCard)
        {
            if (ModelState.IsValid)
            {
                //db.CreditCards.Add(creditCard);                
                //db.Accounts.Add(account);
                //db.SaveChanges();

                db.Accounts.Add(account);                
                db.SaveChanges();
                saveCreditCard(creditCard.cardID, creditCard.cardNumber, creditCard.cardExpirationDate, creditCard.cardSecurityCode, creditCard.cardMaxAllowed, 
                    account.accountID, creditCard.firstName, creditCard.lastName);
                //The below is to to get the last created accountID and copy it to the account number
                //I could not think of a better way to represent the account number other than having it as the id itself:
                duplicateAccountNumberFromAccountId();
                return RedirectToAction("Index");
            }

            return View(account);
        }
        public void saveCreditCard(int cardID, long cardNumber, DateTime cardExpirationDate, string cardSecurityCode, int cardMaxAllowed, int accountID, string firstName, string lastName)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into CreditCards (cardNumber, cardExpirationDate, cardSecurityCode, cardMaxAllowed, accountID, firstName, lastName) " +
                "values ('"+cardNumber+"', '"+cardExpirationDate+"', '"+cardSecurityCode+"', '"+cardMaxAllowed+"', '"+accountID+"', '"+firstName+"', '"+lastName+"') ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        public void duplicateAccountNumberFromAccountId()
        {
            connect.Open();
            //Call a CreateCommand method from the SQLCommand class to use it for writing queries:
            SqlCommand cmd = connect.CreateCommand();
            //Define the type of SQL commands as text:
            cmd.CommandType = CommandType.Text;
            //get the number of rows in the Accounts table:
            cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY accountID ASC) ,* FROM Accounts) as t";
            int totalRows = Convert.ToInt32(cmd.ExecuteScalar());
            //Select the last account ID:
            cmd.CommandText = "select accountID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY accountID ASC) ,* FROM Accounts) as t where rowNum = '" + totalRows + "' ";
            int accountId = Convert.ToInt32(cmd.ExecuteScalar());
            //Now, update the accountNumber and set equal to accountID:
            cmd.CommandText = "update accounts set accountNumber = '" + accountId + "' where accountID = '" + accountId + "'  ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            //To get the accountID then use it when calling the POST method:
            if(id != null)
                editId = Convert.ToInt32(id);
            return View(account);
        }
        //To store the value of accountID:
        static int editId;


        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "accountID,accountNumber,accountBalance,accountName,accountFirstName,accountLastName,accountAddress,accountCity,accountState,accountZip,accountPhone,accountMax")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                //Change the accountNumber again to match the accountID:
                changeAccountNumberToAccountID(editId);
                return RedirectToAction("Index");
            }
            return View(account);
        }
        public void changeAccountNumberToAccountID(int editId)
        {
            connect.Open();            
            SqlCommand cmd = connect.CreateCommand();           
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "update accounts set accountNumber = '"+editId+"' where accountID = '"+editId+"' ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }


        public Boolean checkIfThereIsAtLeastOneCard(int id)
        {
            Boolean thereIs = false;
            //Open a new connection to the database:
            connect.Open();
            //Call a CreateCommand method from the SQLCommand class to use it for writing queries:
            SqlCommand cmd = connect.CreateCommand();
            //Define the type of SQL commands as text:
            cmd.CommandType = CommandType.Text;
            //Make sure that the transaction exists to avoid SQL exceptions:
            cmd.CommandText = "select count(*) from CreditCards where accountID = '" + id + "' ";
            //Store the result as an integer value:
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            //IDs are unique and there will be either one ID or none. If the ID exists, the result will be 1:
            if (count > 0)
                thereIs = true;
            //Close the connection to the database:
            connect.Close();
            return thereIs;
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
        }

        public void deleteCreditCards(int id)
        {
            //Open a new connection to the database:
            connect.Open();
            //Call a CreateCommand method from the SQLCommand class to use it for writing queries:
            SqlCommand cmd = connect.CreateCommand();
            //Define the type of SQL commands as text:
            cmd.CommandType = CommandType.Text;
            //count the related cards for the selected account:
            cmd.CommandText = "select count(*) from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY cardID ASC) ,* FROM CreditCards where accountID = '" + id + "') as t";
            int totalCards = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= totalCards; i++)
            {
                //select the cardID for the selected account:
                cmd.CommandText = "select cardID from (SELECT rowNum = ROW_NUMBER() OVER (ORDER BY cardID ASC) ,* FROM CreditCards where accountID = '" + id + "') as t";
                int cardID = Convert.ToInt32(cmd.ExecuteScalar());
                //check if the selected card has any transactions attached to it and delete them:
                checkAttachedTransactions(cardID);
                //Now, delete the card from the database:                
                cmd.CommandText = "delete from CreditCards where accountID = '" + id + "' ";
                cmd.ExecuteScalar();
            }
            //Close the connection to the database:
            connect.Close();
        }
        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            //Check if there is at least one credit card associated with that account:
            Boolean thereIs = checkIfThereIsAtLeastOneCard(id);
            //If there is no credit card for that account, delete it:
            if (!thereIs)
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
            }
            //If there is one or more credit cards associated with the selected account, delete them first then delete the account:
            else if (thereIs)
            {
                //delete all dredit cards associated with that account:
                deleteCreditCards(id);
                db.Accounts.Remove(account);
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
