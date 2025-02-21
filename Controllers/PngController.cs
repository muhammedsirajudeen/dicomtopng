using FellowOakDicom.Imaging;
using GroupDocs.Conversion;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("/api/[controller]")]
public class PngController:ControllerBase{
    private readonly string _uploadsFolder=Path.Combine(Directory.GetCurrentDirectory(),"uploads");
    private readonly string _pngFolder=Path.Combine(Directory.GetCurrentDirectory(),"png");
    [HttpGet("{pathName}")]

    public IActionResult Get(string pathName){
        try
        {
            System.Console.WriteLine(pathName);
            if(!System.IO.File.Exists(Path.Combine(_uploadsFolder,pathName))){
                return NotFound(new {message=ResponseModel.NotFound});
            }
            string filePathDcm=Path.Combine(_uploadsFolder,pathName);
            if(!Directory.Exists(_pngFolder)){
                Directory.CreateDirectory(_pngFolder);
            }
            string[] fileName=pathName.Split('.');
            string withoutExtension=fileName[0];
            FluentConverter.Load(filePathDcm).ConvertTo(Path.Combine(_pngFolder,$"{withoutExtension}.png")).Convert();
            return Redirect("/png");
            // return Ok(new {message=ResponseModel.Success});
        }
        catch (System.Exception e)
        {
            return StatusCode(500,new {message=ResponseModel.InternalServerError,error=e.Message});
        }
    }
    [HttpGet]
    public IActionResult GetPngs(){
        try
        {
            List<string> extractedNames=new List<string>();
            string[] files=Directory.GetFiles(_pngFolder);
            System.Console.WriteLine(files[0]);
            foreach(string file in files){
                extractedNames.Add(Path.GetFileName(file));
            }
            return Ok(new {message=ResponseModel.Success,files=extractedNames});
        }   
        catch (System.Exception e)
        {
            return StatusCode(500,new {message=ResponseModel.InternalServerError,error=e.Message});
        }
    }
    [HttpDelete("{fileName}")]
    public IActionResult Delete(string fileName){
        try
        {
            string pathName=Path.Combine(_pngFolder,fileName);
            if(!System.IO.File.Exists(pathName)){
                return NotFound(new {message=ResponseModel.NotFound});
            }
            System.IO.File.Delete(pathName);
            return NoContent();
        }
        catch (System.Exception e)
        {
            return StatusCode(500,new {message=ResponseModel.InternalServerError,error=e.Message});
        }
    }
}