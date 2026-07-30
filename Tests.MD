SubmissionProcessing.Worker:
- Transient Failures requeues the message.
- Permanent Failures puts the message in the dead letter queue
- If trainee profile isn't fetched from TrainingDirectory.Api then fallback profile should be loaded.
- Successfull processing results in the job being marked as completed.
- Duplicated message is marked as completed (Idempotency is ensured)
- FileNotFoundException should result in the job being marked as failed.

TraineeManagement.Api:
- Valid username,password returns JWT
- Invalied username and password should throw UnauthorizedAccessException
- Files with size above the specified limit should be rejected with a PayloadTooLargeException
- Files with disallowed extensions must be rejected
- A trainee should not be able to download another trainee's files.
- In case of redis failure, the latest data must be fetched from the SQL Database rather than providing stale data.