﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FamilyRegistration.Models
{
    public class Member
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public int FamilyId { get; set; }
        public String Gender { get; set; }
        public String MedicalInformation { get; set; }
        public DateTime BirthDate { get; set; }
        public String FamilyName { get; set; }
        public String Grade { get; set; }

        public Boolean AdultIsValid
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FirstName)) { return false; }
                if (String.IsNullOrWhiteSpace(LastName)) { return false; }
                if (String.IsNullOrWhiteSpace(Email)) { return false; }
                if (String.IsNullOrWhiteSpace(PhoneNumber)) { return false; }

                return true;
            }
        }

        public Boolean ChildIsValid
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FirstName)) { return false; }
                if (String.IsNullOrWhiteSpace(LastName)) { return false; }
                if (String.IsNullOrWhiteSpace(Gender)) { return false; }
                if (BirthDate == null || BirthDate == default(DateTime)) { return false; }

                return true;
            }
        }
    }
}