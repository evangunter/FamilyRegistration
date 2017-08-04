using Arena.NET.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyRegistration.Models
{
    public class AddChildrenModel
    {
        public Person Adult { get; set; }

        public List<Member> Children { get; set; }
    }
}