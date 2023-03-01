using System;
using UnityEngine; using v3 = UnityEngine.Vector3;

[Serializable]
public class ConnectedBody{

    public Transform transform;
    public bool followRope = false;

    public v3 position => transform.position;
    public v3 velocity => body.velocity;
    public float mass => body.mass;

    Rigidbody body => transform.GetComponent<Rigidbody>();

    public RopeSection velPos
    => new RopeSection(pos: position, vel: velocity);

    public void ForcePosAndLookAt(v3 pos, v3 up){
        body.isKinematic = true;
        transform.position = pos;
        transform.up = up;
    }

}
