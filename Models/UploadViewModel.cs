namespace CvApp.Models;
// Detta är som Dto:s
public class UploadViewModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IFormFile Cv { get; set; }
    public string Skills { get; set; }
}