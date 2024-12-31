using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    [SerializeField] private GameObject speechBubble; // ��ȭâ GameObject
    [SerializeField] private GameObject arrow; // ȭ��ǥ GameObject
    [SerializeField] private TMP_Text messageText; // ��ȭ �޽����� ǥ���� TextMeshPro
    [SerializeField] private TMP_Text nameText; // �̸� �ؽ�Ʈ
    [SerializeField] private RectTransform speechBubbleRect; // ��ǳ���� RectTransform
    [SerializeField] private RectTransform nameBoxRect; // ���� �ڽ��� RectTransform

    [SerializeField] private DialogSO dialogSO; // ��ȭ �����͸� ��� �ִ� ScriptableObject

    private Dictionary<int, DialogInfo> dialogDict; // ID�� Ű�� �ϴ� ��ȭ ������ ��ųʸ�
    private string[] currentMessages; // ���� ��� ���� �޽��� �迭
    private int currentMessageIndex; // ���� �޽��� �ε���
    public bool isDialogActive; // ��ȭ ���� ����
    private bool isTyping; // Ÿ���� ������ Ȯ��
    private Coroutine typingCoroutine; // ���� ��� �ڷ�ƾ
    private string currentName; // ���� ��� ���� �̸�

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // ScriptableObject���� �����͸� ��ųʸ��� ��ȯ
        dialogDict = new Dictionary<int, DialogInfo>();
        foreach (var dialog in dialogSO.dialogInfo)
        {
            dialogDict[dialog.ID] = dialog;
        }
    }

    private void Update()
    {
        if (isDialogActive && Input.GetKey(KeyCode.C))
        {
            if (!isTyping) // Ÿ���� ���� �ƴ� ���� ���� �޽����� �̵�
            {
                ShowNextMessage();
            }
        }
    }

    public void DialogStart(int id, Vector3 newPosition)
    {
        // ��ȭ�� �̹� Ȱ��ȭ�Ǿ� ������ ���ο� ��ȭ�� ���۵��� �ʵ��� ����
        if (isDialogActive) return;

        // ��ųʸ����� ID�� ��ȭ �����͸� ������ ã��
        if (dialogDict.TryGetValue(id, out var dialog))
        {
            currentMessages = dialog.message;
            currentName = dialog.name; // �̸� ����
            currentMessageIndex = 0;
            isDialogActive = true;
            speechBubble.SetActive(true);
            nameText.text = currentName; // �̸� �ؽ�Ʈ ����
            AdjustNameBoxSize(); // ���� �ڽ� ũ�� ����

            // speechBubble�� ��ġ�� ������ ��ġ�� �̵� (y ���� 5�� �߰��Ͽ� �ø��ϴ�)
            speechBubble.transform.position = new Vector3(newPosition.x, newPosition.y + 2, newPosition.z);

            ShowNextMessage(); // ù �޽��� ���
        }
    }

    void ShowNextMessage()
    {
        if (currentMessages != null && currentMessageIndex < currentMessages.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // ȭ��ǥ�� ��Ȱ��ȭ�ϰ�, �޽����� ��µǱ� ���� �ٽ� Ȱ��ȭ
            arrow.SetActive(false);

            typingCoroutine = StartCoroutine(TypeMessage(currentMessages[currentMessageIndex])); // �޽��� Ÿ���� ����
            currentMessageIndex++; // ���� �޽����� �̵�
        }
        else
        {
            EndDialog(); // ��� �޽����� �� ����� ��� ��ȭ ����
        }
    }

    IEnumerator TypeMessage(string message)
    {
        isTyping = true; // Ÿ���� ����
        messageText.text = ""; // ���� �ؽ�Ʈ �ʱ�ȭ
        AdjustSpeechBubbleSize(); // ��ǳ�� ũ�� ����

        foreach (char letter in message.ToCharArray())
        {
            messageText.text += letter; // �� ���ھ� �߰�
            AdjustSpeechBubbleSize(); // �ؽ�Ʈ ũ�⿡ �°� ��ǳ�� ����
            yield return new WaitForSeconds(0.02f); // ���� ��� ����
        }

        isTyping = false; // Ÿ���� �Ϸ�
        typingCoroutine = null; // �ڷ�ƾ �ʱ�ȭ

        // �޽����� ������ ȭ��ǥ ǥ��
        arrow.SetActive(true);
    }

    void AdjustSpeechBubbleSize()
    {
        // �ؽ�Ʈ�� Preferred Width �� Preferred Height�� ������
        Vector2 textSize = messageText.GetPreferredValues(messageText.text);

        // ��ǳ���� ũ�⸦ �ؽ�Ʈ ũ�⿡ �°� ����
        speechBubbleRect.sizeDelta = new Vector2(textSize.x + 50f, textSize.y + 70f);
    }

    void AdjustNameBoxSize()
    {
        // �̸� �ؽ�Ʈ�� Preferred Width�� ������
        Vector2 nameSize = nameText.GetPreferredValues(currentName);

        // ���� �ڽ��� ũ�⸦ �̸� ���̿� �°� ����
        nameBoxRect.sizeDelta = new Vector2(nameSize.x + 30f, nameBoxRect.sizeDelta.y);
        // +30f�� ���� �ڽ��� �е� ��
    }

    void EndDialog()
    {
        isDialogActive = false;
        speechBubble.SetActive(false); // ��ȭâ �����
        arrow.SetActive(false); // ��ȭ ������ ȭ��ǥ �����
    }
}
