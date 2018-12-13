using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class CardIsInsertedException : Exception
    {
        public CardIsInsertedException() : base("The card is already inserted")
        {
        }
    }
}
