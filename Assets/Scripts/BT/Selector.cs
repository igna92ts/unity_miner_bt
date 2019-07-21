using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : BTNode {
    protected List<BTNode> childNodes = new List<BTNode>();
    private BTNode runningNode;
    public void AddChild(BTNode node) {
        childNodes.Add(node);
    }
    public override NodeStates Evaluate() {
        foreach (BTNode node in childNodes) { 
            if (runningNode != null && node != runningNode) continue;
            switch (node.Evaluate()) { 
                case NodeStates.FAILURE: 
                    runningNode = null;
                    continue; 
                case NodeStates.SUCCESS: 
                    runningNode = null;
                    this.nodeState = NodeStates.SUCCESS; 
                    return this.nodeState; 
                case NodeStates.RUNNING: 
                    runningNode = node;
                    this.nodeState = NodeStates.RUNNING;
                    return this.nodeState; 
                default: 
                    continue; 
            } 
        } 
        this.nodeState = NodeStates.FAILURE; 
        return this.nodeState; 
    }
}
