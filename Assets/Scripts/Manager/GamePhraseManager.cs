using System.Collections.Generic;
using UnityEngine.Serialization;
using Utils;

namespace Manager
{
    public class GamePhraseManager : Singleton<GamePhraseManager>
    {
        [FormerlySerializedAs("Phrase")] 
        public List<int> phrase;
        public int currentPhrase;

        
        public void SwitchPhrase(int leve)
        {
            currentPhrase = leve;
        }
    }
}