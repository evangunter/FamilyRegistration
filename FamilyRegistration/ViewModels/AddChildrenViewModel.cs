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

        public List<SelectListItem> Grades
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Pre-K", Value = "Pre-K" },
                    new SelectListItem { Text = "K", Value = "Kindergarten" },
                    new SelectListItem { Text = "1st", Value = "1st" },
                    new SelectListItem { Text = "2nd", Value = "2nd" },
                    new SelectListItem { Text = "3rd", Value = "3rd" },
                    new SelectListItem { Text = "4th", Value = "4th" },
                    new SelectListItem { Text = "5th", Value = "5th" },
                    new SelectListItem { Text = "6th", Value = "6th" },
                    new SelectListItem { Text = "7th", Value = "7th" },
                    new SelectListItem { Text = "8th", Value = "8th" },
                    new SelectListItem { Text = "9th", Value = "9th" },
                    new SelectListItem { Text = "10th", Value = "10th" },
                    new SelectListItem { Text = "11th", Value = "11th" },
                    new SelectListItem { Text = "12th", Value = "12th" },
                    new SelectListItem { Text = "N/A", Value = "N/A" }
                };
            }
        }
    }
}