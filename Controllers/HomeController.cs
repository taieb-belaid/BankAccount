using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
namespace BankAccount.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    private bool InSession
    {
        get { return HttpContext.Session.GetInt32("userId") != null; }
    }
    private User LoggedInUser
    {
        get { return _context.Users.Include(u => u.MakeTransfer).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); }
    }

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    //___________________Login_And_Register_____________
    [HttpPost("user/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already exist");
                return View("Index");
            }
            //hashing password
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            //adding to database
            _context.Add(newUser);
            _context.SaveChanges();
            TempData["Message"] = "Registered Successfully";
            return RedirectToAction("Index");
        }
        return View("Index");
    }
    //_______Login_User_____
    [HttpPost("/user/login")]
    public IActionResult Login(Login logUser)
    {
        if (ModelState.IsValid)
        {
            User? userInDb = _context.Users.FirstOrDefault(l => l.Email == logUser.LoginEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LoginEmail", "Incorrect validation");
                return View("Index");
            }
            //comparing password
            PasswordHasher<Login> hasher = new PasswordHasher<Login>();
            var result = hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.LoginPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LoginPassword", "Incorrect validation");
                return View("Index");
            }
            HttpContext.Session.SetInt32("userId", userInDb.UserId);
            return RedirectToAction("Account");
        }
        return View("Index");
    }

    //____________Account_____View________________________________
    [HttpGet("account")]
    public IActionResult Account()
    {
        if (!InSession)
        {
            return View("Index");
        }
        var user = LoggedInUser;
        ViewBag.User = user;
        ViewData["Trans"] = _context.Transactions.OrderByDescending(t => t.CreatedAt).Where(t => t.UserId == user.UserId).ToList();

        return View();
    }
    //_______________Account____Method_________
    [HttpPost("/user/transaction")]
    public IActionResult Transaction(Transaction trans)
    {
        _context.Add(trans);
        _context.SaveChanges();
        return RedirectToAction("Account");
    }
    //

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
