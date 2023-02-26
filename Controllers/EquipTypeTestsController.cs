using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Models;

namespace RoofSafety.Controllers
{
    public class EquipTypeTestsController : Controller
    {
        private readonly dbcontext _context;

        public EquipTypeTestsController(dbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return View(await _context.EquipTypeTest.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EquipTypeTest == null)
            {
                return NotFound();
            }

            var equipTypeTest = await _context.EquipTypeTest
                .FirstOrDefaultAsync(m => m.id == id);
            if (equipTypeTest == null)
            {
                return NotFound();
            }
            return View(equipTypeTest);
        }

        public IActionResult Create(int? id)
        {
            EquipTypeTest ret = new EquipTypeTest();
            var equiptype = _context.InspEquip.Where(i => i.id == id).Include(i=>i.EquipType).FirstOrDefault();
            ret.EquipTypeID = equiptype.EquipTypeID;
            ViewBag.EquipTypeDesc = equiptype.EquipType.EquipTypeDesc;
            ViewBag.inspequipid = id;
            return View(ret);

        }

        public async Task<ActionResult> TestsForEquipType(int id)
        {
            var xxx = _context.EquipTypeTest.Where(i => i.EquipTypeID == id).Include(i => i.EquipType).Include(i => i.EquipType);
            var yyy = await xxx.ToListAsync();
            ViewBag.InspectionID = id;
            InspectionEquipmentsController.DescParID xx = (from ie in _context.EquipType where ie.id == id select new InspectionEquipmentsController.DescParID { Desc = ("Equipment Type " + ie.EquipTypeDesc), ID = ie.id }).FirstOrDefault();
            ViewBag.EquipTypeDesc = xx.Desc;
            ViewBag.EquipTypeID = xx.ID;
            return View("Index", xxx);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,EquipTypeID,Test,Severity")] EquipTypeTest equipTypeTest,string inspequipid)
        {
            if (ModelState.IsValid)
            {
                equipTypeTest.id = 0;
                _context.Add(equipTypeTest);
                await _context.SaveChangesAsync();
                //                return RedirectToAction(nameof(TestsForEquipType),new {id=equipTypeTest.EquipTypeID });
                return RedirectToAction(nameof(Create),"InspEquipTypeTests" ,new { id = inspequipid });

            }
            return View(equipTypeTest);
        }

        public async Task<IActionResult> Edit(int? id,int? ieetid)
        {
            if (id == null || _context.EquipTypeTest == null)
            {
                return NotFound();
            }
            ViewBag.iettid = ieetid;//InspEquipID
            var equipTypeTest = await _context.EquipTypeTest.FindAsync(id);
            ViewBag.EquipTypeTest = equipTypeTest.Test;

            if (equipTypeTest == null)
            {
                return NotFound();
            }
            return View(equipTypeTest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( [Bind("id,EquipTypeID,Test,Severity")] EquipTypeTest equipTypeTest, int iettid)
        {
            ////if (id != equipTypeTest.id)
            ////{
            ////    return NotFound();
            ////}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipTypeTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipTypeTestExists(equipTypeTest.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(TestsForEquipType), new { id = equipTypeTest.EquipTypeID });
                return RedirectToAction(nameof(Edit), "InspEquipTypeTests", new { id = iettid });
            }
            return View(equipTypeTest);
        }

        // GET: EquipTypeTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EquipTypeTest == null)
            {
                return NotFound();
            }

            var equipTypeTest = await _context.EquipTypeTest
                .FirstOrDefaultAsync(m => m.id == id);
            if (equipTypeTest == null)
            {
                return NotFound();
            }

            return View(equipTypeTest);
        }

        // POST: EquipTypeTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EquipTypeTest == null)
            {
                return Problem("Entity set 'dbcontext.EquipTypeTest' is null.");
            }
            var equipTypeTest = await _context.EquipTypeTest.FindAsync(id);
            var etid = equipTypeTest.EquipTypeID;
            if (equipTypeTest != null)
            {
                _context.EquipTypeTest.Remove(equipTypeTest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TestsForEquipType), new { id = etid });
        }

        private bool EquipTypeTestExists(int id)
        {
          return _context.EquipTypeTest.Any(e => e.id == id);
        }
    }
}
