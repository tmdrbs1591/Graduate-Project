using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace INab.WorldScanFX.URP
{
	
	public class ScanFXFeature : ScriptableRendererFeature
	{
		public Material scanMaterial;
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingTransparents;

        private ScanFXPass scanFXPass;

        public override void Create()
		{

            scanFXPass = new ScanFXPass(Event, scanMaterial);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (renderingData.cameraData.cameraType != CameraType.Game && renderingData.cameraData.cameraType != CameraType.SceneView) return;
            
			scanFXPass.Setup();
			renderer.EnqueuePass(scanFXPass);
		}

		public class ScanFXPass : ScriptableRenderPass
		{
			public Material scanMaterial;

			private RenderTargetIdentifier source;
			private RenderTargetIdentifier destination;

            private RenderTargetHandle m_TemporaryColorTexture;

            public ScanFXPass(RenderPassEvent renderPassEvent,Material scanMaterial)
			{
				this.renderPassEvent = renderPassEvent;
                this.scanMaterial = scanMaterial;

				m_TemporaryColorTexture.Init("_TemporaryColorTexture");
            }

            public void Setup()
			{
				ConfigureInput(ScriptableRenderPassInput.Normal);
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get("World Scan FX");
				RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
				opaqueDesc.depthBufferBits = 0;
				var renderer = renderingData.cameraData.renderer;

				source = renderer.cameraColorTarget;
				destination = renderer.cameraColorTarget;

				Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);

				cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, FilterMode.Point);

				Blit(cmd, source, m_TemporaryColorTexture.Identifier(), scanMaterial);
				Blit(cmd, m_TemporaryColorTexture.Identifier(), destination);

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}

			public override void FrameCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
			}
		}
	}
}