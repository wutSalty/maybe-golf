using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverrideRenderPipeline : MonoBehaviour
{
    public static OverrideRenderPipeline instance;

    public RenderPipelineAsset defaultRenderPipelineAsset;
    public RenderPipelineAsset CRTRenderPipelineAsset;

    [ContextMenu("Switch CRT")]
    public void SwitchToCRT()
    {
        GraphicsSettings.defaultRenderPipeline = CRTRenderPipelineAsset;
        QualitySettings.renderPipeline = CRTRenderPipelineAsset;
    }

    [ContextMenu("Switch Default")]
    public void SwitchToDefault()
    {
        GraphicsSettings.defaultRenderPipeline = defaultRenderPipelineAsset;
        QualitySettings.renderPipeline = defaultRenderPipelineAsset;
    }
}
