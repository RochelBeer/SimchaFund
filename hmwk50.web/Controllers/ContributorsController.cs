using hmwk50.data;
using hmwk50.web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace hmwk50.web.Controllers
{
    public class ContributorsController : Controller
    {
        private string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Simchos; Integrated Security=true;";

        public IActionResult Index()
        {
            SimchosManager manager = new(connectionString);
            ViewModel viewModel = new()
            {
                Contributors = manager.GetContributors(),
                GrandTotal = manager.GetGrandTotal()
            };
            foreach (Contributor contributor in viewModel.Contributors)
            {
                contributor.Balance = manager.GetBalance(contributor.Id);
            }
            if (TempData["DepositMessage"] != null)
            {
                viewModel.Message = (string)TempData["DepositMessage"];
            }
            if (TempData["NewContributor"] != null)
            {
                viewModel.Message = (string)TempData["NewContributor"];
            }
            if (TempData["UpdateContributor"] != null)
            {
                viewModel.Message = (string)TempData["UpdateContributor"];
            }
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult NewContributor(Contributor contributor, int initialDeposit, DateTime date)
        {
            SimchosManager manager = new(connectionString);
            int contributorId = manager.Add(contributor);
            Deposit initial = new()
            {
                ContributorId = contributorId,
                Amount = initialDeposit,
                Date = date
            };
            manager.Add(initial);
            TempData["NewContributor"] = "New Contributor Successfully Added!";
            return Redirect("/contributors/index");
        }
        public IActionResult EditContributor(Contributor contributor)
        {
            SimchosManager manager = new(connectionString);
            manager.EditContributor(contributor);
            TempData["UpdateContributor"] = "Contributor Updated Successfully!";
            return Redirect("/contributors/index");
        }
        public IActionResult Deposit(Deposit deposit)
        {
            SimchosManager manager = new(connectionString);
            manager.Add(deposit);
            TempData["DepositMessage"] = "Deposit Successfully Added!";
            return Redirect("/contributors/index");
        }
        public IActionResult History(int id)
        {
            SimchosManager manager = new(connectionString);
            ViewModel viewModel = new();
            viewModel.ContributorName = manager.GetContributorName(id);
            viewModel.Actions = manager.GetHistory(id);
            viewModel.Balance = manager.GetBalance(id);


            return View(viewModel);
        }
       
    }
}
