using System.Text.RegularExpressions;

namespace FigurineFrenzy.Controllers
{
    public interface IValidator
    {
        bool PhoneValidate(string phone);
        bool EmailValidate(string email);
    }
    public class Validator : IValidator
    {
        public bool EmailValidate(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern); ;
        }

        public bool PhoneValidate(string phone)
        {
            string pattern = @"^\d{10,11}$";
            return Regex.IsMatch(phone, pattern);
        }
    }
}
