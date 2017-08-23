using Arena.NET.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyRegistration.Models
{
    public class NewFamilyModel
    {
        public List<Member> NewFamilyMembers { get; set; }
        public Address Address { get; set; }
        public int CampusId { get; set; }
    }
}