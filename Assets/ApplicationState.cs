using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ApplicationState
{
    public abstract void Update(Inputs _inputs);
    public IRenderer StateRenderer;
    public static bool InputMappingInCourse = false;

    public virtual string GetDebugInfo()
    {
        var output = "current state : " + this.GetType().Name + Environment.NewLine;

        if (InputMappingInCourse)
        {
            output += "P1 Joystick : " + RewiredJoystickAssigner.GetInputName(true) + Environment.NewLine;
            output += "P2 Joystick : " + RewiredJoystickAssigner.GetInputName(false) + Environment.NewLine;
        }

        return output;
    }
}