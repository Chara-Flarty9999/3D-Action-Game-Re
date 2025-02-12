using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //ÉVÅ[ÉìêÿÇËë÷Ç¶óp

public class Button_Game_ReStart : MonoBehaviour
{
    bool _enableControl = false;
    int _chooseButton;
    Transform _buttonX;
    //Transform _titleX;
    Image _blackfade;
    [SerializeField] AudioClip _startSe;
    [SerializeField] GameObject introduce;
    AudioSource _titleBGM;
    AudioSource _audiosource;
    GameObject selectButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    void pushed()
    {
        SceneManager.LoadScene("Title");
    }

    // Update is called once per frame
    void Update()
    {
        if (_enableControl)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _audiosource.PlayOneShot(_startSe);
                _enableControl = false;
                selectButton.SetActive(false);
                pushed();
            }

        }
    }
}
