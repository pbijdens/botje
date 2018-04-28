using Botje.Messaging.Models;
using System;

namespace Botje.Messaging.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryCallbackEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public long UpdateID { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CallbackQuery CallbackQuery { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateID"></param>
        /// <param name="callbackQuery"></param>
        public QueryCallbackEventArgs(long updateID, CallbackQuery callbackQuery)
        {
            UpdateID = updateID;
            CallbackQuery = callbackQuery;
        }
    }
}