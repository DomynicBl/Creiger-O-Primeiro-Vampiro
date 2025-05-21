using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DrawModeController : MonoBehaviour
{
    [Header("Novo Draw Mode para aplicar")]
    public SpriteDrawMode drawMode = SpriteDrawMode.Simple;

    [Header("Novo tamanho para sprites com Tiled/Sliced")]
    public float width = 1f;
    public float length = 1f;

    [Header("SpriteRenderers detectados automaticamente")]
    public List<SpriteRenderer> layers = new List<SpriteRenderer>();

    [ContextMenu("Atualizar Lista de Camadas")]
    public void AtualizarLista()
    {
        layers.Clear();
        SpriteRenderer[] found = GetComponentsInChildren<SpriteRenderer>(true);
        layers.AddRange(found);
    }

    [ContextMenu("Aplicar Draw Mode e Tamanho")]
    public void AplicarConfiguracoes()
    {
        Vector2 newSize = new Vector2(width, length);

        foreach (var sr in layers)
        {
            if (sr == null) continue;

            sr.drawMode = drawMode;

            if (drawMode != SpriteDrawMode.Simple)
            {
                sr.size = newSize;
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(sr);
#endif
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}
