using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class CardIsNotExistsException : Exception
    {
        public CardIsNotExistsException(string cardNumber) : base($"Card with the number {cardNumber} is not exist")
        {
        }
    }
}
