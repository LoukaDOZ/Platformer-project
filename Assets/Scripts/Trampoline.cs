using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isPlaying = false;
    
    public void Animate()
    {
        if (!isPlaying)
        {
            animator.SetTrigger("Bounce");
            isPlaying = true;
        }
    }

    public void AnimationStop()
    {
        isPlaying = false;
    }
}