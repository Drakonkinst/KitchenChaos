# Kitchen Chaos

Following along with [CodeMonkey's Unity tutorial](https://www.youtube.com/watch?v=AmGSEH7QcDg&ab_channel=CodeMonkey)

Major changes from the tutorial game:

* Implemented a greatly simplified stove recipe system that elegantly avoids unnecessary code duplication and state machines
* Player continues rotating towards target direction even after they stop moving
* Plates can interact with container counters directly to get their item
* Added dynamic fading sound effect to frying stove
* Added additional logic to generate items silently to avoid unnecessary event firing or sound effects
* Delivery counter is not implemented as a singleton to support multiple delivery counters at once
