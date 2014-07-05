namespace Scionwest.Validatable.Models
{
    /// <summary>
    /// Provides a contract for different message types to be stored during validation
    /// </summary>
    public interface IValidationMessage
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }
    }
}
