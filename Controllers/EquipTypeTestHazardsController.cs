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
    public class EquipTypeTestHazardsController : Controller
    {
        private readonly dbcontext _context;

        public EquipTypeTestHazardsController(dbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
           return View(await _context.EquipTypeTestHazards.Include(i=>i.Hazard).ToListAsync());
        }

        public async Task<IActionResult> HazardsForEquipTypeTests(int? id)
        {
            try
            {
                ViewBag.EquipTypeTest = _context.EquipTypeTest.Find(id).Test;
                ViewBag.EquipTypeTestID = id;
                var xx = await _context.EquipTypeTestHazards.Where(i => i.EquipTypeTestID == id).Include(i => i.Hazard).ToListAsync();
                return View("Index", xx);
            }
            catch (Exception ex) { 
                var mm = ex; }
            return View();
        }


        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Hazard == null)
        //    {
        //        return NotFound();
        //    }

        //    var hazard = await _context.Hazard
        //        .FirstOrDefaultAsync(m => m.id == id);
        //    if (hazard == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hazard);
        //}

                public IActionResult Create(int? id)
                {
                    EquipTypeTestHazards ret = new EquipTypeTestHazards();
                    ret.EquipTypeTestID = id.Value;
//                    ViewBag.HazardID = (from xx in _context.Hazard select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Detail}).ToList();
                    return View(ret);
                }
        //

        [HttpPost]
//        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
                var xx=_context.EquipTypeTestHazards.Find(id);
            if (xx != null)
            {
                _context.EquipTypeTestHazards.Remove(xx);
                await _context.SaveChangesAsync();
                return Json(new { error = "Success" });
            }
            return Json(new { error = "Record not found" });

        }

        public class IDDesc
        {
            public int ID { get; set; }
            public string Desc { get; set; }
        }

        public JsonResult HazardSearch(string searchString)
        {
            var taxon = (from tx in _context.Hazard
                         where (tx.Detail).Contains(searchString)
                         select tx.id.ToString() + "-" + tx.Detail 
                          ).Take(8).ToList();
            taxon.Add("0-"+searchString + "(add new)");

            return Json(new { results = taxon.OrderBy(i => i) });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([Bind("id,HazardID,EquipTypeTestID,Haz")] EquipTypeTestHazards equipTypeTestHazards)
        {
            
//            if (ModelState.IsValid)
            {
                if (equipTypeTestHazards.Haz.Contains("(add new)"))
                {
                    Hazard hz = new Hazard();
                    hz.Detail = equipTypeTestHazards.Haz.Replace("(add new)", "");
                    _context.Hazard.Add(hz);
                    equipTypeTestHazards.HazardID = hz.id;
                }
                //else
                //{
                //    string[] tuple = equipTypeTestHazards.Haz.Split(new char[] { Char.Parse("-") }, StringSplitOptions.RemoveEmptyEntries);
                //    equipTypeTestHazards.HazardID = Convert.ToInt32(tuple[0]);
                //}
                _context.Add(equipTypeTestHazards);
                try
                {
                    equipTypeTestHazards.id = 0;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
                return Json(new { error = "Success" });
            }
            return Json(new { error = "Not valid" });
        }


        /*        public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null || _context.EquipType == null)
                    {
                        return NotFound();
                    }

                    var equipType = await _context.EquipType.FindAsync(id);
                    if (equipType == null)
                    {
                        return NotFound();
                    }
                    return View(equipType);
                }

                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("id,EquipTypeDesc")] EquipType equipType)
                {
                    if (id != equipType.id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(equipType);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!EquipTypeExists(equipType.id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    return View(equipType);
                }

                public async Task<IActionResult> Delete(int? id)
                {
                    if (id == null || _context.EquipType == null)
                    {
                        return NotFound();
                    }

                    var equipType = await _context.EquipType
                        .FirstOrDefaultAsync(m => m.id == id);
                    if (equipType == null)
                    {
                        return NotFound();
                    }

                    return View(equipType);
                }

                // POST: EquipTypes/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    if (_context.EquipType == null)
                    {
                        return Problem("Entity set 'dbcontext.EquipType'  is null.");
                    }
                    var equipType = await _context.EquipType.FindAsync(id);
                    if (equipType != null)
                    {
                        _context.EquipType.Remove(equipType);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                private bool EquipTypeExists(int id)
                {
                  return _context.EquipType.Any(e => e.id == id);
                }
        */
    }
}
