using System.Collections.Generic;
using v3 = UnityEngine.Vector3;

// NOTE: Heuns extrapolation refines the result obtained via
// linear extrapolation
// This model does not care what is attached to the rope or not;
// update every node
public class HeunsExtrapolation{

    List<RopeSection> @out = new ();

    public List<RopeSection> Eval(
        List<RopeSection> sections,
        List<v3> accel, List<RopeSection> nextPosVelFromEuler,
        v3 tailVelocity, v3 tailPosition,
        float timeStep
    ){
        int count = accel.Count;
        @out.Clear();
        var halfStep = 0.5f * timeStep;
        for (int i = 0; i < count; i++){
            var section = RopeSection.zero;
            // Heuns method
            // vel = vel + (acc + accFromForwardEuler) * 0.5 * t
            section.vel =
                sections[i].vel
                + (accel[i] + accel[i]) * halfStep;
            // pos = pos + (vel + velFromForwardEuler) * 0.5f * t
            section.pos = sections[i].pos
                + (sections[i].vel + nextPosVelFromEuler[i].vel)
                                                     * halfStep;
            @out.Add(section);
        }
        return @out;
    }

    //public string[] magnitudes
    //=> from e in @out select $"{@out.pos} {@out.vel}";

}
