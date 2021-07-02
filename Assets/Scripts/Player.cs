using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int health;
    public int score;

    public abstract void FreezePlayer();
    public abstract void UnfreezePlayer();
}
