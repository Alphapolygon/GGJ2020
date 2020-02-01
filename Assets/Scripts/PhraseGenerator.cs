using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class PhraseGenerator : MonoBehaviour
    {
        public enum PhraseType
        {
            Doctor,
            InitialMale,
            InitialFemale,
            Disagree,
            Agree
        }

        private string[] emotions = { "sad", "glad", "angry", "happy" };
        private string[] adjectives = { "awful", "green", "stupid" };
        private string[] verbs = { "does dishes", "makes sweet love to me", "harrasses me", "sleeps with my friends" };

        private static PhraseGenerator instance = null;

        private Dictionary<PhraseType, string> lastGeneratedPhrases;
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
                lastGeneratedPhrases = new Dictionary<PhraseType, string>();
            lastGeneratedPhrases.Clear();
            string verb = verbs[Random.Range(0, verbs.Length)];
            lastGeneratedPhrases.Add(PhraseType.Doctor, "How does that make <emph>you</emph> feel?");
            lastGeneratedPhrases.Add(PhraseType.InitialMale, string.Format("He {0} {1} and that makes me {2}", Random.Range(0, 100) > 50 ? "always" : "never",verb , emotions[Random.Range(0, emotions.Length)]));
            lastGeneratedPhrases.Add(PhraseType.InitialFemale, string.Format("She {0} {1} and that makes me {2}", Random.Range(0, 100) > 50 ? "always" : "never", verb, emotions[Random.Range(0, emotions.Length)]));
            lastGeneratedPhrases.Add(PhraseType.Disagree, "I totally disagree!");
            lastGeneratedPhrases.Add(PhraseType.Agree, "I never do that!");
        }

        public static string getPhrase(PhraseType type)
        {
            return instance.internal_getPhrase(type);
        }

        private string internal_getPhrase(PhraseType type)
        {
            if (lastGeneratedPhrases == null)
            {
                lastGeneratedPhrases = new Dictionary<PhraseType, string>();
                internal_generatePhrases();
            }
            return lastGeneratedPhrases[type];
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