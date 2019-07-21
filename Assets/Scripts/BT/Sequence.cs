using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : BTNode {
    protected List<BTNode> childNodes = new List<BTNode>();
    public void AddChild(BTNode node) {
        childNodes.Add(node);
    }
    private BTNode runningNode;
    public override NodeStates Evaluate() {
        foreach (BTNode node in childNodes) {
            if (runningNode != null && node != runningNode) continue;
            switch (node.Evaluate()) {
                case NodeStates.FAILURE:
                    this.nodeState = NodeStates.FAILURE;
                    return this.nodeState;
                case NodeStates.SUCCESS:
                    runningNode = null;
                    this.nodeState = NodeStates.SUCCESS;
                    continue;
                case NodeStates.RUNNING:
                    runningNode = node;
                    this.nodeState = NodeStates.RUNNING;
                    return this.nodeState;
                default:
                    this.nodeState = NodeStates.SUCCESS;
                    return this.nodeState;
            }
        }
        if (nodeState != NodeStates.RUNNING)
            Reset();
        return this.nodeState;
    }
    public void Reset() {
        runningNode = null;
    }
}
