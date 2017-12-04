using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Inputs
{
    public SinglePlayerInputs P1_Inputs { get; set; }
    public SinglePlayerInputs P2_Inputs { get; set; }

    public Inputs(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs)
    {
        P1_Inputs = _p1Inputs;
        P2_Inputs = _p2Inputs;
    }
}