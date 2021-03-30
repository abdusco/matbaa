using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using AbdusCo.Matbaa.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbdusCo.Matbaa.Pdf
{
    public class PdfController : ApiController
    {
        private readonly IPdfGenerator _pdfGenerator;

        public PdfController(IPdfGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        public record PdfRequest(string Html);

        [Produces(MediaTypeNames.Application.Pdf)]
        [HttpPost]
        public async Task<ActionResult> GeneratePdf(PdfRequest request, CancellationToken cancellationToken)
        {
            var stream = await _pdfGenerator.GeneratePdfFromHtmlAsync(request.Html, cancellationToken);
            return File(stream, MediaTypeNames.Application.Pdf, enableRangeProcessing: true, fileDownloadName:"output.pdf");
        }
    }
}