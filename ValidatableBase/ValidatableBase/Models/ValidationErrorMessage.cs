namespace Sullinger.ValidatableBase.Models
{
    /// <summary>
    /// An implementation of IMessage that can be used for error messages
    /// </summary>
    public class ValidationErrorMessage : IValidationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorMessage"/> class.
        /// </summary>
        public ValidationErrorMessage() : this (string.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationErrorMessage(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that contains Message for this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents the Message in this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Message;
        }
    }
}
