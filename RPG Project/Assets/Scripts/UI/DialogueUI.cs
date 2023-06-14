using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Button nextButton;

        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;

        private PlayerConversant _playerConversant;

        private void Awake()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        private void Start()
        {
            nextButton.onClick.AddListener(Next);

            UpdateUI();
        }

        private void Next()
        {
            _playerConversant.Next();

            UpdateUI();
        }

        private void UpdateUI()
        {
            aiText.text = _playerConversant.GetText();

            nextButton.gameObject.SetActive(_playerConversant.HasNext());

            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);

            foreach (string choiceText in _playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
            }
        }
    }
}
