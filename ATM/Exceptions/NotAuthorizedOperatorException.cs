using System;

namespace ATM.Exceptions
{
    [Serializable]
    public class NotAuthorizedOperatorException : Exception
    {
        public NotAuthorizedOperatorException() : base("You are not authorized to execute this operation")
        {
        }
    }
}
