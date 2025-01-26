using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrogCore.Unity;

[Flags]
public enum CollisionDirection
{
    Left = 1,
    Right = 2,
    Down = 4,
    Up = 8,
    DownLeft = 16,
    DownMiddle = 32,
    DownRight = 64,
    UpLeft = 128,
    UpMiddle = 256,
    UpRight = 512,
    MiddleRight = 1024,
    MiddleLeft = 2048
}

public static class CollisionDirectionUtilities
{
    public const float RaycastLength = 0.08f;

    public static bool CheckDirectionRaycast(Collider2D self, int layer, CollisionDirection direction, float? overrideLength = null)
    {
        if (!overrideLength.HasValue)
            overrideLength = RaycastLength;

        List<Rect> rays = new List<Rect>();

        if (direction.HasFlag(CollisionDirection.Left) || direction.HasFlag(CollisionDirection.UpLeft)) rays.Add(new Rect(self.bounds.min.x, self.bounds.max.y, -1f, 0f));
        if (direction.HasFlag(CollisionDirection.Left) || direction.HasFlag(CollisionDirection.MiddleLeft)) rays.Add(new Rect(self.bounds.min.x, self.bounds.center.y, -1f, 0f));
        if (direction.HasFlag(CollisionDirection.Left) || direction.HasFlag(CollisionDirection.DownLeft)) rays.Add(new Rect(self.bounds.min.x, self.bounds.min.y, -1f, 0f));
        
        if (direction.HasFlag(CollisionDirection.Right) || direction.HasFlag(CollisionDirection.UpRight)) rays.Add(new Rect(self.bounds.max.x, self.bounds.max.y, 1f, 0f));
        if (direction.HasFlag(CollisionDirection.Right) || direction.HasFlag(CollisionDirection.MiddleRight)) rays.Add(new Rect(self.bounds.max.x, self.bounds.center.y, 1f, 0f));
        if (direction.HasFlag(CollisionDirection.Right) || direction.HasFlag(CollisionDirection.DownRight)) rays.Add(new Rect(self.bounds.max.x, self.bounds.min.y, 1f, 0f));
        
        if (direction.HasFlag(CollisionDirection.Down) || direction.HasFlag(CollisionDirection.DownLeft)) rays.Add(new Rect(self.bounds.min.x, self.bounds.min.y, 0f, -1f));
        if (direction.HasFlag(CollisionDirection.Down) || direction.HasFlag(CollisionDirection.DownMiddle)) rays.Add(new Rect(self.bounds.center.x, self.bounds.min.y, 0f, -1f));
        if (direction.HasFlag(CollisionDirection.Down) || direction.HasFlag(CollisionDirection.DownRight)) rays.Add(new Rect(self.bounds.max.x, self.bounds.min.y, 0f, -1f));
        
        if (direction.HasFlag(CollisionDirection.Up) || direction.HasFlag(CollisionDirection.UpLeft)) rays.Add(new Rect(self.bounds.min.x, self.bounds.max.y, 0f, 1f));
        if (direction.HasFlag(CollisionDirection.Up) || direction.HasFlag(CollisionDirection.UpMiddle)) rays.Add(new Rect(self.bounds.center.x, self.bounds.max.y, 0f, 1f));
        if (direction.HasFlag(CollisionDirection.Up) || direction.HasFlag(CollisionDirection.UpRight)) rays.Add(new Rect(self.bounds.max.x, self.bounds.max.y, 0f, 1f));

        foreach (Rect settings in rays)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(settings.position, settings.size, overrideLength.Value, 1 << layer);
            if (raycastHit2D.collider)
                return true;
        }
        return false;
    }

    public static bool CheckDirection(BoxCollider2D self, Vector2 point, CollisionDirection direction)
    {
        Vector2 normalizedPoint = (point - ((Vector2)self.transform.position + self.offset)) / self.size;

        return CheckSimple(normalizedPoint, direction) || CheckComplex(normalizedPoint, direction);
    }

    private static bool CheckSimple(Vector2 normalizedPoint, CollisionDirection direction)
    {
        CollisionDirection middleDir = CollisionDirection.Right;
        if (normalizedPoint.x < 0f)
        {
            normalizedPoint = new Vector2(-normalizedPoint.x, normalizedPoint.y);
            middleDir = CollisionDirection.Left;
        }

        return (direction.HasFlag(CollisionDirection.Up) && normalizedPoint.y > normalizedPoint.x)
            || (direction.HasFlag(middleDir) && normalizedPoint.y > -normalizedPoint.x)
            || (direction.HasFlag(CollisionDirection.Down)); // normalizedPoint.y < normalizedPoint.x
    }

    private static bool CheckComplex(Vector2 normalizedPoint, CollisionDirection direction) =>
           (direction.HasFlag(CollisionDirection.DownLeft) && IsInsideRegion(normalizedPoint, new Rect(-1f, -1f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.DownMiddle) && IsInsideRegion(normalizedPoint, new Rect(-1f/3f, -1f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.DownRight) && IsInsideRegion(normalizedPoint, new Rect(1f/3f, -1f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.MiddleLeft) && IsInsideRegion(normalizedPoint, new Rect(-1f, -1f/3f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.MiddleRight) && IsInsideRegion(normalizedPoint, new Rect(1f/3f, -1f/3f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.UpLeft) && IsInsideRegion(normalizedPoint, new Rect(-1f, 1f/3f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.UpMiddle) && IsInsideRegion(normalizedPoint, new Rect(-1f/3f, 1f/3f, 2f/3f, 2f/3f)))
        || (direction.HasFlag(CollisionDirection.UpRight) && IsInsideRegion(normalizedPoint, new Rect(1f/3f, 1f/3f, 2f/3f, 2f/3f)));

    private static bool IsInsideRegion(Vector2 point, Rect region) => (0f <= (point.x - region.x) && (point.x - region.x) <= region.width) && (0f <= (point.y - region.y) && (point.y - region.y) <= region.height);
}