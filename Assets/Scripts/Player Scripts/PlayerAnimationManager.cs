using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        Animator animator;
        PlayerController _controller;
        private string _currentAnimation = "";

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _controller = GetComponentInParent<PlayerController>();
        }
        private void Start()
        {
            _controller.OnPlayerAttack += PlayAttackAnimation;
        }

        private void Update()
        {
            CheckAnimation();
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
                if(_currentAnimation == animationName)
                {
                    return;
                }
                _currentAnimation = animationName;
                animator.CrossFade(animationName, transitionTime);
            }
        }

        private void CheckAnimation()
        {
            if(_currentAnimation == "Player Attack")
            {
                return;
            }   
            switch(_controller.CurrentState)
            {
                case PlayerState.Idle:
                    ChangeAnimation("Player Idle");
                    break;
                case PlayerState.Moving:
                    ChangeAnimation("Player Move");
                    break;
                case PlayerState.Jumping:
                    ChangeAnimation("Player Jump");
                    break;
                case PlayerState.Hit:
                    ChangeAnimation("Player Hit");
                    break;
                case PlayerState.Blocking:
                    ChangeAnimation("Player Block");
                    break;
                case PlayerState.Dead:
                    ChangeAnimation("Player Dead");
                    break;
            }
        }
        private void PlayAttackAnimation()
        {
            animator.SetFloat("Attack Count", _controller.AttackCounter);
            animator.SetFloat("Attack Type", _controller.AttackType);
            Debug.Log(_controller.AttackCounter);
            ChangeAnimation("Player Attack");
        }
        
    }
}
