using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceTracker.Core.Services;
using PriceTracker.Models;

namespace PriceTracker.Services
{
	public class RazorEmailTemplateService : IEmailTemplateService
	{
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;

		public RazorEmailTemplateService(
			IRazorViewEngine razorViewEngine,
			ITempDataProvider tempDataProvider,
			IServiceProvider serviceProvider)
		{
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
		}

		public async Task<string> RenderEmailConfirmationAsync(string callbackUrl)
		{
			var model = new EmailConfirmationViewModel { CallbackUrl = callbackUrl };
			return await RenderViewToStringAsync("EmailConfirmation", model);
		}

		public async Task<string> RenderPasswordResetAsync(string callbackUrl)
		{
			var model = new PasswordResetViewModel { CallbackUrl = callbackUrl };
			return await RenderViewToStringAsync("PasswordReset", model);
		}

		public async Task<string> RenderWelcomeAsync(string userName)
		{
			var model = new WelcomeViewModel { UserName = userName };
			return await RenderViewToStringAsync("Welcome", model);
		}

		private async Task<string> RenderViewToStringAsync<T>(string viewName, T model)
		{
			var actionContext = GetActionContext();
			var view = FindView(actionContext, viewName);

			await using var output = new StringWriter();
			var viewContext = new ViewContext(
				actionContext,
				view,
				new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = model
				},
				new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
				output,
				new HtmlHelperOptions()
			);

			await view.RenderAsync(viewContext);
			return output.ToString();
		}

		private ActionContext GetActionContext()
		{
			var httpContext = new DefaultHttpContext
			{
				RequestServices = _serviceProvider
			};

			return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
		}

		private IView FindView(ActionContext actionContext, string viewName)
		{
			var getViewResult = _razorViewEngine.GetView(
				executingFilePath: null,
				viewPath: $"~/Views/EmailTemplates/{viewName}.cshtml",
				isMainPage: false);

			if (getViewResult.Success)
				return getViewResult.View;

			var findViewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: false);
			if (findViewResult.Success)
				return findViewResult.View;

			var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
			var errorMessage = string.Join(
				Environment.NewLine,
				new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

			throw new InvalidOperationException(errorMessage);
		}
	}
}