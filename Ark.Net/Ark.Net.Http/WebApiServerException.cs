using Ark;
using Ark.Net.Models;
using System;

// ReSharper disable VirtualMemberCallInConstructor

namespace Ark.Net.Http
{
    /// <summary>
    /// This exception is a real exception returned from the Web API with an Exception DTO to provide client with detailed error.
    /// </summary>
    public class WebApiServerException : Exception
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="WebApiServerException"/> instance.
        /// </summary>
        /// <param name="exception">The exception DTO coming from the web API server.</param>
        public WebApiServerException(ExceptionDto exception)
        {
            exception.Data.Keys.ForEach(key => Data.Add(key, exception.Data[key]));
            ExceptionType = exception.ExceptionType;
            HelpLink = exception.HelpLink;
            InnerException = exception.InnerException != null ? new WebApiServerException(exception.InnerException) : null;
            Message = exception.Message;
            Source = exception.Source;
            StackTrace = exception.StackTrace;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The type of the exception that occurs on the web API server.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// The inner exception if any.
        /// </summary>
        public new WebApiServerException InnerException { get; }

        /// <summary>
        /// The error message if any.
        /// </summary>
        public new string Message { get; }

        /// <summary>
        /// The exception stack trace.
        /// </summary>
        public new string StackTrace { get; }

        #endregion Properties (Public)
    }
}