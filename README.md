# Kitchen Chaos

**Kitchen Chaos** is a small kitchen simulator game built in Unity.

I followed along with [CodeMonkey's 10-hour Unity tutorial](https://www.youtube.com/watch?v=AmGSEH7QcDg&ab_channel=CodeMonkey) to create this game. The original game by CodeMonkey is [published on Steam](https://store.steampowered.com/app/2275820/Kitchen_Chaos__Learn_Game_Development/), but my version includes additional features detailed below.

From the Steam page:

> The customers are hungry, it's up to you to fill them up!
>
> Pick up some ingredients, prepare them, put them on a plate and deliver them.
> Work with the various counters to prepare them.
> Pick up a full Cheese block, cut it into slices, then cook a Meat Patty (but don't let it burn!), pick them up on a Plate, add some Bread and you have a nice burger!
>
> Features:
>
> * 7 Ingredients to use
> * 3 Complete Recipes to create
> * 6 unique Kitchen Counters to interact with

## Changes From Tutorial Game

### Gameplay

* Added time bonuses for delivering recipes successfully and time penalties for delivering the wrong recipes, so the game runs longer the more efficient the player is.
* Added an additional delivery counter, patty container counter, and plate counter to the unused corner of the map
* Added several quality of life features to make gameplay more intuitive:
  * Interacting with a container counter while holding a plate puts the container's item directly onto the plate
  * Interating with a plate counter while holding a valid item puts that item on a plate
* Player visually rotates towards target direction even after they stop moving
* Music starts playing once the countdown ends, and stops when the game ends

### Technical

* Implemented a greatly simplified stove recipe system that avoids unnecessary code duplication and state machines
* Added dynamic fading in/out of stove frying audio
* Added additional logic to generate items silently to avoid unwanted sound effects
* Volume options properly affect all sounds, including frying and footstep sounds that are not managed by the main sound manager
* Made volume cycling more consistent by using integers instead of floats, avoiding floating point errors
* Implemented the delivery counter without using singletons to support multiple delivery counters at once

### User Interface

* Made options menu more robust
  * Rearranged user interface to be more readable and intuitive
  * Added a "Reset to Defaults" button
  * Options menu smoothly swaps between displaying controller and keyboard bindings based on which device is active instead of using additional UI
