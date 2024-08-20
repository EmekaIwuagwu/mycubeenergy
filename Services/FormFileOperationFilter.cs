using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CubeEnergy.Services
{
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody?.Content != null)
            {
                foreach (var content in operation.RequestBody.Content)
                {
                    if (content.Key == "multipart/form-data")
                    {
                        // Clear existing properties that might cause duplicate keys
                        content.Value.Schema.Properties.Clear();

                        // Add individual file properties
                        content.Value.Schema.Properties.Add("bankStatement", new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        });

                        content.Value.Schema.Properties.Add("passportDataPage", new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        });

                        content.Value.Schema.Properties.Add("nationalID", new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        });

                        // Add any other form fields
                        content.Value.Schema.Properties.Add("email", new OpenApiSchema
                        {
                            Type = "string"
                        });
                    }
                }
            }
        }
    }

}
