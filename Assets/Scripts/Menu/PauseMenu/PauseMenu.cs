using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        
        private void OnEnable()
        {
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        public void Resume()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }

        public void BackMainMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}