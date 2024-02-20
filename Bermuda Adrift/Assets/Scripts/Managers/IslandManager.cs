using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class IslandManager : MonoBehaviour
{
    public static event Action<Island> islandDiscovered;

    [SerializeField] private GameObject islandPrefab;
    private GameObject island;

    [SerializeField] private float islandRarity;    //Chance for an island to appear after a round

    [SerializeField] private Island[] islands;

    private void OnEnable()
    {
        GameManager.onRoundEnd += createIsland;
        GameManager.OnRoundStart += removeIsland;
    }
    private void OnDisable()
    {
        GameManager.onRoundEnd -= createIsland;
        GameManager.OnRoundStart -= removeIsland;
    }

    private bool islandAppears()
    {
        if (Random.Range(0, 1f) < islandRarity)
            return true;

        return false;
    }

    #region Island randomness/rarity
    private Island randomIsland()
    {
        for (int i = 0; i < islands.Length; i++)
        {
            float random = Random.Range(0f, 1f);
            if (trueRarityCalc(i) >= random)
                return islands[i];
        }
        return islands[0];
    }
    private float trueRarityCalc(int index) //Goal is something like 10% rarity, will do the math to make that the actual probability
    {
        if (index == 0)
            return islands[index].getRarity();

        float previousRarities = 1 - islands[0].getRarity();

        for (int i = 1; i < index; i++)
        {
            previousRarities *= trueRarityRecursive(i);
        }
        //goal = chance to get that far * true rarity

        return islands[index].getRarity() / previousRarities;
    }
    private float trueRarityRecursive(int index)
    {
        if (index <= 0) return 1 - islands[index].getRarity();

        else return 1 - (islands[index].getRarity() / trueRarityDenominator(index));
    }
    private float trueRarityDenominator(int index)
    {
        float output = trueRarityRecursive(0);
        for (int i = 1; i < index; i++)
        {
            output *= trueRarityRecursive(index - i);
        }
        return output;
    }
    #endregion

    private void createIsland()
    {
        if (!islandAppears()) return;

        island = Instantiate(islandPrefab);  //Island animator will move it around properly
        Island chosenIsland = randomIsland();

        island.SendMessage("setIsland", chosenIsland);
        islandDiscovered?.Invoke(chosenIsland);
    }
    private void removeIsland()
    {
        if (island == null) return;

        if (island.GetComponent<Animator>() != null)
            island.GetComponent<Animator>().Play("SlideOut");

        if (island.GetComponent<CircleCollider2D>() != null)
            island.GetComponent<CircleCollider2D>().enabled = false;

        StartCoroutine(deleteIsland(5));
    }
    private IEnumerator deleteIsland(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(island);
    }

    private void interact(Island island)
    {
        if (island.getIslandType() == Island.islandType.Materials)
            FindObjectOfType<GameManager>().addScrap(island.getScrapBonus());

        if (island.getIslandType() == Island.islandType.Buff)
            FindObjectOfType<Attack>().StartCoroutine("Buff", island.getBuff());

        removeIsland();
    }
}
