using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using MvcMovie.Models.Entities;
using MvcMovie.Models.Process;
using Newtonsoft.Json.Converters;
using OfficeOpenXml;
using X.PagedList;
using X.PagedList.Extensions;

namespace MvcMovie.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        private ExcelProcess _excelProcess = new ExcelProcess();

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    ModelState.AddModelError("", "Please choose excel file to upload!");
                }
                else
                {
                    var fileName = DateTime.Now.ToShortTimeString() + fileExtension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excels", fileName);
                    var fileLocation = new FileInfo(filePath).ToString();
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        // đọc dữ liệu từ file Excel và đổ vào DataTable
                        var dt = _excelProcess.ExcelToDataTable(fileLocation);
                        // sử dụng vòng lặp for để đọc dữ liệu từ dt
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            // tạo đối tượng Person mới
                            var ps = new Models.Person();
                            // gán giá trị cho các thuộc tính
                            ps.PersonId = dt.Rows[i][0].ToString();    // cột 0: Mã nhân viên
                            ps.FullName = dt.Rows[i][1].ToString();    // cột 1: Họ tên
                            ps.Address = dt.Rows[i][2].ToString();     // cột 2: Địa chỉ
                            // thêm đối tượng vào context
                            _context.Add(ps);
                        }
                        // lưu các thay đổi vào cơ sở dữ liệu
                        await _context.SaveChangesAsync();
                        // chuyển hướng về trang Index
                        return RedirectToAction(nameof(Index));

                    }
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? PageSize)
        {
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="3", Text="3" },
                new SelectListItem() { Value="5", Text="5" },
                new SelectListItem() { Value="10", Text="10" },
                new SelectListItem() { Value="15", Text="15" },
                new SelectListItem() { Value="25", Text="25" },
                new SelectListItem() { Value="50", Text="50" },
            };
            int pagesize = (PageSize ?? 3);
            ViewBag.psize = pagesize;
            var model = _context.Person
            .Select(p => new MvcMovie.Models.Entities.Person
            {
            PersonId = p.PersonId,
            FullName = p.FullName,
            Address = p.Address
            })
            .ToList().ToPagedList(page ?? 1, pagesize);
            return View(model);
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
        
        public IActionResult Download()
        {
            // Name the file when downloading
            var fileName = "YourFileName" + ".xlsx";
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                // add some text to cell A1
                worksheet.Cells["A1"].Value = "PersonID";
                worksheet.Cells["B1"].Value = "FullName";
                worksheet.Cells["C1"].Value = "Address";

                // get all Person
                var personList = _context.Person.ToList();

                // fill data to worksheet
                worksheet.Cells["A2"].LoadFromCollection(personList);

                var stream = new MemoryStream(excelPackage.GetAsByteArray());

                // download file
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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
       
    }
}