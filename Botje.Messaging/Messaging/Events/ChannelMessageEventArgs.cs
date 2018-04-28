using Botje.Messaging.Models;
using System;

namespace Botje.Messaging.Events
{
    /// <summary>
    /// Event arguments
    /// </summary>
    public class ChannelMessageEventArgs : EventArgs
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
        /// <param name="channelPost"></param>
        public ChannelMessageEventArgs(long updateID, Message channelPost)
        {
            UpdateID = updateID;
            Message = channelPost;
        }
    }
}