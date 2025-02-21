using Microsoft.AspNetCore.Mvc;
using FellowOakDicom;
// using System.Net;
using System.IO.Compression;
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    private readonly string _zipFolder = Path.Combine(Directory.GetCurrentDirectory(), "zip");
    private readonly string _extractedZip=Path.Combine(Directory.GetCurrentDirectory(),"extracted");
    public FileController(){
        if(!Directory.Exists(_extractedZip)) Directory.CreateDirectory(_extractedZip);
    }
    [HttpGet("uploads")]
    public IActionResult Get()
    {
        try
        {
            List<string> formattedFiles = new List<string>();
            String[] files = Directory.GetFiles(_uploadsFolder);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                formattedFiles.Add(fileName);
            }
            return Ok(new { message = ResponseModel.Success, files = formattedFiles });
        }
        catch (System.Exception e)
        {
            return StatusCode(500, new { message = ResponseModel.InternalServerError, error = e.Message });
        }
    }
    [HttpPost("uploads")]
    public async Task<IActionResult> Post([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { message = ResponseModel.BadRequest });
        }

        try
        {
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            var uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                Console.WriteLine(file.FileName);
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(_uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (!DicomFile.HasValidHeader(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return BadRequest(new { message = ResponseModel.BadRequest });
                }

                uploadedFiles.Add(fileName);
            }
            return Redirect("/");

            // return Ok(new { message = ResponseModel.Success, files = uploadedFiles });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ResponseModel.InternalServerError, error = ex.Message });
        }
    }
    [HttpDelete("uploads/{fileName}")]
    public IActionResult Delete(string fileName)
    {
        try
        {
            string pathName = Path.Combine(_uploadsFolder, fileName);
            if (!System.IO.File.Exists(pathName))
            {
                return NotFound(new { message = ResponseModel.NotFound });
            }
            System.IO.File.Delete(pathName);
            return NoContent();
        }
        catch (System.Exception e)
        {
            return StatusCode(500, new { message = ResponseModel.InternalServerError, error = e.Message });
        }
    }
    //does not handle nested directories
    [HttpPost("uploads/zip")]
    public async Task<IActionResult> UploadDcmFromZip([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { message = ResponseModel.BadRequest });
        }
        if (!Directory.Exists(_zipFolder))
        {
            Directory.CreateDirectory(_zipFolder);
        }
        try
        {
            foreach (var file in files)
            {
                Console.WriteLine(file.FileName);
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(_zipFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                ZipFile.ExtractToDirectory(filePath,_uploadsFolder);
                
            }
            return Redirect("/");
        }
        catch (System.Exception e)
        {
            return StatusCode(500, new { message = ResponseModel.InternalServerError, error = e.Message });
        }
    }


}