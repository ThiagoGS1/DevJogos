using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    [Header("Minimap References")]
    public RenderTexture minimapRenderTexture;
    public RawImage minimapDisplay;
    
    [Header("Minimap Settings")]
    public float cameraHeight = 350f;         // Aumentado para ficar mais alto
    public float orthographicSize = 900f;     // Muito aumentado para mostrar toda a pista
    public Vector3 mapCenter = new Vector3(0f, 0f, 0f); // Centro da pista - AJUSTE ESTE VALOR
    
    private Camera minimapCamera;

    void Start()
    {
        // Verificar se já existe uma câmera do minimapa
        minimapCamera = GameObject.Find("MinimapCamera")?.GetComponent<Camera>();
        
        if (minimapCamera == null)
        {
            // Criar câmera do minimapa se não existir
            GameObject minimapCameraObj = new GameObject("MinimapCamera");
            minimapCamera = minimapCameraObj.AddComponent<Camera>();
        }
        
        // Configuração da câmera
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        minimapCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = orthographicSize; 
        minimapCamera.cullingMask = -1; // Everything
        minimapCamera.targetTexture = minimapRenderTexture;
        minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        minimapCamera.depth = -10;
        
        // Certifique-se de desativar o AudioListener
        AudioListener audioListener = minimapCamera.GetComponent<AudioListener>();
        if (audioListener != null)
            Destroy(audioListener);
        
        // Posicionar a câmera acima do centro do mapa
        minimapCamera.transform.position = new Vector3(mapCenter.x, cameraHeight, mapCenter.z);
        
        // Atribuir o RenderTexture ao RawImage
        if (minimapDisplay != null && minimapRenderTexture != null)
        {
            minimapDisplay.texture = minimapRenderTexture;
            minimapDisplay.color = Color.white; // Garante opacidade total
        }
        else
        {
            Debug.LogError("MinimapManager: Faltando referência ao RawImage ou RenderTexture!");
        }
    }
}