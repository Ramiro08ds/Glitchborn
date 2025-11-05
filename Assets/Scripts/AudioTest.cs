using UnityEngine;

/// <summary>
/// Script de prueba simple para verificar que el audio funciona
/// Agrégalo temporalmente al Player
/// </summary>
public class AudioTest : MonoBehaviour
{
    void Update()
    {
        // Presiona T para probar el audio de pasos
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== TEST DE AUDIO ===");
            
            if (AudioManager.instance == null)
            {
                Debug.LogError("❌ AudioManager.instance es NULL");
                return;
            }
            
            Debug.Log("✅ AudioManager existe");
            
            if (AudioManager.instance.sonidoPasos == null)
            {
                Debug.LogError("❌ sonidoPasos es NULL");
                return;
            }
            
            Debug.Log("✅ sonidoPasos asignado: " + AudioManager.instance.sonidoPasos.name);
            
            // Intentar reproducir
            AudioManager.instance.IniciarPasos();
            Debug.Log("✅ IniciarPasos() ejecutado");
            
            // Verificar AudioSources
            AudioSource[] sources = AudioManager.instance.GetComponents<AudioSource>();
            Debug.Log($"🔊 AudioSources en AudioManager: {sources.Length}");
            
            if (sources.Length < 3)
            {
                Debug.LogError("❌ Faltan AudioSources! Solo hay " + sources.Length);
            }
            else
            {
                Debug.Log("✅ Los 3 AudioSources existen");
                Debug.Log($"   [0] Volume: {sources[0].volume}, Loop: {sources[0].loop}");
                Debug.Log($"   [1] Volume: {sources[1].volume}, Loop: {sources[1].loop}");
                Debug.Log($"   [2] Volume: {sources[2].volume}, Loop: {sources[2].loop}, Playing: {sources[2].isPlaying}");
            }
        }
        
        // Presiona Y para detener
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.DetenerPasos();
                Debug.Log("🔇 Pasos detenidos");
            }
        }
    }
}
