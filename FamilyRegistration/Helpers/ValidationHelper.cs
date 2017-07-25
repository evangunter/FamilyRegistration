using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FamilyRegistration.Helpers
{
    public static class ValidationHelper
    {
        public static Boolean IsValidEmail(String email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static Boolean IsValidPhone(String phoneNumber)
        {
            if(!phoneNumber.All(char.IsDigit)) { return false; }
            if(phoneNumber.Length != 10) { return false; }

            return true;
        }

        public static String CleanPhoneNumber(String phoneNumber)
        {
            return new String(phoneNumber.Where(Char.IsDigit).ToArray());
        }
    }
}