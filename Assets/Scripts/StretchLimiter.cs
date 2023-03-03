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
    List<v3> @out = new List<v3>();

    public void Apply(
        List<RopeSection> input, float length,
        float hint,
        bool backward, bool forward
    ){
        sections = input;
        this.length = length * hint;
        if(backward == forward){
            //BidiDefault();
            if(hint < 1f) CompressBidi(hint); else ExpandBidi();
        }else if(backward){
            Backward();
        }else if(forward){
            Forward();
        }
    }

    void CompressBidi(float hint){
        InitOutput();
        v3 u = (sections[count-1].pos - sections[0].pos) / count;
        for(int i = 1; i < count - 1; i++){
            v3 left = sections[i - 1].pos, right = sections[i + 1].pos;
            v3 self = sections[i].pos;
            v3 flat = sections[0].pos + u * i;
            v3 smooth = (left + right)/2;  // neighbours, averaged
            var μ = 1 - hint;
            // Let's say the target length is 0.8:
            // First, lerp by 0.2 towards 'smooth' node - this is
            // not very effective but visibly helps getting a much
            // smoother result.
            // Next and more drastically, move towards the "flat"
            // image by μ; value of μ should remain small, and
            // vanish as we tend towards desired length
            // The first (less effective) step may be removed for
            // comparison.
            @out[i] = v3.Lerp(self, smooth, μ);
            @out[i] = v3.Lerp(@out[i], flat, μ * μ);
        }
        // NOTE - 'UpdateSection' exists cause mutable structs are bs
        for(int i = 1; i < count - 1; i++){
            UpdateSection(@out[i], i);
        }
    }

    // Simple method where each segment (spannning two consecutive
    // points) pushes points outwards; good enough for stretching,
    // which usually does not show artefacts (spring model errors
    // tend to overstretch the rope, not compress it)
    void ExpandBidi(){
        InitOutput();
        for(int i = 0; i < count - 1; i++){
            v3 A0 = sections[i].pos, B0 = sections[i + 1].pos;
            var P = (A0 + B0) / 2;
            var u = (B0 - A0).normalized;
            var s = length * 0.5f;
            @out[i + 0] += P - u * s;
            @out[i + 1] += P + u * s;
        }
        for(int i = 1; i < count - 1; i++){
            UpdateSection(@out[i]/2, i);
        }
        UpdateSection(@out[0], 0);
        UpdateSection(@out[count - 1], count - 1);
    }

    // May work either for compressing or stretching the rope; legacy
    // approach adapted from single attachment cases; produces
    // bathtub shaped curves ;-)
    void BidiDefault(){
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
        }
    }

    void InitOutput(){
        @out.Clear();
        for(int i = 0; i < count; i++) @out.Add(v3.zero);
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

    void EnforceMaxStretch(
        RopeSection x, RopeSection y, int i, int flip, float length
    ){
        float dist = (x.pos - y.pos).magnitude;
        float stretch = dist / length;
        if (stretch > maxStretch){
            float compressLength = dist - (length * maxStretch);
            var compressDir = (x.pos - y.pos).normalized;
            var change = compressDir * compressLength;
            MoveSection(change, i + flip);
        }
        else if (stretch < minStretch){
            float stretchLength = (length * minStretch) - dist;
            var stretchDir = (y.pos - x.pos).normalized;
            var change = stretchDir * stretchLength;
            MoveSection(change, i + flip);
        }
    }

    // NOTE - bs used to just update one point cause mutable struct
    private void UpdateSection(Vector3 P, int listPos){
        var next = sections[listPos];
        next.pos = P;
        sections[listPos] = next;
    }

    // NOTE - bs used to just update one point cause mutable struct
    private void MoveSection(Vector3 change, int listPos){
        var next = sections[listPos];
        v3 pos = next.pos;
        pos += change;
        next.pos = pos;
        sections[listPos] = next;
    }

    int count => sections.Count;

}
