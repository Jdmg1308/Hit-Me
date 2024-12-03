using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullscreenEffectRendererFeature : ScriptableRendererFeature
{
    public static FullscreenEffectRendererFeature instance; // Static instance for runtime access

    private FullscreenEffectPass _fullscreenPass;
    private Material _currentMaterial;

    public override void Create()
    {
        instance = this; // Assign the instance when the feature is created
        _fullscreenPass = new FullscreenEffectPass(RenderPassEvent.AfterRenderingPostProcessing);

    }

    public void SetEffectMaterial(Material material)
    {
        _currentMaterial = material;
        _fullscreenPass.SetMaterial(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_currentMaterial != null)
        {
            renderer.EnqueuePass(_fullscreenPass);
        }
    }

    class FullscreenEffectPass : ScriptableRenderPass
    {
        private Material _material;

        public FullscreenEffectPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
        }

        public void SetMaterial(Material material)
        {
            _material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null)
            {
                //Debug.LogWarning("FullscreenEffectPass: No material assigned.");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("Fullscreen Overlay Pass");

            // Create a temporary render target
            int tempTargetId = Shader.PropertyToID("_TempTarget");
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempTargetId, descriptor);

            RenderTargetIdentifier tempTarget = new RenderTargetIdentifier(tempTargetId);

            // Copy the screen to the temporary texture
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, tempTarget);

            // Bind the temporary texture to _MainTex

            cmd.SetGlobalTexture("_MainTex", tempTarget);

            // Apply the effect and render back to the screen
            cmd.Blit(tempTarget, renderingData.cameraData.renderer.cameraColorTarget, _material);

            // Release the temporary render target
            cmd.ReleaseTemporaryRT(tempTargetId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
