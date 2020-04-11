using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using SharePointOnlineTasker.Interfaces;
using SharePointOnlineTasker.Interfaces.DriveTasks;

namespace SharePointOnlineTasker.Services
{
    public class TasksRunner : ITasksRunner
    {
        private readonly ILogger<TasksRunner> _logger;
        private readonly IAuthenticationProvider _authenticator;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly IGoogleDriveFileTask _googleDriveFileTask;

        public TasksRunner(ILogger<TasksRunner> logger, IAuthenticationProvider authenticator, IConfiguration configuration, IMemoryCache cache, IGoogleDriveFileTask googleDriveFileTask)
        {
            _logger = logger;
            _authenticator = authenticator;
            _configuration = configuration;
            _cache = cache;
            _googleDriveFileTask = googleDriveFileTask;
        }

        public async Task ExecuteAsync()
        {
            GraphServiceClient graphClient = new GraphServiceClient(_authenticator);

            var groups = await graphClient.Groups.Request().GetAsync();
            Group group = groups.FirstOrDefault(g => g.DisplayName.Equals("Family", StringComparison.InvariantCultureIgnoreCase));
            // Make a request
            var drives = await graphServiceClient.Groups[group.Id].Drives
                .Request()
                .GetAsync();
        }
    }
}