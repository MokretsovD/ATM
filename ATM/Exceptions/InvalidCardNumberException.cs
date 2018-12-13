using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class InvalidCardNumberException : Exception
    {
        public InvalidCardNumberException(string cardNumber) : base($"Card number {cardNumber} is invalid") {}
    }
}
