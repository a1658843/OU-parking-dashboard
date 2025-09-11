using Microsoft.AspNetCore.Mvc;

namespace OuStudentPortal.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About() => View(); // we'll add a view next
}
