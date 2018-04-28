using Botje.Messaging.Models;
using System;

namespace Botje.Messaging.Events
{
    /// <summary>
    /// Event arguments
    /// </summary>
    public class ChannelMessageEditedEventArgs : EventArgs
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
        /// <param name="editedChannelPost"></param>
        public ChannelMessageEditedEventArgs(long updateID, Message editedChannelPost)
        {
            UpdateID = updateID;
            Message = editedChannelPost;
        }
    }
}