using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContentComponents;

namespace cms.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("ServerAPI",
            client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
          .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
              .CreateClient("ServerAPI"));

           // builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
                options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:ApiIdentifier"]);
            });

            builder.Services.AddContentComponentServices();


            await builder.Build().RunAsync();
        }
    }
}
