using System.Threading.Tasks;

namespace SharePointOnlineTasker.Interfaces
{
    public interface ITasksRunner
    {
        Task ExecuteAsync();
    }
}