using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble; // ��ȭâ GameObject
    [SerializeField] private TMP_Text messageText; // ��ȭ �޽����� ǥ���� TextMeshPro

    [SerializeField] private DialogSO dialogSO; // ��ȭ �����͸� ��� �ִ� ScriptableObject

    private string[] currentMessages; // ���� ��� ���� �޽��� �迭
    private int currentMessageIndex; // ���� �޽��� �ε���
    private bool isDialogActive; // ��ȭ ���� ����

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
                ShowNextMessage(); // ù �޽��� ���
                break;
            }
        }
    }

    void ShowNextMessage()
    {
        if (currentMessages != null && currentMessageIndex < currentMessages.Length)
        {
            messageText.text = currentMessages[currentMessageIndex]; // ���� �޽��� ǥ��
            currentMessageIndex++; // ���� �޽����� �̵�
        }
        else
        {
            EndDialog(); // ��� �޽����� �� ����� ��� ��ȭ ����
        }
    }

    void EndDialog()
    {
        isDialogActive = false;
        speechBubble.SetActive(false); // ��ȭâ �����
    }
}
