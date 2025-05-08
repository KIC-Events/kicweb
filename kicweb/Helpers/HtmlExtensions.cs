using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlExtensions
{
    public class SectionHeadingOptions
    {
        public string Variant { get; set; } = "default"; // or "alt"
        public string Tag { get; set; } = "h2";
        public bool FullWidth { get; set; } = false;
    }

    public static IHtmlContent SectionHeading(this IHtmlHelper htmlHelper, string text, SectionHeadingOptions? options = null)
    {
        options ??= new SectionHeadingOptions();

        var variantClass = options.Variant == "alt" ? "section-heading--alt" : "section-heading";
        var widthClass = options.FullWidth ? "w-full" : "w-1/2";

        var tag = options.Tag;
        var fullClass = $"{variantClass} {widthClass}";

        var result = $"<{tag} class=\"{fullClass}\">{text}</{tag}>";

        return new HtmlString(result);
    }
}