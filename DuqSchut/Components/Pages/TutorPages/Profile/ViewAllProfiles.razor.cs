using Microsoft.AspNetCore.Components;
using DuqSchut.Models;
using Microsoft.EntityFrameworkCore;
using DuqSchut.Data;

namespace DuqSchut.Components.Pages.TutorPages.Profile
{
    /**
    <summary>
    This component page will display a select option for term and generate a profile information table for the user.
    </summary>

    <remarks>
        Expectations:
        <list type="bullet">
            <item>Only users with the Admin role should be able to view this page.</item>
            <item>The default table will display profiles for the current term. If there is no current term, it will show the term with the latest start date.</item>
            <item>The table allows the user to sort profiles by the tutor's first name or last name.</item>
        </list>
    </remarks>
    */
    public partial class ViewAllProfiles() : ComponentBase
    {
        [Inject] 
        private ILogger<ViewAllProfiles> Logger { get; set; }

        [Inject]
        private IDbContextFactory<DuqSchutContext> DbFactory { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set;} = default!;

        // This page uses a component lifetime context for queries. This context is disposed of when the page is closed.
        private DuqSchutContext Context;

        // These two variables are used to query the database. They return an IQueryable object that is used by QuickGrid.
        // The TutorProfiles return value is dependent on the current selected term.
        private IQueryable<Term> Terms => Context.Terms.AsQueryable();
        private IQueryable<TutorProfile> TutorProfiles => Context.TutorProfiles.AsQueryable().Where(p => p.TermID.ToString() == SelectedTermID);

        // The selected term ID is bound to the select tag in the html content. It updates when the user selects a different term.
        private string SelectedTermID;

        // I'm not sure if a loading variable is needed while creating a database context, but I included it as an extra precaution.
        private bool IsLoading = true;


        /**
        <summary>
        Loads the database and initial data for rendering the page.
        </summary>

        <remarks>
            <para>
                This method overrides the default OnInitializedAsync method to create a new database context when a user loads this page.
                Additionally, it will set an initial SelectedTermID based on this component's requirement description (Check the component's documentation).
            </para>
        </remarks>
        */
        protected override async Task OnInitializedAsync()
        {
            try{
                IsLoading = true;
                StateHasChanged();

                Context = await DbFactory.CreateDbContextAsync();

                SelectedTermID = GetCurrentTermIDAsString(Terms);

                IsLoading = false;
                StateHasChanged();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to Initialize Page");
                NavigationManager.NavigateTo("/Error");
            }
        }

        /**
        <summary>
        This method uses the requirements described in the component's summary to find the value that SelectedTermID should have on this page's initialization.
        </summary>

        <param name="terms">A queryable list of terms from the database.</param>

        <returns>
            <item>
                <term>string</term>
                <description>The correct selected term ID</description>
            </item>
        </returns>

        <remarks>
            <para>
                Expected return values:
                <list type="bullet">
                    <item>If no terms exist, this should return an empty string.</item>
                    <item>If there is no current term in the database, the term with the latest start date should be returned.</item>
                </list>
            </para>
        </remarks>
        */
        private static string GetCurrentTermIDAsString(IQueryable<Term> terms)
        {
            try{
                // Get today's date and store it in a variable.
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                // Get all terms that contain today's date (startDate <= today <= endDate)
                IQueryable<Term> currentTerms = terms.Where(term => term.StartDate.HasValue && term.StartDate.Value.CompareTo(today) <= 0 &&
                    term.EndDate.HasValue && term.EndDate.Value.CompareTo(today) >= 0);

                // If there are no terms for the current date,
                // return the term with the latest start date
                if (!currentTerms.Any())
                {
                    var firstTerm = terms.OrderByDescending(term => term.StartDate).FirstOrDefault();
                    return firstTerm?.ID.ToString() ?? "No Terms Available";
                }

                // If we have reached this point, we should expect only one term to match the conditions.
                // Return this term.
                return currentTerms.First().ID.ToString();
            }
            catch (Exception)
            {
                // Error getting Term ID, return empty string
                return string.Empty;
            }
        }

        // The lifetime context is disposed when the page is closed.
        public async ValueTask DisposeAsync() => await (Context?.DisposeAsync() ?? ValueTask.CompletedTask);

        /**
        <summary>
        Method <c>CreateTutorProfile</c> Navigates to the Create Tutor Profile page
        </summary>
        <returns></returns>
        */
        public void CreateTutorProfile()
        {
            try{
                NavigationManager.NavigateTo("/tutor/profile/create");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to navigate to Create Tutor Profile Page");
                NavigationManager.NavigateTo("/Error");
            }
        }

    }
}