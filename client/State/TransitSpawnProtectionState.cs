using System.Collections.Generic;
using UnityEngine;

namespace Vagabond.Client.State;

internal static class TransitSpawnProtectionState
{
    private sealed class ProtectedArea
    {
        public Vector3 Position;
        public float Radius;
        public float ExpireAt;
    }

    private static readonly List<ProtectedArea> Areas = new();

    public static void Add(Vector3 position, float radius, float lifetimeSeconds)
    {
        Cleanup();

        Areas.Add(new ProtectedArea
        {
            Position = position,
            Radius = radius,
            ExpireAt = Time.time + lifetimeSeconds
        });
    }

    public static bool HasActiveAreas()
    {
        Cleanup();
        return Areas.Count > 0;
    }

    public static bool IsProtected(Vector3 position)
    {
        Cleanup();

        foreach (var area in Areas)
        {
            var radiusSq = area.Radius * area.Radius;
            if ((position - area.Position).sqrMagnitude < radiusSq)
            {
                return true;
            }
        }

        return false;
    }

    public static float MinDistanceSq(Vector3 position)
    {
        Cleanup();

        if (Areas.Count == 0)
        {
            return float.MaxValue;
        }

        var min = float.MaxValue;

        foreach (var area in Areas)
        {
            var distSq = (position - area.Position).sqrMagnitude;
            if (distSq < min)
            {
                min = distSq;
            }
        }

        return min;
    }

    public static void Clear()
    {
        Areas.Clear();
    }

    private static void Cleanup()
    {
        Areas.RemoveAll(x => Time.time > x.ExpireAt);
    }
}