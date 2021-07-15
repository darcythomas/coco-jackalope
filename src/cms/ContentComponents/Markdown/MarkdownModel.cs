using Ganss.XSS;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace ContentComponents
{
    public class MarkdownModel : ComponentBase
    {
        [Inject] public IHtmlSanitizer HtmlSanitizer { get; set; }
        
        [Parameter]
        public string Content
        {
            set
            {
                HtmlContent = ConvertStringToMarkupString(value);
            }
        }

        public MarkupString HtmlContent { get; private set; }

        private MarkupString ConvertStringToMarkupString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var html = Markdig.Markdown.ToHtml(value, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

                var sanitizedHtml = HtmlSanitizer.Sanitize(html);

                return new MarkupString(sanitizedHtml);
            }

            return new MarkupString();
        }
    }
}