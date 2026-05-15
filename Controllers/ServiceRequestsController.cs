using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EAPD7111wPOE_Part1.Data;
using EAPD7111wPOE_Part1.Models;
using EAPD7111wPOE_Part1.Models.ViewModels;
using System.Text.Json;
using System.Net.Http;

namespace EAPD7111wPOE_Part1.Controllers
{
    public class ServiceRequestsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceRequestsController(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: Service Request Page
        public async Task<IActionResult> Index()
        {
           
            try
            {
                // Test database connection
                bool canConnect = await _context.Database.CanConnectAsync();

                ViewBag.CanConnect = canConnect;

                // Load contracts
                var contracts = await _context.Contracts.ToListAsync();

                // Load requests
                var requests = await _context.ServiceRequests.ToListAsync();

                ViewBag.ContractCount = contracts.Count;
                ViewBag.RequestCount = requests.Count;

                // Populate dropdown
                /*ViewBag.ContractID = contracts
                .Select(c => new
                {
                    c.ContractID,
                    DisplayText = $"Contract {c.ContractID} - {c.Status}"
                })
                .ToList();*/
                var contractList = contracts.Select(c => new ContractDropdownViewModel
                {
                    ContractID = c.ContractID,
                    DisplayText = $"Contract ID: {c.ContractID} - {c.Status}"
                }).ToList();

                ViewBag.ContractList = contractList;

                return View(
                    "~/Views/Home/ServiceRequest.cshtml",
                    requests
                );
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();

                return View(
                    "~/Views/Home/ServiceRequest.cshtml",
                    new List<ServiceRequest>()
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            // REMOVE NAVIGATION VALIDATION ISSUE
            ModelState.Remove("Contract");

            // =========================
            // FIELD VALIDATION
            // =========================
            if (string.IsNullOrWhiteSpace(request.Description))
                ModelState.AddModelError("Description", "Description is required.");

            if (request.Cost <= 0)
                ModelState.AddModelError("Cost", "Cost must be greater than 0.");

            if (request.ContractID <= 0)
                ModelState.AddModelError("ContractID", "Please select a contract.");

            // =========================
            // LOAD CONTRACT
            // =========================
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractID == request.ContractID);

            if (contract == null)
            {
                ModelState.AddModelError("ContractID", "Contract not found.");
            }
            else if (contract.Status == "Expired" || contract.Status == "On Hold")
            {
                ModelState.AddModelError("ContractID",
                    "Cannot create Service Request. Contract is Expired or On Hold.");
            }

            // =========================
            // SAVE
            // =========================
            if (ModelState.IsValid)
            {
                // =====================================
                // DEFAULT TO ZAR
                // =====================================

                decimal finalZarAmount = request.Cost;

                // =====================================
                // CONVERT FOREIGN CURRENCY TO ZAR
                // =====================================

                if (!string.IsNullOrEmpty(request.Currency)
                    && request.Currency != "ZAR")
                {
                    try
                    {
                        var client =
                            _httpClientFactory.CreateClient();

                        string url =
                            $"https://open.er-api.com/v6/latest/{request.Currency}";

                        var response =
                            await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var json =
                                await response.Content
                                    .ReadAsStringAsync();

                            using JsonDocument doc =
                                JsonDocument.Parse(json);

                            var root = doc.RootElement;

                            decimal zarRate =
                                root
                                .GetProperty("rates")
                                .GetProperty("ZAR")
                                .GetDecimal();

                            finalZarAmount =
                                request.Cost * zarRate;
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("",
                            "Currency conversion failed.");
                    }
                }

                // =====================================
                // SAVE ONLY ZAR VALUE
                // =====================================

                request.Cost =
                    Math.Round(finalZarAmount, 2);

                request.Status = "Draft";

                _context.ServiceRequests.Add(request);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    $"Service Request created successfully. Saved Amount: R {request.Cost}";

                return RedirectToAction(nameof(Index));
            }

            // =========================
            // RELOAD DATA
            // =========================
            var contracts = await _context.Contracts.ToListAsync();

            ViewBag.ContractList = contracts
                .Select(c => new ContractDropdownViewModel
                {
                    ContractID = c.ContractID,
                    DisplayText = $"Contract {c.ContractID} - {c.Status}"
                }).ToList();

            var requests = await _context.ServiceRequests.ToListAsync();

            return View("~/Views/Home/ServiceRequest.cshtml", requests);
        }

        
    }
}
