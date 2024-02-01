using UnityEngine;


public class LookAtObject : MonoBehaviour
{

    Transform _transform;

    Transform _lookAt;

    public void Init(Transform lookAt)
    {  
        _transform = transform;
        _lookAt = lookAt;
    }

    
    void Update()
    {
        if(_lookAt != null)
        {

            _transform.LookAt(_lookAt);
        }
    }
}
 