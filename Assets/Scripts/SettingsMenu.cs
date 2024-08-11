/*
* Author: Rayn Bin Kamaludin
* Date: 2024-08-03
* Description: Manages the settings menu, including volume and mute settings.
*/

using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the volume slider UI
    public Toggle muteToggle; // Reference to the mute toggle UI

    private float defaultVolume = 0.5f;

    private void Start()
    {
        // Set default values
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", defaultVolume);
        muteToggle.isOn = PlayerPrefs.GetInt("Mute", 0) == 1;

        // Apply the settings
        SetVolume(volumeSlider.value);
        MuteAudio(muteToggle.isOn);
    }

    // Function to adjust the volume
    public void SetVolume(float volume)
    {
        if (muteToggle.isOn)
        {
            AudioListener.volume = 0; // Mute volume if muted
        }
        else
        {
            AudioListener.volume = volume; // Adjust volume
        }

        PlayerPrefs.SetFloat("Volume", volume); // Save volume setting
    }

    // Function to mute/unmute audio
    public void MuteAudio(bool isMuted)
    {
        if (isMuted)
        {
            AudioListener.volume = 0; // Mute audio
            PlayerPrefs.SetInt("Mute", 1); // Save mute setting
        }
        else
        {
            SetVolume(volumeSlider.value); // Restore volume
            PlayerPrefs.SetInt("Mute", 0); // Save unmute setting
        }
    }
}
