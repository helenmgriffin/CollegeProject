using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeProject.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace CollegeProject.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        public ActionResult Index()
        {
            IList<Ticket> tickets = this.GetTickets();
            if(tickets.Count <= 0)
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");

            return View(tickets);
        }

        // GET: Tickets/Details
        public ActionResult Details(Guid id)
        {
            Ticket ticket = new Ticket();

            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return View(ticket);
            }
            ticket = this.GetTicket(id);

            if (ticket == null)
            {
                return View(ticket);
            }
            return View(ticket);
        }


        // GET: Tickets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.TicketGuid = Guid.NewGuid();
                try
                {
                    IList<Ticket> tickets = this.GetTickets();
                    ticket.TicketNumber = tickets.Max(t => t.TicketNumber) + 1;
                }
                catch
                {
                    ticket.TicketNumber = 1;
                }
                ticket.CreationDate = DateTime.Now;

                using (var client = new HttpClient())
                {
                    string url = _configuration["AWS:CreateEndpointUrl"];
                    //HTTP POST
                    var postTask = client.PutAsJsonAsync<Ticket>(url, ticket);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Ticket successfully created!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                    }
                }
            }
            return View(ticket);
        }

        // GET : Tickets/Close
        public ActionResult Close(Guid id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            Ticket ticket = this.GetTicket(id);
            if (ticket == null)
            {
                return View(ticket);
            }
            return View(ticket);
        }

        // POST: Tickets/Close/        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Close(Ticket ticket)
        {
            if (ModelState.IsValid)
            {               
                ticket.ClosedDate = DateTime.Now;

                if (this.UpdateTicket(ticket))
                {
                    TempData["Message"] = "Ticket successfully closed!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                }
            }
            return View(ticket);
        }
        // POST: Tickets/Reopen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reopen(Guid id)
        {
            if (ModelState.IsValid)
            {
                Ticket ticket = this.GetTicket(id);
                ticket.ClosedDate = null;
                ticket.ClosingComments = "";
                if (this.UpdateTicket(ticket))
                {
                    TempData["Message"] = "Ticket successfully re-opened!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        //[Authorize]
        public ActionResult Profile()
        {
            return View(HttpContext.User.Claims);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[Authorize(Roles = "Everyone")]
        //public IActionResult Everyone()
        //{
        //    return View();
        //}
        //[Authorize(Roles = "Admin")]
        //public IActionResult Admin()
        //{
        //    return View();
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IList<Ticket> GetTickets()
        {
            IList<Ticket> tickets = new List<Ticket>();

            using (var client = new HttpClient())
            {
                
                string url = _configuration["AWS:GetEndpointUrl"];
                //HTTP GET
                var responseTask = client.GetAsync(url);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Ticket>>();
                    readTask.Wait();

                    tickets = readTask.Result;
                }
                else //web api sent error response 
                {
                    tickets = new List<Ticket>();
                }
            }

            return tickets;
        }
        private Ticket GetTicket(Guid id)
        {
            Ticket ticket = new Ticket();

            using (var client = new HttpClient())
            {
                ticket.TicketGuid = id;

                string url = _configuration["AWS:GetByIDEndpointUrl"];

                //HTTP GET
                var responseTask = client.PostAsJsonAsync(url, ticket);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Ticket>>();
                    readTask.Wait();

                    IList<Ticket> tickets = readTask.Result;

                    if (tickets.Count > 0)
                        ticket = tickets[0];
                }
            }

            return ticket;
        }

        private bool UpdateTicket(Ticket ticket)
        {
            using (var client = new HttpClient())
            {
                
                string url = _configuration["AWS:UpdateEndpointUrl"];
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Ticket>(url, ticket);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
