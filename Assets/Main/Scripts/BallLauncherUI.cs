using UnityEngine;
using UnityEngine.UI;


public class BallLauncherUI : MonoBehaviour
{
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private Image reloadIndicator;


    private void OnEnable()
    {
        //ballLauncher.OnReloadEvent += HandleReload;
    }

    private void OnDisable()
    {
        //ballLauncher.OnReloadEvent -= HandleReload;
    }

    private void HandleReload(float progress)
    {
        reloadIndicator.fillAmount = progress;
        reloadIndicator.gameObject.SetActive(progress >= 0.99f ? false : true);
    }
}