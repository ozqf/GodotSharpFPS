
## Boss/Large enemies

Intention is to construct bosses out of a core body with turrets attached.

* Turrets need to be controlled by the boss core.
* Turrets independently destroyable for bonsues.

### Node layout

#### Boss

* Boss Actor
	* movement kinematic body
	* shootable 'area' mesh
	* turret mount node
		* child turret
	* turret mount node
		* child turret

#### Turret


* Base node (source of projectiles)
	* model if required (might be invisible)
	* collision area (if shootable)

On death turrets will deactivate their components and spawn a 'corpse' node which they jettison. If a boss phase involves regenerating the turret, it is reactivated.
