using Botje.Messaging.Models;
using System;

namespace Botje.Messaging.Events
{
    /// <summary>
    /// Event arguments
    /// </summary>
    public class ChosenInlineQueryResultEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public long UpdateID { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ChosenInlineResult ChosenInlineResult { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateID"></param>
        /// <param name="chosenInlineResult"></param>
        public ChosenInlineQueryResultEventArgs(long updateID, ChosenInlineResult chosenInlineResult)
        {
            UpdateID = updateID;
            ChosenInlineResult = chosenInlineResult;
        }
    }
}