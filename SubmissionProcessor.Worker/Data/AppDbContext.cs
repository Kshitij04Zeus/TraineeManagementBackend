using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using SubmissionProcessor.Worker.Models;

namespace SubmissionProcessor.Worker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Trainee> Trainees
    {
        get; set;
    }

    public DbSet<User> Users
    {
        get; set;
    }

    public DbSet<Mentor> Mentors
    {
        get; set;
    }

    public DbSet<LearningTask> LearningTasks
    {
        get; set;
    }

    public DbSet<TaskAssignment> TaskAssignments
    {
        get; set;
    }

    public DbSet<Submission> Submissions
    {
        get; set;
    }

    public DbSet<Review> Reviews
    {
        get; set;
    }

    public DbSet<SubmissionFile> SubmissionFiles
    {
        get; set;
    }

    public DbSet<ProcessingJob> ProcessingJobs
    {
        get; set;
    }
}