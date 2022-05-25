using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    

    void Start()
    {
        InitialSetup();
    }

    

    
    
    #region Setup
    Color color;

    [SerializeField] GameObject rules;
    [SerializeField] TextMeshProUGUI highScore, currentScore;
    [SerializeField] TextMeshPro currentStreakText; 

    public string currentStreak, inputStreak;
    public bool inAttempt, inAnimation, inMenu, waitingForTomorrow;
    public int lastDayCompleted;

    void InitialSetup(){
        color = Color.yellow;
        currentStreak =  CurrentStreak();
        waitingForTomorrow = HaveIDoneToday();
        currentScore.text = "Current Streak: " + (currentStreak.Length-1).ToString();
        ExistingPlayerCheck();
        SetText();
    }
    void ExistingPlayerCheck(){

        if(!PlayerPrefs.HasKey("ExistingPlayer")){
            rules.SetActive(true);
            PlayerPrefs.SetInt("ExistingPlayer", 1);
            PlayerPrefs.SetInt("lastDayCompleted",-1);
        }
    }

    bool HaveIDoneToday(){
        DateTime currentTime = DateTime.Now;
        return (currentTime.DayOfYear == PlayerPrefs.GetInt("lastDayCompleted") & currentStreak.Length >= 3);
    }

    void SetText(int greaterThan = 3){
        DateTime currentTime = DateTime.Now;
        if(waitingForTomorrow){
            currentStreakText.color = Color.red;
            currentStreakText.text = "UNRDY";
        }else{
            currentStreakText.text = "";
            currentStreakText.color = Color.yellow;
        }
    }
    #endregion

    #region AttemptManagement
    [SerializeField] TextMeshProUGUI beginText;
    [SerializeField] GameObject doneForToday;

    public void BeginAttempt(){
        if(!waitingForTomorrow){
            if(inAttempt){
                if(inputStreak.Length > 0){
                    ConfirmAttempt();
                }
            }
            else{
                if(!inAnimation){
                    inAttempt = true;
                    beginText.text = "Confirm";
                }
            }
        }else{
            doneForToday.SetActive(true);
        }
    }

    public void ConfirmAttempt(){
        beginText.text = "Begin";
        inAttempt = false;
        if(inputStreak == currentStreak){
            //play success animation
            StartCoroutine(SuccessAnimation());
            DateTime currentTime = DateTime.Now;
            PlayerPrefs.SetString("CurrentStreak",currentStreak);
            PlayerPrefs.SetInt("lastDayCompleted", currentTime.DayOfYear);
            
            if(currentStreak.Length >= 3){
                currentStreak += CreateDailyRandom().ToString();
                PlayerPrefs.SetString("CurrentStreak",currentStreak);
                waitingForTomorrow = true;
            }else{
                currentStreak += UnityEngine.Random.Range(0,10).ToString();
                PlayerPrefs.SetString("CurrentStreak",currentStreak);
            }
            currentScore.text = "Current Streak: " + (currentStreak.Length-1).ToString();
        }
        else{
            //play failed animation
            waitingForTomorrow = false;
            StartCoroutine(FailAnimation());
            currentStreak = "0";
            PlayerPrefs.SetString("CurrentStreak","0");
            currentScore.text = "Current Streak: " + (currentStreak.Length-1).ToString();
        }
        inputStreak = "";
        
    }
    int CreateDailyRandom(){
        //creates a "Random" number based on the day - beauty is that its the same for everyone
        int n = DateTime.Now.DayOfYear;
        //can change equation as required;
        return (9*n^4+12*n^3+ 2*n^2+n+6)% 10;

    }
    public void ResetStreak(){
        waitingForTomorrow = false;
        inAttempt = false;
        StartCoroutine(FailAnimation());
        currentStreak = "0";
        PlayerPrefs.SetString("CurrentStreak","0");
        currentScore.text = "Current Streak: " + (currentStreak.Length-1).ToString();
        inputStreak = "";
    }
    #endregion

    #region Animations
    IEnumerator SuccessAnimation(){
        inAnimation = true;
        currentStreakText.text = "SUCCES";
        currentStreakText.color = Color.green;
        yield return new WaitForSeconds(2f);
        currentStreakText.text = "";
        currentStreakText.color = color;
        inAnimation = false;
        SetText(4);
    }
    IEnumerator FailAnimation(){
        inAnimation = true;
        color = currentStreakText.color;
        currentStreakText.text = "RESET";
        currentStreakText.color = Color.red;
        yield return new WaitForSeconds(2f);
        currentStreakText.text = "";
        currentStreakText.color = color;
        inAnimation = false;
        SetText();
    }
    #endregion

    #region ButtonManagement
    [SerializeField] Button[] buttons;
    public void ShowNext(){
        buttons[NextButton()].ImNext();
    }

    int NextButton(){
        return Int32.Parse(currentStreak[currentStreak.Length-1].ToString());
    }

    public void AddNumber(string numb){
        inputStreak +=numb;
        if(currentStreakText.text.Length >= 6){
            currentStreakText.text = currentStreakText.text.Remove(0,1);
        }
        currentStreakText.text += numb;
    }
    #endregion

    #region UpdateStats
    public void UpdateHighScores(){
        if(currentStreak.Length> HighScore()){
            PlayerPrefs.SetInt("HighScore",currentStreak.Length);
        }
        highScore.text = HighScore().ToString();
    }

    int HighScore(){
        if(!PlayerPrefs.HasKey("HighScore")){
            PlayerPrefs.SetInt("HighScore",0);
        }
        return PlayerPrefs.GetInt("HighScore");
    }
    string CurrentStreak(){
        if(!PlayerPrefs.HasKey("CurrentStreak")){
            PlayerPrefs.SetString("CurrentStreak","0");
        }
        return PlayerPrefs.GetString("CurrentStreak");
    }

    #endregion
    
    #region GeneralSettings
    public void ColourMode(bool darkmode = true){
        //darkmode
    }
    #endregion
}
