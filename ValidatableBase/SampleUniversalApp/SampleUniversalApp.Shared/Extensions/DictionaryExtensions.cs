using Sullinger.ValidatableBase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SampleUniversalApp.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts a dictionary of validation messages from an IValidatableBase instance to an observable dictionary that can be bound
        /// to by the UI.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
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
