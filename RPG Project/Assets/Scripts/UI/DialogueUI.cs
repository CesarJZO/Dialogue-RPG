using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI conversantName;
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private GameObject aIResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;

        private PlayerConversant _playerConversant;

        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        private void Start()
        {
            _playerConversant.ConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(_playerConversant.Next);
            quitButton.onClick.AddListener(_playerConversant.Quit);

            UpdateUI();
        }

        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.HasDialogue);
            if (!_playerConversant.HasDialogue)
                return;

            conversantName.text = _playerConversant.GetCurrentConversantName();

            aIResponse.SetActive(!_playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing());

            if (_playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = _playerConversant.GetText();
                nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (DialogueNode choice in _playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.Text;
                var button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => _playerConversant.SelectChoice(choice));
            }
        }
    }
}
