using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(S_O_Saving saver);
    void SaveData(S_O_Saving saver);
}
