using System;
using System.Collections.Generic;
using UnityEngine; using v3 = UnityEngine.Vector3;

// NOTE: Stretch correction is directional; proceed from one
// attached/driven end to the loose end or (bidi) middle of the rope.
[Serializable] public class StretchLimiter{

    public float minStretch = 0.9f;
    public float maxStretch = 1.1f;
    List<RopeSection> sections;  // all rope sections
    float length;  // the target length

    public void Apply(
        List<RopeSection> input, float length,
        bool backward, bool forward
    ){
        sections = input;
        this.length = length;
        if(backward == forward){
            Debug.Log("Bidi");
            Bidi();
        }else if(backward){
            Debug.Log("Backward");
            Backward();
        }else if(forward){
            Debug.Log("Forward");
            Forward();
        }
    }

    void Backward(){
        for (int i = count - 1; i >= 1; i--) EnforceMaxStretch(
            x: sections[i], y: sections[i - 1],
            i: i, flip: -1, length: length
        );
    }

    void Forward(){
        for (int i = 0; i < count - 1; i++) EnforceMaxStretch(
            x: sections[i], y: sections[i + 1],
            i: i, flip: 1, length: length
        );
    }

    void Bidi(){
        int max = count - 1;
        for (int i = 0; i < count/2; i++){
            EnforceMaxStretch(
                x: sections[i],
                y: sections[i + 1],
                i: i, flip: +1, length: length
            );
            EnforceMaxStretch(
                x: sections[max - i],
                y: sections[max - i - 1],
                i: max - i, flip: -1, length: length
            );
        }  // end for loops
    }

    void EnforceMaxStretch(RopeSection x, RopeSection y, int i, int flip, float length){
        float dist = (x.pos - y.pos).magnitude;
        // How stretched/compressed?
        float stretch = dist / length;
        if (stretch > maxStretch){
            // How far do we need to compress the spring?
            float compressLength = dist - (length * maxStretch);
            // In what direction should we compress the spring?
            var compressDir = (x.pos - y.pos).normalized;
            var change = compressDir * compressLength;
            MoveSection(change, i + flip);
        }
        else if (stretch < minStretch){
            // How far do we need to stretch the spring?
            float stretchLength = (length * minStretch) - dist;
            // In what direction should we compress the spring?
            var stretchDir = (y.pos - x.pos).normalized;
            var change = stretchDir * stretchLength;
            MoveSection(change, i + flip);
        }
    }

    // Move a rope section based on stretch/compression
    // TODO what is this like... adding 1 number???
    private void MoveSection(Vector3 change, int listPos){
        var next = sections[listPos];
        v3 pos = next.pos;
        pos += change;
        next.pos = pos;
        sections[listPos] = next;
    }

    int count => sections.Count;

}
