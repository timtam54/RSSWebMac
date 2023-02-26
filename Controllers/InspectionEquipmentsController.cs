using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Models;
using RoofSafety.Services.Abstract; 

namespace RoofSafety.Controllers
{
    public class InspectionEquipmentsController : Controller
    {
        private readonly dbcontext _context;
        private readonly IImageService _imageservice;
        public InspectionEquipmentsController(dbcontext context, IImageService imageservice)
        {
            _context = context;
            _imageservice = imageservice;
        }

        public  class DescParID
        {
            public string Desc { get; set; }
            public int ID { get; set; }

        }
        public async Task<ActionResult> EquipForInspections(int id)
        {
            var xxx = _context.InspEquip.Where(i => i.InspectionID == id).Include(i => i.EquipType).Include(i => i.Inspection).Include(i=>i.EquipType);
            var yyy= await xxx.ToListAsync();
            ViewBag.InspectionID = id;
            DescParID xx = (from ie in _context.Inspection join bd in _context.Building on ie.BuildingID equals bd.id where ie.id == id select new DescParID { Desc = ("Building " + bd.BuildingName + " @ " + ie.InspectionDate.ToString("dd-MM-yyyy")),ID=ie.BuildingID }).FirstOrDefault();
            ViewBag.InspectionDesc = xx.Desc;
            ViewBag.BuildingID = xx.ID;
            return View("Index",xxx);
        }

        public async Task<ActionResult> EquipForInspectionsAll(int id)
        {
            InspectionRpt ret = new InspectionRpt();
            ret.Inspector = (from ins in _context.Inspection join emp in _context.Employee on ins.InspectorID equals emp.id where ins.id == id select emp.Given + " " + emp.Surname).FirstOrDefault();
            var insp = _context.Inspection.Where(i => i.id == id).FirstOrDefault();
            ret.InspDate = insp.InspectionDate;
            ret.Areas = insp.Areas;
            ret.Instrument = insp.TestingInstruments;
            ret.Tests = "Test";
            ret.Title = (from bd in _context.Building where bd.id == insp.BuildingID select bd.BuildingName).FirstOrDefault();
            ret.Items = (from ie in _context.InspEquip join et in _context.EquipType on ie.EquipTypeID equals et.id where ie.InspectionID == id select new InspEquipTest { Manufacturer =ie.Manufacturer, EquipName = et.EquipTypeDesc, Notes = ie.Notes, Location = ie.Location, id = ie.id, EquipType = et }).ToList();//.Include(i => i.EquipType).Include(i => i.Inspection).Include(i => i.EquipType)=efe
            ret.Versions = _context.Version.ToList();
            if (insp.Photo!=null)
                ret.Photo = _imageservice.GetImageURL(insp.Photo);
            foreach (var item in ret.Items)
            {
                item.RequiredControls = "";
                item.TestResult = (from iet in _context.InspEquipTypeTest join ett in _context.EquipTypeTest on iet.EquipTypeTestID equals ett.id where iet.InspEquipID == item.id select new TestResult { Test=ett.Test, PassFail=iet.Pass, FailReason=iet.Reason, EquipTypeTestID=iet.EquipTypeTestID }).ToList();//.Include(i => i.EquipType).Include(i => i.Inspection).Include(i => i.EquipType)=efe//HazardIfNonCompliant = ett.HazardIfNonCompliant, 
                List<int> HazardID = new List<int>();
                foreach (var tr in item.TestResult)
                {
                    if (tr.PassFail==false)
                    {
                        HazardID=HazardID.Concat( _context.EquipTypeTestHazards.Where(i => i.EquipTypeTestID == tr.EquipTypeTestID).Select(o => o.HazardID).ToList()).ToList();
                    }
                }
                item.Hazards = "";
                foreach (var hz in _context.Hazard.Where(i => HazardID.Contains(i.id)).Select(i => i.Detail).ToList())
                {
                    item.Hazards= item.Hazards+hz+", ";
                }
                item.Photos = _context.InspPhoto.Where(i=>i.InspEquipID==item.id).ToList();
                foreach (var phot in item.Photos)
                {
                    phot.photoname = _imageservice.GetImageURL(phot.photoname);
                }
            }
            ViewBag.InspectionID = id;
            DescParID xx = (from ie in _context.Inspection join bd in _context.Building on ie.BuildingID equals bd.id where ie.id == id select new DescParID { Desc = ("Building " + bd.BuildingName + " @ " + ie.InspectionDate.ToString("dd-MM-yyyy")), ID = ie.BuildingID }).FirstOrDefault();
            ViewBag.InspectionDesc = xx.Desc;
            ViewBag.BuildingID = xx.ID;
            return View(ret);
        }

        public async Task<IActionResult> Index(int InspectionID)
        {
            var xxx = _context.InspEquip.Where(i => i.InspectionID == InspectionID).Include(i => i.EquipType).Include(i=>i.Inspection);
            var yyy = await xxx.ToListAsync();
            return View(yyy);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InspEquip == null)
            {
                return NotFound();
            }
            var inspectionEquipment = await _context.InspEquip
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            return View(inspectionEquipment);
        }

        public IActionResult Create(int? id)
        {
            ViewBag.EquipmentTypeID = (from xx in _context.EquipType select new SelectListItem() { Value = xx.id.ToString(), Text = xx.EquipTypeDesc }).ToList();
            InspEquip ret = new InspEquip();
            ret.InspectionID = id.Value;
           
         
            return View(ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,InspectionID,EquipTypeID,Location,Notes,Inspection")] InspEquip inspEquip)
        {
            var xx = ModelState.Values.SelectMany(i => i.Errors);
            //if (ModelState.IsValid)
            {
                inspEquip.id = 0;
                _context.Add(inspEquip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EquipForInspections),new { id = inspEquip.InspectionID });
            }
            return View(inspEquip);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InspEquip == null)
            {
                return NotFound();
            }

            var inspectionEquipment = await _context.InspEquip.FindAsync(id);
            var Photos =  _context.InspPhoto.Where(i => i.InspEquipID == id).ToList();
            foreach (var item in Photos)
            {
                item.photoname = _imageservice.GetImageURL(item.photoname);
            }
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            ViewBag.EquipmentTypeID = (from xx in _context.EquipType select new SelectListItem() { Value = xx.id.ToString(), Text = xx.EquipTypeDesc }).ToList();
            inspectionEquipment.Photos = Photos;

            return View(inspectionEquipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,InspectionID,Location,Notes,EquipTypeID")] InspEquip inspEquip)
        {
            if (id != inspEquip.id)
            {
                return NotFound();
            }

           // if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspEquip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionEquipmentExists(inspEquip.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EquipForInspections),new {id=inspEquip.InspectionID });
            }
            return View(inspEquip);
        }

        // GET: InspectionEquipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InspEquip == null)
            {
                return NotFound();
            }

            var inspectionEquipment = await _context.InspEquip
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            return View(inspectionEquipment);
        }

        // POST: InspectionEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InspEquip == null)
            {
                return Problem("Entity set 'dbcontext.InspectionEquipment'  is null.");
            }
            var inspectionEquipment = await _context.InspEquip.FindAsync(id);
            int inspid = inspectionEquipment.InspectionID;
            if (inspectionEquipment != null)
            {
                _context.InspEquip.Remove(inspectionEquipment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EquipForInspectionsAll), new { id = inspid  });
        }

        private bool InspectionEquipmentExists(int id)
        {
          return _context.InspEquip.Any(e => e.id == id);
        }
    }
}
