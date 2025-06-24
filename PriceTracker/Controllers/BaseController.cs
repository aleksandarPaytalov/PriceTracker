using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PriceTracker.Controllers
{
	[Authorize]
	public class BaseController : Controller
	{
		protected bool IsUserAuthenticated => User?.Identity?.IsAuthenticated == true;
	}
}
