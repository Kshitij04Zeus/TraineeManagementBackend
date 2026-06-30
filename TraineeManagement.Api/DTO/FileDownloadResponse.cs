using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class FileDownloadResponse
{
    public Stream Stream {get; set;}=null!;
    public string ContentType {get; set;}="";
    public string FileName {get; set;}="";
}