using System.Collections.Generic;
using v3 = UnityEngine.Vector3;

public class EulerExtrapolation{

    List<RopeSection> @out = new ();

    // NOTE: 'Euler' is just linearly extrapolating velocity from
    // accel and position from velocity.
    // This model does not care what is attached to the rope or not,
    // update every node
    public List<RopeSection> Eval(
        List<RopeSection> sections, List<v3> accel, float timeStep
    ){
        var count = accel.Count;
        @out.Clear();
        for (int i = 0; i < count; i++){
            var section = RopeSection.zero;
            section.vel = sections[i].vel + accel[i] * timeStep;
            section.pos = sections[i].pos + sections[i].vel * timeStep;
            @out.Add(section);
        }
        return @out;
    }

    //public string[] magnitudes
    //=> from e in @out select $"{@out.pos} {@out.vel}";

}
