using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnCharSelect : MonoBehaviour
{
    CharacterInfoSO charInfo;
    [SerializeField] GameObject ability;

    public void InitializeData(CharacterInfoSO selectedCharInfo)
    {
        charInfo = selectedCharInfo;
    }

    public void AddDescription(TextMeshProUGUI text)
    {
        text.text = charInfo.description;
    }
    /*
    public void AddAbility(int index)
    {
        GameObject newAbility = Instantiate(ability, this.transform.position, this.transform.rotation, this.transform);
        newAbility.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = charInfo.abilities[index].name;
        newAbility.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = charInfo.abilities[index].description;
    }
    */
}
