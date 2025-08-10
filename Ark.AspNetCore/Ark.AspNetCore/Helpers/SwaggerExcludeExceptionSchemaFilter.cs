using System;
using System.Linq;
using Ark.Net.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ark.AspNetCore
{
    /// <summary>
    /// This schema filter is used to remove the Exception properties from the model view.
    /// This has to be done because of a problem with Exception complex type in Swashbuckle RC5.0.2.
    /// </summary>
    public class SwaggerExcludeExceptionSchemaFilter : ISchemaFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;

            if (typeof(Exception).IsAssignableFrom(context.Type))
                return;

            if (!typeof(ResultDto).IsAssignableFrom(context.Type))
                return;
            context.Type.GetProperties()
                .Where(t => typeof(Exception).IsAssignableFrom(t.PropertyType))
                .ForEach(excludedProperty =>
                {
                    if (schema.Properties.ContainsKey(excludedProperty.Name))
                        schema.Properties.Remove(excludedProperty.Name);
                });
        }
    }
}