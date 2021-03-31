using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.Matbaa.Abstractions
{
    public interface IPdfGenerator
    {
        public Task<Stream> GeneratePdfFromHtmlAsync(PdfRequest request, CancellationToken cancellationToken = default);
    }
}