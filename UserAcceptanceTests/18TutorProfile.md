# User Story 18 User Acceptance  
## As a tutor, I want to create a draft of my own profile for approval   

**Valid Input When Creating Tutor Profile**  
1. Log in to DuqSchut as a tutor or admin. If logged in as a tutor, click the "View Profile" button on the TutorHome page, and then click "Create Profile". If logged in as an admin, click the "Tutor Profiles" button on the admin navigation page and then click "Create Profile".
2. Once naviagted to the Create Tutor Profile page, you should see "Create Your Tutoring Profile" with a series of instructions asking for your input
3. Your email address and name should auto-populate in the "Duq Email:" and "Full Name:" fields
4. Click "Pick a Term" in the "Select a Term" box and choose the term "Spring"
5. Click "Select a Course" in the "Select Courses" box, click on "COSC160", and then click "Add Course"
6. If need be, click on the course in the "Selected Courses" section to remove the course
7. In the calendar located in the "Select Availability" section, click on each half hour time slot that you are available to tutor (blue blocks indicate hours you are available). You could also click-and-drag the slots that you are available.
8. If need be, click on a half hour time slot again to remove it from your available hours
9. In the "About Me" section, input a quick description of yourself (example: "I am a tutor")
10. Click "Submit For Approval"
11. Your tutor profile will be saved and become accessible to an administrator to approve, and for now, you will be redirected to /tutor/profile/ page where your profile will be listed under the correct term
12. On the /tutor/profile/ page, click the "details" link for your profile to navigate to a details page displaying the values of your profile
13. (The edit and remove links are not yet fully functional since it is not part of this user story)

**Duplicate Profiles for Same Term**  
1. Once you submit a profile for the Spring, you should see it listed in the /tutor/profile/ page under the Spring dropdown
2. From there, click "Create Profile" and try to create another profile for that term
3. Your email address and name should auto-populate in the "Duq Email:" and "Full Name:" fields
4. Click "Pick a Term" in the "Select a Term" box and choose "Spring"
5. Click "Select a Course" in the "Select Courses" box, click on "MATH101", and then click "Add Course"
6. If need be, click on the course in the "Selected Courses" section to remove the course
7. In the calendar located in the "Select Availability" section, click on each half hour time slot that you are available to tutor (blue blocks indicate hours you are available). You could also click-and-drag the slots that you are available.
8. If need be, click on a half hour time slot again to remove it from your available hours
9. In the "About Me" section, input a quick description of yourself (example: "I am a tutor")
10. Click "Submit For Approval"
11. The submission will not go through, and you will see the message "You already have a profile for this term". This ensures that a tutor can only create one profile for each term
  
**Invalid Input When Creating Tutor Profile (No courses or about me)**  
1. Log in to DuqSchut as a tutor or admin. If logged in as a tutor, click the "View Profile" button on the TutorHome page, and then click "Create Profile". If logged in as an admin, click the "Tutor Profiles" button on the admin navigation page and then click "Create Profile".
2. Once naviagted to the Create Tutor Profile page, you should see "Create Your Tutoring Profile" with a series of instructions asking for your input
3. Your email address and name should auto-populate in the "Duq Email:" and "Full Name:" fields
4. Click "Pick a Term" in the "Select a Term" box and choose the term "Spring"
5. **Do not select and add any courses**
6. In the calendar located in the "Select Availability" section, click on each half hour time slot that you are available to tutor (blue blocks indicate hours you are available). You could also click-and-drag the slots that you are available.
7. If need be, click on a half hour time slot again to remove it from your available hours
8. **Leave the About Me section blank**
9. Click "Submit For Approval"
10. The submission will not go through, and you will see the messages: "Please provide some information about yourself" and "Please select at least one course"
11. You will have to add valid input for those fields in order for your profile to successfully be submitted
