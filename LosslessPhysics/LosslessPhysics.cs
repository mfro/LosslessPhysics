using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LosslessPhysics
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class LosslessPhysics : MonoBehaviour
  {
    // relevant private fields
    private static FieldInfo field_last_rate = typeof(TimeWarp).GetField("last_rate", BindingFlags.Instance | BindingFlags.NonPublic);
    private static FieldInfo field_curr_rate = typeof(TimeWarp).GetField("curr_rate", BindingFlags.Instance | BindingFlags.NonPublic);
    private static FieldInfo field_LerpStartTime = typeof(TimeWarp).GetField("LerpStartTime", BindingFlags.Instance | BindingFlags.NonPublic);

    // updateRate method
    // this method updates Planetarium.TimeScale, Time.timeScale, and Time.fixedDeltaTime
    private static MethodInfo method_updateRate = typeof(TimeWarp).GetMethod("updateRate", BindingFlags.Instance | BindingFlags.NonPublic);

    void Start()
    {
      DontDestroyOnLoad(this);

      var harmony = new Harmony("LosslessPhysics");
      var postfix = SymbolExtensions.GetMethodInfo(() => after_UpdateRate());

      harmony.Patch(method_updateRate, postfix: new HarmonyMethod(postfix));

      Debug.Log("LosslessPhysics harmony patch complete");
    }

    private static void after_UpdateRate()
    {
      if (TimeWarp.fetch.Mode == TimeWarp.Modes.LOW)
      {
        // Debug.Log($"updateRate call");
        // Debug.Log($"  timeScale:                {Time.timeScale}");
        // Debug.Log($"  planetarium:              {Planetarium.TimeScale}");

        // Debug.Log($"  deltaTime:                {Time.deltaTime}");
        // Debug.Log($"  fixedDeltaTime:           {Time.fixedDeltaTime}");
        // Debug.Log($"  unscaledDeltaTime:        {Time.unscaledDeltaTime}");
        // Debug.Log($"  fixedUnscaledDeltaTime:   {Time.fixedUnscaledDeltaTime}");

        // Debug.Log($"  smoothDeltaTime:          {Time.smoothDeltaTime}");
        // Debug.Log($"  maximumDeltaTime:         {Time.maximumDeltaTime}");

        // Debug.Log($"  time:                     {Time.time}");
        // Debug.Log($"  fixedTime:                {Time.fixedTime}");
        // Debug.Log($"  unscaledTime:             {Time.unscaledTime}");
        // Debug.Log($"  fixedUnscaledTime:        {Time.fixedUnscaledTime}");

        // Debug.Log($"  maxModeSwitchRate_index:  {TimeWarp.fetch.maxModeSwitchRate_index}");
        // Debug.Log($"  maxPhysicsRate_index:     {TimeWarp.fetch.maxPhysicsRate_index}");
        // Debug.Log($"  current_rate_index:       {TimeWarp.fetch.current_rate_index}");
        // Debug.Log($"  mode:                     {TimeWarp.fetch.Mode}");
        // Debug.Log($"  tgt_rate:                 {TimeWarp.fetch.tgt_rate}");
        // Debug.Log($"  curr_rate:                {TimeWarp.CurrentRate}");
        // Debug.Log($"  deltaTime:                {TimeWarp.deltaTime}");
        // Debug.Log($"  fixedDeltaTime:           {TimeWarp.fixedDeltaTime}");
        // Debug.Log($"  MaxPhysicsRate:           {TimeWarp.MaxPhysicsRate}");

        // Constant Time.fixedDeltaTime keeps a consistent physics step.
        // Note we do not revert Time.timeScale here as that ensures that FixedUpdate is called at the correct frequency:
        // FixedUpdate interval = Time.fixedDeltaTime / Time.timeScale
        // Planetarium.TimeScale is used by the game's physics engine, so we want that to always be one
        Time.fixedDeltaTime = 0.02f;
        Planetarium.TimeScale = 1;
      }
    }

    void FixedUpdate()
    {
      // Debug.Log($"fixed update");
      // Debug.Log($"  timeScale:                {Time.timeScale}");
      // Debug.Log($"  planetarium:              {Planetarium.TimeScale}");

      // Debug.Log($"  deltaTime:                {Time.deltaTime}");
      // Debug.Log($"  fixedDeltaTime:           {Time.fixedDeltaTime}");
      // Debug.Log($"  unscaledDeltaTime:        {Time.unscaledDeltaTime}");
      // Debug.Log($"  fixedUnscaledDeltaTime:   {Time.fixedUnscaledDeltaTime}");

      // Debug.Log($"  smoothDeltaTime:          {Time.smoothDeltaTime}");
      // Debug.Log($"  maximumDeltaTime:         {Time.maximumDeltaTime}");

      // Debug.Log($"  time:                     {Time.time}");
      // Debug.Log($"  fixedTime:                {Time.fixedTime}");
      // Debug.Log($"  unscaledTime:             {Time.unscaledTime}");
      // Debug.Log($"  fixedUnscaledTime:        {Time.fixedUnscaledTime}");

      if (TimeWarp.fetch is TimeWarp warp)
      {
        // Debug.Log($"  maxModeSwitchRate_index:  {warp.maxModeSwitchRate_index}");
        // Debug.Log($"  maxPhysicsRate_index:     {warp.maxPhysicsRate_index}");
        // Debug.Log($"  current_rate_index:       {warp.current_rate_index}");
        // Debug.Log($"  mode:                     {warp.Mode}");
        // Debug.Log($"  tgt_rate:                 {warp.tgt_rate}");
        // Debug.Log($"  curr_rate:                {TimeWarp.CurrentRate}");
        // Debug.Log($"  deltaTime:                {TimeWarp.deltaTime}");
        // Debug.Log($"  fixedDeltaTime:           {TimeWarp.fixedDeltaTime}");
        // Debug.Log($"  MaxPhysicsRate:           {TimeWarp.MaxPhysicsRate}");

        warp.physicsWarpRates = new float[] { 1, 2, 4, 8 };

        // Because Planetarium.TimeScale is always 1, TimeWarp doesn't call updateRate when the target rate is 1
        // We conditionally update the rate just as TimeWarp FixedUpdate does in that case
        if (warp.Mode == TimeWarp.Modes.LOW && warp.tgt_rate == 1 && TimeWarp.CurrentRate != 1)
        {
          var last_rate = (float)field_last_rate.GetValue(warp);
          var LerpStartTime = (float)field_LerpStartTime.GetValue(warp);

          var curr_rate = Mathf.Lerp(last_rate, warp.tgt_rate, Time.time - LerpStartTime);
          field_curr_rate.SetValue(warp, curr_rate);

          method_updateRate.Invoke(warp, new object[] { curr_rate, true });
        }
      }
    }
  }
}
