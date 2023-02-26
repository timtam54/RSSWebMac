using System.ComponentModel.DataAnnotations;

namespace RoofSafety.Models
{
    public class EquipType
    {
        public int id { get; set; }
        [Display(Name ="Equipment Type")]
        public string? EquipTypeDesc { get; set; }
      
    }

}
