using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class InvalidDeclaredAmountException : Exception
    {
        public InvalidDeclaredAmountException() : base("Declared money amount is not correct")
        {

        }
    }
}
