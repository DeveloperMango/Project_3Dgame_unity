
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Text npcText;
    public InputField inputField; // 하나의 입력 필드 사용
    public GameObject npcDialog; // NPC 대화창

    private string playerName;
    private string playerHeight;
    private string playerWeight;

    private enum DialogueState
    {
        Intro,
        WaitForNameInput,
        NameEntered,
        WaitForHeightInput,
        HeightEntered,
        WaitForWeightInput,
        WeightEntered,
        ShowPower,
        End
    }

    private DialogueState currentState;

    void Start()
    {
        npcDialog.SetActive(false);
        inputField.gameObject.SetActive(false);
        currentState = DialogueState.Intro;
        UpdateDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AdvanceDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            npcDialog.SetActive(true);
            currentState = DialogueState.Intro;
            UpdateDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            npcDialog.SetActive(false);
        }
    }

    void AdvanceDialogue()
    {
        switch (currentState)
        {
            case DialogueState.Intro:
                npcText.text = "누구냐! 신분을 밝혀라!";
                npcText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                currentState = DialogueState.WaitForNameInput;
                break;
            case DialogueState.WaitForNameInput:
                inputField.gameObject.SetActive(true);
                inputField.placeholder.GetComponent<Text>().text = "이름을 입력하시오: ___";
                inputField.ActivateInputField();
                npcText.gameObject.SetActive(false);
                currentState = DialogueState.NameEntered;
                break;
            case DialogueState.NameEntered:
                playerName = inputField.text;
                npcText.text = playerName + "이군, 큰일이야. 앞에는 몬스터가 점령해서 더 나아갈 수 없어.";
                inputField.text = "";
                npcText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                currentState = DialogueState.WaitForHeightInput;
                break;
            case DialogueState.WaitForHeightInput:
                inputField.gameObject.SetActive(true);
                inputField.placeholder.GetComponent<Text>().text = "키를 입력하시오: ___";
                inputField.ActivateInputField();
                npcText.gameObject.SetActive(false);
                currentState = DialogueState.HeightEntered;
                break;
            case DialogueState.HeightEntered:
                playerHeight = inputField.text;
                npcText.text = "몸무게를 알려줄 수 있어?";
                inputField.text = "";
                npcText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                currentState = DialogueState.WaitForWeightInput;
                break;
            case DialogueState.WaitForWeightInput:
                inputField.gameObject.SetActive(true);
                inputField.placeholder.GetComponent<Text>().text = "몸무게를 입력하시오: ___";
                inputField.ActivateInputField();
                npcText.gameObject.SetActive(false);
                currentState = DialogueState.WeightEntered;
                break;
            case DialogueState.WeightEntered:
                playerWeight = inputField.text;
                npcText.text = playerHeight + " cm / " + playerWeight + " kg 이구나.";
                inputField.gameObject.SetActive(false);
                npcText.gameObject.SetActive(true);
                currentState = DialogueState.ShowPower;
                break;
            case DialogueState.ShowPower:
                npcText.text = "알겠어, 너에게 맞는 힘을 부여해줄게!";
                currentState = DialogueState.End;
                break;
            case DialogueState.End:
                npcText.text = "터널문이 열릴거야, 가서 몬스터들을 처치해줘.";
                npcDialog.SetActive(false);
                break;
        }
    }

    void UpdateDialogue()
    {
        // This function can be used for additional dialogue updates if needed
    }
}
