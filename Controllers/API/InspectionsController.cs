﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using RoofSafety.Data;

namespace RSSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class inspectionsController : ControllerBase
    {
        private readonly dbcontext _context;

        public inspectionsController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/inspection
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InpsectionView>>> Getinspection()
        {
            if (_context.Inspection == null)
            {
                return NotFound();
            }
            return await (from insp in _context.Inspection join bd in _context.Building on insp.BuildingID equals bd.id join emp in _context.Employee on insp.InspectorID equals emp.id join cli in _context.Client on bd.ClientID equals cli.id select new InpsectionView { Areas=insp.Areas, id=insp.id, TestingInstruments=insp.TestingInstruments, InspDate=insp.InspectionDate, Address=bd.Address, Inspector=emp.Given + " "+emp.Surname , ClientName=cli.name }).ToListAsync();
        }

        // GET: api/inspection/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inspection>> Getinspection(int id)
        {
            if (_context.Inspection == null)
            {
                return NotFound();
            }
            var inspection = await _context.Inspection.FindAsync(id);

            if (inspection == null)
            {
                return NotFound();
            }

            return inspection;
        }

        // PUT: api/inspection/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putinspection(int id, Inspection inspection)
        {
            if (id != inspection.id)
            {
                return BadRequest();
            }

            _context.Entry(inspection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!inspectionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/inspection
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inspection>> Postinspection(Inspection inspection)
        {
            if (_context.Inspection == null)
            {
                return Problem("Entity set 'dbContext.inspection'  is null.");
            }
            _context.Inspection.Add(inspection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getinspection", new { id = inspection.id }, inspection);
        }

        // DELETE: api/inspection/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteinspection(int id)
        {
            if (_context.Inspection == null)
            {
                return NotFound();
            }
            var inspection = await _context.Inspection.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }

            _context.Inspection.Remove(inspection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool inspectionExists(int id)
        {
            return (_context.Inspection?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
