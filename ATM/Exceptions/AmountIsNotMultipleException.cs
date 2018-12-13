using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class AmountIsNotMultipleException : Exception
    {
        public AmountIsNotMultipleException(int multiplicator) : base($"The requested amount must be a multiple of {multiplicator}")
        {

        }
    }
}
