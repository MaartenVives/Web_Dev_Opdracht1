using Microsoft.AspNetCore.Mvc;
using opdracht_1.Models;
using opdracht_1.Data;
using Microsoft.EntityFrameworkCore;

namespace opdracht_1.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ZooData _context;

        public TicketsController(ZooData context)
        {
            _context = context;
        }

        public IActionResult AddTickets()
        {
            // Haal de gebruikersnaam op uit de sessievariabele
            var username = HttpContext.Session.GetString("UserName");
            // Zoek de gebruiker op basis van de gebruikersnaam
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            // Controleer of de gebruiker niet null is (d.w.z. ingelogd)
            if (user == null)
            {
                // Stuur de gebruiker naar de LogError actie van HomeController
                return RedirectToAction("LogError", "Home");
            }

            // Controleer of de gebruiker de rol "Admin" heeft
            if (user.Role != RoleType.Admin) // Hier ga ik ervan uit dat RoleType.Admin de Admin-rol is
            {
                // Stuur de gebruiker naar de RechtenError actie van HomeController
                return RedirectToAction("RechtenError", "Home");
            }
            var tickets = _context.Tickets.ToList();
            return View(tickets);
        }
        public IActionResult RemoveTicket(string name)
        {
            var ticketToRemove = _context.Tickets.FirstOrDefault(t => t.Name == name);
            if (ticketToRemove != null)
            {
                _context.Tickets.Remove(ticketToRemove);
                _context.SaveChanges();
            }
            return RedirectToAction("AddTickets");
        }

        [HttpPost]
        public IActionResult AddTickets(Tickets model)
        {

            if (ModelState.IsValid)
            {
                var ticket = new Tickets
                {
                    Name = model.Name,
                    Price = model.Price
                };

                _context.Tickets.Add(ticket);
                _context.SaveChanges();

                return RedirectToAction("AddTickets"); // Redirect to home page or any other page
            }

            return View(model);
        }
        // Actie om het bestelformulier weer te geven
        public IActionResult OrderTicket()
        {
            // Haal de gebruikersnaam op uit de sessievariabele
            var username = HttpContext.Session.GetString("UserName");

            // Zoek de gebruiker op basis van de gebruikersnaam
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            // Controleer of de gebruiker niet null is (d.w.z. ingelogd)
            if (user == null)
            {
                // Stuur de gebruiker naar de LogError actie van HomeController
                return RedirectToAction("LogError", "Home");
            }
            var tickets = _context.Tickets.ToList();
            return View(tickets);
        }

        // Actie om de bestelling te verwerken
        [HttpPost]
        public IActionResult OrderTicket(Dictionary<int, int> quantities)
        {
            // Haal de gebruikersnaam op uit de sessievariabele
            var username = HttpContext.Session.GetString("UserName");

            // Zoek de gebruiker op basis van de gebruikersnaam
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            // Controleer of de gebruiker niet null is (d.w.z. ingelogd)
            if (user == null)
            {
                // Stuur de gebruiker naar de LogError actie van HomeController
                return RedirectToAction("LogError", "Home");
            }

            // Maak een nieuwe bestelling aan voor de gebruiker
            var order = new Order
            {
                OrderPlace = DateTime.Now,
                User = user,
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Voeg de bestelde tickets toe aan de bestelling
            foreach (var ticketId in quantities.Keys)
            {
                var quantity = quantities[ticketId];
                var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);

                if (ticket == null)
                {
                    // Ticket niet gevonden, doorgaan met volgende ticket
                    continue;
                }

                var orderDetail = new OrderDetails
                {
                    OrderId = order.Id,
                    Quantity = quantity,
                    Ticket = ticket,
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            // Doorsturen naar de bevestigingspagina
            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
        public IActionResult OrderDetails()
        {
            // Haal de gebruikersnaam op uit de sessievariabele
            var username = HttpContext.Session.GetString("UserName");
            // Zoek de gebruiker op basis van de gebruikersnaam
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            // Controleer of de gebruiker niet null is (d.w.z. ingelogd)
            if (user == null)
            {
                // Stuur de gebruiker naar de LogError actie van HomeController
                return RedirectToAction("LogError", "Home");
            }

            // Controleer of de gebruiker niet de rol "Admin" heeft
            if (user.Role != RoleType.Admin && user.Role != RoleType.Worker)
            {
                // Stuur de gebruiker naar de RechtenError actie van HomeController
                return RedirectToAction("RechtenError", "Home");
            }

            var orderDetails = _context.OrderDetails.ToList();
            foreach (var rdr in orderDetails)
            {
                rdr.Ticket = _context.Tickets.First(t => t.Id == rdr.TicketId);
            }
            var groupedOrderDetails = orderDetails.GroupBy(od => od.TicketId);

            List<OrderDetails> consolidatedOrderDetails = new List<OrderDetails>();

            foreach (var group in groupedOrderDetails)
            {
                OrderDetails consolidatedDetail = group.First(); // Neem het eerste detail als referentie
                consolidatedDetail.Quantity = group.Sum(od => od.Quantity); // Tel de hoeveelheden op
                consolidatedOrderDetails.Add(consolidatedDetail);
            }

            return View(consolidatedOrderDetails);
        }


        [HttpPost]
        public IActionResult RemoveOrder(int id)
        {
            var order = _context.OrderDetails.Find(id);
            if (order != null)
            {
                _context.OrderDetails.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction("OrderDetails");
        }
        public IActionResult IndiDetails()
        {
            // Haal de gebruikersnaam op uit de sessievariabele
            var username = HttpContext.Session.GetString("UserName");

            // Zoek de gebruiker op basis van de gebruikersnaam
            var user = _context.Users.Include(u => u.Orders)
                                       .ThenInclude(o => o.OrderDetails)
                                       .ThenInclude(od => od.Ticket)
                                       .FirstOrDefault(u => u.UserName == username);

            // Controleer of de gebruiker niet null is (d.w.z. ingelogd)
            if (user == null)
            {
                // Stuur de gebruiker naar de LogError actie van HomeController
                return RedirectToAction("LogError", "Home");
            }

            // Haal alle orderdetails op van de gebruiker
            var orderDetails = _context.OrderDetails.ToList();
            var orderslist = _context.Orders.ToList();

            List<Order> orderFromUserList = new List<Order>();
            foreach (var ordr in orderslist)
            {
                if (ordr.UserId == user.Id)
                {
                    ordr.User = _context.Users.First(u => u.Id == ordr.UserId);
                    orderFromUserList.Add(ordr);
                }
            }

            foreach (var ordrDtls in orderDetails)
            {
                foreach (var ordr in orderFromUserList)
                {
                    if (ordrDtls.OrderId == ordr.Id)
                    {
                        ordr.OrderDetails.Add(ordrDtls);
                    }
                }
            }

            return View(orderFromUserList);
        }

    }
}
