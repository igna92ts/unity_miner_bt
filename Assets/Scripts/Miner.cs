using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public GameObject[] mines;
    public GameObject bank;
    public GameObject house;
    public GameObject bar;
    public int goldInPocket = 0;
    public int goldInBank = 0;
    public int energy = 20;
    private int mineAmount = 20;
    public float movementSpeed = 3f;
    public float mineCooldown = 1;
    public float sleepTime = 5f;
    public float mineTime = 1f;
    public float depositTime = 5f;
    private GameObject targetMine;
    public GameObject TargetMine { set { targetMine = value; } get { return targetMine; } }

    public void Start() {
        mines = GameObject.FindGameObjectsWithTag("Mine");
        this.targetMine = mines[0];
    }
    public void Rest() {
        energy = 20;
    }
    public void Drink() {
        energy--;
    }
    public void MineOre() {
        var actualAmount = TargetMine.GetComponent<Mine>().ReduceOreBy(mineAmount);
        energy--;
        this.goldInPocket += actualAmount;
    }
}
