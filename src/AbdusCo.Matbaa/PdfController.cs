using System;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.Matbaa.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbdusCo.Matbaa
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfGenerator _pdfGenerator;

        public PdfController(IPdfGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        /// <summary>
        /// Generate PDF from HTML with options
        /// </summary>
        /// <remarks>
        /// ## Usage
        /// You only need to specify the `html`. The rest is optional.
        /// ```
        /// <![CDATA[
        /// {
        ///   "html": "<h1>hello world</h1><img src=\"https://unsplash.it/400/200\" />"
        /// }
        /// ]]>
        /// ```
        ///
        /// ### Images and other assets
        /// If you need to add images, you can convert them to base64, and set source as [data URIs](https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/Data_URIs).
        /// 
        /// ```html
        /// <h1>hello world</h1>
        /// <img src="data:image/jpg,base64;..." />
        /// ```
        /// Or just use plain URLS `https://unsplash.it/200/100`.
        /// But keep in mind that if the asset is slow to load, the request could timeout.
        ///
        /// ### PDF Options
        /// - If you're using CSS to set the page size, remember to enable `preferCSSPageSize`. This is enabled by default.
        /// - If you don't want to display a header/footer, disable it with `displayHeaderFooter` option. This is disabled by default.
        /// 
        /// You can override all these defaults in `pdfOptions`.
        ///
        /// For more information about PDF options, [see Puppeteer docs](https://github.com/puppeteer/puppeteer/blob/main/docs/api.md#pagepdfoptions)
        /// </remarks>
        /// <param name="request"></param>
        /// <param name="suggestFilename">Suggest filename for download</param>
        /// <param name="cancellationToken"></param>
        [Produces(MediaTypeNames.Application.Pdf, MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 503)]
        [HttpPost]
        public async Task<ActionResult> GeneratePdf(PdfRequest request,
                                                    string suggestFilename = "generated.pdf",
                                                    CancellationToken cancellationToken = default)
        {
            try
            {
                var stream = await _pdfGenerator.GeneratePdfFromHtmlAsync(request, cancellationToken);

                return File(
                    stream,
                    MediaTypeNames.Application.Pdf,
                    enableRangeProcessing: true,
                    fileDownloadName: suggestFilename
                );
            }
            catch (Exception e)
            {
                return Problem(title: "Failed to generate PDF", detail: e.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}