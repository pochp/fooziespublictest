using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Match
{
    public class MatchOutcome
    {
        public enum Result { P1_Win, P2_Win}
        public Result MatchResult;
        public MatchOutcome(Result _res)
        {
            MatchResult = _res;
        }
    }
}
