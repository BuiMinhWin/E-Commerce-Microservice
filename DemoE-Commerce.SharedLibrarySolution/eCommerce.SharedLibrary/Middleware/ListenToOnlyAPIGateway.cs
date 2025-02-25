using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyAPIGateway(RequestDelegate next )
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Extract specific header from the request 
            var signedHeader = context.Request.Headers["Api-Gateway"];
            //null means, the request is not coming from the api gateway
            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
