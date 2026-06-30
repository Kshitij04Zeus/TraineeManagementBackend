# Trainee Management Application

## Technology Used
The technology/framework used was 
- C#
- MySQL
- Redis
- RabbitMQ
- Docker & Docker Compose

## Project Structure
```text
TraineeManagement.Api
│
├── Controllers
├── Services
├── DTO
├── Models
├── Middleware
├── Migrations
├── Uploads
├── Data
├── logs
└── Settings
 
 
SubmissionProcessor.Worker
│
├── Workers
├── Services
├── DTO
├── Models
├── Data
├── Settings
└── logs
 
TrainingDirectory.Api
│
├── Controllers
└── DTO
```
 
## Features Completed
- Created Web API project 
- Created Health Check API
- Created Trainee model
- Created GET and POST APIS, which fetches details of all trainees, fetches details of trainees based on a specific id, and also  allows creation of a trainee
- Created and used 3 DTOs which were used for creating trainee, updating trainee and for returning trainee details.
- Implemented Service Layer which is imeplemented and registered using dependency injection
- PUT and DELETE APIs have also been made along with the status codes for success, validation and not found scenarios
- Enhanced GET Api which searches trainees by first name, last name, email and tech stack.
- Used EF Core InMemory Database
- Used async and await operations
- Integrated MySQL database setup into the application
- Created User Model
- Implemented Login Api for the user model along with the respective DTOs
- Implemented Password Hashing
- Implemented JWT Token creation
- Protected all the trainee APIs using JWT
- Configured Swagger UI to automatically include JWT Tokens in all API requests
- Enhanced GET API of trainees by adding pagination with additional filters for Search and Status fields
- Configured CORS for React application
- Implemented structured logging
- Created Mentor model with all the DTOs and APIs
- Created Learning Task model with all the DTOs and APIs
- Created Task Assignment, Review and Submission models with all the DTOs and APIs
- Created local file storage and saved its metadata in MySQL. Added size limits and content-type validation. Also created download and deletion APIs
- Implemented REDIS caching thereby ensuring faster retrieval of data
- Logged cache failures and ensured fall back to MYSQL whenever REDIS fails.
- Implemented RabbitMQ durable messaging for asynchronous submission processing
- Created SubmissionProcessor.Worker as a separate Worker Service
- Implemented durable queue and persistent messages
- Added background consumer with manual acknowledgements
- Implemented asynchronous submission processing
- Created TrainingDirectory.Api as a separate internal service
- Implemented typed HttpClient using IHttpClientFactory
- Added timeout, retry and circuit breaker resilience policies
- Implemented correlation ID propagation across API, Worker and internal service
- Implemented Docker Compose for MySQL, Redis and RabbitMQ

## Backend Setup Steps
- Open the project folder.
- Install dotnet 
- Install MySQL server
- Create Trainee Management Database
- Create .env file with the required parameters.
- Restore packages using the command: dotnet restore.
- Apply migrations using the command: dotnet ef database update
- Run the command dotnet run 
- Navigate to swagger endpoint (/swagger) to check the apis and test them out.

## Docker Compose Startup
 
Start all services: docker compose up -d
 
Stop all services: docker compose down
 
Rebuild: docker compose up --build
 
The Docker Compose file starts
 
- MySQL
- Redis
- RabbitMQ
- TraineeManagement.Api
- SubmissionProcessor.Worker
- TrainingDirectory.Api
 
## MySQL Setup Steps
- Install MySql Server
- Login to mysql using command: mysql -u root -p
- Create database of trainee management
 
## EF Core migration commands
- dotnet ef migrations add <Migration Name>
- dotnet ef database update
 
## Login credentials for testing
```json
{
  "username": "admin",
  "password": "admin123"
}
```
 
## JWT usage instructions
- navigate to /swagger
- click authorize button
- Enter: Bearer <token>
- Click authorize
- Check protected apis

## Configuration
- .env files
- Docker Compose environment variables

## API Endpoints:
- GET /api/health: Returns application health status
- GET /api/trainees: Returns all trainees
- GET /api/trainees?search=amit: Searches trainees by first name, last name, email and tech stack
- GET /api/trainees/{id}: Get trainee by Id
- POST /api/trainees: Create a trainee
- PUT /api/trainees/{id}: Update a trainee
- DELETE /api/trainees/{id}: Delete a trainee
- POST /api/auth/login: Used for Logging in a user

- POST /api/auth/login: Returns JWT Token along with login minutes on correct credentials.
 
- GET /api/mentors  :  Returns list of mentors. Also contains search param to filter by required mentors.
- GET /api/mentors/{id} : Get specific mentor by id
- POST /api/mentors : Used for creating new mentor
- PUT  /api/mentors/{id} : Update mentor contents of a particular ID.
- DELETE /api/mentors/{id} : Update mentor contents of a particular ID.
 
- GET    /api/learning-tasks : Returns list of Learning Tasks. Also contains search param to filter by required Tasks.
- GET    /api/learning-tasks/{id} : Get specific Tasks by id
- POST   /api/learning-tasks  : Used for creating new Tasks
- PUT    /api/learning-tasks/{id} : Update Tasks contents of a particular ID.
- DELETE /api/learning-tasks/{id} : Delete a Tasks by specified ID

- GET /api/task-assignments : Gets all the Task assignments from the database
- GET /api/task-assignments/{id} : Gets a particular Task assignment with the id parameter
- POST /api/task-assignments : Adds a new Task assignment to the database
- PUT /api/task-assignments/{id}/status : Changes status of a particular Task assignment using id parameter

- POST /api/submissions : Adds a new Submission to the database
- GET /api/submissions : Gets all the Submissions from the database
- GET /api/submissions/{id} : Gets a particular Submission with the id parameter

- POST /api/submissions/{submissionId}/files : Upload a submission file to the local storage
- GET /api/submission-files/{id}/download : Download the submission file
- DELETE /api/submission-files/{id} : Delete a submission file 

 
- POST /api/submissions/{submissionId}/files
- GET /api/submission-files/{id}/download
- DELETE /api/submission-files/{id}

- GET /api/training-directory/assignments/{assignmentId}/processing-profile

## Workflow Diagram
<img width="2428" height="1228" alt="Image" src="https://github.com/user-attachments/assets/e065443c-7faf-42f6-b9dd-4ed572c46059" />

## Design Decisions
 
### Redis
- Cache-aside pattern
- Absolute TTL: 60 Minutes
- Sliding Expiration: 10 minutes
- Automatic database fallback
 
### RabbitMQ
- Durable Queue
- Persistent Messages
- Manual Acknowledgements
- Prefetch Count = 1
 
### Retry Strategy
- HTTP Client Resilience
- Timeout Handling
- Retry for Transient Failures
 
### Idempotency
- MessageId
- CorrelationId
These identifiers help prevent duplicate processing.

### Failure Behaviour
 
MySQL Unavailable
 
- Readiness check reports Unhealthy.
- API returns database-related errors.
 
Redis Unavailable
 
- Application falls back to MySQL.
- Cache failures are logged.
 
RabbitMQ Unavailable
 
- Publish operation logs failure.
- Worker remains unavailable until RabbitMQ recovers.
 
Worker Failure
 
- Message is negatively acknowledged (NACK).
- Processing is retried according to RabbitMQ configuration.

## Sample Request JSON
```json
{
  "firstName": "Jason",
  "lastName": "Bourne",
  "email": "jbourne@gmail.com",
  "techStack": "Python",
  "status": "Active"
}
```

## Sample Response JSON	
```json
{
  "id": 1,
  "firstName": "Jason",
  "lastName": "Bourne",
  "email": "jbourne@gmail.com",
  "techStack": "Python",
  "status": Active,
  "createdDate": "2026-06-08T12:46:39.9723059+00:00",
  "updatedDate": "2026-06-08T12:46:39.9723105+00:00"
}
```

## Known Limitations 
- No email verification 
- Password reset functionality not present
- No cloud file storage integration

## Challenges Faced
Some of the technical/logical challenges faced were:
- Dotnet installation of packages (particularly Swagger) due to proxy server difficulties

## Next Improvement Areas
- Integration with frontend
