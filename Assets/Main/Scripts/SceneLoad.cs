using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main
{
    [RequireComponent(typeof(Button))]
    public class SceneLoad : MonoBehaviour
    {
        [SerializeField, Scene] private string sceneName;
        
        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}