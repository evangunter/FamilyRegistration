using FamilyRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyRegistration.ViewModels
{
    public class RegistrationCompleteViewModel
    {
        public List<Member> ChildrenAdded { get; set; }
        public Boolean AddedChildren
        {
            get
            {
                if (ChildrenAdded == null || ChildrenAdded.Count == 0) { return false; }

                return true;
            }
        }
    }
}