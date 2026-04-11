using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Controllers;

public class SitePagesController : Controller
{
    private readonly Hshop2023Context _db;

    public SitePagesController(Hshop2023Context db)
    {
        _db = db;
    }

    [HttpGet("/p/{slug}")]
    public IActionResult Detail(string slug)
    {
        var page = _db.SitePages.SingleOrDefault(x => x.Slug == slug);
        if (page == null) return NotFound();

        return View(page);
    }
}