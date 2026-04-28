using CvApp.Core.Interface;
using CvApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CvApp.Controllers;


[Route("candidate")]
public class CandidateController : Controller
{
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }


    [HttpGet("upload")]
    public async Task<IActionResult> UploadForm()
    {
        return View("Upload");
    }

    [HttpGet("confirmation")]
    public IActionResult Confirmation()
    {
        return View();
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetCandidatesBySkills(string skills)
    {
        if (string.IsNullOrWhiteSpace(skills))
            return View("Search");

        try
        {
            var candidate = await _candidateService.GetCandidatesBySkillsAsync(skills);
            return View("Search", candidate);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ModelState.AddModelError("", "Inga kandidater hittades");
            return View("Search");
        }

    }


    [HttpPost("upload")]
    public async Task<IActionResult> Upload(UploadViewModel model)
    {
        try
        {
            await _candidateService.UploadCandidateAsync(model);
            return RedirectToAction("Confirmation"); // skickar användaren till confiration sida


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ModelState.AddModelError("", "Något gick fel vid uppladdningen, försök igen.");
            return View("Upload", model); // detta skickas tillbaks till Upload.cshtml sidan och kör if satsen

        }

    }







}