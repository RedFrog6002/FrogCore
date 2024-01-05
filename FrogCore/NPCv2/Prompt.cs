using System;
using System.Collections;

namespace FrogCore.NPCv2
{
    public abstract class Prompt : IEnumerable
    {
        public abstract IEnumerator Up();
        public abstract IEnumerator Down();
        public abstract IEnumerator WaitForSelection();
        public IEnumerator GetEnumerator() => WaitForSelection();
    }
}