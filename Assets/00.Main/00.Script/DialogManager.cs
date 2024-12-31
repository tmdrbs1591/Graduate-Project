using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    [SerializeField] private GameObject speechBubble; // 대화창 GameObject
    [SerializeField] private GameObject arrow; // 화살표 GameObject
    [SerializeField] private TMP_Text messageText; // 대화 메시지를 표시할 TextMeshPro
    [SerializeField] private TMP_Text nameText; // 이름 텍스트
    [SerializeField] private RectTransform speechBubbleRect; // 말풍선의 RectTransform
    [SerializeField] private RectTransform nameBoxRect; // 네임 박스의 RectTransform

    [SerializeField] private DialogSO dialogSO; // 대화 데이터를 담고 있는 ScriptableObject

    private Dictionary<int, DialogInfo> dialogDict; // ID를 키로 하는 대화 데이터 딕셔너리
    private string[] currentMessages; // 현재 출력 중인 메시지 배열
    private int currentMessageIndex; // 현재 메시지 인덱스
    public bool isDialogActive; // 대화 진행 상태
    private bool isTyping; // 타이핑 중인지 확인
    private Coroutine typingCoroutine; // 글자 출력 코루틴
    private string currentName; // 현재 출력 중인 이름

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // ScriptableObject에서 데이터를 딕셔너리로 변환
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
            if (!isTyping) // 타이핑 중이 아닐 때만 다음 메시지로 이동
            {
                ShowNextMessage();
            }
        }
    }

    public void DialogStart(int id, Vector3 newPosition)
    {
        // 대화가 이미 활성화되어 있으면 새로운 대화가 시작되지 않도록 방지
        if (isDialogActive) return;

        // 딕셔너리에서 ID로 대화 데이터를 빠르게 찾기
        if (dialogDict.TryGetValue(id, out var dialog))
        {
            currentMessages = dialog.message;
            currentName = dialog.name; // 이름 설정
            currentMessageIndex = 0;
            isDialogActive = true;
            speechBubble.SetActive(true);
            nameText.text = currentName; // 이름 텍스트 설정
            AdjustNameBoxSize(); // 네임 박스 크기 조정

            // speechBubble의 위치를 설정된 위치로 이동 (y 값은 5를 추가하여 올립니다)
            speechBubble.transform.position = new Vector3(newPosition.x, newPosition.y + 2, newPosition.z);

            ShowNextMessage(); // 첫 메시지 출력
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

            // 화살표를 비활성화하고, 메시지가 출력되기 전에 다시 활성화
            arrow.SetActive(false);

            typingCoroutine = StartCoroutine(TypeMessage(currentMessages[currentMessageIndex])); // 메시지 타이핑 시작
            currentMessageIndex++; // 다음 메시지로 이동
        }
        else
        {
            EndDialog(); // 모든 메시지를 다 출력한 경우 대화 종료
        }
    }

    IEnumerator TypeMessage(string message)
    {
        isTyping = true; // 타이핑 시작
        messageText.text = ""; // 기존 텍스트 초기화
        AdjustSpeechBubbleSize(); // 말풍선 크기 조정

        foreach (char letter in message.ToCharArray())
        {
            messageText.text += letter; // 한 글자씩 추가
            AdjustSpeechBubbleSize(); // 텍스트 크기에 맞게 말풍선 조정
            yield return new WaitForSeconds(0.02f); // 글자 출력 간격
        }

        isTyping = false; // 타이핑 완료
        typingCoroutine = null; // 코루틴 초기화

        // 메시지가 끝나면 화살표 표시
        arrow.SetActive(true);
    }

    void AdjustSpeechBubbleSize()
    {
        // 텍스트의 Preferred Width 및 Preferred Height를 가져옴
        Vector2 textSize = messageText.GetPreferredValues(messageText.text);

        // 말풍선의 크기를 텍스트 크기에 맞게 조정
        speechBubbleRect.sizeDelta = new Vector2(textSize.x + 50f, textSize.y + 70f);
    }

    void AdjustNameBoxSize()
    {
        // 이름 텍스트의 Preferred Width를 가져옴
        Vector2 nameSize = nameText.GetPreferredValues(currentName);

        // 네임 박스의 크기를 이름 길이에 맞게 조정
        nameBoxRect.sizeDelta = new Vector2(nameSize.x + 30f, nameBoxRect.sizeDelta.y);
        // +30f는 네임 박스의 패딩 값
    }

    void EndDialog()
    {
        isDialogActive = false;
        speechBubble.SetActive(false); // 대화창 숨기기
        arrow.SetActive(false); // 대화 끝나면 화살표 숨기기
    }
}
