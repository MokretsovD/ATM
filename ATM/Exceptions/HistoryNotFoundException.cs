using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class HistoryNotFoundException : Exception
    {
        public HistoryNotFoundException() : base("History for the requested item does not exist")
        {

        }
    }
}
