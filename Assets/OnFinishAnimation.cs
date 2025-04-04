using System.Collections;
using UnityEngine;

namespace Rougelike2D
{
    public class OnFinishAnimation : StateMachineBehaviour
    {
    [SerializeField] private string animation;
    [SerializeField] private float crossfade = 0.05f;
    [HideInInspector] public bool cancel = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cancel = false;
        PlayerController.Instance.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(stateInfo.length - crossfade);

            if (cancel) yield break;

            AnimatorBrain target = animator.GetComponent<AnimatorBrain>();
            target.Play(animation, crossfade);
        }
    }
    }
}
