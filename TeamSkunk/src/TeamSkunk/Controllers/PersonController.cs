using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamSkunk.Data;
using TeamSkunk.Models;
using TeamSkunk.Services;
using Kendo.Mvc.UI;
using TeamSkunk.ViewModels;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TeamSkunk.Controllers
{
    public class PersonController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public PersonController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        IUnitOfWork UoW)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            this.work = UoW;
        }

        public async Task<ActionResult> Index()
        {
            //USER PERMISSION CHECK? (This is a really dumb way of doing it but it works for now until I create some kind of Role Validator Class...And Roles for that matter...)
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            return View();
        }

        // GET: Members
        public ActionResult ReadPerson([DataSourceRequest]DataSourceRequest request)
        {
            //Get Models
            List<Person> models = work.Person.All().Include(guild => guild.Members).ToList();

            //Convert to ViewModels
            List<PersonVM> viewModels = new List<PersonVM>();

            foreach (Person model in models)
            {
                PersonVM vm = new PersonVM();

                //alter it's values to the new ones
                vm.PersonId = model.PersonId;
                vm.DiscordName = model.DiscordName;
                vm.NumberOfMembers = model.Members.Count;

                viewModels.Add(vm);
            }

            return Json(viewModels.ToDataSourceResult(request));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreatePerson([DataSourceRequest] DataSourceRequest request, PersonVM vm)
        {

            //check for validation
            if (vm != null && ModelState.IsValid)
            {
                //convert the VM into an actual activity
                Person model = new Person
                {
                    DiscordName = vm.DiscordName,
                    Members = vm.Members
                };

                //Insert the new activity into the database
                model = work.Person.Add(model);
                work.Save();
                vm.PersonId = model.PersonId;

            }

            //return the stuff back to the view
            return Json(new List<PersonVM> { vm }.ToDataSourceResult(new DataSourceRequest(), ModelState));
        }

        /// <summary>
        /// Updates a member
        /// </summary>
        /// <param name="request"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdatePerson([DataSourceRequest] DataSourceRequest request, PersonVM person)
        {
            //Check to make sure the form is valid
            if (person != null && ModelState.IsValid)
            {
                //look up the activity
                Person target = work.Person.Find(a => a.PersonId == person.PersonId);

                //check to make sure it exists
                if (target != null)
                {
                    //alter it's values to the new ones
                    target.DiscordName = person.DiscordName;
                    target.Members = person.Members;

                    //update the database
                    work.Person.Update(target);
                    work.Save();

                    person.PersonId = target.PersonId;
                }
            }

            //pass the result back to the view
            return Json(new[] { person }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>Destroys a guild record</summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DestroyPerson([DataSourceRequest] DataSourceRequest request, PersonVM person)
        {
            //check to make sure we've got a member
            if (person != null)
            {
                //find the member
                Person target = work.Person.Find(a => a.PersonId == person.PersonId);

                //delete it from the database
                work.Person.Delete(target);
                work.Save();
            }

            //return the result to the view
            return Json(new[] { person }.ToDataSourceResult(request, ModelState));
        }

        private void PopulateTiers()
        {
            List<SelectionVM> guildSelectionVMs = EnumToSelectList(typeof(GuildTiers));
            ViewData["Tiers"] = guildSelectionVMs;
        }

        public List<SelectionVM> EnumToSelectList(Type enumType)
        {
            return Enum
              .GetValues(enumType)
              .Cast<int>()
              .Select(i => new SelectionVM
              {
                  Value = i,
                  Text = Enum.GetName(enumType, i),
              }
              )
              .ToList();
        }

        //Really should be in another class a level above...
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
