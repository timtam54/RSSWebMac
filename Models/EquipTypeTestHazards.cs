using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class EquipTypeTestHazards
    {
        public int id { get; set; }
        public int HazardID { get; set; }
        public int EquipTypeTestID { get; set; }
        public Hazard? Hazard { get; set; } 
        public string? Haz { get; set; }
    }
}
