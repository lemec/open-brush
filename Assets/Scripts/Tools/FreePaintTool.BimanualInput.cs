﻿using UnityEngine;

namespace TiltBrush
{

  public partial class FreePaintTool
  {
    [SerializeField] private Transform m_BimanualGuideLine;
    [SerializeField] private Transform m_BimanualGuideLineOutline;
    private Renderer m_BimanualGuideLineRenderer;
    private Renderer m_BimanualGuideLineOutlineRenderer;

    private float m_BimanualGuideLineDrawInTime = 0.0f;


    [SerializeField] private Transform m_BimanualGuideIntersect;
    [SerializeField] private Transform m_BimanualGuideIntersectOutline;
    private Renderer m_BimanualGuideIntersectRenderer;
    private Renderer m_BimanualGuideIntersectOutlineRenderer;
    private bool m_BimanualGuideIntersectVisible;

    private float m_BimanualGuideLineT;
    private float m_BimanualGuideIntensity = 1;
    [SerializeField] private float m_BimanualGuideLineHorizontalOffset = 0.75f;
    [SerializeField] private float m_BimanualGuideLineOutlineWidth = 0.05f;
    [SerializeField] private float m_BimanualGuideLineBaseWidth = 0.015f;
    [SerializeField] private float m_BimanualGuideHintIntensity = 0.75f;
    [SerializeField] private float m_BimanualGuideDrawInDuration = 0.3f;

    private bool m_BimanualTape;
    private Vector3 m_btCursorPos;
    private Quaternion m_btCursorRot;

    Vector3 m_btIntersectGoal;

    private void InitBimanualTape()
    {
      m_BimanualTape = false;

      m_BimanualGuideLineRenderer = m_BimanualGuideLine.GetComponent<Renderer>();
      m_BimanualGuideLineOutlineRenderer = m_BimanualGuideLineOutline.GetComponent<Renderer>();
      m_BimanualGuideLineRenderer.enabled = false;
      m_BimanualGuideLineOutlineRenderer.enabled = false;

      m_BimanualGuideIntersectRenderer = m_BimanualGuideIntersect.GetComponent<Renderer>();
      m_BimanualGuideIntersectOutlineRenderer = m_BimanualGuideIntersectOutline.GetComponent<Renderer>();
      m_BimanualGuideIntersectRenderer.enabled = false;
      m_BimanualGuideIntersectOutlineRenderer.enabled = false;

      EndBimanualTape();
    }

    private void BeginBimanualTape()
    {
      m_BimanualTape = true;
      Transform rAttachPoint = InputManager.m_Instance.GetBrushControllerAttachPoint();

      m_btCursorPos = rAttachPoint.position;
      m_btCursorRot = rAttachPoint.rotation * sm_OrientationAdjust;

      m_BimanualGuideLineRenderer.material.SetFloat("_Intensity", m_BimanualGuideHintIntensity);
      m_BimanualGuideIntensity = m_BimanualGuideHintIntensity;
      m_BimanualGuideLineRenderer.enabled = true;
      m_BimanualGuideLineOutlineRenderer.enabled = true;
    }

    private void EndBimanualTape()
    {
      m_BimanualTape = false;

      m_BimanualGuideLineDrawInTime = 0.0f;
      m_BimanualGuideLineT = 0.0f;
      m_BimanualGuideLineRenderer.enabled = false;
      m_BimanualGuideLineOutlineRenderer.enabled = false;

      if (m_BimanualGuideIntersectVisible)
        EndBimanualIntersect();
    }

    void UpdateBimanualGuideLineT()
    {
      if (m_BimanualGuideLineT < 1.0f)
      {
        m_BimanualGuideLineT = Mathf.SmoothStep(0.0f, 1.0f,
            Mathf.Clamp(m_BimanualGuideLineDrawInTime / m_BimanualGuideDrawInDuration, 0.0f, 1.0f));
        m_BimanualGuideLineDrawInTime += Time.deltaTime;
      }
    }


    private void UpdateBimanualGuideVisuals()
    {
      if (!m_PaintingActive)
      {
        Transform brushAttachTransform = InputManager.m_Instance.GetBrushControllerAttachPoint();
        m_btCursorPos = brushAttachTransform.position;
        m_btCursorRot = brushAttachTransform.rotation * sm_OrientationAdjust;
      }

      Transform wandAttachTransform = InputManager.m_Instance.GetWandControllerAttachPoint();

      Vector3 brush_pos = m_btCursorPos;
      Vector3 wand_pos = wandAttachTransform.position;

      brush_pos = Vector3.Lerp(wand_pos, brush_pos, m_BimanualGuideLineT);

      float line_length = (brush_pos - wand_pos).magnitude;
      if (line_length > 0.0f)
      {
        Vector3 brush_to_wand = (brush_pos - wand_pos).normalized;
        Vector3 centerpoint = brush_pos - (brush_pos - wand_pos) / 2.0f;
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


    private void BeginBimanualIntersect()
    {
      m_BimanualGuideIntersectRenderer.material.SetFloat("_Intensity", m_BimanualGuideHintIntensity);
      m_BimanualGuideIntersectRenderer.enabled = true;
      m_BimanualGuideIntersectOutlineRenderer.enabled = true;
      m_BimanualGuideIntersectVisible = true;
    }
    private void EndBimanualIntersect()
    {
      m_BimanualGuideIntersectRenderer.enabled = false;
      m_BimanualGuideIntersectOutlineRenderer.enabled = false;
      m_BimanualGuideIntersectVisible = false;
    }

    private void UpdateBimanualIntersectVisuals()
    {
      if (!m_BimanualGuideIntersectVisible)
        BeginBimanualIntersect();

      Transform brushAttachTransform = InputManager.m_Instance.GetBrushControllerAttachPoint();

      Vector3 brush_pos = brushAttachTransform.position;
      Vector3 intersect_pos = m_btIntersectGoal;

      brush_pos = Vector3.Lerp(intersect_pos, brush_pos, m_BimanualGuideLineT);

      float line_length = (brush_pos - intersect_pos).magnitude;
      if (line_length > 0.0f)
      {
        Vector3 brush_to_wand = (brush_pos - intersect_pos).normalized;
        Vector3 centerpoint = brush_pos - (brush_pos - intersect_pos) / 2.0f;
        transform.position = centerpoint;
        m_BimanualGuideIntersect.position = centerpoint;
        m_BimanualGuideIntersect.up = brush_to_wand;
        m_BimanualGuideIntersectOutline.position = centerpoint;
        m_BimanualGuideIntersectOutline.up = brush_to_wand;
        Vector3 temp = Vector3.one * m_BimanualGuideLineBaseWidth * m_BimanualGuideIntensity;
        temp.y = line_length / 2.0f;
        m_BimanualGuideIntersect.localScale = temp;
        temp.y = line_length / 2.0f + m_BimanualGuideLineOutlineWidth * Mathf.Min(1.0f, 1.0f / line_length) * m_BimanualGuideIntensity;
        temp.x += m_BimanualGuideLineOutlineWidth;
        temp.z += m_BimanualGuideLineOutlineWidth;
        m_BimanualGuideIntersectOutline.localScale = temp;
      }
      else
      {
        // Short term disable of line
        m_BimanualGuideIntersect.localScale = Vector3.zero;
        m_BimanualGuideIntersectOutline.localScale = Vector3.zero;
      }

      m_BimanualGuideIntersectRenderer.material.SetColor("_Color",
          SketchControlsScript.m_Instance.m_GrabHighlightActiveColor);
    }




    void ApplyBimanualTape(ref Vector3 pos, ref Quaternion rot)
    {
      float brushTriggerRatio = InputManager.Brush.GetTriggerRatio();

      Transform lAttachPoint = InputManager.m_Instance.GetWandControllerAttachPoint();
      Vector3 lPos = lAttachPoint.position;
      Quaternion lrot = lAttachPoint.rotation * sm_OrientationAdjust;

      Vector3 deltaPos = lPos - m_btCursorPos;
      Vector3 deltaBTCursor = pos - m_btCursorPos;


      Vector3 btCursorGoalDelta = Vector3.Project(deltaBTCursor, deltaPos.normalized);


      if (Vector3.Dot(btCursorGoalDelta.normalized, deltaPos.normalized) > 0)
      {
        m_btIntersectGoal = m_btCursorPos + btCursorGoalDelta;

        float lerpRate = Mathf.Lerp(Time.deltaTime * brushTriggerRatio, 1, Mathf.Pow(brushTriggerRatio, 5));

        btCursorGoalDelta = Vector3.Lerp(Vector3.zero, btCursorGoalDelta, lerpRate);

        if (btCursorGoalDelta.magnitude < deltaPos.magnitude)
        {
          m_btCursorPos = m_btCursorPos + btCursorGoalDelta;

          Transform rAttachPoint = InputManager.m_Instance.GetBrushControllerAttachPoint();
          Vector3 intersectDelta = m_btIntersectGoal - rAttachPoint.position;
          if (intersectDelta.magnitude > GetSize())
          {            
            Quaternion btCursorRotGoal = Quaternion.LookRotation(intersectDelta.normalized, deltaPos.normalized) * sm_OrientationAdjust;
            m_btCursorRot = Quaternion.Slerp(m_btCursorRot, btCursorRotGoal, lerpRate);
          }
        }
        else
          m_btCursorPos = lPos;
      }

      pos = m_btCursorPos;
      rot = m_btCursorRot;
    }


  }



}