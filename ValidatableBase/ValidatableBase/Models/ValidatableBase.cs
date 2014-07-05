using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Scionwest.Validatable.Models
{
    /// <summary>
    /// An implementation of IValidatable and INotifyPropertyChanged for validation and property change tracking.
    /// </summary>
    public class ValidatableBase : IValidatable, INotifyPropertyChanged
    {
        /// <summary>
        /// The ValidationMessages backing field.
        /// </summary>
        private Dictionary<string, List<IValidationMessage>> validationMessages = new Dictionary<string, List<IValidationMessage>>();

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the validation messages.
        /// </summary>
        /// <value>
        /// The validation messages.
        /// </value>
        public Dictionary<string, List<IValidationMessage>> ValidationMessages
        {
            get
            {
                return this.validationMessages;
            }

            private set
            {
                this.validationMessages = value;
                this.OnPropertyChanged("ValidationMessages");
            }
        }

        /// <summary>
        /// Registers an objects properties its validation Messages are accessible for observers to access.
        /// </summary>
        /// <param name="propertyName">The name of the property you want to register.</param>
        public void RegisterProperty(string propertyName)
        {
            if (!this.ValidationMessages.ContainsKey(propertyName))
            {
                this.ValidationMessages[propertyName] = new List<IValidationMessage>();
            }
        }

        /// <summary>
        /// Determines whether the object has any validation message Type's matching T for the the specified property.
        /// </summary>
        /// <typeparam name="T">A Type implementing IValidationMessage</typeparam>
        /// <param name="property">The property this validation was performed against.</param>
        /// <returns></returns>
        public bool HasValidationMessageType<T>(string property = "") where T : IValidationMessage, new()
        {
            if (string.IsNullOrEmpty(property))
            {
                bool result = this.validationMessages.Values.Any(collection => collection.Any(msg => msg is T));
                return result;
            }

            return this.validationMessages.ContainsKey(property);
        }

        /// <summary>
        /// Adds the supplied validation message to the ValidationMessages collection.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property this validation was performed against.</param>
        public void AddValidationMessage(IValidationMessage message, string property = "")
        {   
            if (string.IsNullOrEmpty(property))
            {
                return;
            }

            // If the key does not exist, then we create one.
            if (!this.validationMessages.ContainsKey(property))
            {
                this.validationMessages[property] = new List<IValidationMessage>();
            }

            if (this.validationMessages[property].Any(msg => msg.Message.Equals(message.Message) || msg == message))
            {
                return;
            }

            this.validationMessages[property].Add(message);
        }

        /// <summary>
        /// Removes the validation message from the ValidationMessages collection.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property this validation was performed against.</param>
        public void RemoveValidationMessage(string message, string property = "")
        {
            if (string.IsNullOrEmpty(property))
            {
                return;
            }

            if (!this.validationMessages.ContainsKey(property))
            {
                return;
            }
            
            if (this.validationMessages[property].Any(msg => msg.Message.Equals(message)))
            {
                // Remove the error from the key's collection.
                this.validationMessages[property].Remove(
                    this.validationMessages[property].FirstOrDefault(msg => msg.Message.Equals(message)));
            }
        }

        /// <summary>
        /// Removes all of the validation messages associated to the supplied property from the ValidationMessages collection.
        /// </summary>
        /// <param name="property">The property this validation was performed against.</param>
        public void RemoveValidationMessages(string property = "") 
        {
            if (string.IsNullOrEmpty(property))
            {
                return;
            }

            if (!this.validationMessages.ContainsKey(property))
            {
                return;
            }

            this.validationMessages[property].Clear();
            this.validationMessages.Remove(property);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public virtual void Validate()
        {
            this.OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="validationDelegate">The validation delegate.</param>
        /// <param name="failureMessage">The failure message.</param>
        /// <param name="propertyName"></param>
        /// <returns>
        /// Returns a validation message if the validation failed. Otherwise, null is returned.
        /// </returns>
        public IValidationMessage ValidateProperty(Func<string, IValidationMessage> validationDelegate, string failureMessage, string propertyName = "")
        {
            IValidationMessage result = validationDelegate(failureMessage);
            if (result != null)
            {
                this.AddValidationMessage(result, propertyName);
            }
            else
            {
                this.RemoveValidationMessage(failureMessage, propertyName);
            }

            return result;
        }

        /// <summary>
        /// Called when the specified property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
