using UnityEngine;
using UnityEngine.UI;

public class UISettingsPanelScreenManager : UIPanelScreenManager
{
    protected override void Awake()
    {
        base.Awake();

        volumeMusicSlider.value = 0.05f;
        volumeEffectsSlider.value = 0.05f;
        currentMusicVolume = volumeMusicSlider.value;
        currentEffectsVolume = volumeEffectsSlider.value;

        volumeMusicSlider.onValueChanged.AddListener(OnVolumeMusicChanged);
        volumeEffectsSlider.onValueChanged.AddListener(OnVolumeEffectsChanged);
    }

    [SerializeField] public Slider volumeMusicSlider;
    [SerializeField] public Slider volumeEffectsSlider;
    private float currentMusicVolume;
    private float currentEffectsVolume;


    private void OnVolumeMusicChanged(float newValue)
    {
        currentMusicVolume = newValue;
    }

    private void OnVolumeEffectsChanged(float newValue)
    {
        currentEffectsVolume = newValue;
    }

    public void Return()
    {
        HideUI();
    }
}
