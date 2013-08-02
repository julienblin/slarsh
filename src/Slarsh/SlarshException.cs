namespace Slarsh
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A <c>Slarsh</c> exception.
    /// </summary>
    [Serializable]
    public class SlarshException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlarshException"/> class.
        /// </summary>
        public SlarshException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlarshException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public SlarshException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlarshException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public SlarshException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlarshException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected SlarshException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
