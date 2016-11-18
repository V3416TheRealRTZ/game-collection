using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
	
	// Update is called once per frame

    public void setTarget(Transform _target)
    {
        target = _target;
    }

	void Update ()
    {
	    if (target)
	    {
	        Vector3 point = GetComponent<Camera>().WorldToViewportPoint(new Vector3(target.position.x, target.position.y + 0.75f, target.position.z));
            Vector3 delta = new Vector3(target.position.x, target.position.y + 0.75f, target.position.z) 
                - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
	        Vector3 destination = transform.position + delta;
	        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
	    }
	}
}
