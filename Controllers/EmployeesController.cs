using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeAPP.Data;
using EmployeeAPP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeAPP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeAPPDBContext _context;

        public EmployeesController(EmployeeAPPDBContext context)
        {
            _context = context;
        }

        // GET: Employees
        // EmployeeViewModel يستخدم لحفظ قائمة الموظفين من قاعدة البيانات
        // ويحتوي على متغير isLoggedIn للتحقق مما إذا كان المستخدم قد انتهى من تسجيل الدخول أم لا
        public async Task<IActionResult> Index(EmployeeViewModel employeeViewModel)
        {
            if (employeeViewModel != null && employeeViewModel.isLoggedin)
            {
               employeeViewModel.Employees = await _context.Employee.ToListAsync();
               return View(employeeViewModel);
            }
            return RedirectToAction(nameof(Login));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id, bool? isLoggedin)
        {
            if (id == null)
            {
                return NotFound();
            }

            // isLoggedin = تسجيل دخول المستخدم أو لا
            if (isLoggedin == null || !(bool)isLoggedin)
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Phone,Password")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id, bool? isLoggedin)
        {
            if (id == null)
            {
                return NotFound();
            }

            // isLoggedin = تسجيل دخول المستخدم أو لا
            if (isLoggedin == null || !(bool)isLoggedin)
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,Password")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // يجب اجتياز EmployeeViewModel وisLoggedin = true حتى لا يقوم المستخدم بتسجيل الدخول مرة أخرى
                EmployeeViewModel empViewModel = new EmployeeViewModel();
                empViewModel.isLoggedin = true;

                // أضف هنا empViewModel كمعلمة
                return RedirectToAction(nameof(Index), empViewModel);
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? isLoggedin)
        {
            if (id == null)
            {
                return NotFound();
            }

            // isLoggedin = تسجيل دخول المستخدم أو لا
            if (isLoggedin == null || !(bool)isLoggedin)
            {
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }

        // طريقة التحقق من البريد الإلكتروني وكلمة المرور في قاعدة البيانات
        private bool EmployeeExistsByEmailPassword(String inputEmail, String inputPassword)
        {
            // تحقق مما إذا كان المستخدم قد أدخل البريد الإلكتروني وكلمة المرور
            if (inputEmail != null && inputPassword != null )
            {
                // هنا استدعاء قاعدة البيانات للتحقق من البريد الإلكتروني وكلمة المرور
                var employee = _context.Employee.
                    First(m => m.Email.Equals(inputEmail) & m.Password.Equals(inputPassword));

                return employee != null;
            }
            return false;
        }

        //login - الطريقة التي أضيفها للتعامل مع تسجيل الدخول
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // login بعد أن ينقر المستخدم على إرسال
        [HttpPost]
        public IActionResult Login(Employee inputEmployee)
        {
               var Email = inputEmployee.Email;
                var Password = inputEmployee.Password;

            // طريقة الاتصال للتحقق مما إذا كان البريد الإلكتروني وكلمة المرور في قاعدة البيانات
            var found = EmployeeExistsByEmailPassword(Email, Password);
                if (found)
                {
                    EmployeeViewModel employeeViewModel = new EmployeeViewModel();
                    employeeViewModel.isLoggedin = true;
                    employeeViewModel.Employees = new List<Employee>();
                    return RedirectToAction(nameof(Index), employeeViewModel);
                }
            // لعرض الخطأ في صفحة تسجيل الدخول إذا لم يتم العثور عليه في قاعدة البيانات
            inputEmployee.LoginValid = false;
            return View(inputEmployee);
        }

    }
}
