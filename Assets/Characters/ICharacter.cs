using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

abstract class Character
{
    readonly public int MaxGauge;
    readonly public int MinCost;

    readonly int Startup;
    readonly int Active;
    readonly int Recovery;
    readonly string AttackType; //make an enum for this. mid, high, low, unblockable, nothing, 
}
