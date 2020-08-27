### Godot Sharp FPS

Investigating code/components required to implement a basic arcade FPS game using the Mono version of the Godot game engine.

#### Main points

* Streamlined way of managing game entity interactions such as finding/targetting each other, causing damage, giving items and entering/exiting the scene.
* How to build and arrange Godot prefab objects
* Core set of functions/utils for manipulating Godot objects.

#### Interaction messiness

Currently looking for a nice way to handle physical interactions between 'actors' in the game scene. For example causing damage, giving items, applying push forces.
Current usage of IActor or ITouchable interfaces are icky and required code duplication. IProvider interface feels okay, but health or inventory actions should be handled with a specialised health/inventory nodes that their parents subscribe to.

Attacker -> Is victim shape IHealthProvider? If so, cast and retrieve IHealth and call .Damage function.

