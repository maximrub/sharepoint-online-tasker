using System.Threading.Tasks;
using SharePointOnlineTasker.Entities;

namespace SharePointOnlineTasker.Interfaces.DriveTasks
{
    public interface IDriveFileTask
    {
        string Name { get; }
        Task ExecuteAsync(DriveFile driveFile);
    }
}