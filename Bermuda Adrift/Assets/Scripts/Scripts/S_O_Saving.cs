using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_O_Saving
{

    #region Towers
    public Locks_n_Logs Ballista;
    public Locks_n_Logs Cannon;
    public Locks_n_Logs LightningRod;
    public Locks_n_Logs MachineGun;
    public Locks_n_Logs ScrapTurret;
    public Locks_n_Logs TorpedoLauncher;
    public Locks_n_Logs TotemPole;
    public Locks_n_Logs TriCannon;

    public Locks_n_Logs getTowerSave(string name)
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
        return null;
    }
    /*public void setTowerSave(string name, bool unlock)
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
    */
    #endregion

    #region Barriers
    public Locks_n_Logs Barricade;
    public Locks_n_Logs Fish_Nets;
    #endregion

    #region Characters
    public Locks_n_Logs Pirate;
    public Locks_n_Logs Pilot;
    public Locks_n_Logs Old_Man;

    public Locks_n_Logs getCharacterSave(string name)
    {
        if (name.CompareTo("P_Pirate") == 0) return Pirate;
        else if (name.CompareTo("P_Pilot") == 0) return Pilot;
        else if (name.CompareTo("P_OldMan") == 0) return Old_Man;

        Debug.Log("No matches for " + name);
        return null;
    }
    /*public void setCharacterSave(string name, bool unlock)
    {
        if (name.CompareTo("P_Pirate") == 0) Pirate = unlock;
        else if (name.CompareTo("P_Pilot") == 0) Pilot = unlock;
        else if (name.CompareTo("P_OldMan") == 0) Old_Man = unlock;
        else
            Debug.Log("No matches for " + name);
    }
    */
    #endregion

    #region Skills
    #endregion

    #region Enemy Logs
    public Locks_n_Logs Angler_Fish;
    public Locks_n_Logs Crow;
    public Locks_n_Logs Electric_Eel;
    public Locks_n_Logs Fish_Person;
    public Locks_n_Logs Hamster;
    public Locks_n_Logs Jellyfish;
    public Locks_n_Logs Octopus;
    public Locks_n_Logs Ray;
    public Locks_n_Logs Seagull;
    public Locks_n_Logs Shadow_Creature;
    public Locks_n_Logs Shark;
    public Locks_n_Logs Swordfish;
    public Locks_n_Logs Turtle;

    public Locks_n_Logs Sea_Serpent;
    public Locks_n_Logs Kraken;
    public Locks_n_Logs Shadow_Self;

    public Locks_n_Logs getEnemySave(string name)
    {
        if (name.CompareTo("E_AnglerFish") == 0) return Angler_Fish;
        if (name.CompareTo("E_Crow") == 0) return Crow;
        if (name.CompareTo("E_ElectricEel") == 0) return Electric_Eel;
        if (name.CompareTo("E_FishPersonBlue") == 0) return Fish_Person;
        if (name.CompareTo("E_Hamster") == 0) return Hamster;
        if (name.CompareTo("E_Jellyfish") == 0) return Jellyfish;
        if (name.CompareTo("E_Octopus") == 0) return Octopus;
        if (name.CompareTo("E_Ray") == 0) return Ray;
        if (name.CompareTo("E_Seagull") == 0) return Seagull;
        if (name.CompareTo("E_ShadowCreature") == 0) return Shadow_Creature;
        if (name.CompareTo("E_Shark") == 0) return Shark;
        if (name.CompareTo("E_Swordfish") == 0) return Swordfish;
        if (name.CompareTo("E_Turtle") == 0) return Turtle;
        if (name.CompareTo("BossE_Serpent") == 0) return Sea_Serpent;
        if (name.CompareTo("BossE_Kraken") == 0) return Kraken;
        if (name.CompareTo("BossE_ShadowSelf") == 0) return Shadow_Self;

        Debug.Log("No matches for " + name);
        return null;
    }
    #endregion

    #region Achievements
    public Locks_n_Logs Click_Me;
    public Locks_n_Logs Completionist;
    public Locks_n_Logs Devout_Follower;
    public Locks_n_Logs Did_We_Escape;
    public Locks_n_Logs Even_Further_Beyond;
    public Locks_n_Logs Fin_Soup;
    public Locks_n_Logs Full_Arsenal;
    public Locks_n_Logs Gotcha;
    public Locks_n_Logs Gotta_Go_Fast;
    public Locks_n_Logs Hemophobia;
    public Locks_n_Logs Immovable_Object;
    public Locks_n_Logs In_And_Out;
    public Locks_n_Logs Journalist;
    public Locks_n_Logs Kamikaze_Director;
    public Locks_n_Logs Land_Ho;
    public Locks_n_Logs Looping_Memories;
    public Locks_n_Logs Man_In_The_Mirror;
    public Locks_n_Logs New_Captain;
    public Locks_n_Logs Not_Even_Worth_The_Time;
    public Locks_n_Logs Pew_Pew;
    public Locks_n_Logs Removable_Object;
    public Locks_n_Logs Seedling;
    public Locks_n_Logs Single_Minded;
    public Locks_n_Logs Skill_Issue;
    public Locks_n_Logs Swordmaster;
    public Locks_n_Logs Tea_Time;
    public Locks_n_Logs Thats_A_Lotta_Damage;
    public Locks_n_Logs Unlucky;
    public Locks_n_Logs Unstoppable_Force;
    public Locks_n_Logs Were_Rich;
    public Locks_n_Logs Why_Are_You_Hoarding;
    public Locks_n_Logs World_Tree;

    public Locks_n_Logs getAchievementSave(string name)
    {
        if (name.CompareTo("Ach_Click_Me") == 0) return Click_Me;
        if (name.CompareTo("Ach_Completionist") == 0) return Completionist;
        if (name.CompareTo("Ach_Devout_Follower") == 0) return Devout_Follower;
        if (name.CompareTo("Ach_Did_We_Escape") == 0) return Did_We_Escape;
        if (name.CompareTo("Ach_Even_Further_Beyond") == 0) return Even_Further_Beyond;
        if (name.CompareTo("Ach_Fin_Soup") == 0) return Fin_Soup;
        if (name.CompareTo("Ach_Full_Arsenal") == 0) return Full_Arsenal;
        if (name.CompareTo("Ach_Gotcha") == 0) return Gotcha;
        if (name.CompareTo("Ach_Gotta_Go_Fast") == 0) return Gotta_Go_Fast;
        if (name.CompareTo("Ach_Hemophobia") == 0) return Hemophobia;
        if (name.CompareTo("Ach_Immovable_Object") == 0) return Immovable_Object;
        if (name.CompareTo("Ach_In_And_Out") == 0) return In_And_Out;
        if (name.CompareTo("Ach_Journalist") == 0) return Journalist;
        if (name.CompareTo("Ach_Kamikaze_Director") == 0) return Kamikaze_Director;
        if (name.CompareTo("Ach_Land_Ho") == 0) return Land_Ho;
        if (name.CompareTo("Ach_Looping_Memories") == 0) return Looping_Memories;
        if (name.CompareTo("Ach_Man_In_The_Mirror") == 0) return Man_In_The_Mirror;
        if (name.CompareTo("Ach_New_Captain") == 0) return New_Captain;
        if (name.CompareTo("Ach_Not_Even_Worth_The_Time") == 0) return Not_Even_Worth_The_Time;
        if (name.CompareTo("Ach_Pew_Pew") == 0) return Pew_Pew;
        if (name.CompareTo("Ach_Removable_Object") == 0) return Removable_Object;
        if (name.CompareTo("Ach_Seedling") == 0) return Seedling;
        if (name.CompareTo("Ach_Single_Minded") == 0) return Single_Minded;
        if (name.CompareTo("Ach_Skill_Issue") == 0) return Skill_Issue;
        if (name.CompareTo("Ach_Swordmaster") == 0) return Swordmaster;
        if (name.CompareTo("Ach_Tea_Time") == 0) return Tea_Time;
        if (name.CompareTo("Ach_Thats_A_Lotta_Damage") == 0) return Thats_A_Lotta_Damage;
        if (name.CompareTo("Ach_Unlucky") == 0) return Unlucky;
        if (name.CompareTo("Ach_Unstoppable_Force") == 0) return Unstoppable_Force;
        if (name.CompareTo("Ach_Were_Rich") == 0) return Were_Rich;
        if (name.CompareTo("Ach_Why_Are_You_Hoarding") == 0) return Why_Are_You_Hoarding;
        if (name.CompareTo("Ach_World_Tree") == 0) return World_Tree;

        Debug.Log("No matches for " + name);
        return null;
    }
    #endregion

    #region Settings
    public AudioData audioData;
    public VideoData videoData;
    #endregion

    #region Last-picked items
    #endregion
    public S_O_Saving()
    {
        #region Towers
        Ballista = new Locks_n_Logs();
        Cannon = new Locks_n_Logs();
        Cannon.unlocked = true;     //Start the game with just the cannon
        LightningRod = new Locks_n_Logs();
        MachineGun = new Locks_n_Logs();
        ScrapTurret = new Locks_n_Logs();
        TorpedoLauncher = new Locks_n_Logs();
        TotemPole = new Locks_n_Logs();
        TriCannon = new Locks_n_Logs();
        #endregion

        #region Barriers
        Barricade = new Locks_n_Logs();
        Fish_Nets = new Locks_n_Logs();
        #endregion

        #region Characters
        Pirate = new Locks_n_Logs();
        Pirate.unlocked = true;     //Start with only the pirate unlocked
        Pilot = new Locks_n_Logs();
        Old_Man = new Locks_n_Logs();
        #endregion

        #region Enemies
        Angler_Fish = new Locks_n_Logs();
        Crow = new Locks_n_Logs();
        Electric_Eel = new Locks_n_Logs();
        Fish_Person = new Locks_n_Logs();
        Hamster = new Locks_n_Logs();
        Jellyfish = new Locks_n_Logs();
        Octopus = new Locks_n_Logs();
        Ray = new Locks_n_Logs();
        Seagull = new Locks_n_Logs();
        Shadow_Creature = new Locks_n_Logs();
        Shark = new Locks_n_Logs();
        Swordfish = new Locks_n_Logs();
        Turtle = new Locks_n_Logs();

        //Bosses
        Sea_Serpent = new Locks_n_Logs();
        Kraken = new Locks_n_Logs();
        Shadow_Self = new Locks_n_Logs();
        #endregion

        #region Achievements
        Click_Me = new Locks_n_Logs();
        Completionist = new Locks_n_Logs();
        Devout_Follower = new Locks_n_Logs();
        Did_We_Escape = new Locks_n_Logs();
        Even_Further_Beyond = new Locks_n_Logs();
        Fin_Soup = new Locks_n_Logs();
        Full_Arsenal = new Locks_n_Logs();
        Gotcha = new Locks_n_Logs();
        Gotta_Go_Fast = new Locks_n_Logs();
        Hemophobia = new Locks_n_Logs();
        Immovable_Object = new Locks_n_Logs();
        In_And_Out = new Locks_n_Logs();
        Journalist = new Locks_n_Logs();
        Kamikaze_Director = new Locks_n_Logs();
        Land_Ho = new Locks_n_Logs();
        Looping_Memories = new Locks_n_Logs();
        Man_In_The_Mirror = new Locks_n_Logs();
        New_Captain = new Locks_n_Logs();
        Not_Even_Worth_The_Time = new Locks_n_Logs();
        Pew_Pew = new Locks_n_Logs();
        Removable_Object = new Locks_n_Logs();
        Seedling = new Locks_n_Logs();
        Single_Minded = new Locks_n_Logs();
        Skill_Issue = new Locks_n_Logs();
        Swordmaster = new Locks_n_Logs();
        Tea_Time = new Locks_n_Logs();
        Thats_A_Lotta_Damage = new Locks_n_Logs();
        Unlucky = new Locks_n_Logs();
        Unstoppable_Force = new Locks_n_Logs();
        Were_Rich = new Locks_n_Logs();
        Why_Are_You_Hoarding = new Locks_n_Logs();
        World_Tree = new Locks_n_Logs();
        #endregion

        #region Settings
        audioData = new AudioData();
        videoData = new VideoData();
        #endregion
    }
}
