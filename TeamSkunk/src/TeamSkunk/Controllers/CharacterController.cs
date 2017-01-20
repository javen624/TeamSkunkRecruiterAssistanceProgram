using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamSkunk.ViewModels;
using TeamSkunk.ViewModels.CharacterViewModels;
using TeamSkunk.Services;
using AutoMapper;
using TeamSkunk.Models;
using TeamSkunk.Data;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.IO;

namespace TeamSkunk.Controllers
{
    [Authorize]
    public class CharacterController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public CharacterController(
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

        //
        // GET: /Character/Index
        [HttpGet]
        public async Task<ActionResult> Index(CharacterMessageId? message = null)
        {
            
            //USER PERMISSION CHECK? (This is a really dumb way of doing it but it works for now until I create some kind of Role Validator Class...And Roles for that matter...)
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            return View();
        }

        //
        // GET: /Character/Index
        [HttpGet]
        public async Task<ActionResult> CharacterGrid()
        {

            //USER PERMISSION CHECK? (This is a really dumb way of doing it but it works for now until I create some kind of Role Validator Class...And Roles for that matter...)
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            return View();
        }

        // GET: Characters
        public ActionResult ReadCharacters([DataSourceRequest]DataSourceRequest request)
        {
            //Get Models
            List<Character> models = work.Character.All().Include(c => c.Member).ToList();

            //Convert to ViewModels
            List<CharacterVM> viewModels = new List<CharacterVM>();

            foreach (Character model in models)
            {
                CharacterVM vm = new CharacterVM();

                //alter it's values to the new ones
                vm.CharacterId = model.CharacterId;
                vm.Name = model.Name;
                vm.MemberId = model.MemberId;
                vm.Gear = model.Gear;
                vm.Level = model.Level;
                vm.Stars = model.Stars;
                vm.SevenStarG10 = model.SevenStarG10;
                viewModels.Add(vm);
            }

            //return Json(viewModels);
            return Json(viewModels.ToDataSourceResult(request));
        }

        /*[HttpGet]
        public JsonResult GetPlayers(int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetPlayers(page, limit, sortBy, direction, searchString, out total);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(Player player)
        {
            new GridModel().Save(player);
            return Json(true);
        }

        [HttpPost]
        public JsonResult Remove(int id)
        {
            new GridModel().Remove(id);
            return Json(true);
        }*/

        public List<Character> getMemberCharacters(int memberId)
        {
            List<Character> characters = new List<Character>();
            return characters;
        }

        [HttpGet]
        public ActionResult AddCharacter()
        {
            return View();
        }
    
        //Really don't need this anymore since we pull from swgoh.gg
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CharacterVM character)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var config = new MapperConfiguration(cfg => {
                        cfg.CreateMap<Character, CharacterVM>();
                    });

                    IMapper mapper = config.CreateMapper();
                    var vm = character;
                    var model = mapper.Map<CharacterVM, Character>(vm);

                    work.Character.Add(model);
                    work.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(character);
        }

        //
        // POST: /Character/RemoveCharacter
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateCharacter(CharacterVM character)
        {
            CharacterMessageId? message = CharacterMessageId.Error;

            Character model = _characterRepository.GetCharacterByID(character.CharacterId);

            var result = _characterRepository.UpdateCharacter(model);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                message = CharacterMessageId.RemoveCharacterSuccess;
            }
            return RedirectToAction(nameof(character), new { Message = message });
        }*/

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum CharacterMessageId
        {
            RemoveCharacterSuccess,
            ChangeCharacterNameSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
