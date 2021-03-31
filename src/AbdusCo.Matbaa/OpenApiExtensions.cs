using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbdusCo.Matbaa
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var streamResponse = context.ApiDescription.SupportedResponseTypes
                .FirstOrDefault(x => x.Type == typeof(FileStreamResult));

            if (streamResponse != null)
            {
                // operation.Responses["200"].Schema = new Schema {Type = "file"};
            }
        }
    }
}