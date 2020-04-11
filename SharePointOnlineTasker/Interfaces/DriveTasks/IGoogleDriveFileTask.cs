using System.Threading.Tasks;
using SharePointOnlineTasker.Entities;

namespace SharePointOnlineTasker.Interfaces.DriveTasks
{
    public interface IGoogleDriveFileTask
    {
        string Name { get; }
        Task ExecuteAsync(GoogleDriveFile googleDriveFile);
    }
}