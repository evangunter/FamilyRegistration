using Arena.NET.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyRegistration.Models
{
    public class EditFamilyModel
    {
        public List<Person> ExistingFamilyMembers { get; set; }

        public List<Person> NewFamilyMembers { get; set; }

        public String FamilyName
        {
            get
            {
                if (!ExistingFamilyMembers.Any(x => x.FamilyMemberRole.ToLower() == "adult")) { return String.Empty; }

                return String.Format("{0} Family", ExistingFamilyMembers.First(x => x.FamilyMemberRole.ToLower() == "adult").LastName);
            }
        }
    }
}