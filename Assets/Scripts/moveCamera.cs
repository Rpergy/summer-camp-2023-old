using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public Vector3 topDownPosition;
    public Vector3 topDownAngle;

    public Vector3 threeQuatersPosition;
    public Vector3 threeQuartersAngle;

    public void topDown() {
        gameObject.transform.position = topDownPosition;
        gameObject.transform.rotation = Quaternion.Euler(topDownAngle);
        gameObject.GetComponent<Camera>().orthographic = true;
    }

    public void threeQuarters() {
        gameObject.transform.position = threeQuatersPosition;
        gameObject.transform.rotation = Quaternion.Euler(threeQuartersAngle);
        gameObject.GetComponent<Camera>().orthographic = false;
    }
}
