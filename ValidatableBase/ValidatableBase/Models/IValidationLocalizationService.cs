using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sullinger.ValidatableBase.Models
{
    public interface IValidationLocalizationService
    {
        string GetLocalizedMessage(string key);
    }
}
