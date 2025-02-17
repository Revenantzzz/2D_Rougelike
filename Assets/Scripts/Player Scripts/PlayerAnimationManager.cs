using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        Animator animator;
        public string CurrentAnimation { get; private set; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void ChangeAnimation(string animationName, float transitionTime = 0.05f, float playTime = 0)
        {
            if(playTime > 0)
            {
                StartCoroutine(Wait());
            }
            else
            {
                Excute();
            }

            IEnumerator Wait()
            {
                yield return new WaitForSeconds(playTime - transitionTime);
                Excute();
            }      
            void Excute()
            {
                if(CurrentAnimation == animationName)
                {
                    return;
                }
                CurrentAnimation = animationName;
                animator.CrossFade(animationName, transitionTime);
            }
        }
        
    }
}
