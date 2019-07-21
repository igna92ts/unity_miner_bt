using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeStates {
    SUCCESS = 0,
    FAILURE = 1,
    RUNNING = 2
}

public abstract class BTNode {
    protected NodeStates nodeState;
    public abstract NodeStates Evaluate();
}
