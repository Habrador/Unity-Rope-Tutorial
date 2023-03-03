using System.Collections.Generic;
using UnityEngine; using v3 = UnityEngine.Vector3;

public partial class RopeModel : MonoBehaviour{

    // Connects the last section in the list
    // In original model, was assumed to be fixed
    public ConnectedBody lastNodeConnectedBody;
    // Connects the first section in the list (index 0)
    // In original model, was assumed this is a body hanging from
    // the rope
    public ConnectedBody firstNodeConnectedBody;

    public float length = 7f;
    public float kRope = 40f;     // spring constant
    public float dRope = 2f;      // damping from rope friction
    public float aRope = 0.05f;   // damping from air resistance
    [Min(0)] public float mRopeSection = 0.2f;  // per section mass
    [Min(2)] public int sectionCount = 7;
    [Range(1, 10)] public int solverIterations = 1;
    [Range(1, 10)] public int maxStretchIterations = 2;
    public StretchLimiter stretchCorrection;
    [Header("Debug")]
    public bool debugLength = true;
    public string stretchPcnt;  // debug only
    [Header("Drawing")]
    public bool debugDraw;
    public float drawScale = 0.1f;
    public float accelDrawScale = 1f;
    public float velDrawScale = 1f;
    //
    float etl = 7f;
    List<RopeSection> sections = new ();
    List<v3> accel, allForces;
    EulerExtrapolation euler;
    HeunsExtrapolation heuns;

    public int count => sections.Count;

    public float actualLength{ get{
        var sum = 0f;
        for (var i = 0; i < count - 1; i++)
            sum += (this[i] - this[i + 1]).magnitude;
        return sum;
    }}

    public v3 this[int index] => sections[index].pos;

    void Start(){
        // Build the rope top-down
        //var pos = lastNodeConnectedBody.position;
        etl = length;
        var ropePositions = new List<v3>(sectionCount);
        var A = firstNodeConnectedBody.position;
        var B = lastNodeConnectedBody.position;
        var u = (B-A).normalized * ropeSectionLength;
        for (int i = 0; i < sectionCount; i++){
            v3 pos = B + u * i;
            ropePositions.Add(pos);
            //pos.y -= ropeSectionLength;
        }
        // ...then add sections bottom-up because it's easier to
        // add more sections to it if we have a winch
        for (int i = ropePositions.Count - 1; i >= 0; i--){
            sections.Add(new RopeSection(ropePositions[i]));
        }

    }

    void Update(){
        if(debugLength)
            stretchPcnt = $"{actualLength/length:P0}";
    }

    void FixedUpdate(){
        if (count <= 0) return;
        var timeStep = Time.fixedDeltaTime / (float)solverIterations;
        for (int i = 0; i < solverIterations; i++){
            UpdateRopeSimulation(sections, timeStep);
        }
        if(firstNodeConnectedBody.followRope){
            firstNodeConnectedBody.ForcePosAndLookAt(
                sections[0].pos,
                sections[1].pos - sections[0].pos
            );
        }
        if(lastNodeConnectedBody.followRope){
            lastNodeConnectedBody.ForcePosAndLookAt(
                sections[maxIndex].pos,
                sections[maxIndex-1].pos - sections[maxIndex].pos
            );
        }
    }

    private void UpdateRopeSimulation(List<RopeSection> sections, float timeStep){
        // TODO here should not update first section if driven
        // NOTE: we should not need to do this now, since we always
        // do it at the end.
        // Move the last position (top segment) to what the rope is attached to
        //RopeSection lastRopeSection = sections[count - 1];
        //lastRopeSection.pos = lastNodeConnectedBody.position;
        //sections[count - 1] = lastRopeSection;
        //
        var accel = EvalAccel(sections);
        var nextPosVelForwardEuler = euler.Eval(
            sections, accel, timeStep
        );
        // If either end not driven by the rope, pin pos/vel
        PinEndsAsNeeded(nextPosVelForwardEuler);
        // Calculate the next pos with Heun's method (Improved Euler)
        var accelFromEuler = EvalAccel(nextPosVelForwardEuler);
        var nextPosVelHeunsMethod = heuns.Eval(
            sections,
            accelFromEuler,
            nextPosVelForwardEuler,
            firstNodeConnectedBody.velocity,
            firstNodeConnectedBody.position,
            timeStep
        );
        // If either end not driven by the rope, pin pos/vel
        PinEndsAsNeeded(nextPosVelHeunsMethod);
        // Update all pos/vel in main list
        for (int i = 0; i < count; i++){
            sections[i] = nextPosVelHeunsMethod[i];
        }
        // Enforce max stretch
        for (int i = 0; i < maxStretchIterations; i++){
            stretchCorrection.Apply(
                sections, ropeSectionLength,
                hint: length / actualLength,
                backward: firstNodeConnectedBody.followRope,
                forward: lastNodeConnectedBody.followRope

            );
        }
        // Max stretch hack
        // if actual length is 10 but target length is 5
        // then target 5 x 5 / 10 = 2.5
        var w = length / actualLength; w *= w;
        etl = length * w * w;
    }

    // --------------------------------------------------------------

    // When doing a velocity/position update pass, ends will be
    // pinned depending on connected body state
    void PinEndsAsNeeded(List<RopeSection> s){
        if(!firstNodeConnectedBody.followRope){
            //ebug.Log("pin first connected body");
            s[0] = firstNodeConnectedBody.velPos;
        }
        if(!lastNodeConnectedBody.followRope){
            //ebug.Log("pin last connected body");
            s[maxIndex] = lastNodeConnectedBody.velPos;
        }
    }

    List<v3> EvalAccel(List<RopeSection> sections){
        accel.Clear();
        float k = kRope;  // spring
        float d = dRope;  // damping
        float a = aRope;  // damping from air resistance
        float m = mRopeSection; // mass per section
        float wantedLength = ropeSectionLength;
        // Calculate all forces once because some sections are using
        // the same force but negative
        allForces.Clear();
        for (int i = 0; i < count - 1; i++){
            // From Physics for game developers book, force exerted on
            // body 1 pos1 (above) - pos2
            v3 vectorBetween = sections[i + 1].pos - sections[i].pos;
            float distanceBetween = vectorBetween.magnitude;
            v3 dir = vectorBetween.normalized;
            float springForce = k * (distanceBetween - wantedLength);
            // Damping from rope friction
            // vel1 (above) - vel2
            float frictionForce = d * ((v3.Dot(sections[i + 1].vel - sections[i].vel, vectorBetween)) / distanceBetween);
            //The total force on the spring
            v3 springForceVec = -(springForce + frictionForce) * dir;
            // Flip sign for opposite force
            springForceVec = -springForceVec;
            allForces.Add(springForceVec);
            if(i == count - 2){
                allForces.Add(-springForceVec);
            }
        }
        // Loop through all safe last since connected, eval accel
        for (int i = 0; i < count; i++){
            var springForce = v3.zero;
            // Spring 1 - above
            springForce += allForces[i];
            // Spring 2 - below
            // The first spring is at the bottom so it doesnt have a
            // section below it
            if (i != 0) springForce -= allForces[i - 1];
            // Damping from air resistance depends on velocity, squared
            float vel = sections[i].vel.magnitude;
            var dampingForce = a * vel * vel * sections[i].vel.normalized;
            // The mass attached to this spring
            float springMass = m;
            //end of the rope is attached to a box with a mass
            if (IsDriven(i)){
                springMass += ConnectedBodyAt(i).mass;
            }
            var gravityForce = springMass * new v3(0f, -9.81f, 0f);
            var totalForce = springForce + gravityForce - dampingForce;
            // Acceleration a = F / m
            v3 acceleration = totalForce / springMass;
            accel.Add(acceleration);
        }
        //ebug.Log($"number of accel nodes: {accel.Count}");
        // Last segment's acceleration is 0 since attached to
        // something
        if(!lastNodeConnectedBody.followRope)
            accel[maxIndex] = v3.zero;
        if(!firstNodeConnectedBody.followRope)
            accel[0] = v3.zero;
        return accel;
    }

    ConnectedBody ConnectedBodyAt(int i){
        if(i == 0) return firstNodeConnectedBody;
        if(i == count - 1) return lastNodeConnectedBody;
        throw new System.Exception("Nothing connected");
    }

    bool IsDriven(int index)
    => (index == 0 && firstNodeConnectedBody.followRope)
    || (index == count - 1 && lastNodeConnectedBody.followRope);

    bool IsDriver(int index)
    => (index == 0 && !firstNodeConnectedBody.followRope)
    || (index == count - 1 && !lastNodeConnectedBody.followRope);

    void OnEnable(){
        int count = Mathf.Max(this.count, sectionCount);
        euler = new ();
        heuns = new ();
        accel = new List<v3>(count);
        allForces = new List<v3>(count);
    }

    void OnDisable(){ euler = null; heuns = null; accel = null; }

    void OnDrawGizmos(){
        if(!debugDraw) return;
        int i = 0;
        foreach(var k in sections){
            Debug.DrawRay(k.pos, k.vel * drawScale * velDrawScale, Color.green);
            if(i < accel.Count)
                Debug.DrawRay(k.pos, accel[i++] * drawScale * accelDrawScale, Color.red);
        }
    }

    int maxIndex => count - 1;

    float ropeSectionLength => etl / sectionCount;

}
