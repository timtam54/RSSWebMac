using Microsoft.AspNetCore.Mvc;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;
using System.Diagnostics;
using RoofSafety.Data;

namespace RoofSafety.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IImageService _imageservice;

        private readonly dbcontext _context;


            public HomeController(ILogger<HomeController> logger, IImageService imageservice, dbcontext context)
        {
            _context = context;

            _logger = logger;
            _imageservice = imageservice;
        }

        public IActionResult Index(int id)
        {
            ImageModel im = new ImageModel();
            im.InspEquipID = id;
            return View(im);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePicture(ImageModel imageModel)
        {
            try
            {
                if (imageModel.File == null || imageModel.File.FileName == null)
                    return View("Index");
                var UniqueName = _imageservice.UploadImageToAzure(imageModel.File);
                InspPhoto ip = new InspPhoto();
                ip.InspEquipID = imageModel.InspEquipID;
                ip.description = imageModel.description;
                ip.photoname = UniqueName;
                _context.Add(ip);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }
            return RedirectToAction("Edit","InspectionEquipments",new {id=imageModel.InspEquipID });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}