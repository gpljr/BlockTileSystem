﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [SerializeField] GameObject UIPanel;
    float tempMusicVolume;
    float tempAudioVolume;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider audioSlider;

    [SerializeField] Image MusicButton;
    [SerializeField] Image AudioButton;
    [SerializeField] Sprite MusicOn;
    [SerializeField] Sprite MusicOff;
    [SerializeField] Sprite AudioOn;
    [SerializeField] Sprite AudioOff;

    [SerializeField] GameObject cameraControl;

    // Use this for initialization
    void Start () {
        HideUIPanel();
        musicSlider.onValueChanged.AddListener(delegate {
                musicValueUpdate();
            });
        musicSlider.onValueChanged.AddListener(delegate {
                audioValueUpdate();
            });
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ShowUIPanel();
        }
	
    }
    public void ShowUIPanel () {
        UIPanel.SetActive(!UIPanel.activeSelf);
        cameraControl.GetComponent<CameraControl>().CameraBlur(UIPanel.activeSelf);
    }
    public void HideUIPanel () {
        UIPanel.SetActive(false);
        cameraControl.GetComponent<CameraControl>().CameraBlur(false);
    }
    public void QuitGame () {
        Application.Quit();
    }
    public void RestartGame () {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void MuteMusic () {
        if (LevelCode.musicVolume > 0) {
            //mute
            tempMusicVolume = LevelCode.musicVolume;
            LevelCode.musicVolume = 0f;
            musicSlider.value = 0f;
            MusicButton.sprite = MusicOff;
        } else {
            //unmute
            LevelCode.musicVolume = tempMusicVolume;
            musicSlider.value = tempMusicVolume;
            MusicButton.sprite = MusicOn;
        }
    }
    public void MuteAudio () {
        if (LevelCode.audioVolume > 0) {
            //mute
            tempAudioVolume = LevelCode.audioVolume;
            LevelCode.audioVolume = 0f;
            audioSlider.value = 0f;
            AudioButton.sprite = AudioOff;
        } else {
            //unmute
            LevelCode.audioVolume = tempAudioVolume;
            audioSlider.value = tempAudioVolume;
            AudioButton.sprite = AudioOn;
        }
    }
    void musicValueUpdate () {
        LevelCode.musicVolume = musicSlider.value;
    }
    void audioValueUpdate () {
        LevelCode.audioVolume = audioSlider.value;
    }
}
