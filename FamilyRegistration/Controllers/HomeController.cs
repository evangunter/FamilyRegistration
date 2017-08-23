using Arena.NET.Objects;
using Arena.NET.Repositories;
using FamilyRegistration.Helpers;
using FamilyRegistration.Models;
using FamilyRegistration.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static FamilyRegistration.Helpers.MemberHelper;

namespace FamilyRegistration.Controllers
{
    public class HomeController : Controller
    {
        [Route("~")]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("New")]
        public ActionResult New()
        {
            NewFamilyViewModel viewModel = new NewFamilyViewModel();

            NewFamilyModel model = new NewFamilyModel();
            model.NewFamilyMembers = new List<Member>();
            model.NewFamilyMembers.Add(new Member());

            //starting address
            Address address = new Address();
            address.State = "KY";
            model.Address = address;

            viewModel.States = GetStates();
            viewModel.Campuses = GetCampuses();
            viewModel.NewFamilyModel = model;

            return View(viewModel);
        }

        [HttpPost]
        [Route("New")]
        public async Task<ActionResult> New(NewFamilyModel NewFamilyModel)
        {
            //we need to validate the data server side
            if(NewFamilyModel.NewFamilyMembers.Any(x => !x.AdultIsValid))
            {
                ModelState.AddModelError("", "Every family member requires a valid first name, last name, email and phone number.");
            }

            //ensure at least 1 person is being added
            if(NewFamilyModel.NewFamilyMembers.Count == 0)
            {
                ModelState.AddModelError("", "At least 1 family member is required to register a family.");
            }

            //check the email addresses and phone numbers
            foreach(var member in NewFamilyModel.NewFamilyMembers)
            {
                Boolean isValid = true;
                if (!ValidationHelper.IsValidEmail(member.Email)) { ModelState.AddModelError("", String.Format("{0} is not a valid email address", member.Email)); isValid = false; }
                if (!ValidationHelper.IsValidPhone(ValidationHelper.CleanPhoneNumber(member.PhoneNumber))) { ModelState.AddModelError("", String.Format("{0} is not a valid phone number", member.PhoneNumber)); isValid = false; }

                if(isValid)
                {
                    //lets just verify this email doesn't already exist in the system, if it does, we should redirect
                    List<Person> personsFound = await ArenaAPIHelper.GetPersons(new PersonListOptions { Email = member.Email });
                    if(personsFound.Count > 0)
                    {
                        return RedirectToAction("Find", new { email = member.Email });
                    }
                }
            }

           
            

            if(!ModelState.IsValid)
            {
                NewFamilyViewModel viewModel = new NewFamilyViewModel();
                viewModel.Campuses = GetCampuses();
                viewModel.States = GetStates();
                viewModel.NewFamilyModel = NewFamilyModel;

                return View(viewModel);
            }

            //add the first person
            Person requiredAdult = MemberHelper.GetAdultPersonFromMember(NewFamilyModel.NewFamilyMembers.First(), (Campus) NewFamilyModel.CampusId);
            requiredAdult.Addresses = new List<Address> { NewFamilyModel.Address };

            ArenaPostResult result = await ArenaAPIHelper.AddPerson(requiredAdult);
            if(result.WasSuccessful)
            {
                //we need the familyId
                requiredAdult = await ArenaAPIHelper.GetPerson(result.ObjectId);

                for(int i = 1; i < NewFamilyModel.NewFamilyMembers.Count; i++)
                {
                    Person additonalAdult = MemberHelper.GetAdultPersonFromMember(NewFamilyModel.NewFamilyMembers[i], (Campus)NewFamilyModel.CampusId);
                    
                    additonalAdult.FamilyId = requiredAdult.FamilyId;
                    additonalAdult.Addresses = requiredAdult.Addresses;

                    result = await ArenaAPIHelper.AddPerson(additonalAdult);
                }

                return RedirectToAction("AddChildren", new { id = requiredAdult.PersonId });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> AddChildren(AddChildrenModel AddChildrenModel)
        {
            RegistrationCompleteViewModel viewModel = new RegistrationCompleteViewModel();
            viewModel.ChildrenAdded = AddChildrenModel.Children;
            viewModel.Adult = AddChildrenModel.Adult;

            if (AddChildrenModel.Children == null || AddChildrenModel.Children.Count == 0)
            {
                //family opted to not add any children
                return View("RegistrationComplete", viewModel);
            }

            //we need to validate the data server side
            if (AddChildrenModel.Children.Any(x => !x.ChildIsValid))
            {
                ModelState.AddModelError("", "Every child family member requires a valid first name, last name, birthdate and gender.");
            }

            if (!ModelState.IsValid)
            {
                AddChildrenViewModel addChildviewModel = new AddChildrenViewModel();
                addChildviewModel.AddChildrenModel.Adult = AddChildrenModel.Adult;
                addChildviewModel.AddChildrenModel = AddChildrenModel;
                return View(viewModel);
            }

            Person adult = await ArenaAPIHelper.GetPerson(AddChildrenModel.Adult.PersonId);
            viewModel.Adult = adult;

            //everything looks good, lets add the kids to the family
            foreach (var child in AddChildrenModel.Children)
            {
                Person familyChild = MemberHelper.GetChildPersonFromMember(child, (Campus) adult.CampusId);
                familyChild.Addresses = adult.Addresses;
                familyChild.FamilyId = adult.FamilyId;
                familyChild.FamilyName = adult.FamilyName;
                familyChild.Phones = adult.Phones;
                ArenaPostResult result = await ArenaAPIHelper.AddPerson(familyChild);

                if(!result.WasSuccessful)
                {
                    //stop processing and report error
                    return View("Error", new HandleErrorInfo(new Exception("API request to add child failed."), "Home", "AddChildren"));
                }

                //add to group
                ArenaPostResult groupResult = await ArenaAPIHelper.AddPersonToGroup(result.ObjectId, ((Campus)adult.CampusId == Campus.Brownsboro) ? (int)VisitorGroups.BrownsboroVisitors : (int)VisitorGroups.CliftonVisitors);
                if (!groupResult.WasSuccessful)
                {
                    //stop processing and report error
                    return View("Error", new HandleErrorInfo(new Exception("API request to add child to group failed."), "Home", "AddChildren"));
                }
            }

            return View("RegistrationComplete", viewModel);
        }


        public void SetCampusSession(int id)
        {
            Session["campusId"] = id;
        }

        [Route("{id}/Edit")]
        [HttpGet]
        public async Task<ActionResult> AddChildren(int id)
        {
            if (id == default(int)) { RedirectToAction("New"); }

            AddChildrenViewModel viewModel = new AddChildrenViewModel();
            viewModel.AddChildrenModel = new AddChildrenModel();

            Person person = await ArenaAPIHelper.GetPerson(id);
            viewModel.ExistingFamilyMembers = await ArenaAPIHelper.GetFamily(person.FamilyId);
            viewModel.AddChildrenModel.Adult =  person;
            viewModel.AddChildrenModel.Children = new List<Member>();
            viewModel.AddChildrenModel.Children.Add(new Member());

            return View(viewModel);
        }

        [Route("Find")]
        public ActionResult Find(String email)
        {
            SearchPersonModel viewModel = new SearchPersonModel();
            if (!String.IsNullOrWhiteSpace(email))
            {
                viewModel.Email = email;
                ViewData["fromRegister"] = true;
                return View(viewModel);
            }

            viewModel.Email = String.Empty;
            return View(viewModel);
        }

        public async Task<ActionResult> Search(SearchPersonModel model)
        {
            List<Person> persons = new List<Person>();

            if(!String.IsNullOrWhiteSpace(model.Email))
            {
                //search by email
                PersonListOptions options = new PersonListOptions() { Email = model.Email };
                persons = await ArenaAPIHelper.GetPersons(options);
            }
            else if (!String.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                //search by phone
                PersonListOptions options = new PersonListOptions() { PhoneNumber = ValidationHelper.CleanPhoneNumber(model.PhoneNumber) };
                persons = await ArenaAPIHelper.GetPersons(options);
            }

            return PartialView("_persons", persons);
        }

        private List<SelectListItem> GetStates()
        {
            //state options
            List<SelectListItem> states = new List<SelectListItem>();
            states.Add(new SelectListItem { Text = "KY", Value = "KY" });
            states.Add(new SelectListItem { Text = "IN", Value = "IN" });

            return states;
        }

        private List<SelectListItem> GetCampuses()
        {
            //state options
            List<SelectListItem> campuses = new List<SelectListItem>();
            campuses.Add(new SelectListItem { Text = "Brownsboro", Value = "1", Selected = true });
            campuses.Add(new SelectListItem { Text = "Clifton", Value = "2" });

            return campuses;
        }
    }
}