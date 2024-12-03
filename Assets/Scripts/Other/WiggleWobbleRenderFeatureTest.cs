using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WiggleWobbleRenderFeatureTest : ScriptableRendererFeature
{
    class WiggleWobblePass : ScriptableRenderPass
    {
        private readonly Material _material;
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _temporaryColorTexture;
        public bool IsEnabled { get; set; } = true; // Toggle control

        public WiggleWobblePass(Material material)
        {
            _material = material;
            _temporaryColorTexture.Init("_TemporaryColorTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            _source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null || !IsEnabled) return;

            CommandBuffer cmd = CommandBufferPool.Get("Wiggle Wobble Effect");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(_temporaryColorTexture.id, opaqueDesc);

            Blit(cmd, _source, _temporaryColorTexture.Identifier(), _material);
            Blit(cmd, _temporaryColorTexture.Identifier(), _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_temporaryColorTexture.id);
        }
    }

    [SerializeField] private Material _wobbleMaterial;
    private WiggleWobblePass _wobblePass;

    public override void Create()
    {
        _wobblePass = new WiggleWobblePass(_wobbleMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        _wobblePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(_wobblePass);
    }

    public void ToggleWobbleEffect(bool isEnabled)
    {
        _wobblePass.IsEnabled = isEnabled;
    }
}
