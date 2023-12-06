
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Text npcText;
    public InputField inputField; // �ϳ��� �Է� �ʵ� ���
    public GameObject npcDialog; // NPC ��ȭâ

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
                npcText.text = "������! �ź��� ������!";
                npcText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                currentState = DialogueState.WaitForNameInput;
                break;
            case DialogueState.WaitForNameInput:
                inputField.gameObject.SetActive(true);
                inputField.placeholder.GetComponent<Text>().text = "�̸��� �Է��Ͻÿ�: ___";
                inputField.ActivateInputField();
                npcText.gameObject.SetActive(false);
                currentState = DialogueState.NameEntered;
                break;
            case DialogueState.NameEntered:
                playerName = inputField.text;
                npcText.text = playerName + "�̱�, ū���̾�. �տ��� ���Ͱ� �����ؼ� �� ���ư� �� ����.";
                inputField.text = "";
                npcText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                currentState = DialogueState.WaitForHeightInput;
                break;
            case DialogueState.WaitForHeightInput:
                inputField.gameObject.SetActive(true);
                inputField.placeholder.GetComponent<Text>().text = "Ű�� �Է��Ͻÿ�: ___";
                inputField.ActivateInputField();
                npcText.gameObject.SetActive(false);
                currentState = DialogueState.HeightEntered;
                break;
            case DialogueState.HeightEntered:
                playerHeight = inputField.text;
                npcText.text = "�����Ը� �˷��� �� �־�?";
                inputField.text = "";
                npcText.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                currentState = DialogueState.WaitForWeightInput;
                break;
            case DialogueState.WaitForWeightInput:
                inputField.gameObject.SetActive(true);
                inputField.placeholder.GetComponent<Text>().text = "�����Ը� �Է��Ͻÿ�: ___";
                inputField.ActivateInputField();
                npcText.gameObject.SetActive(false);
                currentState = DialogueState.WeightEntered;
                break;
            case DialogueState.WeightEntered:
                playerWeight = inputField.text;
                npcText.text = playerHeight + " cm / " + playerWeight + " kg �̱���.";
                inputField.gameObject.SetActive(false);
                npcText.gameObject.SetActive(true);
                currentState = DialogueState.ShowPower;
                break;
            case DialogueState.ShowPower:
                npcText.text = "�˰ھ�, �ʿ��� �´� ���� �ο����ٰ�!";
                currentState = DialogueState.End;
                break;
            case DialogueState.End:
                npcText.text = "�ͳι��� �����ž�, ���� ���͵��� óġ����.";
                npcDialog.SetActive(false);
                break;
        }
    }

    void UpdateDialogue()
    {
        // This function can be used for additional dialogue updates if needed
    }
}
