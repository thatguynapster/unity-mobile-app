using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginView;

    [SerializeField] private GameObject signUpView;

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
