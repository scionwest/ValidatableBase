using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Sullinger.ValidatableBase.Models
{
    public class ValidationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="messages">The messages.</param>
        public ValidationChangedEventArgs(string property, IEnumerable<IValidationMessage> messages)
        {
            this.ChangedProperty = property;
            this.ValidationMessages = messages;
        }

        /// <summary>
        /// Gets the changed property name.
        /// </summary>
        /// <value>
        /// The changed property.
        /// </value>
        public string ChangedProperty { get; private set; }

        /// <summary>
        /// Gets the validation messages.
        /// </summary>
        /// <value>
        /// The validation messages.
        /// </value>
        public IEnumerable<IValidationMessage> ValidationMessages { get; private set; }
    }
}
