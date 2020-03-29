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
    public class Migration
    {
        private IServiceProvider _service;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;


        private int Count { get; set; }

        public Migration(IServiceProvider service,RoleManager<ApplicationRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Run()
        {
            WebSetting();
            Roles();
            Users();
            City();
            Jobstage();
            SubOps();
            Projects();
            AllowanceForm();
            PackageType();
            Activity();
            ClaimCategory();
            AccountName();
            Department();
            DepartmentSub();
            CostCenter();
            ServicePackCategory();
            NetworkNumber();
            ServicePack();
            AllowanceList();
        }

        #region Web Setting
        private void WebSetting()
        {
            String JsonFile = "websetting.json";
            if (FileIO.Exists(JsonFile))
            {
                var fs = FileIO.OpenRead(JsonFile);
                using (var r = new StreamReader(fs))
                {
                    var json = r.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<List<WebSetting>>(json);
                    Count = 0;
                    var dbService = _service.GetService<IWebSettingService>();
                    if(data!=null)
                    {
                        foreach (var item in data)
                        {
                            dbService.Add(new Domain.Models.Core.WebSetting()
                            {
                                Name = item.Name,
                                OtherInfo = item.OtherInfo,
                                SystemSetting = item.SystemSetting,
                                Value = item.Value == null ? "" : item.Value
                            });
                            Count++;
                        }
                    }
                    Console.WriteLine(Count + " Web Settings record has been saved ");
                }
            }
        }
        #endregion

        #region Identity
        private void Roles()
        {
            Count = 0;
            Data dt = new Data("_roles");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                var Result = _roleManager.CreateAsync(new ApplicationRole() {  Name = Data[0].ToString(), Description = Data[0].ToString() }).Result;
                if(Result.Succeeded)
                {
                    Count++;
                }

            }
            Console.WriteLine(Count + " Roles record has been saved ");
        }

        private void Users()
        {
            Count = 0;
            Data dt = new Data("_users_approver");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;

                String Email = Data[0].ToString();
                String Username = Data[1].ToString();
                List<String> Role = Data[2].Split(',').ToList();
                ApplicationUser NewUser = new ApplicationUser()
                {
                    Email = Email,
                    UserName = Username,
                    UserProfile = new UserProfile()
                    {
                        Address = Data[3].ToString(),
                        Birthdate = (!string.IsNullOrEmpty(Data[4].ToString()) ? DateTime.Parse(Data[4].ToString()) : DateTime.MinValue),
                        Email = Email,
                        Gender = (Gender)Enum.ToObject(typeof(Gender), int.Parse(Data[6])),
                        HomePhoneNumber = Data[7].ToString(),
                        IdNumber = Data[9].ToString(),
                        IsActive = true,
                        MobilePhoneNumber = Data[8].ToString(),
                        Name = Data[10].ToString(),
                        Roles = Data[2].ToString(),
                        UserName = Username,
                        AhId = Data[6].ToString(),
                        IsBlacklist = false,
                        IsTerminate = false,
                        Photo = Data[11].ToString(),
                    }
                };
                var result = _userManager.CreateAsync(NewUser, "welcome1!").Result;
                if (result.Succeeded)
                {
                    result = _userManager.AddToRolesAsync(NewUser, Role).Result;
                    if (result.Succeeded)
                    {
                        var code = _userManager.GenerateEmailConfirmationTokenAsync(NewUser).Result;
                        var confirm = _userManager.ConfirmEmailAsync(NewUser, code);
                        if (confirm.Result.Succeeded)
                        {
                            Count++;
                        }
                    }
                }

            }
            Console.WriteLine(Count + " Users record has been saved ");
        }

        #endregion

        #region Core
        private void City()
        {
            Count = 0;
            var dbService = _service.GetService<ICityService>();
            Data dt = new Data("city");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if(!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.City()
                    {
                        Name = Data[0].ToString(),
                        Status = (Status)Enum.ToObject(typeof(Status) , int.Parse(Data[1]))
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " City record has been saved ");
        }

        private void Jobstage()
        {
            Count = 0;
            var dbService = _service.GetService<IJobStageService>();
            Data dt = new Data("jobstage");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                dbService.Add(new Domain.Models.Core.JobStage()
                {
                    Stage = Data[0].ToString(),
                    Description = Data[1].ToString(),
                });
                Count++;
            }
            Console.WriteLine(Count + " Jobstage record has been saved ");
        }

        private void SubOps()
        {
            Count = 0;
            var dbService = _service.GetService<ISubOpsService>();
            Data dt = new Data("subopscode");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.SubOps()
                    {
                        Code = Data[0].ToString(),
                        Description = Data[1].ToString(),
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Sub Ops Code record has been saved ");
        }

        private void Projects()
        {
            Count = 0;
            var dbService = _service.GetService<IProjectsService>();
            Data dt = new Data("project");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.Projects()
                    {
                        Name = Data[0].ToString(),
                        Description = Data[1].ToString(),
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Project record has been saved ");
        }

        private void AllowanceForm()
        {
            Count = 0;
            var dbService = _service.GetService<IAllowanceFormService>();
            Data dt = new Data("allowance_form");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.AllowanceForm ()
                    {
                        Name = Data[0].ToString(),
                        Value = int.Parse(Data[1].ToString()),
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Allowance Form record has been saved ");
        }

        private void PackageType()
        {
            Count = 0;
            var dbService = _service.GetService<IPackageTypeService>();
            Data dt = new Data("package_type");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.PackageType()
                    {
                        Name = Data[0].ToString(),
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Package Type  record has been saved ");
        }

        private void TimeSheetType()
        {
            Count = 0;
            var dbService = _service.GetService<ITimeSheetTypeService>();
            Data dt = new Data("package_type");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.TimeSheetType()
                    {
                        Type = Data[0].ToString(),
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " TimeSheet Type  record has been saved ");
        }

        private void Activity()
        {
            Count = 0;
            var dbService = _service.GetService<IActivityCodeService>();
            Data dt = new Data("activity_code");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.ActivityCode()
                    {
                        Code = Data[0].ToString(),
                        Description = Data[1].ToString(),
                        Status = (Status)Enum.ToObject(typeof(Status), int.Parse(Data[2]))
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Activity Code Type  record has been saved ");
        }

        private void ClaimCategory()
        {
            Count = 0;
            var dbService = _service.GetService<IClaimCategoryService>();
            Data dt = new Data("claim_category");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.ClaimCategory()
                    {
                       Name = Data[0].ToString(),
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Claim Category record has been saved ");
        }


        private void AccountName()
        {
            Count = 0;
            var dbService = _service.GetService<IAccountNameService>();
            var Profile = _service.GetService<IUserProfileService>();
            Data dt = new Data("account_name");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]) &&
                    !string.IsNullOrEmpty(Data[1]))
                {
                    var Users = _userManager.FindByEmailAsync(Data[2].ToString()).Result;
                    if(Users==null)
                    {
                        Users = _userManager.FindByEmailAsync("muhammad.ivan.azrian@ericsson.com").Result;
                    }
                    var GetProfile = Profile.GetByUserId(Users.Id);

                    dbService.Add(new Domain.Models.Core.AccountName()
                    {
                        Name = Data[0].ToString(),
                        Status = Data[1].ToString().Equals("true") ? true : false,
                        Com = GetProfile.Id
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Account Name record has been saved ");
        }

        private void Department()
        {
            Count = 0;
            var dbService = _service.GetService<IDepartementService>();
            var Profile = _service.GetService<IUserProfileService>();
            Data dt = new Data("department_list");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[3]))
                {
                    var Users = _userManager.FindByEmailAsync(Data[3].ToString()).Result;
                    var GetProfile = Profile.GetByUserId(Users.Id);
                    dbService.Add(new Domain.Models.Core.Departement()
                    {
                        Name = Data[0].ToString(),
                        OperateOrNon = Data[1].ToString().Equals("1") ? 1 : 0,
                        Description = Data[2].ToString(),
                        HeadId = GetProfile.Id
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Department record has been saved ");
        }

        private void DepartmentSub()
        {
            Count = 0;
            var dbService = _service.GetService<IDepartementSubService>();
            var Profile = _service.GetService<IUserProfileService>();
            var Department = _service.GetService<IDepartementService>();
            Data dt = new Data("departmentsub_list");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]) &&
                    !string.IsNullOrEmpty(Data[2]) &&
                    !string.IsNullOrEmpty(Data[3]))
                {
                    var Users = _userManager.FindByEmailAsync(Data[3].ToString()).Result;
                    var GetProfile = Profile.GetByUserId(Users.Id);
                    var Dept = Department.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[2].ToString().ToLower().Trim())).FirstOrDefault(); 
                    dbService.Add(new Domain.Models.Core.DepartementSub()
                    {
                        SubName = Data[0].ToString(),
                        DsStatus = Data[1].ToString().Equals("1") ? 1 : 0,
                        DepartmentId = Dept.Id,
                        LineManagerid = GetProfile.Id
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Department Sub record has been saved ");
        }

        private void CostCenter()
        {
            Count = 0;
            var dbService = _service.GetService<ICostCenterService>();
            var Department = _service.GetService<IDepartementService>();
            Data dt = new Data("cost_center_list");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]) && !string.IsNullOrEmpty(Data[3]))
                {
                    var Users = _userManager.FindByEmailAsync(Data[3].ToString()).Result;
                    var Dept = Department.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[3].ToString().ToLower().Trim())).FirstOrDefault();
                    dbService.Add(new Domain.Models.Core.CostCenter()
                    {
                        Code = Data[0].ToString(),
                        Description = Data[1].ToString(),
                        Status = (Status)Enum.ToObject(typeof(Status), int.Parse(Data[2])),
                        Departement = Dept
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Cost Center record has been saved ");
        }

        private void ServicePackCategory()
        {
            Count = 0;
            var dbService = _service.GetService<IServicePackCategoryService>();
            Data dt = new Data("ssow_category");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]))
                {
                    dbService.Add(new Domain.Models.Core.ServicePackCategory()
                    {
                        Name = Data[0].ToString(),
                        Level = (Level)Enum.ToObject(typeof(Level), int.Parse(Data[1])),
                        Status = (ServicePackStatus)Enum.ToObject(typeof(ServicePackStatus), int.Parse(Data[2]))
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Service Pack Category record has been saved ");
        }

        private void NetworkNumber()
        {
            Count = 0;
            var dbService = _service.GetService<INetworkNumberService>();
            var Project = _service.GetService<IProjectsService>();
            var Account = _service.GetService<IAccountNameService>();
            var Department = _service.GetService<IDepartementService>();
            Data dt = new Data("network_number_list");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                if (!string.IsNullOrEmpty(Data[0]) &&
                    !string.IsNullOrEmpty(Data[3]) &&
                    !string.IsNullOrEmpty(Data[4]) &&
                    !string.IsNullOrEmpty(Data[5]) &&
                    !string.IsNullOrEmpty(Data[6]) &&
                    !string.IsNullOrEmpty(Data[7]))
                {
                    var _project = Project.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[3].ToString().ToLower().Trim())).FirstOrDefault();
                    var _account = Account.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[5].ToString().ToLower().Trim())).FirstOrDefault();
                    var _department = Department.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[6].ToString().ToLower().Trim())).FirstOrDefault();
                    dbService.Add(new Domain.Models.Core.NetworkNumber()
                    {
                        Code = Data[0].ToString(),
                        Description = Data[1].ToString(),
                        Status = (NetworkStatus)Enum.ToObject(typeof(NetworkStatus), int.Parse(Data[2])),
                        Project = _project,
                        AccountName = _account,
                        ProjectManager = GetUser(Data[4].ToString()),
                        Departement = _department,
                        LineManager = GetUser(Data[7].ToString())
                    });
                    Count++;
                }
            }
            Console.WriteLine(Count + " Network number record has been saved ");
        }

        private void ServicePack()
        {
            Count = 0;
            var dbService = _service.GetService<IServicePackService>();
            var Category = _service.GetService<IServicePackCategoryService>();
            Data dt = new Data("ssow");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                var _category = Category.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[10].ToString().ToLower().Trim())).FirstOrDefault();
                int TypeSsOW = (int)Enum.Parse(typeof(PackageTypes), Data[0].ToString());
                dbService.Add(new Domain.Models.Core.ServicePack()
                {
                    Type = (PackageTypes)TypeSsOW,
                    Name = Data[1].ToString(),
                    Code = Data[2].ToString(),
                    Rate = Decimal.Parse(Data[3].ToString()),
                    Hourly = Decimal.Parse(Data[4].ToString()),
                    Otp20 = Decimal.Parse(Data[5].ToString()),
                    Otp30 = Decimal.Parse(Data[6].ToString()),
                    Otp40 = Decimal.Parse(Data[7].ToString()),
                    Laptop = Decimal.Parse(Data[8].ToString()),
                    Usin = Decimal.Parse(Data[9].ToString()),
                    ServicePackCategory = _category
                });
                Count++;
            }
            Console.WriteLine(Count + " Service Pack record has been saved ");
        }

        private void AllowanceList()
        {
            Count = 0;
            var dbService = _service.GetService<IAllowanceListService>();
            var ServicePack = _service.GetService<IServicePackService>();
            Data dt = new Data("allowance_list");
            foreach (var row in dt.GetData())
            {
                var Data = (List<String>)row;
                var _ssow = ServicePack.GetAll().Where(x => x.Name.ToLower().Trim().Equals(Data[7].ToString().ToLower().Trim())).FirstOrDefault();
                dbService.Add(new Domain.Models.Core.AllowanceList()
                {
                   AllowanceNote = Data[0].ToString(),
                   OnCallNormal = Decimal.Parse(Data[1].ToString()),
                   ShiftNormal = Decimal.Parse(Data[2].ToString()),
                   OnCallHoliday = Decimal.Parse(Data[3].ToString()),
                   ShiftHoliday = Decimal.Parse(Data[4].ToString()),
                   GrantedHoliday14 = Decimal.Parse(Data[5].ToString()),
                   AllowanceStatus = (Status)Enum.ToObject(typeof(Status), int.Parse(Data[6])),
                   ServicePack = _ssow
                });
                Count++;
            }
            Console.WriteLine(Count + " Allowance List record has been saved ");
        }

        #endregion

        private UserProfile GetUser(string email)
        {
            var Profile = _service.GetService<IUserProfileService>();
            if (!string.IsNullOrEmpty(email))
            {
                var User = _userManager.FindByEmailAsync(email).Result;
                if(User!=null)
                {
                    return Profile.GetByUserId(User.Id);
                }
            }
            return null;
        }


    }
}
