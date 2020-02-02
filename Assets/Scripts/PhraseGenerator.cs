using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class PhraseGenerator : MonoBehaviour
    {
        private string[] emotions = { "sad", "glad", "angry", "happy", "adorable","amused","concern","fear","unease","nervous","mad","worry","joyless","envious","violated", "spoiled","ignored", "neglected","disturbed","shamed","loved","cherished"};
        private string[] adjectives = { "awful", "green", "stupid" };
        private string[] verbs = { "does dishes", "makes sweet love to me", "harrasses me", "sleeps with my friends","talks to me","helps me with my work","takes care of the kids","takes out the trash","does chores","smiles to me","drinks too much","spends all of our money","spends too much time on the phone","is working","massages me","spends time with the kids", "watches tv during meals", "is jealous", "is a Trump supporter", "is mad at me", "is watching too much porn", "does stuff I want to do","opens doors to me","helps me","watches tv without me", "hangs the toilet paper wrong way", "walks the dog","breathing too loud","spends too much time on the toilet", "likes pineapple on pizza","snores too loud", "leaves the toilet seat up","is a communist","likes javascript", "thinks outside the box", "likes cats", "likes dogs", "drives a BMW", "drives an AUDI", "has night terrors", "is horny","lies to me","tells dad jokes","is too serious","hates me","kisses me","hugs me","touches me","has restless leg syndrome","goes to the gym","looks at my ass","looks weird","holds my hand","yells at me","ignores me" };
        private string[] feelingStrings = { "Just {0}.", "I feel {0}!", "That makes me {0}!", "{0}.", "I go through.","I endure.","I bear.", "I suffer.","I perceive.","Are you serious?","Get of my back!","Don't point at me!", "You're crazy!"};
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
            lastGeneratedPhrases.Add("Doctor0", "How does that make you feel? Who wants to continue?");
            lastGeneratedPhrases.Add("Doctor1", "How are you feeling right now? Who's next?");
            lastGeneratedPhrases.Add("Doctor2", "What feelings does that bring up? Who wants to elaborate?");
            lastGeneratedPhrases.Add("Doctor3", "How does that make you feel? Who wants to elaborate?");
            lastGeneratedPhrases.Add("Doctor4", "How are you feeling right now? Who wants to continue?");
            lastGeneratedPhrases.Add("Doctor5", "What feelings does that bring up? Who's next?");
            lastGeneratedPhrases.Add("DoctorStart", "Hello, let's start. Who wants to go first?");
            lastGeneratedPhrases.Add("DoctorEnd", "That's all the time we have today. See you next week.");
            var details = SessionLogic.GetSessionDetails();
            List<string> tempVerbs = new List<string>();
            tempVerbs.AddRange(verbs);
            for (int i = 0; i < details.roundCount; ++i)
            {
                string line = "";
                if (i > 0)
                    line = string.Format(feelingStrings[Random.Range(0, feelingStrings.Length)], emotions[Random.Range(0, emotions.Length)]) + " ";
                string verb1 = verbs[Random.Range(0, verbs.Length)];
                string accusationM = null;
                string accusationF = null;
                if (verb1.StartsWith("is")) {
                    int firstSpace = verb1.IndexOf(" ");
                    accusationM = line + string.Format("He is {0}{1}", Random.Range(0, 100) > 50 ? "always" : "never", verb1.Substring(firstSpace));
                    accusationF = line + string.Format("She is {0}{1}", Random.Range(0, 100) > 50 ? "always" : "never", verb1.Substring(firstSpace));
                }
                else { 
                    accusationM = line + string.Format("He {0} {1}", Random.Range(0, 100) > 50 ? "always" : "never", verb1);
                    accusationF = line + string.Format("She {0} {1}", Random.Range(0, 100) > 50 ? "always" : "never", verb1);
                }
                lastGeneratedPhrases.Add("lineM" + i, accusationM);
                lastGeneratedPhrases.Add("lineF" + i, accusationF);
                lastGeneratedPhrases.Add("lineD" + i, "Honk honk, hunk. Honk! Honk?");
            }
            foreach (var line in lastGeneratedPhrases) {
                Debug.Log("Generated: " + line.Key + " -> " + line.Value);
            }
        }

        public static string getPhrase(int index, SessionLogic.PersonType type)
        {
            return instance.internal_getPhrase(index, type);
        }

        public static string getDoctorPhrase()
        {
            return instance.internal_getPhrase("Doctor" + Random.Range(0,6));
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