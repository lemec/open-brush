using UnityEngine;

namespace TiltBrush
{

  public partial class FreePaintTool
  {
    private bool m_lazyInput;
    private bool m_showLazyInputVisuals;
    private float m_lazyInputRate;

    void UpdateLazyInputRate()
    {
      float brushTriggerRatio = InputManager.Brush.GetTriggerRatio();
      float lerpRateGoal = brushTriggerRatio * Time.deltaTime * Mathf.Lerp(0.2f, 5, Mathf.Pow(brushTriggerRatio, 5));

      // add laziness to the rate at which laziness changes!
      m_lazyInputRate = Mathf.Lerp(m_lazyInputRate, lerpRateGoal, Time.deltaTime * 0.01f);
      m_lazyInputRate = Mathf.MoveTowards(m_lazyInputRate, lerpRateGoal, Time.deltaTime * 0.01f);
    }

    void ApplyLazyInput(ref Vector3 pos, ref Quaternion rot)
    {
      if (!m_PaintingActive || !m_lazyInput)
      {
        m_btCursorPos = pos;
        m_btCursorRot = rot;
        m_lazyInputRate = 0;

        EndLazyInputVisuals();
        return;
      }

      Vector3 deltaPos = pos - m_btCursorPos;

      UpdateLazyInputRate();

      Vector3 deltaBTCursor = Vector3.Lerp(Vector3.zero, deltaPos, m_lazyInputRate);

      if (deltaBTCursor.magnitude > 0)
      {
        m_btCursorPos = m_btCursorPos + deltaBTCursor;

        m_btCursorRot = Quaternion.Slerp(m_btCursorRot, rot, m_lazyInputRate);
      }

      pos = m_btCursorPos;
      rot = m_btCursorRot;

      UpdateLazyInputVisuals();
    }

    private void UpdateLazyInputVisuals()
    {
      BeginLazyInputVisuals();

      Transform brushAttachTransform = InputManager.m_Instance.GetBrushControllerAttachPoint();

      Vector3 cursorPos = m_btCursorPos;
      Vector3 brushPos = brushAttachTransform.position;

      cursorPos = Vector3.Lerp(brushPos, cursorPos, m_BimanualGuideLineT);

      float line_length = (cursorPos - brushPos).magnitude;
      if (line_length > 0.0f)
      {
        Vector3 brush_to_wand = (cursorPos - brushPos).normalized;
        Vector3 centerpoint = cursorPos - (cursorPos - brushPos) / 2.0f;
        transform.position = centerpoint;
        m_BimanualGuideLine.position = centerpoint;
        m_BimanualGuideLine.up = brush_to_wand;
        m_BimanualGuideLineOutline.position = centerpoint;
        m_BimanualGuideLineOutline.up = brush_to_wand;
        Vector3 temp = Vector3.one * m_BimanualGuideLineBaseWidth * m_BimanualGuideIntensity;
        temp.y = line_length / 2.0f;
        m_BimanualGuideLine.localScale = temp;
        temp.y = line_length / 2.0f + m_BimanualGuideLineOutlineWidth * Mathf.Min(1.0f, 1.0f / line_length) * m_BimanualGuideIntensity;
        temp.x += m_BimanualGuideLineOutlineWidth;
        temp.z += m_BimanualGuideLineOutlineWidth;
        m_BimanualGuideLineOutline.localScale = temp;
      }
      else
      {
        // Short term disable of line
        m_BimanualGuideLine.localScale = Vector3.zero;
        m_BimanualGuideLineOutline.localScale = Vector3.zero;
      }

      m_BimanualGuideLineRenderer.material.SetColor("_Color",
          SketchControlsScript.m_Instance.m_GrabHighlightActiveColor);
    }

    private void BeginLazyInputVisuals()
    {
      if (m_showLazyInputVisuals)
        return;

      m_showLazyInputVisuals = true;
      m_BimanualGuideLineT = 1;

      m_BimanualGuideLineRenderer.material.SetFloat("_Intensity", m_BimanualGuideHintIntensity);
      m_BimanualGuideIntensity = m_BimanualGuideHintIntensity;
      m_BimanualGuideLineRenderer.enabled = true;
      m_BimanualGuideLineOutlineRenderer.enabled = true;
    }

    private void EndLazyInputVisuals()
    {
      if (!m_showLazyInputVisuals)
        return;

      m_showLazyInputVisuals = false;

      m_BimanualGuideLineT = 0;
      m_BimanualGuideLineDrawInTime = 0.0f;
      m_BimanualGuideLineT = 0.0f;
      m_BimanualGuideLineRenderer.enabled = false;
      m_BimanualGuideLineOutlineRenderer.enabled = false;
    }

  }
}