using UnityEngine;

public class AutoHideAnimation : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            gameObject.SetActive(false);
    }
}