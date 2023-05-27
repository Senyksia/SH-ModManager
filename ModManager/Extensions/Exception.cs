using System;

namespace ModManager.Extensions
{
    /// <summary>
    /// An exception that only represents a message.
    /// </summary>
    /// <inheritdoc />
    internal class MessageOnlyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOnlyException"/> class with a specified error message.
        /// </summary>
        /// <inheritdoc cref="Exception(string)"/>
        public MessageOnlyException(string message) : base(message) { }

        public override string ToString()
        {
            return Message;
        }
    }
}
