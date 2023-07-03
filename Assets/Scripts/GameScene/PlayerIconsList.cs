using System.Collections.Generic;
using UnityEngine;

public class PlayerIconsList : MonoBehaviour
{
    [SerializeField] private List<Sprite> playerIconsList;

    public Sprite getIcon(int i)
    {
        return playerIconsList[i];
    }
}
