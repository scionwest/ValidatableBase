using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SampleUniversalApp.Models;
using SampleUniversalApp.Extensions;
using Sullinger.ValidatableBase.Models;

namespace SampleUniversalApp.ViewModels
{
    /// <summary>
    /// The view model associated with the MainPage view.
    /// </summary>
    public class MainPageViewModel : ICommand, INotifyPropertyChanged
    {
        private string passwordConfirmation = string.Empty;

        private Dictionary<string, ObservableCollection<IValidationMessage>> validationMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.AppUser = new User();
            this.ValidationMessages = this.AppUser.GetValidationMessages().ConvertValidationMessagesToObservable();
        }

        /// <summary>
        /// Executed when the view model's ability to execute its command has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Executed when the view model has had a property value changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the User for the view model.
        /// </summary>
        public User AppUser { get; private set; }

        public string PasswordConfirmation
        {
            get
            {
                return this.passwordConfirmation;
            }

            set
            {
                this.passwordConfirmation = value;
                this.OnPropertyChanged("PasswordConfirmation");
            }
        }

        /// <summary>
        /// A wrapper around the non-observable collection of validation messages stored in our model.
        /// </summary>
        public Dictionary<string, ObservableCollection<IValidationMessage>> ValidationMessages
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
        /// This always returns true.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command for this view model
        /// </summary>
        /// <param name="parameter">The command does not require a parameter to be supplied.</param>
        public void Execute(object parameter)
        {
            this.AppUser.RemoveValidationMessages();
            // Perform validation on the user's built in validation.
            this.AppUser.ValidateAll();

            // Piggy-back on top of the user default validation with an additional level of validation in our view model.
            // We ensure the password and password confirmation matches.
            this.AppUser.ValidateProperty(
                () => PasswordConfirmation.Equals(this.AppUser.Password),
                new ValidationErrorMessage("Passwords do not match!"),
                    "Password");

            // Check if there are any errors.
            // We assign the reference to the view models, which contains an observable collection for the UI.
            if (this.AppUser.HasValidationMessages<ValidationErrorMessage>())
            {
                this.ValidationMessages = this.AppUser.GetValidationMessages().ConvertValidationMessagesToObservable();
                return;
            }

            // Do stuff.
            return;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(propertyName));
            this.OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;

            if (handler == null)
            {
                return;
            }

            handler(this, new EventArgs());
        }
    }
}
