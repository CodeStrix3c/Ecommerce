using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

// NOTE: this is an API controller (JSON only)
//[Area("Admin")]

[ApiController]
[Route("api/admin/[controller]")]
public class CategoryApiController : BaseAdminApiController
{
    #region Fields
    private readonly ICategoryModelFactory _categoryModelFactory;
    private readonly ICategoryService _categoryService;
   
    #endregion

    #region Ctor
    public CategoryApiController(
        ICategoryModelFactory categoryModelFactory,
        ICategoryService categoryService
       )
    {
        _categoryModelFactory = categoryModelFactory;
        _categoryService = categoryService;
       
    }
    #endregion


    #region Api Controllers

    // GET: api/admin/categories/{id}
    [HttpGet("{id:int}")]
    // [CheckPermission(StandardPermission.Catalog.CATEGORIES_VIEW)]
    public async Task<IActionResult> Get(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null || category.Deleted)
            return NotFound();

        var model = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);
        return Ok(model);
    }

     #endregion

}
