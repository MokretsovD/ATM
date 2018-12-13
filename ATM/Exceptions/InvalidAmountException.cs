using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException() : base("The requested amount is incorrect")
        {

        }
    }
}
