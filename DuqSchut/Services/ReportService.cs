using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DuqSchut.Models;

// Before writing tests for pages or services that use this as a dependency, please
// review the Servies section of the coding standards

namespace DuqSchut.Services
{
    public class ReportService : IReportDependency
    {
        private readonly ILogger<ReportService> _logger;
        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
        }

        /**
        <summary>
            Create an excel file that contains a basic summary report showing the number of appointments per start time for each day of the week. 
            More advanced information about inserting text into an openxml cell can be found here:
            https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-insert-text-into-a-cell-in-a-spreadsheet?tabs=cs-1%2Ccs-2%2Ccs-3%2Ccs-4%2Ccs
            more on using memory streams can be found here and here:
            https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads?view=aspnetcore-9.0 
            https://stackoverflow.com/questions/45082779/download-file-with-openxml
        </summary>
        <param name="sheetName">Name of the sheet. Preferably the name of the report</param>
        <param name="appointmnets">List of appointments to use to generate the report</param>
        */
        public MemoryStream GenerateBasicReport(string sheetName, List<Appointment> appointments)
        {
            try
            {
                var memory = new MemoryStream();
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(memory, SpreadsheetDocumentType.Workbook))
                {
                    // create the workbook and worksheet then get the sheet data
                    WorksheetPart worksheetPart = CreateWorkbook(document, sheetName);
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // create a dictionary to store all time slots for the given appointments and their associated counts
                    // this will be used to create column totals
                    var totalSlots = CountTimeSlots(appointments);

                    // Add a header row 
                    List<TimeOnly> startTimes = totalSlots.Keys.OrderBy(t => t).ToList();
                    Row headerRow = new Row();
                    headerRow.Append(CreateCell("Day of Week", CellValues.String)); // first cell is a label for the day of the week
                    foreach (var startTime in startTimes)
                    {
                        headerRow.Append(CreateCell(startTime.ToString(), CellValues.String));
                    }
                    headerRow.Append(CreateCell("Total", CellValues.String)); // last cell is a label for the total
                    sheetData.Append(headerRow);

                    // assuming we do not tutor on weekends, add data to the report for each day of the week
                    List<string> Days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];

                    foreach (var day in Days)
                    {
                        // keep track of the total to put at the end of the row
                        int rowTotal = 0;

                        // create a new row, and add the day as the first cell
                        Row row = new();
                        row.Append(CreateCell(day, CellValues.String));

                        var appointmentsByDay = appointments.Where(appointment => appointment.Date.DayOfWeek.ToString().ToLower() == day.ToLower()).ToList();
                        var timeSlotsByDay = CountTimeSlots(appointmentsByDay);

                        // add a cell and value for each starttime and corresponding count or 0 if there are no appointments
                        foreach (var startTime in startTimes)
                        {
                            int count = 0;
                            if (timeSlotsByDay.ContainsKey(startTime))
                            {
                                count = timeSlotsByDay[startTime];
                            }
                            else
                            {
                                count = 0;
                            }
                            row.Append(CreateCell(count, CellValues.Number));
                            rowTotal += count;
                        }

                        // add the total to the end of the row
                        row.Append(CreateCell(rowTotal, CellValues.Number)); 
                        sheetData.Append(row);
                    }

                    // create a row for the column totals
                    Row columnTotalRow = new Row();
                    columnTotalRow.Append(CreateCell("Total", CellValues.String)); // first cell is a label for the total
                    int grandTotal = 0;
                    foreach (var startTime in startTimes)
                    {
                        int columnTotal = totalSlots[startTime];
                        columnTotalRow.Append(CreateCell(columnTotal, CellValues.Number));
                        grandTotal += columnTotal;
                    }

                    // add the total of all columns to the end 
                    columnTotalRow.Append(CreateCell(grandTotal, CellValues.Number));
                    sheetData.Append(columnTotalRow);
                    
                    // save the changes, the file should automatically close when the using block ends
                    worksheetPart.Worksheet.Save();
                }
                memory.Position = 0;
                return memory;
            }
            catch (Exception ex)
            {
                // Log the error message
                _logger.LogError(ex, "Error generating report: {Message}", ex.Message);
                return new MemoryStream();
            }
        }

        /**
        <summary>
            Create a new workbook and add a worksheet and sheet to it. This is meant to be used within
            a using block to ensure that the workbook is disposed of properly. 
            More information about the contents of this function can be found:
            https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-create-a-spreadsheet-document-by-providing-a-file-name?tabs=cs-0%2Ccs-100%2Ccs-1%2Ccs
        </summary>
        <param name="sheetName">Name of the sheet. Preferably the name of the report</param>
        <returns name="worksheetPart">Worksheet created in a new workbook that can be used to generate a report</returns>
        */
        private WorksheetPart CreateWorkbook(SpreadsheetDocument document, string sheetName)
        {
            try 
            {
                // Create and add a workbook to the document (workbook = root element for the main document)
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Create and add a worksheet to the workbook (worksheet = A type of sheet that represent a grid of cells)
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Create a new list of sheets and add this to the workbook
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                // Create a new sheet and add it to the list of sheets assocaited with the workbook
                Sheet sheet = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = $"{sheetName}"
                };                
                sheets.Append(sheet);

                // save the workbook and return the worksheet so we can add data to it later
                workbookPart.Workbook.Save();
                return worksheetPart; 
            }
            catch (Exception ex)
            {
                // Log the error message
                _logger.LogError(ex, "Error creating workbook: {Message}", ex.Message);
                return null;
            }
        }

        /**
        <summary>
            Util function to more easily create a new cell.
        </summary>
        <param name="value">data to be inserted into the cell</param>
        <param name="dataType">CellValues data type of the cell.</param>
        <returns name="cell">New cell object to place into the row/worksheet</returns>
        */
        private Cell CreateCell(object value, CellValues dataType)
        {
            // util method that creates a cell based on the value and data type
            return new Cell { DataType = dataType, CellValue = new CellValue(value.ToString()) };
        }

        /**
        <summary>
            Util function to modularize the counting of time slots for the basic report.
        </summary>
        <param name="appointments">appointments to be counted</param>
        <returns name="timeSlotCounts">Dictionary of time slots and their counts</returns>
        */

        private Dictionary<TimeOnly, int> CountTimeSlots(List<Appointment> appointments)
        {
            // store each unique 30 time slot and the number of appointments associated with it. Each time slot
            // is 30 mintes, if the length of the appointment is longer than 30 minutes, then add a slot for each 30 minute 
            // interval. This would allow us to count the number of appointments that last for one hour or longer (if necessary)
            var timeSlotCounts = new Dictionary<TimeOnly, int>();

            foreach (var appointment in appointments)
            {
                double totalMinutes = (appointment.EndTime - appointment.StartTime).TotalMinutes;
                int numberOfSlots = (int)(totalMinutes / 30);
                for (int i = 0; i < numberOfSlots; i++)
                {
                    TimeOnly currStartTime = appointment.StartTime.Add(TimeSpan.FromMinutes(30 * i));
                    if (timeSlotCounts.ContainsKey(currStartTime))
                    {
                        timeSlotCounts[currStartTime]++;
                    }
                    else
                    {
                        timeSlotCounts[currStartTime] = 1;
                    }
                }
            }

            return timeSlotCounts;
        }
    }
}