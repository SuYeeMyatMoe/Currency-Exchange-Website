using Currency_Exchange_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Currency_Exchange_Website.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(decimal amount, string fromCurrency, string toCurrency, string actionType)
        {
            ViewBag.SelectedCurrency = fromCurrency;
            ViewBag.DestinationCurrency= toCurrency;
            ViewBag.InputAmount = amount;

            if (amount <= 0 || string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
            {
                ViewData["Error"] = "All fields are required.";
                return View();
            }

            decimal convertedAmount = ConvertCurrency(amount, fromCurrency, toCurrency);
            ViewData["Result"] = $"{convertedAmount:F2} {toCurrency}";

            if (actionType == "reset")// Clear result with name="actionType" in reset button of the form
            {
                ViewData["Result"] = ""; 
                ViewBag.SelectedCurrency = "";
                ViewBag.DestinationCurrency = "";
                ViewBag.InputAmount = 0;
                return View();
            }
            return View();
        }

        // Reusing the logic from CurrencyConvertV2 for conversion
        private decimal ConvertCurrency(decimal inputAmount, string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency)
            {
                return inputAmount; // No conversion needed
            }

            switch (fromCurrency)
            {
                case "USD":
                    return toCurrency switch
                    {
                        "EUR" => inputAmount * 0.92m,
                        "SDG" => inputAmount * 4500m,
                        _ => 0
                    };

                case "EUR":
                    return toCurrency switch
                    {
                        "USD" => inputAmount * 1.09m,
                        "SDG" => inputAmount * 3000m,
                        _ => 0
                    };

                case "SDG":
                    return toCurrency switch
                    {
                        "USD" => inputAmount * 0.00022m,
                        "EUR" => inputAmount * 0.00033m,
                        _ => 0
                    };

                default:
                    return 0;
            }
        }



        [HttpGet]
        public IActionResult Privacy()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Privacy(string name, string email, string password, string confirmPassword, string gender, string dob, string city, string address)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewData["Error"] = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewData["Error"] = "Passwords do not match.";
                return View();
            }

            // Here you can add code to save user data to the database

            TempData["Msg"] = $"You are successfully registered with {email} address.";
            return RedirectToAction("Privacy");
        }


        [HttpGet]
        public IActionResult Tax()
        {
            ViewBag.result = "သင်အခွန်ပေးဆောင်ရန် မလိုသေးပါ။";
            ViewBag.annual = "";
            return View();
        }

        [HttpPost]
        public IActionResult Tax(decimal income, string parent1, string parent2, string marital_status, string children)
        {
            decimal deductions = 0;
            ViewBag.income = income;
            ViewBag.parent1 = parent1;  // Ensure parent1 and parent2 are passed correctly
            ViewBag.parent2 = parent2;

            // Apply deductions
            if (!string.IsNullOrEmpty(parent1)) deductions += 50000;
            if (!string.IsNullOrEmpty(parent2)) deductions += 50000;
            if (marital_status == "married") deductions += 30000;

            // Children deduction
            if (int.TryParse(children, out int childrenCount) && childrenCount > 0)
            {
                deductions += childrenCount * 30000;
            }

            // Ensure deductions do not exceed income
            decimal taxableIncome = Math.Max(0, income - deductions);

            // Tax threshold set to 300,000 MMK per month
            decimal taxThreshold = 300000M;

            // If taxable income is below or equal to 300,000, no tax is required
            if (taxableIncome <= taxThreshold)
            {
                ViewBag.result = "သင်အခွန်ပေးဆောင်ရန် မလိုသေးပါ။"; // No tax required
                ViewBag.annual = "";
                return View();
            }

            // Apply tax only to the amount exceeding 300,000
            decimal taxableAmount = taxableIncome - taxThreshold;
            decimal taxAmount = taxableAmount * 0.03M; // 3% tax

            // Update ViewBag messages
            ViewBag.result = $"သင်ပေးအပ်ရမယ့် အခွန်မှာ တစ်လလျှင် {taxAmount:N0} ကျပ်ဖြစ်ပြီး";
            ViewBag.annual = $"တစ်နှစ်လျှင် {taxAmount * 12:N0} ကျပ်ဖြစ်ပါသည်။";

            return View();
        }






    }
}
