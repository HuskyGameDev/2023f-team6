using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraChoicesHandler : MonoBehaviour
{
    [SerializeField] private BarrierScriptable[] barriers;
    [SerializeField] private CenterpieceScriptable[] centerpieces;
    [SerializeField] private Skill TwoBarriers;
    private void OnEnable()
    {
        updateMenu();
    }

    void updateMenu()
    {
        #region Barrier buttons
        //Assuming order is Barricade -> Fishing nets -> Explosive Barricade -> Reinforced Barricade -> Floating Mine -> Platform Barrier
        //Barrier 1 buttons

        //Assume barricade is always unlocked
        for (int i = 1; i < 6; i++)
        {
            if (barriers[i].getUnlocked())
            {
                transform.GetChild(1).GetChild(2).GetChild(i + 1).GetComponent<Image>().color = Color.white;
                transform.GetChild(1).GetChild(2).GetChild(i + 1).GetComponent<Button>().interactable = true;
            }
            else
            {
                transform.GetChild(1).GetChild(2).GetChild(i + 1).GetComponent<Image>().color = new Color(72f / 255f, 72f / 255f, 72f / 255f);
                transform.GetChild(1).GetChild(2).GetChild(i + 1).GetComponent<Button>().interactable = false;
            }

        }
        /*  ChoiceOutlines does this
        //Update Outline
        BarrierScriptable chosenBarrier = FindObjectOfType<SettingsTracker>().getBarrier1();
        for (int i = 1; i <= 6; i++)
        {
            if (chosenBarrier == barriers[i])
                FindObjectOfType<ChoiceOutlines>().updateOutline1(transform.GetChild(1).GetChild(2).GetChild(i).transform);
        }
        */

        //Barrier 2 buttons
        if (TwoBarriers.getUnlocked() && numOfUnlockedBarriers() >= 2)
        {
            transform.GetChild(2).gameObject.SetActive(true);

            if (FindObjectOfType<SettingsTracker>().getBarrier1() != barriers[0])
                FindObjectOfType<ChoiceOutlines>().updateOutline1(transform.GetChild(1).GetChild(2).GetChild(1).transform);
            else
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (barriers[i].getUnlocked())
                    {
                        FindObjectOfType<ChoiceOutlines>().updateOutline1(transform.GetChild(1).GetChild(2).GetChild(i).transform);
                        break;
                    }
                }
            }
        }
        else
            transform.GetChild(2).gameObject.SetActive(false);

        for (int i = 1; i < 6; i++)
        {
            if (barriers[i].getUnlocked())
            {
                transform.GetChild(2).GetChild(2).GetChild(i + 1).GetComponent<Image>().color = Color.white;
                transform.GetChild(2).GetChild(2).GetChild(i + 1).GetComponent<Button>().interactable = true;
            }
            else
            {
                transform.GetChild(2).GetChild(2).GetChild(i + 1).GetComponent<Image>().color = new Color(72f / 255f, 72f / 255f, 72f / 255f);
                transform.GetChild(2).GetChild(2).GetChild(i + 1).GetComponent<Button>().interactable = false;

            }
        }
        /*
        //Update Outline
        chosenBarrier = FindObjectOfType<SettingsTracker>().getBarrier2();
        for (int i = 0; i < 6; i++)
        {
            if (chosenBarrier == barriers[i])
                FindObjectOfType<ChoiceOutlines>().updateOutline2(transform.GetChild(1).GetChild(2).GetChild(i).transform);
        }
        */
        #endregion

        #region Centerpieces
        for (int i = 0; i < 4; i++)
        {
            if (centerpieces[i].getUnlocked())
                transform.GetChild(3).GetChild(2).GetChild(i + 1).gameObject.SetActive(true);
            else
                transform.GetChild(3).GetChild(2).GetChild(i + 1).gameObject.SetActive(false);
        }
        #endregion
    }

    int numOfUnlockedBarriers()
    {
        int count = 0;
        foreach (BarrierScriptable barrier in barriers)
        {
            if (barrier.getUnlocked())
                count++;
        }
        return count;
    }
}
