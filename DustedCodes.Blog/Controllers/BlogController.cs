using System.Threading.Tasks;
using System.Web.Mvc;
using DustedCodes.Blog.Services;
using DustedCodes.Blog.ViewModels;

namespace DustedCodes.Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IViewModelFactory _viewModelFactory;

        public BlogController(IArticleService articleService, IViewModelFactory viewModelFactory)
        {
            _articleService = articleService;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<ActionResult> Index()
        {
            var articles = await _articleService.GetMostRecentAsync(10);

            var viewModel = _viewModelFactory.CreateIndexViewModel(articles);

            return View(viewModel);
        }

        public async Task<ActionResult> Article(string id)
        {
            var article = await _articleService.FindByIdAsync(id);

            if (article == null)
                return HttpNotFound();

            var viewModel = _viewModelFactory.CreateArticleViewModel(article);

            return View(viewModel);
        }

        public async Task<ActionResult> Tagged(string tag)
        {
            var articles = await _articleService.FindByTagAsync(tag);

            var viewModel = _viewModelFactory.CreateIndexViewModel(articles);

            return View("Index", viewModel);
        }

        public ActionResult ArticleRedirect(string id)
        {
            return RedirectToActionPermanent("Article", new { id });
        }
    }
}