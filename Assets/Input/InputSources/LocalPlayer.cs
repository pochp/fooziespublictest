using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Input
{
    class LocalPlayer : IInputSource
    {
        private bool _p1;

        public LocalPlayer(bool p1)
        {
            _p1 = p1;
        }


        public SinglePlayerInputs GetInputs()
        {
             return InputReaderRewired.GetInputs(_p1);
        }

        public bool IsP1()
        {
            throw new NotImplementedException();
        }
    }
}
