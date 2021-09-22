using System;

namespace Priceall.Appraisal
{
    /// <summary>
    /// Markets supported by appraisal services.
    /// </summary>
    [Flags]
    enum AppraisalMarket
    {
        Universe    = 0b_0000_0000_0001,    // A_
        Jita        = 0b_0000_0000_0010,    // AJ
        TheForge    = 0b_0000_0000_0100,    // _J
        Amarr       = 0b_0000_0000_1000,    // A_
        Dodixie     = 0b_0000_0001_0000,    // A_
        Hek         = 0b_0000_0010_0000,    // A_
        Rens        = 0b_0000_0100_0000,    // A_
        Perimeter   = 0b_0000_1000_0000,    // AJ
        SystemR1OGn = 0b_0001_0000_0000,    // _J
        NPC         = 0b_0010_0000_0000     // _J
    }
}
