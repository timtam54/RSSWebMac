using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class InspEquipTypeTest
    {
        public int id { get; set; }
        public int InspEquipID { get; set; }

        [Display(Name = "Test")]
        public int EquipTypeTestID { get; set; }

        public bool Pass { get; set; }

        public string? Reason { get; set; }

        public EquipTypeTest? EquipTypeTest { get; set; }

    }

}
