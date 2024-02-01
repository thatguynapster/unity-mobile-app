using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [SerializeField] private GameObject loginView;

    [SerializeField] private GameObject signUpView;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenLoginView()
    {
        loginView.SetActive(true);
        signUpView.SetActive(false);
    }

    public void OpenSignupView()
    {
        signUpView.SetActive(true);
        loginView.SetActive(false);
    }

}
