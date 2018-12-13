using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class CannotAccessCardException : Exception
    {
        public CannotAccessCardException() : base("Cannot access card. Is it inserted?")
        {
        }
    }
}
