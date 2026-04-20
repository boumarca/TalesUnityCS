using UnityEngine.UI;

namespace Framework.Extensions
{
    public static class ScrollRectExtensions
    {
        public static bool IsScrollable(this ScrollRect scrollRect)
        {
            return scrollRect.content.rect.height > scrollRect.viewport.rect.height;
        }
    }
}
