namespace TraineeDirectory.Api.DTO;
public class DirectoryTraineeProfileResponse
{
    public int TraineeId {get; set;}
    public string FullName {get; set;}="";
    public string Email {get; set;}="";
    public string TechStack {get; set;}="";
    public string Status{get; set;}="";
    public string ProfileNote {get; set;}="";
}