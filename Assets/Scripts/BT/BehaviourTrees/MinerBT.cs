using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerBT : MonoBehaviour {

    private Miner miner;
    public GameObject house;
    public GameObject bank;
    public GameObject bar;
    public Pathfinding pathfinding;
    public struct Context {
        public List<Node> pathToDestination;
        public Node nextNode;
        public GameObject destinationObject;
        public float timer;
    }
    public Context context;
    public BTNode behaviourTree;
    void Start() {
        context = new Context();
        context.timer = 0;
        miner = GetComponent<Miner>();

        var mainSelector = new Selector();
        mainSelector.AddChild(MakeSleepSequence());
        mainSelector.AddChild(MakeMiningSequence());
        mainSelector.AddChild(MakeDepositSequence());
        mainSelector.AddChild(MakeDrinkingSequence());

        this.behaviourTree = mainSelector;
    }

    void Update() {
        this.behaviourTree.Evaluate();
    }

    public delegate NodeStates ActionNodeDelegate();

    public BTNode SetDestination(GameObject destination) {
        var setDestination = new ActionNode(delegate () {
            Debug.Log("Set Destination");
            context.destinationObject = destination;
            return NodeStates.SUCCESS;
        });
        return setDestination;
    }
    public BTNode MakeSleepSequence() {
        var hasNoEnergy = new Inverter(HasEnergy());
        var setDestination = SetDestination(house);
        var goToDestination = MakeGoToDestinationSequence();
        var sleep = new ActionNode(delegate () {
            Debug.Log("Sleeping");
            context.timer += Time.deltaTime; 
            if (context.timer >= miner.sleepTime) {
                context.timer = 0;
                miner.Rest(); 
                return NodeStates.SUCCESS;
            } else {
                return NodeStates.RUNNING;
            }
        });
        var sleepSequence = new Sequence();
        sleepSequence.AddChild(hasNoEnergy);
        sleepSequence.AddChild(setDestination);
        sleepSequence.AddChild(goToDestination);
        sleepSequence.AddChild(sleep);
        return sleepSequence;
    }
    
    public BTNode HasEnergy() {
        return new ActionNode(delegate () {
            Debug.Log("Has no Energy");
            return miner.energy > 0 ? NodeStates.SUCCESS : NodeStates.FAILURE;
        });
    }

    public BTNode MakeMiningSequence() {
        var goToMineSequence = new Sequence();
        goToMineSequence.AddChild(AreMinesAvailable());
        goToMineSequence.AddChild(MakeGoToDestinationSequence());
        
        var miningSequence = new UntilFail();
        miningSequence.AddChild(HasEnergy());
        miningSequence.AddChild(goToMineSequence);
        miningSequence.AddChild(new ActionNode(delegate () {
            Debug.Log("Mining");
            context.timer += Time.deltaTime; 
            if (context.timer >= miner.mineTime) {
                context.timer = 0;
                miner.MineOre(); 
                return NodeStates.SUCCESS;
            } else {
                return NodeStates.RUNNING;
            }
        }));

        return new Inverter(miningSequence);
    }

    public BTNode AreMinesAvailable() {
        return new ActionNode(delegate () {
            Debug.Log("Available Mines?");
            var myPos = this.transform.position;
            var shortestDistance = 0f;
            GameObject currentMine = null;
            for (var i = 0; i < miner.mines.Length; i++) {
                var newDistance = Vector2.Distance(myPos, miner.mines[i].transform.position); 
                var mine = miner.mines[i].GetComponent<Mine>();
                if ((shortestDistance == 0 || newDistance < shortestDistance) && !mine.isEmpty()) {
                    shortestDistance = newDistance;
                    currentMine = miner.mines[i];
                }
            }
            miner.TargetMine = currentMine; 
            context.destinationObject = currentMine;
            return currentMine != null ? NodeStates.SUCCESS : NodeStates.FAILURE;
        });
    }

    public BTNode MakeDepositSequence() {
        var depositSequence = new Sequence();
        depositSequence.AddChild(new ActionNode(delegate () {
            Debug.Log("Gold in Pocket?");
            return miner.goldInPocket > 0 ? NodeStates.SUCCESS : NodeStates.FAILURE;       
        }));
        depositSequence.AddChild(SetDestination(bank));
        depositSequence.AddChild(MakeGoToDestinationSequence());
        depositSequence.AddChild(new ActionNode(delegate () {
            Debug.Log("Depositing");
            context.timer += Time.deltaTime; 
            if (context.timer >= miner.depositTime) {
                context.timer = 0;
                miner.goldInBank += miner.goldInPocket;
                miner.goldInPocket = 0;
                return NodeStates.SUCCESS;
            } else {
                return NodeStates.RUNNING;
            }
        }));
        return depositSequence;
    }

    public BTNode MakeDrinkingSequence() {
        var drinkingSequence = new Sequence();
        drinkingSequence.AddChild(SetDestination(bar));
        drinkingSequence.AddChild(MakeGoToDestinationSequence());
        var drinkUntilFail = new UntilFail();
        drinkUntilFail.AddChild(HasEnergy());
        drinkUntilFail.AddChild(new ActionNode(delegate () {
            Debug.Log("Drinking");
            context.timer += Time.deltaTime; 
            if (context.timer >= miner.depositTime) {
                context.timer = 0;
                miner.Drink();
                return NodeStates.SUCCESS;
            } else {
                return NodeStates.RUNNING;
            }
        }));
        drinkUntilFail.AddChild(new Inverter(AreMinesAvailable()));
        drinkingSequence.AddChild(drinkUntilFail);
        return drinkingSequence;
    }

    public BTNode MakeGoToDestinationSequence() {
        var findPath = new ActionNode(delegate () {
            Debug.Log("Find Path");
            var pathToDestination = new List<Node>();
            pathToDestination = pathfinding.FindPath(this.transform.position, context.destinationObject.transform.position, context.destinationObject);
            if (pathToDestination != null) {
                context.pathToDestination = pathToDestination;
                return NodeStates.SUCCESS;
            } else {
                return NodeStates.FAILURE;
            }
        });
        var untilFail = new UntilFail();
        untilFail.AddChild(new ActionNode(delegate () {
            Debug.Log("Pop From Path");
            Node node = context.pathToDestination[0];
            context.pathToDestination.RemoveAt(0);
            context.nextNode = node;
            return NodeStates.SUCCESS;
        }));
        untilFail.AddChild(new Inverter(WillCollideWithDestination()));
        untilFail.AddChild(new ActionNode(delegate () {
            Debug.Log("Walk to Node");
            var node = context.nextNode;
            this.transform.position = Vector2.MoveTowards(this.transform.position, node.position, miner.movementSpeed * Time.deltaTime);
            return (Vector2)this.transform.position == node.position ? NodeStates.SUCCESS : NodeStates.RUNNING;
        }));

        var sequence = new Sequence();
        sequence.AddChild(findPath);
        sequence.AddChild(untilFail);
        return sequence;
    }

    public BTNode WillCollideWithDestination() {
        return new ActionNode(delegate () {
            Debug.Log("Will collide with destination");
            var destCollider = context.destinationObject.GetComponent<Collider2D>();
            if (destCollider.bounds.Contains(context.nextNode.position)) {
                return NodeStates.SUCCESS;
            }
            return NodeStates.FAILURE;
        });
    }
}
