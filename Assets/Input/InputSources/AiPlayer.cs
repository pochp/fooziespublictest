using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Input
{
    class AiPlayer : IInputSource
    {
        //just as a test, he'll mash A every 100 ms or so
        private float timeSinceLastPress;
        private float timeBetweenPresses;

        public AiPlayer()
        {
            timeSinceLastPress = 0f;
            timeBetweenPresses = 0.1f;
        }

        public SinglePlayerInputs GetInputs()
        {
            SinglePlayerInputs inputs = new SinglePlayerInputs();
            timeSinceLastPress += Time.deltaTime;
            if(timeSinceLastPress > timeBetweenPresses)
            {
                timeSinceLastPress = 0;
                inputs.A = true;
            }

            return inputs;
        }

        public bool IsP1()
        {
            throw new NotImplementedException();
        }
    }
}
