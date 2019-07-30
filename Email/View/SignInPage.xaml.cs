using AE.Net.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Email.ViewModel;
using Email.Model;
using System.Text.RegularExpressions;

namespace Email
{
    /// <summary>
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private EmailViewModel vm;
        Frame frame;
        const string HOST = "imap.gmail.com";
        private Button btnSignIn;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="f"></param>
        public SignInPage(Frame f)
        {
            InitializeComponent();
            btnSignIn = signBtn;
            frame = f;
            //the View has access to the ViewModel wich has access to the Model
            this.vm = new EmailViewModel(new EmailModel());
            //should comment out when not testing.
            emailBox.Text = "";
            pwdBox.Password = "";
        }

        /// <summary>
        /// Signs in to user's gmail account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignInClicked(object sender, RoutedEventArgs e)
        {
            SignIn();
        }

        /// <summary>
        /// Signs in to user's gmail account.
        /// </summary>
        private void SignIn()
        {
            this.vm = new EmailViewModel(new EmailModel());
            if (ValidateAddress())
            {
                //email ends with "@gmail.com".
                vm.VM_EmailAddress = emailBox.Text;
                if (pwdBox.Password != "")
                {
                    vm.VM_Password = pwdBox.Password;
                    if (vm.SignIn())
                    {
                        //show user's inbox
                        frame.Content = new InboxPage(this.vm, this.frame);
                    }
                    else
                    {
                        Console.WriteLine("failed connection");
                    }
                }
                else
                {
                    MessageBox.Show("Fill in password",
                        "Empty Password detected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Email address has to end with \"@gmail.com\" \nand contain at least 1 character" +
                    " before @gmail.com" ,
                    "Invalid Email Address",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Validates given email address. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool ValidateAddress()
        {
            //address must contain at least 1 character and end with @gmail.com
            Regex regex = new Regex("^([a-zA-Z0-9_\\-\\.]+)[+@gmail.com]");
            bool ret = regex.IsMatch(emailBox.Text);
            return ret;
        }
      

        /// <summary>
        /// Changes sign in button background, to show user it's been clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // if enter key clicked
                //change signIn button background via style.
                signBtn.Style = Application.Current.FindResource("YellowSignInButton") as Style;
            }
        }

        /// <summary>
        /// Signs in to user's gmail account if "enter" key pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //sign in if user pressed "enter" key.
                SignIn();
                //change signIn button background via style.
                signBtn.Style = Application.Current.FindResource("GreenButton") as Style;
            }
        }
    }
}
