using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamSkunk.Data;
using TeamSkunk.Models;
using TeamSkunk.ViewModels;
using AutoMapper;
using TeamSkunk.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TeamSkunk.Controllers
{
    public class MembersController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public MembersController(
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

            PopulateGuilds();
            PopulatePersons();
            PopulateRecruiters();
            PopulateTimezones();
            return View();
        }

        // GET: Members
        public ActionResult ReadMembers([DataSourceRequest]DataSourceRequest request)
        {
            //Get Models
            List<Member> models = work.Member.All().Include(person => person.Person).Include(guild => guild.Guild).ToList();

            //Convert to ViewModels
            List<MemberVM> viewModels = new List<MemberVM>();

            foreach (Member model in models)
            {
                MemberVM vm = new MemberVM();

                //alter it's values to the new ones
                vm.MemberId = model.MemberId;
                vm.Name = model.Name;
                vm.GuildId = model.GuildId;
                vm.Guild = new SelectionVM { Value = model.Guild.GuildId, Text = model.Guild.Name };
                vm.JoinDate = model.JoinDate;
                vm.RecruitedBy = new SelectionVM { Value = model.recruitedById, Text = model.recruitedByName };
                vm.Timezone = new SelectionStringVM { Value = model.TimezoneId, Text = model.TimezoneName };
                vm.PersonId = model.PersonId;
                vm.Person = new SelectionVM { Value = model.Person.PersonId, Text = model.Person.DiscordName };
                vm.isLeader = model.isLeader;
                vm.isOfficer = model.isOfficer;
                vm.isRecruiter = model.isRecruiter;
                vm.swgohURL = model.swgohURL;

                viewModels.Add(vm);
            }

            return Json(viewModels.ToDataSourceResult(request));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateMember([DataSourceRequest] DataSourceRequest request, MemberVM vm)
        {

            //check for validation
            if (vm != null && ModelState.IsValid)
            {
                //Make our Person Object
                Person person = work.Person.Find(p => p.PersonId == vm.Person.Value);

                //Make our Guild Object
                Guild guild = work.Guild.Find(g => g.GuildId == vm.Guild.Value);

                //convert the VM into an actual activity
                Member model = new Member();

                model.Name = vm.Name;
                model.Guild = guild;
                model.JoinDate = DateTime.Now;
                if (!vm.isLeader)
                {
                    model.recruitedById = vm.RecruitedBy.Value;
                    model.recruitedByName = vm.RecruitedBy.Text;
                }
                model.TimezoneId = vm.Timezone.Value;
                model.TimezoneName = vm.Timezone.Text;
                model.Person = person;
                model.isLeader = vm.isLeader;
                model.isOfficer = vm.isOfficer;
                model.isRecruiter = vm.isRecruiter;
                model.swgohURL = vm.swgohURL;

                //Insert the new activity into the database
                model = work.Member.Add(model);
                work.Save();
                vm.MemberId = model.MemberId;
            }

            //return the stuff back to the view
            return Json(new[] { vm }.ToDataSourceResult(new DataSourceRequest(), ModelState));
        }

        /// <summary>
        /// Updates a member
        /// </summary>
        /// <param name="request"></param>
        /// <param name="member"></param>
        /// <returns></returns>
		[HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateMember([DataSourceRequest] DataSourceRequest request, MemberVM member)
        {
            //Check to make sure the form is valid
            if (member != null && ModelState.IsValid)
            {
                //look up the activity
                Member target = work.Member.Find(a => a.MemberId == member.MemberId);

                //check to make sure it exists
                if (target != null)
                {
                    //Make our Person Object
                    Person person = work.Person.Find(p => p.PersonId == member.Person.Value);

                    //Make our Guild Object
                    Guild guild = work.Guild.Find(g => g.GuildId == member.Guild.Value);

                    //alter it's values to the new ones
                    target.Name = member.Name;
                    target.Guild = guild;
                    target.JoinDate = member.JoinDate;
                    if (!member.isLeader)
                    {
                        target.recruitedById = member.RecruitedBy.Value;
                        target.recruitedByName = member.RecruitedBy.Text;
                    }
                    target.TimezoneId = member.Timezone.Value;
                    target.TimezoneName = member.Timezone.Text;
                    target.Person = person;
                    target.isLeader = member.isLeader;
                    target.isOfficer = member.isOfficer;
                    target.isRecruiter = member.isRecruiter;
                    target.swgohURL = member.swgohURL;

                    //update the database
                    target = work.Member.Update(target);
                    work.Save();
                    member.MemberId = target.MemberId;
                }
            }

            //pass the result back to the view
            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>Destroys a member record</summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DestroyMember([DataSourceRequest] DataSourceRequest request, MemberVM member)
        {
            //check to make sure we've got a member
            if (member != null)
            {
                //find the member
                Member target = work.Member.Find(a => a.MemberId == member.MemberId);

                //delete it from the database
                work.Member.Delete(target);
                work.Save();
            }

            //return the result to the view
            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        private void PopulateGuilds()
        {
            List<SelectionVM> guildSelectionVMs = new List<SelectionVM>();
            List<Guild> guilds = work.Guild.All().ToList();

            foreach (Guild guild in guilds)
            {
                SelectionVM guildSelectionVM = new SelectionVM()
                {
                    Text = guild.Name,
                    Value = guild.GuildId
                };
                guildSelectionVMs.Add(guildSelectionVM);
            }

            ViewData["Guilds"] = guildSelectionVMs;
            if (guildSelectionVMs.Count > 0)
            {
                ViewData["defaultGuild"] = guildSelectionVMs.First();
            }
        }

        private void PopulatePersons()
        {
            List<SelectionVM> personSelectionVMs = new List<SelectionVM>();
            List<Person> persons = work.Person.All().ToList();

            foreach (Person person in persons)
            {
                SelectionVM personSelectionVM = new SelectionVM()
                {
                    Text = person.DiscordName,
                    Value = person.PersonId
                };
                personSelectionVMs.Add(personSelectionVM);
            }

            ViewData["Persons"] = personSelectionVMs;
            if (personSelectionVMs.Count > 0)
            {
                ViewData["defaultPerson"] = personSelectionVMs.First();
            }
        }

        private void PopulateRecruiters()
        {
            List<SelectionVM> recruiterSelectionVMs = new List<SelectionVM>();
            List<Person> recruiters = work.Person.All().Where(r => r.Members.Any(m => m.isRecruiter == true)).ToList();

            //Default Recruiter
            if (recruiters.Count < 1)
            {
                SelectionVM recruiterSelectionVM = new SelectionVM()
                {
                    Text = "MilesRiker",
                    Value = 1
                };
                recruiterSelectionVMs.Add(recruiterSelectionVM);
            }

            //Add Recruiters
            foreach (Person recruiter in recruiters)
            {
                SelectionVM recruiterSelectionVM = new SelectionVM()
                {
                    Text = recruiter.DiscordName,
                    Value = recruiter.PersonId
                };
                recruiterSelectionVMs.Add(recruiterSelectionVM);
            }

            ViewData["Recruiters"] = recruiterSelectionVMs;
            if (recruiterSelectionVMs.Count > 0)
            {
                ViewData["defaultRecruiter"] = recruiterSelectionVMs.First();
            }
        }

        private void PopulateTimezones()
        {
            List<SelectionStringVM> timezoneSelectionVMs = new List<SelectionStringVM>();
            List<TimeZoneInfo> timezones = TimeZoneInfo.GetSystemTimeZones().ToList();

            foreach (TimeZoneInfo timezone in timezones)
            {
                SelectionStringVM timezoneSelectionVM = new SelectionStringVM()
                {
                    Text = timezone.DisplayName,
                    Value = timezone.Id
                };
                timezoneSelectionVMs.Add(timezoneSelectionVM);
            }

            ViewData["Timezones"] = timezoneSelectionVMs;
            if (timezoneSelectionVMs.Count > 0)
            {
                ViewData["defaultTimezone"] = timezoneSelectionVMs.First();
            }
        }

        public string SaveMemberInfo(int memberId, List<Character> characters)
        {
            foreach (Character character in characters)
            {
                Character model = work.Character.Find(ch => ch.CharacterId == character.CharacterId);

                if (model != null)
                {
                    model.CharacterId = character.CharacterId;
                    model.Gear = character.Gear;
                    model.Level = character.Level;
                    model.Stars = character.Stars;
                    model.Name = character.Name;
                    model.MemberId = character.MemberId;
                    model = work.Character.Update(model);
                    work.Save();
                }
                else
                {
                    Character newCharacter = new Character();
                    newCharacter.Gear = character.Gear;
                    newCharacter.Level = character.Level;
                    newCharacter.Stars = character.Stars;
                    newCharacter.Name = character.Name;
                    newCharacter.MemberId = memberId;

                    //Insert the new member into the database
                    newCharacter = work.Character.Add(newCharacter);
                    work.Save();
                }
            }

            return "Sync'd";
        }

        // convert a Roman Numeral to a number
        public int ConvertRomanNumeral(string value)
        {
            switch (value)
            {
                case "I":
                    return 1;
                case "II":
                    return 2;
                case "III":
                    return 3;
                case "IV":
                    return 4;
                case "V":
                    return 5;
                case "VI":
                    return 6;
                case "VII":
                    return 7;
                case "VIII":
                    return 8;
                case "IX":
                    return 9;
                case "X":
                    return 10;
                case "XI":
                    return 11;
                case "XII":
                    return 12;
            }

            return 0;
        }

        public struct statStruct
        {
            public int level, gear, stars;

            public statStruct(int lvl, int geer, int strs)
            {
                level = lvl;
                gear = geer;
                stars = strs;
            }
        }

        // get the stats for the given hero
        public statStruct GetAllStats(string[] lines, int lineIdx, string name)
        {
            //<a href="/u/tigore/collection/tie-fighter-pilot/" class="char-portrait-full-link" rel="nofollow">
            //  <img class="char-portrait-full-img" src="//swgoh.gg/static/img/assets/tex.charui_tiepilot.png" alt="TIE Fighter Pilot" title="TIE Fighter Pilot">
            //    <div class="char-portrait-full-gear"></div>
            //    <div class="star star1"></div>
            //    <div class="star star2"></div>
            //    <div class="star star3"></div>
            //    <div class="star star4"></div>
            //    <div class="star star5"></div>
            //    <div class="star star6"></div>
            //    <div class="star star7 star-inactive"></div>
            //    <div class="char-portrait-full-level">85</div>
            //    <div class="char-portrait-full-gear-level">X</div>

            var starToken = "title=\"" + name + "\"";
            var found = false;
            var level = 0;
            var gear = 0;
            var stars = 0;

            for (int i = lineIdx, iLen = lines.Length; i < iLen; i++)
            {
                var line = lines[i];

                if (found)
                {
                    if (line.IndexOf("star star") != -1 && line.IndexOf("star-inactive") == -1)
                    {
                        // update stars
                        stars++;
                    }
                    else if (line.IndexOf("char-portrait-full-level") != -1)
                    {
                        // get hero level
                        var startIdx = line.IndexOf(">") + 1;
                        var endIdx = line.IndexOf("</div>");
                        var toEndLength = line.Length - endIdx;
                        var newLength = line.Length - toEndLength;
                        newLength = newLength - startIdx;
                        level = Int32.Parse(line.Substring(startIdx, newLength));
                    }
                    else if (line.IndexOf("char-portrait-full-gear-level") != -1)
                    {
                        // get gear level
                        var startIdx = line.IndexOf(">") + 1;
                        var endIdx = line.IndexOf("</div>");
                        var toEndLength = line.Length - endIdx;
                        var newLength = line.Length - toEndLength;
                        newLength = newLength - startIdx;
                        var gearVal = line.Substring(startIdx, newLength);
                        gear = ConvertRomanNumeral(gearVal);
                        break;
                    }
                }
                else if (line.IndexOf(starToken) != -1)
                {
                    found = true;
                }
            }

            statStruct result = new statStruct(level, gear, stars);
            return result;
        }

        public async Task getHtml(int memberId, string playerLink)
        {
            string collectionLink = playerLink + "collection/";
            var req = WebRequest.Create(collectionLink);
            var r = await req.GetResponseAsync().ConfigureAwait(false);

            var responseReader = new StreamReader(r.GetResponseStream());
            var responseData = await responseReader.ReadToEndAsync();

            List<Character> characters = GetMemberHeroes(memberId, responseData);
            SaveMemberInfo(memberId, characters);
        }

        public List<Character> GetMemberHeroes(int memberId, string responseData)
        {
            // Tigore's Characters (74)
            //Member member = work.Member.Find(m => m.MemberId == memberId);

            string[] lines = responseData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<Character> result = new List<Character>();
            var foundPlayer = false;
            string heroName = "None";
            statStruct stats = new statStruct(0, 0, 0);
            var playerLevel = 1;
            var heroCount = 0;
            var qualified = 0;

            //result[0] = ["Level 0", "Hero", "Level", "Gear", "Stars", "7* G10+"];

            for (int i = 0, iLen = lines.Length; i < iLen; i++)
            {
                var line = lines[i];
                var lineIdx = i;

                if (foundPlayer)
                {
                    // <img ... alt="Biggs Darklighter" title="Biggs Darklighter">
                    if (line.IndexOf("alt=") != -1 && line.IndexOf("title=") != -1)
                    {
                        // found new hero
                        var startIdx = line.IndexOf("title=\"") + 7;
                        var endIdx = line.IndexOf("\">");
                        var toEndLength = line.Length - endIdx;
                        var newLength = line.Length - toEndLength;
                        newLength = newLength - startIdx;
                        heroName = line.Substring(startIdx, newLength);
                        stats = GetAllStats(lines, i, heroName);
                        Character c = new Character();
                        c.Name = heroName.Replace("/ &quot;/ g", "\"");
                        c.Level = stats.level;
                        c.Gear = stats.gear;
                        c.Stars = stats.stars;
                        result.Add(c);


                        // count heroes that are 7* and gear 10+
                        if (stats.stars >= 7 && stats.gear >= 10)
                        {
                            qualified++;
                        }
                        heroCount++;
                    }
                }
                else
                {
                    if (line.IndexOf("'s Characters (") != -1)
                    {
                        foundPlayer = true;
                    }
                    else if (line == "Level")
                    {
                        line = lines[i + 1];

                        //<h5 class="m-y-0">85</h5>
                        var startIdx = line.IndexOf(">") + 1;
                        var endIdx = line.IndexOf("</h5>");
                        var toEndLength = line.Length - endIdx;
                        var newLength = line.Length - toEndLength;
                        newLength = newLength - startIdx;
                        playerLevel = Int32.Parse(line.Substring(startIdx, newLength));
                        //result.Add = "Level " + playerLevel;
                    }
                }
            }

            //result[1][5] = qualified;

            return result;
        }
        /*
        public void QualifiedCount(playerLink)
        {
            //playerLink = "https://swgoh.gg/u/tigore/";

            // Tigore's Characters (74)
            var response;
            try
            {
                response = UrlFetchApp.fetch(playerLink + "collection/");
            }
            catch (e)
            {
                return "";
            }

            var text = response.getContentText();
            var lines = text.split("\n");
            var foundPlayer = false;
            var heroName = "None";
            var stats = 0;
            var qualified = 0;
            var heroes = "";

            for (var i = 0, iLen = lines.length; i < iLen; i++)
            {
                var line = lines[i];
                lineIdx = i;

                if (foundPlayer)
                {
                    // <img ... alt="Biggs Darklighter" title="Biggs Darklighter">
                    if (line.indexOf("alt=") !== -1 && line.indexOf("title=") !== -1)
                    {
                        // found new hero
                        var startIdx = line.indexOf("title=\"") + 7;
                        var endIdx = line.indexOf("\">");
                        heroName = line.substring(startIdx, endIdx);
                        stats = GetAllStats(lines, i, heroName);

                        // count heroes that are 7* and gear 10+
                        if (stats[2] >= 7 && stats[1] >= 10)
                        {
                            qualified++;
                            if (heroes.length > 0)
                            {
                                heroes += ",";
                            }
                            heroes += heroName.replace(/ &quot;/ g, "\"");

                if (stats[2] < 7)
                {
                  // no longer in the 7* heroes
                  break;
                }
              }
            }
            else if (line.indexOf("'s Characters (") !== -1)
            {
              foundPlayer = true;
            }
          }

          var result = [qualified];
        result[0] = [qualified, heroes];

          return result;  
        }
            }*/
        //Really should be in another class a level above...
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
