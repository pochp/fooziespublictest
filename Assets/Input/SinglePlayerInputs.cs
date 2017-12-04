using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SinglePlayerInputs
{
    //789
    //456
    //123
    public short JoystickDirection;
    public bool A, B, C, Start;
    public SinglePlayerInputs()
    {
        JoystickDirection = 5;
        A = false;
        B = false;
        C = false;
        Start = false;
    }
}

