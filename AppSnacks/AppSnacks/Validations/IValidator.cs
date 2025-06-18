using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSnacks.Validations
{
    public interface IValidator
    {
        string NameError { get; set; }
        
        string EmailError { get; set; }

        string PhoneError { get; set; }

        string PasswordError { get; set; }

        Task<bool> Validate(string name, string email, string phone, string password);
    }
}
