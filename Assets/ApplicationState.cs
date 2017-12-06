using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ApplicationState
{
    public abstract void Update(Inputs _inputs);

    public virtual string GetDebugInfo()
    {
        return "current state : " + this.GetType().Name;
    }
}