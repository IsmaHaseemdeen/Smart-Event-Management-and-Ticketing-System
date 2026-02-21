using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventTicketingSystem.Data;
using EventTicketingSystem.Models;

namespace EventTicketingSystem.Controllers
{
    public class EventSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EventSessions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EventSessions.Include(e => e.Event);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EventSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EventSessions == null)
            {
                return NotFound();
            }

            var eventSession = await _context.EventSessions
                .Include(e => e.Event)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (eventSession == null)
            {
                return NotFound();
            }

            return View(eventSession);
        }

        // GET: EventSessions/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Status");
            return View();
        }

        // POST: EventSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,EventId,SessionDate,StartDateTime,EndDateTime,SessionStatus,Notes")] EventSession eventSession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Status", eventSession.EventId);
            return View(eventSession);
        }

        // GET: EventSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EventSessions == null)
            {
                return NotFound();
            }

            var eventSession = await _context.EventSessions.FindAsync(id);
            if (eventSession == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Status", eventSession.EventId);
            return View(eventSession);
        }

        // POST: EventSessions/Edit/5
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,EventId,SessionDate,StartDateTime,EndDateTime,SessionStatus,Notes")] EventSession eventSession)
        {
            if (id != eventSession.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventSessionExists(eventSession.SessionId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Status", eventSession.EventId);
            return View(eventSession);
        }

        // GET: EventSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EventSessions == null)
            {
                return NotFound();
            }

            var eventSession = await _context.EventSessions
                .Include(e => e.Event)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (eventSession == null)
            {
                return NotFound();
            }

            return View(eventSession);
        }

        // POST: EventSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EventSessions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EventSessions'  is null.");
            }
            var eventSession = await _context.EventSessions.FindAsync(id);
            if (eventSession != null)
            {
                _context.EventSessions.Remove(eventSession);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventSessionExists(int id)
        {
          return (_context.EventSessions?.Any(e => e.SessionId == id)).GetValueOrDefault();
        }
    }
}
