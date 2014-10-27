using Sullinger.ValidatableBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Provides a contract allowing an implementation of IValidatable to access and use an Attribute implementing validation rules.
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        /// Gets or sets the type of the validation message.
        /// </summary>
        Type ValidationMessageType { get; set; }

        /// <summary>
        /// Gets or sets the failure message.
        /// </summary>
        string FailureMessage { get; set; }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="property">The property to validate.</param>
        /// <param name="sender">The owner of the property.</param>
        /// <returns>
        /// Returns an an instance of the Type implementing IValidationmessage specified by the ValidationMessageType property if validation fails.
        /// If validation succeeds, null is returned.
        /// </returns>
        IValidationMessage Validate(PropertyInfo property, IValidatable sender);
    }
}
