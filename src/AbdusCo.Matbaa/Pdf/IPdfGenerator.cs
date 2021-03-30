using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AbdusCo.Matbaa.Pdf
{
    public interface IPdfGenerator
    {
        public Task<Stream> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken = default);
    }

    class BrowserlessSettings
    {
        public const string Key = "Browserless";
        public string ApiBaseUrl { get; set; }
        public string Token { get; set; }
        public int TimeoutSeconds { get; set; } = 10;
    }

    internal static class BrowserlessServiceCollectionExtensions
    {
        public static IServiceCollection AddBrowserless(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<BrowserlessSettings>, ConfigureBrowserless>();
            services.AddHttpClient<IPdfGenerator, BrowserlessPdfGenerator>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<BrowserlessSettings>>().Value;

                if (options.Token != null)
                {
                    var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Token));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedToken);
                }

                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                client.BaseAddress = new Uri(options.ApiBaseUrl);
            });
            return services;
        }


        private class ConfigureBrowserless : IConfigureOptions<BrowserlessSettings>
        {
            private readonly IConfiguration _configuration;

            public ConfigureBrowserless(IConfiguration configuration) => _configuration = configuration;

            public void Configure(BrowserlessSettings options) =>
                _configuration.GetSection(BrowserlessSettings.Key).Bind(options);
        }
    }

    class BrowserlessPdfGenerator : IPdfGenerator
    {
        private readonly HttpClient _http;

        public BrowserlessPdfGenerator(HttpClient http)
        {
            _http = http;
        }

        private static string ReadEmbeddedScript()
        {
            const string templateName = "Pdf.pdf.js";
            var resourceName = $"{Assembly.GetExecutingAssembly().GetName().Name}.{templateName}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new ApplicationException("Cannot find embedded script");
            }

            var sr = new StreamReader(stream!);
            return sr.ReadToEnd();
        }

        public async Task<Stream> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken = default)
        {
            var res = await _http.PostAsJsonAsync("function", new
            {
                code = ReadEmbeddedScript(),
                context = new
                {
                    html = html,
                }
            }, cancellationToken);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStreamAsync(cancellationToken);
        }
    }
}