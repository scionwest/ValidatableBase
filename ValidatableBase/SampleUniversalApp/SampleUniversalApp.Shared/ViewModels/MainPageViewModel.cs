using System;
using System.Windows.Input;
using SampleUniversalApp.Models;
using Scionwest.Validatable.Models;

namespace SampleUniversalApp.ViewModels
{
    /// <summary>
    /// The view model associated with the MainPage view.
    /// </summary>
    public class MainPageViewModel : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.AppUser = new User();
        }

        /// <summary>
        /// Executed when the view model's ability to execute its command has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets the User for the view model.
        /// </summary>
        public User AppUser { get; private set; }

        /// <summary>
        /// This always returns true.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            // TODO: Move validation checks in here at some point.
            return true;
        }

        /// <summary>
        /// Executes the command for this view model
        /// </summary>
        /// <param name="parameter">The command does not require a parameter to be supplied.</param>
        public void Execute(object parameter)
        {
            // Perform validation on the user.
            this.AppUser.Validate();

            // Check if there are any errors.
            if (this.AppUser.HasValidationMessageType<ValidationErrorMessage>())
            {
                return;
            }

            return;
        }
    }
}
