using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HelloMVCCore;

[HtmlTargetElement("ajax-anchor", Attributes = LabelAttributeName)]
public class AjaxAnchorTagHelper(ILogger<AjaxAnchorTagHelper> logger) : TagHelper
{
    private readonly ILogger<AjaxAnchorTagHelper> _logger = logger;
    private const string LabelAttributeName = "label";

    [HtmlAttributeName("label")]
    public string? Label { get; set; }
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        logger.LogInformation("DI DI?");
        
        var childContent = output.Content.IsModified ? output.Content.GetContent() :
            (await output.GetChildContentAsync()).GetContent();
        output.TagMode = TagMode.StartTagAndEndTag;
        output.TagName = null;

        var tagBuilder = new TagBuilder("a");
        tagBuilder.Attributes.Add("href", "https://www.andreaskluth.net");

        var span = new TagBuilder("span");
        span.InnerHtml.Append(Label ?? "Please provide a label!");
        tagBuilder.InnerHtml.AppendHtml(span);
        tagBuilder.InnerHtml.AppendHtml(childContent);
        
        output.Content.AppendHtml(tagBuilder);
    }
}