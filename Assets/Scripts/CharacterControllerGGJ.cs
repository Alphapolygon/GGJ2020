using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class CharacterControllerGGJ : MonoBehaviour
    {
        public Animator animator;

        public GameObject playButton;
        public int index;

        public ParticleSystem m_particleSystem;
        public bool Talk = false;
        // Start is called before the first frame update
        void Start()
        {
            animator = this.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            animator.SetBool("talk", Talk);
        }

        public void QuestionButtonClicked()
        {
            GGJ2020.GameController.QuestionButtonClicked(index);
        }

        public void EmitLove()
        {
            m_particleSystem.Play();
        }
    }
}