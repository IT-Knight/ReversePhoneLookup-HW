using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Responses;

namespace ReversePhoneLookup.Web.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private static Dictionary<StatusCode, string> CodeMessages = new Dictionary<StatusCode, string>()
        {
            { StatusCode.ServerError, "Internal error" },
            { StatusCode.InvalidPhoneNumber, "Invalid phone number" },
            { StatusCode.NoDataFound, "No data found" },
            { StatusCode.InvalidPersonName, "InvalidPersonName" }
        };

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApiException apiException)
            {
                ErrorResponse response = new ErrorResponse()
                {
                    Code = apiException.Code,
                    Message = CodeMessages[apiException.Code]
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int)(apiException.Code == StatusCode.ServerError ? HttpStatusCode.InternalServerError : HttpStatusCode.BadRequest)
                };
            }
        }
    }
}
