using AspNotes.Core.Section;
using AspNotes.Web.Models;
using AspNotes.Web.Models.Sections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNotes.Web.Controllers;

public class HomeController(
    IConfiguration configuration,
    IOptions<AllNotesSection> allNotesSection,
    ISectionsService sectionsService
    ) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var applicationTitle = configuration["ApplicationTitle"] ?? "Notes";
        ViewData["Title"] = applicationTitle;

        var sections = await sectionsService.GetSections();

        var preloadedState = new
        {
            Sections = new {
                AllNotesSection = allNotesSection.Value,
                List = sections.Select(x => new SectionItemResponse(x)).ToList()
            }
        };

        ViewData["PreloadedState"] = preloadedState;

        return View();
    }
}
