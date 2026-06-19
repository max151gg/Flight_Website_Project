using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models
{
    // Base class for the data models (User, Flight, ...). It runs the validation rules
    // written as [Required], [EmailAddress] etc. on each property, and remembers any errors.
    // Call Validate() to check the whole object; IsValid is true when there are no errors.
    public abstract class Model : INotifyDataErrorInfo
    {
        // Stores the validation errors. Key = property name (e.g. "Email"),
        // Value = the list of error messages for that property.
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private object threadLock = new object();
        private bool isValid;
        public Model() { }
        public bool HasErrors
        {
            get { return errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0); }
        }

        public bool IsValid
        {
            get { return !this.HasErrors; }
            
        }

        public void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
                
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (errors.ContainsKey(propertyName) && (errors[propertyName] != null) && errors[propertyName].Count > 0)
                    return errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return errors.SelectMany(err => err.Value.ToList());
        }

        public Dictionary<string, List<string>> AllErrors()
        {
            return this.errors;
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (threadLock)
            {
                var validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);
               //clear previous errors from tested property
                if (errors.ContainsKey(propertyName))
                    errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        // Checks every property against its validation rules at once.
        // The website calls this before sign-up to make sure the form is filled in correctly.
        public void Validate()
        {
            this.isValid = true; //reset IsValid before validation
            lock (threadLock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                // Runs all the [Required]/[EmailAddress]/etc. rules and collects any failures.
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous errors
                var propNames = errors.Keys.ToList();
                errors.Clear();
                HandleValidationResults(validationResults);
                this.isValid = validationResults.Count == 0;
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            //Group validation results by property names
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;

            //add errors to dictionary and inform  binding engine about errors
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }
        }
    }


}
