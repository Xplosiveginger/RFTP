using UnityEngine;

public class MenuStartCutscene : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator= GetComponent<Animator>();
    }
    public void Skip()
    {
        animator.SetTrigger("skip");
    }
}
