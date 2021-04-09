using UnityEngine;

namespace TiltBrush {

  public partial class DraftingTool : BaseTool {
#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)

    public bool m_brushTrigger { get; private set; }
    public bool m_brushTriggerDown { get; private set; }
    public bool m_wandTrigger { get; private set; }
    public bool m_wandTriggerDown { get; private set; }

    public bool m_brushUndoButton { get; private set; }
    public bool m_brushUndoButtonDown { get; private set; }

    public bool m_brushUndoButtonUp { get; private set; }
    public bool m_brushUndoButtonTapped { get; private set; }
    private bool m_brushUndoButtonTapInvalid { get; set; }
    private float m_brushUndoButtonTapExpiry { get; set; }
    private const float TapDelayTime = 0.333f;
    public bool m_brushUndoButtonHeld { get; private set; }

    public float m_brushTriggerRatio { get; private set; }
    public float m_wandTriggerRatio { get; private set; }

    public override bool BlockPinCushion() {
      return m_brushTrigger || m_wandTrigger;
    }

    private void UpdateInputs() {
      m_wandTriggerRatio = InputManager.Wand.GetTriggerRatio();
      m_wandTrigger = InputManager.Wand.GetCommand(InputManager.SketchCommands.Activate);
      m_wandTriggerDown = InputManager.Wand.GetCommandDown(InputManager.SketchCommands.Activate);

      m_brushTriggerRatio = InputManager.Brush.GetTriggerRatio();
      m_brushTrigger = InputManager.Brush.GetCommand(InputManager.SketchCommands.Activate);
      m_brushTriggerDown = InputManager.Brush.GetCommandDown(InputManager.SketchCommands.Activate);

      m_brushUndoButton = InputManager.Brush.GetCommand(InputManager.SketchCommands.Undo);
      m_brushUndoButtonDown = InputManager.Brush.GetCommandDown(InputManager.SketchCommands.Undo);

      m_brushUndoButtonUp = InputManager.Brush.GetCommandUp(InputManager.SketchCommands.Undo);
      m_brushUndoButtonTapped = m_brushUndoButtonUp && !m_brushUndoButtonTapInvalid;

      if (m_brushUndoButtonDown) {
        m_brushUndoButtonTapInvalid = false;
        m_brushUndoButtonTapExpiry = TapDelayTime;
      }

      if (!m_brushUndoButtonTapInvalid) {
        m_brushUndoButtonTapExpiry = Mathf.MoveTowards(m_brushUndoButtonTapExpiry, 0, Time.deltaTime);
        if (m_brushTriggerDown || m_brushUndoButtonTapExpiry <= 0)
          m_brushUndoButtonTapInvalid = true;
      }

      m_brushUndoButtonHeld = m_brushUndoButtonTapInvalid && m_brushUndoButton;
    }

    public override void UpdateTool() {
      UpdateInputs();

      base.UpdateTool();
    }

#endif // (UNITY_EDITOR || EXPERIMENTAL_ENABLED)
  }

}
