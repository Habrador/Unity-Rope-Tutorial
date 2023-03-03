using UnityEngine;

public class RopeRenderer : MonoBehaviour{

    public float width = 0.1f;
    public LineRenderer _renderer;
    public RopeModel model;
    Vector3[] positions;

    void Start(){
        model = model ?? GetComponent<RopeModel>();
        _renderer = GetComponent<LineRenderer>();
    }

    public void Update(){
        _renderer.startWidth = _renderer.endWidth = width;
        var count = model.count;
        if(positions == null || positions.Length < count){
            positions = new Vector3[count];
        }
        for (int i = 0; i < count; i++){
            positions[i] = model[i];
        }
        _renderer.positionCount = count;
        _renderer.SetPositions(positions);
    }

}
