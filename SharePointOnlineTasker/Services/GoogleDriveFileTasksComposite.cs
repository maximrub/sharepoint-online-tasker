using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharePointOnlineTasker.Entities;
using SharePointOnlineTasker.Interfaces.DriveTasks;

namespace SharePointOnlineTasker.Services
{
    public class GoogleDriveFileTasksComposite : IGoogleDriveFileTask
    {
        private readonly IEnumerable<IGoogleDriveFileTask> _googleDriveFileTasks;
        private readonly ILogger<GoogleDriveFileTasksComposite> _logger;

        public GoogleDriveFileTasksComposite(IEnumerable<IGoogleDriveFileTask> googleDriveFileTasks, ILogger<GoogleDriveFileTasksComposite> logger)
        {
            _googleDriveFileTasks = googleDriveFileTasks;
            _logger = logger;
        }

        public string Name => "GoogleDriveFileTasksComposite";

        public async Task ExecuteAsync(GoogleDriveFile googleDriveFile)
        {
            foreach (IGoogleDriveFileTask task in _googleDriveFileTasks)
            {
                try
                {
                    _logger.LogInformation($"Starting executing '{task.Name}' on file '{googleDriveFile.FullName}'");
                    await task.ExecuteAsync(googleDriveFile);
                    _logger.LogInformation($"Completed executing '{task.Name}' on file '{googleDriveFile.FullName}'");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while executing '{task.Name}' on file '{googleDriveFile.FullName}'");
                }
                
            }
        }
    }
}