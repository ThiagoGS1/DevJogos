using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    [SerializeField] private float typingSpeed = 0.05f;
    
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;
    
    [SerializeField] private Sprite[] characterSprites; // Array de sprites de personagens
    [SerializeField] private string[] characterNames;   // Array de nomes de personagens
    
    // Dados para controlar qual personagem fala cada linha
    [SerializeField] private int[] lineToCharacterIndex; // Índice do personagem para cada linha
    
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private int nextSceneIndex = -1;
    [SerializeField] private bool useSceneName = false;

    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    
    // Inicialização
    private void Start()
    {
        dialogueBox.SetActive(false);
        
        // Iniciar o diálogo automaticamente ao carregar a cena
        TriggerDialogue();
    }
    
    // Atualização a cada frame
    private void Update()
    {
        // Se o diálogo estiver ativo, verificar input
        if (isDialogueActive)
        {
            // Detecta se a barra de espaço foi pressionada
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    // Se estiver digitando, completa o texto imediatamente
                    CompleteText();
                }
                else
                {
                    // Se não estiver digitando, avança para próxima linha
                    NextDialogueLine();
                }
            }
        }
    }
    
    // Inicia o diálogo
    public void StartDialogue()
    {
        if (!isDialogueActive)
        {
            isDialogueActive = true;
            currentLineIndex = 0;
            dialogueBox.SetActive(true);
            
            // Inicia a primeira linha de diálogo
            StartCoroutine(TypeDialogueLine(dialogueLines[currentLineIndex]));
            
            // Define o personagem para a primeira linha
            UpdateCharacterInfo(currentLineIndex);
        }
    }
    
    // Avança para a próxima linha de diálogo
    private void NextDialogueLine()
    {
        currentLineIndex++;
        
        // Verifica se chegou ao fim do diálogo
        if (currentLineIndex < dialogueLines.Length)
        {
            // Avança para próxima linha
            StartCoroutine(TypeDialogueLine(dialogueLines[currentLineIndex]));
            
            // Atualiza informações do personagem
            UpdateCharacterInfo(currentLineIndex);
        }
        else
        {
            // Fim do diálogo
            EndDialogue();
        }
    }
    
    // Efeito de digitação do texto
    private IEnumerator TypeDialogueLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }
    
    // Completa o texto atual imediatamente
    private void CompleteText()
    {
        StopAllCoroutines();
        dialogueText.text = dialogueLines[currentLineIndex];
        isTyping = false;
    }
    
    // Finaliza o diálogo
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
        dialogueText.text = "";

        // Carrega a próxima cena
        if (useSceneName && !string.IsNullOrEmpty(nextSceneName))
        {
            // Carrega pelo nome da cena
            SceneManager.LoadScene(nextSceneName);
        }
        else if (nextSceneIndex >= 0)
        {
            // Carrega pelo índice da cena
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Carrega a próxima cena sequencialmente
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    
    // Atualiza as informações do personagem (ícone e nome)
    private void UpdateCharacterInfo(int lineIndex)
    {
        int characterIndex = 0;
        
        // Se tiver mapeamento de linha para personagem, use-o
        if (lineToCharacterIndex != null && lineToCharacterIndex.Length > lineIndex)
        {
            characterIndex = lineToCharacterIndex[lineIndex];
        }
        
        // Atualiza o sprite do personagem
        if (characterSprites != null && characterSprites.Length > characterIndex)
        {
            characterIcon.sprite = characterSprites[characterIndex];
        }
        
        // Atualiza o nome do personagem
        if (characterNames != null && characterNames.Length > characterIndex)
        {
            characterNameText.text = characterNames[characterIndex];
        }
    }
    
    // Muda manualmente o sprite do personagem
    public void ChangeCharacterSprite(Sprite newSprite)
    {
        if (characterIcon != null && newSprite != null)
        {
            characterIcon.sprite = newSprite;
        }
    }
    
    // Método para iniciar diálogo externamente (chamado automaticamente em Start)
    public void TriggerDialogue()
    {
        StartDialogue();
    }
}