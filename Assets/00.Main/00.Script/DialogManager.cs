using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

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

    [SerializeField] private PlayableDirector playableDirector; // Ÿ�Ӷ����� ������ PlayableDirector
    [SerializeField] private AudioClip tickSound; // ��� ��� �� �Ҹ�
    [SerializeField] private AudioClip clickSound; // ��� �ѱ� �� �Ҹ�
    private AudioSource audioSource; // AudioSource ������Ʈ

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ �ʱ�ȭ
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
                PlayClickSound(); // ��� �ѱ� �� �Ҹ� ���
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
            PlayTickSound(); // ��� ��� �� �Ҹ� ���
            yield return new WaitForSeconds(0.02f); // ���� ��� ����
        }

        isTyping = false; // Ÿ���� �Ϸ�
        typingCoroutine = null; // �ڷ�ƾ �ʱ�ȭ

        // �޽����� ������ ȭ��ǥ ǥ��
        arrow.SetActive(true);
    }

    void PlayTickSound()
    {
        if (audioSource != null && tickSound != null)
        {
            audioSource.PlayOneShot(tickSound); // ��� ��� �� �Ҹ� ���
        }
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound); // ��� �ѱ� �� �Ҹ� ���
        }
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
        if (TimeLineManager.instance.isCutScene)
            ResumeTimeline();

        isDialogActive = false;
        speechBubble.SetActive(false); // ��ȭâ �����
        arrow.SetActive(false); // ��ȭ ������ ȭ��ǥ �����
    }

    public void CutSceneDialogStart(int id)
    {
        // ��ȭ�� �̹� Ȱ��ȭ�Ǿ� ������ ���ο� ��ȭ�� ���۵��� �ʵ��� ����
        if (isDialogActive) return;

        PauseTimeline();

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

            ShowNextMessage(); // ù �޽��� ���
        }
    }

    public void CutSceneDialogStartPosition(Transform newPosition)
    {
        speechBubble.transform.position = new Vector3(newPosition.position.x, newPosition.position.y, newPosition.position.z);
    }

    public void PauseTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause(); // Ÿ�Ӷ��� ���߱�
        }
    }

    public void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play(); // Ÿ�Ӷ��� ���
        }
    }
}
