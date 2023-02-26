using System;
using System.ComponentModel;

namespace RoofSafety.Models
{
	public class ImageModel
	{
        public int InspEquipID { get; set; }
        //[DisplayName("Upload your file")]
		//public string? FileDetails { get; set; }
		public IFormFile? File { get; set; }
        public string? description { get; set; }
    }
    public class InspPhoto
    {
        public int id { get; set; }
        public int InspEquipID { get; set; }
        public string photoname { get; set; }
        public string? description { get; set; }
    }

   
}

