using Botje.Messaging.Models;
using System;

namespace Botje.Messaging.Events
{
    /// <summary>
    /// Event arguments
    /// </summary>
    public class PrivateMessageEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public long UpdateID { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Message Message { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateID"></param>
        /// <param name="editedMessage"></param>
        public PrivateMessageEventArgs(long updateID, Message editedMessage)
        {
            UpdateID = updateID;
            Message = editedMessage;
        }
    }
}