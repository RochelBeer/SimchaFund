using hmwk50.data;
using hmwk50.web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hmwk50.web.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Simchos; Integrated Security=true;";

        public IActionResult Index()
        {
            SimchosManager manager = new(connectionString);
            ViewModel viewModel = new()
            {
                Simchos = manager.GetSimchos(),
                ContributorTotal = manager.ContributorTotal()
            };
            foreach (Simcha simcha in viewModel.Simchos)
            {
                simcha.ContributorCount = manager.ContributorCount(simcha.Id);
                simcha.TotalContribution = manager.GetTotalContribution(simcha.Id);
            }
            if (TempData["NewSimcha"] != null)
            {
                viewModel.Message = (string)TempData["NewSimcha"];
            }
            if (TempData["UpdateContribution"] != null)
            {
                viewModel.Message = (string)TempData["UpdateContribution"];
            }
            return View(viewModel);
        }
        public IActionResult NewSimcha(Simcha simcha)
        {
            SimchosManager manager = new(connectionString);
            manager.Add(simcha);
            TempData["NewSimcha"] = "New Simcha Added Successfully!";
            return Redirect("/home/index");
        }
        public IActionResult Contributions(int id)
        {
            SimchosManager manager = new(connectionString);
            ViewModel viewModel = new()
            {
                Contributors = manager.GetContributors(),
                SimchaName = manager.GetSimchaName(id),
                SimchaId = id
            };
            int index = 0;
            foreach (Contributor contributor in viewModel.Contributors)
            {
                contributor.Balance = manager.GetBalance(contributor.Id);
                contributor.ContributionAmount = manager.GetContributionAmount(contributor.Id,id);
                
                contributor.Index = index;
                index++;
            }
            return View(viewModel);
        }
        public IActionResult UpdateContributions(List<Contribution> contributors, int simchaId)
        {
            SimchosManager manager = new(connectionString);
            manager.ClearContributions(simchaId);
          
            IEnumerable<Contribution> YesContributing = contributors.Where(c => c.Include);
            foreach(Contribution contribution in YesContributing)
            {
                contribution.SimchaId = simchaId;
            }
            manager.AddManyContributions(YesContributing.ToList());
            TempData["UpdateContribution"] = "Contributions Updated Successfully!";
            return Redirect("/home/index");
        }
        
            

    }
}