using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Match
{
    public class SetData
    {
        private List<MatchResult> m_results;
        public int P1_Score
        {
            get
            {
                return m_results.Count(o => o.Match_Result == MatchResult.Result.P1_Win);
            }
        }
        public int P2_Score
        {
            get
            {
                return m_results.Count(o => o.Match_Result == MatchResult.Result.P2_Win);
            }
        }

        public MatchInitializationData InitData;
        public Character P1_SelectedCharacter { get { return InitData.P1_Character; } }
        public Character P2_SelectedCharacter { get { return InitData.P2_Character; } }

        public SetData()
        {
            m_results = new List<MatchResult>();
            InitData = new MatchInitializationData();
        }
        public void AddResult(MatchOutcome _outcome)
        {
            //MatchResult res = new MatchResult()
            //m_results.Add(_outcome);
        }
        public void SetMatchInitData(MatchInitializationData _initData)
        {
            InitData = _initData;
        }
    }
}