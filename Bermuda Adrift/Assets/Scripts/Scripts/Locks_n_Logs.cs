using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Locks_n_Logs
{
    public bool unlocked;
    public bool logged;

    public Locks_n_Logs()
    {
        this.unlocked = false;
        this.logged = false;
    }
    public Locks_n_Logs(bool unlocked)
    {
        this.unlocked = unlocked;
        this.logged = false;
    }
}
