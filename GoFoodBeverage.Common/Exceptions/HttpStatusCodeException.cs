using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GoFoodBeverage.Common.Exceptions.ErrorModel;

namespace GoFoodBeverage.Common.Exceptions
{
    [Serializable]
    public class HttpStatusCodeException : Exception
    {
        public List<ErrorItemModel> Errors { get; set; }

        public virtual HttpStatusCode HttpStatusCode { get; }

        public HttpStatusCodeException()
        {
        }

        public HttpStatusCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpStatusCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HttpStatusCodeException(string message) : base(message)
        {
        }

        public HttpStatusCodeException(ErrorItemModel error)
        {
            Errors = new List<ErrorItemModel> { error };
        }

        public HttpStatusCodeException(List<ErrorItemModel> errors) : base(errors?.FirstOrDefault()?.Message)
        {
            Errors = errors;
        }
    }
}
