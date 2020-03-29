using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using App.Services.Core.Interfaces;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using App.Data.DAL;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using FluentScheduler;
using App.Helper;
using MimeKit;
using MailKit.Net.Smtp;
using RazorLight;
using System.IO;
using App.Services.Identity;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using App.Domain.Models.Core;
using System.Linq.Expressions;

namespace App.Background
{
    public class Job
    {
        private IServiceProvider _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private String _hosting;

        public Job(IServiceProvider service, UserManager<ApplicationUser> userManager,String hosting)
        {
            _service = service;
            _userManager = userManager;
            _hosting = hosting;
        }

        public void Run()
        {
            Console.WriteLine("==> Staring Background Tasks "+DateTime.Now);
            Console.WriteLine("");
            Console.WriteLine("Please Wait....");
            JobManager.AddJob(() => AutoTerminate(), (s) => s.WithName("Auto Terminated").ToRunEvery(1).Days().At(00, 00));
            JobManager.AddJob(() => LineManageReminder("End Soon Reminder", "Reminder Task E-Servicebased.com", -15, "Extension"), (s) => s.WithName("End Contract").ToRunEvery(1).Days().At(09, 00));
            JobManager.AddJob(() => LineManageReminder("End Soon Reminder", "Reminder Task E-Servicebased.com", -30, "Extension"), (s) => s.WithName("End Contract").ToRunEvery(1).Days().At(09, 00));
            JobManager.AddJob(() => LineManageReminder("End Soon Reminder", "Reminder Task E-Servicebased.com", -45, "Extension"), (s) => s.WithName("End Contract").ToRunEvery(1).Days().At(09, 00));
            JobManager.AddJob(() => LineManageReminder("Terminate / Blacklist", "Reminder Task E-Servicebased.com", -15, "Terminate / Blacklist"), (s) => s.WithName("Terminate Or Blacklist").ToRunEvery(1).Days().At(09, 00));
            JobManager.AddJob(() => PendingTask(), (s) => s.WithName("Pending Task").ToRunEvery(1).Days().At(15, 00));
            JobManager.AddJob(() => DeleteJunkFiles(), (s) => s.WithName("Delete Junk Files").ToRunEvery(1).Days().At(24, 00));
        }

        public void Testing(string email)
        {
            var Service = _service.GetService<ISrfRequestService>();
            Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[11];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;
            var Data = Service.GetAll(Includes).Where(x => x.IsLocked == false && x.IsActive == true && x.Candidate.IsUser == true).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if(Data!=null)
            {
                var Model = new
                {
                    Email = email,
                    ContractorName = Data.Candidate.Name,
                    LineManagername = email,
                    Date = Data.SrfEnd.Value.ToString("dd MMM yyyy"),
                    SSOW = Data.ServicePack.Name,
                    Type = "Test"
                };
                Send("Test", "Test", Data.Candidate.Name, email, "EndSoon", Model, null);
                Console.WriteLine("Testing Send From " + Data.Candidate.Name);
            }
        }

        private void LineManageReminder(string title,string subject,int days,string type)
        {
            var Service = _service.GetService<ISrfRequestService>();
            var Candidate = _service.GetService<ICandidateInfoService>();
            var UserProfile = _service.GetService<IUserProfileService>();
            var Vacancy = _service.GetService<IVacancyListService>();

            Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[11];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;

            var Data = Service.GetAll(Includes).Where(x=> x.SrfEnd.HasValue && x.SrfEnd.Value.AddDays(days).Date == DateTime.Now.Date && x.IsLocked == false && x.IsActive ==true && x.Candidate.IsUser == true).ToList();
            if(Data!=null)
            {
                foreach(var row in Data)
                {
                    
                    var LineManager = UserProfile.GetById(row.ApproveOneId);
                    var ServiceLine = UserProfile.GetById(row.ApproveTwoId);
                    var Sourcing = UserProfile.GetById(row.Candidate.Vacancy.ApproverTwoId);

                    var LineManagerUser = _userManager.FindByIdAsync(LineManager.ApplicationUserId).Result;
                    var ServiceLineUser = _userManager.FindByIdAsync(ServiceLine.ApplicationUserId).Result;
                    var SourcingUser = _userManager.FindByIdAsync(Sourcing.ApplicationUserId).Result;

                    List<String> UserCC = new List<string>();
                    UserCC.Add(LineManagerUser.Email);  // Line Manager
                    if(ServiceLineUser!=null)
                    {
                        UserCC.Add(ServiceLineUser.Email);  // Service Line
                    }
                    if(SourcingUser!=null)
                    {
                        UserCC.Add(SourcingUser.Email);  // Sourcing
                    }

                    if (LineManagerUser != null)
                    {
                        var Model = new
                        {
                            Email = LineManagerUser.Email,
                            ContractorName = row.Candidate.Name,
                            LineManagername = LineManager.Name,
                            Date = row.SrfEnd.Value.ToString("dd MMM yyyy"),
                            SSOW = row.ServicePack.Name,
                            Type = type
                        };
                        Send(title, subject, LineManager.Name, LineManagerUser.Email, "EndSoon", Model, UserCC);
                    }
                }
            }
        }
        private void PendingTask()
        {
            var TimeSheet = _service.GetService<IAttendaceExceptionListService>();
            var Travel = _service.GetService<IClaimService>();
            var Claim = _service.GetService<IClaimService>();
            var ListProjectManager = GetByRoleName("Project Manager").ToList();
            var ListLineManager = GetByRoleName("Line Manager").ToList();
            var Profile = _service.GetService<IUserProfileService>();
            if (ListProjectManager != null)
            {
                foreach (var row in ListProjectManager)
                {
                    var CountTimeSheet = TimeSheet
                        .GetAll()
                        .Where(x => x.ApproverOneId.Equals(row.Id) && x.StatusOne == StatusOne.Waiting)
                        .Count();

                    var CountClaim = Claim
                        .GetAll()
                        .Where(x => x.ClaimApproverOneId.Equals(row.Id) && x.ClaimType == ClaimType.GeneralClaim && x.StatusOne == StatusOne.Waiting && x.StatusTwo == StatusTwo.Waiting)
                        .Count();

                    var CountTravel = Claim
                        .GetAll()
                        .Where(x => x.ClaimApproverOneId.Equals(row.Id) && x.ClaimType == ClaimType.GeneralClaim && x.StatusOne == StatusOne.Waiting && x.StatusTwo == StatusTwo.Waiting)
                        .Count();

                    var UserProjectManager = _userManager.FindByIdAsync(row.ApplicationUserId).Result;
                    var ProfilePm = Profile.GetById(row.Id);

                    if (UserProjectManager != null)
                    {
                        var Model = new
                        {
                            Username = UserProjectManager.UserName,
                            CountTimeSheet = CountTimeSheet,
                            CountClaim = CountClaim,
                            CountTravel = CountTravel,
                            TimeSheetUrl = _hosting + "Admin/Registration",
                            ClaimUrl = _hosting + "Admin/Claim",
                            TravelUrl = _hosting + "Admin/Travel",
                        };

                        if (CountTimeSheet > 0 || CountClaim > 0 || CountTravel > 0)
                        {
                            List<string> _UserCC = new List<string>();

                            var ListUserAgency = GetByRoleName("HR Agency");
                            var ListUserSourcing = GetByRoleName("Sourcing");

                            foreach(var ag in ListUserAgency)
                            {
                               var input = _userManager.FindByIdAsync(ag.ApplicationUserId).Result;
                                _UserCC.Add(input.Email);
                            }

                            foreach (var src in ListUserSourcing)
                            {
                                var input = _userManager.FindByIdAsync(src.ApplicationUserId).Result;
                                _UserCC.Add(input.Email);
                            }


                            Send("Pending Task", "PENDING TASK E-Servicebased.com", ProfilePm.Name, UserProjectManager.Email, "EmailDaily", Model, _UserCC);
                        }
                    }

                }
            }

            if(ListLineManager!=null)
            {
                foreach (var row in ListLineManager)
                {
                    var CountTimeSheet = TimeSheet
                       .GetAll()
                       .Where(x => x.ApproverTwoId.Equals(row.Id) && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Waiting)
                       .Count();

                    var CountClaim = Claim
                        .GetAll()
                        .Where(x => x.ClaimApproverTwoId.Equals(row.Id) && x.ClaimType == ClaimType.TravelClaim && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Waiting)
                        .Count();

                    var CountTravel = Claim
                        .GetAll()
                        .Where(x => x.ClaimApproverTwoId.Equals(row.Id) && x.ClaimType == ClaimType.TravelClaim && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Waiting)
                        .Count();

                    var UserLineManager = _userManager.FindByIdAsync(row.ApplicationUserId).Result;
                    var ProfileLm = Profile.GetById(row.Id);

                    if (UserLineManager != null)
                    {
                        var Model = new
                        {
                            Username = UserLineManager.UserName,
                            CountTimeSheet = CountTimeSheet,
                            CountClaim = CountClaim,
                            CountTravel = CountTravel,
                            TimeSheetUrl = _hosting + "Admin/Registration",
                            ClaimUrl = _hosting + "Admin/Claim",
                            TravelUrl = _hosting + "Admin/Travel",
                        };

                        if (CountTimeSheet > 0 || CountClaim > 0 || CountTravel > 0)
                        {
                            Send("Pending Task", "PENDING TASK E-Servicebased.com", ProfileLm.Name, UserLineManager.Email, "EmailDaily", Model);
                        }
                    }
                }
            }

        }
        private void Test()
        {
            var dbService = _service.GetService<ICityService>();
            var Data = dbService.GetAll().ToList();
            Console.WriteLine(JsonConvert.SerializeObject(Data));
        }
        private async void DeleteJunkFiles()
        {
            var myHttpClient = new HttpClient();
            var response = await myHttpClient.GetAsync(_hosting + "File/Delete");
            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("Delete junk file has been deleted at " + DateTime.Now);
            }
        }
        private void Send<T>(string Activity,string Subject, String Name,String Email,String View,T model,List<String> UserCC = null)
        {
            try
            {
                IRazorLightEngine engine = EngineFactory.CreatePhysical(@"" + Path.GetFullPath("Emails"));
                var _config = _service.GetService<ConfigHelper>();
                var emailForm = _config.GetConfig("smtp.from.email");
                var password = _config.GetConfig("smtp.password");
                var SmtpServer = _config.GetConfig("smtp.server");
                var SmtpPortNumber = _config.GetConfig("smtp.port");
                var useSSL = _config.GetConfig("smtp.ssl");
                var message = new MimeMessage();
                var bodyBuilder = new BodyBuilder();
                message.From.Add(new MailboxAddress("noreply@e-servicebased.net", emailForm));
                message.To.Add(new MailboxAddress(Name, Email));

                if(UserCC!=null)
                {
                    foreach(var row in UserCC)
                    {
                        message.Cc.Add(new MailboxAddress(row, row));
                    }
                }
               

                message.Subject = Subject;
                bodyBuilder.HtmlBody = engine.Parse(View+".cshtml", model);
                message.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, int.Parse(SmtpPortNumber), Boolean.Parse(useSSL));
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailForm, password);
                    client.Send(message);
                    client.Disconnect(true);
                    Console.WriteLine(Activity + " notification has been sent at " + DateTime.Now);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void AutoTerminate()
        {
            var Service = _service.GetService<ISrfRequestService>();
            var Candidate = _service.GetService<ICandidateInfoService>();
            var UserProfile = _service.GetService<IUserProfileService>();
            var Vacancy = _service.GetService<IVacancyListService>();

            Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[11];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;

            var Data = Service.GetAll(Includes).Where(x => x.TerminatedDate.Date == DateTime.Now.Date && x.IsLocked == false && x.IsActive == true && x.Candidate.IsUser == true).ToList();

            if (Data.Any())
            {
                foreach(var row in Data)
                {
                    var srf = Service.GetById(row.Id);
                    srf.TeriminateNote = "Terminate Resource";
                    srf.TerimnatedBy = "";
                    Service.Update(srf);

                    // Update User To Terminate
                    var contractor = Candidate.GetById(srf.CandidateId);
                    if (contractor != null && contractor.AccountId.HasValue)
                    {
                        var userProfile = UserProfile.GetById(contractor.AccountId);
                        userProfile.IsActive = false;
                        userProfile.IsTerminate = true;
                        userProfile.IsBlacklist = false;
                        UserProfile.Update(userProfile);
                    }

                }
            }

        }

        public List<UserProfile> GetByRoleName(string role)
        {
            List<UserProfile> item = new List<UserProfile>();
            var _userProfile = _service.GetService<IUserProfileService>();
            if (!string.IsNullOrEmpty(role))
            {
                var User = _userManager.GetUsersInRoleAsync(role).Result.ToList();
                foreach (var row in User)
                {
                    var UserProfile = _userProfile.GetByUserId(row.Id);
                    item.Add(UserProfile);
                }
            }
            return item;
        }

    }
}
