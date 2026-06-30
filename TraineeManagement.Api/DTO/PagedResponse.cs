using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.DTO;
public class PagedResponse
{
    public int PageNumber {get; set;}=1;
    public int PageSize {get; set;}=10;
    public int TotalRecords {get; set;}=25;
    public List<TraineeResponse> Data {get; set;}=[];
}