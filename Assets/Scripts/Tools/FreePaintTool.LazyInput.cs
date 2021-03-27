using UnityEngine;

namespace TiltBrush
{

  public partial class FreePaintTool
  {
    private bool m_lazyInput;
    private float m_lazyInputRate;

    void ApplyLazyInput(ref Vector3 pos, ref Quaternion rot)
    {
      if (!m_PaintingActive)
      {
        m_btCursorPos = pos;
        m_btCursorRot = rot;
        m_lazyInputRate = 0;

        return;
      }

      Vector3 deltaPos = pos - m_btCursorPos;

      float brushTriggerRatio = InputManager.Brush.GetTriggerRatio();
      float lerpRateGoal = Time.deltaTime * Mathf.Lerp(0.1f, 5, Mathf.Pow(brushTriggerRatio, 5));

      // add laziness to the rate at which laziness changes!
      m_lazyInputRate = Mathf.Lerp(m_lazyInputRate, lerpRateGoal, Time.deltaTime * 0.01f);
      m_lazyInputRate = Mathf.MoveTowards(m_lazyInputRate, lerpRateGoal, Time.deltaTime * 0.01f);
      Vector3 deltaBTCursor = Vector3.Lerp(Vector3.zero, deltaPos, m_lazyInputRate);

      if (deltaBTCursor.magnitude > 0)
      {
        m_btCursorPos = m_btCursorPos + deltaBTCursor;

        m_btCursorRot = Quaternion.Slerp(m_btCursorRot, rot, m_lazyInputRate);
      }

      pos = m_btCursorPos;
      rot = m_btCursorRot;
    }

  }
}