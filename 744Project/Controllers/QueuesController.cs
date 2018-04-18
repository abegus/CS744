using _744Project.Models;
using _744Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _744Project.Controllers
{
    public class QueuesController : Controller
    {
        private MasterModel db = new MasterModel();
        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);

        // GET: Connections
        public ActionResult Index()
        {
            //There is no need to display, so this function wont be used.
            return View(db.Regions.ToList());
        }

        public void getAllIps()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();            
            cmd.CommandText = "select count(*) from Relays";
            int totalRelays = Convert.ToInt32(cmd.ExecuteScalar());            
            List<string> ip = new List<string>();            
            int ip1Counter = 0;                        
            for (int i = 1; i <= totalRelays; i++)
            {
                cmd.CommandText = "select relayId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays) as t where rowNum = '" + i + "'";
                string relayId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select relayIp from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY relayID ASC), *FROM Relays) as t where rowNum = '" + i + "'";
                string relayIp = cmd.ExecuteScalar().ToString();
                ip1Counter++;
                ip.Add(relayIp);
            }
            ViewBag.ip = ip;
            connect.Close();
        }


        // GET: Queues/Update
        public ActionResult Update()
        {
            //get all relay IPs 
            getAllIps();            
            return View();
        }             
        public Boolean IsGateway(string ip)
        {
            Boolean isGateway = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();            
            cmd.CommandText = "select isGateway from Relays where relayIP like '" + ip + "' ";
            int intIsGateway = Convert.ToInt32(cmd.ExecuteScalar());
            if (intIsGateway == 1)//true
                isGateway = true;            
            connect.Close();
            return isGateway;
        }


        // POST: Queues/Update
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string ip, int? queueLimit)
        {
            //START TEST
            string tempIp = "";
            tempIp = ip;
            //END TEST
            Boolean thereIsError = false;           
            //check if queueLimit == NULL
            if (string.IsNullOrWhiteSpace(queueLimit.ToString()) || queueLimit == null)
            {
                ModelState.AddModelError("queueLimit", "The Queue Limit field is required");
                thereIsError = true;
            }
            //check if store.storeWeight < 1 || > 500
            else if (queueLimit < 1 || queueLimit > 500)
            {
                ModelState.AddModelError("queueLimit", "The Queue Limit must be from 1 to 500");
                thereIsError = true;
            }

            if (thereIsError)
            {
                //get all Relay IPs
                getAllIps();                
                return View();
            }
            //get all Relay IPs
            getAllIps();
            changeQueueLimit(ip, queueLimit);
            ViewBag.SuccessMessage = "Relay: (" + ip + ") has new queue limit: (" + queueLimit+")";
            return View();

        }        
        public string getId(string ip)
        {
            string id = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select relayID from relays where relayip like '" + ip + "' ";
            id = cmd.ExecuteScalar().ToString();
            connect.Close();
            return id;
        }        
        public void changeQueueLimit(string ip, int? newLimit)
        {
            string id = getId(ip);

            //abes code to set inactive
            var relay = db.Relays.Find(id);
            relay.isActive = false;
            db.SaveChanges();
            //end of abes code to set inactive
            
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "update Relays set relayQueue = '" + newLimit + "' where relayId = '" + id + "' ";
            cmd.ExecuteScalar();                
            connect.Close();
        }
    }
}