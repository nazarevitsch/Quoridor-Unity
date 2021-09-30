using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButtonScript : MonoBehaviour
{
    private bool IsActiv;

    public void ChangeActiveState()
    {
        IsActiv = !IsActiv;
        gameObject.SetActive(IsActiv);
    }
}
