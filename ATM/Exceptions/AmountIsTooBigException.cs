using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class AmountIsTooBigException : Exception
    {
        public AmountIsTooBigException() : base("The requested amount is too big")
        {

        }
    }
}
