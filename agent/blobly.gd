extends CharacterBody2D
'''
@export var speed: int = 100
@onready var animations = $Sprite2D/AnimationPlayer
var target_position = Vector2()
var click_position = Vector2()



func handleInput():
	var moveDirection = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
	velocity = moveDirection * speed

func updateAnimation():
	if velocity.length() == 0:
		animations.stop()
	else:
		var direction = "Down"
		if velocity.x < 0: direction = "Left"
		elif velocity.x > 0: direction = "Right"
		elif velocity.y < 0: direction = "Up"
		

func go_there(delta):
	target_position = (click_position - position).normalized()
	velocity = target_position * speed
	move_and_slide()

	

func _physics_process(delta):
	handleInput()
	updateAnimation()

	if Input.is_action_just_pressed("left_click"):
		click_position = get_global_mouse_position()
		
	if position.distance_to(click_position) > 15:
		go_there(delta)

	
'''
