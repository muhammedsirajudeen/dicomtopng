using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller{
    [Route("")]
    public IActionResult Index(){
        
        return View();
    }

    [Route("/png")]
    public IActionResult Png(){
        return View();
    }
}