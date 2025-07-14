using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using MvcMovie.Models.Entities;
using MvcMovie.Models.Process;

namespace MvcMovie.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
             var model = await _context.Person
        .Select(p => new MvcMovie.Models.Entities.Person
        {
            PersonId = p.PersonId,
            FullName = p.FullName,
            Address = p.Address
        }).ToListAsync();

        return View(model); // truyền đúng kiểu mà View đang chờ
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            var entityPerson = new MvcMovie.Models.Entities.Person
            {
                PersonId = person.PersonId,
                FullName = person.FullName,
                Address = person.Address
            };

            return View(entityPerson);
        }
        public IActionResult Create()
        {
            AutoGenerateId autoGenerateId = new AutoGenerateId();
            var person = _context.Person.OrderByDescending(p => p.PersonId).FirstOrDefault();
            var personId = person == null ? "ST000" : person.PersonId;
            var newPersonId = autoGenerateId.GenerateId(personId);
            var newPerson = new MvcMovie.Models.Entities.Person
            {
                PersonId = newPersonId,
                FullName = string.Empty,
                Address = string.Empty
            };
            return View(newPerson);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FullName,Address")] Models.Entities.Person person)
        {
            if (ModelState.IsValid)
            {
                var newPerson = new MvcMovie.Models.Person
                {
                    PersonId = person.PersonId,
                    FullName = person.FullName,
                    Address = person.Address
                };
                _context.Add(newPerson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);   
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Person == null)
            {
                return NotFound();
            }
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
             // Ánh xạ sang Entities.Person
            var entityPerson = new MvcMovie.Models.Entities.Person
            {
                PersonId = person.PersonId,
                FullName = person.FullName,
                Address = person.Address
            };

            return View(entityPerson); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonId,FullName,Address")] Models.Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                     var updatedPerson = new MvcMovie.Models.Person
                    {
                        PersonId = person.PersonId,
                        FullName = person.FullName,
                        Address = person.Address
                    };
                    _context.Update(updatedPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
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
            return View(person);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Person == null)
            {
                return NotFound();
            }
            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }
            var entityPerson = new MvcMovie.Models.Entities.Person
            {
                PersonId = person.PersonId,
                FullName = person.FullName,
                Address = person.Address
            };

            return View(entityPerson);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Person == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Person' is null.");
            }
            var person = await _context.Person.FindAsync(id);
            if (person != null)
            {
                _context.Person.Remove(person);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool PersonExists(string id)
        {
            return (_context.Person?.Any(e => e.PersonId == id)).GetValueOrDefault();
        } 
        [HttpPost]
        public IActionResult Index(Models.Person ps)
        {
            string strOutput = "Xin chao " + ps.PersonId + "-" + ps.FullName + "-" + ps.Address;
            ViewBag.infoPerson = strOutput;
            return View();
        }
    }
}