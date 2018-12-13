using ATM.Cash.Enum;
using System.Collections.Generic;

namespace ATM.Cash.Struct
{
    public struct Money
    {
        public int Amount { get; set; }
        public Dictionary<PaperNote, int> Notes { get; set; }
    }
}
