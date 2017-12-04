using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Match
{
    public class SetData
    {
        private List<MatchOutcome> m_results;
        public int P1_Score
        {
            get
            {
                return m_results.Count(o => o.MatchResult == MatchOutcome.Result.P1_Win);
            }
        }
        public int P2_Score
        {
            get
            {
                return m_results.Count(o => o.MatchResult == MatchOutcome.Result.P2_Win);
            }
        }

        public MatchInitializationData m_initData;
        public Character P1_SelectedCharacter { get { return m_initData.P1_Character; } }
        public Character P2_SelectedCharacter { get { return m_initData.P2_Character; } }

        public SetData()
        {
            m_results = new List<MatchOutcome>();
            m_initData = new MatchInitializationData();
        }
        public void AddResult(MatchOutcome _outcome)
        {
            m_results.Add(_outcome);
        }
        public void SetMatchInitData(MatchInitializationData _initData)
        {
            m_initData = _initData;
        }
    }
}