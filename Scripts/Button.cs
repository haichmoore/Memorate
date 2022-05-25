using System;
using UnityEngine;
using TMPro;

public class Button : MonoBehaviour
{
    int buttonNum;
    GameManager gameManager;
    Animator anim;

    [SerializeField] TextMeshPro buttontext;

    void Start()
    {
        anim = GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if(buttontext){
            buttonNum = Int32.Parse(buttontext.text);
        }
    }
    #region ButtonFunctionality
    private void OnMouseDown() {
        ButtonPressed();
    }

    private void OnMouseUp() {
        ButtonReleased();
    }
    
    public void ButtonPressed(){
        //if in attempt add to current streak, else do nothing
        anim.Play("Base Layer.pressDown");
    }

    public void ButtonReleased(){
        anim.Play("Base Layer.pressUp");
        if(gameManager.inAttempt & !gameManager.inAnimation){
            gameManager.AddNumber(buttontext.text);
        }
    }

    public void BackSpace(){
        if(gameManager.inAttempt & gameManager.inputStreak.Length > 0){
            gameManager.inputStreak = gameManager.inputStreak.Substring(0, gameManager.inputStreak.Length - 1);
        }
    }

    public void ImNext(){
        anim.Play("Base Layer.showNext");
    }
    #endregion

    #region HeaderButtons
    public void OpenClose(GameObject menu){
        menu.SetActive(!menu.activeSelf);
    }
    #endregion
}
