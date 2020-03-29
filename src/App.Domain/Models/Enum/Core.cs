using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Enum
{
    public enum Status
    {
        Close = 0,
        Active = 1
    }

    public enum AttendanceExceptionListType
    {
        Etc = 0,
        Regular = 1
    }

    public enum StatusOne
    {
        Reject = 0,
        Waiting = 1,
        Approved = 2
    }

    public enum StatusTwo
    {
        Rejected = 0,
        Waiting = 1,
        Approved = 2
    }

    

    public enum ActiveStatus
    {
        Cancel = 0,
        Active = 1,
        Done = 2
    }

    public enum Cycle
    {
        Daily = 0,
        Monthly  = 1
    }

    public enum Gender
    {
        Male = 0,
        Female = 1
    }
    public enum CandidateStatus
    {
        NotProvided = 0,
        WaitingBooking = 1,
        Booked = 2,
        Issued = 3
    }
    public enum ApproverStatus
    {
        Shortlist = 0,
        Selected = 1,
        Completed = 2,
        Rejected = 3,
    }

    public enum AgencyType
    {
        Agency = 1,
        EID = 2
    }

    public enum ClaimType
    {
        GeneralClaim  = 0,
        TravelClaim = 1
    }

    public enum Martial
    {
        BK = 0,
        D0 = 1,
        D1 = 2,
        D2 = 3,
        Divorce = 4,
        K0 = 5,
        K1 = 6,
        K2 = 7,
        K3 = 8,
        Ko = 9,
        M = 10,
        M1 = 11,
        M0 = 12,
        M2 = 13,
        M3 = 14, 
        M4 = 15, 
        married = 16,
        MO = 17,
        S = 18,
        Single = 19,
        TK = 20,
    }

    public enum Schedule
    {
        [Description("Morning/Daytime schedule")]
        MorningDay = 1,

        [Description("Afternoon/Night schedule")]
        AfternoonNight = 2
    }

    public enum TripType
    {
        [Description("Domestic")]
        Domestic = 1,

        [Description("International")]
        International = 2
    }

    public enum AllowanceOptions
    {
        [Description("Option 1")]
        Option1 = 1,

        [Description("Option 2")]
        Option2 = 2
    }

    public enum EmployeeLevel
    {
        Staff = 1,
        Manager =2 
    }

    public enum DayType
    {
        [Description("Week Day & Week End")]
        WeekDay = 1,

        [Description("Lebaran/Christmas/Granted Holiday")]
        Holiday = 2
    }

    public enum CustomerType
    {
        Showroom = 0,
        Rental = 1
    }

    public enum PendingActive
    {
        NotPending = 0,
        PendingActiveUntilContract = 1
    }
    public enum Activities { Claim,Flight,Request,SRF,Timesheet,Travel,Vacant }
    public enum NetworkStatus { Closed, Active }
    public enum NotificationInboxStatus { Error, Request, Approval, Reject }
    public enum PackageStatus { Inactive, Actived }
    public enum ProjectStatus { Close, Active }
    public enum PackageTypes { A, B, FSO, EXPAT, WP }
    public enum SpacateStatus { Close, Active }
    public enum Level { Staff, Manager }
    public enum ServicePackStatus { Close, Active }
    public enum StatusEscalation { Submitted, Waiting, Done ,Reject }
    public enum SrfType { New , Extension }
    public enum SrfApproveStatus { Waiting , Submitted, Approved, Reject }
    public enum BastApproveStatus { Waiting, Approved, Reject }
    public enum SrfStatus { Waiting, Done, WaitingTerminate, Terminate, Blacklist }
    public enum RateType { Normal, SpecialRate }
    public enum TicketStatus { Close, Open }
    public enum TicketInfoStatus {
        [Description("Cancel")]
        Cancel,
        [Description("Waiting")]
        WaitingBooking,
        [Description("Booked")]
        Booked,
        [Description("Issue")]
        Issue
    }
    public enum VacanStatusFirst { Rejected, Waiting, Selected }
    public enum VacanStatusSecond { Rejected, WaitingReview, Approved, WaitingSelectedCV }
    public enum VacanStatusThirth { Closed, Active, Done }
    public enum VacanStatusFourth { Rejected, Approved }
    public enum VacanStatusFive { Closed, Active, Done }
    public enum WoItemType { Jasa, Part }
    public enum WotStatus { Rejected, Waiting, Approved }
    public enum WotStatusFinal { Close, Active, Done }

    public enum UserApprover
    {
        Administrator = 0,
        LineManager = 1,
        ProjectManager = 2,
        Agency = 3,
        ServiceCoordinator = 4,
        HeadOfServiceLine = 5,
        HeadOfOperation = 6,
        HeadOfNonOperation = 7,
        HeadOfSourcing = 8,
        Sourcing = 9
    }
}
