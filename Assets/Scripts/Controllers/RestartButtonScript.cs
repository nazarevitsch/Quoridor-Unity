using UnityEngine;

namespace Controllers
{
    public class RestartButtonScript : MonoBehaviour
    {
        private bool IsActiv;

        public void ChangeActiveState()
        {
            IsActiv = !IsActiv;
            gameObject.SetActive(IsActiv);
        }
    }
}
