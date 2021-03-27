using UnityEngine;

namespace TiltBrush
{

  public partial class FreePaintTool
  {
    private bool m_lazyInput;

    void ApplyLazyInput(ref Vector3 pos, ref Quaternion rot)
    {
      if (!m_PaintingActive)
      {
        m_btCursorPos = pos;
        m_btCursorRot = rot;

        return;
      }

      Vector3 deltaPos = pos - m_btCursorPos;

      float brushTriggerRatio = InputManager.Brush.GetTriggerRatio();
      float lerpRate = Mathf.Lerp(Time.deltaTime * brushTriggerRatio * 0.25f, 1, Mathf.Pow(brushTriggerRatio, 5));

      Vector3 deltaBTCursor = Vector3.Lerp(Vector3.zero, deltaPos, lerpRate);

      if (deltaBTCursor.magnitude > 0)
      {
        m_btCursorPos = m_btCursorPos + deltaBTCursor;

        m_btCursorRot = Quaternion.Slerp(m_btCursorRot, rot, lerpRate);
      }

      pos = m_btCursorPos;
      rot = m_btCursorRot;
    }

  }
}