using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAPI
{
    public class SwaggerOperationNameFilter : IOperationFilter
    {
       
        public void Apply(Swashbuckle.AspNetCore.Swagger.Operation operation, OperationFilterContext context)
        {
            operation.OperationId = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
               .Union(context.MethodInfo.GetCustomAttributes(true))
               .OfType<SwaggerOperationAttribute>()
               .Select(a => a.OperationId)
               .FirstOrDefault();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SwaggerOperationAttribute : Attribute
    {
        public SwaggerOperationAttribute(string operationId)
        {
            OperationId = operationId;
        }

        public string OperationId { get; }
    }


}
