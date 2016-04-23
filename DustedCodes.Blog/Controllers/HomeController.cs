using System.Web.Mvc;
using DustedCodes.Blog.ViewModels;

namespace DustedCodes.Blog.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IViewModelFactory _viewModelFactory;

        public HomeController(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public ActionResult About()
        {
            var viewModel = _viewModelFactory.CreateAboutViewModel();

            return View(viewModel);
        }
    }
}