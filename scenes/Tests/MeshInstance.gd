extends Spatial

var _material: ShaderMaterial;
var light: DirectionalLight;

func _ready():
	var mesh = get_node("MeshInstance")
	_material = mesh.get_active_material(0);
	light = get_node("DirectionalLight");
	set_physics_process(true)

func _physics_process(delta):
	light.rotation_degrees.x += 20 * delta;
#	$Camera.rotation_degrees.y += 10 * delta;
	var lightVec = light.global_transform.basis.z;
	print(lightVec)
	_material.set_shader_param("light_vector", lightVec);
