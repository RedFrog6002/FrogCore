using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InControl;

namespace FrogCore
{
    /// <summary>
    /// UNFINISHED! DO NOT USE
    /// </summary>
    public abstract class InventoryPanel
    {
        public GameObject go;
        public PlayMakerFSM cursorFSM;
        public Vector2 ULcursorOffset = Vector2.zero;
        public Vector2 URcursorOffset = Vector2.zero;
        public Vector2 DLcursorOffset = Vector2.zero;
        public Vector2 DRcursorOffset = Vector2.zero;
        public InvSelectable Current { get; private set; }
        public bool AcceptingInput = true;
        public float timeAcceptingInput = 0f;
        private bool built = false;
        /// <summary>
        /// call this method, then you can use the customization methods and fields
        /// </summary>
        public void Build()
        {
            if (!built)
            {
                InventoryHelper.AddInventoryPanel(this);
                built = true;
            }
        }
        public virtual void OnEnable()
        {
            cursorFSM.SendEvent("CURSOR ACTIVATE");
        }
        public virtual void OnDisable()
        {
            cursorFSM.SendEvent("DOWN");
        }
        public virtual void OnUpdate()
        {
            if (AcceptingInput)
            {
                if (InputHandler.Instance.inputActions.up.WasPressed)
                {
                    SetSelected(Up());
                    timeAcceptingInput = 0.2f;
                    AcceptingInput = false;
                }
                if (InputHandler.Instance.inputActions.down.WasPressed)
                {
                    SetSelected(Down());
                    timeAcceptingInput = 0.2f;
                    AcceptingInput = false;
                }
                if (InputHandler.Instance.inputActions.left.WasPressed)
                {
                    SetSelected(Left());
                    timeAcceptingInput = 0.2f;
                    AcceptingInput = false;
                }
                if (InputHandler.Instance.inputActions.right.WasPressed)
                {
                    SetSelected(Right());
                    timeAcceptingInput = 0.2f;
                    AcceptingInput = false;
                }
                if (InputHandler.Instance.inputActions.menuSubmit.WasPressed)
                {
                    SetSelected(Select());
                    timeAcceptingInput = 0.2f;
                    AcceptingInput = false;
                }
                /*if (InputHandler.Instance.inputActions.menuCancel.WasPressed)
                {
                    SetSelected(Cancel());
                    timeAcceptingInput = 0.2f;
                    AcceptingInput = false;
                }*/
            }
            else
            {
                timeAcceptingInput -= Time.deltaTime;
                if (timeAcceptingInput <= 0)
                    AcceptingInput = true;
            }
        }
        public void SetCursorSprite(Sprite sprite, CursorPart part = CursorPart.UL | CursorPart.UR | CursorPart.DL | CursorPart.DR)
        {
            if (part.HasFlag(CursorPart.UL))
                cursorFSM.transform.Find("TL").Find("TL Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
            if (part.HasFlag(CursorPart.UR))
                cursorFSM.transform.Find("TR").Find("TR Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
            if (part.HasFlag(CursorPart.DL))
                cursorFSM.transform.Find("BL").Find("BL Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
            if (part.HasFlag(CursorPart.DR))
                cursorFSM.transform.Find("BR").Find("BR Sprite").GetComponent<SpriteRenderer>().sprite = sprite;
        }
        public void SetSelected(InvSelectable? selected)
        {
            if (selected.HasValue)
            {
                UpdateCursorFSM(selected.Value.selected?.GetComponent<BoxCollider2D>()?.size ?? selected.Value.sizeOverride ?? Vector2.zero,
                    selected.Value.selected?.GetComponent<BoxCollider2D>()?.offset ?? selected.Value.offsetOverride ?? Vector2.zero,
                    selected.Value.selected?.transform?.position ?? go?.transform?.position ?? Vector3.zero);
                Current = selected.Value;
            }
        }
        private void UpdateCursorFSM(Vector2 bounds, Vector2 offset, Vector3 pos)
        {
            cursorFSM.FsmVariables.FindFsmVector3("MoveToPos").Value = pos;
            cursorFSM.FsmVariables.FindFsmVector2("ColliderBounds").Value = bounds;
            cursorFSM.FsmVariables.FindFsmFloat("Box Offset X").Value = offset.x;
            cursorFSM.FsmVariables.FindFsmFloat("Box Offset Y").Value = offset.y;
            cursorFSM.SendEvent("CURSOR MOVE");
        }
        internal void SetCursorOffsets()
        {
            cursorFSM.FsmVariables.FindFsmVector3("TL Pos").Value += (Vector3)ULcursorOffset;
            cursorFSM.FsmVariables.FindFsmVector3("TR Pos").Value += (Vector3)URcursorOffset;
            cursorFSM.FsmVariables.FindFsmVector3("BL Pos").Value += (Vector3)DLcursorOffset;
            cursorFSM.FsmVariables.FindFsmVector3("BR Pos").Value += (Vector3)DRcursorOffset;
        }
        public abstract InvSelectable FromLeft();
        public abstract InvSelectable FromRight();
        public abstract InvSelectable FromOpen();
        public abstract InvSelectable? Up();
        public abstract InvSelectable? Down();
        public abstract InvSelectable? Left();
        public abstract InvSelectable? Right();
        public abstract InvSelectable? Select();
        //public abstract InvSelectable? Cancel();
    }
    public struct InvSelectable
    {
        public GameObject selected;
        public Vector2? sizeOverride;
        public Vector2? offsetOverride;
    }
    [Flags]
    public enum CursorPart
    {
        UL,
        UR,
        DL,
        DR
    }
}
