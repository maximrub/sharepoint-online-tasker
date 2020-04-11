using System.Threading.Tasks;

namespace SharePointOnlineTasker.Interfaces
{
    public interface IAuthenticator
    {
        Task<string> AuthorizeAsync();
    }
}
