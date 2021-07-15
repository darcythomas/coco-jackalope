using Ganss.XSS;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace ContentComponents
{
    public class MarkdownModel : ComponentBase
    {
        [Inject] public IHtmlSanitizer HtmlSanitizer { get; set; }
        private string _content;


        [Parameter]
        public string Content
        {
           // get => _content;
            set
            {
                _content = value;
                HtmlContent = ConvertStringToMarkupString(_content);
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