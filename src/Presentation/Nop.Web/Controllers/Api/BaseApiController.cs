using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    // Base for JSON APIs in Admin area
    [Area(AreaNames.ADMIN)]
    [ApiController]
    [Route("api/admin/[controller]")]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    [ValidateVendor]
    public abstract class BaseAdminApiController : ControllerBase
    {
              
    }
}
