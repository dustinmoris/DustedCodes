using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DustedCodes.Blog.ViewModels;
using DustedCodes.Core.Services;

namespace DustedCodes.Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly int _pageSize;

        public BlogController(IArticleService articleService, IViewModelFactory viewModelFactory, int pageSize)
        {
            _articleService = articleService;
            _viewModelFactory = viewModelFactory;
            _pageSize = pageSize;
        }

        public async Task<ActionResult> Index(int page)
        {
            var result = await _articleService.GetMostRecentAsync(_pageSize, page);
            var viewModel = _viewModelFactory.CreateIndexViewModel(result.Items, result.TotalPages, result.PageNumber);

            return View(viewModel);
        }

        public async Task<ActionResult> Article(string id)
        {
            var article = await _articleService.GetByIdAsync(id);

            if (article == null)
                return HttpNotFound();

            var viewModel = _viewModelFactory.CreateArticleViewModel(article);

            return View(viewModel);
        }

        public async Task<ActionResult> Tagged(string tag)
        {
            var articles = await _articleService.GetByTagAsync(tag);
            var viewModel = _viewModelFactory.CreateIndexViewModel(articles, 1, 1);

            return View("Index", viewModel);
        }

        public ActionResult ArticleRedirect(string id)
        {
            return RedirectToActionPermanent("Article", new { id });
        }

        public async Task<ActionResult> Archive()
        {
            var articles = await _articleService.GetAllAsync();
            var metadata = articles.Select(a => a.Metadata);
            var viewModel = _viewModelFactory.CreateArchiveViewModel(metadata);

            return View(viewModel);
        }
    }
}