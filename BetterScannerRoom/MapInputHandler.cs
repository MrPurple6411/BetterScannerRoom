namespace BetterScannerRoom;

using UnityEngine;

internal class MapInputHandler : IInputHandler
{
    public bool canHandleInput;
    private int focusFrame = -1;
    public MapRoomFunctionality mapRoom;

    public bool HandleInput()
    {
        return true;
    }

    public bool HandleLateInput()
    {
        if (!canHandleInput || mapRoom == null)
            return false;

        if (this.focusFrame != Time.frameCount && GameInput.GetButtonDown(GameInput.Button.UIMenu))
        {
            canHandleInput = false;
            return false;
        }

        var delta = GameInput.GetLookDelta();
        if (delta.magnitude > 0)
        {
            var height = delta.y / 2;
            var rotation = delta.x * 10;

            var holder = mapRoom.miniWorld.hologramHolder;

            if (GameInput.GetButtonHeld(GameInput.Button.LeftHand))
                holder.Translate(new Vector3(0, height));
            if (GameInput.GetButtonHeld(GameInput.Button.RightHand))
                holder.Rotate(new Vector3(0, rotation));
        }

        return true;
    }

    public void OnFocusChanged(InputFocusMode mode)
    {
        switch (mode)
        {
            case InputFocusMode.Add:
            case InputFocusMode.Restore:
                this.focusFrame = Time.frameCount;
                UWE.Utils.lockCursor = true;
                return;
            case InputFocusMode.Remove:
                break;
            case InputFocusMode.Suspend:
                break;
            default:
                return;
        }
    }
}
