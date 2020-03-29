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
using App.Services.Identity;
using System.IO;
using App.Domain.Models.Core;
using System.Text;
using FileIO = System.IO.File;

namespace App.Migration
{

    public class EnableSrf
    {
        private IServiceProvider _service;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISrfRequestService _srf;
        private readonly IUserProfileService _userProfile;
        private readonly ICandidateInfoService _candidate;
        private readonly IVacancyListService _vacancy;
        private readonly ISrfEscalationRequestService _escalation;
        private string Source = "source/";

        public EnableSrf(IServiceProvider service, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _roleManager = roleManager;
            _userManager = userManager;
            _userProfile = _service.GetService<IUserProfileService>();
            _srf = _service.GetService<ISrfRequestService>();
            _candidate = _service.GetService<ICandidateInfoService>();
            _vacancy = _service.GetService<IVacancyListService>();
            _escalation = _service.GetService<ISrfEscalationRequestService>();
        }

        public void Run()
        {
           // Activation();
            //AutoAnnual();
            Upload();
            Console.WriteLine("==> Srf Activation, Upload File has been updated !!");
        }

        private void Activation()
        {
            var AllSrf = _srf.GetAll().ToList();
            var AllUser = _userProfile.GetAll().ToList();
            var AllVacancy = _vacancy.GetAll().ToList();
            var AllCandidate = _candidate.GetAll().ToList();
            var AllEscalation = _escalation.GetAll().ToList();
            var GetLastSrf = _srf.GetAll().OrderByDescending(x => x.CreatedAt).GroupBy(x => x.CandidateId).Select(x => x.Key).ToArray();
            var SrfCandidate = _srf.GetAll().Where(x => GetLastSrf.Contains(x.CandidateId)).ToArray();
            foreach (var srf in SrfCandidate)
            {
                // Set Active Srf To User
                var _temp = _srf.GetById(srf.Id);
                _temp.IsActive = true;


                // Set Terminate
                if (!string.IsNullOrEmpty(srf.TeriminateNote))
                {
                    var Candidate = _candidate.GetById(srf.CandidateId);
                    if (Candidate.Account != null)
                    {
                        var pro = _userProfile.GetById(Candidate.AccountId);
                        pro.IsBlacklist = false;
                        pro.IsTerminate = true;
                        _userProfile.Update(pro);
                    }
                    _temp.Status = SrfStatus.Terminate;
                }

                _srf.Update(_temp);
            }

           

            // Generate SRF NUMBER
            //foreach (var row in AllSrf)
            //{
            //    String SrfNumber = null;
            //    if (string.IsNullOrEmpty(row.Number))
            //    {
            //        SrfNumber = "0000";
            //    }
            //    else
            //    {
            //        if (row.Number.Length > 4)
            //        {
            //            SrfNumber = row.Number.Substring(0, 4);
            //        }
            //        else
            //        {
            //            SrfNumber = row.Number;
            //        }
            //    }
            //    var _temp = _srf.GetById(row.Id);
            //    _temp.Number = SrfNumber;
            //    _srf.Update(_temp);
            //}
        }

        private void AutoAnnual()
        {
            var allsrf = _srf.GetAll().Where(x => x.SrfBegin.HasValue && x.SrfEnd.HasValue).OrderBy(x => x.CreatedAt).ToList();

            if (allsrf != null)
            {
                foreach (var row in allsrf)
                {
                    var temp = _srf.GetById(row.Id);
                    if (temp.Extend != null)
                    {
                        var Parent = _srf.GetById(temp.ExtendFrom);
                        temp.AnnualLeave = temp.AnnualLeave + Parent.AnnualLeave;
                    }
                    else
                    {
                        if (row.SrfBegin.HasValue && row.SrfEnd.HasValue)
                        {
                            temp.AnnualLeave = (temp.SrfEnd.Value.Month - temp.SrfBegin.Value.Month) <= 0 ? 0 : (temp.SrfEnd.Value.Month - temp.SrfBegin.Value.Month);
                        }
                        else
                        {
                            temp.AnnualLeave = 0;
                        }

                    }
                    _srf.Update(temp);
                }
            }
        }

        private void Upload()
        {

            var AllUser = _userProfile.GetAll().ToList();
            var AllVacancy = _vacancy.GetAll().ToList();
            var AllCandidate = _candidate.GetAll().ToList();
            var AllEscalation = _escalation.GetAll().ToList();

            foreach (var row in AllUser)
            {

                String IdNumber = null;
                if (!string.IsNullOrEmpty(row.IdNumber))
                {
                    if (row.IdNumber.Length > 16)
                    {
                        IdNumber = row.IdNumber.Substring(0, 16);
                    }
                    else
                    {
                        IdNumber = row.IdNumber;
                    }
                }
                else
                {
                    Random random = new Random();
                    IdNumber = random.Next(0, 16).ToString();
                }

                var Pro = _userProfile.GetById(row.Id);

                // Upload User Profile
                if (!String.IsNullOrEmpty(row.Photo))
                {
                    var File = System.IO.File.Exists(Source + "/" + row.Photo);
                    if (File == true)
                    {     
                        String src = Upload("temp", row.Photo);
                        FileInfo info = new FileInfo(Source + "/" + row.Photo);
                        var result = new Attachment()
                        {
                            Name = info.Name,
                            FileType = info.Extension,
                            Type = Attachment.FILE_TYPE_UPLOAD,
                            Path = src,
                            Size = info.Length / 1024
                        };
                        string output = JsonConvert.SerializeObject(result);
                        Pro.Photo = output;
                    }
                }


                Pro.IdNumber = IdNumber;
                _userProfile.Update(Pro);

            }



           // Vacancy

            foreach (var row in AllVacancy)
            {
                List<string> files = new List<string>();
                var vac = _vacancy.GetById(row.Id);

                if (!String.IsNullOrEmpty(row.Files))
                {
                    var attach = row.Files.Split(',');
                    if (attach != null)
                    {
                        foreach (var a in attach)
                        {
                            var File = System.IO.File.Exists(Source + "/" + a);
                            if (File == true)
                            {
                                String src = Upload("vacancy", a);
                                files.Add(src);
                            }
                        }
                    }
                }

                if (files != null)
                {
                    vac.Files = JsonConvert.SerializeObject(files);
                    _vacancy.Update(vac);
                }

            }


            // Candidate
            foreach (var row in AllCandidate)
            {
                List<string> files = new List<string>();
                var can = _candidate.GetById(row.Id);

                if (!String.IsNullOrEmpty(row.Attachments))
                {
                    var attach = row.Attachments.Split(',');
                    if (attach != null)
                    {
                        foreach (var a in attach)
                        {
                            var File = System.IO.File.Exists(Source + "/" + a);
                            if (File == true)
                            {
                                String src = Upload("candidate", a);
                                files.Add(src);
                            }
                        }
                    }
                }

                if (files != null)
                {
                    can.Attachments = JsonConvert.SerializeObject(files);
                    _candidate.Update(can);
                }
            }


            //Srf Escalation
            foreach (var row in AllEscalation)
            {
                List<string> files = new List<string>();
                var esc = _escalation.GetById(row.Id);

                if (!String.IsNullOrEmpty(row.Files))
                {
                    var attach = row.Files.Split(',');
                    if (attach != null)
                    {
                        foreach (var a in attach)
                        {
                            var File = System.IO.File.Exists(Source + "/" + a);
                            if (File == true)
                            {
                                String src = Upload("escalation", a);
                                files.Add(src);
                            }
                        }
                    }
                }

                if (files != null)
                {
                    esc.Files = JsonConvert.SerializeObject(files);
                    _escalation.Update(esc);
                }
            }
        }

        private string Upload(string dir,string file)
        {
            string path = "files/"+dir;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var from = System.IO.Path.Combine(Source + ""+ file);
            File.Move(from, path + "/"+file);
            return "uploads/"+dir+"/"+ file;
        }

    }
}
