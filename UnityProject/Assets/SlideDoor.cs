using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoor : MonoBehaviour
{
    Animator animator;
    public bool isHolding = true;
    public PickupController pickupController;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isHolding = pickupController.target != null;
    }

    public void OpenDoor()
    {
        if (!isHolding)
        {
            animator.SetBool("NotHolding", true);
            // disable update
            this.enabled = false;
        }
    }
}
