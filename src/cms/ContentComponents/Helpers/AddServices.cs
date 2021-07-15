using Ganss.XSS;
using Microsoft.Extensions.DependencyInjection;

namespace ContentComponents
{
    public static class ComponentServices
    {
        public static void AddContentComponentServices(this IServiceCollection selfServices)
        {
            selfServices.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(x =>
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                return sanitizer;
            });
        }
    }
}