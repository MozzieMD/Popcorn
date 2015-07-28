using System;
using System.Net;
using System.Runtime.Serialization;

namespace Popcorn.Service.Api
{
    [Serializable]
    public class ApiServiceException : Exception
    {
        #region Properties

        #region Property -> Details
        /// <summary>
        /// Exception's details
        /// </summary>
        public string Details { get; set; }
        #endregion

        #region Property -> Status
        /// <summary>
        /// Status's details
        /// </summary>
        public State Status { get; }
        #endregion

        #endregion

        public enum State
        {
            ConnectionError = 0
        }

        #region Constructors

        /// <summary>
        /// Just create the exception
        /// </summary>
        public ApiServiceException()
            : base()
        {
        }

        /// <summary>
        /// Create the exception with description
        /// </summary>
        /// <param name="message">Exception description</param>
        public ApiServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create the exception with description and inner cause
        /// </summary>
        /// <param name="message">Exception description</param>
        /// <param name="innerException">Exception inner cause</param>
        public ApiServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
            var e = innerException as WebException;

            if (e?.Status == WebExceptionStatus.NameResolutionFailure)
            {
                // There's a connection error.
                Details = "A connection error occured.";
                Status = State.ConnectionError;
            }
        }

        /// <summary>
        /// Create the exception from serialized data.
        /// Usual scenario is when exception is occured somewhere on the remote workstation
        /// and we have to re-create/re-throw the exception on the local machine
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected ApiServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}