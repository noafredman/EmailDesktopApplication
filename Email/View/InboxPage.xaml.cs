using AE.Net.Mail;
using System;
//Syroot.Windows.IO; is part of NuGet, taken from:
//https://stackoverflow.com/questions/10667012/getting-downloads-folder-in-c/21953690
using Syroot.Windows.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Email.ViewModel;
namespace Email
{
    /// <summary>
    /// Interaction logic for Inbox.xaml
    /// </summary>
    public partial class InboxPage : Page
    {
        private ImapClient imapClient;
        private Frame frame;
        private EmailViewModel vm;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imapClient"></param>
        /// <param name="frame"></param>
        public InboxPage(EmailViewModel vm, Frame f)
        {
            this.frame = f;
            this.vm = vm;
            this.imapClient = vm.ImapClient;
            this.DataContext = this.vm;
            InitializeComponent();
            //via dispatcher the Model can make changes to View's data grid wich holds/presents the email messages.
            vm.VM_Dispacher = this.Dispatcher;
            (frame.Parent as Window).Closed += delegate (Object sender, EventArgs e)
            {
                vm.CloseWindow();
            };
        }

        /// <summary>
        /// signs out of user's email account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectClicked(object sender, RoutedEventArgs e)
        {
            vm.DisconnectClicked();
            frame.GoBack();
        }

        /// <summary>
        /// See older emails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Older_Clicked(object sender, RoutedEventArgs e)
        {
            vm.OlderClicked();
        }

        /// <summary>
        /// See newer emails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Newer_Clicked(object sender, RoutedEventArgs e)
        {
            vm.NewerClicked();
        }

    }
}
