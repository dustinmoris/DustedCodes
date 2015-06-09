using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DustedCodes.Blog.Data;
using DustedCodes.Blog.Services;
using DustedCodes.Blog.ViewModels;

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
            var totalCount = await _articleService.GetTotalPageCount(_pageSize);
            var articles = await _articleService.GetMostRecentAsync(page, _pageSize);

            var viewModel = _viewModelFactory.CreateIndexViewModel(articles, totalCount, page);

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
            var result = await _articleService.FindByTagAsync(tag);

            var articles = result as Article[] ?? result.ToArray();
            var viewModel = _viewModelFactory.CreateIndexViewModel(articles, articles.Count(), 1);

            return View("Index", viewModel);
        }

        public ActionResult ArticleRedirect(string id)
        {
            return RedirectToActionPermanent("Article", new { id });
        }
    }
}