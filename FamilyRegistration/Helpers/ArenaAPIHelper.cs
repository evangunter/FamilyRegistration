using Arena.NET;
using Arena.NET.Objects;
using Arena.NET.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FamilyRegistration.Helpers
{
    public static class ArenaAPIHelper
    {
        private static ArenaAPI _arenaAPI { get; set; }

        private static void Initalize()
        {
            if (_arenaAPI != null) { return; }

            //get settings
            String apiURL = ConfigurationManager.AppSettings["Arena_APIUrl"];
            String username = ConfigurationManager.AppSettings["Arena_Username"];
            String password = ConfigurationManager.AppSettings["Arena_Password"];
            String apiKey = ConfigurationManager.AppSettings["Arena_APIKey"];
            String apiSecretKey = ConfigurationManager.AppSettings["Arena_APISecret"];

            var credentials = new Credentials(username, password, apiKey, apiSecretKey);

            //configure our API settings
            _arenaAPI = new ArenaAPI(apiURL, credentials);
        }

        private static async Task StartSession()
        {
            await _arenaAPI.GetSessionAsync();
        }

        public static async Task<Person> GetPerson(int id)
        {
            if(!IsReady()) { await StartSession(); }

            PersonRepository repository = new PersonRepository(_arenaAPI);
            return await repository.Get(id);
        }

        public static async Task<List<Person>> GetFamily(int id)
        {
            if (!IsReady()) { await StartSession(); }

            PersonRepository repository = new PersonRepository(_arenaAPI);
            return await repository.GetFamily(id);
        }

        public static async Task<ArenaPostResult> AddPerson(Person newPerson)
        {
            if (!IsReady()) { await StartSession(); }

            PersonRepository repository = new PersonRepository(_arenaAPI);
            return await repository.InsertOrUpdate(newPerson);
        }

        public static async Task<ArenaPostResult> AddPersonToGroup(int personId, int groupId)
        {
            if (!IsReady()) { await StartSession(); }

            GroupRepository repository = new GroupRepository(_arenaAPI);
            return await repository.Insert(groupId, personId, new GroupMember { IsActive = true, RoleId = 24 });
        }

        public static async Task<ArenaPostResult> AddPersonNote(int personId, String note)
        {
            if (!IsReady()) { await StartSession(); }

            PersonRepository repository = new PersonRepository(_arenaAPI);
            return await repository.AddNote(personId, new PersonNote { IsPrivate = false, SecurityTemplateId = 0, Note = note });
        }

        public static async Task<List<Person>> GetPersons(PersonListOptions options)
        {
            if (!IsReady()) { await StartSession(); }

            PersonRepository repository = new PersonRepository(_arenaAPI);
            return await repository.Get(options);
        }

        private static Boolean IsReady()
        {
            if(_arenaAPI == null) { Initalize(); }

            if(_arenaAPI.Session == null || _arenaAPI.Session.IsExpired) { return false; }

            return true;
        }

    }
}