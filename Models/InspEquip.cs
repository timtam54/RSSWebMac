using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace RoofSafety.Models
{
    public class InspEquip
    {
        public int id { get; set; }
        [Display(Name = "Type")]
        public int EquipTypeID { get; set; }
        [Display(Name = "Inspection")]
        public int InspectionID { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }

        public Inspection? Inspection { get; set; }
        public EquipType? EquipType { get; set; }

        public string? Manufacturer { get; set; }
        public string? Installer { get; set; }
        public string? Rating { get; set; }
        public string? SerialNo { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public List<InspPhoto>? Photos { get; set; } 
    }

    public class InspectionRpt
    {
        public List<Version> Versions { get; set; } 
        public string Title { get; set; }   
        public DateTime InspDate { get; set; }
        public string Areas { get; set; }
        public string Callibration { get; set; }
        public string Tests { get; set; }
        public string Inspector { get; set; }

        public string Instrument { get; set; }
        public List<InspEquipTest> Items { get; set; }
        public string Photo { get; set; }

    }
    public class InspEquipTest
    {
        public string Result
        {
            get
            {
                return (TestResult.Where(i => i.PassFail == false).Count() == 0) ? "Compliant" : "Non-Compliant";
            }
        }
        public string Risk
        {
            get
            {
                return (TestResult.Where(i => i.PassFail == false).Count() == 0) ? "Maintained" : "Risk";
            }
        }

        public string? Hazards
        {
            get
            
                ;
            

            set ; 
        }
        public class Exp
        {
            public string? P1 { get; set; }
            public string? P2 { get; set; }
        }
        public Exp Explanation
        {
            get
            {
                Exp ret = new Exp();
                ret.P2 = ""; ret.P1 = "";
                if (TestResult.Count(i => i.PassFail == false) == 0)
                {
                   
                    foreach (var tr in TestResult.ToList())
                    {
                        if (ret.P1 == "")
                            ret.P1 = tr.FailReason;
                        else
                            ret.P1 = ret.P1 + ", " + tr.FailReason;

                    }

                }
                else
                {
                    foreach (var tr in TestResult.Where(i => i.PassFail == false).ToList())
                    {
                        if (ret.P1 == "")
                            ret.P1 = tr.FailReason;
                        else
                            ret.P1 = ret.P1 + ", " + tr.FailReason;

                        if (ret.P2 == "")
                            ret.P2 = tr.Test;
                        else
                            ret.P2 = ret.P2 + ", " + tr.Test;
                    }
                }
                return ret ;
                //return (TestResult.Where(i => i.PassFail == false).Count() == 0) ? "Maintained" : "Risk";
            }
        }

        public int id { get; set; }

        [Display(Name = "Inspection")]
        public int InspectionID { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public string? EquipName { get; set; }
        public EquipType? EquipType { get; set; }


        public string? RequiredControls { get; set; }

        public List<TestResult> TestResult { get; set; }


        public string? Manufacturer { get; set; }
        public string? Installer { get; set; }
        public string? Rating { get; set; }
        public string? SerialNo { get; set; }
        public DateTime? WithdrawalDate { get; set; }

        public List<InspPhoto>? Photos { get; set; }
    }

    public class TestResult
    {
        public string? Test { get; set; }
        public bool? PassFail { get; set; }

        public int? EquipTypeTestID { get; set; }

        public string? FailReason { get; set; }

    }



}
