using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException() : base("Not enough money on the account to perform the operation")
        {

        }
    }
}
