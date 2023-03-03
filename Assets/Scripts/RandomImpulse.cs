using UnityEngine;
using v3 = UnityEngine.Vector3;

public class RandomImpulse : MonoBehaviour
{
    [Range(0.00001f, 0.1f)]
    public float chance = 0.001f;
    public float power = 1f;

    void Update(){
        if(Random.value > chance) return;
        var u = Random.insideUnitSphere;
        u.z = 0f;
        GetComponent<Rigidbody>().AddForce
                                  (power * u, ForceMode.Impulse);
    }
}
