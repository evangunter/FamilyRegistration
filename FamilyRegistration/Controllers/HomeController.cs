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
            viewModel.NewFamilyModel = model;

            return View(viewModel);
        }

        [HttpPost]
        [Route("New")]
        public async Task<ActionResult> New(NewFamilyModel NewFamilyModel)
        {
            //we need to validate the data server side
            if(NewFamilyModel.NewFamilyMembers.Any(x => !x.IsValid))
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
                if (!ValidationHelper.IsValidEmail(member.Email)) { ModelState.AddModelError("", String.Format("{0} is not a valid email address", member.Email)); }
                if (!ValidationHelper.IsValidPhone(ValidationHelper.CleanPhoneNumber(member.PhoneNumber))) { ModelState.AddModelError("", String.Format("{0} is not a valid phone number", member.PhoneNumber)); }
            }

            if(!ModelState.IsValid)
            {
                NewFamilyViewModel viewModel = new NewFamilyViewModel();
                viewModel.States = GetStates();
                viewModel.NewFamilyModel = NewFamilyModel;

                return View(viewModel);
            }

            //add the first person
            Person requiredAdult = MemberHelper.GetAdultPersonFromMember(NewFamilyModel.NewFamilyMembers.First());
            requiredAdult.Addresses = new List<Address> { NewFamilyModel.Address };

            ArenaPostResult result = await ArenaAPIHelper.AddPerson(requiredAdult);
            if(result.WasSuccessful)
            {
                //we need the familyId
                requiredAdult = await ArenaAPIHelper.GetPerson(result.ObjectId);

                for(int i = 1; i < NewFamilyModel.NewFamilyMembers.Count; i++)
                {
                    Person additonalAdult = MemberHelper.GetAdultPersonFromMember(NewFamilyModel.NewFamilyMembers[i]);
                    additonalAdult.FamilyId = requiredAdult.FamilyId;
                    additonalAdult.Addresses = requiredAdult.Addresses;

                    result = await ArenaAPIHelper.AddPerson(additonalAdult);
                }

                return RedirectToAction("AddChildren", new { id = requiredAdult.PersonId });
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> AddChildren(List<Member> NewChildFamilyMembers)
        {
            RegistrationCompleteViewModel viewModel = new RegistrationCompleteViewModel();

            if(NewChildFamilyMembers == null || NewChildFamilyMembers.Count == 0)
            {
                //family opted to not add any children
                return View("Success", viewModel);
            }

            //we need to validate the data server side
            if (NewChildFamilyMembers.Any(x => !x.IsValid))
            {
                ModelState.AddModelError("", "Every child family member requires a valid first name, last name, birthdate and gender.");
            }

            if (!ModelState.IsValid)
            {
                AddChildViewModel addChildviewModel = new AddChildViewModel();

                //Person person = await ArenaAPIHelper.GetPerson(id);
                //we lost the personId, not sure what I need here
                var firstChild = NewChildFamilyMembers.First();
                addChildviewModel.Person = new Person { FamilyId = firstChild.FamilyId, FamilyName = firstChild.FamilyName };
                addChildviewModel.NewChildFamilyMembers = NewChildFamilyMembers;
                return View(viewModel);
            }

            //everything looks good, lets add the kids to the family
            foreach(var child in NewChildFamilyMembers)
            {
                Person familyChild = MemberHelper.GetChildPersonFromMember(child);
                ArenaPostResult result = await ArenaAPIHelper.AddPerson(familyChild);

                if(!result.WasSuccessful)
                {
                    //stop processing and report error
                    return View("Error");
                }
            }

            viewModel.ChildrenAdded = NewChildFamilyMembers;
            return View("RegistrationComplete", viewModel);
        }

        [Route("{id}/Edit")]
        public async Task<ActionResult> AddChildren(int id)
        {
            if (id == default(int)) { RedirectToAction("New"); }

            AddChildViewModel viewModel = new AddChildViewModel();

            Person person = await ArenaAPIHelper.GetPerson(id);
            viewModel.Person =  person;
            viewModel.NewChildFamilyMembers = new List<Member>();
            viewModel.NewChildFamilyMembers.Add(new Member());

            return View(viewModel);
        }

        [Route("Find")]
        public ActionResult Find()
        {
            return View();
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
    }
}