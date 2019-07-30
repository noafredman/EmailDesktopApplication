using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using AE.Net.Mail;
using Email.Model;
using Email.ViewModel;

namespace Email.ViewModel
{
    public class EmailViewModel : INotifyPropertyChanged
    {
        //fields
        private EmailModel model;
        private ImapClient imapClient;
        private ObservableCollection<EmailTableRow> messageTable;
        //events
        public event PropertyChangedEventHandler PropertyChanged;
        //properties
        public string VM_EmailAddress
        {
            set { model.Model_EmailAddress = value; }
        }

        public string VM_Password
        {
            set { model.Model_Password = value; }
        }

        public ObservableCollection<EmailTableRow> VM_RowInfo
        {
            get
            {
                ObservableCollection<EmailTableRow> ri = model.RowInfo;
                return model.RowInfo;
            }
            set { this.messageTable = value; }
        }

        public string VM_CloseWindow
        {
            set { model.Model_EmailAddress = value; }
        }

        public int VM_PageNumber
        {
            get { return model.PageNumber; }
        }

        public int VM_NumOfPages
        {
            get { return model.NumOfPages - 1; }
        }

        public ImapClient ImapClient
        {
            get { return this.imapClient; }
        }

        public Dispatcher VM_Dispacher
        {
            set { model.Model_Dispacher = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model"></param>
        public EmailViewModel(EmailModel model)
        {
            messageTable = new ObservableCollection<EmailTableRow>();
            this.model = model;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        //Methods

        /// <summary>
        /// Signs in to account.
        /// </summary>
        /// <returns></returns>
        public bool SignIn()
        {
            //assigning value to model.SignIn will varify account.
            model.SignIn = true;
            if (model.SignIn)
            {
                this.imapClient = model.ImapClient;
            }
            return model.SignIn;
        }

        /// <summary>
        /// Closes window properlly.
        /// </summary>
        public void CloseWindow()
        {
            model.CloseWindow();
        }

        /// <summary>
        /// Uploads older messages
        /// </summary>
        public void OlderClicked()
        {
            model.Older_Clicked();
        }

        /// <summary>
        /// Uploads newer messages
        /// </summary>
        public void NewerClicked()
        {
            model.Newer_Clicked();
        }

        /// <summary>
        /// Signs out of user's gmail account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisconnectClicked()
        {
            model.DisconnectClicked();
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
