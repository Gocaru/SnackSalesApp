using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppSnacks.Validations
{
    public class Validator : IValidator
    {
        public Validator()
        {
            
        }

        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string NameEmptyErrorMsg = "Please enter your name.";
        private const string NameInvalidErrorMsg = "Please enter a valid name.";
        private const string EmailEmptyErrorMsg = "Please enter your email.";
        private const string EmailInvalidErrorMsg = "Please enter a valid email.";
        private const string PhoneEmptyErrorMsg = "Please enter your phone number.";
        private const string PhoneInvalidErrorMsg = "Please enter a valid phone number.";
        private const string PasswordEmptyErrorMsg = "Please enter your password.";
        private const string PasswordInvalidErrorMsg = "The password must be at least 8 characters long, including letters and numbers.";


        public Task<bool> Validate(string name, string email, string phone, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPhoneValid = ValidatePhone(phone);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = NameEmptyErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = NameInvalidErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmailEmptyErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^[\w\.\-]+@[\w\-]+(\.[a-zA-Z]{2,3})+$"))
            {
                EmailError = EmailInvalidErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                PhoneError = PhoneEmptyErrorMsg;
                return false;
            }

            if (phone.Length < 12)
            {
                PhoneError = PhoneInvalidErrorMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = PasswordEmptyErrorMsg;
                return false;
            }

            if (password.Length < 8 ||
                !Regex.IsMatch(password, @"[a-zA-Z]") ||
                !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = PasswordInvalidErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
