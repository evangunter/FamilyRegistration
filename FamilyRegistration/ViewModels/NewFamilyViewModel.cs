using FamilyRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FamilyRegistration.ViewModels
{
    public class NewFamilyViewModel
    {
        public NewFamilyModel NewFamilyModel { get; set; }
        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Campuses { get; set; }
    }
}