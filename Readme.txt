This project is a .NET 8 MVC Web App with simple Login to view Employee List. 
Application built in Visual Studio 2022 and uses SQL Server Express as database.

DATABASE:

SQL Server Express
Database: EMPDB
Tables: Employee
Web application .NET 8 MVC use Microsoft Entity Framework package. Entity Framework takes care of creating the DB Context and creates Controllers with CRUD codes.

HOW TO CREATE THE APPLICATION:

1. Create database EMPDB and table Employee in SqlServer.

2. Create ASP.NET Core Web App (Model-View-Controller).  No Authorization (so not use Microsoft Identity).

3. Create model Models/Employee.cs file.

4. Add Controller from Employee.cs model. Visual Studio create Contollers/EmployeeControler.cs file.

5. Add Views from EmployeeController. Visual Studio create files
    - Views/Employees/Create.cshtml  
    - Views/Employees/Delete.cshtml 
	- Views/Employees/Edit.cshtml
	- Views/Employees/Index.cshtml

6. Change database connection in appsettings.json.

```
  "ConnectionStrings": {
    "EmployeeAPPDBContext": "Server=localdb\\SQLEXPRESS;Database=EMPDB;TrustServerCertificate=true;Trusted_Connection=True;MultipleActiveResultSets=true"
```

7. For user login, create 
    - Views/Employees/Login.cshtml
    
8. User must login to see Employees tab
   a. create new model Models/EmployeeViewModel.cs file.
   b. Add 3 methods in Controllers/EmployeesController.cs
```
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
 ```

  c. Make change Views/Employees/Index.html to use EmployeeViewModel 
```       
        <!-- model IEnumerable<EmployeeAPP.Models.Employee> -->
        @model EmployeeAPP.Models.EmployeeViewModel
```       
  d. also add if statement before for loop

```
          @if (Model.Employees != null && Model.Employees.Count > 0)
```
  e. also add isLoggedin as another parameter
```
          <!-- asp-route يعني المعلمة
               يعني asp-route-id أنني أقوم بتمرير المعرف كمعلمة وتستقبله وحدة التحكم كمعلمة إدخال
               asp-route-isLoggedin يعني أنني أمرر كمعلمة وتستقبلها وحدة التحكم كمعلمة إدخال
          -->
          <a asp-action="Edit" asp-route-id="@item.Id" asp-route-isLoggedin="@Model.isLoggedin">Edit</a> |
          <a asp-action="Details" asp-route-id="@item.Id" asp-route-isLoggedin="@Model.isLoggedin">Details</a> |
          <a asp-action="Delete" asp-route-id="@item.Id" asp-route-isLoggedin="@Model.isLoggedin">Delete</a>
```

 
9. Not allow user to use Details, Edit and Delete if not login to methods to add isLoggedin in input

 ```
 public async Task<IActionResult> Details(int? id, bool? isLoggedin)
 public async Task<IActionResult> Edit(int? id, bool? isLoggedin)
 public async Task<IActionResult> Delete(int? id, bool? isLoggedin)
 ```

 and inside the Details, Edit and Delete methods, add check if logged in

```
            // isLoggedin = تسجيل دخول المستخدم أو لا
            if (isLoggedin == null || !(bool)isLoggedin)
            {
                return RedirectToAction(nameof(Index));
            }
```

DATABASE ACCESS
The Microsoft Entity Framework "using Microsoft.EntityFrameworkCore;" in Controllers to create (insert), read, delete and update Employee record from database.

For Get (SELECT) list of Employees - method Index()
===================================================
Entity Framework use ```await _context.Employee.ToListAsync();```

Example -
```
public async Task<IActionResult> Index(EmployeeViewModel employeeViewModel) {
...
  employeeViewModel.Employees = await _context.Employee.ToListAsync();
...
}
```

For GET (SELECT) one Employee Record - method Details()
=======================================================
Entity Framework use ```_context.Employee.FirstOrDefaultAsync(m => m.Id == id);```

Example -
```
public async Task<IActionResult> Details(int? id, bool? isLoggedin) {
...
 var employee = await _context.Employee
     .FirstOrDefaultAsync(m => m.Id == id);
...
}
```

FOR Add (Insert) Employee - method Create()
===========================================
Entity Framework use ``` _context.Add(employee);  await _context.SaveChangesAsync();```

Example -
```
 public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Phone,Password")] Employee employee)
 {
  ...    
         _context.Add(employee);
         await _context.SaveChangesAsync();
 }
```

For Change (UPDATe) Employee - method 
Entity Framework use ```_context.Update(employee); await _context.SaveChangesAsync(); ```

Example -
```
public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,Password")] Employee employee)
{
    _context.Update(employee);
     await _context.SaveChangesAsync();
}
```

For delete (DELETE) Employee - method Delete()
==============================================
Entity Framework use ```  _context.Employee.Remove(employee); ```

Example -
```
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
}
```

For Find (SEARCH) Employee - this I wrote method EmployeeExistsByEmailPassword()
================================================================================

```
private bool EmployeeExistsByEmailPassword(String inputEmail, String inputPassword)
{
...
   var employee = _context.Employee.
            First(m => m.Email.Equals(inputEmail) & m.Password.Equals(inputPassword));
...
}
```




