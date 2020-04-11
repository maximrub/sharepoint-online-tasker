using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharePointOnlineTasker.Entities;
using SharePointOnlineTasker.Interfaces.DriveTasks;

namespace SharePointOnlineTasker.Services
{
    public class DriveFileTasksComposite : IDriveFileTask
    {
        private readonly IEnumerable<IDriveFileTask> _googleDriveFileTasks;
        private readonly ILogger<DriveFileTasksComposite> _logger;

        public DriveFileTasksComposite(IEnumerable<IDriveFileTask> googleDriveFileTasks, ILogger<DriveFileTasksComposite> logger)
        {
            _googleDriveFileTasks = googleDriveFileTasks;
            _logger = logger;
        }

        public string Name => "GoogleDriveFileTasksComposite";

        public async Task ExecuteAsync(DriveFile driveFile)
        {
            foreach (IDriveFileTask task in _googleDriveFileTasks)
            {
                try
                {
                    _logger.LogInformation($"Starting executing '{task.Name}' on file '{driveFile.FullName}'");
                    await task.ExecuteAsync(driveFile);
                    _logger.LogInformation($"Completed executing '{task.Name}' on file '{driveFile.FullName}'");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while executing '{task.Name}' on file '{driveFile.FullName}'");
                }
                
            }
        }
    }
}