using UnityEngine;

[RequireComponent(typeof(SpriteRoot))]
public class GameRenderTarget : MonoBehaviour
{
	private SpriteRoot m_sprite;

	private Camera m_camera;

	private void Start()
	{
		m_sprite = base.gameObject.GetComponent<SpriteRoot>();
		m_sprite.GetComponent<Renderer>().enabled = false;
	}

	private void OnDestroy()
	{
		if (m_sprite != null && m_sprite.GetComponent<Renderer>().sharedMaterial != null)
		{
			RenderTexture renderTexture = (RenderTexture)m_sprite.GetComponent<Renderer>().sharedMaterial.mainTexture;
			if (renderTexture != null)
			{
				TBFUtils.DebugLog("Unloading render target");
				Resources.UnloadAsset(renderTexture);
			}
		}
	}

	public Camera SetCamera(Camera newCam)
	{
		if (m_camera == newCam)
		{
			return m_camera;
		}
		Camera camera = m_camera;
		if (camera != null)
		{
			camera.targetTexture = null;
			camera.ResetAspect();
		}
		if (m_sprite != null)
		{
			if (newCam != null)
			{
				if (m_sprite.GetComponent<Renderer>().sharedMaterial != null)
				{
					RenderTexture renderTexture = (RenderTexture)m_sprite.GetComponent<Renderer>().sharedMaterial.mainTexture;
					if (renderTexture != null)
					{
						newCam.targetTexture = renderTexture;
						float num = Screen.width;
						float num2 = Screen.height;
						if (m_sprite != null)
						{
							float worldUnitsPerScreenPixel = m_sprite.GetWorldUnitsPerScreenPixel();
							float w = num * worldUnitsPerScreenPixel;
							float h = num2 * worldUnitsPerScreenPixel;
							m_sprite.SetSize(w, h);
						}
						newCam.aspect = num / num2;
					}
					m_sprite.GetComponent<Renderer>().enabled = true;
				}
			}
			else
			{
				m_sprite.GetComponent<Renderer>().enabled = false;
			}
		}
		m_camera = newCam;
		return camera;
	}
}
