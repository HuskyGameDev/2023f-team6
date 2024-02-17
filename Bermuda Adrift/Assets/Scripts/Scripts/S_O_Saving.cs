using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_O_Saving
{

    #region Towers
    public bool Ballista;
    public bool Cannon;
    public bool LightningRod;
    public bool MachineGun;
    public bool ScrapTurret;
    public bool TorpedoLauncher;
    public bool TotemPole;
    public bool TriCannon;

    public bool getTowerSave(string name)
    {
        if (name.CompareTo("T_Ballista") == 0) return Ballista;
        else if (name.CompareTo("T_Cannon") == 0) return Cannon;
        else if (name.CompareTo("T_LightningRod") == 0) return LightningRod;
        else if (name.CompareTo("T_Machine Gun") == 0) return MachineGun;
        else if (name.CompareTo("T_ScrapTurret") == 0) return ScrapTurret;
        else if (name.CompareTo("T_TorpedoLauncher") == 0) return TorpedoLauncher;
        else if (name.CompareTo("T_TotemPole") == 0) return TotemPole;
        else if (name.CompareTo("T_TriCannon") == 0) return TriCannon;

        Debug.Log(name + " had no matches");
        return false;
    }
    public void setTowerSave(string name, bool unlock)
    {
        if (name.CompareTo("T_Ballista") == 0) Ballista = unlock;
        else if (name.CompareTo("T_Cannon") == 0) Cannon = unlock;
        else if (name.CompareTo("T_LightningRod") == 0) LightningRod = unlock;
        else if (name.CompareTo("T_Machine Gun") == 0) MachineGun = unlock;
        else if (name.CompareTo("T_ScrapTurret") == 0) ScrapTurret = unlock;
        else if (name.CompareTo("T_TorpedoLauncher") == 0) TorpedoLauncher = unlock;
        else if (name.CompareTo("T_TotemPole") == 0) TotemPole = unlock;
        else if (name.CompareTo("T_TriCannon") == 0) TriCannon = unlock;
        else
            Debug.Log(name + " had no matches");
    }
    #endregion

    #region Barriers
    #endregion

    #region Characters
    public bool Pirate;
    public bool Pilot;
    public bool Old_Man;

    public bool getCharacterSave(string name)
    {
        if (name.CompareTo("P_Pirate") == 0) return Pirate;
        else if (name.CompareTo("P_Pilot") == 0) return Pilot;
        else if (name.CompareTo("P_OldMan") == 0) return Old_Man;

        Debug.Log("No matches for " + name);
        return false;
    }
    public void setCharacterSave(string name, bool unlock)
    {
        if (name.CompareTo("P_Pirate") == 0) Pirate = unlock;
        else if (name.CompareTo("P_Pilot") == 0) Pilot = unlock;
        else if (name.CompareTo("P_OldMan") == 0) Old_Man = unlock;
        else
            Debug.Log("No matches for " + name);
    }
    #endregion

    #region Skills
    #endregion

    #region Achievements
    #endregion

    #region Settings
    #endregion

    #region Last-picked items
    #endregion
    public S_O_Saving()
    {
        Ballista = false;
        Cannon = true;  //Start the game with just the cannon
        LightningRod = false;
        MachineGun = false;
        ScrapTurret = false;
        TorpedoLauncher = false;
        TotemPole = false;
        TriCannon = false;

        Pirate = true;  //Start with only the pirate unlocked
        Pilot = false;
        Old_Man = false;
    }
}
