using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    Animator _animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void updateAnimator(float velocity)
    {
        _animator.SetFloat("Speed", velocity);
    }
}
