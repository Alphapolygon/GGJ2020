using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2020
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private CameraController cameraController;
        [SerializeField]
        private CharacterController leftCharacter;
        [SerializeField]
        private CharacterController rightCharacter;
        [SerializeField]
        private GameObject leftPlayButton;
        [SerializeField]
        private GameObject rightPlayButton;
        [SerializeField] 
        private GameObject playButton;
        public enum GameState
        {
            MainMenu,
            Started,
            StartPlaySession,
            PlaySessionStarted,
            PlaySessionRunning,
            PlaySessionEnded
        }

        private const float DoctorSpeechWaitTime = 3.75f;
        private const float FemaleClientSpeechWaitTime = 3.75f;
        private const float MaleClientSpeechWaitTime = 5.75f;

        private static GameController instance = null;

        private GameState gameState = GameState.MainMenu;

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

        public void StartGamePressed()
        {
            if (gameState == GameState.MainMenu || gameState == GameState.PlaySessionEnded)
            {
                playButton.SetActive(false);
                StartCoroutine(ChangeState(GameState.Started));
            }
        }

        private IEnumerator ChangeState(GameState targetState)
        {
            switch (targetState)
            {
                case GameState.Started:
                    {
                        SessionLogic.InitSession();
                        cameraController.FadeInCamera();
                        yield return new WaitForSeconds(1);
                        gameState = targetState;
                        break;
                    }
                case GameState.StartPlaySession:
                    {
                        gameState = GameState.PlaySessionStarted;
                        WindowsVoice.getInstance().speak(SessionLogic.GetDoctorPrefix() + PhraseGenerator.getPhrase("DoctorStart"));
                        yield return new WaitForSeconds(DoctorSpeechWaitTime);
                        leftPlayButton.GetComponent<Animator>().SetTrigger("appear");
                        rightPlayButton.GetComponent<Animator>().SetTrigger("appear");
                        gameState = GameState.PlaySessionRunning;
                        break;
                    }
                case GameState.PlaySessionEnded:
                    {
                        playButton.SetActive(true);
                        cameraController.FadeOutCamera();
                        yield return new WaitForSeconds(1);
                        gameState = targetState;
                        break;
                    }
                default:
                    break;
            }
        }

        public void QuestionButtonClicked(int index)
        {
            StartCoroutine(HandleQuestionButtonClick(index));
        }

        private IEnumerator HandleQuestionButtonClick(int index)
        {
            leftPlayButton.GetComponent<Animator>().SetTrigger("disappear");
            rightPlayButton.GetComponent<Animator>().SetTrigger("disappear");
            bool correct = SessionLogic.HandleSelection(index);
            if (correct)
            {
                if (index == 0)
                {
                    leftCharacter.EmitLove();
                }
                else
                {
                    rightCharacter.EmitLove();
                }
            }
            SessionLogic.PersonType type = SessionLogic.GetPersonType(index);
            if (type == SessionLogic.PersonType.Male)
                yield return new WaitForSeconds(MaleClientSpeechWaitTime);
            else
                yield return new WaitForSeconds(FemaleClientSpeechWaitTime);

            if (SessionLogic.HasSessionEnded())
            {
                StartCoroutine(ChangeState(GameState.PlaySessionEnded));
            }
            else
            {
                WindowsVoice.getInstance().speak(SessionLogic.GetDoctorPrefix() + PhraseGenerator.getPhrase("Doctor"));
                yield return new WaitForSeconds(DoctorSpeechWaitTime);
                leftPlayButton.GetComponent<Animator>().SetTrigger("appear");
                rightPlayButton.GetComponent<Animator>().SetTrigger("appear");
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (gameState == GameState.Started)
            {
                StartCoroutine(ChangeState(GameState.StartPlaySession));
            }
        }
    }
}
