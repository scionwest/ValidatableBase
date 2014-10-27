//-----------------------------------------------------------------------
// <copyright file="MainPageViewModel.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SampleUniversalApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using SampleUniversalApp.Extensions;
    using SampleUniversalApp.Models;
    using Sullinger.ValidatableBase.Models;

    /// <summary>
    /// The view model associated with the MainPage view.
    /// </summary>
    public class MainPageViewModel : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// Backing field for the PasswordConfirmation property.
        /// </summary>
        private string passwordConfirmation = string.Empty;

        /// <summary>
        /// Backing field for the view models wrapper around the model validations.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the PasswordConfirmation that the user must enter. Must match the AppUser.Password property.
        /// </summary>
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
        /// Gets a wrapper around the non-observable collection of validation messages stored in our model.
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
        /// <param name="parameter">The command does not require a parameter to be supplied.</param>
        /// <returns>Always returns true</returns>
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
            // Perform validation on the user's built in validation.
            this.AppUser.ValidateAll();

            // Piggy-back on top of the user default validation with an additional level of validation in our view model.
            // We ensure the password and password confirmation matches.
            this.AppUser.ValidateProperty(
                () => this.PasswordConfirmation.Equals(this.AppUser.Password),
                new ValidationErrorMessage("Passwords do not match!"),
                    "Password");

            // Check if there are any errors.
            // We assign the reference to the view models, which contains an observable collection for the UI.
            if (this.AppUser.HasValidationMessages<ValidationErrorMessage>())
            {
                this.ValidationMessages = this.AppUser.GetValidationMessages().ConvertValidationMessagesToObservable();
                return;
            }
            else
            {
                this.ValidationMessages.AsParallel().ForAll(item => item.Value.Clear());
            }

            // Do stuff.
            return;
        }

        /// <summary>
        /// Notifies observers that a specific property has changed.
        /// </summary>
        /// <param name="propertyName">The property that has changed</param>
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

        /// <summary>
        /// Notifies the view that the command needs to be re-evaluated
        /// </summary>
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
