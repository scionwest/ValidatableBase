namespace Sullinger.ValidatableBase.Models
{
    /// <summary>
    /// An implementation of IMessage that can be used for warning messages
    /// </summary>
    public class ValidationWarningMessage : IValidationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationWarningMessage"/> class.
        /// </summary>
        public ValidationWarningMessage() : this(string.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationWarningMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationWarningMessage(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

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
