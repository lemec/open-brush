using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)

namespace TiltBrush {

  public class DepthPing : MonoBehaviour {
    [Range(0, 1)]
    public float m_pingRate = 1;
    public float m_pingThickness = 0.1f;
    public float m_geometryScale = 15;

    [Range(1, 100)]
    public float m_pingDiameter = 15;
    private float m_pingPhase;

    private List<Transform> m_shells;
    private List<Renderer> m_renderers;
    public Gradient m_Color;

    // Start is called before the first frame update
    void Start() {
      m_pingPhase = 0;

      m_shells = new List<Transform>();
      m_renderers = new List<Renderer>();

      for (int i = 0; i < transform.childCount; i++) {
        m_shells.Add(transform.GetChild(i));
        m_renderers.Add(m_shells[i].GetComponent<Renderer>());
      }
    }

    // Update is called once per frame
    void Update() {
      float incr = 1f / m_shells.Count;
      m_pingPhase = (m_pingPhase + m_pingRate * Time.deltaTime) % 1f;

      for (int i = 0; i < m_shells.Count; i++) {

        float pingPhase = (m_pingPhase + i * incr) % 1f;
        float diameter = Mathf.Lerp(0, m_pingDiameter, pingPhase);
        float scale = (1f / m_geometryScale) * diameter;
        m_shells[i].localScale = Vector3.one * scale;
        m_renderers[i].material.SetFloat("_HighlightThresholdMax", Mathf.Lerp(0, m_pingDiameter, pingPhase) + m_pingThickness);
        m_renderers[i].material.SetColor("_Color", m_Color.Evaluate(pingPhase).FadeAlpha(Mathf.InverseLerp(m_pingDiameter, m_pingDiameter - m_pingThickness, diameter)));
      }
    }
  }

}
#endif