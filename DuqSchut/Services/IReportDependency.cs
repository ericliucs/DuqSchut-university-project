using DuqSchut.Models;

namespace DuqSchut.Services
{
    /**
     <summary>
      Interface for testing with the ReportService. See best practices for testing.
     </summary>
    */
    public interface IReportDependency
    {
        MemoryStream GenerateBasicReport(string sheetName, List<Appointment> appointments);
    }
}