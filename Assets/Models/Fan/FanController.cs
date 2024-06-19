using UnityEngine;
using UnityEngine.UI;

public class FanController : MonoBehaviour
{
    private Animator animator;
    public Button spinButton; // Assign this in the Inspector

    void Start()
    {
        // animator.SetBool("isSpinning", false);
        animator = GetComponent<Animator>();
        // Adds a listener to the button, which invokes the ToggleAnimation method when the button is clicked
        spinButton.onClick.AddListener(ToggleAnimation);
    }

    private void ToggleAnimation()
    {
        // Toggle the value of IsSpinning
        bool isSpinning = animator.GetBool("IsSpinning");
        animator.SetBool("IsSpinning", !isSpinning);
    }
}