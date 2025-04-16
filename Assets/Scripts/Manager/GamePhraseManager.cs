using System.Collections.Generic;
using UnityEngine.Serialization;
using Utils;

namespace Manager
{
    public class GamePhraseManager : Singleton<GamePhraseManager>
    {
        [FormerlySerializedAs("Level")] 
        public List<string> level;
        public string currentLevel = "";

        
        public void SwitchLevel(string leve)
        {
            currentLevel = leve;
        }
    }
}