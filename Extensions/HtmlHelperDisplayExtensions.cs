using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KiCWeb.Extensions
{
    /// <summary>
    /// Provides strongly-typed access to Display metadata such as Prompt and Description.
    /// </summary>
    public static class HtmlHelperDisplayExtensions
    {
        /// <summary>
        /// Retrieves the Display Prompt (used as a placeholder) for the given model expression.
        /// Returns HtmlString.Empty if no prompt is defined.
        /// </summary>
        public static IHtmlContent PromptFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string defaultPrompt = "")
        {
            var provider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider)) as IModelExpressionProvider;
            var modelExpression = provider.CreateModelExpression(htmlHelper.ViewData, expression);
            var prompt = modelExpression.Metadata.Placeholder;

            // Use provided default if no prompt is set
            if (string.IsNullOrWhiteSpace(prompt))
            {
                prompt = defaultPrompt;
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                return HtmlString.Empty;
            }

            return new HtmlString(prompt);
        }
        
        /// <summary>
        /// Retrieves the Display Description and wraps it in a div with class "field__description".
        /// Returns HtmlString.Empty if no description is defined.
        /// </summary>
        public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var provider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider)) as IModelExpressionProvider;
            var modelExpression = provider.CreateModelExpression(htmlHelper.ViewData, expression);
            var description = modelExpression.Metadata.Description;

            if (string.IsNullOrWhiteSpace(description))
            {
                return HtmlString.Empty;
            }

            var html = $"<div class=\"field__description\">{description}</div>";
            return new HtmlString(html);
        }

        /// <summary>
        /// Retrieves the raw Display Description text without any HTML wrapping.
        /// Returns HtmlString.Empty if no description is defined.
        /// </summary>
        public static IHtmlContent DescriptionTextFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var provider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider)) as IModelExpressionProvider;
            var modelExpression = provider.CreateModelExpression(htmlHelper.ViewData, expression);
            var description = modelExpression.Metadata.Description;

            if (string.IsNullOrWhiteSpace(description))
            {
                return HtmlString.Empty;
            }

            return new HtmlString(description);
        }
    }
}