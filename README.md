## LosslessPhysics

This is an addon for [Kerbal Space Program](https://www.kerbalspaceprogram.com/) that changes the behavior of the "physics warp" from constant performance/lossy accuracy to low performance/constant accuracy.

By default, physics warp increases the $\Delta t$ of each physics step. This causes inaccuracies in the physics engine but maintains consistent performance. This addon changes that behavior to instead increase the rate of physics steps and keep the same $\Delta t$.

**Warning**: This can significantly strain the CPU as physics calculations are largely single-threaded.

Default physics:
- $\Delta t$ = 0.02s (game time), physics step every 0.02s (real time)

Base behavior:
- 2x: $\Delta t$ = 0.04s (game time), physics step every 0.02s (real time)
- 3x: $\Delta t$ = 0.06s (game time), physics step every 0.02s (real time)
- 4x: $\Delta t$ = 0.08s (game time), physics step every 0.02s (real time)

Modified behavior:
- 2x: $\Delta t$ = 0.02s (game time), physics step every 0.01s (real time)
- 4x: $\Delta t$ = 0.02s (game time), physics step every 0.005s (real time)
- 8x: $\Delta t$ = 0.02s (game time), physics step every 0.0025s (real time)
