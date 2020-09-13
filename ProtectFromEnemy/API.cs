using HarmonyLib;

namespace ProtectFromEnemy
{
    public class API : IModApi
    {
        public void InitMod()
        {
            Log.Out("ProtectFromEnemy: patching game engine...");
#if DEBUG
            Harmony.DEBUG = true;
#endif
            var harmony = new Harmony("example.7daystodie.protectfromenemy");
            harmony.PatchAll();
            Log.Out("ProtectFromEnemy: done");
        }
    }
}

public class EntityDummy : EntityAlive {}

[HarmonyPatch(typeof(World), "GetLandProtectionHardnessModifier", typeof(Vector3i), typeof(EntityAlive),
    typeof(PersistentPlayerData))]
internal class Patch0
{
    private static EntityDummy _entityDummy = new EntityDummy();
    private static void Prefix(Vector3i blockPos, ref EntityAlive lpRelative, ref PersistentPlayerData ppData)
    {
        if (!(lpRelative is EntityEnemy))
            return;

        lpRelative = _entityDummy;
        ppData = null;
    }
}

[HarmonyPatch(typeof(UnityEngine.Object), "CompareBaseObjects")]
internal class Patch1
{
    private static bool Prefix(ref bool __result, UnityEngine.Object lhs, UnityEngine.Object rhs)
    {
        if (lhs is EntityDummy && rhs is null)
        {
            __result = false;
            return false;
        }

        return true;
    }
}
