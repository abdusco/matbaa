using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AbdusCo.Matbaa
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IContentTypeProvider _contentTypeProvider;

        public FilesController(IContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        private const int MaxFileSize = 1_000_000;

        public record EncodedFileResult(string FileName,
                                        string MimeType,
                                        long FileSize,
                                        string ContentBase64)
        {
            /// <summary>
            /// Concatenate prefix with <see cref="ContentBase64"/> to produce a data URI that can be used in HTML.
            /// </summary>
            public string DataUriPrefix => $"data:{MimeType};base64,";
        }

        /// <summary>
        /// Encode a file as base64
        /// </summary>
        /// <remarks>
        /// Convert a file into base64 to include in it HTML as a data URI.
        /// This can be done in Javascript without using this endpoint.
        ///
        /// - [`atob`](https://developer.mozilla.org/en-US/docs/Web/API/WindowOrWorkerGlobalScope/atob#example) function for converting a string to base64.  
        /// - [`URL.createObjectURL`](https://developer.mozilla.org/en-US/docs/Web/API/URL/createObjectURL) creates a data URI
        /// </remarks>
        /// <param name="file"></param>
        /// <returns>Encoded file</returns>
        [HttpPost("base64")]
        public async Task<ActionResult<EncodedFileResult>> ConvertToBase64(
            IFormFile file)
        {
            if (file.Length > MaxFileSize)
            {
                return Problem("Files must be smaller than 1MB (100000B)", statusCode: 400);
            }

            if (!_contentTypeProvider.TryGetContentType(file.FileName, out var contentType))
            {
                contentType = file.ContentType;
            }

            var base64Encoded = await file.OpenReadStream().ReadAsBase64Async();
            return Ok(new EncodedFileResult(
                file.FileName,
                contentType,
                file.Length,
                base64Encoded
            ));
        }
    }

    internal static class StreamExtensions
    {
        public static async Task<string> ReadAsBase64Async(this Stream stream)
        {
            await using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
}