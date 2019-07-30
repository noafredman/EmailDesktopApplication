# EmailDesktopApplication
Written in C#.NET.
I programmed a desktop app in WPF which was built according to MVVM architecture with use of DATA BINDING between the View and the ViewModel.
The View has 2 pages that are shown within a frame in the main window to the client. One page is for the client to log into a Gmail account. The other page will show the client emails from the inbox – with sender, subject, and if there are attachments available to download with a corresponding download button.
The Model is responsible to verify the Gmail account and password, and to update the table that shows the email messages, and download attachments if client clicked the download button.
