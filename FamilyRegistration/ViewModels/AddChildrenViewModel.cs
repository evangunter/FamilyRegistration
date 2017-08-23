using Arena.NET.Objects;
using FamilyRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FamilyRegistration.ViewModels
{
    public class AddChildrenViewModel
    {
        public AddChildrenModel AddChildrenModel { get; set; }
        public List<Person> ExistingFamilyMembers { get; set; }
        public List<SelectListItem> Genders
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" }
                };
            }
        }
    }
}