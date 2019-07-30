using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Email
{
    public class EmailTableRow
    {
        string sender, subject;
        Button hasAttachments;
        //properties
        public string Sender
        {
            get { return sender; }
            set
            {
                sender = value;
            }
        }

        public string Subject
        {
            get { return subject; }
            set
            {
                subject = value;
            }
        }

        public Button HasAttachments
        {
            get { return hasAttachments; }
            set
            {
                hasAttachments = value;
            }
        }

        /// <summary>
        /// constructor.
        /// </summary>
        public EmailTableRow() { }

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="subject"></param>
        /// <param name="hasAttachments"></param>
        public EmailTableRow(string sender, string subject, Button hasAttachments)
        {
            this.sender = sender;
            this.subject = subject;
            this.hasAttachments = hasAttachments;
        }
    }
}
