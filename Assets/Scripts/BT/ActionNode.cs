using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : BTNode {
    public delegate NodeStates ActionNodeDelegate();
    private ActionNodeDelegate action;
    public ActionNode(ActionNodeDelegate action) {
        this.action = action;
    }
    public override NodeStates Evaluate() {
        switch (action()) {
            case NodeStates.SUCCESS:
                this.nodeState = NodeStates.SUCCESS;
                return this.nodeState;
            case NodeStates.FAILURE:
                this.nodeState = NodeStates.FAILURE;
                return this.nodeState;
            case NodeStates.RUNNING:
                this.nodeState = NodeStates.RUNNING;
                return this.nodeState;
            default:
                this.nodeState = NodeStates.FAILURE;
                return this.nodeState;
        }
    }
}
