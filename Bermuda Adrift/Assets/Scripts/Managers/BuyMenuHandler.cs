using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyMenuHandler : MonoBehaviour
{
    [SerializeField] Tower[] towers;
   [SerializeField] BarrierScriptable[] barriers;
    int xPadding = 3;
    int yPadding = 3;
    int initialX = 17;
    int initialY = 5;

    void Start()
    {
        int xMult = 0;
        int yMult = 0;
        foreach (Tower t in towers)
        {
            GameObject tower = new GameObject();
            Button button = tower.AddComponent<Button>();
            Image image = tower.AddComponent<Image>();
            image.sprite = t.getImage();
            button.targetGraphic = image;
            tower.transform.parent = this.transform;
            if (xMult % 2 == 0)
            {
                tower.transform.position = new Vector3(initialX, initialY - yPadding * yMult, transform.position.z);
            }
            else
            {
                tower.transform.position = new Vector3(initialX + xPadding, initialY - yPadding * yMult, transform.position.z);
                yMult++;
            }

            tower.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            xMult++;
        }
    }

    void Update()
    {
        
    }
}
