using System.Collections.Generic;
using UnityEngine.Serialization;
using Utils;

namespace Manager
{
    public class GamePhraseManager : Singleton<GamePhraseManager>
    {
        [FormerlySerializedAs("Phrase")] 
        public List<string> phrase;
        public string currentPhrase = "";

        
        public void SwitchPhrase(string leve)
        {
            currentPhrase = leve;
        }
    }
}