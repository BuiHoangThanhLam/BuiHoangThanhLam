using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
namespace MVC.Controllers
{
    public class HelloWorldController : Controller
    { 
        // GET: /HelloWorld/
        public IActionResult Index()
        {
            return View();
        } 
       
         public IActionResult Person()
        {
            return View();
        } 
         public IActionResult Employee()
        {
            return View();
        } 
         // GET: /HelloWodonetrld/Welcome/ 
        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}
