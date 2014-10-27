//-----------------------------------------------------------------------
// <copyright file="IValidatable.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sullinger.ValidatableBase.Models
{
    using System;
    using System.Collections.Generic;
    using Sullinger.ValidatableBase.Models.ValidationRules;

    /// <summary>
    /// Provides methods for creating a validatable concrete Type.
    /// </summary>
    public interface IValidatable
    {		
        /// <summary>
        /// Occurs when the instances validation state has changed.
        /// </summary>
        event EventHandler<ValidationChangedEventArgs> ValidationChanged;

        /// <summary>
        /// Adds a validation message to the instance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property.</param>
        void AddValidationMessage(IValidationMessage message, string property);

        /// <summary>
        /// Removes all of the instances validation messages.
        /// </summary>
        void RemoveValidationMessages();

        /// <summary>
        /// Removes all of the instances validation messages for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        void RemoveValidationMessages(string property);

        /// <summary>
        /// Removes the given validation message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property.</param>
        void RemoveValidationMessage(IValidationMessage message, string property);

        /// <summary>
        /// Determines whether this instance has validation messages for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Returns true if validation messages exist for the given property.</returns>
        bool HasValidationMessages(string property = "");

        /// <summary>
        /// Determines whether this instance has validation messages for the specified property, whose IMessage implementation matches the Type given.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="property">The property.</param>
        /// <returns>Returns true if validation messages exist for the given property.</returns>
        bool HasValidationMessages(Type messageType, string property = "");

        /// <summary>
        /// Determines whether this instance has validation messages for the specified property, whose IMessage implementation matches the Type given.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>Returns true if validation messages exist for the given property.</returns>
        bool HasValidationMessages<TMessage>(string property = "") where TMessage : IValidationMessage, new();

        /// <summary>
        /// Gets a key/value representation of this instances validation messages.
        /// </summary>
        /// <returns>Returns a key/value pair. The key represents the instances property names while the value holds a collection of validation messages for the property.</returns>
        Dictionary<string, IEnumerable<IValidationMessage>> GetValidationMessages();

        /// <summary>
        /// Gets the validation messages for the given property..
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Returns a collection of validation messages.</returns>
        IEnumerable<IValidationMessage> GetValidationMessages(string property);

        /// <summary>
        /// Performs validation on all of the instances properties that are eligible for validation.
        /// </summary>
        void ValidateAll();

        /// <summary>
        /// Validates the specified property if the property is decorated with an IValidationRule attribute.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void ValidateProperty(string propertyName = "");

        /// <summary>
        /// Validates the specified property via the supplied method delegate.
        /// </summary>
        /// <param name="validationDelegate">The validation delegate.</param>
        /// <param name="failureMessage">The failure message.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="validationProxy">The validation proxy.</param>
        /// <returns>Returns an IMessage if validation was not successful, otherwise null is returned to indicate success.</returns>
        IValidationMessage ValidateProperty(Func<bool> validationDelegate, IValidationMessage failureMessage, string propertyName, IValidatable validationProxy = null);

        /// <summary>
        /// Refreshes the validation.
        /// </summary>
        /// <param name="property">The property.</param>
        void RefreshValidation(string property);

        /// <summary>
        /// Performs the given validation rule for the specified property.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="property">The property.</param>
        /// <param name="validationProxy">
        /// A validation proxy can be specified. 
        /// When supplied, the validation rule and any errors associated with it will be stored within the proxy object instead of this instance.
        /// </param>
        void PerformValidation(IValidationRule rule, string property, IValidatable validationProxy = null);
    }
}
