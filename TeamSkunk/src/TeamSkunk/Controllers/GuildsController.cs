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
using System.Net.Http;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TeamSkunk.Controllers
{
    public class GuildsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public GuildsController(
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

            PopulateTiers();
            return View();
        }

        // GET: Guild
        [HttpGet, ValidateAntiForgeryToken]
        public ActionResult ReadGuild([DataSourceRequest]DataSourceRequest request)
        {
            //Get Models
            List<Guild> models = work.Guild.All().Include(members => members.Members).ToList();

            //Convert to ViewModels
            List<GuildVM> viewModels = new List<GuildVM>();

            foreach (Guild model in models)
            {
                string recruiters = "";
                string officers = "";
                List<Member> officerList = model.Members.Where(m => m.isOfficer == true).ToList();
                List<Member> recruiterList = model.Members.Where(m => m.isRecruiter == true).ToList();

                //Make our concatinated recruiter string
                foreach (Member member in recruiterList)
                {
                    if(recruiterList.Count > 1)
                    {
                        recruiters = recruiters + member.Name + ", ";
                    }
                    else
                    {
                        recruiters = recruiters + member.Name;
                    }
                    
                }

                //Make our concatinated officer string (TODO: Do it better)
                foreach (Member member in officerList)
                {
                    if (officerList.Count > 1)
                    {
                        officers = officers + member.Name + ", ";
                    }
                    else
                    {
                        officers = officers + member.Name;
                    }
                    
                }

                GuildVM vm = new GuildVM();

                //alter it's values to the new ones
                vm.GuildId = model.GuildId;
                vm.Name = model.Name;
                vm.swgohLink = model.swgohLink;
                vm.Order = model.Order;
                vm.TransferTrump = model.TransferTrump;
                vm.RaidTimes = model.RaidTimes;
                vm.Remarks = model.Remarks;
                vm.ResetTime = model.ResetTime;
                vm.Tier = new SelectionVM { Value = (int)model.Tier, Text = model.Tier.ToString() };
                vm.MemberCount = model.Members.Count;
                vm.Recruiters = recruiters;
                vm.Officers = officers;
                
                viewModels.Add(vm);
            }

            return Json(viewModels.ToDataSourceResult(request));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateGuild([DataSourceRequest] DataSourceRequest request, GuildVM vm)
        {

            //check for validation
            if (vm != null && ModelState.IsValid)
            {
                //convert the VM into an actual activity
                Guild model = new Guild
                {
                    Name = vm.Name,
                    swgohLink = vm.swgohLink,
                    Order = vm.Order,
                    TransferTrump = vm.TransferTrump,
                    RaidTimes = vm.RaidTimes,
                    Remarks = vm.Remarks,
                    ResetTime = vm.ResetTime,
                    Tier = (GuildTiers)vm.Tier.Value
                };               

                //Insert the new activity into the database
                model = work.Guild.Add(model);
                work.Save();
                vm.GuildId = model.GuildId;

            }

            //return the stuff back to the view
            return Json(new List<GuildVM> { vm }.ToDataSourceResult(new DataSourceRequest(), ModelState));
        }

        /// <summary>
        /// Updates a member
        /// </summary>
        /// <param name="request"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
		[HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateGuild([DataSourceRequest] DataSourceRequest request, GuildVM guild)
        {
            //Check to make sure the form is valid
            if (guild != null && ModelState.IsValid)
            {
                //look up the activity
                Guild target = work.Guild.Find(a => a.GuildId == guild.GuildId);

                //check to make sure it exists
                if (target != null)
                {
                    //alter it's values to the new ones
                    target.Name = guild.Name;
                    target.swgohLink = guild.swgohLink;
                    target.Order = guild.Order;
                    target.TransferTrump = guild.TransferTrump;
                    target.RaidTimes = guild.RaidTimes;
                    target.Remarks = guild.Remarks;
                    target.ResetTime = guild.ResetTime;
                    target.Tier = (GuildTiers)guild.Tier.Value;

                    //update the database
                    work.Guild.Update(target);
                    work.Save();

                    guild.GuildId = target.GuildId;
                }
            }

            //pass the result back to the view
            return Json(new[] { guild }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>Destroys a guild record</summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DestroyGuild([DataSourceRequest] DataSourceRequest request, GuildVM guild)
        {
            //check to make sure we've got a member
            if (guild != null)
            {
                //find the member
                Guild target = work.Guild.Find(a => a.GuildId == guild.GuildId);

                //delete it from the database
                work.Guild.Delete(target);
                work.Save();
            }

            //return the result to the view
            return Json(new[] { guild }.ToDataSourceResult(request, ModelState));
        }

        private void PopulateTiers()
        {
            List<SelectionVM> guildSelectionVMs = EnumToSelectList(typeof(GuildTiers));
            ViewData["Tiers"] = guildSelectionVMs;
        }

        public async Task getHtml(int guildId, string link)
        {
            var req = WebRequest.Create(link);
            var r = await req.GetResponseAsync().ConfigureAwait(false);

            var responseReader = new StreamReader(r.GetResponseStream());
            var responseData = await responseReader.ReadToEndAsync();

            List<Member> members = GetGuildRoster(guildId, responseData);
            SaveGuildRoster(members);
        }

        public string SaveGuildRoster(List<Member> members)
        {
            foreach (Member member in members)
            {
                Member model = work.Member.Find(m => m.swgohURL == member.swgohURL);

                if (model != null)
                {
                    model.MemberId = member.MemberId;
                    model.PersonId = member.PersonId;
                    model.GuildId = member.GuildId;
                    model.Name = member.Name;
                    model.swgohURL = member.swgohURL;
                    model = work.Member.Update(model);
                    work.Save();
                }
                else
                {
                    Member newMember = new Member();
                    newMember.PersonId = member.PersonId;
                    newMember.GuildId = member.GuildId;
                    newMember.Name = member.Name;
                    newMember.swgohURL = member.swgohURL;
                    //Insert the new member into the database
                    model = work.Member.Add(newMember);
                    work.Save();
                }
            }
            return "Sync'd";
        }


        public List<Member> GetGuildRoster(int guildId, string guildLink)
        {
            string[] lines = guildLink.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            System.Collections.Generic.List<Member> result = new List<Member>();

            for (int i = 0, iLen = lines.Length; i < iLen; i++)
            {
                var line = lines[i].Trim();

                // found the next member
                if (line.IndexOf("href=\"/u/") != -1)
                {
                    // <td><a href="/u/tigore/">Tigore</a></td>

                    // member's link
                    var startIdx = line.IndexOf("/u/") + 3;
                    var endIdx = line.LastIndexOf("\">");
                    var toEndLength = line.Length - endIdx;
                    var newLength = line.Length - toEndLength;
                    newLength = newLength - startIdx;
                    var link = line.Substring(startIdx, newLength);

                    // member's name
                    startIdx = line.IndexOf("\">") + 2;
                    endIdx = line.LastIndexOf("</a>");
                    toEndLength = line.Length - endIdx;
                    newLength = line.Length - toEndLength;
                    newLength = newLength - startIdx;
                    var name = line.Substring(startIdx, newLength);
                    name = name.Replace("&#39;", "'");

                    Member member = work.Member.Find(m => m.swgohURL == "https://swgoh.gg/u/" + link);
                    if(member != null)
                    {
                        member.Name = name;
                        member.GuildId = guildId;
                        member.PersonId = 1;
                    }
                    else
                    {
                        member = new Member();
                        member.Name = name;
                        member.GuildId = guildId;
                        member.PersonId = 1;

                        member.swgohURL = "https://swgoh.gg/u/" + link;
                    }
                    result.Add(member);
                }
            }

            return result;
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
