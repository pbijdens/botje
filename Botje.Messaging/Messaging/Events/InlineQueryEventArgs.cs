using Botje.Messaging.Models;
using System;

namespace Botje.Messaging.Events
{
    /// <summary>
    /// Event arguments
    /// </summary>
    public class InlineQueryEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public long UpdateID { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public InlineQuery Query { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateID"></param>
        /// <param name="inlineQuery"></param>
        public InlineQueryEventArgs(long updateID, InlineQuery inlineQuery)
        {
            UpdateID = updateID;
            Query = inlineQuery;
        }
    }
}