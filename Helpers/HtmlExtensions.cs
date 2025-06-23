using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using KiCWeb.Models;

public static class HtmlHelpers
{
    public static IHtmlContent Accordion(this IHtmlHelper htmlHelper, IEnumerable<AccordionItem> items)
    {
        return htmlHelper.PartialAsync("Components/_Accordion", items).Result;
    }
}