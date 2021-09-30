using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject textField;
    private bool isInUse;
    private bool isPaused;

    public void ChangeText()
    {
        if (!isPaused)
        {
            isInUse = !isInUse;
            if (isInUse)
            {
                textField.GetComponent<Text>().text = "STOP";
            }
            else
            {
                textField.GetComponent<Text>().text = "USE";
            }
        }
    }
    
    public void ChangePauseState()
    {
        isPaused = !isPaused;
    }
    
    public void ChangePauseStateToFalse()
    {
        isPaused = false;
    }
}
