# User Acceptance Tests

Add user acceptance tests to this folder, one file per user story.  The file name should begin with the user story number followed by a brief description of the user story.  The filename should end with the extension .md, which means that it will be treated by GitHub and VS Code as a markdown file.  You can add simple formatting to a markdown file, such as headings and boldface text.  See https://docs.github.com/github/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax for details on markdown. You can also view the raw version of this file, which is a markdown file, as an example.

A user acceptance test is a script along the lines of

1. On the home page, click Login.  System will respond with login page.
2. Enter Multipass lipeckya@duq.edu and name Alex Lipecky.  Click the Login button.  System will display Admin landing page.

Typically, the file for a user story will contain multiple acceptance tests:
- For the example above, a second test might take the user to the Tutor landing page, a third test to the Student landing page.
- Often, there will be one or more user acceptance tests that have the user purposely enter erronoeous data to demonstrate how the system will respond.
