using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Popcorn.Service.Movie
{
    [Serializable]
    public class MovieServiceException : Exception
    {
        #region Properties

        #region Property -> Details

        /// <summary>
        /// Exception's details
        /// </summary>
        public readonly string Details;

        #endregion

        #region Property -> Status

        /// <summary>
        /// Status's details
        /// </summary>
        public readonly State Status;
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
        public MovieServiceException()
            : base()
        {
        }

        /// <summary>
        /// Create the exception with description
        /// </summary>
        /// <param name="message">Exception description</param>
        public MovieServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create the exception with description and inner cause
        /// </summary>
        /// <param name="message">Exception description</param>
        /// <param name="innerException">Exception inner cause</param>
        public MovieServiceException(string message, Exception innerException)
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
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected MovieServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Details = info.GetString("MovieServiceException.Details");
            Status = (State)info.GetValue("MovieServiceException.Status", typeof(State));
        }
        #endregion



        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("MovieServiceException.Details", Details);
            info.AddValue("MovieServiceException.Status", Status, typeof(State));
        }
    }
}