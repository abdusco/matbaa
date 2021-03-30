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
                                        string Content);

        /// <summary>
        /// Encode file as base64
        /// </summary>
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