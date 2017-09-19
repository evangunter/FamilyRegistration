using Arena.NET.Objects;
using FamilyRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyRegistration.Helpers
{
    public static class MemberHelper
    {
        public enum FamilyMemberRole { Adult = 29, Child = 31 };
        public enum Campus {  Brownsboro = 1, Clifton = 2 };
        public enum PhoneType {  Cell = 282, Main = 276, Business = 277 };
        public enum MemberStatus { Visitor = 12394, Member = 958, ChildNonMember = 11940, Prospect = 11961, };
        public enum GroupMemberRoleId { Member = 24 };
        public enum VisitorGroups { BrownsboroVisitors = 1125, CliftonVisitors = 1126 };

        public static Person GetAdultPersonFromMember(Member member, Campus campus)
        {
            return new Person
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                Emails = new List<Email> { new Email { Address = member.Email } },
                Phones = new List<Phone> {  new Phone { Number = member.PhoneNumber, PhoneTypeId = (int)PhoneType.Main } },
                FamilyMemberRoleId = (int)FamilyMemberRole.Adult,
                CampusId = (int)campus,
                MemberStatusId = (int)MemberStatus.Visitor
            };
        }

        public static Person GetChildPersonFromMember(Member member, Campus campus)
        {
            return new Person
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                //Emails = new List<Email> { new Email { Address = member.Email } },
                //Phones = new List<Phone> { new Phone { Number = member.PhoneNumber, PhoneTypeId = (int)PhoneType.Main } },
                FamilyMemberRoleId = (int)FamilyMemberRole.Child,
                CampusId = (int) campus,
                MemberStatusId = (int)MemberStatus.Visitor,
                FamilyId = member.FamilyId,
                BirthDate = member.BirthDate,
                Gender = member.Gender,
                MedicalInformation = member.MedicalInformation
            };
        }
    }
}