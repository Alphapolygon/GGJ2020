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
        private CharacterControllerGGJ leftCharacter;
        [SerializeField]
        private CharacterControllerGGJ rightCharacter;

        [SerializeField]
        private GameObject leftPlayButton;
        [SerializeField]
        private GameObject rightPlayButton;
        [SerializeField] 
        private GameObject playButton;
        [SerializeField]
        private GameObject malePrefab;
        [SerializeField]
        private GameObject femalePrefab;


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
                        var parent = leftCharacter.transform.parent;
                        
                        
                        GameObject tempLeft;
                        GameObject tempRight;
                        var leftTrans = leftCharacter.transform;
                        var rightTrans = rightCharacter.transform;
                        if (SessionLogic.GetPersonType(0) == SessionLogic.PersonType.Male) {
                            tempLeft = GameObject.Instantiate(malePrefab, leftTrans.position, leftTrans.rotation, parent);
                            tempLeft.transform.localScale = new Vector3(tempLeft.transform.localScale.x*-1, tempLeft.transform.localScale.y, tempLeft.transform.localScale.z);
                        }
                        else 
                            tempLeft = GameObject.Instantiate(femalePrefab, leftTrans.position, leftTrans.rotation, parent);
                        if (SessionLogic.GetPersonType(1) == SessionLogic.PersonType.Male)
                            tempRight = GameObject.Instantiate(malePrefab, rightTrans.position, rightTrans.rotation, parent);
                        else {
                            tempRight = GameObject.Instantiate(femalePrefab, rightTrans.position, rightTrans.rotation, parent);
                            tempRight.transform.localScale = new Vector3(tempRight.transform.localScale.x*-1, tempRight.transform.localScale.y, tempRight.transform.localScale.z);
                        }
                        Destroy(leftCharacter.gameObject);
                        Destroy(rightCharacter.gameObject);
                        yield return null;
                        leftCharacter = tempLeft.GetComponent<CharacterControllerGGJ>();
                        rightCharacter = tempRight.GetComponent<CharacterControllerGGJ>();
                        leftPlayButton = leftCharacter.playButton;
                        rightPlayButton = rightCharacter.playButton;
                        leftCharacter.index = 0;
                        rightCharacter.index = 1;
                        if (leftCharacter == null || rightCharacter == null)
                            Debug.Log("no component");

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

        public static void QuestionButtonClicked(int index)
        {
            instance.StartCoroutine(instance.HandleQuestionButtonClick(index));
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
            if (index == 0) leftCharacter.Talk = true;
            else if (index == 1) rightCharacter.Talk = true;
            SessionLogic.PersonType type = SessionLogic.GetPersonType(index);
            if (type == SessionLogic.PersonType.Male)
                yield return new WaitForSeconds(MaleClientSpeechWaitTime);
            else
                yield return new WaitForSeconds(FemaleClientSpeechWaitTime);
            if (index == 0) leftCharacter.Talk = false;
            else if (index == 1) rightCharacter.Talk = false;
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
