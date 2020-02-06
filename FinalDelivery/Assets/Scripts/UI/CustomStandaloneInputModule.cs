using UnityEngine.EventSystems;

namespace UI
{
    public class CustomStandaloneInputModule : StandaloneInputModule
    {
        protected override void Awake()
        {
            inputOverride = GetComponent<BaseInput>();
        }
    }
}