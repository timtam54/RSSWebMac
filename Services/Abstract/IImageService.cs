using System;
namespace RoofSafety.Services.Abstract
{
	public interface IImageService
	{
		string UploadImageToAzure(IFormFile file);
		string GetImageURL(string name);
	}
}

