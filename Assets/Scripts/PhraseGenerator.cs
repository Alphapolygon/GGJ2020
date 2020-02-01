using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class PhraseGenerator : MonoBehaviour
    {
        private string[] emotions = { "sad", "glad", "angry", "happy" };
        private string[] adjectives = { "awful", "green", "stupid" };
        private string[] verbs = { "does dishes", "makes sweet love to me", "harrasses me", "sleeps with my friends" };
        private string[] feelingStrings = { "Just {0}.", "I feel {0}!", "That makes me {0}!", "Hrmh. {0}" };
        private static PhraseGenerator instance = null;

        private Dictionary<string, string> lastGeneratedPhrases;
        void OnEnable()
        {
            if (instance == null)
                instance = this;
        }
        void Start()
        {
            if (instance == null)
                instance = this;
        }

        public static void generatePhrases()
        {
            instance.internal_generatePhrases();
        }

        private void internal_generatePhrases()
        {
            if (lastGeneratedPhrases == null)
                lastGeneratedPhrases = new Dictionary<string, string>();
            lastGeneratedPhrases.Clear();
            lastGeneratedPhrases.Add("Doctor", "How does that make <emph>you</emph> feel?");
            var details = SessionLogic.GetSessionDetails();
            List<string> tempVerbs = new List<string>();
            tempVerbs.AddRange(verbs);
            for (int i = 0; i < details.roundCount; ++i)
            {
                string line = "";
                if (i > 0)
                    line = string.Format(feelingStrings[Random.Range(0, feelingStrings.Length)], emotions[Random.Range(0, emotions.Length)]);
                string verb = verbs[Random.Range(0, verbs.Length)];
                string accusationM = line + string.Format("He {0} {1}", Random.Range(0, 100) > 50 ? "always" : "never", verb);
                string accusationF = line + string.Format("She {0} {1}", Random.Range(0, 100) > 50 ? "always" : "never", verb);
                lastGeneratedPhrases.Add("lineM" + i, accusationM);
                lastGeneratedPhrases.Add("lineF" + i, accusationF);
                lastGeneratedPhrases.Add("lineD" + i, "Honk honk, hunk. <emph>Honk</emph>! Honk?");
            }
        }

        public static string getPhrase(int index, SessionLogic.PersonType type)
        {
            return instance.internal_getPhrase(index, type);
        }

        public static string getPhrase(string lineId)
        {
            return instance.internal_getPhrase(lineId);
        }

        private string internal_getPhrase(int index, SessionLogic.PersonType type)
        {
            if (lastGeneratedPhrases == null)
            {
                lastGeneratedPhrases = new Dictionary<string, string>();
                internal_generatePhrases();
            }
            return lastGeneratedPhrases["line" + (type == SessionLogic.PersonType.Female ? "F" : type == SessionLogic.PersonType.Male ? "M" : "D") + index];
        }

        private string internal_getPhrase(string lineId)
        {
            if (lastGeneratedPhrases == null)
            {
                lastGeneratedPhrases = new Dictionary<string, string>();
                internal_generatePhrases();
            }
            return lastGeneratedPhrases[lineId];
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                WindowsVoice.getInstance().test();
            }
        }
    }
}