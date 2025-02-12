using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        Animator animator;
        PlayerMovementController movementController;
        private string _currentAnimation = "";

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movementController = GetComponentInParent<PlayerMovementController>();
        }
        
        private void Update()
        {
            CheckAnimation();
        }   

        private void ChangeAnimation(string animationName, float transitionTime = 0, float playTime = 0)
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
                Excute();
                yield return new WaitForSeconds(playTime);
            }      
            void Excute()
            {
                if(_currentAnimation == animationName)
                {
                    return;
                }
                animator.CrossFade(animationName, transitionTime);
                _currentAnimation = animationName;
            }
        }

        private void CheckAnimation()
        {
            if(movementController.IsMoving)
            {
                ChangeAnimation("Player Move");
            }
            else
            {
                ChangeAnimation("Player Idle");
            }
        }
    }
}
