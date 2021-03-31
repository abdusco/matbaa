using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.Matbaa.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

#nullable enable
namespace AbdusCo.Matbaa
{
    class PuppeteerSettings
    {
        public const string Key = "Puppeteer";
        public string? BrowserWsEndpoint { get; set; }
        public string? Token { get; set; }
        public int? TimeoutSeconds { get; set; } = 5;
    }

    internal static class PuppeteerServiceCollectionExtensions
    {
        public static IServiceCollection AddPuppeteer(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<PuppeteerSettings>, ConfigurePuppeteer>();
            services.AddTransient<IPdfGenerator, PuppeteerPdfGenerator>();
            return services;
        }

        private class ConfigurePuppeteer : IConfigureOptions<PuppeteerSettings>
        {
            private readonly IConfiguration _configuration;

            public ConfigurePuppeteer(IConfiguration configuration) => _configuration = configuration;

            public void Configure(PuppeteerSettings options) =>
                _configuration.GetSection(PuppeteerSettings.Key).Bind(options);
        }
    }


    class PuppeteerPdfGenerator : IPdfGenerator
    {
        private readonly PuppeteerSettings _settings;
        private readonly ILoggerFactory _loggerFactory;

        public PuppeteerPdfGenerator(IOptions<PuppeteerSettings> settings, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _settings = settings.Value;
        }

        public async Task<Stream> GeneratePdfFromHtmlAsync(PdfRequest request,
                                                           CancellationToken cancellationToken = default)
        {
            await using var browser = await ConnectBrowserAsync();
            await using var page = await browser.NewPageAsync();

            await SetupPageAsync(request, page);

            return await page.PdfStreamAsync(request.PdfOptions ?? PdfRequest.DefaultPdfOptions);
        }

        private Task<Browser> ConnectBrowserAsync()
        {
            var options = new ConnectOptions
            {
                BrowserWSEndpoint = _settings.Token != null
                    ? $"{_settings.BrowserWsEndpoint}?token={_settings.Token}"
                    : _settings.BrowserWsEndpoint,
            };

            return Puppeteer.ConnectAsync(options, _loggerFactory);
        }

        private Task<Browser> LaunchBrowserAsync()
        {
            return Puppeteer.LaunchAsync(new LaunchOptions
            {
                Product = Product.Chrome,
                Headless = true
            }, _loggerFactory);
        }

        private async Task SetupPageAsync(PdfRequest request, Page page)
        {
            if (request.Options?.EnableJavascript != null)
            {
                await page.SetJavaScriptEnabledAsync(request.Options.EnableJavascript);
            }

            await page.SetContentAsync(request.Html, new NavigationOptions
            {
                WaitUntil = new[] {WaitUntilNavigation.Networkidle0},
                Timeout = _settings.TimeoutSeconds * 1000,
            });

            if (request.Options?.AddScriptTags is {Count : > 0})
            {
                foreach (var tag in request.Options.AddScriptTags)
                {
                    await page.AddScriptTagAsync(tag);
                }
            }

            if (request.Options?.AddStyleTags is {Count : > 0})
            {
                foreach (var tag in request.Options.AddStyleTags)
                {
                    await page.AddStyleTagAsync(tag);
                }
            }
        }

        private static string InjectLoaderScript(string html)
        {
            const string script = "<script>window.onload = function() { window.IsPageLoaded= true; };</script>";
            if (html.Contains("</body>"))
                return html.Replace("</body>",
                    $"{script}</body>");
            return html + script;
        }
    }
}