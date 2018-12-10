using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Inputs
{
    public SinglePlayerInputs P1_Inputs { get; set; }
    public SinglePlayerInputs P2_Inputs { get; set; }
    public CommonInputs Common_Inputs { get; set; }

    public Inputs(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs, CommonInputs _commonInputs)
    {
        P1_Inputs = _p1Inputs;
        P2_Inputs = _p2Inputs;
        Common_Inputs = _commonInputs;
    }
}

public class CommonInputs
{
    public bool F4;//remap controllers
    public CommonInputs()
    {
        F4 = false;
    }
}