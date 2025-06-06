using System.Threading.Tasks;

namespace DuqSchut.Services
{
    /**
     <summary>
      Interface for testing with the EmailService. See best practices for testing.
     </summary>
    */
    public interface IEmailDependency
    {
        Task<bool> SendEmailAsync(string recipientEmail, string subject, string body);
    }
}