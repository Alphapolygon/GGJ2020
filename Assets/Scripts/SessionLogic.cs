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

        public static string[] MALE_NAMES = { "Cal", "Tom", "Davis", "Hank", "John", "Paul", "Ryan", "Raipe", "Pertsa", "George", "Vladimir", "Tauski", "Harold", "Donald", "Mohamed", "Kang"};
        public static string[] FEMALE_NAMES = { "Raija", "Tuktuk", "Johku", "Pam", "Tatjana", "Sophia", "Olivia", "Ava", "Emily", "Mia", "Melanie", "Angel", "Khushi", "Zeynep", "Chizu" };

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
                roundCount = 5;
                currentIndex = 0;
                correctAnswerIndex = new List<int>();
                type1 = (PersonType)Random.Range(0, 2);
                type2 = (PersonType)Random.Range(0, 2);
                name1 = type1 == PersonType.Male ? MALE_NAMES[Random.Range(0, MALE_NAMES.Length)] : type1 == PersonType.Female ? FEMALE_NAMES[Random.Range(0, FEMALE_NAMES.Length)] : "Duck";
                name2 = type2 == PersonType.Male ? MALE_NAMES[Random.Range(0, MALE_NAMES.Length)] : type2 == PersonType.Female ? FEMALE_NAMES[Random.Range(0, FEMALE_NAMES.Length)] : "Duck";
                for (int i = 0; i < roundCount; ++i)
                {
                    correctAnswerIndex.Add(Random.Range(0, 100) > 50 ? 0 : 1);
                }
            }
        }

        public static string GetDoctorPrefix()
        {
            return "<pitch absmiddle='-10'>";
        }

        private string GetTypePrefix(PersonType type)
        {
            return type == PersonType.Female ? "<pitch absmiddle='10'> <rate absspeed ='5'>" : type == PersonType.Male ? "<pitch absmiddle='-10'> <rate absspeed ='-2'>" : "<pitch absmiddle='10'> <rate absspeed ='5'>";
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

        public static SessionDetail GetSessionDetails()
        {
            return instance.internal_GetSessionDetails();
        }

        private SessionDetail internal_GetSessionDetails()
        {
            if (sessionEnded || sessionDetails == null)
            {
                internal_InitSession();
            }
            return sessionDetails;
        }

        public static bool HasSessionEnded()
        {
            return instance.internal_HasSessionEnded();
        }

        private bool internal_HasSessionEnded()
        {
            return sessionEnded;
        }

        private void internal_InitSession()
        {
            sessionDetails = new SessionDetail();
            sessionEnded = false;
        }

        public static bool HandleSelection(int selection)
        {
            return instance.internal_HandleSelection(selection);
        }

        public static PersonType GetPersonType(int index)
        {
            return instance.internal_GetPersonType(index);
        }

        private PersonType internal_GetPersonType(int index)
        {
            PersonType type = index == 0 ? sessionDetails.type1 : sessionDetails.type2;
            return type;
        }

        private bool internal_HandleSelection(int selection)
        {
            PersonType type = selection == 0 ? sessionDetails.type2 : sessionDetails.type1;
            PersonType talkerType = selection == 0 ? sessionDetails.type1 : sessionDetails.type2;
            string talkerName = selection == 0 ? sessionDetails.name1 : sessionDetails.name2;
            WindowsVoice.getInstance().speak(talkerName, GetTypePrefix(talkerType), PhraseGenerator.getPhrase(sessionDetails.currentIndex, type));
            bool correct = sessionDetails.correctAnswerIndex[sessionDetails.currentIndex++] == selection;
            sessionEnded = sessionDetails.currentIndex >= sessionDetails.correctAnswerIndex.Count;
            return correct;
        }
    }
}
