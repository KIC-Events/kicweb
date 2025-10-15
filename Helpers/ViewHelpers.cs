using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KiCWeb.Helpers;

// From https://weblog.west-wind.com/posts/2022/Jun/21/Back-to-Basics-Rendering-Razor-Views-to-String-in-ASPNET-Core
public static class ViewHelpers
{
    public static async Task<string> RenderViewToStringAsync(
        string viewName, object model,
        ControllerContext controllerContext,
        bool isPartial = false)
    {
        var actionContext = controllerContext as ActionContext;
    
        var serviceProvider = controllerContext.HttpContext.RequestServices;
        var razorViewEngine = serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
        var tempDataProvider = serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

        using (var sw = new StringWriter())
        {
            var viewResult = razorViewEngine.FindView(actionContext, viewName, !isPartial);

            if (viewResult?.View == null)
                throw new ArgumentException($"{viewName} does not match any available view");
        
            var viewDictionary =
                new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    { Model = model };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}