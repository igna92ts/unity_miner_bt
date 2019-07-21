using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntilFail : Sequence
{
    public override NodeStates Evaluate() {
       var result = base.Evaluate(); 
       if (this.nodeState == NodeStates.FAILURE) {
           this.nodeState = NodeStates.SUCCESS;
           return this.nodeState;
       } else {
           this.nodeState = NodeStates.RUNNING;
           return this.nodeState;
       }
    }
}
