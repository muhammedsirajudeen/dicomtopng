using Microsoft.AspNetCore.Mvc;
using FellowOakDicom;
[ApiController]
[Route("api/[controller]")]
public class FileController:ControllerBase{
    private readonly string _uploadsFolder=Path.Combine(Directory.GetCurrentDirectory(),"uploads");
    [HttpGet("upload")]
    public IActionResult Get(){
        try
        {
            List<string> formattedFiles=new List<string>();
            String[] files=Directory.GetFiles(_uploadsFolder);
            for(int i=0;i<files.Length;i++){
                string fileName=Path.GetFileName(files[i]);
                formattedFiles.Add(fileName);
            }
            return Ok(new {message=ResponseModel.Success,files=formattedFiles});
        }
        catch (System.Exception e)
        {
            return StatusCode(500,new {message=ResponseModel.InternalServerError,error=e.Message});
        }
    }
    [HttpPost("upload")]
    public async Task<IActionResult> Post([FromForm] IFormFile files){
        if(files==null){
            return BadRequest(new {message=ResponseModel.BadRequest});
        }
        try{
        Console.WriteLine(files.FileName);
        if(!Directory.Exists(_uploadsFolder)){
            Directory.CreateDirectory(_uploadsFolder);
        }
        
        var fileName=$"{Guid.NewGuid()}_{files.FileName}";
        var filePath=Path.Combine(_uploadsFolder,fileName);
        
        using (var stream=new FileStream(filePath,FileMode.Create)){
            await files.CopyToAsync(stream);
        }
        if(!DicomFile.HasValidHeader(Path.Combine(_uploadsFolder,fileName))){
            return BadRequest(new {message=ResponseModel.BadRequest});
        }
        return Ok(new {message=ResponseModel.Success});
        }
        catch(Exception ex){
            return StatusCode(500,new {message=ResponseModel.InternalServerError,error=ex.Message});
        }

    }
    
}