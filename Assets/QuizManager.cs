using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Thirdweb;
using System;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;
    public GameObject QuizPanel;
    public GameObject GOPanel;
    public Text QuestionTxt;
    public Text ScoreTxt;
    int TotalQuestions = 0;
    public int score;

    public string Address { get; private set; }
    string peopleAddressSmartContract = "0x75137A098367eeC33D5FE06D34301dC54c5ae853";
    string processAddressSmartContract = "0x0F6482B9f33366e8Da35ce4d723123305bF21FFb";
    string businessAddressSmartContract = "0x853467eCFaCc820CCFEabFf85ac33BE599Ad3F25";
    string addressSmartContract = "";
    public Button backToMainMenuButton;
    public Button NFTClaim;

    public Text statusText;


    private void Start()
    {
        backToMainMenuButton.gameObject.SetActive(false);
        NFTClaim.gameObject.SetActive(false);
        statusText.gameObject.SetActive(false);

        TotalQuestions = QnA.Count;
        GOPanel.SetActive(false);
        generateQuestion();
    }

    void GameOver()
    {
        // Lấy tên của Scene hiện tại
        string sceneName = SceneManager.GetActiveScene().name;

        // Kiểm tra nếu tên Scene là "People"
        if (sceneName == "People")
        {
            if (score >= 8)
            {
                NFTClaim.gameObject.SetActive(true);
                statusText.text = "You Pass";
                statusText.gameObject.SetActive(true);
                ResourceBoost.Instance.people = 1;
            }
            else
            {
                backToMainMenuButton.gameObject.SetActive(true);
                statusText.text = "You Fail";
                statusText.gameObject.SetActive(true);
                ResourceBoost.Instance.people = 2;
            }
        } else if (sceneName == "Process") {
            if (score >= 8)
            {
                NFTClaim.gameObject.SetActive(true);
                statusText.text = "You Pass";
                statusText.gameObject.SetActive(true);
                ResourceBoost.Instance.process = 1;
            }
            else
            {
                backToMainMenuButton.gameObject.SetActive(true);
                statusText.text = "You Fail";
                statusText.gameObject.SetActive(true);
                ResourceBoost.Instance.process = 2;
            }
        }
        else if (sceneName == "Environment")
        {
            if (score >= 8)
            {
                NFTClaim.gameObject.SetActive(true);
                statusText.text = "You Pass";
                statusText.gameObject.SetActive(true);
                ResourceBoost.Instance.business = 1;
            }
            else
            {
                backToMainMenuButton.gameObject.SetActive(true);
                statusText.text = "You Fail";
                statusText.gameObject.SetActive(true);
                ResourceBoost.Instance.business = 2;
            }
        }
        QuizPanel.SetActive(false);
        GOPanel.SetActive(true);
        ScoreTxt.text = score + "/" + TotalQuestions;
    }

    public void correct()
    {
        score += 1;
        generateQuestion();
    }

    public void wrong()
    {
        generateQuestion();
    }
    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswersScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[currentQuestion].Answers[i];
            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswersScript>().isCorrect = true;
            }
        }
    }
    void generateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = UnityEngine.Random.Range(0, QnA.Count);
            QuestionTxt.text = QnA[currentQuestion].Questions;
            SetAnswers();
            QnA.RemoveAt(currentQuestion);
        }
        else
        {
            Debug.Log("Out of Question");
            GameOver();
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public async void ClaimCertificateNFT()
    {
        Address = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
        statusText.text = "Claiming Result...";
        statusText.gameObject.SetActive(true);
        NFTClaim.interactable = false;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "People")
        { addressSmartContract = peopleAddressSmartContract; }
        else if (sceneName == "Process")
        { addressSmartContract = processAddressSmartContract; }
        else if (sceneName == "Environment")
        { addressSmartContract = businessAddressSmartContract; }


            var contract = ThirdwebManager.Instance.SDK.GetContract(addressSmartContract);
        try
        {
            var result = await contract.ERC721.ClaimTo(Address, 1);
            statusText.text = "Claimed Result!";
            statusText.gameObject.SetActive(true);
            NFTClaim.gameObject.SetActive(false);
            backToMainMenuButton.gameObject.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while claiming the NFT: {ex.Message}");
            // Optionally, update the UI to inform the user of the error
            statusText.text = "Failed to claim Result. Please try again.";
            statusText.gameObject.SetActive(true);
            NFTClaim.interactable = true;
        }
    }

}
