using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.DTO;
public class PagedMentorResponse
{
    public int PageNumber {get; set;}=1;
    public int PageSize {get; set;}=10;
    public int TotalRecords {get; set;}=25;
    public List<MentorResponse> Data {get; set;}=[];
}