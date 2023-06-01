using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    public Transform springStartTransform;
    public Transform springEndTransform;



    private void OnDrawGizmos()
    {
        Vector3 p1 = springStartTransform.position;
        Vector3 p2 = springEndTransform.position;

        float distance = (p1 - p2).magnitude;

        //Vector3 vec = p1 + -Vector3.up * distance;
        Vector3 vec = p1 + -Vector3.up * distance;

        //vec = Quaternion.Euler(45f, 45f, 45f) * vec;

        //vec = Quaternion.Euler(45f, 0f, 0f) * Quaternion.Euler(0f, 45f, 0f) * Quaternion.Euler(0f, 0f, 45f) * vec;

        //vec = Quaternion.Euler(45f, 0f, 0f) * vec;
        //vec = Quaternion.Euler(0f, 45f, 0f) * vec;
        //vec = Quaternion.Euler(0f, 0f, 45f) * vec;

        Vector3 dir = (p2 - p1).normalized;

        //Debug.Log(dir);

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        //vec -= p1;

        //Debug.Log(rot.eulerAngles);

        //Our vectors forward is in down direction, so we have to translate it to the forward direction, which is what LookRotation cares about
        rot *= Quaternion.FromToRotation(Vector3.down, Vector3.forward);

        vec = rot * vec;

        //vec += p1;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(p1, vec);


    }
}
