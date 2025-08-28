using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Core.Rss;
using Nop.Services.Blogs;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Controllers;

[Route("api/admin/blogs")]
[ApiController]
public partial class BlogsApiController : BaseController
{
    #region Fields

    private readonly BlogSettings _blogSettings;
    private readonly IBlogModelFactory _blogModelFactory;
    private readonly IBlogService _blogService;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPermissionService _permissionService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public BlogsApiController(
        BlogSettings blogSettings,
        IBlogModelFactory blogModelFactory,
        IBlogService blogService,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWebHelper webHelper)
    {
        _blogSettings = blogSettings;
        _blogModelFactory = blogModelFactory;
        _blogService = blogService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _webHelper = webHelper;
    }

    #endregion

    #region GET Actions

    [HttpGet("list-blogs")]

    public virtual async Task<IActionResult> List(BlogPagingFilteringModel command)
    {
        if (!_blogSettings.Enabled)
            return  Forbid();

        var model = await _blogModelFactory.PrepareBlogPostListModelAsync(command);
       return Ok(model);
    }


    #endregion
}
