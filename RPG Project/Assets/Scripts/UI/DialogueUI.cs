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
        }
    }
}
