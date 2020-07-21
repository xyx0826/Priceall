using System;

namespace Priceall.Appraisal
{
    /// <summary>
    /// Markets supported by appraisal services.
    /// </summary>
    [Flags]
    enum AppraisalMarket
    {
        Universe = 0b_0000_0001,
        Jita     = 0b_0000_0010,
        TheForge = 0b_0000_0100,
        Amarr    = 0b_0000_1000,
        Dodixie  = 0b_0001_0000,
        Hex      = 0b_0010_0000,
        Rens     = 0b_0100_0000,
    }
}
