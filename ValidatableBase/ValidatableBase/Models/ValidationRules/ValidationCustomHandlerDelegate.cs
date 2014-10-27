using System;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Allows a method to receive a validation request by a property designated for validation by delegate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ValidationCustomHandlerDelegate : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the delegate.
        /// </summary>
        public string DelegateName { get; set; }
    }
}
