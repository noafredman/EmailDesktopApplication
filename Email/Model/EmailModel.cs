using AE.Net.Mail;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Email.Model
{
    public class EmailModel : INotifyPropertyChanged
    {
        private IEnumerable<AE.Net.Mail.MailMessage> mailMessage;
        private int numOfMsgToShow = 25, msgNum;
        private ImapClient imapClient;
        private Dictionary<string, ICollection<AE.Net.Mail.Attachment>> allAttachments;
        private static int id = 0;
        private int tableNum = 1;
        private int pageNum = 1;
        private int numOfPagesToAdd = 2;
        private Thread oneThread;
        private System.Windows.Controls.DataGrid dg;
        const string HOST = "imap.gmail.com";
        private string emailAddress;
        private string password;
        private Table table;

        //INotifyPropertyChanged implementation:
        private bool signIn;
        private ObservableCollection<EmailTableRow> infos;
        //page is a somewhat dummy for infos. when want to send a page/table/rows to view will update infos to page value.
        private ObservableCollection<EmailTableRow> page;
        private Dispatcher dispatcher;
        private Dictionary<int, ObservableCollection<EmailTableRow>> allTables;

        //events
        public event PropertyChangedEventHandler PropertyChanged;

        //properties
        public bool SignIn
        {
            get { return this.signIn; }
            set
            {
                signIn = VarifyAccount();
                NotifyPropertyChanged("SignIn");
                if (signIn)
                {
                    this.signIn = true;
                    this.msgNum = imapClient.GetMessageCount() - 3;
                    //retrieves user's inbox from gmail account.
                    imapClient.SelectMailbox("INBOX");
                    GetMoreMsgs();
                    InitializeGrid();
                    UpdateTable();
                    if (oneThread == null || !oneThread.IsAlive)
                    {
                        oneThread = new Thread(GetMoreTables);
                        //start thread that gets more emails from inbox.
                        oneThread.Start();
                    }
                }

            }
        }

        public ImapClient ImapClient
        {
            get { return this.imapClient; }
        }

        public string Model_EmailAddress
        {
            get { return this.emailAddress; }
            set { this.emailAddress = value; }
        }

        public string Model_Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public ObservableCollection<EmailTableRow> RowInfo
        {
            get { return this.infos; }
            set
            {
                this.infos = value;
                NotifyPropertyChanged("RowInfo");
            }
        }

        public ObservableCollection<EmailTableRow> Page
        {
            get { return this.page; }
            set
            {
                this.page = value;
            }
        }

        public int PageNumber
        {
            get { return this.pageNum; }
            set
            {
                this.pageNum = value;
                NotifyPropertyChanged("PageNumber");
            }
        }

        public int NumOfPages
        {
            get { return this.tableNum; }
            set
            {
                this.tableNum = value;
                NotifyPropertyChanged("NumOfPages");
            }
        }

        public Dispatcher Model_Dispacher
        {
            set { this.dispatcher = value; }
        }

        //methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmailModel()
        {
            RowInfo = new ObservableCollection<EmailTableRow>();
            allAttachments = new Dictionary<string, ICollection<AE.Net.Mail.Attachment>>();
            allTables = new Dictionary<int, ObservableCollection<EmailTableRow>>();
        }

        /// <summary>
        /// Varifies user's gmail account.
        /// </summary>
        /// <returns></returns>
        private bool VarifyAccount()
        {
            try
            {
                // Connect to the IMAP server. The 'true' parameter specifies to use SSL
                // which is important (for Gmail at least)
                imapClient = new ImapClient(HOST, Model_EmailAddress, Model_Password,
                            AuthMethods.Login, 993, true);
                return true;
            }
            catch (Exception e)
            {
                //unable to sign in
                MessageBox.Show("Connection Error. Try Again.",
            "Error in Connection",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Gets more messages from inbox.
        /// </summary>
        private void GetMoreMsgs()
        {
            int lastNmsg = msgNum - numOfMsgToShow * (allTables.Count) - numOfPagesToAdd * numOfMsgToShow;
            //the false - gets not headers only -gets body of messages. the true - lets us get the (invisible) attachments.
            mailMessage = imapClient.GetMessages(lastNmsg, msgNum - numOfMsgToShow * (allTables.Count), false, true);
            mailMessage = mailMessage.Reverse();
        }

        /// <summary>
        /// Initializes the email inbox messages table.
        /// </summary>
        private void InitializeGrid()
        {
            int numOfAttachments = 0, index = 0;
            string msgFrom = "";
            Page = new ObservableCollection<EmailTableRow>();
            foreach (AE.Net.Mail.MailMessage msg in mailMessage)
            {
                numOfAttachments = msg.Attachments.Count;
                if (msg.From == null)
                {
                    msgFrom = "null";
                }
                else
                {
                    msgFrom = msg.From.ToString();
                }
                //Sender
                if (msgFrom.LastIndexOf("\"") > 1)
                {
                    msgFrom = msgFrom.Substring(1, msgFrom.LastIndexOf("\"") - 1);
                }
                else
                {
                    // do nothing
                }
                Button downloadBtn = new Button();
                downloadBtn.Content = "download";
                downloadBtn.Click += DownloadAttachments;
                downloadBtn.Name = "a" + id.ToString();
                downloadBtn.Style = Application.Current.FindResource("DownloadButton") as Style;
                id++;
                //are there attachments
                if (numOfAttachments > 0)
                {
                    //email has attachments.
                    allAttachments.Add(downloadBtn.Name, msg.Attachments);
                    downloadBtn.Content += " " + msg.Attachments.Count.ToString() + " file";
                    if (numOfAttachments > 1)
                    {
                        downloadBtn.Content += "s";
                    }
                    else
                    {
                        downloadBtn.Content += " ";
                    }
                }
                if (numOfAttachments == 0)
                {
                    downloadBtn = null;
                }
                else
                {
                    //do nothing
                }
                Page.Add(new EmailTableRow
                {
                    Sender = msgFrom,
                    Subject = msg.Subject,
                    HasAttachments = downloadBtn,
                });
                //add row to table.
                if (++index == numOfMsgToShow)
                {
                    //add table to dictionary and start new table.
                    allTables.Add(NumOfPages++, Page);
                    Page = new ObservableCollection<EmailTableRow>();
                    index = 0;//start new table
                }
            }
        }

        /// <summary>
        /// Updates "page" to newer/older emails (table)
        /// </summary>
        private void UpdateTable()
        {
            if (pageNum <= allTables.Count)
            {
                RowInfo = allTables[pageNum];
            }
        }

        /// <summary>
        /// Get more emails to load into allTables
        /// </summary>
        public void GetMoreTables()
        {
            int lastNmsg;

            lastNmsg = this.msgNum - numOfMsgToShow * (allTables.Count) - numOfPagesToAdd * numOfMsgToShow;
            while (lastNmsg + numOfMsgToShow > 0 && allTables.Count < 500)
            {
                try
                {
                    GetMoreMsgs();
                    this.dispatcher.Invoke(() =>
                  {
                      InitializeGrid();
                  });
                }
                catch (Exception e)
                {
                    //do nothing. try again.
                }
                lastNmsg = this.msgNum - numOfMsgToShow * (allTables.Count) - numOfPagesToAdd * numOfMsgToShow;
            }

        }
        
        /// <summary>
        /// Aborts open thread on window close.
        /// </summary>
        public void CloseWindow()
        {
            if (oneThread != null && oneThread.IsAlive)
            {
                oneThread.Abort();
            }
        }

        /// <summary>
        /// Downloads attachments to user's downlaods folder.
        /// </summary>
        /// <param name="msg"> message user wnts to download attachments from.</param>
        private void DownloadAttachments(object sender, RoutedEventArgs e)
        {
            string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path;
            string fileName = "";
            ICollection<AE.Net.Mail.Attachment> attachments = allAttachments[
                ((System.Windows.Controls.Button)sender).Name];
            foreach (AE.Net.Mail.Attachment attachment in attachments)
            {
                fileName = attachment.Filename;
                // uncomment if wnat to see file type in name.// + System.IO.Path.GetExtension(attachment.Filename);
                attachment.Save(downloadsPath + "\\" + fileName);
            }
            //open alert to notify user downloads' complete.
            MessageBoxResult result = FilesDownloadedAlert();
            if (result == MessageBoxResult.Yes)
            {
                //open Downloads folder (where attachments were saved).
                OpenDownloadsFolder(downloadsPath, fileName);
            }
        }

        /// <summary>
        /// Open window alert to notify user downloads' complete.
        /// </summary>
        /// <returns></returns>
        private MessageBoxResult FilesDownloadedAlert()
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Information;
            return System.Windows.MessageBox.Show("Would you like to view the downloaded files?",
                "Files download complete", button, icon);
        }

        /// <summary>
        /// Opens new "window explorrer" window where attachments were saved.
        /// </summary>
        /// <param name="downloadsPath"></param>
        /// <param name="fileName"></param>
        private void OpenDownloadsFolder(string downloadsPath, string fileName)
        {
            Process ExplorerWindowProcess = new Process();
            //opens a new WindowsExplorrer to file downloaded location - the folder Downloads.
            ExplorerWindowProcess.StartInfo.FileName = "explorer.exe";
            ///select,<object>  Opens a window view with the specified folder, file or application selected.
            ExplorerWindowProcess.StartInfo.Arguments = "/select,\"" + downloadsPath + "\\" + fileName + "\"";
            ExplorerWindowProcess.Start();
        }

        /// <summary>
        /// See older emails.
        /// </summary>
        public void Older_Clicked()
        {
            int lastNmsg;
            int numOfTables = allTables.Count;
            lastNmsg = this.msgNum - numOfMsgToShow * (pageNum + 1); ;
            if (lastNmsg + numOfMsgToShow > 0 && pageNum < allTables.Count)
            {
                PageNumber++;// += 1;
                UpdateTable();
                //   Console.WriteLine("");
            }
        }

        /// <summary>
        /// See newer emails.
        /// </summary>
        public void Newer_Clicked()
        {
            int lastNmsg = this.msgNum - numOfMsgToShow;
            if (pageNum > 1)
            {
                PageNumber--;
                UpdateTable();
            }
        }

        /// <summary>
        /// Signs out of user's gmail account.
        /// </summary>
        public void DisconnectClicked()
        {
            if (oneThread != null && oneThread.IsAlive)
            {
                oneThread.Abort();
            }
            imapClient.Disconnect();
        }

        /// <summary>
        /// Will notify the elemnt bounded to given propName that change has been made.
        /// </summary>
        /// <param name="propName">property changed.</param>
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
