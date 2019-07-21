using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : BTNode {
    private BTNode child;
    public BTNode node {
        get { return child; }
    }
    public Inverter(BTNode node) {
        child = node;
    }
    public override NodeStates Evaluate() { 
        switch (child.Evaluate()) { 
            case NodeStates.FAILURE: 
                this.nodeState = NodeStates.SUCCESS; 
                return this.nodeState; 
            case NodeStates.SUCCESS: 
                this.nodeState = NodeStates.FAILURE; 
                return this.nodeState; 
            case NodeStates.RUNNING: 
                this.nodeState = NodeStates.RUNNING; 
                return this.nodeState; 
        } 
        this.nodeState = NodeStates.SUCCESS; 
        return this.nodeState; 
    }
}
