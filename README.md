# Kitchen Chaos

Following along with [CodeMonkey's Unity tutorial](https://www.youtube.com/watch?v=AmGSEH7QcDg&ab_channel=CodeMonkey)

Major changes from the tutorial game:

* Implemented a greatly simplified stove recipe system that elegantly avoids unnecessary code duplication and state machines
* Player continues rotating towards target direction even after they stop moving
* Made gameplay more intuitive:
  * Plates can interact with container counters directly to get their item
  * Player can interact with plate counter directly to put whatever they are holding onto a plate
* Added dynamic fading sound effect to frying stove
* Added additional logic to generate items silently to avoid unnecessary event firing or sound effects
* Delivery counter is not implemented as a singleton to support multiple delivery counters at once
* Made options menu more robust
  * Modified user interface to be more readable and intuitive
  * Volume options properly affect all sounds, including those not managed by SoundManager (frying and footstep sounds)
  * Volume cycling is more consistent by using integers instead of floats, avoiding floating point errors
  * Options menu does not overlay the pause menu
  * Added a "Reset to Defaults" button
