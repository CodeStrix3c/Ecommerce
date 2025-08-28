using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Factories;

namespace Nop.Web.Controllers;

[ApiController]
[Route("api/vendor")]
public class VendorApiController : ControllerBase
{
    private readonly IVendorModelFactory _vendorModelFactory;
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    private readonly IVendorService _vendorService;
    private readonly VendorSettings _vendorSettings;

    public VendorApiController(
        IVendorModelFactory vendorModelFactory,
        ICustomerService customerService,
        IWorkContext workContext,
        IVendorService vendorService,
        VendorSettings vendorSettings)
    {
        _vendorModelFactory = vendorModelFactory;
        _customerService = customerService;
        _workContext = workContext;
        _vendorService = vendorService;
        _vendorSettings = vendorSettings;
    }

    // GET /api/vendor/apply
    [HttpGet("apply")]
    public async Task<IActionResult> ApplyVendor()
    {
        if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
            return Forbid(); // or return NotFound() if you prefer to hide capability

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Unauthorized();

        // same data as the MVC View model, but returned as JSON
        var model = await _vendorModelFactory.PrepareApplyVendorModelAsync(
            new Web.Models.Vendors.ApplyVendorModel(),
            // setDefaultValues: true,
            validateVendor: true,
            excludeProperties: false,

             null
            );

        return Ok(model);
    }

    // GET /api/vendor/info
    [HttpGet("info")]
    public async Task<IActionResult> Info()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Unauthorized();

        var vendor = await _workContext.GetCurrentVendorAsync();
        if (vendor == null || !_vendorSettings.AllowVendorsToEditInfo)
            return Forbid();

        var model = await _vendorModelFactory.PrepareVendorInfoModelAsync(
            new Web.Models.Vendors.VendorInfoModel(),
            excludeProperties: false);

        return Ok(model);
    }
}
