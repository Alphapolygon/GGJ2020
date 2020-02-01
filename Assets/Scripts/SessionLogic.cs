using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class SessionLogic : MonoBehaviour
    {
        public enum PersonType
        {
            Male,
            Female,
            Duck
        }

        public static const string[] MALE_NAMES = { "Raipe", "Pertsa", "George", "Vladimir", "Tauski", "Harold" };
        public static const string[] FEMALE_NAMES = { "Raija", "Tuktuk", "Johku", "Pam", "Tatjana" };

        public class SessionDetail
        {
            public List<int> correctAnswerIndex;
            public string name1;
            public PersonType type1;
            public string name2;
            public PersonType type2;
            public int currentIndex;
            public int roundCount;

            public SessionDetail()
            {
                roundCount = Random.Range(5, 8);
                currentIndex = 0;
                correctAnswerIndex = new List<int>();
                type1 = Random.Range(0, 3);
                type2 = Random.Range(0, 3);
                name1 = type1 == PersonType.Male ? MALE_NAMES[Random.Range(0, MALE_NAMES.Length)] : type1 == PersonType.Female ? FEMALE_NAMES[Random.Range(0, FEMALE_NAMES.Length)] : "Duck";
                name2 = type2 == PersonType.Male ? MALE_NAMES[Random.Range(0, MALE_NAMES.Length)] : type2 == PersonType.Female ? FEMALE_NAMES[Random.Range(0, FEMALE_NAMES.Length)] : "Duck";
                for (int i = 0; i < roundCount; ++i)
                {
                    correctAnswerIndex.Add(Random.Range(0, 100) > 50 ? 0 : 1);
                }
            }
        }
        private static SessionLogic instance = null;

        private SessionDetail sessionDetails;

        private bool sessionEnded;

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

        public static void InitSession()
        {
            instance.internal_InitSession();
        }

        public static SessionDetail GetSessionDetails() {
            return instance.internal_GetSessionDetails();
        }
        
        private SessionDetail internal_GetSessionDetails() {
            if (sessionEnded || sessionDetails == null) {
                internal_InitSession();
            }
            return sessionDetails;
        }

        private void internal_InitSession()
        {
            sessionDetails = new SessionDetail();
            sessionEnded = false;
        }

        public static bool HandleSelection(int selection)
        {
            return instance.internal_HandleSelection();
        }

        private bool internal_HandleSelection(int selection) {
            bool correct = sessionDetails.correctAnswerIndex[i++] == selection; 
            sessionEnded = i >= sessionDetails.correctAnswerIndex.Count;
            return correct;
        }
    }
}
