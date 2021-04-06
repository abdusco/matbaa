using System.Collections.Generic;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace AbdusCo.Matbaa.Abstractions
{
    public class PdfRequest
    {
        public static readonly PdfOptions DefaultPdfOptions = new PdfOptions()
        {
            Format = PaperFormat.A4,
            DisplayHeaderFooter = false,
            PrintBackground = true,
            PreferCSSPageSize = true,
        };

        /// <summary>
        /// HTML to create PDF from
        /// </summary>
        public string Html { get; set; }

        public PageOptions? Options { get; set; }

        /// <summary>
        /// Puppeteer compatible PDF options. See [puppeteer documentation](https://github.com/puppeteer/puppeteer/blob/main/docs/api.md#pagepdfoptions)
        /// </summary>
        public PdfOptions? PdfOptions { get; set; }

        public class PageOptions
        {
            /// <summary>
            /// Enable Javascript evaluation on the page
            /// </summary>
            public bool EnableJavascript { get; set; } = true;

            /// <summary>
            /// Add script tags. See [puppeteer docs](https://github.com/puppeteer/puppeteer/blob/main/docs/api.md#frameaddscripttagoptions)
            /// You can also add script tags in HTML
            /// </summary>
            public List<AddTagOptions>? AddScriptTags { get; set; }

            /// <summary>
            /// Add style tags. See [puppeteer docs](https://github.com/puppeteer/puppeteer/blob/main/docs/api.md#frameaddstyletagoptions)
            /// You can also add style tags in HTML
            /// </summary>
            public List<AddTagOptions>? AddStyleTags { get; set; }
        }
    }
}