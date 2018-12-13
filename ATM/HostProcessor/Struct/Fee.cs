using System;

namespace ATM.HostProcessor.Struct
{
    public struct Fee
    {
        public string CardNumber { get; set; }
        public decimal WithdrawalFeeAmount { get; set; }
        public DateTime WithdrawalDate { get; set; }
    }
}
