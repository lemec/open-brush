using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TiltBrush {
  public class DepthGuide : MonoBehaviour {
#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)

    public Renderer meshRenderer;

    [SerializeField]
    protected float m_MaxRadius = 2;
    protected float m_lerpRadius;

    // Start is called before the first frame update
    void Start() {

    }

    private void OnEnable() {
      if (!Config.IsExperimental) {
        enabled = false;
        return;
      }

      m_lerpRadius = 0;
    }

    private void OnDisable() {
      meshRenderer.enabled = false;
    }

    private void FollowBrushController() {
      if (!InputManager.Brush.IsTrackedObjectValid) {
        meshRenderer.enabled = false;
        return;
      }

      if (!meshRenderer.enabled) {
        meshRenderer.enabled = true;
        m_lerpRadius = 0;
      }

      float headDist = (transform.parent.position - transform.position).magnitude;

			
      if (SketchControlsScript.m_Instance.IsUserInteractingWithUI()) {
        headDist = 0;
      }

      float headLerpTarget = Mathf.InverseLerp(0, m_MaxRadius, headDist);

      m_lerpRadius = Mathf.Lerp(m_lerpRadius, headLerpTarget, Time.deltaTime * 3);
      transform.localScale = Vector3.one * Mathf.Lerp(0, 0.1f * m_MaxRadius, m_lerpRadius);
      transform.position = InputManager.m_Instance.GetBrushControllerAttachPoint().position;

      meshRenderer.material.SetFloat("_MaxDistance", headDist);
      meshRenderer.material.SetFloat("_Radius", m_lerpRadius * m_MaxRadius);
    }

    // Update is called once per frame
    void Update() {
      FollowBrushController();
    }
  }

#endif
}
