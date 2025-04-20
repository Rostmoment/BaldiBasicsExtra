using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Helpers
{
    class FirstTimeBBEException : Exception
    {
        public FirstTimeBBEException(string message) : base(message) { }
    }
    /// <summary>
    /// Represents an exception that is thrown when an invalid fun setting is encountered
    /// </summary>
    class InvalidFunSettingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFunSettingException"/> class
        /// </summary>
        public InvalidFunSettingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFunSettingException"/> class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public InvalidFunSettingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFunSettingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        public InvalidFunSettingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    class InvalidPositionException : Exception
    {
        public InvalidPositionException(string message) : base(message)
        {
        }
    }
}
