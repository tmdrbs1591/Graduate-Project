using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble; // 대화창 GameObject
    [SerializeField] private TMP_Text messageText; // 대화 메시지를 표시할 TextMeshPro

    [SerializeField] private DialogSO dialogSO; // 대화 데이터를 담고 있는 ScriptableObject

    private string[] currentMessages; // 현재 출력 중인 메시지 배열
    private int currentMessageIndex; // 현재 메시지 인덱스
    private bool isDialogActive; // 대화 진행 상태

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            DialogStart(1);
        }

        if (isDialogActive && Input.GetKeyDown(KeyCode.C))
        {
            ShowNextMessage();
        }
    }

    void DialogStart(int id)
    {
        foreach (var dialog in dialogSO.dialogInfo)
        {
            if (dialog.ID == id)
            {
                currentMessages = dialog.message;
                currentMessageIndex = 0;
                isDialogActive = true;
                speechBubble.SetActive(true);
                ShowNextMessage(); // 첫 메시지 출력
                break;
            }
        }
    }

    void ShowNextMessage()
    {
        if (currentMessages != null && currentMessageIndex < currentMessages.Length)
        {
            messageText.text = currentMessages[currentMessageIndex]; // 현재 메시지 표시
            currentMessageIndex++; // 다음 메시지로 이동
        }
        else
        {
            EndDialog(); // 모든 메시지를 다 출력한 경우 대화 종료
        }
    }

    void EndDialog()
    {
        isDialogActive = false;
        speechBubble.SetActive(false); // 대화창 숨기기
    }
}
