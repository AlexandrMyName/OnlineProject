using UnityEngine;

public class Jump : MonoBehaviour
{


    Rigidbody rigidbody;
    public float jumpStrength = 2;
    public event System.Action Jumped;

   

    void Ыефк()
    {
        // Get rigidbody.
        rigidbody = GetComponent<Rigidbody>();
    }

    
}
