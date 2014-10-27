//-----------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SampleUniversalApp.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Sullinger.ValidatableBase.Models;

    /// <summary>
    /// Provides method extensions for Dictionary objects.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts a dictionary of validation messages from an IValidatableBase instance to an observable dictionary that can be bound
        /// to by the UI.
        /// </summary>
        /// <param name="messages">An existing validation dictionary that needs it's messages observable.</param>
        /// <returns>Returns a new Dictionary that validation messages placed within an ObservableCollection</returns>
        public static Dictionary<string, ObservableCollection<IValidationMessage>> ConvertValidationMessagesToObservable(this Dictionary<string, IEnumerable<IValidationMessage>> messages)
        {
            if (messages == null)
            {
                return null;
            }

            var observableCollection = new Dictionary<string, ObservableCollection<IValidationMessage>>();
            foreach (KeyValuePair<string, IEnumerable<IValidationMessage>> pair in messages)
            {
                observableCollection.Add(pair.Key, new ObservableCollection<IValidationMessage>(pair.Value));
            }

            return observableCollection;
        }
    }
}
